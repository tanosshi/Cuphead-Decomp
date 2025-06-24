using System.Collections;
using UnityEngine;

public class PlatformingLevelParryablePlatform : ParrySwitch
{
	[SerializeField]
	private GameObject platform;

	[SerializeField]
	private float openDuration = 3f;

	[SerializeField]
	private float platformWidth = 36f;

	private Color pink;

	public new bool enabled
	{
		get
		{
			return GetComponent<Collider2D>().enabled;
		}
		set
		{
			GetComponent<Collider2D>().enabled = value;
		}
	}

	protected override void Start()
	{
		base.Start();
		platform.SetActive(false);
		platform.transform.SetScale(platformWidth, 5f, 5f);
		pink = GetComponent<SpriteRenderer>().color;
	}

	public override void OnParryPostPause(AbstractPlayerController player)
	{
		platform.SetActive(true);
		enabled = false;
		GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
		StartCoroutine(timer_cr());
	}

	private IEnumerator timer_cr()
	{
		yield return CupheadTime.WaitForSeconds(this, openDuration);
		platform.SetActive(false);
		enabled = true;
		GetComponent<SpriteRenderer>().color = pink;
	}
}
