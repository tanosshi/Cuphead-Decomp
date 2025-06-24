using System.Collections;
using UnityEngine;

public class BatLevelBat : LevelProperties.Bat.Entity
{
	public enum Direction
	{
		Right = 0,
		Left = 1
	}

	public enum State
	{
		Idle = 0,
		Bouncer = 1,
		Lightning = 2,
		Phase2 = 3
	}

	[SerializeField]
	private Transform bouncerRoot;

	[SerializeField]
	private Transform coffinRoot;

	[SerializeField]
	private Transform pentagramRoot;

	[SerializeField]
	private BatLevelBouncer bouncerPrefab;

	[SerializeField]
	private BatLevelGoblin goblinPrefab;

	[SerializeField]
	private BatLevelMiniBat minibatPrefab;

	[SerializeField]
	private BatLevelLightning lightningPrefab;

	private BatLevelLightning lightning;

	[SerializeField]
	private BatLevelPentagram pentagramPrefab;

	[SerializeField]
	private BasicProjectile wolfProjectile;

	[SerializeField]
	private BatLevelCross crossPrefab;

	private BatLevelCross cross;

	[SerializeField]
	private BatLevelHomingSoul soulPrefab;

	private Direction direction = Direction.Left;

	private DamageDealer damageDealer;

	private bool waitForTurn;

	private bool slowDown;

	private bool onRight;

	private bool inMovingPhase;

	private bool moving;

	private string[] anglePattern;

	private int angleIndex;

	private float speed;

	private float originalSpeed;

	private Vector3 startPosition;

	private Coroutine pattern;

	public State state { get; private set; }

	public override void LevelInit(LevelProperties.Bat properties)
	{
		base.LevelInit(properties);
		GetComponent<DamageReceiver>().OnDamageTaken += OnDamageTaken;
		speed = properties.CurrentState.movement.movementSpeed;
		originalSpeed = speed;
		inMovingPhase = true;
		moving = true;
		startPosition = base.transform.position;
		startPosition.y = properties.CurrentState.movement.startPosY;
		base.transform.position = startPosition;
		damageDealer = new DamageDealer(1f, 0.2f, true, false, false);
		damageDealer.SetDirection(DamageDealer.Direction.Left, base.transform);
		StartCoroutine(intro_cr());
	}

	private IEnumerator intro_cr()
	{
		LevelProperties.Bat.State p = base.properties.CurrentState;
		anglePattern = p.batBouncer.bounceAngleString.GetRandom().Split(',');
		angleIndex = Random.Range(0, anglePattern.Length);
		yield return CupheadTime.WaitForSeconds(this, 5f);
		base.animator.SetTrigger("OnIntro");
		StartCoroutine(bat_movement_cr());
		state = State.Idle;
	}

	public void Die()
	{
		base.animator.SetTrigger("OnDeath");
	}

	protected override void Update()
	{
		base.Update();
		damageDealer.Update();
		if (state != State.Phase2)
		{
			VaryingSpeed();
		}
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionPlayer(hit, phase);
		damageDealer.DealDamage(hit);
	}

	private void OnDamageTaken(DamageDealer.DamageInfo info)
	{
		base.properties.DealDamage(info.damage);
	}

	private void OnTurnAnimComplete()
	{
		waitForTurn = false;
		base.transform.SetScale(base.transform.localScale.x * -1f);
	}

