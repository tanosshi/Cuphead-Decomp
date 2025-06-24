using System.Collections;
using UnityEngine;

public class SallyStagePlayApplauseHandler : AbstractPausableComponent
{
	private const float FRAME_TIME = 1f / 24f;

	[SerializeField]
	private SallyStagePlayLevelRose rose;

	[SerializeField]
	private Transform[] hands;

	private Vector3[] handsStartPos;

	[SerializeField]
	private Transform[] roseHands;

	[SerializeField]
	private Transform roseStill;

	[SerializeField]
	private Transform endPos;

	[SerializeField]
	private string pinkString;

	private string[] pinkPattern;

	private int pinkIndex;

	protected override void Start()
	{
		base.Start();
		handsStartPos = new Vector3[hands.Length];
		for (int i = 0; i < hands.Length; i++)
		{
			hands[i].GetComponent<Animator>().Play((!Rand.Bool()) ? "B" : "A");
			handsStartPos[i] = hands[i].transform.position;
		}
		pinkPattern = pinkString.Split(',');
		pinkIndex = Random.Range(0, pinkPattern.Length);
	}

	protected override void OnDrawGizmos()
	{
		base.OnDrawGizmos();
		Gizmos.DrawLine(base.transform.position, endPos.transform.position);
	}

	public void SlideApplause(bool slideIn)
	{
		for (int i = 0; i < hands.Length; i++)
		{
			StartCoroutine(slide_cr(hands[i], handsStartPos[i], slideIn, Random.Range(0.3f, 0.8f)));
			AudioManager.Play("sally_audience_applause");
		}
	}

	private IEnumerator slide_cr(Transform hand, Vector3 handStart, bool slideIn, float delay)
	{
		Vector3 start = ((!slideIn) ? new Vector3(hand.transform.position.x, endPos.position.y) : handStart);
		Vector3 end = ((!slideIn) ? handStart : new Vector3(hand.transform.position.x, endPos.position.y));
		float t = 0f;
		float frameTime = 0f;
		float time = 0.6f;
		yield return CupheadTime.WaitForSeconds(this, delay);
		while (t < time)
		{
			frameTime += (float)CupheadTime.Delta;
			t += (float)CupheadTime.Delta;
			if (frameTime > 1f / 24f)
			{
				frameTime -= 1f / 24f;
				hand.transform.position = Vector3.Lerp(start, end, t / time);
			}
			yield return null;
		}
		hand.transform.position = end;
		yield return null;
	}

	public void ThrowRose(Vector3 pos, LevelProperties.SallyStagePlay.Roses properties)
	{
		StartCoroutine(throw_rose_cr(pos, properties, roseHands[Random.Range(0, roseHands.Length)]));
	}

	private IEnumerator throw_rose_cr(Vector3 pos, LevelProperties.SallyStagePlay.Roses properties, Transform arm)
	{
		Animator component = arm.GetComponent<Animator>();
		string stateName;
		string animationName = (stateName = ((!Rand.Bool()) ? "Rose_2_A" : "Rose_1_A"));
		component.Play(stateName);
		arm.transform.SetPosition(pos.x);
		float speed = 900f;
		yield return arm.GetComponent<Animator>().WaitForAnimationToEnd(this, animationName);
		roseStill.transform.position = new Vector3(arm.transform.position.x, arm.transform.position.y + 50f);
		while (roseStill.transform.position.y < (float)Level.Current.Ceiling + 100f)
		{
			roseStill.transform.position += Vector3.up * speed * CupheadTime.Delta;
			yield return null;
		}
		SallyStagePlayLevelRose r = rose.Create(pos, properties);
		r.SetParryable(pinkPattern[pinkIndex][0] == 'P');
		pinkIndex = (pinkIndex + 1) % pinkPattern.Length;
		yield return null;
	}
}
