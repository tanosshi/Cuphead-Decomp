using System;
using System.Collections;
using UnityEngine;

public class AirshipClamLevelClam : LevelProperties.AirshipClam.Entity
{
	[SerializeField]
	private BasicProjectile pearlPrefab;

	[SerializeField]
	private AirshipClamLevelBarnacle barnaclePrefab;

	[SerializeField]
	private AirshipClamLevelBarnacleParryable barnacleParryablePrefab;

	private bool attacking;

	private float idleSpeed;

	private int pShotAttackDelayIndex;

	private int barnacleAttackDelayIndex;

	private int barnacleTypeIndex;

	private bool clamOut;

	private int clamOutShotCountIndex;

	private Vector3 pivotPoint;

	[SerializeField]
	private Transform[] spawnPoints;

	private Action callback;

	private float time;

	private DamageDealer damageDealer;

	private DamageReceiver damageReceiver;

	protected override void Awake()
	{
		damageDealer = DamageDealer.NewEnemy();
		damageReceiver = GetComponent<DamageReceiver>();
		damageReceiver.OnDamageTaken += OnDamageTaken;
		base.Awake();
	}

	protected override void Update()
	{
		damageDealer.Update();
		base.Update();
	}

	private void OnDamageTaken(DamageDealer.DamageInfo info)
	{
		base.properties.DealDamage(info.damage);
	}

	public override void LevelInit(LevelProperties.AirshipClam properties)
	{
		base.LevelInit(properties);
		pivotPoint.x = Level.Current.Left + Level.Current.Width / 2;
		pivotPoint.y = (float)Level.Current.Ground + (float)Level.Current.Height * 0.65f;
		pivotPoint.z = 0f;
		attacking = false;
		clamOut = false;
		time = 0f;
		idleSpeed = properties.CurrentState.spit.movementSpeedScale;
		pShotAttackDelayIndex = UnityEngine.Random.Range(0, properties.CurrentState.spit.attackDelayString.Split(',').Length);
		barnacleAttackDelayIndex = UnityEngine.Random.Range(0, properties.CurrentState.barnacles.attackDelayString.Split(',').Length);
		barnacleTypeIndex = UnityEngine.Random.Range(0, properties.CurrentState.barnacles.typeString.Split(',').Length);
		clamOutShotCountIndex = UnityEngine.Random.Range(0, properties.CurrentState.clamOut.shotString.Split(',').Length);
		StartCoroutine(move_cr());
	}

	private IEnumerator move_cr()
	{
		while (true)
		{
			if (!attacking)
			{
				Vector3 pos = pivotPoint + Vector3.right * Mathf.Sin(time * idleSpeed) * 300f;
				base.transform.position = pos + Vector3.up * Mathf.Sin(time * (idleSpeed * 4f)) * 50f;
				time += CupheadTime.Delta;
			}
			yield return null;
		}
	}

	public void OnSpitStart(Action callback)
	{
		this.callback = callback;
		StartCoroutine(spit_cr());
	}

	private IEnumerator spit_cr()
	{
		yield return CupheadTime.WaitForSeconds(this, base.properties.CurrentState.spit.initialShotDelay);
		attacking = true;
		base.animator.SetTrigger("OnPearlShot");
		yield return CupheadTime.WaitForSeconds(this, base.properties.CurrentState.spit.preShotDelay);
		Vector3 target = PlayerManager.GetNext().center + Vector3.up * 50f;
		float rotation = Vector3.Angle(Vector3.down, base.transform.position - target);
		rotation = ((!(target.x > base.transform.position.x)) ? (270f - rotation) : (rotation + 270f));
		pearlPrefab.Create(spawnPoints[0].position, 0f - rotation, base.properties.CurrentState.spit.bulletSpeed);
		base.animator.SetTrigger("OnPearlShot");
		yield return base.animator.WaitForAnimationToEnd(this, true);
		attacking = false;
		yield return CupheadTime.WaitForSeconds(this, float.Parse(base.properties.CurrentState.spit.attackDelayString.Split(',')[pShotAttackDelayIndex]));
		pShotAttackDelayIndex++;
		if (pShotAttackDelayIndex >= base.properties.CurrentState.spit.attackDelayString.Split(',').Length)
		{
			pShotAttackDelayIndex = 0;
		}
		if (callback != null)
		{
			callback();
		}
	}

	public void OnBarnaclesStart(Action callback)
	{
		this.callback = callback;
		StartCoroutine(spawnBarnacles_cr());
	}

