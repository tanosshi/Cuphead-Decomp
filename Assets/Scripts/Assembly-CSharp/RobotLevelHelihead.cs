using System;
using System.Collections;
using UnityEngine;

public class RobotLevelHelihead : AbstractCollidableObject
{
	private enum state
	{
		first = 0,
		second = 1,
		dead = 2
	}

	[SerializeField]
	private float verticalMovementStrength;

	[SerializeField]
	private float horizontalMovementStrength;

	[SerializeField]
	private Transform spawnPoint;

	private GameObject pivotPoint;

	private bool introActive;

	private int coordinateIndex;

	private string[] screenHeights;

	private float speed;

	private float width;

	private float attackDelay;

	private bool offScreen;

	private int attackTypeIndex;

	private state current;

	private LevelProperties.Robot properties;

	private DamageDealer damageDealer;

	private DamageReceiver damageReceiver;

	[SerializeField]
	private GameObject bombBotPrefab;

	[SerializeField]
	private RobotLevelBlockade blockadeSegement;

	[SerializeField]
	private RobotLevelGem gem;

	public Action OnDeath;

	protected override void Awake()
	{
		damageDealer = DamageDealer.NewEnemy();
		damageReceiver = GetComponent<DamageReceiver>();
		damageReceiver.OnDamageTaken += OnDamageTaken;
		current = state.first;
		base.Awake();
	}

	protected override void Update()
	{
		if (damageDealer != null)
		{
			damageDealer.Update();
		}
		if (properties != null && properties.CurrentHealth <= 0f)
		{
			StopAllCoroutines();
		}
		base.Update();
	}

	protected void OnDamageTaken(DamageDealer.DamageInfo info)
	{
		properties.DealDamage(info.damage);
		if (properties.CurrentHealth <= 0f && current != state.dead)
		{
			current = state.dead;
			StartDeath();
		}
	}

	public void InitHeliHead(LevelProperties.Robot properties)
	{
		introActive = true;
		screenHeights = properties.CurrentState.heliHead.onScreenHeight.Split(',');
		coordinateIndex = UnityEngine.Random.Range(0, screenHeights.Length);
		attackTypeIndex = UnityEngine.Random.Range(0, properties.CurrentState.inventor.gemColourString.Split(',').Length);
		pivotPoint = new GameObject("pivotPoint");
		pivotPoint.transform.position = new Vector3(Level.Current.Right - Level.Current.Width / 4, Level.Current.Ground + Level.Current.Height / 2, 0f);
		speed = properties.CurrentState.heliHead.heliheadMovementSpeed;
		attackDelay = properties.CurrentState.heliHead.attackDelay;
		width = 300f;
		this.properties = properties;
		speed = properties.CurrentState.heliHead.heliheadMovementSpeed;
		base.transform.Rotate(Vector3.forward, 90f);
		base.transform.position = spawnPoint.position;
		StartCoroutine(horizontalMovement_cr());
		StartCoroutine(attack_cr());
		StartCoroutine(check_sound_cr());
	}

	private void SpinSFX()
	{
		AudioManager.Play("robot_headspin");
		emitAudioFromObject.Add("robot_headspin");
	}

	private IEnumerator check_sound_cr()
	{
		bool onscreen = false;
		while (current == state.first)
		{
			if (base.transform.position.x < (float)Level.Current.Right && base.transform.position.x > (float)Level.Current.Left)
			{
				if (!onscreen)
				{
					SpinSFX();
					onscreen = true;
				}
			}
			else if (onscreen)
			{
				AudioManager.Stop("robot_headspin");
				onscreen = false;
			}
			yield return null;
		}
	}