	private IEnumerator bat_movement_cr()
	{
		float offset = 200f;
		float stopDist = 100f;
		LevelProperties.Bat.Movement p = base.properties.CurrentState.movement;
		Vector3 pos = base.transform.position;
		while (true)
		{
			if (direction == Direction.Left)
			{
				while (base.transform.position.x > -640f + offset)
				{
					if (moving)
					{
						float f = -640f + offset - base.transform.position.x;
						f = Mathf.Abs(f);
						pos.x = Mathf.MoveTowards(base.transform.position.x, -640f + offset, speed * (float)CupheadTime.Delta);
						if (f < stopDist)
						{
							slowDown = true;
						}
						base.transform.position = pos;
					}
					yield return null;
				}
				base.animator.SetTrigger("OnTurn");
				if (!inMovingPhase)
				{
					onRight = false;
					StartCoroutine(phase_2_handler_cr());
					yield break;
				}
				direction = Direction.Right;
				yield return null;
			}
			else if (direction == Direction.Right)
			{
				while (base.transform.position.x < 640f - offset)
				{
					if (moving)
					{
						float f2 = 640f - offset - base.transform.position.x;
						f2 = Mathf.Abs(f2);
						pos.x = Mathf.MoveTowards(base.transform.position.x, 640f - offset, speed * (float)CupheadTime.Delta);
						if (f2 < stopDist)
						{
							slowDown = true;
						}
						base.transform.position = pos;
					}
					yield return null;
				}
				base.animator.SetTrigger("OnTurn");
				if (!inMovingPhase)
				{
					break;
				}
				direction = Direction.Left;
				yield return null;
			}
			yield return null;
		}
		onRight = true;
		StartCoroutine(phase_2_handler_cr());
	}

	private void VaryingSpeed()
	{
		float num = 10f;
		if (slowDown)
		{
			if (speed <= 50f)
			{
				slowDown = false;
			}
			else
			{
				speed -= num;
			}
		}
		else if (speed < originalSpeed)
		{
			speed += num;
		}
	}

	public void StartBouncer()
	{
		if (pattern != null)
		{
			StopCoroutine(pattern);
		}
		pattern = StartCoroutine(bouncer_cr());
	}

	private void SpawnBouncer()
	{
		LevelProperties.Bat.BatBouncer batBouncer = base.properties.CurrentState.batBouncer;
		float result = 0f;
		float.TryParse(anglePattern[angleIndex], out result);
		result = ((direction != Direction.Right) ? result : (result + 90f));
		BatLevelBouncer batLevelBouncer = Object.Instantiate(bouncerPrefab);
		batLevelBouncer.Init(base.properties.CurrentState.batBouncer, bouncerRoot.position, result);
		angleIndex = (angleIndex + 1) % anglePattern.Length;
	}

	private IEnumerator bouncer_cr()
	{
		LevelProperties.Bat.BatBouncer p = base.properties.CurrentState.batBouncer;
		state = State.Bouncer;
		moving = false;
		yield return CupheadTime.WaitForSeconds(this, p.stopDelay);
		SpawnBouncer();
		moving = true;
		yield return CupheadTime.WaitForSeconds(this, p.hesitate);
		state = State.Idle;
		yield return null;
	}

	public void StartGoblin()
	{
		StartCoroutine(goblin_cr());
	}

	private IEnumerator goblin_cr()
	{
		LevelProperties.Bat.Goblins p = base.properties.CurrentState.goblins;
		string[] delayPattern = p.appearDelayString.GetRandom().Split(',');
		string[] entrancePattern = p.entranceString.GetRandom().Split(',');
		int delayIndex = Random.Range(0, delayPattern.Length);
		int entranceIndex = Random.Range(0, entrancePattern.Length);
		int counter = 0;
		float delay = 0f;
		float startX = 0f;
		float pickShooter = p.shooterOccuranceRange.RandomInt();
		float startY = (float)Level.Current.Ground + 100f;
		bool isShooter = false;
		while (true)
		{
			float.TryParse(delayPattern[delayIndex], out delay);
			yield return CupheadTime.WaitForSeconds(this, delay);
			if (entrancePattern[entranceIndex][0] == 'R')
			{
				startX = 640f;
				Vector2 startPos = new Vector2(startX, startY);
				SpawnGoblin(false, startPos, isShooter);
			}
			else if (entrancePattern[entranceIndex][0] == 'L')
			{
				startX = -640f;
				Vector2 startPos = new Vector2(startX, startY);
				SpawnGoblin(true, startPos, isShooter);
			}
			isShooter = false;
			counter++;
			if ((float)counter == pickShooter)
			{
				isShooter = true;
				counter = 0;
			}
			entranceIndex = (entranceIndex + 1) % entrancePattern.Length;
			yield return null;
		}
	}

