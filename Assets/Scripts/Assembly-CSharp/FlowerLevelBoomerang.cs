using System.Collections;
using UnityEngine;

public class FlowerLevelBoomerang : BasicProjectile
{
	private SpriteRenderer shadowSprite;

	private float returnXPosition;

	private float endXPosition;

	private float offScreenDelay;

	private int BoomerangNumberSFX;

	protected override bool DestroyedAfterLeavingScreen
	{
		get
		{
			return false;
		}
	}

	public void OnBoomerangStart(float delay)
	{
		BoomerangNumberSFX++;
		if (BoomerangNumberSFX == 1)
		{
			AudioManager.FadeSFXVolume("flower_boomerang_1", 1f, 1f);
			AudioManager.PlayLoop("flower_boomerang_1");
			emitAudioFromObject.Add("flower_boomerang_1");
		}
		else if (BoomerangNumberSFX != 1)
		{
			AudioManager.FadeSFXVolume("flower_boomerang_2", 1f, 1f);
			AudioManager.PlayLoop("flower_boomerang_2");
			emitAudioFromObject.Add("flower_boomerang_2");
		}
		shadowSprite = base.transform.GetChild(0).GetComponent<SpriteRenderer>();
		offScreenDelay = delay;
		returnXPosition = Level.Current.Left - 100;
		endXPosition = Level.Current.Right + 500;
		StartCoroutine(boomerangStart_cr());
	}

	private IEnumerator boomerangStart_cr()
	{
		base.transform.GetChild(0).transform.position = new Vector3(base.transform.position.x, Level.Current.Ground, 0f);
		while (base.transform.position.x > returnXPosition)
		{
			yield return null;
		}
		move = false;
		yield return CupheadTime.WaitForSeconds(this, offScreenDelay);
		OnBoomerangReturn();
	}

	private void OnBoomerangReturn()
	{
		Speed = 0f - Speed;
		move = true;
		base.transform.position = new Vector3(base.transform.position.x, Level.Current.Ground + Level.Current.Height / 6, 0f);
		StartCoroutine(boomerangReturn_cr());
	}

	private IEnumerator boomerangReturn_cr()
	{
		base.transform.GetChild(0).transform.position = new Vector3(base.transform.position.x, Level.Current.Ground, 0f);
		while (base.transform.position.x < endXPosition)
		{
			yield return null;
		}
		if (BoomerangNumberSFX == 1)
		{
			AudioManager.FadeSFXVolume("flower_boomerang_1", 0f, 3f);
			AudioManager.FadeSFXVolume("flower_boomerang_2", 0f, 3f);
		}
		else if (BoomerangNumberSFX != 1)
		{
			AudioManager.FadeSFXVolume("flower_boomerang_1", 0f, 3f);
			AudioManager.FadeSFXVolume("flower_boomerang_2", 0f, 3f);
		}
		BoomerangNumberSFX--;
		Object.Destroy(base.gameObject);
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		if (base.damageDealer != null)
		{
			base.damageDealer.DealDamage(hit);
		}
		base.OnCollisionPlayer(hit, phase);
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
	}

	public override void OnLevelEnd()
	{
		AudioManager.Stop("flower_boomerang_1");
		AudioManager.Stop("flower_boomerang_2");
		base.OnLevelEnd();
	}
}
