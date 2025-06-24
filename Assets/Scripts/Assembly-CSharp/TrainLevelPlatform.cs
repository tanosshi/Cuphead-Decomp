using System.Collections;
using UnityEngine;

public class TrainLevelPlatform : LevelPlatform
{
	public enum CartPosition
	{
		Left = 0,
		Middle = 1,
		Right = 2
	}

	private const float DISTANCE = 390f;

	[SerializeField]
	private ParrySwitch rightSwitch;

	[SerializeField]
	private ParrySwitch leftSwitch;

	[SerializeField]
	private Transform[] sparkRoots;

	[SerializeField]
	private Effect sparkEffectPrefab;

	private CartPosition position;

	private new Animator animator;

	private AnimationHelper animHelper;

	private SpriteRenderer spriteRenderer;

	private float middlePos;

	private float leftPos;

	private float rightPos;

	protected override void Awake()
	{
		base.Awake();
		animator = GetComponent<Animator>();
		animHelper = GetComponent<AnimationHelper>();
		middlePos = base.transform.position.x + 390f;
		leftPos = base.transform.position.x;
		rightPos = base.transform.position.x + 780f;
		position = CartPosition.Left;
		rightSwitch.OnActivate += OnRight;
		leftSwitch.OnActivate += OnLeft;
		StartCoroutine(spark_cr());
	}

	protected override void Update()
	{
		base.Update();
	}

	private void OnLeft()
	{
		AudioManager.Play("train_hand_car_valves_spin");
		emitAudioFromObject.Add("train_hand_car_valves_spin");
		position = ((position == CartPosition.Right) ? CartPosition.Middle : CartPosition.Left);
		Move(SelectPosition());
	}

	private void OnRight()
	{
		AudioManager.Play("train_hand_car_valves_spin");
		emitAudioFromObject.Add("train_hand_car_valves_spin");
		position = ((position == CartPosition.Left) ? CartPosition.Middle : CartPosition.Right);
		Move(SelectPosition());
	}

	private float SelectPosition()
	{
		float result = 0f;
		switch (position)
		{
		case CartPosition.Left:
			result = leftPos;
			break;
		case CartPosition.Right:
			result = rightPos;
			break;
		case CartPosition.Middle:
			result = middlePos;
			break;
		}
		return result;
	}

	private void Move(float x)
	{
		StartCoroutine(move_cr(x));
	}

	private IEnumerator move_cr(float x)
	{
		rightSwitch.gameObject.SetActive(false);
		leftSwitch.gameObject.SetActive(false);
		animator.SetTrigger("OnSlap");
		animator.SetBool("Spinning", true);
		animator.SetBool("Effect", false);
		animHelper.Speed = 1f;
		float t = 0f;
		float time = 1.5f;
		float startX = base.transform.position.x;
		base.transform.SetPosition(startX);
		yield return null;
		while (t < time)
		{
			t += (float)CupheadTime.Delta;
			float val = t / time;
			base.transform.SetPosition(EaseUtils.Ease(EaseUtils.EaseType.easeOutCubic, startX, x, val));
			if (val > 0.5f)
			{
				animHelper.Speed = 0.5f;
			}
			yield return null;
		}
		base.transform.SetPosition(x);
	}

	private void FadeIn()
	{
		rightSwitch.gameObject.SetActive(true);
		leftSwitch.gameObject.SetActive(true);
		animHelper.Speed = 1f;
		animator.SetTrigger("OnContinue");
		animator.SetBool("Effect", true);
	}

	private IEnumerator spark_cr()
	{
		while (true)
		{
			if (!leftSwitch.isActiveAndEnabled)
			{
				yield return null;
				continue;
			}
			yield return CupheadTime.WaitForSeconds(this, Random.Range(0.5f, 1f));
			sparkEffectPrefab.Create(sparkRoots.RandomChoice().position);
			yield return CupheadTime.WaitForSeconds(this, 1f / 3f);
		}
	}
}
