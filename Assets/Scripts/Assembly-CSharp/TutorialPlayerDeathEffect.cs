using System.Collections;
using UnityEngine;

public class TutorialPlayerDeathEffect : PlayerDeathEffect
{
	protected Vector3 startPos;

	private Transform tr;

	protected override void Awake()
	{
		base.Awake();
		tr = base.transform;
		startPos = tr.position;
	}

	protected override void Start()
	{
		base.Start();
		Init();
	}

	protected override void Update()
	{
		base.Update();
		if (tr.localPosition.y >= 270f)
		{
			tr.position = startPos;
		}
	}

	protected override void OnParrySwitch()
	{
		base.OnParrySwitch();
		if (parrySwitch.enabled)
		{
			base.animator.SetTrigger("OnParryTutorial");
		}
		parrySwitch.enabled = false;
	}

	private void Init()
	{
		tr.position = startPos;
		playerId = PlayerId.PlayerOne;
		base.animator.SetInteger("Mode", 0);
		base.animator.SetBool("CanParry", true);
		spriteRenderer = cuphead;
		cuphead.gameObject.SetActive(true);
		mugman.gameObject.SetActive(false);
		parrySwitch.enabled = true;
		parrySwitch.gameObject.SetActive(true);
	}

	protected override void OnReviveParryAnimComplete()
	{
		StopAllCoroutines();
		base.animator.Play("Level_Start");
		exiting = false;
		Init();
		StartCoroutine(float_cr());
	}

	protected override IEnumerator checkOutOfFrame_cr()
	{
		yield return null;
	}
}