	private IEnumerator horizontalMovement_cr()
	{
		offScreen = false;
		yield return new WaitForEndOfFrame();
		base.transform.position += Vector3.left * GetComponent<SpriteRenderer>().bounds.size.x / 10f;
		while (true)
		{
			base.transform.position += Vector3.left * speed * CupheadTime.Delta;
			if (base.transform.position.x <= (float)Level.Current.Left - width)
			{
				GetComponent<BoxCollider2D>().enabled = false;
				offScreen = true;
				if (introActive)
				{
					introActive = false;
					base.animator.Play("Loop");
				}
				speed = 0f;
				base.transform.position = new Vector3(base.transform.position.x, Level.Current.Ground + int.Parse(screenHeights[coordinateIndex]), base.transform.position.z);
				base.transform.Rotate(Vector3.forward, 180f);
				coordinateIndex++;
				if (coordinateIndex >= screenHeights.Length)
				{
					coordinateIndex = 0;
				}
				yield return CupheadTime.WaitForSeconds(this, properties.CurrentState.heliHead.offScreenDelay);
				speed = -properties.CurrentState.heliHead.heliheadMovementSpeed;
				base.transform.position += Vector3.right * 50f;
				offScreen = false;
				GetComponent<BoxCollider2D>().enabled = true;
			}
			if (base.transform.position.x >= (float)Level.Current.Right + width)
			{
				GetComponent<BoxCollider2D>().enabled = false;
				offScreen = true;
				speed = 0f;
				base.transform.position = new Vector3(base.transform.position.x, Level.Current.Ground + int.Parse(screenHeights[coordinateIndex]), base.transform.position.z);
				base.transform.Rotate(Vector3.forward, 180f);
				base.transform.position += Vector3.left * 50f;
				coordinateIndex++;
				if (coordinateIndex >= screenHeights.Length)
				{
					coordinateIndex = 0;
				}
				yield return CupheadTime.WaitForSeconds(this, properties.CurrentState.heliHead.offScreenDelay);
				speed = properties.CurrentState.heliHead.heliheadMovementSpeed;
				offScreen = false;
				GetComponent<BoxCollider2D>().enabled = true;
			}
			yield return null;
		}
	}

	private IEnumerator inventorIntro_cr()
	{
		Vector3 end = new Vector3(pivotPoint.transform.position.x, -760f);
		Vector3 start = base.transform.position;
		float pct = 0f;
		while (pct < 1f)
		{
			base.transform.position = Vector3.Lerp(start, end, pct);
			pct += (float)CupheadTime.Delta;
			yield return null;
		}
		base.transform.position = end;
		StartCoroutine(stateEasing_cr());
	}

	private IEnumerator stateEasing_cr()
	{
		base.transform.rotation = Quaternion.identity;
		Vector3 start = base.transform.position;
		Vector3 end = new Vector3(pivotPoint.transform.position.x - 200f, pivotPoint.transform.position.y);
		float pct = 0f;
		while (pct < 1f)
		{
			base.transform.position = Vector3.Lerp(start, end, pct);
			pct += (float)CupheadTime.Delta;
			yield return null;
		}
		AudioManager.Stop("robot_headspin");
		base.animator.Play("Inventor Intro");
		StartCoroutine(verticalMovement_cr());
		speed *= 2f;
		yield return base.animator.WaitForAnimationToEnd(this, "End", true);
		yield return CupheadTime.WaitForSeconds(this, properties.CurrentState.inventor.initialAttackDelay);
		float normalizedTime = base.animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1f;
		float delay = 0f;
		delay = ((!(base.animator.GetCurrentAnimatorStateInfo(0).length / normalizedTime < 1f)) ? (delay + (1f - normalizedTime)) : (delay - normalizedTime));
		yield return CupheadTime.WaitForSeconds(this, delay);
		StartCoroutine(blockade_cr());
		if (properties.CurrentState.inventor.gemColourString.Split(',')[attackTypeIndex] == "R")
		{
			base.animator.Play("Red Gem Attack");
			yield return base.animator.WaitForAnimationToEnd(this, "Red Gem Attack");
			base.animator.Play("RedGemFXIntro", 2);
			gem.InitFinalStage(this, properties, false);
		}
		else
		{
			base.animator.Play("Blue Gem Attack");
			yield return base.animator.WaitForAnimationToEnd(this, "Blue Gem Attack");
			base.animator.Play("BlueGemFXIntro", 2);
			gem.InitFinalStage(this, properties, true);
		}
		speed /= 2f;
		StartCoroutine(easeValues_cr());
	}

