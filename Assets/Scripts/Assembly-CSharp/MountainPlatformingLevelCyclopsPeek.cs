using System.Collections;
using UnityEngine;

public class MountainPlatformingLevelCyclopsPeek : AbstractPausableComponent
{
	public enum EyeState
	{
		left = 0,
		middle = 1,
		right = 2
	}

	private EyeState currentEyeState;

	private float offset = 300f;

	private AbstractPlayerController player;

	protected override void Start()
	{
		base.Start();
		StartCoroutine(check_player_cr());
		emitAudioFromObject.Add("castle_giant_head_peer");
	}

	private IEnumerator check_player_cr()
	{
		yield return CupheadTime.WaitForSeconds(this, 0.2f);
		player = PlayerManager.GetNext();
		float t = 0f;
		float time = Random.Range(3f, 6f);
		float laughTime = Random.Range(6f, 10f);
		float t_laugh = 0f;
		while (true)
		{
			if (PlayerManager.GetPlayer(PlayerId.PlayerTwo) != null)
			{
				t += (float)CupheadTime.Delta;
				if (t >= time)
				{
					player = PlayerManager.GetNext();
					t = 0f;
					time = Random.Range(3f, 6f);
				}
			}
			if (Vector3.Distance(PlayerManager.GetPlayer(PlayerId.PlayerOne).transform.position, base.transform.position) < 1000f)
			{
				t_laugh += (float)CupheadTime.Delta;
				if (t_laugh >= laughTime)
				{
					SoundGiantHeadPeer();
					t_laugh = 0f;
					laughTime = Random.Range(6f, 10f);
				}
			}
			if (player.transform.position.x < base.transform.position.x - offset)
			{
				if (currentEyeState != EyeState.left)
				{
					base.animator.SetInteger("SideOn", 0);
					currentEyeState = EyeState.left;
				}
			}
			else if (player.transform.position.x > base.transform.position.x + offset)
			{
				if (currentEyeState != EyeState.right)
				{
					base.animator.SetInteger("SideOn", 2);
					currentEyeState = EyeState.right;
				}
			}
			else if (currentEyeState != EyeState.middle)
			{
				base.animator.SetInteger("SideOn", 1);
				currentEyeState = EyeState.middle;
			}
			yield return null;
		}
	}

	private void SoundGiantHeadPeer()
	{
		AudioManager.Play("castle_giant_head_peer");
		emitAudioFromObject.Add("castle_giant_head_peer");
	}
}
