using System.Collections;
using UnityEngine;

public class DevilLevelEffectSpawner : AbstractPausableComponent
{
	[SerializeField]
	private bool isSmoke3;

	public MinMax waitTime;

	public Effect effectPrefab;

	private Effect effect;

	protected override void Start()
	{
		base.Start();
		StartCoroutine(main_cr());
	}

	private IEnumerator main_cr()
	{
		yield return CupheadTime.WaitForSeconds(this, Random.Range(0f, waitTime.max));
		yield return null;
		while (true)
		{
			effect = effectPrefab.Create(base.transform.position);
			effect.transform.parent = base.transform;
			while (effect != null)
			{
				yield return null;
			}
			yield return CupheadTime.WaitForSeconds(this, waitTime.RandomFloat());
			yield return null;
		}
	}

	public void KillSmoke()
	{
		StopAllCoroutines();
		if (isSmoke3)
		{
			StartCoroutine(fade_out_cr());
		}
	}

	private IEnumerator fade_out_cr()
	{
		float t = 0f;
		float time = 0.5f;
		while (t < time)
		{
			t += (float)CupheadTime.Delta;
			if (effect != null)
			{
				effect.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f - t / time);
			}
			yield return null;
		}
		yield return null;
	}
}
