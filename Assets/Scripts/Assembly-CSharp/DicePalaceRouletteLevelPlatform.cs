using System.Collections;
using UnityEngine;

public class DicePalaceRouletteLevelPlatform : ParrySwitch
{
	[SerializeField]
	private SpriteRenderer sheen;

	[SerializeField]
	private bool isOffset;

	[SerializeField]
	private GameObject platform;

	private LevelProperties.DicePalaceRoulette.Platform properties;

	private Color pink;

	private int maxCounter;

	private int counter;

	public new bool enabled
	{
		get
		{
			return GetComponent<CircleCollider2D>().enabled && !platform.GetComponent<BoxCollider2D>().enabled;
		}
		set
		{
			GetComponent<CircleCollider2D>().enabled = value;
			platform.GetComponent<BoxCollider2D>().enabled = !value;
		}
	}

	protected override void Start()
	{
		base.Start();
		maxCounter = Random.Range(1, 4);
		base.animator.SetBool("isOffset", isOffset);
		enabled = true;
		StartCoroutine(sparkles_cr());
	}

	public override void OnParryPostPause(AbstractPlayerController player)
	{
		enabled = false;
		base.animator.SetBool("isFlipped", !enabled);
		StartCoroutine(timer_cr());
	}

	private IEnumerator timer_cr()
	{
		yield return CupheadTime.WaitForSeconds(this, properties.platformOpenDuration);
		enabled = true;
		base.animator.SetBool("isFlipped", !enabled);
	}

	public void Init(LevelProperties.DicePalaceRoulette.Platform properties)
	{
		this.properties = properties;
	}

	private void CheckSheen()
	{
		if (counter < maxCounter)
		{
			sheen.enabled = false;
			counter++;
		}
		else
		{
			sheen.enabled = true;
			maxCounter = Random.Range(1, 4);
			counter = 0;
		}
	}

	private IEnumerator sparkles_cr()
	{
		while (true)
		{
			yield return CupheadTime.WaitForSeconds(this, Random.Range(0.25f, 1f));
			if (base.animator.GetCurrentAnimatorStateInfo(0).IsName("Idle_1") || base.animator.GetCurrentAnimatorStateInfo(0).IsName("Idle_2"))
			{
				base.animator.SetTrigger("onSparkle");
			}
			yield return null;
		}
	}
}
