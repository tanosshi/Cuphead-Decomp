using System.Collections;
using UnityEngine;

public class FunhousePlatformingLevelDuckCarSpawner : PlatformingLevelEnemySpawner
{
	[SerializeField]
	private Effect honkEffect;

	[SerializeField]
	private Transform topSpawnRoot;

	[SerializeField]
	private Transform bottomSpawnRoot;

	[Header("Cars")]
	[SerializeField]
	private FunhousePlatformingLevelCar carPrefabNormal;

	[SerializeField]
	private float carSpeed;

	[SerializeField]
	private float carDelay;

	[SerializeField]
	private float carSpacing;

	[SerializeField]
	private int carCount;

	[Header("Ducks")]
	[SerializeField]
	private FunhousePlatformingLevelDuck bigDuckPrefab;

	[SerializeField]
	private FunhousePlatformingLevelDuck smallDuckPrefab;

	[SerializeField]
	private FunhousePlatformingLevelDuck smallDuckPinkPrefab;

	[SerializeField]
	private float duckDelay;

	[SerializeField]
	private float duckCount;

	[SerializeField]
	private float duckSpacing;

	[SerializeField]
	private string duckPinkString;

	private int duckPinkIndex;

	private bool carsSpawning;

	private bool ducksSpawning;

	private bool ducksTop;

	private bool firstTime = true;

	protected override void Start()
	{
		base.Start();
		duckPinkIndex = Random.Range(0, duckPinkString.Split(',').Length);
	}

	protected override void StartSpawning()
	{
		base.StartSpawning();
		StartCoroutine(start_spawning_cr());
	}

	protected override void EndSpawning()
	{
		StopCoroutine(start_spawning_cr());
		base.EndSpawning();
	}

	private IEnumerator start_spawning_cr()
	{
		yield return CupheadTime.WaitForSeconds(this, initalSpawnDelay.RandomFloat());
		while (true)
		{
			if (firstTime)
			{
				ducksTop = false;
				firstTime = false;
			}
			else
			{
				ducksTop = !ducksTop;
			}
			StartCoroutine(spawn_ducks_cr());
			StartCoroutine(spawn_cars_cr());
			ducksSpawning = true;
			carsSpawning = true;
			while (ducksSpawning || carsSpawning)
			{
				yield return null;
			}
			yield return CupheadTime.WaitForSeconds(this, spawnDelay.RandomFloat());
			yield return null;
		}
	}

	private IEnumerator spawn_ducks_cr()
	{
		yield return CupheadTime.WaitForSeconds(this, duckDelay);
		float bigDuckSize = bigDuckPrefab.GetComponentInChildren<SpriteRenderer>().bounds.size.x;
		float smallDuckSize = smallDuckPrefab.GetComponentInChildren<SpriteRenderer>().bounds.size.x;
		FunhousePlatformingLevelDuck lastDuck = null;
		Vector2 startPos = Vector2.zero;
		startPos.x = CupheadLevelCamera.Current.Bounds.xMax + bigDuckSize + duckSpacing;
		startPos.y = ((!ducksTop) ? bottomSpawnRoot.position.y : topSpawnRoot.position.y);
		FunhousePlatformingLevelDuck bigDuck = bigDuckPrefab.Spawn(startPos);
		bigDuck.transform.SetScale(null, (!ducksTop) ? bigDuck.transform.localScale.y : (0f - bigDuck.transform.localScale.y));
		for (int i = 1; (float)i < duckCount; i++)
		{
			startPos.x = CupheadLevelCamera.Current.Bounds.xMax + bigDuckSize + duckSpacing + (smallDuckSize + duckSpacing) * (float)i;
			startPos.y = ((!ducksTop) ? bottomSpawnRoot.position.y : topSpawnRoot.position.y);
			if (duckPinkString.Split(',')[duckPinkIndex][0] == 'P')
			{
				FunhousePlatformingLevelDuck funhousePlatformingLevelDuck = smallDuckPrefab.Spawn(startPos);
				funhousePlatformingLevelDuck.transform.SetScale(null, (!ducksTop) ? funhousePlatformingLevelDuck.transform.localScale.y : (0f - funhousePlatformingLevelDuck.transform.localScale.y));
				funhousePlatformingLevelDuck.smallFirst = i == 1;
				if ((float)i == duckCount - 1f)
				{
					funhousePlatformingLevelDuck.smallLast = true;
					lastDuck = funhousePlatformingLevelDuck;
				}
				else
				{
					funhousePlatformingLevelDuck.smallLast = false;
				}
			}
			else if (duckPinkString.Split(',')[duckPinkIndex][0] == 'R')
			{
				FunhousePlatformingLevelDuck funhousePlatformingLevelDuck2 = smallDuckPinkPrefab.Spawn(startPos);
				funhousePlatformingLevelDuck2.transform.SetScale(null, (!ducksTop) ? funhousePlatformingLevelDuck2.transform.localScale.y : (0f - funhousePlatformingLevelDuck2.transform.localScale.y));
				funhousePlatformingLevelDuck2.smallFirst = i == 1;
				if ((float)i == duckCount - 1f)
				{
					funhousePlatformingLevelDuck2.smallLast = true;
					lastDuck = funhousePlatformingLevelDuck2;
				}
				else
				{
					funhousePlatformingLevelDuck2.smallLast = false;
				}
			}
			duckPinkIndex = (duckPinkIndex + 1) % duckPinkString.Split(',').Length;
		}
		while (lastDuck != null && !(lastDuck.transform.position.x < CupheadLevelCamera.Current.transform.position.x))
		{
			yield return null;
		}
		ducksSpawning = false;
		yield return null;
	}