	private void SpawnGoblin(bool leftSide, Vector2 startPos, bool isShooter)
	{
		LevelProperties.Bat.Goblins goblins = base.properties.CurrentState.goblins;
		BatLevelGoblin batLevelGoblin = Object.Instantiate(goblinPrefab);
		batLevelGoblin.Init(goblins, startPos, leftSide, isShooter, goblins.HP);
	}

	public void StartLightning()
	{
		if (pattern != null)
		{
			StopCoroutine(pattern);
		}
		pattern = StartCoroutine(lightning_cr());
	}

	private void SpawnCloud(Vector2 startPos)
	{
		lightning = Object.Instantiate(lightningPrefab);
		lightning.Init(base.properties.CurrentState.batLightning, startPos);
	}

	private IEnumerator lightning_cr()
	{
		state = State.Lightning;
		moving = false;
		LevelProperties.Bat.BatLightning p = base.properties.CurrentState.batLightning;
		string[] attackDelayString = p.initialAttackDelayString.GetRandom().Split(',');
		string[] offsetString = p.centerOffset.GetRandom().Split(',');
		int offsetIndex = 0;
		float offset = 0f;
		Vector2 pos = Vector2.zero;
		pos.y = p.cloudHeight;
		for (int i = 0; (float)i < p.cloudCount; i++)
		{
			float.TryParse(offsetString[offsetIndex], out offset);
			pos.x = p.cloudDistance * (float)i + offset - (float)(Level.Current.Right / 2);
			SpawnCloud(pos);
			offsetIndex %= offsetString.Length;
		}
		while (lightning != null)
		{
			yield return null;
		}
		moving = true;
		yield return CupheadTime.WaitForSeconds(this, p.hesitate);
		state = State.Idle;
		yield return null;
	}

	public void StartPhase2()
	{
		inMovingPhase = false;
	}

	private IEnumerator phase_2_handler_cr()
	{
		float yPos = base.transform.position.y - 100f;
		state = State.Phase2;
		speed = originalSpeed;
		while (base.transform.position.y != yPos)
		{
			Vector3 pos = base.transform.position;
			pos.y = Mathf.MoveTowards(base.transform.position.y, yPos, speed * (float)CupheadTime.Delta);
			base.transform.position = pos;
			yield return null;
		}
		StartMiniBats();
		StartPentagram();
		StartCross();
		yield return null;
	}

	public void StartMiniBats()
	{
		StartCoroutine(mini_bats_cr());
	}

	private void SpawnMiniBat(float angle)
	{
		LevelProperties.Bat.MiniBats miniBats = base.properties.CurrentState.miniBats;
		float num = 0f;
		float num2 = 0f;
		num2 = ((!onRight) ? angle : (0f - angle));
		num = ((!onRight) ? miniBats.speedX : (0f - miniBats.speedX));
		minibatPrefab.Create(coffinRoot.position, num2, num, miniBats.speedY, miniBats.yMinMax, miniBats.HP, miniBats);
	}

	private IEnumerator mini_bats_cr()
	{
		LevelProperties.Bat.MiniBats p = base.properties.CurrentState.miniBats;
		string[] angleString = p.batAngleString.GetRandom().Split(',');
		int angleIndex = Random.Range(0, angleString.Length);
		float angle = 0f;
		while (base.properties.CurrentState.stateName == LevelProperties.Bat.States.Coffin)
		{
			float.TryParse(angleString[angleIndex], out angle);
			SpawnMiniBat(angle);
			yield return CupheadTime.WaitForSeconds(this, p.delay);
			angleIndex = (angleIndex + 1) % angleString.Length;
			yield return null;
		}
		yield return null;
	}

	public void StartPentagram()
	{
		StartCoroutine(pentagram_cr());
	}