	private IEnumerator spawnBarnacles_cr()
	{
		bool parryable = false;
		for (float duration = base.properties.CurrentState.barnacles.attackDuration.RandomFloat(); duration > 0f; duration -= float.Parse(base.properties.CurrentState.barnacles.attackDelayString.Split(',')[barnacleAttackDelayIndex]))
		{
			if (base.properties.CurrentState.barnacles.typeString.Split(',')[barnacleTypeIndex][0] == 'P')
			{
				parryable = true;
			}
			barnacleTypeIndex++;
			if (barnacleTypeIndex >= base.properties.CurrentState.barnacles.typeString.Split(',').Length)
			{
				barnacleTypeIndex = 0;
			}
			if (!parryable)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(barnaclePrefab.gameObject, spawnPoints[0].position, Quaternion.identity);
				gameObject.transform.localScale = Vector3.one * base.properties.CurrentState.barnacles.barnacleScale;
				gameObject.GetComponent<AirshipClamLevelBarnacle>().InitBarnacle(-1, base.properties);
				gameObject = UnityEngine.Object.Instantiate(barnaclePrefab.gameObject, spawnPoints[1].position, Quaternion.identity);
				gameObject.transform.localScale = Vector3.one * base.properties.CurrentState.barnacles.barnacleScale;
				gameObject.GetComponent<AirshipClamLevelBarnacle>().InitBarnacle(1, base.properties);
			}
			else
			{
				GameObject gameObject2 = UnityEngine.Object.Instantiate(barnacleParryablePrefab.gameObject, spawnPoints[0].position, Quaternion.identity);
				gameObject2.transform.localScale = Vector3.one * base.properties.CurrentState.barnacles.barnacleScale;
				gameObject2.GetComponent<AirshipClamLevelBarnacleParryable>().InitBarnacle(-1, base.properties);
				gameObject2 = UnityEngine.Object.Instantiate(barnacleParryablePrefab.gameObject, spawnPoints[1].position, Quaternion.identity);
				gameObject2.transform.localScale = Vector3.one * base.properties.CurrentState.barnacles.barnacleScale;
				gameObject2.GetComponent<AirshipClamLevelBarnacleParryable>().InitBarnacle(1, base.properties);
			}
			yield return CupheadTime.WaitForSeconds(this, float.Parse(base.properties.CurrentState.barnacles.attackDelayString.Split(',')[barnacleAttackDelayIndex]));
		}
		barnacleAttackDelayIndex++;
		if (barnacleAttackDelayIndex >= base.properties.CurrentState.barnacles.attackDelayString.Split(',').Length)
		{
			barnacleAttackDelayIndex = 0;
		}
		if (!clamOut && callback != null)
		{
			callback();
		}
	}

	private void OnStringShot()
	{
		base.animator.SetBool("OnStringShot", true);
		StartCoroutine(clamOut_cr());
	}

	private IEnumerator clamOut_cr()
	{
		damageReceiver.enabled = true;
		int max = int.Parse(base.properties.CurrentState.clamOut.shotString.Split(',')[clamOutShotCountIndex]);
		Vector3 target = PlayerManager.GetNext().center + Vector3.up * 50f;
		yield return CupheadTime.WaitForSeconds(this, base.properties.CurrentState.clamOut.preShotDelay);
		for (int i = 0; i < max; i++)
		{
			if (i != 0)
			{
				yield return CupheadTime.WaitForSeconds(this, base.properties.CurrentState.clamOut.bulletRepeatDelay);
			}
			target = PlayerManager.GetNext().center;
			target = PlayerManager.GetNext().center + Vector3.up * 50f;
			float rotation = Vector3.Angle(Vector3.down, base.transform.position - target);
			rotation = ((!(target.x > base.transform.position.x)) ? (270f - rotation) : (rotation + 270f));
			pearlPrefab.Create(spawnPoints[0].position, 0f - rotation, base.properties.CurrentState.spit.bulletSpeed);
		}
		yield return CupheadTime.WaitForSeconds(this, base.properties.CurrentState.clamOut.bulletMainDelay);
		clamOutShotCountIndex++;
		if (clamOutShotCountIndex >= base.properties.CurrentState.clamOut.shotString.Split(',').Length)
		{
			clamOutShotCountIndex = 0;
		}
		base.animator.SetBool("OnStringShot", false);
		yield return base.animator.WaitForAnimationToEnd(this, true);
		damageReceiver.enabled = false;
		if (callback != null)
		{
			callback();
		}
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		if (damageDealer != null)
		{
			damageDealer.DealDamage(hit);
		}
		base.OnCollisionPlayer(hit, phase);
	}

	protected override void OnCollisionOther(GameObject hit, CollisionPhase phase)
	{
		if (!attacking)
		{
			AirshipClamLevelBarnacleParryable component = hit.GetComponent<AirshipClamLevelBarnacleParryable>();
			if (component != null && component.parried)
			{
				base.animator.SetBool("OnBarnacles", false);
				OnStringShot();
				UnityEngine.Object.Destroy(hit.gameObject);
			}
		}
		base.OnCollisionOther(hit, phase);
	}

	protected override void OnDestroy()
	{
		StopAllCoroutines();
		base.OnDestroy();
	}
}
