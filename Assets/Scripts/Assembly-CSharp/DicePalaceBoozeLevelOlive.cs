using System;
using System.Collections;
using UnityEngine;

public class DicePalaceBoozeLevelOlive : AbstractCollidableObject
{
	[SerializeField]
	private BasicProjectile pimentoPrefab;

	private int verticalCoordinateIndex;

	private int horizontalCoordinateindex;

	private float health;

	private int shotCount;

	private int shotCountMax;

	private int moveCount;

	private int moveCountMaxIndex;

	private int moveCountMax;

	private bool isDead;

	private bool moving;

	private string yCoordinates;

	private string xCoordinates;

	private PlayerId nextPlayerTarget;

	private Vector3 moveToTarget;

	private LevelProperties.DicePalaceBooze properties;

	private DamageReceiver damageReceiver;

	private DamageDealer damageDealer;

	protected override void Awake()
	{
		damageDealer = DamageDealer.NewEnemy();
		damageReceiver = GetComponent<DamageReceiver>();
		damageReceiver.OnDamageTaken += OnDamageTaken;
		moving = false;
		shotCount = 0;
		moveCount = 0;
		nextPlayerTarget = PlayerId.PlayerOne;
		base.Awake();
	}

	protected override void Update()
	{
		if (damageDealer != null)
		{
			damageDealer.Update();
		}
		base.Update();
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionPlayer(hit, phase);
		damageDealer.DealDamage(hit);
	}

	private void OnDamageTaken(DamageDealer.DamageInfo info)
	{
		health -= info.damage;
		if (health < 0f && !isDead)
		{
			isDead = true;
			OnDeath();
		}
	}

	public void InitOlive(LevelProperties.DicePalaceBooze properties, int maxShotCount, string yCoordinates, string xCoordinates)
	{
		this.properties = properties;
		shotCountMax = maxShotCount;
		health = properties.CurrentState.martini.oliveHP;
		this.yCoordinates = yCoordinates;
		this.xCoordinates = xCoordinates;
		moveCountMaxIndex = UnityEngine.Random.Range(0, properties.CurrentState.martini.moveString.Split(',').Length);
		moveCountMax = int.Parse(properties.CurrentState.martini.moveString.Split(',')[moveCountMaxIndex]);
		verticalCoordinateIndex = UnityEngine.Random.Range(0, yCoordinates.Split(',').Length);
		horizontalCoordinateindex = UnityEngine.Random.Range(0, xCoordinates.Split(',').Length);
		moveToTarget.y = Level.Current.Ground + int.Parse(yCoordinates.Split(',')[verticalCoordinateIndex]);
		moveToTarget.x = Level.Current.Left + 50 + int.Parse(xCoordinates.Split(',')[horizontalCoordinateindex]);
		Level.Current.OnWinEvent += OnDeath;
		StartCoroutine(attack_cr());
	}

	public void ResetOlive(int maxShotCount)
	{
		GetComponent<Collider2D>().enabled = true;
		shotCountMax = maxShotCount;
		health = properties.CurrentState.martini.oliveHP;
		StartCoroutine(attack_cr());
		isDead = false;
	}

	private IEnumerator attack_cr()
	{
		while (true)
		{
			if (moveCount < moveCountMax)
			{
				GetNextTarget();
				StartCoroutine(move_cr());
				moveCount++;
				while (moving)
				{
					yield return null;
				}
				yield return CupheadTime.WaitForSeconds(this, properties.CurrentState.martini.oliveStopDuration);
			}
			else
			{
				AudioManager.Play("booze_olive_attack");
				emitAudioFromObject.Add("booze_olive_attack");
				base.animator.SetTrigger("OnAttack");
				yield return base.animator.WaitForAnimationToEnd(this, "Attack");
				yield return CupheadTime.WaitForSeconds(this, properties.CurrentState.martini.oliveHesitateAfterShooting);
			}
		}
	}

	private void Shoot()
	{
		StartCoroutine(shoot_cr());
	}

	private IEnumerator shoot_cr()
	{
		moveCount = 0;
		Vector3 target = PlayerManager.GetPlayer(nextPlayerTarget).center - base.transform.position;
		BasicProjectile proj = pimentoPrefab.Create(base.transform.position, 0f, properties.CurrentState.martini.bulletSpeed);
		proj.animator.SetBool("Reverse", Rand.Bool());
		proj.transform.right = target;
		IEnumerator enumerator = proj.GetComponentInChildren<Transform>().GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				Transform transform = (Transform)enumerator.Current;
				transform.SetEulerAngles(0f, 0f, 0f);
			}
		}
		finally
		{
			IDisposable disposable2;
			IDisposable disposable = (disposable2 = enumerator as IDisposable);
			if (disposable2 != null)
			{
				disposable.Dispose();
			}
		}
		shotCount++;
		if (shotCount >= shotCountMax)
		{
			proj.GetComponent<SpriteRenderer>().color = Color.magenta;
			proj.SetParryable(true);
			shotCount = 0;
		}
		yield return null;
	}

	private IEnumerator move_cr()
	{
		moving = true;
		while (Vector3.Distance(base.transform.position, moveToTarget) > 5f)
		{
			Vector3 dir = (moveToTarget - base.transform.position).normalized;
			base.transform.position += dir * properties.CurrentState.martini.oliveSpeed * CupheadTime.Delta;
			yield return null;
		}
		moving = false;
	}

	private void GetNextTarget()
	{
		verticalCoordinateIndex++;
		if (verticalCoordinateIndex >= yCoordinates.Split(',').Length)
		{
			verticalCoordinateIndex = 0;
		}
		horizontalCoordinateindex++;
		if (horizontalCoordinateindex >= xCoordinates.Split(',').Length)
		{
			horizontalCoordinateindex = 0;
		}
		moveToTarget.y = Level.Current.Ground + int.Parse(yCoordinates.Split(',')[verticalCoordinateIndex]);
		moveToTarget.x = Level.Current.Left + 50 + int.Parse(xCoordinates.Split(',')[horizontalCoordinateindex]);
	}

	protected override void OnDestroy()
	{
		StopAllCoroutines();
		base.OnDestroy();
	}

	private void OnDeath()
	{
		AudioManager.Play("booze_olive_death");
		emitAudioFromObject.Add("booze_olive_death");
		GetComponent<Collider2D>().enabled = false;
		StopAllCoroutines();
		base.animator.SetTrigger("OnDeath");
	}

	private void Deactivate()
	{
		base.gameObject.SetActive(false);
	}
}
