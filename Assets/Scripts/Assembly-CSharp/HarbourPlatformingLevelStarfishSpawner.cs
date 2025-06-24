using System.Collections;
using UnityEngine;

public class HarbourPlatformingLevelStarfishSpawner : AbstractPausableComponent
{
	[SerializeField]
	private MinMax initialSpawnTime;

	[SerializeField]
	private MinMax spawnTime;

	[SerializeField]
	private HarbourPlatformingLevelStarfish starfishPrefab;

	[SerializeField]
	private string speedXString;

	[SerializeField]
	private string typeString;

	[SerializeField]
	private MinMax speedYRange;

	[SerializeField]
	private float xRange;

	[SerializeField]
	private float loopSize;

	private int typeIndex;

	private int speedXIndex;

	private string[] speedX;

	protected override void Start()
	{
		base.Start();
		speedX = speedXString.Split(',');
		speedXIndex = Random.Range(0, speedX.Length);
		typeIndex = Random.Range(0, typeString.Split(',').Length);
		StartCoroutine(spawn_cr());
	}

	private IEnumerator spawn_cr()
	{
		bool hashadSuccessfulSpawn = false;
		while (true)
		{
			if (hashadSuccessfulSpawn)
			{
				yield return CupheadTime.WaitForSeconds(this, spawnTime.RandomFloat());
			}
			else
			{
				yield return CupheadTime.WaitForSeconds(this, initialSpawnTime.RandomFloat());
			}
			Vector2 spawnPos = base.transform.position;
			spawnPos.y = base.transform.position.y;
			spawnPos.x += Random.Range(0f - xRange, xRange);
			AbstractPlayerController player = PlayerManager.GetNext();
			if (CupheadLevelCamera.Current.ContainsPoint(spawnPos, new Vector2(0f, 1000f)))
			{
				Vector3 vector = player.transform.position - (Vector3)spawnPos;
				MathUtils.DirectionToAngle(vector);
				starfishPrefab.Spawn(spawnPos).Init(90f, float.Parse(speedX[speedXIndex]), speedYRange.RandomFloat(), loopSize, typeString.Split(',')[typeIndex]);
				hashadSuccessfulSpawn = true;
				speedXIndex = (speedXIndex + 1) % speedX.Length;
				typeIndex = (typeIndex + 1) % typeString.Split(',').Length;
			}
			yield return null;
		}
	}

	protected override void OnDrawGizmos()
	{
		Gizmos.color = new Color(1f, 1f, 0f, 1f);
		Gizmos.DrawLine(base.baseTransform.position - new Vector3(xRange, 0f, 0f), base.baseTransform.position + new Vector3(xRange, 0f, 0f));
	}
}
