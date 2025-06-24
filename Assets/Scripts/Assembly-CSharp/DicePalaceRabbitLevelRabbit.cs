using System.Collections;
using UnityEngine;

public class DicePalaceRabbitLevelRabbit : LevelProperties.DicePalaceRabbit.Entity
{
	public enum State
	{
		Idle = 0,
		MagicWand = 1,
		MagicParry = 2
	}

	private const float OrbAppearTime = 2f;

	[SerializeField]
	private AbstractProjectile orbPrefab;

	[SerializeField]
	private DicePalaceRabbitLevelMagic magicPrefab;

	[SerializeField]
	private FlowerLevelPlatform platform1;

	[SerializeField]
	private FlowerLevelPlatform platform2;

	[SerializeField]
	private Effect explosionPrefab;

	private bool attacking;

	private bool isDying;

	private int playerOneCircleIndex;

	private int playerTwoCircleIndex;

	private DamageDealer damageDealer;

	private DamageReceiver damageReceiver;

	private bool isMagicParryTop;

	private int parryCurrentIndex;

	private GameObject currentCenterPoint;

	private bool AttackSFXPlaying;

	private bool StickTwirlActive;

	public State state { get; private set; }

	protected override void Awake()
	{
		base.Awake();
		damageDealer = DamageDealer.NewEnemy();
		damageReceiver = GetComponent<DamageReceiver>();
		damageReceiver.OnDamageTaken += OnDamageTaken;
		StartCoroutine(idle_voice_sfx_cr());
		StartCoroutine(idle_sfx_cr());
	}

	private void OnDamageTaken(DamageDealer.DamageInfo info)
	{
		base.properties.DealDamage(info.damage);
	}

	protected override void Update()
	{
		base.Update();
		if (damageDealer != null)
		{
			damageDealer.Update();
		}
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionPlayer(hit, phase);
		damageDealer.DealDamage(hit);
	}

	public override void LevelInit(LevelProperties.DicePalaceRabbit properties)
	{
		base.LevelInit(properties);
		attacking = false;
		playerOneCircleIndex = (playerTwoCircleIndex = Random.Range(0, properties.CurrentState.magicWand.safeZoneString.Split(',').Length));
		Vector2 zero = Vector2.zero;
		Vector2 zero2 = Vector2.zero;
		zero.x = int.Parse(properties.CurrentState.general.platformOnePosition.Split(',')[0]);
		zero.y = int.Parse(properties.CurrentState.general.platformOnePosition.Split(',')[1]);
		platform1.transform.position = zero;
		platform1.YPositionUp = zero.y;
		zero2.x = int.Parse(properties.CurrentState.general.platformTwoPosition.Split(',')[0]);
		zero2.y = int.Parse(properties.CurrentState.general.platformTwoPosition.Split(',')[1]);
		platform2.transform.position = zero2;
		platform2.YPositionUp = zero2.y;
		isMagicParryTop = Rand.Bool();
		state = State.Idle;
		Level.Current.OnWinEvent += Death;
		StartCoroutine(intro_cr());
	}

	private IEnumerator intro_cr()
	{
		yield return CupheadTime.WaitForSeconds(this, 1f);
		base.animator.SetTrigger("Continue");
		yield return base.animator.WaitForAnimationToEnd(this, "Intro_Continue");
		base.animator.Play("Off");
		yield return null;
	}

