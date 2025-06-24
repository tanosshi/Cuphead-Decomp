using System.Collections;
using UnityEngine;

public class BeeLevelQueen : LevelProperties.Bee.Entity
{
	public enum State
	{
		Intro = 0,
		Idle = 1,
		BlackHole = 2,
		Triangle = 3,
		Follower = 4,
		Chain = 5,
		Death = 6
	}

	private enum AttackAnimations
	{
		BlackHole = 0,
		Triangle = 1,
		Follower = 2
	}

	private enum Triggers
	{
		Continue = 0
	}

	private enum Integers
	{
		Attack = 0
	}

	private enum Bools
	{
		Repeat = 0
	}

	public const float SPELL_X = 290f;

	[SerializeField]
	private BeeLevelAirplane airplane;

	[SerializeField]
	private Transform bottomHoney;

	[SerializeField]
	private Effect puff;

	[Space(5f)]
	[SerializeField]
	private Transform[] puffRoots;

	[Space(5f)]
	[SerializeField]
	private GameObject head;

	[SerializeField]
	private GameObject body;

	[SerializeField]
	private GameObject chain;

	[Space(10f)]
	[SerializeField]
	private BeeLevelQueenSpitProjectile spitPrefab;

	[SerializeField]
	private Transform spitRoot;

	[Space(10f)]
	[SerializeField]
	private BeeLevelQueenBlackHole blackHolePrefab;

	[SerializeField]
	private Transform[] blackHoleRoots;

	[Space(10f)]
	[SerializeField]
	private BeeLevelQueenTriangle trianglePrefab;

	[SerializeField]
	private BeeLevelQueenTriangle triangleInvinciblePrefab;

	[Space(10f)]
	[SerializeField]
	private float followerRadius = 200f;

	[SerializeField]
	private Transform followerRoot;

	[SerializeField]
	private BeeLevelQueenFollower followerPrefab;

	[Space(10f)]
	[SerializeField]
	private Effect dustEffect;

	[SerializeField]
	private Effect sparkEffect;

	private new Animator animator;

	private DamageReceiver damageReceiver;

	private DamageDealer damageDealer;

	private LevelProperties.Bee.Chain currentChain;

	public State state { get; private set; }

	protected override void Awake()
	{
		base.Awake();
		animator = GetComponent<Animator>();
		RegisterCollisionChild(head.gameObject);
		EnableBody(false);
		damageReceiver = GetComponent<DamageReceiver>();
		damageReceiver.OnDamageTaken += OnDamageTaken;
		damageDealer = DamageDealer.NewEnemy();
	}

	protected override void Start()
	{
		base.Start();
		AudioManager.Play("bee_queen_intro_vocal");
		emitAudioFromObject.Add("bee_queen_intro_vocal");
		Level.Current.OnIntroEvent += OnIntro;
	}

	private void OnIntro()
	{
		StartCoroutine(intro_cr());
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
		if (damageDealer != null)
		{
			damageDealer.DealDamage(hit);
		}
	}

	protected override void OnDrawGizmosSelected()
	{
		base.OnDrawGizmosSelected();
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere((Vector2)followerRoot.position, followerRadius);
	}

	private void OnDamageTaken(DamageDealer.DamageInfo info)
	{
		base.properties.DealDamage(info.damage);
		if (base.properties.CurrentHealth <= 0f && state != State.Death)
		{
			state = State.Death;
			Death();
		}
	}

	private void EnableBody(bool p)
	{
		head.SetActive(p);
		body.SetActive(p);
		chain.SetActive(p);
	}

	private void MagicEffect()
	{
		Transform transform = dustEffect.Create(base.transform.position).transform;
		Transform transform2 = sparkEffect.Create(base.transform.position).transform;
		transform.SetParent(base.transform);
		transform2.SetParent(base.transform);
		transform.ResetLocalTransforms();
		transform2.ResetLocalTransforms();
	}

	private IEnumerator intro_cr()
	{
		SetTrigger(Triggers.Continue);
		state = State.Idle;
		yield return null;
	}

	private void SfxIntroKnife()
	{
		AudioManager.Play("bee_queen_intro_cutlery");
		emitAudioFromObject.Add("bee_queen_intro_cutlery");
	}

	private void SfxIntroSnap()
	{
		AudioManager.Play("bee_queen_intro_finger_click");
		emitAudioFromObject.Add("bee_queen_intro_finger_click");
	}

