using System.Collections;
using UnityEngine;

public class TreePlatformingLevelBeetleSpawner : PlatformingLevelEnemySpawner
{
	private TreePlatformingLevelBeetle[] beetles;

	protected override void Awake()
	{
		base.Awake();
		beetles = new TreePlatformingLevelBeetle[GetComponentsInChildren<TreePlatformingLevelBeetle>().Length];
		beetles = GetComponentsInChildren<TreePlatformingLevelBeetle>();
	}

	protected override void Start()
	{
		base.Start();
		StartCoroutine(check_to_play_sfx());
	}

	protected override void Spawn()
	{
		base.Spawn();
		Activate();
	}

	private void Activate()
	{
		int num = Random.Range(0, beetles.Length);
		if (!beetles[num].isActivated)
		{
			beetles[num].Activate();
		}
	}

	private IEnumerator check_to_play_sfx()
	{
		while (true)
		{
			TreePlatformingLevelBeetle[] array = beetles;
			foreach (TreePlatformingLevelBeetle treePlatformingLevelBeetle in array)
			{
				if (treePlatformingLevelBeetle.onCamera)
				{
					treePlatformingLevelBeetle.PlayIdleSFX();
				}
			}
			yield return null;
		}
	}
}
