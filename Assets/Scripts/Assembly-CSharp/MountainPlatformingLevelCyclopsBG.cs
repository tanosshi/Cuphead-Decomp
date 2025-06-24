using System.Collections;
using UnityEngine;

public class MountainPlatformingLevelCyclopsBG : AbstractPausableComponent
{
	[SerializeField]
	private SpriteRenderer eye;

	[SerializeField]
	private float rockDelay;

	[SerializeField]
	private float rockSpeed;

	[SerializeField]
	private int rockCount;

	[SerializeField]
	private MountainPlatformingLevelRock projectile;

	private int blinkCounter;

	private int blinkCounterMax;

	private float sizeY;

	private bool isAltPattern;

	private AbstractPlayerController player;

	public Vector3 start;

	public bool isDead;

	public bool isWalking { get; private set; }

	protected override void Start()
	{
		base.Start();
		sizeY = GetComponent<SpriteRenderer>().bounds.size.y;
		blinkCounterMax = Random.Range(3, 6);
		eye.enabled = false;
	}

	private void OnTurn()
	{
		base.transform.SetScale(0f - base.transform.localScale.x);
		if (base.transform.localScale.x == 1f)
		{
			base.transform.AddPosition(-47f);
		}
		else
		{
			base.transform.AddPosition(47f);
		}
	}

	private void DropRocks()
	{
		StartCoroutine(drop_rocks_cr());
	}

	private void StopWalking()
	{
		isWalking = false;
	}

	private void StartWalking()
	{
		isWalking = true;
	}

	public void GetPlayer(AbstractPlayerController player)
	{
		this.player = player;
	}

	private IEnumerator drop_rocks_cr()
	{
		for (int i = 0; i < rockCount; i++)
		{
			Vector2 zero = Vector2.zero;
			zero.y = CupheadLevelCamera.Current.Bounds.yMax + 200f;
			zero.x = CupheadLevelCamera.Current.Bounds.xMin + projectile.GetComponent<Renderer>().bounds.size.x / 2f + projectile.GetComponent<Renderer>().bounds.size.x * (float)i;
			projectile.Create(base.transform.position, zero, rockSpeed, rockDelay * (float)i);
		}
		yield return null;
	}

	private void SlideUp()
	{
		if (!isDead)
		{
			StartCoroutine(slide_up_cr());
		}
	}

	private void SlideDown()
	{
		StartCoroutine(slide_down_cr());
	}

	private IEnumerator slide_up_cr()
	{
		player = PlayerManager.GetNext();
		base.transform.SetPosition(player.transform.position.x);
		float t = 0f;
		float time = 0.4f;
		float startPos = base.transform.position.y;
		float endPos = start.y;
		while (t < time)
		{
			t += (float)CupheadTime.Delta;
			base.transform.SetPosition(null, Mathf.Lerp(startPos, endPos, t / time));
			yield return null;
		}
	}

	private IEnumerator slide_down_cr()
	{
		float t = 0f;
		float time = 0.8f;
		float startPos = base.transform.position.y;
		float endPos = start.y - sizeY;
		while (t < time)
		{
			t += (float)CupheadTime.Delta;
			base.transform.SetPosition(null, Mathf.Lerp(startPos, endPos, t / time));
			yield return null;
		}
		if (isDead)
		{
			base.gameObject.SetActive(false);
		}
		else
		{
			yield return CupheadTime.WaitForSeconds(this, 0.8f);
			base.animator.SetTrigger("Continue");
		}
		yield return null;
	}

	private void BlinkMaybe()
	{
		if (blinkCounter < blinkCounterMax)
		{
			eye.enabled = false;
			blinkCounter++;
		}
		else
		{
			eye.enabled = true;
			blinkCounter = 0;
			blinkCounterMax = Random.Range(3, 6);
		}
	}

	private void SoundGiantRockThrow()
	{
		AudioManager.Play("castle_giant_rock_throw");
		emitAudioFromObject.Add("castle_giant_rock_throw");
	}

	private void SoundGiantRockThrowAppear()
	{
		AudioManager.Play("castle_giant_rock_throw_appear");
		emitAudioFromObject.Add("castle_giant_rock_throw_appear");
	}

	private void SoundGiantStartle()
	{
		AudioManager.Play("castle_giant_startle");
		emitAudioFromObject.Add("castle_giant_startle");
	}
}
