using System.Collections;
using UnityEngine;

public class MouseLevelCat : LevelProperties.Mouse.Entity
{
	public enum State
	{
		Init = 0,
		Idle = 1,
		Claw = 2,
		GhostMouse = 3,
		Dying = 4
	}

	private const string EnemiesLayerName = "Enemies";

	private const int BODY_LAYER = 0;

	private const int TAIL_LAYER = 1;

	private const int HEAD_LAYER = 2;

	[SerializeField]
	private Vector2 startPosition;

	[SerializeField]
	private MouseLevelBrokenCanMouse mouse;

	[SerializeField]
	private Animator wallAnimator;

	[SerializeField]
	private LevelPlatform wallPlatform;

	[SerializeField]
	private GameObject foreground;

	[SerializeField]
	private GameObject alternateForeground;

	[SerializeField]
	private GameObject[] toDestroyOnWallBreakStart;

	[SerializeField]
	private GameObject[] toDestroyOnWallBreakEnd;

	[SerializeField]
	private SpriteRenderer blinkOverlaySprite;

	[SerializeField]
	private Transform head;

	[SerializeField]
	private MouseLevelCatPaw leftPaw;

	[SerializeField]
	private MouseLevelCatPaw rightPaw;

	[SerializeField]
	private Transform headMoveTransform;

	[SerializeField]
	private MouseLevelFallingObject[] fallingObjectPrefabs;

	[SerializeField]
	private MouseLevelGhostMouse[] twoGhostMice;

	[SerializeField]
	private MouseLevelGhostMouse[] fourGhostMice;

	[SerializeField]
	private SpriteRenderer headFrontRenderer;

	private DamageReceiver damageReceiver;

	private Vector2 headStartPos;

	private int fallingObjectsIndex;

	private int blinks;

	private int maxBlinks = 8;

	private bool alreadyManagingGhostMice;

	public State state { get; private set; }

	protected override void Awake()
	{
		base.Awake();
		damageReceiver = GetComponent<DamageReceiver>();
		damageReceiver.OnDamageTaken += OnDamageTaken;
		headStartPos = head.localPosition;
	}

	public override void LevelInit(LevelProperties.Mouse properties)
	{
		base.LevelInit(properties);
		fallingObjectsIndex = Random.Range(0, properties.CurrentState.claw.fallingObjectStrings.Length);
	}

	private void OnDamageTaken(DamageDealer.DamageInfo info)
	{
		base.properties.DealDamage(info.damage);
	}

	public void StartIntro()
	{
		base.properties.OnBossDeath += OnBossDeath;
		StartCoroutine(intro_cr());
	}

	private IEnumerator intro_cr()
	{
		yield return CupheadTime.WaitForSeconds(this, 1.5f);
		base.transform.position = startPosition;
		base.animator.SetTrigger("StartIntro");
		yield return base.animator.WaitForAnimationToEnd(this, "Intro", 2);
		state = State.Idle;
	}

	public void EatMouse()
	{
		mouse.BeEaten();
		StartCoroutine(tail_cr());
	}

	public void StartWallBreak()
	{
		wallAnimator.SetTrigger("OnContinue");
		GameObject[] array = toDestroyOnWallBreakStart;
		foreach (GameObject obj in array)
		{
			Object.Destroy(obj);
		}
	}

	public void EndWallBreak()
	{
		wallAnimator.SetTrigger("OnContinue");
		GameObject[] array = toDestroyOnWallBreakEnd;
		foreach (GameObject obj in array)
		{
			Object.Destroy(obj);
		}
		Object.Destroy(foreground);
		alternateForeground.SetActive(true);
	}

	private IEnumerator tail_cr()
	{
		while (true)
		{
			yield return CupheadTime.WaitForSeconds(this, Random.Range(0f, 0.75f));
			base.animator.SetTrigger("SwitchTailDirection");
			yield return base.animator.WaitForAnimationToStart(this, "Idle_Left", 1);
			yield return CupheadTime.WaitForSeconds(this, Random.Range(0f, 0.75f));
			base.animator.SetTrigger("SwitchTailDirection");
			yield return base.animator.WaitForAnimationToStart(this, "Idle_Right", 1);
		}
	}

	private void BlinkMaybe()
	{
		blinks++;
		if (blinks >= maxBlinks)
		{
			blinks = 0;
			maxBlinks = Random.Range(8, 16);
			blinkOverlaySprite.enabled = true;
			base.animator.SetBool("Blinking", true);
		}
		else
		{
			blinkOverlaySprite.enabled = false;
			base.animator.SetBool("Blinking", false);
		}
	}

