using System;
using System.Collections;
using UnityEngine;

public class BaronessLevelWaffle : BaronessLevelMiniBossBase
{
	public enum State
	{
		Enter = 0,
		Move = 1,
		Attack = 2,
		Dying = 3
	}

	[Serializable]
	public class WafflePieces
	{
		public Transform wafflepiece;

		public Animator waffleFX;

		public Vector3 direction;

		public float distanceFromCenter;

		public DamageDealer damageDealer;
	}

	private static float pauseValue;

	[SerializeField]
	private Effect explosion;

	[SerializeField]
	private Effect explosionReverse;

	[SerializeField]
	private WafflePieces[] diagonalPieces;

	[SerializeField]
	private WafflePieces[] straightPieces;

	[SerializeField]
	private Transform mouth;

	private LevelProperties.Baroness.Waffle properties;

	private AbstractPlayerController player;

	private Transform pivotPoint;

	private DamageDealer damageDealer;

	private DamageReceiver damageReceiver;

	private float health;

	private float speed;

	private float angle;

	private float loopSize = 200f;

	private float lerpTime = 300f;

	private bool switchedOn;

	private bool pathA;

	private bool check;

	private bool onBottom;

	private bool diagFirst;

	private bool isDead;

	private bool pivotMovingLeft;

	private bool isHitPaused;

	private Vector3 startPos;

	private Vector3 originalPivotPos;

	public State state { get; private set; }

	protected override void Awake()
	{
		base.Awake();
		float num = UnityEngine.Random.Range(0, 2);
		pathA = num == 0f;
		check = true;
		isDead = false;
		isDying = false;
		damageDealer = DamageDealer.NewEnemy();
		damageReceiver = GetComponent<DamageReceiver>();
		damageReceiver.OnDamageTaken += OnDamageTaken;
		mouth.GetComponent<DamageReceiver>().OnDamageTaken += OnDamageTaken;
		GetComponent<Collider2D>().enabled = true;
		for (int i = 0; i < diagonalPieces.Length; i++)
		{
			diagonalPieces[i].wafflepiece.GetComponent<DamageReceiver>().OnDamageTaken += OnDamageTaken;
			diagonalPieces[i].wafflepiece.GetComponent<Collider2D>().enabled = false;
			diagonalPieces[i].wafflepiece.GetComponent<CollisionChild>().OnPlayerCollision += OnCollisionPlayer;
		}
		for (int j = 0; j < straightPieces.Length; j++)
		{
			straightPieces[j].wafflepiece.GetComponent<DamageReceiver>().OnDamageTaken += OnDamageTaken;
			straightPieces[j].wafflepiece.GetComponent<Collider2D>().enabled = false;
			straightPieces[j].wafflepiece.GetComponent<CollisionChild>().OnPlayerCollision += OnCollisionPlayer;
		}
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionPlayer(hit, phase);
		damageDealer.DealDamage(hit);
	}

	public void Init(LevelProperties.Baroness.Waffle properties, AbstractPlayerController player, Vector2 pos, Transform pivot, float speed, float health)
	{
		this.properties = properties;
		this.player = player;
		this.speed = speed;
		this.health = health;
		base.transform.position = pos;
		pivotPoint = pivot;
		state = State.Enter;
		StartCoroutine(enter_cr());
		StartCoroutine(switchLayer_cr());
	}

	protected override void Update()
	{
		base.Update();
		if (damageDealer != null)
		{
			damageDealer.Update();
		}
	}

	private IEnumerator switchLayer_cr()
	{
		yield return CupheadTime.WaitForSeconds(this, 3f);
		base.gameObject.GetComponent<SpriteRenderer>().sortingLayerName = SpriteLayer.Enemies.ToString();
		base.gameObject.GetComponent<SpriteRenderer>().sortingOrder = 2;
	}

	protected override void OnDamageTaken(DamageDealer.DamageInfo info)
	{
		base.OnDamageTaken(info);
		health -= info.damage;
		if (health < 0f && state == State.Move)
		{
			isDead = true;
			StartCoroutine(death_cr());
		}
	}

	private IEnumerator enter_cr()
	{
		YieldInstruction wait = new WaitForFixedUpdate();
		originalPivotPos = pivotPoint.transform.position;
		if (pathA)
		{
			startPos = pivotPoint.position + Vector3.right * loopSize;
			angle = -(float)Math.PI / 2f;
		}
		else
		{
			startPos = pivotPoint.position + Vector3.down * loopSize;
			angle = (float)Math.PI;
			speed = 0f - speed;
		}
		while (base.transform.position != startPos)
		{
			base.transform.position = Vector3.MoveTowards(base.transform.position, startPos, properties.movementSpeed * 300f * CupheadTime.FixedDelta);
			yield return wait;
		}
		StartCircle();
		yield return null;
	}