	private IEnumerator idle_voice_sfx_cr()
	{
		MinMax delay = new MinMax(1f, 4f);
		while (true)
		{
			if (!base.animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
			{
				yield return null;
				continue;
			}
			yield return CupheadTime.WaitForSeconds(this, delay);
			AudioManager.Play("dice_palace_rabbit_idle_vox");
			emitAudioFromObject.Add("dice_palace_rabbit_idle_vox");
			yield return null;
		}
	}

	private IEnumerator idle_sfx_cr()
	{
		bool loopingIdle = false;
		while (true)
		{
			if (base.animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
			{
				if (loopingIdle)
				{
				}
			}
			else if (!loopingIdle)
			{
			}
			yield return null;
		}
	}

	public void OnMagicWand()
	{
		StartCoroutine(magicwand_cr());
	}

	private IEnumerator magicwand_cr()
	{
		attacking = true;
		state = State.MagicWand;
		yield return CupheadTime.WaitForSeconds(this, base.properties.CurrentState.magicWand.initialAttackDelay);
		AbstractPlayerController player = PlayerManager.GetNext();
		base.animator.SetTrigger("OnAttack");
		StartCoroutine(orbs_cr(player.id, int.Parse(base.properties.CurrentState.magicWand.safeZoneString.Split(',')[playerOneCircleIndex])));
		playerOneCircleIndex++;
		if (playerOneCircleIndex >= base.properties.CurrentState.magicWand.safeZoneString.Split(',').Length)
		{
			playerOneCircleIndex = 0;
		}
		yield return CupheadTime.WaitForSeconds(this, base.properties.CurrentState.magicWand.attackDelayRange.RandomFloat());
		attacking = false;
		base.animator.SetTrigger("OnAttackEnd");
		yield return CupheadTime.WaitForSeconds(this, base.properties.CurrentState.magicWand.hesitate);
		state = State.Idle;
	}

	private IEnumerator orbs_cr(PlayerId target, int safeZone)
	{
		GameObject centerPoint = new GameObject();
		AbstractPlayerController player = PlayerManager.GetPlayer(target);
		centerPoint.transform.position = player.center;
		currentCenterPoint = centerPoint;
		Vector3 dir = Vector3.up;
		float dist = base.properties.CurrentState.magicWand.circleDiameter / 2f;
		safeZone = GetSafeZone(safeZone);
		Transform[] orbs = new Transform[7];
		int orbsIndex = 0;
		float initialRotation = Random.Range(0, 350);
		for (int i = 0; i < 8; i++)
		{
			if (i != safeZone)
			{
				DicePalaceRabbitLevelOrb dicePalaceRabbitLevelOrb = orbPrefab.Create(player.center + dir * dist, 0f, Vector2.one) as DicePalaceRabbitLevelOrb;
				dicePalaceRabbitLevelOrb.transform.parent = centerPoint.transform;
				dicePalaceRabbitLevelOrb.transform.Rotate(Vector3.forward, 0f - initialRotation);
				dicePalaceRabbitLevelOrb.SetAsGold(i % 2 == 1);
				Color color = dicePalaceRabbitLevelOrb.GetComponent<SpriteRenderer>().color;
				color.a = 0.2f;
				dicePalaceRabbitLevelOrb.GetComponent<SpriteRenderer>().color = color;
				orbs[orbsIndex] = dicePalaceRabbitLevelOrb.transform;
				orbsIndex++;
			}
			dir = Quaternion.AngleAxis(-45f, Vector3.forward) * dir;
		}
		centerPoint.transform.Rotate(Vector3.forward, initialRotation);
		while (attacking)
		{
			if (player != null && !player.IsDead)
			{
				centerPoint.transform.position = player.center;
			}
			centerPoint.transform.Rotate(Vector3.forward * CupheadTime.FixedDelta, (0f - base.properties.CurrentState.magicWand.spinningSpeed) * CupheadTime.FixedDelta);
			for (int j = 0; j < orbs.Length; j++)
			{
				SpriteRenderer component = orbs[j].GetComponent<SpriteRenderer>();
				Color color2 = component.color;
				color2.a += (float)CupheadTime.Delta / 2f;
				component.color = color2;
				if (color2.a >= 1f)
				{
					orbs[j].GetComponent<Collider2D>().enabled = true;
				}
				orbs[j].Rotate(Vector3.forward * CupheadTime.FixedDelta, base.properties.CurrentState.magicWand.spinningSpeed * CupheadTime.FixedDelta);
			}
			yield return new WaitForFixedUpdate();
		}
		while (Vector3.Angle(Vector3.up, centerPoint.transform.up) > 5f)
		{
			if (player != null && !player.IsDead)
			{
				centerPoint.transform.position = player.center;
			}
			centerPoint.transform.Rotate(Vector3.forward * CupheadTime.FixedDelta, (0f - base.properties.CurrentState.magicWand.spinningSpeed) * CupheadTime.FixedDelta);
			for (int k = 0; k < orbs.Length; k++)
			{
				orbs[k].Rotate(Vector3.forward * CupheadTime.FixedDelta, base.properties.CurrentState.magicWand.spinningSpeed * CupheadTime.FixedDelta);
			}
			yield return new WaitForFixedUpdate();
		}
		centerPoint.transform.up = Vector3.up;
		StartCoroutine(collapse_cr(centerPoint));
	}

	private IEnumerator collapse_cr(GameObject centerPoint)
	{
		float dist = base.properties.CurrentState.magicWand.circleDiameter / 2f;
		float explodeDist = base.properties.CurrentState.magicWand.circleDiameter * 0.1f;
		while (dist >= explodeDist)
		{
			for (int i = 0; i < 7; i++)
			{
				Vector3 vector = (centerPoint.transform.GetChild(i).position - centerPoint.transform.position).normalized * dist;
				centerPoint.transform.GetChild(i).position = centerPoint.transform.position + vector;
			}
			dist -= base.properties.CurrentState.magicWand.bulletSpeed * (float)CupheadTime.Delta;
			yield return null;
		}
		AudioManager.Play("projectile_explo");
		explosionPrefab.Create(centerPoint.transform.position);
		currentCenterPoint = null;
		Object.Destroy(centerPoint);
	}

	private int GetSafeZone(int index)
	{
		int result = 0;
		switch (index)
		{
		case 1:
			result = 5;
			break;
		case 2:
			result = 4;
			break;
		case 3:
			result = 3;
			break;
		case 4:
			result = 6;
			break;
		case 6:
			result = 2;
			break;
		case 7:
			result = 7;
			break;
		case 8:
			result = 0;
			break;
		case 9:
			result = 1;
			break;
		}
		return result;
	}

	private IEnumerator kill_orbs_cr()
	{
		float t = 0f;
		float time = 1f;
		float speed = 2500f;
		float[] angles = new float[7];
		for (int i = 0; i < 7; i++)
		{
			currentCenterPoint.transform.GetChild(i).GetComponent<Collider2D>().enabled = false;
			angles[i] = Random.Range(0, 360);
		}
		while (t < time)
		{
			t += (float)CupheadTime.Delta;
			for (int j = 0; j < 7; j++)
			{
				currentCenterPoint.transform.GetChild(j).position += (Vector3)MathUtils.AngleToDirection(angles[j]) * speed * CupheadTime.FixedDelta;
			}
			yield return null;
		}
		Object.Destroy(currentCenterPoint);
		yield return null;
	}

	public void OnMagicParry()
	{
		StartCoroutine(magicparry_cr());
	}

	private IEnumerator magicparry_cr()
	{
		attacking = true;
		state = State.MagicParry;
		yield return CupheadTime.WaitForSeconds(this, base.properties.CurrentState.magicParry.initialAttackDelay);
		base.animator.SetTrigger("OnAttack");
		string[] positionsSplits = base.properties.CurrentState.magicParry.magicPositions.Split('-');
		DicePalaceRabbitLevelMagic[] magicOrbs = new DicePalaceRabbitLevelMagic[positionsSplits.Length];
		string[] parryPattern = base.properties.CurrentState.magicParry.pinkString.Split(',');
		string[] parryIndexes = parryPattern[parryCurrentIndex].Split('-');
		float yOffset = base.properties.CurrentState.magicParry.yOffset;
		float posY = ((!isMagicParryTop) ? (-360f + yOffset) : (360f - yOffset));
		int suit = 0;
		for (int i = 0; i < magicOrbs.Length; i++)
		{
			float result = 0f;
			float.TryParse(positionsSplits[i], out result);
			result += -640f;
			magicOrbs[i] = (DicePalaceRabbitLevelMagic)magicPrefab.Create(new Vector3(result, posY));
			magicOrbs[i].IsOffset(i % 2 == 1);
			magicOrbs[i].AppearTime = base.properties.CurrentState.magicParry.attackDelayRange;
			bool flag = false;
			for (int j = 0; j < parryIndexes.Length; j++)
			{
				int result2 = 0;
				if (int.TryParse(parryIndexes[j], out result2) && result2 - 1 == i)
				{
					magicOrbs[i].SetParryable(true);
					flag = true;
				}
			}
			if (!flag)
			{
				magicOrbs[i].SetSuit(suit);
				suit = (suit + 1) % 3;
			}
		}
		yield return CupheadTime.WaitForSeconds(this, base.properties.CurrentState.magicParry.attackDelayRange);
		for (int k = 0; k < magicOrbs.Length; k++)
		{
			magicOrbs[k].ActivateOrb();
			magicOrbs[k].Move(posY, isMagicParryTop, base.properties.CurrentState.magicParry.speed);
		}
		base.animator.SetTrigger("OnAttackEnd");
		yield return CupheadTime.WaitForSeconds(this, base.properties.CurrentState.magicParry.hesitate);
		attacking = false;
		isMagicParryTop = !isMagicParryTop;
		parryCurrentIndex = (parryCurrentIndex + 1) % parryPattern.Length;
		state = State.Idle;
	}

	private void AttackSFX()
	{
		StartCoroutine(attack_sfx_cr());
	}

	private IEnumerator attack_sfx_cr()
	{
		yield return base.animator.WaitForAnimationToStart(this, "Attack");
		yield return base.animator.WaitForAnimationToStart(this, "Attack_End");
		yield return null;
	}

	private void Death()
	{
		AudioManager.Stop("dice_palace_rabbit_idle_loop");
		AudioManager.Stop("dice_palace_rabbit_attack_loop");
		SFX_StickTwirlStop();
		base.animator.SetTrigger("Death");
		StopAllCoroutines();
		if (currentCenterPoint != null)
		{
			StartCoroutine(kill_orbs_cr());
		}
		OnDestroy();
	}

	private void SFX_IntroContinue()
	{
		AudioManager.Play("intro_continue");
		emitAudioFromObject.Add("intro_continue");
	}

	private void SFX_Death()
	{
		AudioManager.Play("dice_palace_rabbit_death");
		emitAudioFromObject.Add("dice_palace_rabbit_death");
	}

	private void SFX_AttackStart()
	{
		AudioManager.Play("dice_palace_rabbit_attack_start");
		emitAudioFromObject.Add("dice_palace_rabbit_attack_start");
	}

	private void SFX_Attack()
	{
		if (!AttackSFXPlaying)
		{
			AudioManager.PlayLoop("dice_palace_rabbit_attack_loop");
			emitAudioFromObject.Add("dice_palace_rabbit_attack_loop");
			AttackSFXPlaying = true;
		}
	}

	private void SFX_AttackEnd()
	{
		AudioManager.Stop("dice_palace_rabbit_attack_loop");
		AudioManager.Play("dice_palace_rabbit_attack_end");
		emitAudioFromObject.Add("dice_palace_rabbit_attack_end");
		AttackSFXPlaying = false;
	}

	private void SFX_IdleRock()
	{
		AudioManager.Play("idle_rock");
		emitAudioFromObject.Add("idle_rock");
	}

	private void SFX_StickTwirl()
	{
		if (!StickTwirlActive)
		{
			StickTwirlActive = true;
			AudioManager.PlayLoop("stick_twirl");
			emitAudioFromObject.Add("stick_twirl");
		}
	}

	private void SFX_StickTwirlStop()
	{
		StickTwirlActive = false;
		AudioManager.Stop("stick_twirl");
	}
}
