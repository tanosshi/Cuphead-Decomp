using UnityEngine;

public class MountainLevelGiantPlatform : LevelPlatform
{
	[SerializeField]
	private Effect explosion;

	[SerializeField]
	private SpriteDeathParts[] sprites;

	protected override void OnCollisionEnemy(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionEnemy(hit, phase);
		if (phase == CollisionPhase.Enter && (bool)hit.GetComponent<MountainPlatformingLevelCyclops>())
		{
			explosion.Create(base.transform.position);
			SpawnParts();
			if (base.transform.childCount > 0 && (bool)GetComponentInChildren<LevelPlayerMotor>())
			{
				GetComponentInChildren<LevelPlayerMotor>().OnPitKnockUp(10f);
			}
			Object.Destroy(base.gameObject);
		}
	}

	private void SpawnParts()
	{
		SpriteDeathParts[] array = sprites;
		foreach (SpriteDeathParts spriteDeathParts in array)
		{
			spriteDeathParts.CreatePart(base.transform.position);
		}
	}
}
