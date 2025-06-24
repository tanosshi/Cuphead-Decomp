using UnityEngine;

public class RetroArcadeCaterpillarManager : LevelProperties.RetroArcade.Entity
{
	[SerializeField]
	private RetroArcadeCaterpillarBodyPart[] bodyPartPrefabs;

	[SerializeField]
	private RetroArcadeCaterpillarSpider spiderPrefab;

	private RetroArcadeCaterpillarBodyPart[] bodyParts;

	private LevelProperties.RetroArcade.Caterpillar p;

	private int numDied;

	private int numSpidersSpawned;

	public float moveSpeed { get; private set; }

	public void StartCaterpillar()
	{
		p = base.properties.CurrentState.caterpillar;
		bodyParts = new RetroArcadeCaterpillarBodyPart[p.bodyParts.Length + 1];
		RetroArcadeCaterpillarBodyPart.Direction direction = ((!Rand.Bool()) ? RetroArcadeCaterpillarBodyPart.Direction.Right : RetroArcadeCaterpillarBodyPart.Direction.Left);
		bodyParts[0] = bodyPartPrefabs[0].Create(0, direction, this, p);
		for (int i = 0; i < p.bodyParts.Length; i++)
		{
			bodyParts[i + 1] = bodyPartPrefabs[p.bodyParts[i]].Create(i + 1, direction, this, p);
		}
		numDied = 0;
		numSpidersSpawned = 0;
		moveSpeed = 640f / p.moveTime;
	}

	public void OnBodyPartDie(RetroArcadeCaterpillarBodyPart alien)
	{
		numDied++;
		moveSpeed = 640f / (p.moveTime - (float)numDied * p.moveTimeDecrease);
		if (numDied >= bodyParts.Length - 1)
		{
			StopAllCoroutines();
			bodyParts[0].Die();
			RetroArcadeCaterpillarBodyPart[] array = bodyParts;
			foreach (RetroArcadeCaterpillarBodyPart retroArcadeCaterpillarBodyPart in array)
			{
				retroArcadeCaterpillarBodyPart.OnWaveEnd();
			}
			base.properties.DealDamageToNextNamedState();
		}
	}

	public void OnReachBottom()
	{
		if (numSpidersSpawned < p.spiderCount)
		{
			numSpidersSpawned++;
			spiderPrefab.Create((!Rand.Bool()) ? RetroArcadeCaterpillarSpider.Direction.Right : RetroArcadeCaterpillarSpider.Direction.Left, p);
		}
	}
}
