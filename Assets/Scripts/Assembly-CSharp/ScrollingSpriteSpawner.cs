using System;
using System.Collections;
using UnityEngine;

public class ScrollingSpriteSpawner : AbstractPausableComponent
{
	[Serializable]
	public class ScrollingSpriteInfo
	{
		public SpriteRenderer sprite;

		public float weight = 1f;
	}

	public const float X_IN = 1280f;

	public const float X_OUT = -1280f;

	[SerializeField]
	private bool customStart;

	[SerializeField]
	private float spawnY;

	[SerializeField]
	[Range(0f, 2000f)]
	private float speed = 100f;

	[SerializeField]
	private float minSpacing = 50f;

	[SerializeField]
	private float averageSpacing = 100f;

	[SerializeField]
	private int sortingOrder;

	[SerializeField]
	private ScrollingSpriteInfo[] spritePrefabs;

	private bool pauseScrolling;

	protected override void Awake()
	{
		base.Awake();
		if (!customStart)
		{
			StartCoroutine(loop_cr());
		}
	}

	public void StartLoop()
	{
		StartCoroutine(loop_cr());
	}

	private IEnumerator loop_cr()
	{
		float leadTime = 2560f / speed;
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
			if (pauseScrolling)
			{
				yield return null;
				continue;
			}
			float waitTime = spacing / speed;
			if (leadTime > waitTime)
			{
				leadTime -= waitTime;
			}
			else
			{
				if (leadTime > 0f)
				{
					waitTime -= leadTime;
					leadTime = 0f;
				}
				yield return CupheadTime.WaitForSeconds(this, waitTime);
			}
			float maxP = totalWeight;
			if (lastSpawned != null)
			{
				maxP -= lastSpawned.weight;
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
			float x = 1280f - leadTime * speed + sprite.bounds.size.x / 2f;
			obj.transform.position = new Vector2(x, spawnY);
			obj.transform.SetParent(base.transform, false);
			sprite.sortingOrder = sortingOrder;
			OneTimeScrollingSprite scrollingSprite = obj.AddComponent<OneTimeScrollingSprite>();
			scrollingSprite.speed = speed;
			spacing = minSpacing + MathUtils.ExpRandom(averageSpacing - minSpacing) + sprite.bounds.size.x;
			OnSpawn(obj);
			lastSpawned = toSpawn;
		}
	}

	public void HandlePausing(bool pause)
	{
		pauseScrolling = pause;
	}

	protected virtual void OnSpawn(GameObject obj)
	{
	}
}
