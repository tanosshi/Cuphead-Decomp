using System.Collections;
using UnityEngine;

public class DicePalaceDominoLevelDomino : LevelProperties.DicePalaceDomino.Entity
{
	public enum State
	{
		Idle = 0,
		Boomerang = 1,
		BouncyBall = 2
	}

	[SerializeField]
	private Transform bouncySpawnpoint;

	[SerializeField]
	private Transform birdSpawnpoint;

	[SerializeField]
	private DicePalaceDominoLevelBouncyBall bouncyBallPrefab;

	[SerializeField]
	private DicePalaceDominoLevelBoomerang boomerangPrefab;

	[SerializeField]
	private DicePalaceDominoLevelFloor floor;

	private int happyAttackAngleIndex;

	private int happyAttackDirectionIndex;

	private string[] happyAttackBallTypePattern;

	private int happyAttackBallTypeIndex;

	private float happyAttackDelay;

	private bool isAttacking;

	private int sadAttackBoomerangTypeIndex;

	private float sadAttackDelay;

	private Color pink;

	private DamageDealer damageDealer;

	private DamageReceiver damageReceiver;

	public State state { get; private set; }

	protected override void Awake()
	{
		damageDealer = DamageDealer.NewEnemy();
		damageReceiver = GetComponent<DamageReceiver>();
		damageReceiver.OnDamageTaken += OnDamageTaken;
		base.Awake();
	}

	protected override void Update()
	{
		base.Update();
		if (damageDealer != null)
		{
			damageDealer.Update();
		}
	}

	private void OnDamageTaken(DamageDealer.DamageInfo info)
	{
		base.properties.DealDamage(info.damage);
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		damageDealer.DealDamage(hit);
		base.OnCollisionPlayer(hit, phase);
	}

	public override void LevelInit(LevelProperties.DicePalaceDomino properties)
	{
		Level.Current.OnIntroEvent += OnIntroEnd;
		Level.Current.OnWinEvent += OnDeath;
		base.transform.parent.GetComponent<DicePalaceDominoLevelDominoSwing>().InitSwing(properties);
		happyAttackAngleIndex = Random.Range(0, properties.CurrentState.bouncyBall.angleString.Split(',').Length);
		happyAttackDirectionIndex = Random.Range(0, properties.CurrentState.bouncyBall.upDownString.Split(',').Length);
		happyAttackBallTypePattern = properties.CurrentState.bouncyBall.projectileTypeString.Split(',');
		happyAttackBallTypeIndex = Random.Range(0, happyAttackBallTypePattern.Length);
		happyAttackDelay = properties.CurrentState.bouncyBall.attackDelayRange.RandomFloat();
		sadAttackBoomerangTypeIndex = Random.Range(0, properties.CurrentState.boomerang.boomerangTypeString.Split(',').Length);
		sadAttackDelay = properties.CurrentState.boomerang.attackDelayRange.RandomFloat();
		pink = new Color(255f, 120f, 229f, 255f);
		floor.InitFloor(properties);
		base.LevelInit(properties);
		StartCoroutine(intro_cr());
	}

	private void OnIntroEnd()
	{
		base.animator.enabled = true;
		floor.StartSpawningTiles();
	}

	private IEnumerator intro_cr()
	{
		AudioManager.PlayLoop("dice_palace_domino_intro_start_loop");
		emitAudioFromObject.Add("dice_palace_domino_intro_start_loop");
		yield return CupheadTime.WaitForSeconds(this, 2f);
		base.animator.SetTrigger("Continue");
		yield return base.animator.WaitForAnimationToStart(this, "Intro");
		AudioManager.Stop("dice_palace_domino_intro_start_loop");
		AudioManager.Play("dice_palace_domino_intro");
		emitAudioFromObject.Add("dice_palace_domino_intro");
		state = State.Idle;
	}

	public void OnBouncyBall()
	{
		StartCoroutine(bouncyBall_cr());
	}

	private IEnumerator bouncyBall_cr()
	{
		isAttacking = true;
		state = State.BouncyBall;
		yield return CupheadTime.WaitForSeconds(this, base.properties.CurrentState.bouncyBall.initialAttackDelay);
		base.animator.SetTrigger("OnProjectile");
		yield return base.animator.WaitForAnimationToStart(this, "Projectile_Attack");
		AudioManager.Play("dice_palace_domino_projectile_attack");
		emitAudioFromObject.Add("dice_palace_domino_projectile_attack");
		yield return base.animator.WaitForAnimationToEnd(this, "Projectile_Attack");
		yield return CupheadTime.WaitForSeconds(this, happyAttackDelay);
		state = State.Idle;
	}