	private void SpawnPentagram()
	{
		AbstractPlayerController next = PlayerManager.GetNext();
		Vector3 position = pentagramRoot.position;
		position.y = (float)Level.Current.Ground - 10f;
		BatLevelPentagram batLevelPentagram = Object.Instantiate(pentagramPrefab);
		batLevelPentagram.Init(position, base.properties.CurrentState.pentagrams, next, onRight);
	}

	private IEnumerator pentagram_cr()
	{
		LevelProperties.Bat.Pentagrams p = base.properties.CurrentState.pentagrams;
		string[] delayString = p.pentagramDelayString.GetRandom().Split(',');
		int delayIndex = Random.Range(0, delayString.Length);
		float delay = 0f;
		while (base.properties.CurrentState.stateName == LevelProperties.Bat.States.Coffin)
		{
			float.TryParse(delayString[delayIndex], out delay);
			yield return CupheadTime.WaitForSeconds(this, delay);
			SpawnPentagram();
			delayIndex %= delayString.Length;
			yield return null;
		}
		yield return null;
	}

	public void StartCross()
	{
		StartCoroutine(cross_cr());
	}

	private void SpawnCross(int count)
	{
		AbstractPlayerController next = PlayerManager.GetNext();
		cross = Object.Instantiate(crossPrefab);
		cross.Init(bouncerRoot.position, base.properties.CurrentState.crossToss, count, next);
	}

	private IEnumerator cross_cr()
	{
		LevelProperties.Bat.CrossToss p = base.properties.CurrentState.crossToss;
		string[] delayString = p.crossDelayString.GetRandom().Split(',');
		string[] countString = p.attackCount.GetRandom().Split(',');
		int delayIndex = Random.Range(0, delayString.Length);
		int countIndex = Random.Range(0, countString.Length);
		float delay = 0f;
		int count = 0;
		while (base.properties.CurrentState.stateName == LevelProperties.Bat.States.Coffin)
		{
			if (cross == null)
			{
				float.TryParse(delayString[delayIndex], out delay);
				int.TryParse(countString[countIndex], out count);
				yield return CupheadTime.WaitForSeconds(this, delay);
				SpawnCross(count);
			}
			delayIndex %= delayString.Length;
			count %= countString.Length;
			yield return null;
		}
		yield return null;
	}

	public void StartPhase3()
	{
		StopAllCoroutines();
		StartCoroutine(shoot_cr());
		StartCoroutine(soul_cr());
	}

	private IEnumerator shoot_cr()
	{
		LevelProperties.Bat.WolfFire p = base.properties.CurrentState.wolfFire;
		AbstractPlayerController player = PlayerManager.GetNext();
		float counter = 0f;
		while (base.properties.CurrentState.stateName == LevelProperties.Bat.States.Wolf)
		{
			yield return CupheadTime.WaitForSeconds(this, p.bulletDelay);
			ShootBullet(p.bulletSpeed, player);
			counter += 1f;
			if (counter >= p.bulletAimCount)
			{
				player = PlayerManager.GetNext();
				counter = 0f;
			}
		}
	}

	private void ShootBullet(float speed, AbstractPlayerController player)
	{
		float x = player.transform.position.x - bouncerRoot.position.x;
		float y = player.transform.position.y - bouncerRoot.position.y;
		float rotation = Mathf.Atan2(y, x) * 57.29578f;
		wolfProjectile.Create(bouncerRoot.position, rotation, speed);
	}

	private void SpawnSoul()
	{
		LevelProperties.Bat.WolfSoul wolfSoul = base.properties.CurrentState.wolfSoul;
		AbstractPlayerController next = PlayerManager.GetNext();
		BatLevelHomingSoul batLevelHomingSoul = Object.Instantiate(soulPrefab);
		batLevelHomingSoul.Init(bouncerRoot.position, next, wolfSoul);
	}

	private IEnumerator soul_cr()
	{
		LevelProperties.Bat.WolfSoul p = base.properties.CurrentState.wolfSoul;
		SpawnSoul();
		yield return null;
	}
}
