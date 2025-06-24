using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingMermaidLevelMerdusa : LevelProperties.FlyingMermaid.Entity
{
	public enum State
	{
		Intro = 0,
		Idle = 1,
		Zap = 2
	}

	[SerializeField]
	private float introMoveTime;

	[SerializeField]
	private float transformMoveX;

	[SerializeField]
	private SpriteRenderer blinkOverlaySprite;

	[SerializeField]
	private Transform blockingColliders;

	[SerializeField]
	private FlyingMermaidLevelLaser laser;

	[SerializeField]
	private FlyingMermaidLevelEel[] eels;

	[SerializeField]
	private FlyingMermaidLevelMerdusaHead head;

	[SerializeField]
	private FlyingMermaidLevelMerdusaBodyPart bodyPrefab;

	[SerializeField]
	private FlyingMermaidLevelMerdusaBodyPart leftArmPrefab;

	[SerializeField]
	private FlyingMermaidLevelMerdusaBodyPart rightArmPrefab;

	[SerializeField]
	private Transform headRoot;

	[SerializeField]
	private Transform bodyRoot;

	[SerializeField]
	private Transform leftArmRoot;

	[SerializeField]
	private Transform rightArmRoot;

	[SerializeField]
	private Effect splashLeft;

	[SerializeField]
	private Effect splashRight;

	[SerializeField]
	private Transform splashRoot;

	private DamageDealer damageDealer;

	private DamageReceiver damageReceiver;

	private Vector2 startPos;

	private int blinks;

	private int maxBlinks = 3;

	public State state { get; private set; }

	protected override void Awake()
	{
		base.Awake();
		damageDealer = DamageDealer.NewEnemy();
		damageReceiver = GetComponent<DamageReceiver>();
		damageReceiver.OnDamageTaken += OnDamageTaken;
		CollisionChild collisionChild = blockingColliders.gameObject.AddComponent<CollisionChild>();
		collisionChild.OnPlayerCollision += OnCollisionPlayer;
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

	public override void LevelInit(LevelProperties.FlyingMermaid properties)
	{
		base.LevelInit(properties);
		FlyingMermaidLevelEel[] array = eels;
		foreach (FlyingMermaidLevelEel flyingMermaidLevelEel in array)
		{
			flyingMermaidLevelEel.Init(properties.CurrentState.eel);
		}
		properties.OnBossDeath += OnBossDeath;
	}

	public void StartIntro(Vector2 pos)
	{
		AudioManager.Play("level_mermaid_merdusa_cackle");
		base.transform.position = pos;
		base.animator.SetTrigger("Continue");
		StartCoroutine(intro_cr());
		StartCoroutine(moveBack_cr());
	}

	private IEnumerator intro_cr()
	{
		StartEels();
		yield return CupheadTime.WaitForSeconds(this, 1f);
		state = State.Idle;
	}

	private IEnumerator moveBack_cr()
	{
		float startX = base.transform.position.x;
		float t = 0f;
		while (t < introMoveTime)
		{
			t += (float)CupheadTime.Delta;
			base.transform.SetPosition(Mathf.Lerp(startX, startX + transformMoveX, t / introMoveTime));
			yield return null;
		}
	}

	private void BlinkMaybe()
	{
		blinks++;
		if (blinks >= maxBlinks)
		{
			blinks = 0;
			maxBlinks = Random.Range(2, 4);
			blinkOverlaySprite.enabled = true;
		}
		else
		{
			blinkOverlaySprite.enabled = false;
		}
	}

	public void StartEels()
	{
		FlyingMermaidLevelEel[] array = eels;
		foreach (FlyingMermaidLevelEel flyingMermaidLevelEel in array)
		{
			flyingMermaidLevelEel.StartPattern();
		}
	}

	private IEnumerator eels_cr()
	{
		FlyingMermaidLevelEel[] array = eels;
		foreach (FlyingMermaidLevelEel prefab in array)
		{
			prefab.Spawn();
		}
		bool allEelsGone = false;
		while (!allEelsGone)
		{
			allEelsGone = true;
			FlyingMermaidLevelEel[] array2 = eels;
			foreach (FlyingMermaidLevelEel flyingMermaidLevelEel in array2)
			{
				if (flyingMermaidLevelEel.state == FlyingMermaidLevelEel.State.Spawned)
				{
					allEelsGone = false;
				}
			}
			yield return null;
		}
		yield return CupheadTime.WaitForSeconds(this, base.properties.CurrentState.eel.hesitateAfterAttack);
		state = State.Idle;
	}

	public void StartZap()
	{
		state = State.Zap;
		StartCoroutine(zap_cr());
	}

	private IEnumerator zap_cr()
	{
		AudioManager.Play("level_mermaid_merdusa_zap_loop_start");
		base.animator.SetTrigger("Zap");
		yield return base.animator.WaitForAnimationToEnd(this, "Zap_Start");
		laser.SetStoneTime(base.properties.CurrentState.zap.stoneTime);
		laser.animator.SetTrigger("Start");
		laser.transform.SetParent(null);
		AudioManager.PlayLoop("level_mermaid_merdusa_zap_loop");
		laser.StartLaser();
		yield return laser.animator.WaitForAnimationToEnd(this, "Lightning_Start");
		laser.animator.SetTrigger("End");
		AudioManager.Stop("level_mermaid_merdusa_zap_loop");
		AudioManager.Play("level_mermaid_merdusa_zap_loop_end");
		laser.StopLaser();
		base.animator.SetTrigger("Continue");
		yield return base.animator.WaitForAnimationToEnd(this, "Zap_End");
		yield return CupheadTime.WaitForSeconds(this, base.properties.CurrentState.zap.hesitateAfterAttack.RandomFloat());
		state = State.Idle;
	}

	public void StartTransform()
	{
		if (state == State.Zap)
		{
			AudioManager.Play("level_mermaid_merdusa_zap_loop_end");
			laser.StopLaser();
		}
		AudioManager.Stop("level_mermaid_merdusa_zap_loop");
		head.StartIntro(headRoot.position);
		base.properties.OnBossDeath -= OnBossDeath;
		Die();
	}

	private void Die()
	{
		List<FlyingMermaidLevelMerdusaBodyPart> list = new List<FlyingMermaidLevelMerdusaBodyPart>();
		StopAllCoroutines();
		AudioManager.Play("level_mermaid_merdusa_fallapart_turnstone");
		list.Add(bodyPrefab.Create(bodyRoot.position));
		list.Add(leftArmPrefab.Create(leftArmRoot.position));
		list.Add(rightArmPrefab.Create(rightArmRoot.position));
		head.CheckParts(list.ToArray());
		StopAllCoroutines();
		CupheadLevelCamera.Current.Shake(20f, 0.7f);
		FlyingMermaidLevelEel[] array = eels;
		foreach (FlyingMermaidLevelEel flyingMermaidLevelEel in array)
		{
			flyingMermaidLevelEel.Die(true, true);
		}
		Object.Destroy(base.gameObject);
	}

	private void DieEasyMode()
	{
		StopAllCoroutines();
		base.animator.SetTrigger("Die");
		FlyingMermaidLevelEel[] array = eels;
		foreach (FlyingMermaidLevelEel flyingMermaidLevelEel in array)
		{
			flyingMermaidLevelEel.Die(true, true);
		}
	}

	private void OnBossDeath()
	{
		if (Level.CurrentMode == Level.Mode.Easy)
		{
			DieEasyMode();
		}
		else
		{
			Die();
		}
	}

	private void RightSplash()
	{
		splashRight.Create(splashRoot.transform.position);
	}

	private void LeftSplash()
	{
		splashLeft.Create(splashRoot.transform.position);
	}
}