	private IEnumerator verticalMovement_cr()
	{
		speed = 1f;
		float time = 0f;
		while (true)
		{
			time += (float)CupheadTime.Delta * 2f;
			base.transform.position = pivotPoint.transform.position + Vector3.left * 200f + Vector3.up * Mathf.Sin(time * speed) * verticalMovementStrength + Vector3.right * Mathf.Sin(time * (2f * speed)) * horizontalMovementStrength;
			yield return null;
		}
	}

	private IEnumerator easeValues_cr(bool easeIn = true)
	{
		if (easeIn)
		{
			speed = properties.CurrentState.inventor.inventorIdleSpeedMultiplier;
		}
		StartCoroutine(easeStrength_cr(easeIn));
		if (easeIn)
		{
			yield return CupheadTime.WaitForSeconds(this, properties.CurrentState.inventor.attackDuration.RandomFloat());
			if (properties.CurrentState.inventor.gemColourString.Split(',')[attackTypeIndex] == "R")
			{
				base.animator.SetTrigger("RedGemAttack");
			}
			else
			{
				base.animator.SetTrigger("BlueGemAttack");
			}
			gem.OnAttackEnd();
			attackTypeIndex++;
			if (attackTypeIndex >= properties.CurrentState.inventor.gemColourString.Split(',').Length)
			{
				attackTypeIndex = 0;
			}
			yield return CupheadTime.WaitForSeconds(this, properties.CurrentState.inventor.attackDelay.RandomFloat());
			StartCoroutine(easeValues_cr(false));
			yield break;
		}
		float normalizedTime = base.animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1f;
		float delay = 0f;
		delay = ((!(base.animator.GetCurrentAnimatorStateInfo(0).length / normalizedTime < 1f)) ? (delay + (1f - normalizedTime)) : (delay - normalizedTime));
		yield return CupheadTime.WaitForSeconds(this, delay);
		if (properties.CurrentState.inventor.gemColourString.Split(',')[attackTypeIndex] == "R")
		{
			base.animator.Play("Red Gem Attack");
			yield return base.animator.WaitForAnimationToEnd(this, "Red Gem Attack");
			base.animator.Play("RedGemFXIntro", 2);
			gem.InitFinalStage(this, properties, false);
		}
		else
		{
			base.animator.Play("Blue Gem Attack");
			yield return base.animator.WaitForAnimationToEnd(this, "Blue Gem Attack");
			base.animator.Play("BlueGemFXIntro", 2);
			gem.InitFinalStage(this, properties, true);
		}
		StartCoroutine(easeValues_cr());
	}

	public void OnGemEnd()
	{
		GemEndSFX();
		base.animator.SetTrigger("StopGemFX");
	}

	private void GemStartSFX()
	{
		AudioManager.Play("robot_diamond_attack_start");
		emitAudioFromObject.Add("robot_diamond_attack_start");
		AudioManager.PlayLoop("robot_diamond_attack_loop");
		emitAudioFromObject.Add("robot_diamond_attack_loop");
	}

	private void GemEndSFX()
	{
		AudioManager.Stop("robot_diamond_attack_loop");
		AudioManager.Play("robot_diamond_attack_end");
		emitAudioFromObject.Add("robot_diamond_attack_end");
	}

	private void IntroSFX()
	{
		AudioManager.Play("robot_head_transform");
		emitAudioFromObject.Add("robot_head_transform");
	}

	private IEnumerator easeSpeed_cr()
	{
		float pct = 0f;
		while (pct < 1f)
		{
			speed = 1f + (properties.CurrentState.inventor.inventorIdleSpeedMultiplier - 1f) * pct;
			pct += 10f * (float)CupheadTime.Delta;
			yield return null;
		}
	}