	public void SpawnBall()
	{
		StartCoroutine(spawn_ball_cr());
	}

	private IEnumerator spawn_ball_cr()
	{
		float angle = int.Parse(base.properties.CurrentState.bouncyBall.angleString.Split(',')[happyAttackAngleIndex]);
		if (base.properties.CurrentState.bouncyBall.upDownString.Split(',')[happyAttackDirectionIndex][0] == 'U')
		{
			angle = 0f - angle;
		}
		Vector3 direction = Vector3.left;
		direction = Quaternion.AngleAxis(angle, Vector3.forward) * direction;
		AbstractProjectile proj = bouncyBallPrefab.Create(bouncySpawnpoint.position);
		proj.SetParryable(happyAttackBallTypePattern[happyAttackBallTypeIndex][0] == 'P');
		proj.GetComponent<DicePalaceDominoLevelBouncyBall>().InitBouncyBall(base.properties.CurrentState.bouncyBall.bulletSpeed, base.properties, floor, direction);
		happyAttackAngleIndex++;
		if (happyAttackAngleIndex >= base.properties.CurrentState.bouncyBall.angleString.Split(',').Length)
		{
			happyAttackAngleIndex = 0;
		}
		happyAttackDirectionIndex++;
		if (happyAttackDirectionIndex >= base.properties.CurrentState.bouncyBall.upDownString.Split(',').Length)
		{
			happyAttackDirectionIndex = 0;
		}
		happyAttackBallTypeIndex++;
		if (happyAttackBallTypeIndex >= happyAttackBallTypePattern.Length)
		{
			happyAttackBallTypeIndex = 0;
		}
		isAttacking = false;
		yield return null;
	}

	public void OnBoomerang()
	{
		StartCoroutine(boomerang_cr());
	}

	private IEnumerator boomerang_cr()
	{
		isAttacking = true;
		state = State.Boomerang;
		yield return CupheadTime.WaitForSeconds(this, base.properties.CurrentState.boomerang.initialAttackDelay);
		base.animator.SetTrigger("OnBird");
		yield return base.animator.WaitForAnimationToEnd(this, "Bird_Attack");
		yield return CupheadTime.WaitForSeconds(this, sadAttackDelay);
		state = State.Idle;
	}

	public void SpawnBoomerang()
	{
		StartCoroutine(spawn_boomerang_cr());
	}

	private IEnumerator spawn_boomerang_cr()
	{
		LevelProperties.DicePalaceDomino.Boomerang p = base.properties.CurrentState.boomerang;
		if (base.properties.CurrentState.boomerang.boomerangTypeString.Split(',')[sadAttackBoomerangTypeIndex][0] == 'R')
		{
			DicePalaceDominoLevelBoomerang proj = boomerangPrefab.Create(birdSpawnpoint.position, p.boomerangSpeed, p.health, p);
		}
		else
		{
			DicePalaceDominoLevelBoomerang proj = boomerangPrefab.Create(birdSpawnpoint.position, p.boomerangSpeed, p.health, p);
			proj.GetComponent<SpriteRenderer>().color = Color.magenta;
			proj.SetParryable(true);
		}
		sadAttackBoomerangTypeIndex++;
		if (sadAttackBoomerangTypeIndex >= base.properties.CurrentState.boomerang.boomerangTypeString.Split(',').Length)
		{
			sadAttackBoomerangTypeIndex = 0;
		}
		isAttacking = false;
		yield return null;
	}

	private void OnDeath()
	{
		AudioManager.PlayLoop("dice_palace_domino_death_start_loop");
		emitAudioFromObject.Add("dice_palace_domino_death_start_loop");
		base.animator.SetTrigger("OnDeath");
	}

	private void EndDeathLoop()
	{
		AudioManager.Stop("dice_palace_domino_death_start_loop");
	}

	private void DeathSFX()
	{
		AudioManager.Play("dice_palace_domino_death");
		emitAudioFromObject.Add("dice_palace_domino_death");
	}

	private void BirdAttackSFX()
	{
		AudioManager.Play("dice_palace_domino_bird_attack");
		emitAudioFromObject.Add("dice_palace_domino_bird_attack");
	}

	private void SwingForwardSFX()
	{
		AudioManager.Play("swing_forward");
		emitAudioFromObject.Add("swing_forward");
	}

	private void SwingBackSFX()
	{
		AudioManager.Play("swing_back");
		emitAudioFromObject.Add("swing_back");
	}
}
