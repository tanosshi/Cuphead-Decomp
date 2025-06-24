using System.Collections;
using UnityEngine;

public class TreePlatformingLevelAudioArea : AbstractPausableComponent
{
	[SerializeField]
	private Transform startPoint;

	[SerializeField]
	private Transform endPoint;

	private bool isFading;

	protected override void Start()
	{
		base.Start();
		StartCoroutine(check_sound_cr());
	}

	private void PlaySound()
	{
		AudioManager.PlayLoop("amb_treecave");
		StartCoroutine(fade_volume_cr(true));
	}

	private void StopSound()
	{
		StartCoroutine(fade_volume_cr(false));
	}

	private IEnumerator fade_volume_cr(bool fadeIn)
	{
		isFading = true;
		float time = 1f;
		float t = 0f;
		while (t < time)
		{
			t += (float)CupheadTime.Delta;
			AudioManager.Attenuation("amb_treecave", true, (!fadeIn) ? (1f - t / time) : (t / time));
			yield return null;
		}
		if (!fadeIn)
		{
			AudioManager.Stop("amb_treecave");
		}
		isFading = false;
		yield return null;
	}

	private IEnumerator check_sound_cr()
	{
		yield return CupheadTime.WaitForSeconds(this, 0.2f);
		AbstractPlayerController player = PlayerManager.GetNext();
		while (true)
		{
			if (player.transform.position.x > startPoint.transform.position.x && player.transform.position.x < endPoint.transform.position.x)
			{
				if (!AudioManager.CheckIfPlaying("amb_treecave"))
				{
					PlaySound();
				}
			}
			else if (AudioManager.CheckIfPlaying("amb_treecave") && !isFading)
			{
				StopSound();
			}
			yield return null;
		}
	}

	private IEnumerator play_one_shots_cr()
	{
		MinMax delay = new MinMax(4f, 8f);
		while (true)
		{
			if (AudioManager.CheckIfPlaying("amb_treecave"))
			{
				yield return CupheadTime.WaitForSeconds(this, delay);
				AudioManager.Play("NAME");
			}
			yield return null;
		}
	}

	protected override void OnDrawGizmos()
	{
		base.OnDrawGizmos();
		Gizmos.color = Color.red;
		Gizmos.DrawLine(new Vector2(startPoint.position.x, startPoint.position.y + 1000f), new Vector2(startPoint.position.x, startPoint.position.y - 1000f));
		Gizmos.color = Color.blue;
		Gizmos.DrawLine(new Vector2(endPoint.position.x, endPoint.position.y + 1000f), new Vector2(endPoint.position.x, endPoint.position.y - 1000f));
	}
}