	public void StartChain()
	{
		state = State.Chain;
		StartCoroutine(chain_cr());
	}

	private void FireChainStartSFX()
	{
		AudioManager.Play("bee_queen_chain_head_spit_start");
		emitAudioFromObject.Add("bee_queen_chain_head_spit_start");
	}

	private void FireChainProjectile()
	{
		AudioManager.Play("bee_queen_chain_head_spit_attack");
		emitAudioFromObject.Add("bee_queen_chain_head_spit_attack");
		spitPrefab.Create(spitRoot.position, new Vector2(base.transform.localScale.x, 1f), currentChain.speed, new Vector2(currentChain.timeX, currentChain.timeY));
	}

	private void ChainFlip()
	{
		base.transform.SetScale(base.transform.localScale.x * -1f, 1f, 1f);
	}

	private IEnumerator chain_cr()
	{
		currentChain = base.properties.CurrentState.chain;
		base.transform.ResetLocalTransforms();
		base.transform.SetPosition(0f, 550f, 0f);
		EnableBody(true);
		SetBool(Bools.Repeat, true);
		animator.Play("Chain_Idle");
		animator.Play("Head_Closed_Idle", animator.GetLayerIndex("Head"));
		yield return StartCoroutine(tween_cr(base.transform, base.transform.position, new Vector2(0f, 300f), EaseUtils.EaseType.easeOutQuart, 0.6f));
		AudioManager.Play("bee_queen_chain_ascend_vocal");
		emitAudioFromObject.Add("bee_queen_chain_ascend_vocal");
		AudioManager.Play("bee_queen_chain_head_ascend");
		emitAudioFromObject.Add("bee_queen_chain_head_ascend");
		StartCoroutine(tween_cr(chain.transform, chain.transform.position, new Vector2(0f, -100f), EaseUtils.EaseType.easeInQuart, 0.6f));
		yield return StartCoroutine(tween_cr(head.transform, head.transform.position, new Vector2(0f, -100f), EaseUtils.EaseType.easeInQuart, 0.6f));
		CupheadLevelCamera.Current.Shake(20f, 0.7f);
		yield return CupheadTime.WaitForSeconds(this, 0.7f);
		animator.Play("Spit_Start", animator.GetLayerIndex("Head"));
		yield return CupheadTime.WaitForSeconds(this, 1f);
		if (!base.properties.CurrentState.chain.chainForever)
		{
			for (int i = 0; i < currentChain.count; i++)
			{
				AudioManager.Play("bee_chain_head_spit_delay");
				emitAudioFromObject.Add("bee_chain_head_spit_delay");
				yield return CupheadTime.WaitForSeconds(this, currentChain.delay);
				if (i >= currentChain.count - 1)
				{
					SetBool(Bools.Repeat, false);
				}
				SetTrigger(Triggers.Continue);
			}
			yield return animator.WaitForAnimationToEnd(this, "Spit_Attack_End", animator.GetLayerIndex("Head"));
			AudioManager.Play("bee_queen_chain_head_decend");
			emitAudioFromObject.Add("bee_queen_chain_head_decend");
			StartCoroutine(tween_cr(chain.transform, chain.transform.position, new Vector2(0f, 300f), EaseUtils.EaseType.easeInQuart, 0.6f));
			yield return StartCoroutine(tween_cr(head.transform, head.transform.position, new Vector2(0f, 300f), EaseUtils.EaseType.easeInQuart, 0.6f));
			CupheadLevelCamera.Current.Shake(20f, 0.7f);
			yield return CupheadTime.WaitForSeconds(this, 0.7f);
			yield return StartCoroutine(tween_cr(base.transform, base.transform.position, new Vector2(0f, 550f), EaseUtils.EaseType.easeInQuart, 0.6f));
			EnableBody(false);
			yield return CupheadTime.WaitForSeconds(this, currentChain.hesitate);
			state = State.Idle;
			yield break;
		}
		while (true)
		{
			AudioManager.Play("bee_chain_head_spit_delay");
			emitAudioFromObject.Add("bee_chain_head_spit_delay");
			yield return CupheadTime.WaitForSeconds(this, currentChain.delay);
			SetTrigger(Triggers.Continue);
			yield return null;
		}
	}

	public void StartBlackHole()
	{
		state = State.BlackHole;
		StartCoroutine(blackHole_cr());
	}

