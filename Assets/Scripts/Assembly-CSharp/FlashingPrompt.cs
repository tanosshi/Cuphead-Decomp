using UnityEngine;

public class FlashingPrompt : AbstractMonoBehaviour
{
	private const float FLASH_TIME = 0.75f;

	private float flashTimer;

	[SerializeField]
	private GameObject child;

	protected virtual bool ShouldShow
	{
		get
		{
			return true;
		}
	}

	protected override void Update()
	{
		if (ShouldShow)
		{
			flashTimer = (flashTimer + (float)CupheadTime.Delta) % 1.5f;
			child.SetActive(flashTimer < 0.75f);
		}
		else
		{
			child.SetActive(false);
			flashTimer = 0f;
		}
	}
}
