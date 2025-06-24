using UnityEngine;

public class HarbourPlatformingLevelKrillSpawner : PlatformingLevelEnemySpawner
{
	[SerializeField]
	private HarbourPlatformingLevelKrill krillPrefab;

	[SerializeField]
	private string posString = "305,640,356";

	[SerializeField]
	private string typeString;

	[SerializeField]
	private string delayString;

	private int posIndex;

	private int typeIndex;

	private int delayIndex;

	private bool parryable;

	protected override void Start()
	{
		base.Start();
		posIndex = Random.Range(0, posString.Split(',').Length);
		typeIndex = Random.Range(0, typeString.Split(',').Length);
		delayIndex = Random.Range(0, delayString.Split(',').Length);
	}

	protected override void Spawn()
	{
		base.Spawn();
		spawnDelay.min = float.Parse(delayString.Split(',')[delayIndex]);
		spawnDelay.max = float.Parse(delayString.Split(',')[delayIndex]);
		Vector2 vector = CupheadLevelCamera.Current.transform.position;
		vector.x = CupheadLevelCamera.Current.transform.position.x + (float)int.Parse(posString.Split(',')[posIndex]);
		vector.y = CupheadLevelCamera.Current.Bounds.yMin - 50f;
		parryable = typeString.Split(',')[typeIndex][0] == 'A';
		HarbourPlatformingLevelKrill harbourPlatformingLevelKrill = krillPrefab.Spawn(null, vector);
		harbourPlatformingLevelKrill.isParryable = parryable;
		harbourPlatformingLevelKrill.SetType(typeString.Split(',')[typeIndex]);
		posIndex = (posIndex + 1) % posString.Split(',').Length;
		typeIndex = (typeIndex + 1) % typeString.Split(',').Length;
		delayIndex = (delayIndex + 1) % delayString.Split(',').Length;
	}
}