	private void StartCircle()
	{
		state = State.Move;
		StartCoroutine(circle_cr());
		StartCoroutine(check_attack_cr());
	}

	private IEnumerator circle_cr()
	{
		YieldInstruction wait = new WaitForFixedUpdate();
		while (!isDead)
		{
			if (state == State.Move)
			{
				MovePivot();
				PathMovement();
				CheckIfTurn();
			}
			yield return wait;
		}
	}

	private void MovePivot()
	{
		Vector3 position = pivotPoint.transform.position;
		float pivotPointMoveAmount = properties.pivotPointMoveAmount;
		if (pivotMovingLeft)
		{
			position.x = Mathf.MoveTowards(pivotPoint.transform.position.x, originalPivotPos.x - pivotPointMoveAmount, properties.XAxisSpeed * CupheadTime.FixedDelta * hitPauseCoefficient());
			pivotMovingLeft = ((pivotPoint.transform.position.x != originalPivotPos.x - pivotPointMoveAmount) ? true : false);
		}
		else
		{
			position.x = Mathf.MoveTowards(pivotPoint.transform.position.x, originalPivotPos.x + pivotPointMoveAmount, properties.XAxisSpeed * CupheadTime.FixedDelta * hitPauseCoefficient());
			pivotMovingLeft = ((pivotPoint.transform.position.x == originalPivotPos.x + pivotPointMoveAmount) ? true : false);
		}
		pivotPoint.transform.position = position;
	}

	private void PathMovement()
	{
		angle += speed * CupheadTime.FixedDelta * hitPauseCoefficient();
		Vector3 vector = new Vector3((0f - Mathf.Sin(angle)) * loopSize, 0f, 0f);
		Vector3 vector2 = new Vector3(0f, Mathf.Cos(angle) * loopSize, 0f);
		base.transform.position = pivotPoint.position;
		base.transform.position += vector + vector2;
	}

	private void CheckIfTurn()
	{
		if (!check)
		{
			return;
		}
		if (base.transform.position.y < pivotPoint.position.y)
		{
			if (!onBottom)
			{
				StartCoroutine(turn_cr());
				check = false;
			}
			onBottom = true;
		}
		else
		{
			if (onBottom)
			{
				StartCoroutine(turn_cr());
				check = false;
			}
			onBottom = false;
		}
	}

	private IEnumerator turn_cr()
	{
		base.animator.SetBool("Turn", true);
		yield return base.animator.WaitForAnimationToEnd(this, "Waffle_Turn");
		base.animator.SetBool("Turn", false);
		check = true;
		yield return null;
	}

	private void Turn()
	{
		base.transform.SetScale(0f - base.transform.localScale.x, 1f, 1f);
	}

	private IEnumerator check_attack_cr()
	{
		if (!isDead)
		{
			yield return CupheadTime.WaitForSeconds(this, properties.attackDelayRange.RandomFloat());
			StartCoroutine(attack_cr());
			state = State.Attack;
			while (state == State.Attack)
			{
				yield return null;
			}
		}
	}

	private IEnumerator attack_cr()
	{
		if (!isDead)
		{
			base.animator.Play("Waffle_Tuck_Start");
			GetComponent<Collider2D>().enabled = false;
			float randomValue = UnityEngine.Random.Range(0, 2);
			diagFirst = randomValue == 0f;
			yield return CupheadTime.WaitForSeconds(this, properties.anticipation);
			base.animator.SetTrigger("Continue");
			StartCoroutine(waffle_pieces((!diagFirst) ? straightPieces : diagonalPieces, true));
			yield return CupheadTime.WaitForSeconds(this, properties.explodeTwoDuration);
			StartCoroutine(waffle_pieces((!diagFirst) ? diagonalPieces : straightPieces, false));
		}
	}

	private void hitPause(int i)
	{
		if (diagonalPieces[i].wafflepiece.GetComponent<DamageReceiver>().IsHitPaused || straightPieces[i].wafflepiece.GetComponent<DamageReceiver>().IsHitPaused)
		{
			pauseValue = 0f;
		}
		else
		{
			pauseValue = 1f;
		}
	}

