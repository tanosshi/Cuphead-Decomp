using System.Collections;
using UnityEngine;

public class FlowerLevelPollenPetal : Effect
{
	protected override void Start()
	{
		base.Start();
		string stateName = ((!Rand.Bool()) ? "Petal_B" : "Petal_A");
		animator.Play(stateName);
		StartCoroutine(fall_cr());
		StartCoroutine(fade_cr());
	}

	private IEnumerator fall_cr()
	{
		float fallSpeed = 100f;
		while (true)
		{
			base.transform.position -= Vector3.up * fallSpeed * CupheadTime.Delta;
			yield return null;
		}
	}

	private IEnumerator fade_cr()
	{
		float t = 0f;
		float time = 2f;
		Color currentColor = GetComponent<SpriteRenderer>().color;
		yield return CupheadTime.WaitForSeconds(this, 1.5f);
		while (t < time)
		{
			GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f - t / time);
			t += (float)CupheadTime.Delta;
			yield return null;
		}
		GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0f);
		OnEffectComplete();
		yield return null;
	}
}
