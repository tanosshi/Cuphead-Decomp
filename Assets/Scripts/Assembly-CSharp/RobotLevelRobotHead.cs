using System.Collections;
using UnityEngine;

public class RobotLevelRobotHead : RobotLevelRobotBodyPart
{
	private RobotLevelHoseLaser laser;

	private AbstractPlayerController currentPlayer;

	private float angle;

	private int attackStringGroup;

	private int attackStringIndex;

	[SerializeField]
	private BasicProjectile nutProjectile;

	public override void InitBodyPart(RobotLevelRobot parent, LevelProperties.Robot properties, int primaryHP = 1, int secondaryHP = 1, float attackDelayMinus = 0f)
	{
		GetComponent<BoxCollider2D>().enabled = true;
		currentPlayer = PlayerManager.GetNext();
		primaryAttackDelay = properties.CurrentState.hose.attackDelayRange.RandomFloat();
		secondaryAttackDelay = properties.CurrentState.cannon.attackDelay;
		attackStringGroup = Random.Range(0, properties.CurrentState.cannon.shootString.Length);
		attackStringIndex = Random.Range(0, properties.CurrentState.cannon.shootString[attackStringGroup].Split(',').Length);
		parent.OnDeathEvent += OnPrimaryDeath;
		primaryHP = properties.CurrentState.hose.health;
		attackDelayMinus = properties.CurrentState.hose.delayMinus;
		base.InitBodyPart(parent, properties, primaryHP, secondaryHP, attackDelayMinus);
		StartPrimary();
	}

	protected override void OnPrimaryAttack()
	{
		if (currentPlayer == null || currentPlayer.IsDead)
		{
			currentPlayer = PlayerManager.GetNext();
		}
		if (current != state.primary)
		{
			return;
		}
		if (currentPlayer.id == PlayerId.PlayerOne)
		{
			if (PlayerManager.GetPlayer(PlayerId.PlayerTwo) != null)
			{
				currentPlayer = PlayerManager.GetPlayer(PlayerId.PlayerTwo);
			}
		}
		else
		{
			currentPlayer = PlayerManager.GetPlayer(PlayerId.PlayerOne);
		}
		StartCoroutine(warningLaser_cr());
		base.OnPrimaryAttack();
	}

	private IEnumerator warningLaser_cr()
	{
		if (current == state.primary)
		{
			yield return CupheadTime.WaitForSeconds(this, properties.CurrentState.hose.warningDuration);
			if (current == state.primary)
			{
				if (currentPlayer == null || currentPlayer.IsDead)
				{
					currentPlayer = PlayerManager.GetNext();
				}
				Vector3 dir = (currentPlayer.center - base.transform.position).normalized;
				angle = Vector3.Angle(Vector3.up, dir);
				if (dir.x > 0f)
				{
					angle *= -1f;
				}
				angle = Mathf.Clamp(angle, properties.CurrentState.hose.aimAngleParameter.min, properties.CurrentState.hose.aimAngleParameter.max);
				yield return null;
				laser = primary.GetComponent<RobotLevelHoseLaser>().Create(base.transform.position, angle - 90f, this);
				laser.animator.SetTrigger("OnWarning");
				AudioManager.Play("robot_raygun_charge");
				emitAudioFromObject.Add("robot_raygun_charge");
			}
			yield return CupheadTime.WaitForSeconds(this, properties.CurrentState.hose.warningDuration);
			if (current == state.primary)
			{
				laser.animator.SetTrigger("OnAttack");
				AudioManager.Play("robot_raygun_shoot");
				emitAudioFromObject.Add("robot_raygun_shoot");
				yield return null;
			}
			else
			{
				AudioManager.Stop("robot_raygun_charge");
			}
			yield return null;
		}
		if (current == state.primary)
		{
			StartCoroutine(attackLaser_cr());
		}
	}

