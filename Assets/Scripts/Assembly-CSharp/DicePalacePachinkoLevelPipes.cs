using System.Collections;
using UnityEngine;

public class DicePalacePachinkoLevelPipes : LevelProperties.DicePalacePachinko.Entity
{
	[SerializeField]
	private Transform[] spawnPoints;

	[SerializeField]
	private DicePalacePachinkoLevelPipeBall ballPrefab;

	private int spawnDelayIndex;

	private int spawnPointIndex;

	private int pinkBallSpawnIndex;

	private int currentBallCount;

	public override void LevelInit(LevelProperties.DicePalacePachinko properties)
	{
		base.LevelInit(properties);
		spawnPointIndex = Random.Range(0, properties.CurrentState.balls.spawnOrderString.Split(',').Length);
		spawnDelayIndex = Random.Range(0, properties.CurrentState.balls.ballDelayString.Split(',').Length);
		pinkBallSpawnIndex = Random.Range(0, properties.CurrentState.balls.pinkString.Split(',').Length);
		currentBallCount = 0;
		Level.Current.OnIntroEvent += StartAttack;
	}

	private void StartAttack()
	{
		StartCoroutine(attack_cr());
	}

	private IEnumerator attack_cr()
	{
		yield return CupheadTime.WaitForSeconds(this, base.properties.CurrentState.balls.initialAttackDelay);
		while (true)
		{
			yield return CupheadTime.WaitForSeconds(this, float.Parse(base.properties.CurrentState.balls.ballDelayString.Split(',')[spawnDelayIndex]));
			AbstractProjectile ball = ballPrefab.Create(spawnPoints[int.Parse(base.properties.CurrentState.balls.spawnOrderString.Split(',')[spawnPointIndex]) - 1].position);
			ball.GetComponent<DicePalacePachinkoLevelPipeBall>().InitBall(base.properties);
			if (currentBallCount < int.Parse(base.properties.CurrentState.balls.pinkString.Split(',')[pinkBallSpawnIndex]))
			{
				currentBallCount++;
			}
			else
			{
				ball.SetParryable(true);
				ball.GetComponentInChildren<SpriteRenderer>().color = Color.red;
				pinkBallSpawnIndex = Random.Range(0, base.properties.CurrentState.balls.pinkString.Split(',').Length);
				currentBallCount = 0;
			}
			spawnPointIndex++;
			if (spawnPointIndex >= base.properties.CurrentState.balls.spawnOrderString.Split(',').Length)
			{
				spawnPointIndex = 0;
			}
			spawnDelayIndex++;
			if (spawnDelayIndex >= base.properties.CurrentState.balls.ballDelayString.Split(',').Length)
			{
				spawnDelayIndex = 0;
			}
		}
	}

	protected override void OnDestroy()
	{
		StopAllCoroutines();
		base.OnDestroy();
	}
}
