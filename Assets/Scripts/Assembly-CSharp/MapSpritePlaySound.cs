using UnityEngine;

public class MapSpritePlaySound : AbstractCollidableObject
{
	public enum SoundToPlay
	{
		Wood = 0,
		Rainbow = 1
	}

	public SoundToPlay getSound;

	protected override void OnCollision(GameObject hit, CollisionPhase phase)
	{
		base.OnCollision(hit, phase);
	}

	public void PlaySoundRight(bool isP1)
	{
		switch (getSound)
		{
		case SoundToPlay.Wood:
			AudioManager.Play((!isP1) ? "player_map_walk_wood_two_p2" : "player_map_walk_wood_two_p1");
			break;
		}
	}

	public void PlaySoundLeft(bool isP1)
	{
		switch (getSound)
		{
		case SoundToPlay.Wood:
			AudioManager.Play((!isP1) ? "player_map_walk_wood_one_p2" : "player_map_walk_wood_one_p1");
			break;
		}
	}
}
