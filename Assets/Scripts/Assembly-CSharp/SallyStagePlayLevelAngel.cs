using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SallyStagePlayLevelAngel : LevelProperties.SallyStagePlay.Entity
{
	public enum State
	{
		Idle = 0,
		Lightning = 1,
		Wave = 2,
		Meteor = 3
	}

	[SerializeField]
	private Material phase4Material;

	[SerializeField]
	private SallyStagePlayApplauseHandler applauseHandler;

	[SerializeField]
	private Animator sign;

	[SerializeField]
	private SallyStagePlayLevelWave wave;

	[SerializeField]
	private SallyStagePlayLevelMeteor meteorPrefab;

	[SerializeField]
	private SallyStagePlayLevelLightning lightningPrefab;

	[SerializeField]
	private SallyStagePlayLevelUmbrella umbrellaPrefab;

	[SerializeField]
	private GameObject birdsDeath;

	[SerializeField]
	private SallyStagePlayLevelFianceDeity husband;

	[SerializeField]
	private GameObject[] shadows;

	[Space(10f)]
	[SerializeField]
	private Transform phase4Root;

	[SerializeField]
	private Transform birdRoot;

	[SerializeField]
	private Transform phase3Root;

	private List<SallyStagePlayLevelMeteor> meteors;

	private DamageDealer damageDealer;

	private DamageReceiver damageReceiver;

	private Vector3 signStart;

	private bool killedHusband;

	private int nextAttack;

	private int lightningShotIndex;

	private int lightningAngleIndex;

	private int lightningSpawnIndex;

	private int lightningMax;

	private int lightningMaxCounter;

	private int meteorSpawnIndex;

	public State state { get; private set; }

	protected override void Awake()
	{
		base.Awake();
		signStart = sign.transform.position;
		damageDealer = DamageDealer.NewEnemy();
		damageReceiver = GetComponent<DamageReceiver>();
		damageReceiver.OnDamageTaken += OnDamageTaken;
	}

	public override void LevelInit(LevelProperties.SallyStagePlay properties)
	{
		base.LevelInit(properties);
		LevelProperties.SallyStagePlay.Lightning lightning = properties.CurrentState.lightning;
		lightningMax = Random.Range((int)lightning.lightningDelayRange.min, (int)lightning.lightningDelayRange.max);
		lightningShotIndex = Random.Range(0, lightning.lightningShotCount.Split(',').Length);
		lightningAngleIndex = Random.Range(0, lightning.lightningAngleString.Split(',').Length);
		lightningSpawnIndex = Random.Range(0, lightning.lightningSpawnString.Split(',').Length);
		meteorSpawnIndex = Random.Range(0, properties.CurrentState.meteor.meteorSpawnString.Split(',').Length);
		meteors = new List<SallyStagePlayLevelMeteor>();
		if (Level.Current.mode == Level.Mode.Easy)
		{
			Level.Current.OnWinEvent += OnEasyDeath;
		}
		else
		{
			Level.Current.OnWinEvent += OnDeath;
		}
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
		damageDealer.DealDamage(hit);
		base.OnCollisionPlayer(hit, phase);
	}

	public void StartPhase3(bool killedHusband)
	{
		this.killedHusband = killedHusband;
		StartCoroutine(intro_cr());
	}

	private IEnumerator intro_cr()
	{
		float t = 0f;
		float time = 3f;
		Vector3 endPos = new Vector3(base.transform.position.x, phase3Root.position.y);
		Vector2 start = base.transform.position;
		StartCoroutine(sally_angel_intro_sound_cr());
		if (killedHusband)
		{
			StartCoroutine(spawn_husband_cr());
		}
		while (t < time)
		{
			float val = EaseUtils.Ease(EaseUtils.EaseType.easeInOutBounce, 0f, 1f, t / time);
			base.transform.position = Vector2.Lerp(start, endPos, val);
			t += (float)CupheadTime.Delta;
			yield return null;
		}
		base.transform.position = endPos;
		nextAttack = 1;
		StartCoroutine(sign_slide_cr());
		yield return CupheadTime.WaitForSeconds(this, 1f);
		StartCoroutine(main_cr());
		yield return null;
	}

	private IEnumerator sally_angel_intro_sound_cr()
	{
		yield return CupheadTime.WaitForSeconds(this, 4f);
		AudioManager.Play("sally_vox_maniacal");
		emitAudioFromObject.Add("sally_vox_maniacal");
	}

	private IEnumerator spawn_husband_cr()
	{
		husband.gameObject.SetActive(true);
		float t = 0f;
		float time = 3.5f;
		Vector3 endPos = new Vector3(phase3Root.transform.position.x, husband.transform.position.y);
		Vector2 start = husband.transform.position;
		while (t < time)
		{
			float val = EaseUtils.Ease(EaseUtils.EaseType.easeInOutBounce, 0f, 1f, t / time);
			husband.transform.position = Vector2.Lerp(start, endPos, val);
			t += (float)CupheadTime.Delta;
			yield return null;
		}
		husband.Init(base.properties.CurrentState.husband);
		yield return null;
	}

	private IEnumerator sign_slide_cr()
	{
		string attackName = string.Empty;
		switch (nextAttack)
		{
		case 0:
			attackName = "Lightning";
			break;
		case 1:
			attackName = "Meteor";
			break;
		case 2:
			attackName = "Wave";
			break;
		}
		sign.Play(attackName);
		float t = 0f;
		float time = 0.1f;
		Vector3 start = sign.transform.position;
		while (t < time)
		{
			t += (float)CupheadTime.Delta;
			sign.transform.position = Vector3.Lerp(start, new Vector3(signStart.x, signStart.y - 100f), t / time);
			yield return null;
		}
		t = 0f;
		yield return CupheadTime.WaitForSeconds(this, 1f);
		start = sign.transform.position;
		while (t < time)
		{
			t += (float)CupheadTime.Delta;
			sign.transform.position = Vector3.Lerp(start, signStart, t / time);
			yield return null;
		}
		yield return null;
	}

	private IEnumerator slide_out_cr()
	{
		float t = 0f;
		float time = 0.1f;
		Vector3 start = sign.transform.position;
		while (t < time)
		{
			t += (float)CupheadTime.Delta;
			sign.transform.position = Vector3.Lerp(start, signStart, t / time);
			yield return null;
		}
	}

	private IEnumerator main_cr()
	{
		LevelProperties.SallyStagePlay.General p = base.properties.CurrentState.general;
		string[] main = p.attackString.GetRandom().Split(',');
		int mainIndex = (nextAttack = Random.Range(0, main.Length));
		while (main[mainIndex] != "M")
		{
			mainIndex = (mainIndex + 1) % main.Length;
			yield return null;
		}
		while (true)
		{
			if (state != State.Idle)
			{
				yield return null;
				continue;
			}
			base.animator.SetBool("OnPh3Attack", true);
			StartCoroutine(sign_slide_cr());
			yield return base.animator.WaitForAnimationToStart(this, "Phase3_Attack_Start");
			switch (main[mainIndex])
			{
			case "L":
				StartCoroutine(lightning_cr());
				break;
			case "M":
				StartCoroutine(meteor_cr());
				break;
			case "T":
				StartCoroutine(tidal_wave_cr());
				break;
			}
			mainIndex = (mainIndex + 1) % main.Length;
			GetNextAttack(main[mainIndex]);
			yield return null;
		}
	}

	private void GetNextAttack(string main)
	{
		switch (main)
		{
		case "L":
			nextAttack = 0;
			break;
		case "M":
			nextAttack = 1;
			break;
		case "T":
			nextAttack = 2;
			break;
		}
	}

	private IEnumerator lightning_cr()
	{
		state = State.Lightning;
		LevelProperties.SallyStagePlay.Lightning p = base.properties.CurrentState.lightning;
		string[] shotString = p.lightningShotCount.Split(',');
		string[] angleString = p.lightningAngleString.Split(',');
		string[] spawnString = p.lightningSpawnString.Split(',');
		float angle = 0f;
		float spawn = 0f;
		float rotation = 0f;
		int shotCount = 0;
		int.TryParse(shotString[lightningShotIndex], out shotCount);
		for (int i = 0; i < shotCount; i++)
		{
			float.TryParse(spawnString[lightningSpawnIndex], out spawn);
			bool aimAtPlayer;
			if (lightningMaxCounter >= lightningMax)
			{
				aimAtPlayer = true;
				lightningMaxCounter = 0;
			}
			else
			{
				aimAtPlayer = false;
				if (lightningMaxCounter == 0)
				{
					lightningMax = Random.Range((int)p.lightningDirectAimRange.min, (int)p.lightningDirectAimRange.max);
				}
				float.TryParse(angleString[lightningAngleIndex], out angle);
				lightningAngleIndex = (lightningAngleIndex + 1) % angleString.Length;
				lightningMaxCounter++;
			}
			Vector3 pos = new Vector3(-640f + spawn, 460f);
			if (aimAtPlayer)
			{
				AbstractPlayerController next = PlayerManager.GetNext();
				Vector3 vector = next.transform.position - pos;
				rotation = MathUtils.DirectionToAngle(vector);
			}
			else
			{
				rotation = angle;
			}
			lightningPrefab.Create(pos, rotation, p.lightningSpeed, p, (i == shotCount - 1) ? true : false);
			lightningSpawnIndex = (lightningSpawnIndex + 1) % spawnString.Length;
			yield return CupheadTime.WaitForSeconds(this, p.lightningDelayRange.RandomFloat());
		}
		base.animator.SetBool("OnPh3Attack", false);
		lightningShotIndex = (lightningShotIndex + 1) % shotString.Length;
		yield return CupheadTime.WaitForSeconds(this, base.properties.CurrentState.general.attackDelayRange.RandomFloat());
		state = State.Idle;
		yield return null;
	}

	private IEnumerator meteor_cr()
	{
		state = State.Meteor;
		LevelProperties.SallyStagePlay.Meteor p = base.properties.CurrentState.meteor;
		string[] meteorSpawnString = p.meteorSpawnString.Split(',');
		int index = 0;
		float sizeY = meteorPrefab.GetComponent<Renderer>().bounds.size.y;
		float spawn = 0f;
		float.TryParse(meteorSpawnString[meteorSpawnIndex], out spawn);
		bool lockedPosition = false;
		for (int i = 0; i < meteors.Count; i++)
		{
			if (meteors[i].state == SallyStagePlayLevelMeteor.State.Leaving)
			{
				meteors.Remove(meteors[i]);
				i++;
			}
		}
		yield return null;
		for (int j = 0; j < meteors.Count; j++)
		{
			if (spawn == meteors[j].spawnPosition)
			{
				index = j;
				lockedPosition = true;
				break;
			}
		}
		bool positionTaken = false;
		int meteorCounter = 0;
		int spawnStringCounter = 0;
		while (lockedPosition)
		{
			while (meteorCounter < meteors.Count)
			{
				meteorCounter++;
				if (spawn == meteors[index].spawnPosition)
				{
					positionTaken = true;
				}
				index = (index + 1) % meteors.Count;
			}
			if (positionTaken)
			{
				meteorSpawnIndex = (meteorSpawnIndex + 1) % meteorSpawnString.Length;
				float.TryParse(meteorSpawnString[meteorSpawnIndex], out spawn);
				spawnStringCounter++;
				if (spawnStringCounter < meteorSpawnString.Length)
				{
					meteorCounter = 0;
					positionTaken = false;
					yield return null;
					continue;
				}
				break;
			}
			lockedPosition = false;
			break;
		}
		if (meteors.Count <= 0)
		{
			lockedPosition = false;
		}
		if (!lockedPosition)
		{
			meteors.Add(meteorPrefab.Create(spawn, p.meteorHP, p));
			meteorSpawnIndex = (meteorSpawnIndex + 1) % meteorSpawnString.Length;
		}
		base.animator.SetBool("OnPh3Attack", false);
		yield return CupheadTime.WaitForSeconds(this, base.properties.CurrentState.general.attackDelayRange.RandomFloat());
		state = State.Idle;
		yield return null;
	}

	private IEnumerator tidal_wave_cr()
	{
		state = State.Wave;
		LevelProperties.SallyStagePlay.Tidal p = base.properties.CurrentState.tidal;
		float sizeX = wave.GetComponent<Renderer>().bounds.size.x;
		Vector2 pos = new Vector3(-640f - sizeX, Level.Current.Ground);
		wave.StartWave(p);
		while (wave.isMoving)
		{
			yield return null;
		}
		base.animator.SetBool("OnPh3Attack", false);
		yield return CupheadTime.WaitForSeconds(this, base.properties.CurrentState.tidal.tidalHesitate);
		state = State.Idle;
		yield return null;
	}

	public void OnPhase4()
	{
		AudioManager.Stop("sally_sally_lightning_move_loop");
		StopAllCoroutines();
		StartCoroutine(slide_out_cr());
		GetComponent<LevelBossDeathExploder>().StartExplosion();
		base.animator.SetTrigger("OnPh3Death");
		if (killedHusband)
		{
			husband.Dead();
		}
		StartCoroutine(start_phase_4_cr());
	}

	private IEnumerator start_phase_4_cr()
	{
		GetComponent<SpriteRenderer>().material = phase4Material;
		float t = 0f;
		float time = 2.5f;
		Vector3 endPos = new Vector3(base.transform.position.x, 860f);
		Vector2 start = base.transform.position;
		yield return CupheadTime.WaitForSeconds(this, 0.5f);
		if (killedHusband)
		{
			StartCoroutine(husband.move_cr());
		}
		while (t < time)
		{
			float val = EaseUtils.Ease(EaseUtils.EaseType.linear, 0f, 1f, t / time);
			base.transform.position = Vector2.Lerp(start, endPos, val);
			t += (float)CupheadTime.Delta;
			yield return null;
		}
		GetComponent<LevelBossDeathExploder>().StopExplosions();
		GameObject[] array = shadows;
		foreach (GameObject gameObject in array)
		{
			gameObject.SetActive(false);
		}
		base.animator.Play("Phase4_Idle");
		yield return CupheadTime.WaitForSeconds(this, 1f);
		t = 0f;
		time = 1f;
		Vector3 pos = base.transform.position;
		pos.x = -640f + base.transform.GetComponent<Renderer>().bounds.size.x / 2f;
		base.transform.position = pos;
		endPos = new Vector3(base.transform.position.x, phase4Root.position.y);
		start = base.transform.position;
		while (t < time)
		{
			float val2 = EaseUtils.Ease(EaseUtils.EaseType.linear, 0f, 1f, t / time);
			base.transform.position = Vector2.Lerp(start, endPos, val2);
			t += (float)CupheadTime.Delta;
			yield return null;
		}
		StartCoroutine(move_cr());
		StartCoroutine(spawn_roses_cr());
		SpawnUmbrella();
		yield return null;
	}

	private void SpawnUmbrella()
	{
		SallyStagePlayLevelUmbrella sallyStagePlayLevelUmbrella = Object.Instantiate(umbrellaPrefab);
		sallyStagePlayLevelUmbrella.GetProperties(base.properties);
		sallyStagePlayLevelUmbrella.EnableHoming = false;
		float x = ((!Rand.Bool()) ? 140f : (-140f));
		sallyStagePlayLevelUmbrella.transform.position = new Vector2(x, 460f);
		StartCoroutine(umbrella_cr(sallyStagePlayLevelUmbrella));
	}

	private IEnumerator umbrella_cr(SallyStagePlayLevelUmbrella umbrella)
	{
		while (true)
		{
			umbrella.TrackingPlayer = PlayerManager.GetNext();
			yield return CupheadTime.WaitForSeconds(this, base.properties.CurrentState.umbrella.homingUntilSwitchPlayer);
			yield return null;
		}
	}

	private IEnumerator move_cr()
	{
		float t = 0f;
		float time = base.properties.CurrentState.general.finalMovementSpeed;
		float sizeX = base.transform.GetComponent<Renderer>().bounds.size.x / 2f;
		EaseUtils.EaseType ease = EaseUtils.EaseType.easeInOutSine;
		float start = -640f + sizeX;
		float end = 640f - sizeX;
		while (true)
		{
			t = 0f;
			while (t < time)
			{
				TransformExtensions.SetPosition(x: EaseUtils.Ease(ease, start, end, t / time), transform: base.transform);
				t += (float)CupheadTime.Delta;
				yield return null;
			}
			base.transform.SetPosition(end);
			t = 0f;
			while (t < time)
			{
				TransformExtensions.SetPosition(x: EaseUtils.Ease(ease, end, start, t / time), transform: base.transform);
				t += (float)CupheadTime.Delta;
				yield return null;
			}
			base.transform.SetPosition(start);
		}
	}

	private IEnumerator spawn_roses_cr()
	{
		LevelProperties.SallyStagePlay.Roses p = base.properties.CurrentState.roses;
		string[] roseString = p.spawnString.GetRandom().Split(',');
		int roseIndex = Random.Range(0, roseString.Length);
		float yCoord = 460f;
		float xCoord = 0f;
		int maxCount = p.playerAimRange.RandomInt();
		int counter = 0;
		while (true)
		{
			if (counter < maxCount)
			{
				float.TryParse(roseString[roseIndex], out xCoord);
				counter++;
			}
			else
			{
				AbstractPlayerController next = PlayerManager.GetNext();
				xCoord = next.transform.position.x;
				counter = 0;
				maxCount = p.playerAimRange.RandomInt();
			}
			Vector3 position = new Vector3(-640f + xCoord, yCoord);
			applauseHandler.ThrowRose(position, p);
			roseIndex = (roseIndex + 1) % roseString.Length;
			yield return CupheadTime.WaitForSeconds(this, p.spawnDelayRange.RandomFloat());
			yield return null;
		}
	}

	private void OnEasyDeath()
	{
		StopAllCoroutines();
		GetComponent<Collider2D>().enabled = false;
		if (killedHusband)
		{
			husband.GetComponent<Animator>().SetTrigger("OnDeath");
		}
		base.animator.SetTrigger("OnPh3Death");
	}

	private void OnDeath()
	{
		StopAllCoroutines();
		StartCoroutine(sally_angel_death_sound_cr());
		GetComponent<Collider2D>().enabled = false;
		base.animator.SetTrigger("OnPh4Death");
		StartCoroutine(birds_death_cr());
	}

	private IEnumerator sally_angel_death_sound_cr()
	{
		yield return CupheadTime.WaitForSeconds(this, 1.5f);
		AudioManager.Play("sally_p4_angel_death_vox");
	}

	private IEnumerator birds_death_cr()
	{
		float t = 0f;
		float time = 2f;
		Vector3 pos = birdsDeath.transform.position;
		birdsDeath.SetActive(true);
		while (t < time)
		{
			pos.y = Mathf.Lerp(t: EaseUtils.Ease(EaseUtils.EaseType.easeInOutSine, 0f, 1f, t / time), a: pos.y, b: birdRoot.transform.position.y);
			birdsDeath.transform.position = pos;
			t += (float)CupheadTime.Delta;
			yield return null;
		}
		yield return null;
	}

	private void SoundAngelIdle()
	{
		AudioManager.Play("sally_angel_idle");
		emitAudioFromObject.Add("sally_angel_idle");
	}

	private void SoundAngelDeath()
	{
		AudioManager.Play("sally_angel_death");
		emitAudioFromObject.Add("sally_angel_death");
	}
}
