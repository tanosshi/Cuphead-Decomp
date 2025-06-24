using System.Collections;
using UnityEngine;

public class RetroArcadeAlienManager : LevelProperties.RetroArcade.Entity
{
	private const float TOP_ROW_Y = 230f;

	private const float COLUMN_SPACING = 50f;

	private const float ROW_SPACING = 40f;

	private const float TURNAROUND_X = 320f;

	private const float MIN_Y = -40f;

	private const float OFFSCREEN_MOVE_Y = 170f;

	[SerializeField]
	private RetroArcadeAlien[] alienPrefabs;

	private RetroArcadeAlien[,] aliens;

	[SerializeField]
	private RetroArcadeBonusAlien bonusAlien;

	private MinMax shotRate;

	private int numDied;

	private float currentTopRowY;

	private LevelProperties.RetroArcade.Aliens p;

	public RetroArcadeAlien.Direction direction { get; private set; }

	public float moveSpeed { get; private set; }

	public void StartAliens()
	{
		p = base.properties.CurrentState.aliens;
		aliens = new RetroArcadeAlien[p.numColumns, alienPrefabs.Length];
		direction = ((!Rand.Bool()) ? RetroArcadeAlien.Direction.Right : RetroArcadeAlien.Direction.Left);
		for (int i = 0; i < aliens.GetLength(0); i++)
		{
			for (int j = 0; j < aliens.GetLength(1); j++)
			{
				Vector2 position = new Vector2(50f * ((float)i - (float)(aliens.GetLength(0) - 1) / 2f), 230f - (float)j * 40f + 170f);
				aliens[i, j] = alienPrefabs[j].Create(position, i, this, p);
				aliens[i, j].MoveY(-170f);
			}
		}
		numDied = 0;
		moveSpeed = 640f / p.moveTime;
		shotRate = p.shotRate.Clone();
		currentTopRowY = 230f;
		StartCoroutine(turn_cr());
		StartCoroutine(shoot_cr());
		StartCoroutine(randomShot_cr());
		StartCoroutine(bonus_cr());
	}

	private IEnumerator turn_cr()
	{
		while (true)
		{
			if ((direction == RetroArcadeAlien.Direction.Right && getRightmost().transform.position.x > 320f) || (direction == RetroArcadeAlien.Direction.Left && getLeftmost().transform.position.x < -320f))
			{
				direction = ((direction == RetroArcadeAlien.Direction.Left) ? RetroArcadeAlien.Direction.Right : RetroArcadeAlien.Direction.Left);
				float num = -40f;
				if (currentTopRowY - (float)(aliens.GetLength(1) - 1) * 40f + num < -40f)
				{
					num = 230f - currentTopRowY;
				}
				for (int i = 0; i < aliens.GetLength(0); i++)
				{
					if (isColumnAlive(i))
					{
						for (int j = 0; j < aliens.GetLength(1); j++)
						{
							aliens[i, j].MoveY(num);
						}
					}
				}
				currentTopRowY += num;
			}
			yield return new WaitForFixedUpdate();
		}
	}

	private IEnumerator shoot_cr()
	{
		string[] columnPattern = p.shotColumnPattern.RandomChoice().Split(',');
		int columnPatternIndex = Random.Range(0, columnPattern.Length);
		yield return CupheadTime.WaitForSeconds(this, shotRate.RandomFloat());
		while (true)
		{
			columnPatternIndex = (columnPatternIndex + 1) % columnPattern.Length;
			int column = 0;
			int.TryParse(columnPattern[columnPatternIndex], out column);
			column--;
			if (isColumnAlive(column))
			{
				getBottommostInColumn(column).Shoot();
				yield return CupheadTime.WaitForSeconds(this, shotRate.RandomFloat());
			}
		}
	}

