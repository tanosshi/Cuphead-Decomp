using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevilLevelGiantHead : LevelProperties.Devil.Entity
{
	public enum State
	{
		Intro = 0,
		Idle = 1,
		BombEye = 2,
		SkullEye = 3
	}

	private struct SwooperSlot
	{
		public float xPos;

		public DevilLevelSwooper swooper;

		public SwooperSlot(float xPos)
		{
			this.xPos = xPos;
			swooper = null;
		}
	}

	public State state;

	[SerializeField]
	private GameObject[] groundPieces;

	[SerializeField]
	private DevilLevelPlatform[] HandsPhaseExit;

	[SerializeField]
	private DevilLevelPlatform[] TearsPhaseExit;

	[SerializeField]
	private DevilLevelPlatform[] raisablePlatforms;

	[SerializeField]
	private Transform stage2Platforms;

	[SerializeField]
	private Transform stage3Platforms;

	[SerializeField]
	private DevilLevelSplitDevil[] splitDevils;

	[SerializeField]
	private DevilLevelFireball fireballPrefab;

	[SerializeField]
	private DevilLevelBomb bombPrefab;

	[SerializeField]
	private DevilLevelSkull skullPrefab;

	[SerializeField]
	private Transform leftEyeRoot;

	[SerializeField]
	private Transform rightEyeRoot;

	[SerializeField]
	private Transform middleRoot;

	[SerializeField]
	private Transform leftTearRoot;

	[SerializeField]
	private Transform rightTearRoot;

	[SerializeField]
	private DevilLevelHand[] hands;

	[SerializeField]
	private DevilLevelSwooper swooperPrefab;

	[SerializeField]
	private DevilLevelTear tearPrefab;

	[SerializeField]
	private SpriteRenderer bottomSprite;

	[SerializeField]
	private DamageReceiver child;

	[SerializeField]
	private Transform[] spawnPoints;

	private bool waitingForTransform;

	private bool bombOnLeft;

	private bool DeadLoopSFXActive;

	private DamageReceiver damageReceiver;

	private Coroutine platformCr;

	private Coroutine fireballCr;

	private Coroutine handsCr;

	private Coroutine swooperSpawnCr;

	private Coroutine swooperSwoopCr;

	private Coroutine tearsCr;

	private Vector2 spawnPos;

	private Color color;

	private SwooperSlot[] swooperSlots;

	private List<DevilLevelSwooper> swoopers;

	protected override void Awake()
	{
		base.Awake();
		base.animator.Play("Idle");
		base.animator.Play("Idle_Body", 1);
		state = State.Intro;
		child.OnDamageTaken += OnDamageTaken;
		damageReceiver = GetComponent<DamageReceiver>();
		damageReceiver.OnDamageTaken += OnDamageTaken;
		Level.Current.OnWinEvent += Death;
	}

	private void OnDamageTaken(DamageDealer.DamageInfo info)
	{
		base.properties.DealDamage(info.damage);
	}

	public void StartIntroTransform()
	{
		DevilLevelSplitDevil[] array = splitDevils;
		foreach (DevilLevelSplitDevil devilLevelSplitDevil in array)
		{
			Object.Destroy(devilLevelSplitDevil.gameObject);
		}
		Object.Destroy(stage2Platforms.gameObject);
		StartCoroutine(intro_cr());
	}

	private IEnumerator intro_cr()
	{
		GetComponent<Collider2D>().enabled = false;
		child.GetComponent<Collider2D>().enabled = false;
		platformCr = StartCoroutine(platforms_cr());
		fireballCr = StartCoroutine(fireballs_cr());
		yield return CupheadTime.WaitForSeconds(this, 1f);
		state = State.Idle;
		waitingForTransform = false;
	}

	private void OnNeck()
	{
		base.animator.Play("Idle_Body");
	}

	private void NoNeck()
	{
		base.animator.Play("Off_Body");
	}

	private IEnumerator platforms_cr()
	{
		LevelProperties.Devil.GiantHeadPlatforms p = base.properties.CurrentState.giantHeadPlatforms;
		string[] pattern = p.riseString.Split(',');
		int patternIndex = Random.Range(0, pattern.Length);
		while (true)
		{
			if (waitingForTransform)
			{
				yield return null;
				continue;
			}
			p = base.properties.CurrentState.giantHeadPlatforms;
			patternIndex = (patternIndex + 1) % pattern.Length;
			int platformIndex;
			int.TryParse(pattern[patternIndex], out platformIndex);
			DevilLevelPlatform platform = raisablePlatforms[platformIndex - 1];
			if (platform.state != DevilLevelPlatform.State.Idle)
			{
				bool noIdlePlatforms = true;
				while (noIdlePlatforms)
				{
					DevilLevelPlatform[] array = raisablePlatforms;
					foreach (DevilLevelPlatform devilLevelPlatform in array)
					{
						if (devilLevelPlatform.state == DevilLevelPlatform.State.Idle)
						{
							noIdlePlatforms = false;
						}
					}
					if (noIdlePlatforms)
					{
						yield return CupheadTime.WaitForSeconds(this, p.riseDelayRange.RandomFloat());
					}
				}
			}
			else
			{
				platform.Raise(p.riseSpeed, p.maxHeight, p.holdDelay);
				yield return CupheadTime.WaitForSeconds(this, p.riseDelayRange.RandomFloat());
			}
		}
	}

	private IEnumerator fireballs_cr()
	{
		bool fromRight = Rand.Bool();
		int index = (fromRight ? (raisablePlatforms.Length - 1) : 0);
		LevelProperties.Devil.Fireballs p = base.properties.CurrentState.fireballs;
		yield return CupheadTime.WaitForSeconds(this, p.initialDelay);
		while (true)
		{
			p = base.properties.CurrentState.fireballs;
			DevilLevelPlatform platform = raisablePlatforms[index];
			index = (((!fromRight) ? (index + 1) : (index - 1)) + raisablePlatforms.Length) % raisablePlatforms.Length;
			if (platform.state == DevilLevelPlatform.State.Dead)
			{
				yield return null;
				continue;
			}
			fireballPrefab.Create(platform.transform.position.x, p.fallSpeed, p.fallAcceleration, p.size / 200f);
			yield return CupheadTime.WaitForSeconds(this, p.spawnDelay);
		}
	}

	public void StartBombEye()
	{
		state = State.BombEye;
		StartCoroutine(eye_cr(base.properties.CurrentState.bombEye.hesitate.RandomFloat()));
	}

	public void StartSkullEye()
	{
		state = State.SkullEye;
		StartCoroutine(eye_cr(base.properties.CurrentState.skullEye.hesitate.RandomFloat()));
	}

	private IEnumerator eye_cr(float hesitateTime)
	{
		if (state == State.BombEye)
		{
			bombOnLeft = Rand.Bool();
			spawnPos = ((!bombOnLeft) ? rightEyeRoot.position : leftEyeRoot.position);
			base.animator.SetTrigger("OnBomb");
			base.animator.SetBool("BombLeft", bombOnLeft);
		}
		else
		{
			spawnPos = middleRoot.transform.position;
			base.animator.SetTrigger("OnSpiral");
		}
		yield return CupheadTime.WaitForSeconds(this, hesitateTime);
		state = State.Idle;
	}

	private void SpawnBomb()
	{
		bombPrefab.Create(spawnPos, base.properties.CurrentState.bombEye, bombOnLeft);
	}

	private void Offset()
	{
		if (GetComponent<SpriteRenderer>().flipX)
		{
			base.transform.AddPosition(-60f);
		}
		else
		{
			base.transform.AddPosition(60f);
		}
	}

	private void SpawnSpiral()
	{
		skullPrefab.Create(spawnPos, base.properties.CurrentState.skullEye);
	}

	public void StartHands()
	{
		base.animator.SetTrigger("OnTransA");
		handsCr = StartCoroutine(hands_cr());
	}

	private IEnumerator hands_cr()
	{
		waitingForTransform = true;
		while (state != State.Idle)
		{
			yield return null;
		}
		bool platformsDown = false;
		while (!platformsDown)
		{
			platformsDown = true;
			DevilLevelPlatform[] array = raisablePlatforms;
			foreach (DevilLevelPlatform devilLevelPlatform in array)
			{
				if (devilLevelPlatform.state == DevilLevelPlatform.State.Raising)
				{
					platformsDown = false;
				}
			}
			yield return null;
		}
		waitingForTransform = false;
		DevilLevelPlatform[] handsPhaseExit = HandsPhaseExit;
		foreach (DevilLevelPlatform devilLevelPlatform2 in handsPhaseExit)
		{
			devilLevelPlatform2.Lower(base.properties.CurrentState.giantHeadPlatforms.exitSpeed);
		}
		StartSwoopers();
		bool leftHandShoot = Rand.Bool();
		hands[0].StartPattern(base.properties.CurrentState.hands);
		hands[1].StartPattern(base.properties.CurrentState.hands);
		StartCoroutine(spawn_hand_cr());
		while (true)
		{
			int handIndex = ((!leftHandShoot) ? 1 : 0);
			if (hands[handIndex] != null)
			{
				hands[handIndex].animator.SetTrigger("OnAttack");
			}
			leftHandShoot = !leftHandShoot;
			yield return CupheadTime.WaitForSeconds(this, base.properties.CurrentState.hands.shotDelay.RandomFloat());
			yield return null;
		}
	}

	private IEnumerator spawn_hand_cr()
	{
		LevelProperties.Devil.Hands p = base.properties.CurrentState.hands;
		yield return CupheadTime.WaitForSeconds(this, p.initialSpawnDelay.RandomFloat());
		hands[0].SpawnIn();
		yield return CupheadTime.WaitForSeconds(this, p.initialSpawnDelay.RandomFloat());
		hands[1].SpawnIn();
		while (!hands[0].isDead)
		{
			while (!hands[0].despawned && !hands[1].despawned)
			{
				yield return null;
			}
			yield return CupheadTime.WaitForSeconds(this, p.spawnDelayRange.RandomFloat());
			if (hands[0].despawned)
			{
				hands[0].SpawnIn();
			}
			else if (hands[1].despawned)
			{
				hands[1].SpawnIn();
			}
			yield return null;
		}
		yield return null;
	}

	public void StartSwoopers()
	{
		swooperSpawnCr = StartCoroutine(swooper_spawn_cr());
		swooperSwoopCr = StartCoroutine(swooper_swoop_cr());
	}

	private IEnumerator swooper_spawn_cr()
	{
		LevelProperties.Devil.Swoopers p = base.properties.CurrentState.swoopers;
		string[] swooperSlotPositions = p.positions.Split(',');
		swoopers = new List<DevilLevelSwooper>();
		swooperSlots = new SwooperSlot[swooperSlotPositions.Length];
		for (int i = 0; i < swooperSlots.Length; i++)
		{
			float result = 0f;
			float.TryParse(swooperSlotPositions[i], out result);
			swooperSlots[i] = new SwooperSlot(result - 600f);
		}
		int swooperSlotIndex = Random.Range(0, swooperSlots.Length);
		float delay = p.initialSpawnDelay.RandomFloat();
		int spawnPoint = Random.Range(0, spawnPoints.Length);
		while (true)
		{
			yield return CupheadTime.WaitForSeconds(this, delay);
			delay = p.spawnDelay.RandomFloat();
			if (swoopers.Count < p.maxCount)
			{
				int numToSpawn = p.spawnCount.RandomInt();
				int numSpawned = 0;
				base.animator.SetBool("IsWhincing", true);
				while (!base.animator.GetCurrentAnimatorStateInfo(0).IsName("Whince"))
				{
					yield return null;
				}
				while (numSpawned < numToSpawn && swoopers.Count < p.maxCount)
				{
					swooperSlotIndex = (swooperSlotIndex + 1) % swooperSlots.Length;
					SwooperSlot slot = swooperSlots[swooperSlotIndex];
					if (slot.swooper == null)
					{
						DevilLevelSwooper item = (slot.swooper = swooperPrefab.Create(this, p, spawnPoints[spawnPoint].position, slot.xPos));
						swoopers.Add(item);
						numSpawned++;
					}
					spawnPoint = (spawnPoint + 1) % spawnPoints.Length;
					yield return CupheadTime.WaitForSeconds(this, 0.4f);
				}
			}
			yield return CupheadTime.WaitForSeconds(this, 1.5f);
			base.animator.SetBool("IsWhincing", false);
		}
	}

	private IEnumerator swooper_swoop_cr()
	{
		LevelProperties.Devil.Swoopers p = base.properties.CurrentState.swoopers;
		while (true)
		{
			if (swoopers.Count == 0)
			{
				yield return null;
				continue;
			}
			List<DevilLevelSwooper> attackSwoopers = new List<DevilLevelSwooper>(swoopers);
			attackSwoopers.Shuffle();
			yield return CupheadTime.WaitForSeconds(this, p.attackDelay.RandomFloat());
			foreach (DevilLevelSwooper swooper in attackSwoopers)
			{
				if (swooper != null && swooper.state == DevilLevelSwooper.State.Idle)
				{
					swooper.Swoop();
					if (swooper == attackSwoopers[attackSwoopers.Count - 1])
					{
						swooper.finalSwooping = true;
					}
					RemoveSwooperFromSlot(swooper);
					if (swooper != attackSwoopers[attackSwoopers.Count - 1])
					{
						yield return CupheadTime.WaitForSeconds(this, p.attackDelay.RandomFloat());
					}
				}
			}
		}
	}

	public void OnSwooperDeath(DevilLevelSwooper swooper)
	{
		swoopers.Remove(swooper);
		RemoveSwooperFromSlot(swooper);
	}

	private void RemoveSwooperFromSlot(DevilLevelSwooper swooper)
	{
		for (int i = 0; i < swooperSlots.Length; i++)
		{
			if (swooperSlots[i].swooper == swooper)
			{
				swooperSlots[i].swooper = null;
			}
		}
	}

	public float PutSwooperInSlot(DevilLevelSwooper swooper)
	{
		float num = float.MaxValue;
		int num2 = 0;
		for (int i = 0; i < swooperSlots.Length; i++)
		{
			if (!(swooperSlots[i].swooper != null))
			{
				float num3 = Mathf.Abs(swooperSlots[i].xPos - swooper.transform.position.x);
				if (num3 < num)
				{
					num = num3;
					num2 = i;
				}
			}
		}
		swooperSlots[num2].swooper = swooper;
		return swooperSlots[num2].xPos;
	}

	public void StartTears()
	{
		tearsCr = StartCoroutine(tears_cr());
	}

	private IEnumerator tears_cr()
	{
		base.animator.SetTrigger("OnTransB");
		waitingForTransform = true;
		while (state != State.Idle)
		{
			yield return null;
		}
		bool platformsDown = false;
		while (!platformsDown)
		{
			platformsDown = true;
			DevilLevelPlatform[] array = raisablePlatforms;
			foreach (DevilLevelPlatform devilLevelPlatform in array)
			{
				if (devilLevelPlatform.state == DevilLevelPlatform.State.Raising)
				{
					platformsDown = false;
				}
			}
			yield return null;
		}
		waitingForTransform = false;
		DevilLevelPlatform[] tearsPhaseExit = TearsPhaseExit;
		foreach (DevilLevelPlatform devilLevelPlatform2 in tearsPhaseExit)
		{
			devilLevelPlatform2.Lower(base.properties.CurrentState.giantHeadPlatforms.exitSpeed);
		}
		if (!base.properties.CurrentState.giantHeadPlatforms.riseDuringTearPhase)
		{
			StopCoroutine(platformCr);
		}
		StopCoroutine(handsCr);
		StopCoroutine(swooperSpawnCr);
		StopCoroutine(swooperSwoopCr);
		while (swoopers.Count > 0)
		{
			swoopers[0].Die();
		}
		DevilLevelHand[] array2 = hands;
		foreach (DevilLevelHand devilLevelHand in array2)
		{
			devilLevelHand.isDead = true;
			devilLevelHand.Die();
		}
		bool spawnLeft = true;
		yield return CupheadTime.WaitForSeconds(this, 2f);
		while (true)
		{
			tearPrefab.Create((!spawnLeft) ? rightTearRoot.transform.position : leftTearRoot.transform.position, base.properties.CurrentState.tears.speed);
			yield return CupheadTime.WaitForSeconds(this, base.properties.CurrentState.tears.delay);
			spawnLeft = !spawnLeft;
		}
	}

	private void Death()
	{
		GetComponent<Collider2D>().enabled = false;
		StopAllCoroutines();
		base.animator.SetTrigger("OnDead");
	}

	private void sfx_p3_bomb_appear()
	{
		AudioManager.Play("p3_bomb_appear");
		emitAudioFromObject.Add("p3_bomb_appear");
	}

	private void sfx_p3_bomb_attack()
	{
		AudioManager.Play("p3_bomb_attack");
		emitAudioFromObject.Add("p3_bomb_attack");
	}

	private void sfx_p3_cry_idle()
	{
		AudioManager.Play("p3_cry_idle");
		emitAudioFromObject.Add("p3_cry_idle");
	}

	private void sfx_p3_dead_loop()
	{
		if (!DeadLoopSFXActive)
		{
			AudioManager.PlayLoop("p3_dead_loop");
			emitAudioFromObject.Add("p3_dead_loop");
			DeadLoopSFXActive = true;
		}
	}

	private void sfx_p3_dead_loop_stop()
	{
		AudioManager.Stop("p3_dead_loop");
		DeadLoopSFXActive = false;
	}

	private void sfx_p3_hand_release_start()
	{
		AudioManager.Play("p3_hand_release_start");
		emitAudioFromObject.Add("p3_hand_release_start");
	}

	private void sfx_p3_hurt_trans_a()
	{
		AudioManager.Play("p3_hurt_trans_a");
		emitAudioFromObject.Add("p3_hurt_trans_a");
	}

	private void sfx_p3_spiral_attack()
	{
		AudioManager.Play("p3_spiral_attack");
		emitAudioFromObject.Add("p3_spiral_attack");
	}

	private void sfx_p3_intro_end()
	{
		AudioManager.Play("p3_intro_end");
		emitAudioFromObject.Add("p3_intro_end");
	}
}
