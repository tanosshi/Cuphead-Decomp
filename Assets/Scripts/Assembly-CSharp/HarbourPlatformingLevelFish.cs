using System.Collections;
using UnityEngine;

public class HarbourPlatformingLevelFish : AbstractPlatformingLevelEnemy
{
	[SerializeField]
	private SpriteRenderer blinkLayer;

	private string letters = "A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P";

	private string type;

	private float rotation;

	private float velocity;

	private float sinVelocity;

	private float sinSize;

	private int num;

	private bool isA;

	private int blinkCounter;

	private int blinkCounterMax;

	public void Init(Vector2 pos, float rotation, float velocity, float sinVelocity, float sinSize, string type)
	{
		base.transform.position = pos;
		this.velocity = velocity;
		this.sinVelocity = sinVelocity;
		this.sinSize = sinSize;
		this.type = type;
		this.rotation = rotation;
	}

	protected override void OnStart()
	{
	}

	protected override void Start()
	{
		blinkLayer.enabled = false;
		blinkCounterMax = Random.Range(10, 20);
		base.transform.SetScale((rotation != 180f) ? (0f - base.transform.localScale.x) : base.transform.localScale.x);
		for (int i = 0; i < 5; i++)
		{
			if (type.Substring(0, 1) == letters.Split(',')[i])
			{
				num = i + 1;
			}
		}
		isA = type.Substring(1, 1) == "A";
		base.animator.SetInteger("Type", num);
		base.animator.SetBool("Is1", isA);
		if (num == 4)
		{
			_canParry = true;
		}
		StartCoroutine(movement_cr());
		base.Start();
	}

	private void FlyingFishSFX()
	{
		AudioManager.Play("harbour_flying_fish_idle");
		emitAudioFromObject.Add("harbour_flying_fish_idle");
	}

	private IEnumerator movement_cr()
	{
		float angle = 0f;
		Vector3 xVelocity = Vector3.zero;
		while (true)
		{
			angle += base.Properties.flyingFishSinVelocity * (float)CupheadTime.Delta;
			xVelocity = ((rotation != 180f) ? base.transform.right : (-base.transform.right));
			Vector3 moveY = new Vector3(0f, Mathf.Sin(angle) * (float)CupheadTime.Delta * 60f * base.Properties.flyingFishSinSize);
			Vector3 moveX = xVelocity * base.Properties.flyingFishVelocity * CupheadTime.Delta;
			if ((float)CupheadTime.Delta != 0f)
			{
				base.transform.position += moveX + moveY;
			}
			if (!CupheadLevelCamera.Current.ContainsPoint(base.transform.position, AbstractPlatformingLevelEnemy.CAMERA_DEATH_PADDING))
			{
				Die();
				AudioManager.Stop("harbour_flying_fish_idle");
			}
			yield return null;
		}
	}

	private void IncrementBlinkCounter()
	{
		FlyingFishSFX();
		if (blinkCounter < blinkCounterMax)
		{
			blinkLayer.enabled = false;
			blinkCounter++;
		}
		else
		{
			blinkLayer.enabled = true;
			blinkCounter = 0;
			blinkCounterMax = Random.Range(5, 10);
		}
	}

	protected override void Die()
	{
		base.Die();
		AudioManager.Stop("harbour_flying_fish_idle");
	}
}
