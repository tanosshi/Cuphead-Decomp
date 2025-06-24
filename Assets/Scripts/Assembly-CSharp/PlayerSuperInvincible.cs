using System.Collections;
using UnityEngine;

public class PlayerSuperInvincible : AbstractPlayerSuper
{
	private const float maxShadowDistance = 500f;

	[SerializeField]
	private Material superMaterial;

	[SerializeField]
	private Effect sparkle;

	[SerializeField]
	private float sparkleSpawnTime;

	[SerializeField]
	private Vector3 shadowOffset;

	[SerializeField]
	private GameObject shadowCuphead;

	[SerializeField]
	private GameObject shadowMugman;

	private GameObject shadow;

	protected override void StartSuper()
	{
		base.StartSuper();
		AudioManager.Play("player_super_invincibility");
		if (player.id == PlayerId.PlayerOne)
		{
			shadowMugman.SetActive(false);
			shadow = shadowCuphead;
		}
		else
		{
			shadowCuphead.SetActive(false);
			shadow = shadowMugman;
		}
		base.transform.position = player.transform.position;
		StartCoroutine(super_cr());
		if (!player.motor.Grounded)
		{
			shadow.SetActive(false);
			shadow.transform.position = player.GetComponent<LevelPlayerShadow>().ShadowPosition() + shadowOffset;
		}
		Level.ScoringData.superMeterUsed += 5;
	}

	private IEnumerator super_cr()
	{
		player.stats.SetInvincible(true);
		yield return CupheadTime.WaitForSeconds(this, WeaponProperties.LevelSuperInvincibility.durationInvincible);
		player.stats.SetInvincible(false);
		yield return null;
	}

	private IEnumerator invincibility_fx_cr()
	{
		IEnumerator sparkleRoutine = sparkle_cr();
		StartCoroutine(sparkleRoutine);
		player.animationController.SetMaterial(superMaterial);
		yield return CupheadTime.WaitForSeconds(this, WeaponProperties.LevelSuperInvincibility.durationFX - 1.25f);
		AudioManager.ChangeBGMPitch(1.8f, 1.5f);
		for (int i = 0; i < 5; i++)
		{
			player.animationController.SetOldMaterial();
			yield return CupheadTime.WaitForSeconds(this, 0.125f);
			player.animationController.SetMaterial(superMaterial);
			yield return CupheadTime.WaitForSeconds(this, 0.125f);
		}
		AudioManager.ChangeBGMPitch(1f, 1.5f);
		player.animationController.SetOldMaterial();
		StopCoroutine(sparkleRoutine);
		yield return null;
	}

	private IEnumerator sparkle_cr()
	{
		while (true)
		{
			float x = Random.Range(0f - player.colliderManager.Width, player.colliderManager.Width);
			float y = Random.Range(player.colliderManager.Height * -0.5f, player.colliderManager.Height * 1.5f);
			sparkle.Create(player.transform.position + new Vector3(x, y, 0f));
			yield return CupheadTime.WaitForSeconds(this, sparkleSpawnTime);
		}
	}

	private void EndPlayerAnimation()
	{
		Fire();
		EndSuper(false);
		StartCoroutine(invincibility_fx_cr());
		StartCoroutine(super_cr());
		player.animationController.SetSpriteProperties(SpriteLayer.Effects, 3000);
	}

	private void BigCupAppears()
	{
		if (!player.motor.Grounded)
		{
			shadow.SetActive(true);
			float num = Mathf.Abs(player.transform.position.y - shadow.transform.position.y);
			float num2 = Mathf.Max(0f, 1f - num / 500f);
			shadow.transform.localScale = Vector3.one * num2;
		}
	}

	private void ResetSpriteOrder()
	{
		player.animationController.ResetSpriteProperties();
	}
}
