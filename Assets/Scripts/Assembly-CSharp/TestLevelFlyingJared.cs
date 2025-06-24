using System.Collections;
using UnityEngine;

public class TestLevelFlyingJared : LevelProperties.Test.Entity
{
	[SerializeField]
	private Transform childSprite;

	private DamageReceiver damageReceiver;

	[SerializeField]
	private AudioSource audioClip;

	public override void LevelInit(LevelProperties.Test properties)
	{
		base.LevelInit(properties);
		damageReceiver = GetComponent<DamageReceiver>();
		damageReceiver.OnDamageTaken += OnDamageTaken;
		Level.Current.OnLevelStartEvent += OnLevelStart;
		AudioManager.PlayLoop("test_sound");
		emitAudioFromObject.Add("test_sound");
	}

	private void OnDamageTaken(DamageDealer.DamageInfo info)
	{
		base.properties.DealDamage(info.damage);
		GetComponent<AudioWarble>().HandleWarble();
	}

	private void OnLevelStart()
	{
		StartCoroutine(moveX_cr());
		StartCoroutine(moveY_cr());
		StartCoroutine(scale_cr());
	}

	private float GetHealthTimeX()
	{
		float i = 1f - base.properties.CurrentHealth / base.properties.TotalHealth;
		return base.properties.CurrentState.moving.timeX.GetFloatAt(i);
	}

	private float GetHealthTimeY()
	{
		float i = 1f - base.properties.CurrentHealth / base.properties.TotalHealth;
		return base.properties.CurrentState.moving.timeY.GetFloatAt(i);
	}

	private float GetHealthTimeScale()
	{
		float i = 1f - base.properties.CurrentHealth / base.properties.TotalHealth;
		return base.properties.CurrentState.moving.timeScale.GetFloatAt(i);
	}

	private IEnumerator moveX_cr()
	{
		float start = base.transform.position.x;
		float end = 0f - start;
		while (true)
		{
			childSprite.transform.SetScale(1f);
			yield return TweenLocalPositionX(start, end, GetHealthTimeX(), EaseUtils.EaseType.easeInOutSine);
			childSprite.transform.SetScale(-1f);
			yield return TweenLocalPositionX(end, start, GetHealthTimeX(), EaseUtils.EaseType.easeInOutSine);
		}
	}

	private IEnumerator moveY_cr()
	{
		float start = base.transform.position.y;
		float end = start - 100f;
		while (true)
		{
			yield return TweenLocalPositionY(start, end, GetHealthTimeY(), EaseUtils.EaseType.easeInOutSine);
			yield return TweenLocalPositionY(end, start, GetHealthTimeY(), EaseUtils.EaseType.easeInOutSine);
		}
	}

	private IEnumerator scale_cr()
	{
		Vector3 start = new Vector3(1f, 1f, 1f);
		Vector3 end = new Vector3(2f, 2f, 2f);
		while (true)
		{
			yield return TweenScale(start, end, GetHealthTimeScale(), EaseUtils.EaseType.easeInOutSine);
			yield return TweenScale(end, start, GetHealthTimeScale(), EaseUtils.EaseType.easeInOutSine);
		}
	}
}
