using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaronessLevelForegroundChange : AbstractPausableComponent
{
	[Serializable]
	public class ScrollingSpriteInfo
	{
		public SpriteRenderer sprite;

		public float weight = 1f;
	}

	public const float X_IN = -1280f;

	public const float X_OUT = 1280f;

	[SerializeField]
	private float spawnY;

	[SerializeField]
	[Range(0f, -2000f)]
	private float speed;

	[SerializeField]
	[Range(0f, -2000f)]
	private float minSpacing;

	[SerializeField]
	[Range(0f, -2000f)]
	private float averageSpacing;

	[SerializeField]
	private int sortingOrder;

	[SerializeField]
	private ScrollingSpriteInfo[] spritePrefabs;

	[SerializeField]
	private BaronessLevelCastle baroness;

	[SerializeField]
	private OneTimeScrollingSprite[] sprites;

	private List<OneTimeScrollingSprite> currentClones;

	private bool bossNotDead;

	protected override void Awake()
	{
		base.Awake();
		bossNotDead = true;
		currentClones = new List<OneTimeScrollingSprite>();
		StartCoroutine(start_phase2_cr());
	}

	private IEnumerator start_phase2_cr()
	{
		for (int i = 0; i < sprites.Length; i++)
		{
			sprites[i].speed = 0f;
		}
		foreach (OneTimeScrollingSprite currentClone in currentClones)
		{
			if (currentClone != null)
			{
				currentClone.GetComponent<OneTimeScrollingSprite>().speed = 0f;
			}
		}
		while (baroness.state != BaronessLevelCastle.State.Chase)
		{
			yield return null;
		}
		StartLoop();
		while (true)
		{
			if (!baroness.pauseScrolling)
			{
				for (int j = 0; j < sprites.Length; j++)
				{
					sprites[j].speed = speed;
				}
				foreach (OneTimeScrollingSprite currentClone2 in currentClones)
				{
					if (currentClone2 != null)
					{
						currentClone2.GetComponent<OneTimeScrollingSprite>().speed = speed;
					}
				}
			}
			else
			{
				for (int k = 0; k < sprites.Length; k++)
				{
					sprites[k].speed = 0f;
				}
				foreach (OneTimeScrollingSprite currentClone3 in currentClones)
				{
					if (currentClone3 != null)
					{
						currentClone3.GetComponent<OneTimeScrollingSprite>().speed = 0f;
					}
				}
			}
			yield return null;
		}
	}

	private void StartLoop()
	{
		StartCoroutine(loop_cr());
	}

	private IEnumerator loop_cr()
	{
		yield return CupheadTime.WaitForSeconds(this, 1f);
		float leadTime = 0f / speed;
		float totalWeight = 0f;
		ScrollingSpriteInfo[] array = spritePrefabs;
		foreach (ScrollingSpriteInfo scrollingSpriteInfo in array)
		{
			totalWeight += scrollingSpriteInfo.weight;
		}
		float spacing = UnityEngine.Random.Range(0f, minSpacing) + MathUtils.ExpRandom(averageSpacing - minSpacing);
		ScrollingSpriteInfo lastSpawned = null;
		while (true)
		{
			if (bossNotDead && !baroness.pauseScrolling)
			{
				float waitTime = spacing / speed;
				if (leadTime > waitTime)
				{
					leadTime -= waitTime;
					yield return null;
				}
				else
				{
					if (leadTime > 0f)
					{
						waitTime -= leadTime;
						leadTime = 0f;
						yield return null;
					}
					yield return CupheadTime.WaitForSeconds(this, waitTime);
				}
				float maxP = totalWeight;
				if (lastSpawned != null)
				{
					maxP -= lastSpawned.weight;
					yield return null;
				}
				float p = UnityEngine.Random.Range(0f, maxP);
				float cumulativeWeight = 0f;
				ScrollingSpriteInfo toSpawn = lastSpawned;
				ScrollingSpriteInfo[] array2 = spritePrefabs;
				foreach (ScrollingSpriteInfo scrollingSpriteInfo2 in array2)
				{
					toSpawn = scrollingSpriteInfo2;
					if (scrollingSpriteInfo2 != lastSpawned)
					{
						cumulativeWeight += scrollingSpriteInfo2.weight;
						if (cumulativeWeight >= p)
						{
							break;
						}
					}
				}
				SpriteRenderer sprite = UnityEngine.Object.Instantiate(toSpawn.sprite);
				GameObject obj = sprite.gameObject;
				float x = -1280f - leadTime * speed + sprite.bounds.size.x / 2f;
				obj.transform.position = new Vector2(x, spawnY);
				obj.transform.SetParent(base.transform, false);
				sprite.sortingOrder = sortingOrder;
				OneTimeScrollingSprite scrollingSprite = obj.AddComponent<OneTimeScrollingSprite>();
				scrollingSprite.speed = speed;
				spacing = minSpacing + MathUtils.ExpRandom(averageSpacing - minSpacing) - sprite.bounds.size.x;
				OnSpawn(obj);
				lastSpawned = toSpawn;
				currentClones.Add(scrollingSprite);
				yield return null;
			}
			yield return null;
		}
	}

	protected virtual void OnSpawn(GameObject obj)
	{
	}
}
