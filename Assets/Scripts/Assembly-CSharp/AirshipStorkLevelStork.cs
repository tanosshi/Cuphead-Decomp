using System.Collections;
using UnityEngine;

public class AirshipStorkLevelStork : LevelProperties.AirshipStork.Entity
{
	[SerializeField]
	private Transform projectileRoot;

	[SerializeField]
	private Transform knobSprite;

	[SerializeField]
	private AirshipStorkLevelProjectile projectile;

	[SerializeField]
	private AirshipStorkLevelProjectile projectilePink;

	[SerializeField]
	private AirshipStorkLevelBaby babyPrefab;

	private bool farRight = true;

	private bool slowDown;

	private int index;

	private DamageDealer damageDealer;

	private DamageReceiver damageReceiver;

	private AirshipLevelKnob knobSwitch;

	protected override void Awake()
	{
		base.Awake();
		damageDealer = DamageDealer.NewEnemy();
		damageReceiver = GetComponent<DamageReceiver>();
		damageReceiver.OnDamageTaken += OnDamageTaken;
	}

	protected override void Start()
	{
		base.Start();
		CupheadLevelCamera.Current.StartFloat(25f, 3f);
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

	public override void LevelInit(LevelProperties.AirshipStork properties)
	{
		base.LevelInit(properties);
		knobSwitch = AirshipLevelKnob.Create(knobSprite.transform);
		knobSwitch.OnActivate += OnKnobParry;
		LevelProperties.AirshipStork.Main main = properties.CurrentState.main;
		Vector3 position = base.transform.position;
		position.y = main.headHeight;
		base.transform.position = position;
		StartCoroutine(intro_cr());
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionPlayer(hit, phase);
		damageDealer.DealDamage(hit);
	}

	private void OnKnobParry()
	{
		base.properties.DealDamage(base.properties.CurrentState.main.parryDamage);
		StartCoroutine(hurt_cr());
	}

	private IEnumerator intro_cr()
	{
		yield return CupheadTime.WaitForSeconds(this, 5f);
		StartCoroutine(move_cr());
		StartCoroutine(spiral_shot_cr());
		StartCoroutine(babies_cr());
	}

	private IEnumerator hurt_cr()
	{
		knobSwitch.enabled = false;
		knobSprite.GetComponent<SpriteRenderer>().enabled = false;
		yield return CupheadTime.WaitForSeconds(this, base.properties.CurrentState.main.pinkDurationOff);
		knobSwitch.enabled = true;
		knobSprite.GetComponent<SpriteRenderer>().enabled = true;
	}

	private IEnumerator move_cr()
	{
		float offset = 220f;
		float stopDist = 200f;
		float moveTime = 0f;
		LevelProperties.AirshipStork.Main p = base.properties.CurrentState.main;
		string[] leftMovementPattern = p.leftMovementTime.GetRandom().Split(',');
		Vector3 pos = base.transform.position;
		float.TryParse(leftMovementPattern[index], out moveTime);
		float t = 0f;
		while (true)
		{
			if (farRight)
			{
				slowDown = false;
				while (t < moveTime)
				{
					base.transform.position -= base.transform.right * (p.movementSpeed * (float)CupheadTime.Delta);
					t += (float)CupheadTime.Delta;
					yield return null;
				}
				t = 0f;
				farRight = !farRight;
			}
			else
			{
				slowDown = false;
				while (base.transform.position.x < 640f - offset)
				{
					float dist = 640f - offset - base.transform.position.x;
					dist = Mathf.Abs(dist);
					pos.x = Mathf.MoveTowards(base.transform.position.x, 640f - offset, p.movementSpeed * (float)CupheadTime.Delta);
					if (dist < stopDist)
					{
						slowDown = true;
					}
					base.transform.position = pos;
					yield return null;
				}
				farRight = !farRight;
			}
			moveTime = (moveTime + 1f) % (float)leftMovementPattern.Length;
		}
	}

	private IEnumerator spiral_shot_cr()
	{
		LevelProperties.AirshipStork.SpiralShot p = base.properties.CurrentState.spiralShot;
		string[] pinkPattern = p.pinkString.GetRandom().Split(',');
		string[] delayPattern = p.shotDelayString.GetRandom().Split(',');
		string[] directionPattern = p.spiralDirection.GetRandom().Split(',');
		int delayIndex = Random.Range(0, delayPattern.Length);
		int pinkIndex = Random.Range(0, pinkPattern.Length);
		int directionIndex = Random.Range(0, directionPattern.Length);
		float seconds = 0f;
		int direction = 0;
		while (true)
		{
			yield return CupheadTime.WaitForSeconds(this, seconds);
			float.TryParse(delayPattern[delayIndex], out seconds);
			int.TryParse(directionPattern[directionIndex], out direction);
			if (pinkPattern[pinkIndex][0] == 'R')
			{
				projectile.Create(projectileRoot.transform.position, 0f, p.movementSpeed, p.spiralRate, direction);
			}
			else if (pinkPattern[pinkIndex][0] == 'P')
			{
				projectilePink.Create(projectileRoot.transform.position, 0f, p.movementSpeed, p.spiralRate, direction);
			}
			pinkIndex = (pinkIndex + 1) % pinkPattern.Length;
			delayIndex = (delayIndex + 1) % delayPattern.Length;
			directionIndex = (directionIndex + 1) % directionPattern.Length;
		}
	}

	private IEnumerator babies_cr()
	{
		LevelProperties.AirshipStork.Babies p = base.properties.CurrentState.babies;
		string[] delayPattern = p.babyDelayString.GetRandom().Split(',');
		int index = Random.Range(0, delayPattern.Length);
		float delay = 0f;
		while (true)
		{
			float.TryParse(delayPattern[index], out delay);
			yield return CupheadTime.WaitForSeconds(this, delay);
			Vector2 pos = base.transform.position;
			pos.y = Level.Current.Ground;
			pos.x = Level.Current.Right;
			AirshipStorkLevelBaby baby = Object.Instantiate(babyPrefab);
			baby.Init(p, pos, p.HP);
			yield return null;
			index = (index + 1) % delayPattern.Length;
		}
	}
}