	private IEnumerator waffle_pieces(WafflePieces[] pieces, bool isFirst)
	{
		float t = 0f;
		float explodeTime = properties.explodeSpeed;
		float returnTime = properties.explodeReturnSpeed;
		YieldInstruction wait = new WaitForFixedUpdate();
		while (!switchedOn)
		{
			yield return null;
		}
		foreach (WafflePieces wafflePieces in pieces)
		{
			wafflePieces.wafflepiece.GetComponent<Collider2D>().enabled = true;
			wafflePieces.waffleFX.Play("Trail", 0, UnityEngine.Random.Range(0f, 0.6f));
		}
		if (isFirst)
		{
			explosion.Create(new Vector2(mouth.position.x, mouth.position.y - 20f));
		}
		while (t < explodeTime)
		{
			for (int j = 0; j < pieces.Length; j++)
			{
				t += CupheadTime.FixedDelta;
				float num = EaseUtils.Ease(EaseUtils.EaseType.easeOutSine, 0f, 1f, t / explodeTime);
				pieces[j].wafflepiece.transform.localPosition = Vector3.Lerp(pieces[j].wafflepiece.transform.localPosition.normalized, pieces[j].direction * properties.explodeDistance, num * pauseValue);
				hitPause(j);
			}
			yield return wait;
		}
		t = 0f;
		foreach (WafflePieces wafflePieces2 in pieces)
		{
			wafflePieces2.wafflepiece.GetComponent<Collider2D>().enabled = true;
			wafflePieces2.waffleFX.SetTrigger("Death");
		}
		if (isFirst)
		{
			explosionReverse.Create(new Vector2(mouth.position.x, mouth.position.y - 20f));
		}
		while (t < returnTime / 2f)
		{
			for (int l = 0; l < pieces.Length; l++)
			{
				t += CupheadTime.FixedDelta;
				float num2 = EaseUtils.Ease(EaseUtils.EaseType.easeInSine, 0f, 1f, t / returnTime);
				pieces[l].wafflepiece.transform.localPosition = Vector3.Lerp(pieces[l].wafflepiece.transform.localPosition, Vector3.zero, num2 * pauseValue);
				hitPause(l);
			}
			yield return wait;
		}
		for (int m = 0; m < pieces.Length; m++)
		{
			pieces[m].wafflepiece.GetComponent<Collider2D>().enabled = false;
			pieces[m].wafflepiece.localPosition = Vector3.zero;
		}
		yield return null;
		if (!isFirst)
		{
			base.animator.SetBool("Split", false);
			GetComponent<Collider2D>().enabled = true;
			yield return base.animator.WaitForAnimationToEnd(this, "Waffle_Return");
			state = State.Move;
			switchedOn = false;
			yield return StartCoroutine(check_attack_cr());
		}
	}

	private void switchAnimation()
	{
		switchedOn = true;
		base.animator.SetBool("Split", true);
	}

	private IEnumerator destroyMouth_cr()
	{
		yield return base.animator.WaitForAnimationToEnd(this, "Waffle_Explode_Death");
		mouth.GetComponent<Collider2D>().enabled = false;
	}

	private IEnumerator death_cr()
	{
		pivotPoint.transform.position = originalPivotPos;
		YieldInstruction wait = new WaitForFixedUpdate();
		StartExplosions();
		Collider2D collider = GetComponent<Collider2D>();
		isDead = true;
		state = State.Dying;
		isDying = true;
		base.animator.SetTrigger("Death");
		GetComponent<Collider2D>().enabled = false;
		yield return CupheadTime.WaitForSeconds(this, 1f);
		base.animator.SetBool("DeathExplode", true);
		bool explodeDeath = true;
		float untilDestroy = 1500f;
		StartCoroutine(destroyMouth_cr());
		while (explodeDeath)
		{
			collider.enabled = false;
			for (int i = 0; i < diagonalPieces.Length; i++)
			{
				diagonalPieces[i].distanceFromCenter = Vector3.Distance(diagonalPieces[i].wafflepiece.transform.localPosition, mouth.transform.localPosition);
				diagonalPieces[i].wafflepiece.GetComponent<Collider2D>().enabled = false;
				diagonalPieces[i].wafflepiece.transform.localPosition += diagonalPieces[i].direction * 700f * CupheadTime.FixedDelta;
				if (diagonalPieces[i].distanceFromCenter >= untilDestroy)
				{
					explodeDeath = false;
					break;
				}
			}
			for (int j = 0; j < straightPieces.Length; j++)
			{
				straightPieces[j].distanceFromCenter = Vector3.Distance(straightPieces[j].wafflepiece.transform.localPosition, mouth.transform.localPosition);
				straightPieces[j].wafflepiece.GetComponent<Collider2D>().enabled = false;
				straightPieces[j].wafflepiece.transform.localPosition += straightPieces[j].direction * 700f * CupheadTime.FixedDelta;
				if (straightPieces[j].distanceFromCenter >= untilDestroy)
				{
					explodeDeath = false;
					break;
				}
			}
			yield return wait;
		}
		Die();
		yield return null;
	}

	private void SoundWaffleExplode()
	{
		AudioManager.Play("level_baroness_waffle_explode");
		emitAudioFromObject.Add("level_baroness_waffle_explode");
	}

	private void SoundWaffleWingflap()
	{
		AudioManager.Play("level_baroness_waffle_wingflap");
		emitAudioFromObject.Add("level_baroness_waffle_wingflap");
	}

	private void SoundWaffleReform()
	{
		AudioManager.Play("level_baroness_waffle_reform");
		emitAudioFromObject.Add("level_baroness_waffle_reform");
	}
}
