using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirshipCrabLevelCrab : LevelProperties.AirshipCrab.Entity
{
	public enum State
	{
		Closed = 0,
		Open = 1,
		Dead = 2
	}

	[SerializeField]
	private Transform barncileRoot;

	[SerializeField]
	private Transform bubbleRoot;

	[SerializeField]
	private Collider2D crabHitBox;

	[SerializeField]
	private BasicProjectile barnicleProjectile;

	[SerializeField]
	private AirshipCrabLevelBubbles bubbleProjectile;

	[SerializeField]
	private AirshipCrabLevelGems gemProjectile;

	private DamageDealer damageDealer;

	private DamageReceiver damageReceiver;

	private Vector3 closedPos;

	private Vector3 openPos;

	private List<AirshipCrabLevelGems> gems;

	private Coroutine stateCoroutine;

	private bool releaseAllAtOnce;

	public State state { get; private set; }

	protected override void Awake()
	{
		base.Awake();
		gems = new List<AirshipCrabLevelGems>();
		damageDealer = DamageDealer.NewEnemy();
		damageReceiver = crabHitBox.GetComponent<DamageReceiver>();
		damageReceiver.OnDamageTaken += OnDamageTaken;
		crabHitBox.enabled = false;
	}

	public override void LevelInit(LevelProperties.AirshipCrab properties)
	{
		base.LevelInit(properties);
		StartCoroutine(intro_cr());
		closedPos = base.transform.position;
		openPos = base.transform.position;
		openPos.y = base.transform.position.y + properties.CurrentState.main.openCrabOffsetY;
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

	private IEnumerator intro_cr()
	{
		state = State.Closed;
		yield return CupheadTime.WaitForSeconds(this, 3f);
		StartCoroutine(barnacle_cr());
		StartCoroutine(spawn_gems_cr());
	}

	private void StateHandler()
	{
		if (state == State.Open)
		{
			base.transform.position = openPos;
			crabHitBox.enabled = true;
			if (stateCoroutine != null)
			{
				StopCoroutine(stateCoroutine);
			}
			stateCoroutine = StartCoroutine(bubbles_cr());
		}
		else if (state == State.Closed)
		{
			base.transform.position = closedPos;
			crabHitBox.enabled = false;
			if (stateCoroutine != null)
			{
				StopCoroutine(stateCoroutine);
			}
			stateCoroutine = StartCoroutine(gems_cr());
		}
	}

	private IEnumerator barnacle_cr()
	{
		LevelProperties.AirshipCrab.Barnicles p = base.properties.CurrentState.barnicles;
		float offsetY = p.barnicleOffsetY;
		float offsetX = p.barnicleOffsetX;
		float rotation = Mathf.Atan2(0f, Level.Current.Left) * 57.29578f;
		while (true)
		{
			for (int i = 0; (float)i < p.barnicleAmount; i++)
			{
				Vector2 pos = barncileRoot.position;
				pos.y = barncileRoot.position.y + offsetY * (float)i;
				pos.x = barncileRoot.position.x + offsetX;
				barnicleProjectile.Create(pos, rotation, p.bulletSpeed);
				yield return CupheadTime.WaitForSeconds(this, p.shotDelay);
			}
			yield return CupheadTime.WaitForSeconds(this, p.hesitate);
		}
	}

	private IEnumerator spawn_gems_cr()
	{
		state = State.Closed;
		LevelProperties.AirshipCrab.Gems p = base.properties.CurrentState.gems;
		string[] anglePattern = p.angleString.GetRandom().Split(',');
		int angleIndex = 0;
		float angle = 0f;
		float offsetX = p.gemOffsetX;
		float offsetY = p.gemOffsetY;
		for (int i = 0; (float)i < p.gemAmount; i++)
		{
			float.TryParse(anglePattern[angleIndex], out angle);
			Vector2 pos = barncileRoot.position;
			pos.y = barncileRoot.position.y + offsetY * (float)i;
			pos.x = barncileRoot.position.x + offsetX;
			AirshipCrabLevelGems gem = Object.Instantiate(gemProjectile);
			gem.Init(p, pos, angle);
			gems.Add(gem);
			angleIndex = (angleIndex + 1) % anglePattern.Length;
			yield return null;
		}
		releaseAllAtOnce = false;
		StartCoroutine(gems_cr());
		yield return null;
	}

	private IEnumerator gems_cr()
	{
		LevelProperties.AirshipCrab.Gems p = base.properties.CurrentState.gems;
		string[] delayPattern = p.gemReleaseDelay.GetRandom().Split(',');
		float waitTime = 0f;
		float t = 0f;
		int delayIndex = 0;
		int counter = 0;
		bool checking = true;
		bool startTimer = false;
		bool resetTimer = false;
		for (int n = 0; (float)n < p.gemATKAmount; n++)
		{
			if (!gems[n].moving)
			{
				if (!releaseAllAtOnce)
				{
					float.TryParse(delayPattern[delayIndex], out waitTime);
				}
				gems[n].parried = false;
				gems[n].lastSideHit = AirshipCrabLevelGems.SideHit.None;
				gems[n].PickMovement();
				if (!releaseAllAtOnce)
				{
					yield return CupheadTime.WaitForSeconds(this, waitTime);
					delayIndex %= delayPattern.Length;
				}
			}
		}
		while (checking)
		{
			for (int i = 0; (float)i < p.gemATKAmount; i++)
			{
				if (gems[i].parried)
				{
					counter++;
				}
				if (gems[i].startTimer)
				{
					gems[i].startTimer = false;
					startTimer = true;
					resetTimer = true;
				}
				if ((float)counter == p.gemATKAmount)
				{
					checking = false;
					break;
				}
			}
			if (startTimer)
			{
				if (resetTimer)
				{
					t = 0f;
					resetTimer = false;
				}
				if (t < p.gemHoldDuration)
				{
					t += (float)CupheadTime.Delta;
				}
				else
				{
					releaseAllAtOnce = true;
					state = State.Closed;
					StateHandler();
					startTimer = false;
				}
			}
			counter = 0;
			yield return null;
		}
		releaseAllAtOnce = false;
		state = State.Open;
		StateHandler();
	}

	private IEnumerator bubbles_cr()
	{
		state = State.Open;
		LevelProperties.AirshipCrab.Bubbles p = base.properties.CurrentState.bubbles;
		string[] bubblePattern = p.bubbleCount.GetRandom().Split(',');
		int index = 0;
		StartCoroutine(bubble_timer_cr());
		while (state == State.Open)
		{
			float count;
			float.TryParse(bubblePattern[index], out count);
			for (int i = 0; (float)i < count; i++)
			{
				AirshipCrabLevelBubbles bubbles = Object.Instantiate(bubbleProjectile);
				bubbles.Init(bubbleRoot.transform.position, p, p.bubbleSpeed);
				yield return CupheadTime.WaitForSeconds(this, p.bubbleRepeatDelay);
			}
			index = (index + 1) % bubblePattern.Length;
			yield return CupheadTime.WaitForSeconds(this, p.bubbleMainDelay);
			yield return null;
		}
	}

	private IEnumerator bubble_timer_cr()
	{
		yield return CupheadTime.WaitForSeconds(this, base.properties.CurrentState.bubbles.openTimer);
		state = State.Closed;
		StopCoroutine(bubbles_cr());
		StateHandler();
	}
}