	private IEnumerator randomShot_cr()
	{
		yield return CupheadTime.WaitForSeconds(this, MathUtils.ExpRandom(p.randomShotAverageTime));
		while (true)
		{
			int column = Random.Range(0, aliens.GetLength(0));
			while (!isColumnAlive(column))
			{
				column = Random.Range(0, aliens.GetLength(0));
			}
			getBottommostInColumn(column).Shoot();
			yield return CupheadTime.WaitForSeconds(this, MathUtils.ExpRandom(p.randomShotAverageTime));
		}
	}

	private IEnumerator bonus_cr()
	{
		for (int i = 0; i < p.bonusAppearCount; i++)
		{
			yield return CupheadTime.WaitForSeconds(this, p.bonusAppearTime.RandomFloat());
			bonusAlien.Create((!Rand.Bool()) ? RetroArcadeBonusAlien.Direction.Right : RetroArcadeBonusAlien.Direction.Left, p);
		}
	}

	private RetroArcadeAlien getLeftmost()
	{
		for (int i = 0; i < aliens.GetLength(0); i++)
		{
			for (int j = 0; j < aliens.GetLength(1); j++)
			{
				if (!aliens[i, j].IsDead)
				{
					return aliens[i, j];
				}
			}
		}
		return null;
	}

	private RetroArcadeAlien getRightmost()
	{
		for (int num = aliens.GetLength(0) - 1; num >= 0; num--)
		{
			for (int i = 0; i < aliens.GetLength(1); i++)
			{
				if (!aliens[num, i].IsDead)
				{
					return aliens[num, i];
				}
			}
		}
		return null;
	}

	private RetroArcadeAlien getTopmost()
	{
		for (int i = 0; i < aliens.GetLength(1); i++)
		{
			for (int j = 0; j < aliens.GetLength(0); j++)
			{
				if (!aliens[j, i].IsDead)
				{
					return aliens[j, i];
				}
			}
		}
		return null;
	}

	private RetroArcadeAlien getBottommost()
	{
		for (int num = aliens.GetLength(1) - 1; num >= 0; num--)
		{
			for (int i = 0; i < aliens.GetLength(0); i++)
			{
				if (!aliens[i, num].IsDead)
				{
					return aliens[i, num];
				}
			}
		}
		return null;
	}

	private bool isColumnAlive(int x)
	{
		for (int i = 0; i < aliens.GetLength(1); i++)
		{
			if (!aliens[x, i].IsDead)
			{
				return true;
			}
		}
		return false;
	}

	private RetroArcadeAlien getBottommostInColumn(int x)
	{
		for (int num = aliens.GetLength(1) - 1; num >= 0; num--)
		{
			if (!aliens[x, num].IsDead)
			{
				return aliens[x, num];
			}
		}
		return null;
	}

	public void OnAlienDie(RetroArcadeAlien alien)
	{
		numDied++;
		moveSpeed = 640f / (p.moveTime - (float)numDied * p.moveTimeDecrease);
		shotRate.max -= p.shotRateDecrease;
		shotRate.min -= p.shotRateDecrease;
		if (!isColumnAlive(alien.ColumnIndex))
		{
			for (int i = 0; i < aliens.GetLength(1); i++)
			{
				aliens[alien.ColumnIndex, i].MoveY(170f + (230f - aliens[alien.ColumnIndex, 0].transform.position.y));
			}
		}
		if (numDied >= aliens.Length)
		{
			StopAllCoroutines();
			base.properties.DealDamageToNextNamedState();
			StartCoroutine(waveOver_cr());
		}
	}

	private IEnumerator waveOver_cr()
	{
		yield return CupheadTime.WaitForSeconds(this, 1f);
		RetroArcadeAlien[,] array = aliens;
		int length = array.GetLength(0);
		int length2 = array.GetLength(1);
		for (int i = 0; i < length; i++)
		{
			for (int j = 0; j < length2; j++)
			{
				RetroArcadeAlien retroArcadeAlien = array[i, j];
				Object.Destroy(retroArcadeAlien.gameObject);
			}
		}
	}
}