	public void StartClaw(bool left)
	{
		state = State.Claw;
		StartCoroutine(claw_cr(left));
	}

	private IEnumerator claw_cr(bool left)
	{
		LevelProperties.Mouse.Claw p = base.properties.CurrentState.claw;
		MouseLevelCatPaw paw = ((!left) ? rightPaw : leftPaw);
		float totalPawAttackTime = 0.584f + 2f * p.holdGroundTime;
		float totalPawLeaveTime = 0.584f * p.moveSpeed / p.leaveSpeed;
		float headMoveBackTime = p.holdGroundTime + totalPawLeaveTime + 0.417f;
		base.animator.SetTrigger((!left) ? "StartClawRight" : "StartClawLeft");
		yield return base.animator.WaitForAnimationToStart(this, (!left) ? "Claw_Right_Start" : "Claw_Left_Start", 2);
		yield return CupheadTime.WaitForSeconds(this, p.attackDelay);
		paw.Attack(p);
		StartCoroutine(spawnFallingObjects_cr(left));
		yield return StartCoroutine(tween_cr(head.transform, headStartPos, headMoveTransform.localPosition, Quaternion.identity, headMoveTransform.rotation, EaseUtils.EaseType.easeOutQuad, totalPawAttackTime));
		StartCoroutine(tween_cr(head.transform, headMoveTransform.localPosition, headStartPos, headMoveTransform.rotation, Quaternion.identity, EaseUtils.EaseType.easeInOutSine, headMoveBackTime));
		yield return CupheadTime.WaitForSeconds(this, headMoveBackTime - 0.417f);
		base.animator.SetTrigger("Continue");
		yield return base.animator.WaitForAnimationToEnd(this, (!left) ? "Claw_Right_End" : "Claw_Left_End", 2);
		yield return CupheadTime.WaitForSeconds(this, p.hesitateAfterAttack);
		state = State.Idle;
	}

	private IEnumerator spawnFallingObjects_cr(bool left)
	{
		LevelProperties.Mouse.Claw p = base.properties.CurrentState.claw;
		MouseLevelCatPaw paw = ((!left) ? rightPaw : leftPaw);
		yield return paw.animator.WaitForAnimationToStart(this, "Attack_Hit");
		fallingObjectsIndex = (fallingObjectsIndex + 1) % p.fallingObjectStrings.Length;
		string[] pattern = p.fallingObjectStrings[fallingObjectsIndex].Split(',');
		float waitTime = 0f;
		string[] array = pattern;
		foreach (string instruction in array)
		{
			if (instruction[0] == 'D')
			{
				float.TryParse(instruction.Substring(1), out waitTime);
				continue;
			}
			yield return CupheadTime.WaitForSeconds(this, waitTime);
			string[] positions = instruction.Split('-');
			string[] array2 = positions;
			foreach (string s in array2)
			{
				float result = 0f;
				float.TryParse(s, out result);
				fallingObjectPrefabs.RandomChoice().Create(result, p);
			}
			waitTime = p.objectSpawnDelay;
		}
	}

	public void StartGhostMouse()
	{
		state = State.GhostMouse;
		StartCoroutine(jailHead_cr());
	}

	private IEnumerator jailHead_cr()
	{
		MouseLevelGhostMouse[] ghostMice = ((!base.properties.CurrentState.ghostMouse.fourMice) ? twoGhostMice : fourGhostMice);
		bool unspawnedGhosts = false;
		MouseLevelGhostMouse[] array = ghostMice;
		foreach (MouseLevelGhostMouse mouseLevelGhostMouse in array)
		{
			if (mouseLevelGhostMouse.state == MouseLevelGhostMouse.State.Unspawned)
			{
				unspawnedGhosts = true;
				break;
			}
		}
		if (unspawnedGhosts)
		{
			base.animator.SetTrigger("StartGhostMouse");
			yield return base.animator.WaitForAnimationToStart(this, "Jail_Loop", 2);
			yield return CupheadTime.WaitForSeconds(this, base.properties.CurrentState.ghostMouse.jailDuration);
			base.animator.SetTrigger("Continue");
			yield return base.animator.WaitForAnimationToEnd(this, "Jail_End", 2);
			yield return CupheadTime.WaitForSeconds(this, base.properties.CurrentState.ghostMouse.hesitateAfterAttack);
		}
		state = State.Idle;
	}

