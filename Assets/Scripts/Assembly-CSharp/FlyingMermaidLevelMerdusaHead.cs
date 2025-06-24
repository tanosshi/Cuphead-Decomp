using System;
using System.Collections;
using UnityEngine;

public class FlyingMermaidLevelMerdusaHead : LevelProperties.FlyingMermaid.Entity
{
	public enum State
	{
		Intro = 0,
		Idle = 1,
		HeadBlast = 2,
		Bubble = 3,
		Both = 4,
		Dead = 5
	}

	private const float PillarTopA = 0f;

	private const float PillarTopB = 0.25f;

	private const float PillarBottom = 0.5f;

	private const float PillarPlain = 0.75f;

	private const float PillarSingle = 1f;

	private const string PillarParameterName = "PillarType";

	private const string PillarStateName = "Pillar";

	[SerializeField]
	private BasicProjectile yellowDot;

	[SerializeField]
	private SpriteRenderer wave1;

	[SerializeField]
	private SpriteRenderer wave2;

	[SerializeField]
	private ScrollingSpriteSpawner[] scrollingSpritesToEnd;

	[SerializeField]
	private ScrollingSpriteSpawner[] scrollingSprites;

	[SerializeField]
	private FlyingMermaidLevelBackgroundChange coral;

	[SerializeField]
	private Transform snakeRoot;

	[SerializeField]
	private Transform eyebeamRoot;

	[SerializeField]
	private FlyingMermaidLevelSkullBubble bubblePrefab;

	[SerializeField]
	private BasicProjectile heatBlastPrefab;

	[SerializeField]
	private float xPosition;

	[SerializeField]
	private float headBackMoveTime;

	private DamageDealer damageDealer;

	private DamageReceiver damageReceiver;

	private Coroutine patternCoroutine;

	public State state { get; private set; }

