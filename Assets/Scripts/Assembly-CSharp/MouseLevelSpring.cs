using System.Collections;
using UnityEngine;

public class MouseLevelSpring : ParrySwitch
{
	[SerializeField]
	private Effect smallExplosion;

	public float knockUpHeight = 100f;

	private bool launchedPlayer;

	private bool isLaunched;

	private Vector2 velocity;

	private float gravity;

	private float offset = 120f;

	protected override void OnCollision(GameObject hit, CollisionPhase phase)
	{
		base.OnCollision(hit, phase);
		if (phase == CollisionPhase.Enter)
		{
			if (!(hit.GetComponent<LevelPlayerMotor>() != null) || !launchedPlayer)
			{
			}
			if (hit.GetComponent<MouseLevelCanMouse>() != null && !isLaunched)
			{
				smallExplosion.Create(base.transform.position);
				base.animator.SetTrigger("OnDeath");
			}
		}
	}

	public override void OnParryPrePause(AbstractPlayerController player)
	{
		base.OnParryPrePause(player);
		player.GetComponent<LevelPlayerMotor>().OnTrampolineKnockUp(knockUpHeight);
		launchedPlayer = true;
		StartCoroutine(check_player_cr(player.GetComponent<LevelPlayerMotor>()));
		base.animator.SetTrigger("OnLaunch");
	}

	private void GotRunOver()
	{
		GetComponent<Collider2D>().enabled = false;
		base.gameObject.SetActive(false);
	}

	private IEnumerator check_player_cr(LevelPlayerMotor player)
	{
		while (!player.Grounded)
		{
			yield return null;
		}
		launchedPlayer = false;
		yield return null;
	}

	public void LaunchSpring(Vector2 position, Vector2 velocity, float gravity)
	{
		base.transform.position = position;
		this.velocity = velocity;
		this.gravity = gravity;
		isLaunched = true;
		launchedPlayer = false;
		base.gameObject.SetActive(true);
	}

	protected override void Update()
	{
		base.Update();
		if (!isLaunched)
		{
			return;
		}
		base.transform.AddPosition(velocity.x * (float)CupheadTime.Delta, velocity.y * (float)CupheadTime.Delta);
		velocity.y -= gravity * (float)CupheadTime.Delta;
		if (base.transform.position.y < (float)Level.Current.Ground + offset)
		{
			base.transform.SetPosition(null, (float)Level.Current.Ground + offset);
			if (isLaunched)
			{
				Landed();
			}
			isLaunched = false;
		}
	}

	private void Landed()
	{
		GetComponent<Collider2D>().enabled = true;
		base.animator.SetTrigger("OnLand");
		AudioManager.Play("level_mouse_can_springboard_land");
		emitAudioFromObject.Add("level_mouse_can_springboard_land");
	}
}
