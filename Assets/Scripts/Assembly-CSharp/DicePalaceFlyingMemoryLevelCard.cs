using System.Collections;
using UnityEngine;

public class DicePalaceFlyingMemoryLevelCard : ParrySwitch
{
	public enum Card
	{
		Cuphead = 0,
		Chips = 1,
		Flowers = 2,
		Shield = 3,
		Spindle = 4,
		Mugman = 5
	}

	public bool permanentlyFlipped;

	private const float ROTATION_TIME = 0.6f;

	private const float ROTATION_BACK = 360f;

	private EaseUtils.EaseType ROTATION_EASE = EaseUtils.EaseType.easeOutBack;

	[SerializeField]
	private Sprite[] flippedUpCards;

	[SerializeField]
	private SpriteRenderer pinkDot;

	private Sprite flippedUpCard;

	private Sprite flippedDownCard;

	private Coroutine rotationCoroutine;

	private float rotation;

	private float fadeTime = 0.7f;

	public Card card;

	public bool flippedUp { get; private set; }

	protected override void Awake()
	{
		base.Awake();
		flippedUp = false;
		flippedDownCard = GetComponent<SpriteRenderer>().sprite;
	}

	public void FlipUp()
	{
		StartCoroutine(rotate_cr(rotation, 360f, 0.6f));
		flippedUpCard = flippedUpCards[(int)card];
		GetComponent<SpriteRenderer>().sprite = flippedUpCard;
		flippedUp = true;
		pinkDot.enabled = false;
	}

	public void EnableCards()
	{
		if (!permanentlyFlipped)
		{
			if (flippedUp)
			{
				StartCoroutine(rotate_cr(rotation, 360f, 0.6f));
				GetComponent<SpriteRenderer>().sprite = flippedDownCard;
				flippedUp = false;
			}
			pinkDot.enabled = true;
			StartCoroutine(fade_pink_cr(false));
			GetComponent<Collider2D>().enabled = true;
		}
	}

	public void DisableCard()
	{
		GetComponent<Collider2D>().enabled = false;
		if (!flippedUp || !permanentlyFlipped)
		{
			StartCoroutine(fade_pink_cr(true));
		}
	}

	private IEnumerator rotate_cr(float start, float end, float time)
	{
		float t = 0f;
		while (t < time)
		{
			float val = t / time;
			base.transform.SetEulerAngles(0f, EaseUtils.Ease(ROTATION_EASE, start, end, val), 0f);
			t += Time.deltaTime;
			yield return null;
		}
		base.transform.SetEulerAngles(0f, 0f, 0f);
		yield return null;
	}

	public override void OnParryPostPause(AbstractPlayerController player)
	{
		base.OnParryPostPause(player);
		FlipUp();
	}

	private IEnumerator fade_pink_cr(bool fadingOut)
	{
		if (fadingOut)
		{
			float t = 0f;
			while (t < fadeTime)
			{
				pinkDot.color = new Color(1f, 1f, 1f, 1f - t / fadeTime);
				t += (float)CupheadTime.Delta;
				yield return null;
			}
			pinkDot.color = new Color(1f, 1f, 1f, 0f);
		}
		else
		{
			float t2 = 0f;
			while (t2 < fadeTime)
			{
				pinkDot.color = new Color(1f, 1f, 1f, t2 / fadeTime);
				t2 += (float)CupheadTime.Delta;
				yield return null;
			}
			pinkDot.color = new Color(1f, 1f, 1f, 1f);
		}
	}
}
