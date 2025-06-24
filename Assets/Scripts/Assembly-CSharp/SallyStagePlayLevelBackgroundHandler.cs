using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SallyStagePlayLevelBackgroundHandler : AbstractPausableComponent
{
	public enum Backgrounds
	{
		Church = 0,
		House = 1,
		Nunnery = 2,
		Purgatory = 3,
		Finale = 4
	}

	[Serializable]
	public class Cupid
	{
		public Transform cupidTransform;

		public Vector3 startPosition;

		public bool acceptableLevel;
	}

	private const float FRAME_TIME = 1f / 24f;

	[Header("Main")]
	[SerializeField]
	private SallyStagePlayLevelSally sally;

	[SerializeField]
	private Transform curtain;

	[SerializeField]
	private Transform curtainSprite;

	[SerializeField]
	private Transform curtainShadow;

	[SerializeField]
	private Transform curtainUpRoot;

	[SerializeField]
	private SpriteRenderer[] flickeringLights;

	[SerializeField]
	private SallyStagePlayApplauseHandler applauseHandler;

	[Header("Church")]
	[SerializeField]
	private Transform[] churchSwingies;

	[SerializeField]
	private Cupid[] cupids;

	[SerializeField]
	private Animator priest;

	[SerializeField]
	private Animator husband;

	[SerializeField]
	private Animator sallyBackground;

	[SerializeField]
	private Transform car;

	[SerializeField]
	private Transform carRoot;

	[SerializeField]
	private Transform chandelier;

	[Header("Residence")]
	[SerializeField]
	private SallyStagePlayLevelHouse residence;

	[SerializeField]
	private Animator husbandPhase2;

	[SerializeField]
	private Transform[] husbandRoots;

	[SerializeField]
	private Animator priestPhase2;

	[SerializeField]
	private Transform[] priestRoots;

	[Header("Purgatory")]
	[SerializeField]
	private Transform[] purgSwingies;

	[Header("Finale")]
	[SerializeField]
	private Transform[] finaleSwingies;

	[Header("Backgrounds")]
	[SerializeField]
	private GameObject[] backgrounds;

	private float fadeWaitMinSecond = 8f;

	private float fadeWaitMaxSecond = 25f;

	private float curtainWaitTime = 2.5f;

	private bool husbandMoving = true;

	private bool dropChandelier;

	private Vector3 curtainStartPos;

	private Vector3 curtainShadowStartPos;

	private LevelProperties.SallyStagePlay properties;

	private SallyStagePlayLevel parent;

	private List<Coroutine> phaseDependentCoroutines;

	public static bool HUSBAND_GONE { get; private set; }

	public static bool CURTAIN_OPEN { get; private set; }

	protected override void Awake()
	{
		base.Awake();
		curtain.gameObject.SetActive(true);
	}

	protected override void Start()
	{
		base.Start();
		Level.Current.OnLevelStartEvent += StartPriestLoop;
		Level.Current.OnLevelStartEvent += StartHusbandLoop;
		curtainStartPos = curtainSprite.position;
		curtainShadowStartPos = curtainShadow.position;
		HUSBAND_GONE = false;
		Cupid[] array = cupids;
		foreach (Cupid cupid in array)
		{
			cupid.startPosition = cupid.cupidTransform.position;
		}
		applauseHandler.SlideApplause(true);
	}

	public void GetProperties(LevelProperties.SallyStagePlay properties, SallyStagePlayLevel parent)
	{
		this.properties = properties;
		this.parent = parent;
		SpriteRenderer[] array = flickeringLights;
		foreach (SpriteRenderer flicker in array)
		{
			StartCoroutine(flicker_cr(flicker));
		}
		phaseDependentCoroutines = new List<Coroutine>();
		phaseDependentCoroutines.Add(StartCoroutine(check_bools_cr()));
		Transform[] array2 = churchSwingies;
		foreach (Transform swing in array2)
		{
			phaseDependentCoroutines.Add(StartCoroutine(swing_cr(swing)));
		}
		AbstractPlayerController next = PlayerManager.GetNext();
		LevelPlayerController levelPlayerController = (LevelPlayerController)next;
		levelPlayerController.motor.OnHitEvent += PlayYay;
		parent.OnPhase2 += OnPhase2;
		parent.OnPhase3 += OnPhase3;
		parent.OnPhase4 += OnPhase4;
	}

	public void OpenCurtain(Backgrounds backgroundSelected)
	{
		if (backgroundSelected != Backgrounds.Finale)
		{
			applauseHandler.SlideApplause(false);
		}
		StartCoroutine(open_curtain_cr(backgroundSelected));
	}

	public void CloseCurtain(Backgrounds backgroundSelected)
	{
		applauseHandler.SlideApplause(true);
		StartCoroutine(close_curtain_cr(backgroundSelected));
	}

	private IEnumerator open_curtain_cr(Backgrounds backgroundsSelected)
	{
		SelectBackground(backgroundsSelected);
		float t = 0f;
		float frameTime = 0f;
		float openTime = 1.58f;
		Vector3 shadowRoot = new Vector3(curtainUpRoot.position.x, curtainUpRoot.position.y - 100f);
		AudioManager.Play("sally_bg_stage_curtain_raise");
		emitAudioFromObject.Add("sally_bg_stage_curtain_raise");
		while (t < openTime)
		{
			frameTime += (float)CupheadTime.Delta;
			t += (float)CupheadTime.Delta;
			if (frameTime > 1f / 24f)
			{
				float t2 = EaseUtils.Ease(EaseUtils.EaseType.linear, 0f, 1f, t / openTime);
				curtainSprite.transform.position = Vector2.Lerp(curtainSprite.transform.position, curtainUpRoot.position, t2);
				curtainShadow.transform.position = Vector2.Lerp(curtainShadow.transform.position, shadowRoot, t2);
				frameTime -= 1f / 24f;
			}
			yield return null;
		}
		CURTAIN_OPEN = true;
		yield return null;
	}

	private IEnumerator close_curtain_cr(Backgrounds backgroundSelected)
	{
		float t = 0f;
		float frameTime = 0f;
		float closeTime = 1.58f;
		AudioManager.Play("sally_bg_stage_curtain_lower");
		emitAudioFromObject.Add("sally_bg_stage_curtain_lower");
		switch (backgroundSelected)
		{
		case Backgrounds.House:
			AudioManager.Play("sally_bg_stage_reset_phase1");
			emitAudioFromObject.Add("sally_bg_stage_reset_phase1");
			break;
		case Backgrounds.Nunnery:
			AudioManager.Play("sally_bg_stage_reset_phase1");
			emitAudioFromObject.Add("sally_bg_stage_reset_phase1");
			break;
		case Backgrounds.Purgatory:
			AudioManager.Play("sally_bg_stage_reset_phase2");
			emitAudioFromObject.Add("sally_bg_stage_reset_phase2");
			break;
		case Backgrounds.Finale:
			AudioManager.Play("sally_bg_stage_reset_phase3");
			emitAudioFromObject.Add("sally_bg_stage_reset_phase3");
			break;
		}
		while (t < closeTime)
		{
			frameTime += (float)CupheadTime.Delta;
			t += (float)CupheadTime.Delta;
			if (frameTime > 1f / 24f)
			{
				float t2 = EaseUtils.Ease(EaseUtils.EaseType.linear, 0f, 1f, t / closeTime);
				curtainSprite.transform.position = Vector2.Lerp(curtainSprite.transform.position, curtainStartPos, t2);
				curtainShadow.transform.position = Vector2.Lerp(curtainShadow.transform.position, curtainShadowStartPos, t2);
				frameTime -= 1f / 24f;
			}
			yield return null;
		}
		CURTAIN_OPEN = false;
		yield return null;
	}

	private void SelectBackground(Backgrounds backgroundSelected)
	{
		for (int i = 0; i < backgrounds.Length; i++)
		{
			if (i == (int)backgroundSelected)
			{
				backgrounds[i].SetActive(true);
			}
			else
			{
				backgrounds[i].SetActive(false);
			}
		}
	}

	private IEnumerator flicker_cr(SpriteRenderer flicker)
	{
		float flickerTime = 0.3f;
		while (true)
		{
			int counter = 0;
			float waitTime = UnityEngine.Random.Range(fadeWaitMinSecond, fadeWaitMaxSecond);
			float t = 0f;
			yield return CupheadTime.WaitForSeconds(this, waitTime);
			while (counter < 2)
			{
				while (t < flickerTime)
				{
					flicker.color = new Color(1f, 1f, 1f, 1f - t / flickerTime);
					t += (float)CupheadTime.Delta;
					yield return null;
				}
				t = 0f;
				flicker.color = new Color(1f, 1f, 1f, 0f);
				while (t < flickerTime)
				{
					flicker.color = new Color(1f, 1f, 1f, t / flickerTime);
					t += (float)CupheadTime.Delta;
					yield return null;
				}
				flicker.color = new Color(1f, 1f, 1f, 1f);
				counter++;
				yield return null;
			}
			yield return null;
		}
	}

	private IEnumerator swing_cr(Transform swing)
	{
		float t = 0f;
		float speed = 0f;
		float maxSpeed = 8f;
		float minSpeed = 3f;
		bool movingRight = Rand.Bool();
		speed = minSpeed;
		while (true)
		{
			t = ((!movingRight) ? (t - (float)CupheadTime.Delta) : (t + (float)CupheadTime.Delta));
			float phase = Mathf.Sin(t);
			swing.localRotation = Quaternion.Euler(new Vector3(0f, 0f, phase * speed));
			if (CupheadLevelCamera.Current.isShaking)
			{
				if (speed < maxSpeed)
				{
					speed += 0.15f;
				}
			}
			else if (speed > minSpeed)
			{
				speed -= 0.05f;
			}
			yield return null;
		}
	}

	private void StartPriestLoop()
	{
		phaseDependentCoroutines.Add(StartCoroutine(priest_animations_cr()));
	}

	private IEnumerator priest_animations_cr()
	{
		while (properties.CurrentState.stateName == LevelProperties.SallyStagePlay.States.Generic)
		{
			bool tuckDown = false;
			int counter = 0;
			int maxCounter = UnityEngine.Random.Range(2, 6);
			while (!tuckDown)
			{
				yield return CupheadTime.WaitForSeconds(this, UnityEngine.Random.Range(2f, 5f));
				priest.SetTrigger("Continue");
				if (counter < maxCounter)
				{
					counter++;
				}
				else
				{
					tuckDown = true;
				}
				yield return null;
			}
			bool isLookingRight = true;
			maxCounter = UnityEngine.Random.Range(4, 8);
			counter = 0;
			priest.Play("Tuck_Down");
			while (tuckDown)
			{
				yield return CupheadTime.WaitForSeconds(this, UnityEngine.Random.Range(2f, 5f));
				priest.SetBool("isLookingRight", isLookingRight);
				if (counter < maxCounter)
				{
					counter++;
				}
				else
				{
					tuckDown = false;
				}
				isLookingRight = !isLookingRight;
				yield return null;
			}
			priest.Play("Stand_Up");
			yield return null;
		}
		priest.Play("Look_Around");
		yield return null;
	}

	private void StartHusbandLoop()
	{
		husband.SetTrigger("Continue");
		phaseDependentCoroutines.Add(StartCoroutine(husband_move_cr()));
	}

	private IEnumerator husband_move_cr()
	{
		bool movingRight = true;
		float time = 2f;
		float end = 0f;
		float moveOffset = 400f;
		yield return husband.WaitForAnimationToStart(this, "Move");
		while (husbandMoving)
		{
			yield return null;
			float t = 0f;
			float start = husband.transform.position.x;
			end = ((!movingRight) ? (-640f + moveOffset) : (640f - moveOffset));
			while (t < time && husbandMoving)
			{
				while (husband.GetCurrentAnimatorStateInfo(0).IsName("OhNo") || husband.GetCurrentAnimatorStateInfo(0).IsName("Yay"))
				{
					yield return null;
				}
				TransformExtensions.SetPosition(x: EaseUtils.Ease(EaseUtils.EaseType.easeInOutSine, start, end, t / time), transform: husband.transform);
				t += (float)CupheadTime.Delta;
				yield return null;
			}
			if (husbandMoving)
			{
				husband.transform.SetPosition(end);
			}
			movingRight = !movingRight;
			yield return null;
		}
	}

	private void PlayYay()
	{
		if (husbandMoving && !AudioManager.CheckIfPlaying("sally_bg_church_fiance_yay"))
		{
			AudioManager.Play("sally_bg_church_fiance_yay");
			emitAudioFromObject.Add("sally_bg_church_fiance_yay");
			husband.Play("Yay");
		}
	}

	private IEnumerator cupid_check_falling(Cupid cupid)
	{
		AbstractPlayerController player = PlayerManager.GetNext();
		LevelProperties.SallyStagePlay.General p = properties.CurrentState.general;
		while (true)
		{
			if (player == null || player.IsDead)
			{
				player = PlayerManager.GetNext();
			}
			if (player.transform.parent != cupid.cupidTransform && cupid.cupidTransform.position.y >= p.cupidDropMaxY)
			{
				player = PlayerManager.GetNext();
				if (cupid.cupidTransform.position.y < cupid.startPosition.y)
				{
					cupid.cupidTransform.position += Vector3.up * p.cupidMoveSpeed * CupheadTime.Delta;
				}
				yield return null;
			}
			else if (cupid.cupidTransform.position.y > p.cupidDropMaxY)
			{
				cupid.cupidTransform.position += Vector3.down * p.cupidMoveSpeed * CupheadTime.Delta;
			}
			if (cupid.cupidTransform.position.y <= p.cupidDropMaxY)
			{
				cupid.acceptableLevel = true;
			}
			else
			{
				cupid.acceptableLevel = false;
			}
			yield return null;
		}
	}

	private IEnumerator check_bools_cr()
	{
		while (!cupids[0].acceptableLevel || !cupids[1].acceptableLevel)
		{
			yield return null;
		}
		StartCoroutine(chandelier_cr());
		properties.DealDamageToNextNamedState();
		HUSBAND_GONE = true;
		float clamp = 20f;
		while (husband.transform.position.x > clamp || husband.transform.position.x < 0f - clamp)
		{
			yield return null;
		}
		husbandMoving = false;
		husband.SetTrigger("Dead");
		SallyStagePlayHusbandExplosion ex = husband.GetComponent<SallyStagePlayHusbandExplosion>();
		if (ex != null)
		{
			ex.StartExplosion();
		}
		yield return husband.WaitForAnimationToEnd(this, "Death");
		dropChandelier = true;
		yield return null;
		float t = 0f;
		float frameTime = 0f;
		float moveTime = 0.15f;
		Vector3 end = new Vector3(chandelier.transform.position.x, sallyBackground.transform.position.y - 30f);
		while (t < moveTime)
		{
			frameTime += (float)CupheadTime.Delta;
			t += (float)CupheadTime.Delta;
			if (frameTime > 1f / 24f)
			{
				float t2 = EaseUtils.Ease(EaseUtils.EaseType.linear, 0f, 1f, t / moveTime);
				chandelier.transform.position = Vector2.Lerp(chandelier.transform.position, end, t2);
				frameTime -= 1f / 24f;
			}
			yield return null;
		}
		yield return null;
		CupheadLevelCamera.Current.Shake(10f, 0.4f);
		yield return CupheadTime.WaitForSeconds(this, 1f);
	}

	private IEnumerator chandelier_cr()
	{
		float frameTime = 0f;
		float posAdd = 15f;
		while (!dropChandelier)
		{
			frameTime += (float)CupheadTime.Delta;
			if (frameTime > 1f / 24f)
			{
				frameTime -= 1f / 24f;
				chandelier.transform.AddPosition(posAdd);
			}
			posAdd = 0f - posAdd;
			yield return null;
		}
	}

	private void OnPhase2()
	{
		if (!HUSBAND_GONE)
		{
			StartCoroutine(just_married_cr());
		}
		else
		{
			StartCoroutine(crying_cr());
		}
	}

	public void RollUpCupids()
	{
		Cupid[] array = cupids;
		foreach (Cupid cupid in array)
		{
			StartCoroutine(roll_up_cupids_cr(cupid));
		}
	}

	private IEnumerator roll_up_cupids_cr(Cupid cupid)
	{
		float t = 0f;
		float frameTime = 0f;
		float moveTime = 3.5f;
		Vector3 end = new Vector3(cupid.cupidTransform.position.x, 800f);
		cupid.cupidTransform.GetComponent<Collider2D>().enabled = false;
		while (t < moveTime)
		{
			frameTime += (float)CupheadTime.Delta;
			t += (float)CupheadTime.Delta;
			if (frameTime > 1f / 24f)
			{
				float t2 = EaseUtils.Ease(EaseUtils.EaseType.linear, 0f, 1f, t / moveTime);
				cupid.cupidTransform.position = Vector2.Lerp(cupid.cupidTransform.position, end, t2);
				frameTime -= 1f / 24f;
			}
			yield return null;
		}
		yield return null;
	}

	private IEnumerator just_married_cr()
	{
		float t = 0f;
		float frameTime = 0f;
		float moveTime = 1.5f;
		Vector3 end = new Vector3(husband.transform.position.x, car.position.y);
		while (sally.state != SallyStagePlayLevelSally.State.Transition)
		{
			yield return null;
		}
		sally.animator.SetTrigger("OnPhase2");
		priest.SetTrigger("CarAppeared");
		husbandMoving = false;
		husband.SetTrigger("Married");
		yield return husband.WaitForAnimationToEnd(this, "Tada_Start");
		sallyBackground.Play("Wave");
		AudioManager.Play("sally_ph1_bg_car_enter");
		sallyBackground.transform.parent = car.transform;
		sallyBackground.transform.position = carRoot.transform.position;
		while (t < moveTime)
		{
			frameTime += (float)CupheadTime.Delta;
			t += (float)CupheadTime.Delta;
			if (frameTime > 1f / 24f)
			{
				float t2 = EaseUtils.Ease(EaseUtils.EaseType.easeInOutSine, 0f, 1f, t / moveTime);
				car.transform.position = Vector2.Lerp(car.transform.position, end, t2);
				frameTime -= 1f / 24f;
			}
			yield return null;
		}
		AudioManager.PlayLoop("sally_ph1_bg_car_loop");
		husband.SetTrigger("Drive");
		husband.transform.parent = car.transform;
		AudioManager.Play("sally_ph1_bg_car_exit");
		AudioManager.Stop("sally_ph1_bg_car_loop");
		t = 0f;
		frameTime = 0f;
		moveTime = 2f;
		end.x = 1140f;
		yield return CupheadTime.WaitForSeconds(this, 0.5f);
		while (t < moveTime)
		{
			frameTime += (float)CupheadTime.Delta;
			t += (float)CupheadTime.Delta;
			if (frameTime > 1f / 24f)
			{
				float t3 = EaseUtils.Ease(EaseUtils.EaseType.easeInSine, 0f, 1f, t / moveTime);
				car.transform.position = Vector2.Lerp(car.transform.position, end, t3);
				frameTime -= 1f / 24f;
			}
			yield return null;
		}
		CloseCurtain(Backgrounds.House);
		yield return CupheadTime.WaitForSeconds(this, curtainWaitTime);
		HaltCoroutines();
		OpenCurtain(Backgrounds.House);
		residence.StartPhase2(parent, properties);
		sally.StartPhase2();
		StartPeeking();
		yield return null;
	}

	private IEnumerator crying_cr()
	{
		while (sally.state != SallyStagePlayLevelSally.State.Transition)
		{
			yield return null;
		}
		sally.animator.SetTrigger("OnPhase2");
		yield return CupheadTime.WaitForSeconds(this, 1.5f);
		priest.Play("Tuck_Down_Disappear");
		sallyBackground.Play("Cry");
		yield return CupheadTime.WaitForSeconds(this, 1f);
		CloseCurtain(Backgrounds.Nunnery);
		SallyStagePlayHusbandExplosion ex = husband.GetComponent<SallyStagePlayHusbandExplosion>();
		if (ex != null)
		{
			ex.StopExplosions();
		}
		yield return CupheadTime.WaitForSeconds(this, curtainWaitTime);
		HaltCoroutines();
		OpenCurtain(Backgrounds.Nunnery);
		residence.StartPhase2(parent, properties);
		sally.StartPhase2();
		StartPeeking();
		yield return null;
	}

	private void HaltCoroutines()
	{
		foreach (Coroutine phaseDependentCoroutine in phaseDependentCoroutines)
		{
			StopCoroutine(phaseDependentCoroutine);
		}
		phaseDependentCoroutines.Clear();
	}

	private void StartPeeking()
	{
		if (HUSBAND_GONE)
		{
			phaseDependentCoroutines.Add(StartCoroutine(peek_cr(priestPhase2, priestRoots)));
		}
		else
		{
			phaseDependentCoroutines.Add(StartCoroutine(peek_cr(husbandPhase2, husbandRoots)));
		}
	}

	private IEnumerator peek_cr(Animator animator, Transform[] roots)
	{
		float waitTime = UnityEngine.Random.Range(8f, 20f);
		while (true)
		{
			yield return CupheadTime.WaitForSeconds(this, waitTime);
			yield return null;
			int rootChosen = UnityEngine.Random.Range(0, roots.Length);
			animator.GetComponent<Transform>().position = roots[rootChosen].position;
			animator.GetComponent<Transform>().SetScale(roots[rootChosen].localScale.x);
			if (HUSBAND_GONE && rootChosen == 1)
			{
				animator.SetBool("isDiag", true);
			}
			else
			{
				animator.SetBool("isDiag", Rand.Bool());
			}
			animator.SetTrigger("Peek");
			waitTime = UnityEngine.Random.Range(8f, 20f);
			yield return null;
		}
	}

	private void OnPhase3()
	{
		phaseDependentCoroutines.Clear();
		Transform[] array = purgSwingies;
		foreach (Transform swing in array)
		{
			phaseDependentCoroutines.Add(StartCoroutine(swing_cr(swing)));
		}
		if (HUSBAND_GONE)
		{
			priestPhase2.Play("Shake");
		}
		else
		{
			husbandPhase2.Play("Cry");
		}
		StartCoroutine(phase3_background_cr());
	}

	private IEnumerator phase3_background_cr()
	{
		yield return CupheadTime.WaitForSeconds(this, 2f);
		CloseCurtain(Backgrounds.Purgatory);
		yield return CupheadTime.WaitForSeconds(this, curtainWaitTime);
		OpenCurtain(Backgrounds.Purgatory);
		yield return null;
	}

	private void OnPhase4()
	{
		HaltCoroutines();
		Transform[] array = finaleSwingies;
		foreach (Transform swing in array)
		{
			phaseDependentCoroutines.Add(StartCoroutine(swing_cr(swing)));
		}
		StartCoroutine(phase4_background_cr());
	}

	private IEnumerator phase4_background_cr()
	{
		CloseCurtain(Backgrounds.Finale);
		yield return CupheadTime.WaitForSeconds(this, curtainWaitTime);
		OpenCurtain(Backgrounds.Finale);
		yield return null;
	}
}
