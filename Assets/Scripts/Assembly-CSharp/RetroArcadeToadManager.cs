using System.Collections;
using UnityEngine;

public class RetroArcadeToadManager : LevelProperties.RetroArcade.Entity
{
	private const float TOAD_MAX_X_POS = 250f;

	[SerializeField]
	private RetroArcadeToad toadPrefab;

	[SerializeField]
	private RetroArcadeToadCar regularCarPrefab;

	[SerializeField]
	private RetroArcadeToadCar missileCarPrefab;

	private LevelProperties.RetroArcade.Toad p;

	private int numDied;

	public float attackDelay { get; private set; }

	public void StartToad()
	{
		p = base.properties.CurrentState.toad;
		attackDelay = p.attackDelay;
		numDied = 0;
		for (int i = 0; i < 4; i++)
		{
			float xPos = (float)i / 3f * 500f - 250f;
			toadPrefab.Create(this, p, xPos);
		}
		StartCoroutine(car_cr());
	}

	public void OnToadDie()
	{
		numDied++;
		attackDelay -= p.attackDelayDecrease;
		if (numDied >= 4)
		{
			StopAllCoroutines();
			base.properties.DealDamageToNextNamedState();
		}
	}

	private IEnumerator car_cr()
	{
		RetroArcadeToadCar.Direction direction = ((!Rand.Bool()) ? RetroArcadeToadCar.Direction.Right : RetroArcadeToadCar.Direction.Left);
		string[] pattern = p.carTypes.Split(',');
		while (true)
		{
			string[] array = pattern;
			foreach (string carType in array)
			{
				yield return CupheadTime.WaitForSeconds(this, p.carDelay);
				if (carType[0] == 'R')
				{
					regularCarPrefab.Create(p, direction);
				}
				else
				{
					missileCarPrefab.Create(p, direction);
				}
			}
		}
	}
}