	private IEnumerator blackHole_cr()
	{
		base.transform.ResetLocalTransforms();
		base.transform.SetScale(MathUtils.PlusOrMinus(), 1f, 1f);
		base.transform.SetPosition(290f * base.transform.localScale.x);
		animator.Play("Warning");
		yield return animator.WaitForAnimationToEnd(this, "Warning");
		ClearTrigger(Triggers.Continue);
		SetAttackAnim(AttackAnimations.BlackHole);
		animator.Play("Spell_Start");
		LevelProperties.Bee.BlackHole properties = base.properties.CurrentState.blackHole;
		string[] patternStrings = properties.patterns[Random.Range(0, properties.patterns.Length)].Split(',');
		int[] patternArray = new int[patternStrings.Length];
		for (int i = 0; i < patternStrings.Length; i++)
		{
			int.TryParse(patternStrings[i], out patternArray[i]);
			patternArray[i] = Mathf.Clamp(patternArray[i], 0, 2);
		}
		int i2 = 0;
		int count = patternArray.Length;
		yield return animator.WaitForAnimationToEnd(this, "Spell_Start");
		AudioManager.PlayLoop("bee_queen_spell_shake_loop");
		emitAudioFromObject.Add("bee_queen_spell_shake_loop");
		while (i2 < count)
		{
			yield return CupheadTime.WaitForSeconds(this, properties.chargeTime);
			SetTrigger(Triggers.Continue);
			yield return animator.WaitForAnimationToEnd(this, "Spell_Charge_End");
			yield return animator.WaitForAnimationToEnd(this, "Spell_Attack_Start");
			BeeLevelQueenBlackHole b = blackHolePrefab.Create(blackHoleRoots[patternArray[i2]].position) as BeeLevelQueenBlackHole;
			b.speed = properties.speed;
			b.health = properties.health;
			b.childDelay = properties.childDelay;
			b.childSpeed = properties.childSpeed;
			if (properties.damageable)
			{
				b.gameObject.AddComponent<Rigidbody2D>();
			}
			yield return CupheadTime.WaitForSeconds(this, properties.attackTime);
			i2++;
			SetBool(Bools.Repeat, i2 != count);
			SetTrigger(Triggers.Continue);
		}
		yield return animator.WaitForAnimationToEnd(this, "Spell_End");
		AudioManager.Stop("bee_queen_spell_shake_loop");
		base.transform.SetPosition(0f);
		yield return CupheadTime.WaitForSeconds(this, properties.hesitate);
		state = State.Idle;
	}

	public void StartTriangle()
	{
		state = State.Triangle;
		StartCoroutine(triangle_cr());
	}

	private IEnumerator triangle_cr()
	{
		base.transform.ResetLocalTransforms();
		base.transform.SetScale(MathUtils.PlusOrMinus(), 1f, 1f);
		base.transform.SetPosition(290f * base.transform.localScale.x);
		animator.Play("Warning");
		yield return animator.WaitForAnimationToEnd(this, "Warning");
		ClearTrigger(Triggers.Continue);
		SetAttackAnim(AttackAnimations.Triangle);
		animator.Play("Spell_Start");
		SetBool(Bools.Repeat, false);
		LevelProperties.Bee.Triangle properties = base.properties.CurrentState.triangle;
		yield return animator.WaitForAnimationToEnd(this, "Spell_Start");
		AudioManager.PlayLoop("bee_queen_spell_shake_loop");
		emitAudioFromObject.Add("bee_queen_spell_shake_loop");
		int i = 0;
		while (i < properties.count)
		{
			yield return CupheadTime.WaitForSeconds(this, properties.chargeTime);
			SetTrigger(Triggers.Continue);
			yield return animator.WaitForAnimationToEnd(this, "Spell_Charge_End");
			yield return animator.WaitForAnimationToEnd(this, "Spell_Attack_Start");
			BeeLevelQueenTriangle.Properties p = new BeeLevelQueenTriangle.Properties(PlayerManager.GetNext(), properties.introTime, properties.speed, properties.rotationSpeed, properties.health, properties.childSpeed, properties.childDelay, properties.childHealth, properties.childCount, properties.damageable);
			if (properties.damageable)
			{
				trianglePrefab.Create(p);
			}
			else
			{
				triangleInvinciblePrefab.Create(p);
			}
			yield return CupheadTime.WaitForSeconds(this, properties.attackTime);
			i++;
			SetBool(Bools.Repeat, i != properties.count);
			SetTrigger(Triggers.Continue);
		}
		yield return animator.WaitForAnimationToEnd(this, "Spell_End");
		AudioManager.Stop("bee_queen_spell_shake_loop");
		base.transform.SetPosition(0f);
		yield return CupheadTime.WaitForSeconds(this, properties.hesitate);
		state = State.Idle;
	}