	private IEnumerator spawn_cars_cr()
	{
		SpawnHonk((!ducksTop) ? topSpawnRoot.position.y : bottomSpawnRoot.position.y, ducksTop ? 1 : (-1), (!ducksTop) ? (-100f) : 100f);
		yield return CupheadTime.WaitForSeconds(this, carDelay);
		float carSize = carPrefabNormal.GetComponentInChildren<SpriteRenderer>().bounds.size.x;
		int index = 0;
		FunhousePlatformingLevelCar lastCar = null;
		Vector2 startPos = Vector2.zero;
		for (int i = 0; i < carCount; i++)
		{
			startPos.x = CupheadLevelCamera.Current.Bounds.xMax + (carSize + carSpacing * (float)i);
			startPos.y = ((!ducksTop) ? topSpawnRoot.position.y : bottomSpawnRoot.position.y);
			FunhousePlatformingLevelCar funhousePlatformingLevelCar = Object.Instantiate(carPrefabNormal);
			funhousePlatformingLevelCar.Init(startPos, 180f, carSpeed, index, i == 0, (i == carCount - 1) ? true : false);
			funhousePlatformingLevelCar.transform.SetScale(null, (!ducksTop) ? funhousePlatformingLevelCar.transform.localScale.y : (0f - funhousePlatformingLevelCar.transform.localScale.y));
			if (i == carCount - 1)
			{
				lastCar = funhousePlatformingLevelCar;
			}
			index = (index + 1) % 4;
		}
		while (lastCar.transform.position.x > CupheadLevelCamera.Current.transform.position.x)
		{
			yield return null;
		}
		carsSpawning = false;
		yield return null;
	}

	private void SpawnHonk(float rootY, float yScale, float offset)
	{
		AudioManager.Play("funhouse_car_honk_sweet");
		Vector2 vector = new Vector2(CupheadLevelCamera.Current.Bounds.xMax, rootY + offset);
		Effect effect = honkEffect.Create(vector);
		effect.transform.parent = CupheadLevelCamera.Current.transform;
		effect.transform.SetScale(null, yScale);
	}

	protected override void OnDrawGizmos()
	{
		Gizmos.color = new Color(1f, 1f, 0f, 1f);
		Gizmos.DrawWireSphere(topSpawnRoot.transform.position, 50f);
		Gizmos.color = new Color(1f, 0f, 1f, 1f);
		Gizmos.DrawWireSphere(bottomSpawnRoot.transform.position, 50f);
	}
}
