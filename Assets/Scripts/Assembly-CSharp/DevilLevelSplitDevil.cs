using System.Collections;
using UnityEngine;

public class DevilLevelSplitDevil : LevelProperties.Devil.Entity
{
	[SerializeField]
	private int facingDirection;

	[SerializeField]
	private DevilLevelSplitDevilWall wallPrefab;

	[SerializeField]
	private DevilLevelSplitDevilProjectile projectilePrefab;

	[SerializeField]
	private DevilLevelSplitDevil otherDevil;

	[SerializeField]
	private Transform projectileRoot;

	private int patternIndex;

	private LevelProperties.Devil.Pattern pattern;

	private DamageDealer damageDealer;

	private DamageReceiver damageReceiver;

	public bool IsSilhouetted { get; private set; }

	public Color Color { get; private set; }

	protected override void Awake()
	{
		base.Awake();
		Color = GetComponent<SpriteRenderer>().color;
		damageDealer = DamageDealer.NewEnemy();
		damageReceiver = GetComponent<DamageReceiver>();
		damageReceiver.OnDamageTaken += OnDamageTaken;
	}

	protected override void Update()
	{
		base.Update();
		LevelPlayerController levelPlayerController = PlayerManager.GetPlayer(PlayerId.PlayerOne) as LevelPlayerController;
		LevelPlayerController levelPlayerController2 = PlayerManager.GetPlayer(PlayerId.PlayerTwo) as LevelPlayerController;
		IsSilhouetted = (levelPlayerController == null || levelPlayerController.transform.localScale.x == (float)facingDirection) && (levelPlayerController2 == null || levelPlayerController2.motor.transform.localScale.x == (float)facingDirection);
		if (GetComponent<SpriteRenderer>().color == Color || GetComponent<SpriteRenderer>().color == Color.black)
		{
			GetComponent<SpriteRenderer>().color = ((!IsSilhouetted) ? Color : Color.black);
		}
		GetComponent<SpriteRenderer>().sortingLayerName = ((!IsSilhouetted) ? "Enemies" : "Default");
		GetComponent<Collider2D>().enabled = !IsSilhouetted;
		if (damageDealer != null)
		{
			damageDealer.Update();
		}
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionPlayer(hit, phase);
		if (damageDealer != null)
		{
			damageDealer.DealDamage(hit);
		}
	}

	public void StartPatterns()
	{
		patternIndex = Random.Range(0, base.properties.CurrentState.patterns.Length);
		nextPattern();
	}

	private void nextPattern()
	{
		patternIndex = (patternIndex + 1) % base.properties.CurrentState.patterns.Length;
		pattern = base.properties.CurrentState.patterns[patternIndex];
		while (pattern == LevelProperties.Devil.Pattern.SplitDevilWallAttack && otherDevil.pattern == LevelProperties.Devil.Pattern.SplitDevilWallAttack)
		{
			patternIndex = (patternIndex + 1) % base.properties.CurrentState.patterns.Length;
			pattern = base.properties.CurrentState.patterns[patternIndex];
		}
		switch (base.properties.CurrentState.patterns[patternIndex])
		{
		case LevelProperties.Devil.Pattern.SplitDevilProjectileAttack:
			StartCoroutine(projectile_cr());
			break;
		case LevelProperties.Devil.Pattern.SplitDevilWallAttack:
			StartCoroutine(wall_cr());
			break;
		}
	}

	private IEnumerator wall_cr()
	{
		LevelProperties.Devil.SplitDevilWall p = base.properties.CurrentState.splitDevilWall;
		DevilLevelSplitDevilWall wall = wallPrefab.Create(p.xRange.min * (float)facingDirection, p.speed.RandomFloat() * (float)facingDirection, p.xRange.max - p.xRange.min, this);
		base.animator.SetBool("Wall", true);
		while (wall != null)
		{
			yield return null;
		}
		base.animator.SetBool("Wall", false);
		yield return CupheadTime.WaitForSeconds(this, p.hesitateAfterAttack.RandomFloat());
		nextPattern();
	}

	private IEnumerator projectile_cr()
	{
		LevelProperties.Devil.SplitDevilProjectiles p = base.properties.CurrentState.splitDevilProjectiles;
		AbstractPlayerController player = PlayerManager.GetNext();
		float delayBetweenProjectiles = p.delayBetweenProjectiles.RandomFloat();
		for (int i = 0; i < p.numProjectiles.RandomInt(); i++)
		{
			if (player == null || player.IsDead)
			{
				player = PlayerManager.GetNext();
			}
			float rotation = MathUtils.DirectionToAngle(player.center - projectileRoot.position);
			projectilePrefab.Create(projectileRoot.position, rotation, p.projectileSpeed, this);
			base.animator.SetTrigger("Shoot");
			yield return CupheadTime.WaitForSeconds(this, delayBetweenProjectiles);
		}
		yield return CupheadTime.WaitForSeconds(this, p.hesitateAfterAttack.RandomFloat());
		nextPattern();
	}

	private void OnDamageTaken(DamageDealer.DamageInfo info)
	{
		base.properties.DealDamage(info.damage);
	}
}