	public void StartMorph()
	{
		StartCoroutine(morph_cr());
	}

	private IEnumerator morph_cr()
	{
		float t = 0f;
		float time = 2.5f;
		float moveSpeed = 0f;
		animator.Play("Warning_Trans");
		yield return animator.WaitForAnimationToEnd(this, "Warning_Trans");
		AudioManager.PlayLoop("bee_queen_spell_antic");
		emitAudioFromObject.Add("bee_queen_spell_antic");
		Vector3 endPos = new Vector3(0f, 230f);
		Vector3 startPos = base.transform.position;
		while (t < time)
		{
			float val = EaseUtils.Ease(EaseUtils.EaseType.linear, 0f, 1f, t / time);
			base.transform.position = Vector2.Lerp(startPos, endPos, val);
			t += (float)CupheadTime.Delta;
			yield return null;
		}
		base.transform.position = endPos;
		AudioManager.Stop("bee_queen_spell_antic");
		animator.SetTrigger("Continue");
		yield return animator.WaitForAnimationToEnd(this, "Morph_Morph");
		yield return CupheadTime.WaitForSeconds(this, 0.54f);
		t = 0f;
		while (t < 0.76f)
		{
			moveSpeed = ((!(t < 0.3f)) ? 300f : 800f);
			base.transform.position += Vector3.up * moveSpeed * CupheadTime.Delta;
			t += (float)CupheadTime.Delta;
			yield return null;
		}
		t = 0f;
		time = 0.67f;
		startPos = base.transform.position;
		endPos = new Vector3(0f, -960f);
		StartCoroutine(spawn_puffs_cr());
		while (t < time)
		{
			float val2 = EaseUtils.Ease(EaseUtils.EaseType.linear, 0f, 1f, t / time);
			base.transform.position = Vector2.Lerp(startPos, endPos, val2);
			t += (float)CupheadTime.Delta;
			yield return null;
		}
		airplane.StartIntro();
		Object.Destroy(base.gameObject);
	}

	private void SnapPosition()
	{
		base.transform.GetComponent<SpriteRenderer>().sortingLayerName = SpriteLayer.Player.ToString();
		base.transform.GetComponent<SpriteRenderer>().sortingOrder = 100;
		base.transform.position = new Vector3(0f, 960f);
	}

	private void MoveHoney()
	{
		StartCoroutine(move_honey_cr());
		StartCoroutine(move_bee_cr());
	}

	private IEnumerator move_honey_cr()
	{
		float t = 0f;
		float time = 2.5f;
		Vector3 startPos = bottomHoney.transform.position;
		Vector3 endPos = new Vector3(0f, -560f);
		while (t < time)
		{
			float val = EaseUtils.Ease(EaseUtils.EaseType.linear, 0f, 1f, t / time);
			bottomHoney.transform.position = Vector2.Lerp(startPos, endPos, val);
			t += (float)CupheadTime.Delta;
			yield return null;
		}
		bottomHoney.transform.position = endPos;
		StartCoroutine(CupheadLevelCamera.Current.change_zoom_cr(0.97f, 2f));
		yield return null;
	}

	private IEnumerator move_bee_cr()
	{
		float t = 0f;
		float time = 3f;
		while (t < time)
		{
			base.transform.position += Vector3.up * 50f * CupheadTime.Delta;
			t += (float)CupheadTime.Delta;
			yield return null;
		}
		yield return null;
	}

	private IEnumerator spawn_puffs_cr()
	{
		Transform[] array = puffRoots;
		foreach (Transform root in array)
		{
			puff.Create(root.position);
			yield return CupheadTime.WaitForSeconds(this, 0.134f);
		}
		yield return null;
	}

	public void StartFollower()
	{
		state = State.Triangle;
		StartCoroutine(follower_cr());
	}

