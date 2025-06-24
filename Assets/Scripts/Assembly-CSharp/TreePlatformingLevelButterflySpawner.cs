using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreePlatformingLevelButterflySpawner : AbstractPausableComponent
{
	private const int WAVE_MAX = 20;

	[SerializeField]
	private TreePlatformingLevelButterfly butterflySmall;

	[SerializeField]
	private Transform hoardRoot;

	private List<TreePlatformingLevelButterfly> butterflies;

	private MinMax delay = new MinMax(1.5f, 3f);

	private MinMax velocity = new MinMax(100f, 200f);

	private float initalDelay = 6f;

	protected override void Awake()
	{
		base.Awake();
		StartCoroutine(spawn_hoard_cr());
		StartCoroutine(spawner_cr());
	}

	protected override void Start()
	{
		base.Start();
	}

	private IEnumerator spawn_hoard_cr()
	{
		MinMax spreadAngle = new MinMax(0f, 85f);
		int WAVES = 3;
		butterflies = new List<TreePlatformingLevelButterfly>();
		yield return CupheadTime.WaitForSeconds(this, 0.1f);
		for (int n = 0; n < WAVES; n++)
		{
			for (int i = 0; i < 20; i++)
			{
				float floatAt = spreadAngle.GetFloatAt((float)i / 19f);
				float num = spreadAngle.max / 2f;
				floatAt -= num;
				TreePlatformingLevelButterfly treePlatformingLevelButterfly = Object.Instantiate(butterflySmall);
				int color = ((!(treePlatformingLevelButterfly == butterflySmall)) ? Random.Range(1, 3) : Random.Range(1, 2));
				treePlatformingLevelButterfly.Init(hoardRoot.transform.position, 55f + floatAt, Random.Range(1200f, 1850f), color, Random.Range(1f, 2f), 0.6f);
				treePlatformingLevelButterfly.transform.parent = base.transform;
				butterflies.Add(treePlatformingLevelButterfly);
			}
			yield return CupheadTime.WaitForSeconds(this, 0.3f);
		}
		yield return CupheadTime.WaitForSeconds(this, 0.8f);
		TreePlatformingLevelButterfly tiny = Object.Instantiate(butterflySmall);
		tiny.Init(hoardRoot.transform.position, 55f, Random.Range(800f, 1000f), Random.Range(1, 3), Random.Range(0.7f, 1f), 0.6f);
		yield return null;
	}

	private IEnumerator spawner_cr()
	{
		bool keepChecking = true;
		TreePlatformingLevelButterfly spawn = null;
		yield return CupheadTime.WaitForSeconds(this, initalDelay);
		while (true)
		{
			keepChecking = true;
			while (keepChecking)
			{
				foreach (TreePlatformingLevelButterfly butterfly in butterflies)
				{
					if (!butterfly.isActive)
					{
						spawn = butterfly;
						keepChecking = false;
						break;
					}
				}
				yield return null;
			}
			bool onLeft = Rand.Bool();
			float y = Random.Range(CupheadLevelCamera.Current.transform.position.y + 500f, CupheadLevelCamera.Current.transform.position.y - 500f);
			float cameraLeft = CupheadLevelCamera.Current.transform.position.x - 1500f;
			float cameraRight = CupheadLevelCamera.Current.transform.position.x + 1500f;
			if (onLeft)
			{
				spawn.transform.position = new Vector3(cameraLeft, y);
				spawn.Init(velocity.RandomFloat(), 1f, Random.Range(1, 4));
			}
			else
			{
				spawn.transform.position = new Vector3(cameraRight, y);
				spawn.Init(0f - velocity.RandomFloat(), -1f, Random.Range(1, 4));
			}
			yield return CupheadTime.WaitForSeconds(this, delay.RandomFloat());
			yield return null;
		}
	}
}
