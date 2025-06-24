using System.Collections;

public class PlanePlayerAudioController : AbstractPlanePlayerComponent
{
	protected override void OnAwake()
	{
		base.OnAwake();
	}

	protected override void Start()
	{
		base.Start();
		base.player.damageReceiver.OnDamageTaken += OnDamageTaken;
		AudioManager.PlayLoop("player_plane_engine");
	}

	public void LevelInit()
	{
	}

	private void OnDamageTaken(DamageDealer.DamageInfo info)
	{
		if (info.damage > 0f)
		{
			AudioManager.Play(Sfx.Player_Plane_Hit);
			if (base.player.stats.Health > 0)
			{
				StartCoroutine(change_pitch_cr());
			}
		}
	}

	private IEnumerator change_pitch_cr()
	{
		if (base.player.stats.Health == 1)
		{
			AudioManager.Play("player_damage_crack_level4");
			emitAudioFromObject.Add("player_damage_crack_level4");
			AudioManager.ChangeSFXPitch("player_plane_engine", 0.3f, 0.4f);
		}
		else if (base.player.stats.Health == 2)
		{
			AudioManager.Play("player_damage_crack_level3");
			emitAudioFromObject.Add("player_damage_crack_level3");
			AudioManager.ChangeSFXPitch("player_plane_engine", 0.35f, 0.4f);
		}
		else if (base.player.stats.Health == 3)
		{
			AudioManager.Play("player_damage_crack_level2");
			emitAudioFromObject.Add("player_damage_crack_level2");
			AudioManager.ChangeSFXPitch("player_plane_engine", 0.4f, 0.4f);
		}
		else
		{
			AudioManager.Play("player_damage_crack_level1");
			emitAudioFromObject.Add("player_damage_crack_level1");
			AudioManager.ChangeSFXPitch("player_plane_engine", 0.5f, 0.4f);
		}
		yield return CupheadTime.WaitForSeconds(this, 1f);
		AudioManager.ChangeSFXPitch("player_plane_engine", 1f, 1f);
		yield return null;
	}
}