	private IEnumerator follower_cr()
	{
		base.transform.ResetLocalTransforms();
		base.transform.SetScale(MathUtils.PlusOrMinus(), 1f, 1f);
		base.transform.SetPosition(290f * base.transform.localScale.x);
		animator.Play("Warning");
		yield return animator.WaitForAnimationToEnd(this, "Warning");
		ClearTrigger(Triggers.Continue);
		SetAttackAnim(AttackAnimations.Follower);
		animator.Play("Spell_Start");
		SetBool(Bools.Repeat, false);
		LevelProperties.Bee.Follower properties = base.properties.CurrentState.follower;
		yield return animator.WaitForAnimationToEnd(this, "Spell_Start");
		int i = 0;
		while (i < properties.count)
		{
			yield return CupheadTime.WaitForSeconds(this, properties.chargeTime);
			SetTrigger(Triggers.Continue);
			yield return animator.WaitForAnimationToEnd(this, "Spell_Charge_End");
			yield return animator.WaitForAnimationToEnd(this, "Spell_Attack_Start");
			Vector2 pos = (Vector2)followerRoot.position + new Vector2(Random.Range(-1, 1), Random.Range(-1, 1)).normalized * (followerRadius * Random.value);
			BeeLevelQueenFollower.Properties p = new BeeLevelQueenFollower.Properties(PlayerManager.GetNext(), properties.introTime, properties.homingSpeed, properties.homingRotation, properties.homingTime, properties.health, properties.childDelay, properties.childHealth, properties.parryable);
			if (properties.damageable)
			{
				followerPrefab.Create(pos, p).gameObject.AddComponent<Rigidbody2D>().isKinematic = true;
			}
			else
			{
				followerPrefab.Create(pos, p);
			}
			yield return CupheadTime.WaitForSeconds(this, properties.attackTime);
			i++;
			SetBool(Bools.Repeat, i != properties.count);
			SetTrigger(Triggers.Continue);
		}
		yield return animator.WaitForAnimationToEnd(this, "Spell_End");
		base.transform.SetPosition(0f);
		yield return CupheadTime.WaitForSeconds(this, properties.hesitate);
		state = State.Idle;
	}

	private IEnumerator tween_cr(Transform trans, Vector2 start, Vector2 end, EaseUtils.EaseType ease, float time)
	{
		float t = 0f;
		trans.position = start;
		while (t < time)
		{
			float val = EaseUtils.Ease(ease, 0f, 1f, t / time);
			trans.position = Vector2.Lerp(start, end, val);
			t += (float)CupheadTime.Delta;
			yield return null;
		}
		trans.position = end;
		yield return null;
	}

	private void SetAttackAnim(AttackAnimations a)
	{
		SetInt(Integers.Attack, (int)a);
	}

	private void SetTrigger(Triggers t)
	{
		animator.SetTrigger(t.ToString());
	}

	private void ClearTrigger(Triggers t)
	{
		animator.ResetTrigger(t.ToString());
	}

	private void SetInt(Integers i, int value)
	{
		animator.SetInteger(i.ToString(), value);
	}

	private void SetBool(Bools b, bool value)
	{
		animator.SetBool(b.ToString(), value);
	}

	private void Death()
	{
		animator.Play("Head_Closed_Idle");
		StopAllCoroutines();
	}

	private void SpellTossSFX()
	{
		AudioManager.Play("bee_queen_spell_toss");
		emitAudioFromObject.Add("bee_queen_spell_toss");
	}

	private void SpellCastSFX()
	{
		AudioManager.Play("bee_queen_spell_cast");
		emitAudioFromObject.Add("bee_queen_spell_cast");
	}

	private void AttackStartSFX()
	{
		AudioManager.Play("bee_queen_attack_start");
		emitAudioFromObject.Add("bee_queen_attack_start");
		AudioManager.PlayLoop("bee_queen_attack_loop");
	}

	private void AttackEndSFX()
	{
		AudioManager.Stop("bee_queen_attack_loop");
		AudioManager.Play("bee_queen_attack_end");
		emitAudioFromObject.Add("bee_queen_attack_end");
	}

	private void WarningSFX()
	{
		AudioManager.Play("bee_queen_warning");
		emitAudioFromObject.Add("bee_queen_warning");
	}

	private void FlyDownSFX()
	{
		AudioManager.Play("bee_airplane_fly_down");
		emitAudioFromObject.Add("bee_airplane_fly_down");
	}
}