	public void SpawnGhostMice()
	{
		StartCoroutine(spawnGhostMice_cr());
	}

	private IEnumerator spawnGhostMice_cr()
	{
		MouseLevelGhostMouse[] ghostMice = ((!base.properties.CurrentState.ghostMouse.fourMice) ? twoGhostMice : fourGhostMice);
		ghostMice.Shuffle();
		MouseLevelGhostMouse[] array = ghostMice;
		foreach (MouseLevelGhostMouse mouse in array)
		{
			if (mouse.state == MouseLevelGhostMouse.State.Unspawned)
			{
				mouse.Spawn(base.properties);
				yield return CupheadTime.WaitForSeconds(this, 0.1f);
			}
		}
		while (ghostMice[ghostMice.Length - 1].state == MouseLevelGhostMouse.State.Intro)
		{
			yield return null;
		}
		if (!alreadyManagingGhostMice)
		{
			StartCoroutine(manageGhostMice_cr());
		}
	}

	private IEnumerator manageGhostMice_cr()
	{
		alreadyManagingGhostMice = true;
		MouseLevelGhostMouse[] ghostMice = ((!base.properties.CurrentState.ghostMouse.fourMice) ? twoGhostMice : fourGhostMice);
		int shotsTillPinkAttack = base.properties.CurrentState.ghostMouse.pinkBallRange.RandomInt();
		bool anyMiceSpawned = true;
		while (anyMiceSpawned)
		{
			ghostMice.Shuffle();
			anyMiceSpawned = false;
			MouseLevelGhostMouse[] array = ghostMice;
			foreach (MouseLevelGhostMouse mouse in array)
			{
				if (mouse.state != MouseLevelGhostMouse.State.Unspawned && mouse.state != MouseLevelGhostMouse.State.Dying)
				{
					anyMiceSpawned = true;
				}
				if (mouse.state == MouseLevelGhostMouse.State.Idle)
				{
					shotsTillPinkAttack--;
					bool pinkAttack = false;
					if (shotsTillPinkAttack == 0)
					{
						pinkAttack = true;
						shotsTillPinkAttack = base.properties.CurrentState.ghostMouse.pinkBallRange.RandomInt();
					}
					mouse.Attack(pinkAttack);
					while (mouse.state == MouseLevelGhostMouse.State.Attack)
					{
						yield return null;
					}
					yield return CupheadTime.WaitForSeconds(this, base.properties.CurrentState.ghostMouse.attackDelayRange.RandomFloat());
				}
			}
		}
		alreadyManagingGhostMice = false;
	}

	public void OnBossDeath()
	{
		state = State.Dying;
		StopAllCoroutines();
		Object.Destroy(leftPaw.gameObject);
		Object.Destroy(rightPaw.gameObject);
		head.transform.localPosition = headStartPos;
		head.transform.SetEulerAngles(0f, 0f, 0f);
		base.animator.SetTrigger("Die");
		headFrontRenderer.sortingLayerName = "Enemies";
		MouseLevelGhostMouse[] array = ((!base.properties.CurrentState.ghostMouse.fourMice) ? twoGhostMice : fourGhostMice);
		MouseLevelGhostMouse[] array2 = array;
		foreach (MouseLevelGhostMouse mouseLevelGhostMouse in array2)
		{
			mouseLevelGhostMouse.Die();
		}
	}

	private IEnumerator tween_cr(Transform trans, Vector2 startPos, Vector2 endPos, Quaternion startRotation, Quaternion endRotation, EaseUtils.EaseType ease, float time)
	{
		float t = 0f;
		trans.localPosition = startPos;
		trans.localRotation = startRotation;
		float accumulator = 0f;
		while (t < time)
		{
			accumulator += (float)CupheadTime.Delta;
			while (accumulator > 1f / 24f)
			{
				accumulator -= 1f / 24f;
				float t2 = EaseUtils.Ease(ease, 0f, 1f, t / time);
				trans.localPosition = Vector2.Lerp(startPos, endPos, t2);
				trans.localRotation = Quaternion.Slerp(startRotation, endRotation, t2);
				t += 1f / 24f;
			}
			yield return null;
		}
		trans.localPosition = endPos;
		trans.localRotation = endRotation;
		yield return null;
	}

	private void SoundCatIntro()
	{
		AudioManager.Play("level_mouse_cat_intro");
	}

	private void SoundCatJailEnd()
	{
		AudioManager.Play("level_mouse_cat_jail_end");
		emitAudioFromObject.Add("level_mouse_cat_jail_end");
	}
}
