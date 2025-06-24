using System.Collections;
using UnityEngine;

public class PlaneSuperBomb : AbstractPlaneSuper
{
	private bool earlyExplosion;

	[SerializeField]
	private Transform boom;

	[SerializeField]
	private Transform boomMM;

	protected override void StartSuper()
	{
		base.StartSuper();
		player.damageReceiver.OnDamageTaken += OnDamageTaken;
		player.stats.OnStoned += OnStoned;
		if (player.id == PlayerId.PlayerOne)
		{
			boom.gameObject.SetActive(true);
		}
		else
		{
			boomMM.gameObject.SetActive(true);
		}
	}

	private IEnumerator super_cr()
	{
		float t = 0f;
		damageDealer = new DamageDealer(WeaponProperties.PlaneSuperBomb.damage, WeaponProperties.PlaneSuperBomb.damageRate, DamageDealer.DamageSource.Super, false, true, true);
		damageDealer.DamageMultiplier *= PlayerManager.DamageMultiplier;
		damageDealer.PlayerId = player.id;
		MeterScoreTracker tracker = new MeterScoreTracker(MeterScoreTracker.Type.Super);
		tracker.Add(damageDealer);
		while (t < WeaponProperties.PlaneSuperBomb.countdownTime && !earlyExplosion)
		{
			t += (float)CupheadTime.Delta;
			yield return null;
		}
		Fire();
		player.PauseAll();
		base.transform.position = player.transform.position;
		base.animator.SetTrigger("Explode");
		AudioManager.Stop("player_plane_bomb_ticktock_loop");
		AudioManager.Play("player_plane_bomb_explosion");
	}

	private void OnStoned()
	{
		earlyExplosion = true;
	}

	private void OnDamageTaken(DamageDealer.DamageInfo info)
	{
		earlyExplosion = true;
	}

	private void EndIntroAnimation()
	{
		StartCountdown();
		AudioManager.PlayLoop("player_plane_bomb_ticktock_loop");
		StartCoroutine(super_cr());
	}

	private void PlayerReappear()
	{
		player.UnpauseAll();
	}

	private void Die()
	{
		Object.Destroy(base.gameObject);
	}

	private void StartBoomScale()
	{
		StartCoroutine(boomScale_cr());
	}

	private IEnumerator boomScale_cr()
	{
		float t = 0f;
		float frameTime = 1f / 24f;
		float scale = 1f;
		while (true)
		{
			t += (float)CupheadTime.Delta;
			while (t > frameTime)
			{
				t -= frameTime;
				scale *= 1.15f;
				boom.SetScale(scale, scale);
				boomMM.SetScale(scale, scale);
			}
			yield return null;
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		player.damageReceiver.OnDamageTaken -= OnDamageTaken;
		player.stats.OnStoned -= OnStoned;
	}

	private void PlaneSuperBombLaughAudio()
	{
		AudioManager.Play("player_plane_bomb_laugh");
	}
}