	private IEnumerator easeStrength_cr(bool easeIn)
	{
		float pct = 0f;
		float hStrength = horizontalMovementStrength;
		float vStrength = verticalMovementStrength;
		while (pct < 1f)
		{
			if (easeIn)
			{
				horizontalMovementStrength = hStrength + (25f - hStrength) * pct;
				verticalMovementStrength = vStrength + (160f - vStrength) * pct;
			}
			else
			{
				horizontalMovementStrength = hStrength + (0f - hStrength) * pct;
				verticalMovementStrength = vStrength + (20f - vStrength) * pct;
			}
			pct += 0.25f * (float)CupheadTime.Delta;
			yield return null;
		}
	}

	private IEnumerator attack_cr()
	{
		while (true)
		{
			if (offScreen)
			{
				attackDelay -= CupheadTime.Delta;
				if (attackDelay <= 0f)
				{
					SpawnBombBot();
					attackDelay = 100f;
				}
			}
			else
			{
				attackDelay = properties.CurrentState.heliHead.attackDelay;
			}
			yield return null;
		}
	}

	private IEnumerator blockade_cr()
	{
		float groupSize = properties.CurrentState.inventor.blockadeGroupSize;
		int dir = 1;
		while (true)
		{
			for (int i = 0; (float)i < groupSize; i++)
			{
				if (dir > 0)
				{
					RobotLevelBlockade robotLevelBlockade = blockadeSegement.Create(new Vector3(Level.Current.Right, Level.Current.Ceiling, 0f), dir);
					robotLevelBlockade.InitBlockade(dir, properties.CurrentState.inventor.blockadeHorizontalSpeed, properties.CurrentState.inventor.blockadeVerticalSpeed);
				}
				else
				{
					RobotLevelBlockade robotLevelBlockade2 = blockadeSegement.Create(new Vector3(Level.Current.Right, Level.Current.Ground, 0f), dir);
					robotLevelBlockade2.InitBlockade(dir, properties.CurrentState.inventor.blockadeHorizontalSpeed, properties.CurrentState.inventor.blockadeVerticalSpeed);
				}
				dir *= -1;
				yield return CupheadTime.WaitForSeconds(this, properties.CurrentState.inventor.blockadeIndividualDelay);
				yield return null;
			}
			yield return CupheadTime.WaitForSeconds(this, properties.CurrentState.inventor.blockadeGroupDelay);
			yield return null;
		}
	}

	private void SpawnBombBot()
	{
		HomingProjectile homingProjectile = bombBotPrefab.GetComponent<RobotLevelHatchBombBot>().Create(base.transform.GetChild(0).transform.position, (int)base.transform.eulerAngles.z + 90, properties.CurrentState.bombBot.initialBombMovementSpeed, properties.CurrentState.bombBot.bombHomingSpeed, properties.CurrentState.bombBot.bombRotationSpeed, properties.CurrentState.bombBot.bombLifeTime, 4f, PlayerManager.GetNext());
		homingProjectile.GetComponent<RobotLevelHatchBombBot>().InitBombBot(properties.CurrentState.bombBot);
	}

	public void ChangeState()
	{
		current = state.second;
		StopAllCoroutines();
		StartCoroutine(inventorIntro_cr());
	}

	protected override void OnDestroy()
	{
		StopAllCoroutines();
		base.OnDestroy();
		AudioManager.Stop("robot_diamond_attack_loop");
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionPlayer(hit, phase);
		damageDealer.DealDamage(hit);
	}

	private void StartDeath()
	{
		StopAllCoroutines();
		if (OnDeath != null)
		{
			OnDeath();
		}
		GetComponent<Collider2D>().enabled = false;
		base.animator.SetTrigger("OnDeath");
		AudioManager.Stop("robot_diamond_attack_loop");
	}
}