	private IEnumerator attackLaser_cr()
	{
		yield return CupheadTime.WaitForSeconds(this, properties.CurrentState.hose.beamDuration);
		AudioManager.Stop("robot_raygun_charge");
		if (laser != null)
		{
			Object.Destroy(laser.gameObject);
			isAttacking = false;
		}
		if ((float)Random.Range(0, 100) <= 25f && !AudioManager.CheckIfPlaying("robot_vocals_laugh"))
		{
			AudioManager.Play("robot_vocals_laugh");
			emitAudioFromObject.Add("robot_vocals_laugh");
		}
	}

	protected override void OnPrimaryDeath()
	{
		if (current != state.secondary && currentHealth[0] <= 0f)
		{
			parent.animator.SetBool("HeadStageTwoTransition", true);
			StartCoroutine(endLasers_cr());
		}
		base.OnPrimaryDeath();
	}

	private IEnumerator endLasers_cr()
	{
		yield return parent.animator.WaitForAnimationToEnd(parent, "Idle", 2, true);
		AudioManager.Play("robot_head_antennae_destroyed");
		GetComponent<BoxCollider2D>().enabled = false;
		StopCoroutine(warningLaser_cr());
		StopCoroutine(attackLaser_cr());
		ExitCurrentAttacks();
		StartSecondary();
		deathEffect.Create(base.transform.position);
		AudioManager.Play("robot_upper_chest_port_destroyed");
		emitAudioFromObject.Add("robot_upper_chest_port_destroyed");
	}

	private IEnumerator startSecondary_cr()
	{
		StartSecondary();
		yield return null;
	}

	protected override void OnSecondaryAttack()
	{
		secondaryAttackDelay = properties.CurrentState.cannon.attackDelay;
		string attackString = properties.CurrentState.cannon.shootString[attackStringGroup].Split(',')[attackStringIndex];
		attackStringIndex++;
		if (attackStringIndex >= properties.CurrentState.cannon.shootString[attackStringGroup].Split(',').Length - 1)
		{
			secondaryAttackDelay = properties.CurrentState.cannon.attackDelay;
			attackStringIndex = 0;
			attackStringGroup++;
			if (attackStringGroup >= properties.CurrentState.cannon.shootString.Length - 1)
			{
				attackStringGroup = 0;
				secondaryAttackDelay = properties.CurrentState.cannon.attackDelayRange.RandomFloat();
			}
		}
		parent.animator.SetTrigger("HeadAttack");
		StartCoroutine(spreadShot_cr(attackString));
		base.OnSecondaryAttack();
	}

	private IEnumerator spreadShot_cr(string attackString)
	{
		yield return parent.animator.WaitForAnimationToEnd(this, "Stage Two Idle", 2, true);
		cannonSpreadShot(attackString);
	}

	private void cannonSpreadShot(string attackString)
	{
		int result = 0;
		int.TryParse(attackString.Substring(1), out result);
		result--;
		string[] array = properties.CurrentState.cannon.spreadVariableGroups[result].Split(',');
		float result2 = 0f;
		int result3 = 0;
		MinMax minMax = new MinMax(0f, 0f);
		string[] array2 = array;
		foreach (string text in array2)
		{
			if (text[0] == 'S')
			{
				float.TryParse(text.Substring(1), out result2);
				continue;
			}
			if (text[0] == 'N')
			{
				int.TryParse(text.Substring(1), out result3);
				continue;
			}
			string[] array3 = text.Split('-');
			float.TryParse(array3[0], out minMax.min);
			float.TryParse(array3[1], out minMax.max);
		}
		AudioManager.Play("robot_head_shoot");
		emitAudioFromObject.Add("robot_head_shoot");
		for (int j = 0; j < result3; j++)
		{
			float floatAt = minMax.GetFloatAt((float)j / ((float)result3 - 1f));
			if (j % 2 == 0)
			{
				BasicProjectile component = secondary.GetComponent<BasicProjectile>();
				component.Create(base.transform.position, floatAt, result2);
			}
			else
			{
				nutProjectile.Create(base.transform.position, floatAt, result2);
			}
		}
	}

	protected override void ExitCurrentAttacks()
	{
		if (laser != null)
		{
			Object.Destroy(laser.gameObject);
		}
		base.ExitCurrentAttacks();
	}
}