	protected override void Awake()
	{
		base.Awake();
		damageDealer = DamageDealer.NewEnemy();
		damageReceiver = GetComponent<DamageReceiver>();
		damageReceiver.OnDamageTaken += OnDamageTaken;
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

	public void StartIntro(Vector2 pos)
	{
		base.transform.position = pos;
		base.properties.OnBossDeath += OnBossDeath;
		StartCoroutine(intro_cr());
	}

	public void CheckParts(FlyingMermaidLevelMerdusaBodyPart[] parts)
	{
		StartCoroutine(check_parts_cr(parts));
	}

	private IEnumerator check_parts_cr(FlyingMermaidLevelMerdusaBodyPart[] parts)
	{
		foreach (FlyingMermaidLevelMerdusaBodyPart part in parts)
		{
			while (!part.IsSinking)
			{
				yield return null;
			}
		}
		coral.speed = base.properties.CurrentState.coral.coralMoveSpeed;
		SpriteRenderer[] componentsInChildren = wave1.GetComponentsInChildren<SpriteRenderer>();
		foreach (SpriteRenderer spriteRenderer in componentsInChildren)
		{
			spriteRenderer.sortingLayerName = SpriteLayer.Background.ToString();
			spriteRenderer.sortingOrder = 100;
		}
		SpriteRenderer[] componentsInChildren2 = wave2.GetComponentsInChildren<SpriteRenderer>();
		foreach (SpriteRenderer spriteRenderer2 in componentsInChildren2)
		{
			spriteRenderer2.sortingLayerName = SpriteLayer.Background.ToString();
			spriteRenderer2.sortingOrder = 101;
		}
		ScrollingSpriteSpawner[] array = scrollingSpritesToEnd;
		foreach (ScrollingSpriteSpawner scrollingSpriteSpawner in array)
		{
			scrollingSpriteSpawner.HandlePausing(true);
		}
		ScrollingSpriteSpawner[] array2 = scrollingSprites;
		foreach (ScrollingSpriteSpawner scrollingSpriteSpawner2 in array2)
		{
			scrollingSpriteSpawner2.StartLoop();
		}
		StartCoroutine(move_head_cr());
		state = State.Idle;
		StartCoroutine(spawn_yellow_dots_cr());
	}

	public override void LevelInit(LevelProperties.FlyingMermaid properties)
	{
		base.LevelInit(properties);
	}

	private void OnBossDeath()
	{
		StopAllCoroutines();
		base.animator.Play("Death");
		state = State.Dead;
	}

	private IEnumerator intro_cr()
	{
		state = State.Intro;
		Level.Current.SetBounds(null, null, null, 300);
		yield return null;
	}

	private IEnumerator move_head_cr()
	{
		Vector2 pos = base.transform.position;
		YieldInstruction wait = new WaitForFixedUpdate();
		float offset = xPosition - pos.x;
		while (true)
		{
			float targetXDistance = float.MaxValue;
			float targetY = 0f;
			foreach (Transform point in coral.points)
			{
				float num = point.position.x - pos.x;
				if (num > 0f && num < targetXDistance)
				{
					targetXDistance = num;
					targetY = point.position.y;
				}
			}
			float t = 0f;
			float time = targetXDistance / coral.speed;
			float startY = pos.y;
			while (t < time)
			{
				if (base.transform.position.x < xPosition)
				{
					pos.x += offset * (CupheadTime.FixedDelta / headBackMoveTime);
				}
				else
				{
					pos.x = xPosition;
				}
				t += CupheadTime.FixedDelta;
				pos.y = EaseUtils.EaseInOutSine(startY, targetY, t / time);
				base.transform.position = pos;
				yield return wait;
			}
			yield return wait;
		}
	}

	public void StartBubble()
	{
		if (patternCoroutine != null)
		{
			StopCoroutine(patternCoroutine);
		}
		patternCoroutine = StartCoroutine(bubble_cr());
	}

	public void StartHeadBlast()
	{
		if (patternCoroutine != null)
		{
			StopCoroutine(patternCoroutine);
		}
		patternCoroutine = StartCoroutine(head_blast_cr());
	}

	public void StartHeadBubble()
	{
		if (patternCoroutine != null)
		{
			StopCoroutine(patternCoroutine);
		}
		patternCoroutine = StartCoroutine(head_blast_bubble_cr());
	}

	private IEnumerator bubble_cr()
	{
		state = State.Bubble;
		base.animator.SetTrigger("OnSnakeATK");
		yield return base.animator.WaitForAnimationToEnd(this, "Snake_Attack");
		yield return CupheadTime.WaitForSeconds(this, base.properties.CurrentState.bubbles.attackDelayRange.RandomFloat());
		state = State.Idle;
		yield return null;
	}

	private IEnumerator head_blast_cr()
	{
		state = State.HeadBlast;
		base.animator.SetTrigger("OnEyewave");
		yield return base.animator.WaitForAnimationToEnd(this, "Eyewave_Attack");
		yield return CupheadTime.WaitForSeconds(this, base.properties.CurrentState.headBlast.attackDelayRange.RandomFloat());
		state = State.Idle;
		yield return null;
	}

	private IEnumerator head_blast_bubble_cr()
	{
		state = State.Both;
		base.animator.SetTrigger("OnBoth");
		yield return base.animator.WaitForAnimationToEnd(this, "Snake_Eyewave");
		yield return CupheadTime.WaitForSeconds(this, base.properties.CurrentState.coral.bubbleEyewaveSpawnDelayRange.RandomFloat());
		state = State.Idle;
		yield return null;
	}

	private void SpawnBubble()
	{
		AbstractPlayerController next = PlayerManager.GetNext();
		Vector3 vector = next.transform.position - base.transform.position;
		LevelProperties.FlyingMermaid.Bubbles bubbles = base.properties.CurrentState.bubbles;
		bubblePrefab.Create(snakeRoot.transform.position, bubbles.movementSpeed, bubbles.waveSpeed, bubbles.waveAmount, MathUtils.DirectionToAngle(vector), bubbles);
	}

	private void SpawnHeadBlast()
	{
		BasicProjectile basicProjectile = heatBlastPrefab.Create(eyebeamRoot.transform.position, 0f, 0f - base.properties.CurrentState.headBlast.movementSpeed);
		basicProjectile.GetComponent<FlyingMermaidLevelLaser>().SetStoneTime(base.properties.CurrentState.zap.stoneTime);
	}

	private IEnumerator spawn_yellow_dots_cr()
	{
		float xPos = 690f;
		LevelProperties.FlyingMermaid.Coral p = base.properties.CurrentState.coral;
		int mainIndex = UnityEngine.Random.Range(0, p.yellowDotPosString.Length);
		string[] yPosString = p.yellowDotPosString[mainIndex].Split(',');
		while (true)
		{
			yPosString = p.yellowDotPosString[mainIndex].Split(',');
			yield return CupheadTime.WaitForSeconds(this, base.properties.CurrentState.coral.yellowSpawnDelayRange.RandomFloat());
			float[] yPos = new float[yPosString.Length];
			for (int i = 0; i < yPosString.Length; i++)
			{
				yPos[i] = float.Parse(yPosString[i]);
			}
			Array.Sort(yPos);
			for (int j = 0; j < yPosString.Length; j++)
			{
				Vector3 vector = new Vector3(xPos, base.transform.position.y - 20f + yPos[j]);
				BasicProjectile basicProjectile = yellowDot.Create(vector, 0f, 0f - p.coralMoveSpeed);
				if (yPosString.Length == 1)
				{
					basicProjectile.animator.SetFloat("PillarType", 1f);
				}
				else if (j == 0)
				{
					basicProjectile.animator.SetFloat("PillarType", 0.5f);
				}
				else if (j == yPosString.Length - 1)
				{
					basicProjectile.animator.SetFloat("PillarType", (!Rand.Bool()) ? 0.25f : 0f);
				}
				else
				{
					basicProjectile.animator.SetFloat("PillarType", 0.75f);
				}
				basicProjectile.animator.Play("Pillar", 0, UnityEngine.Random.value);
				basicProjectile.GetComponent<SpriteRenderer>().sortingOrder = j;
			}
			mainIndex = (mainIndex + 1) % p.yellowDotPosString.Length;
			yield return null;
		}
	}

	private void SoundMermaidPhase3GhostShoot()
	{
		AudioManager.Play("level_mermaid_phase3_ghostshoot");
		emitAudioFromObject.Add("level_mermaid_phase3_ghostshoot");
	}

	private void SoundMermaidPhase3SnakeShoot()
	{
		AudioManager.Play("level_mermaid_phase3_snakeshoot");
		emitAudioFromObject.Add("level_mermaid_phase3_snakeshoot");
	}
}
