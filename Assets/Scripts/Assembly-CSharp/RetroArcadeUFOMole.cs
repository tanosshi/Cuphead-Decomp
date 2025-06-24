using System.Collections;
using UnityEngine;

public class RetroArcadeUFOMole : RetroArcadeEnemy
{
	private enum Direction
	{
		Left = 0,
		Right = 1
	}

	private const float BACKGROUND_Y = -66f;

	private const float UNDERGROUND_Y = -167f;

	private const float POPUP_Y = -114f;

	private const float TURNAROUND_X = 200f;

	private const float MAX_ATTACK_X = 150f;

	private const int BACKGROUND_SORT_ORDER = 90;

	private const int ATTACK_SORT_ORDER = 200;

	private Direction direction;

	private LevelProperties.RetroArcade.UFO properties;

	public RetroArcadeUFOMole Create(LevelProperties.RetroArcade.UFO properties)
	{
		RetroArcadeUFOMole retroArcadeUFOMole = InstantiatePrefab<RetroArcadeUFOMole>();
		retroArcadeUFOMole.properties = properties;
		retroArcadeUFOMole.direction = ((!Rand.Bool()) ? Direction.Right : Direction.Left);
		retroArcadeUFOMole.hp = properties.hp;
		retroArcadeUFOMole.StartCoroutine(retroArcadeUFOMole.main_cr());
		return retroArcadeUFOMole;
	}

	private IEnumerator main_cr()
	{
		base.transform.SetPosition(Random.Range(-200f, 200f), -167f);
		direction = ((!Rand.Bool()) ? Direction.Right : Direction.Left);
		SpriteRenderer sprite = GetComponent<SpriteRenderer>();
		Collider2D col = GetComponent<Collider2D>();
		sprite.sortingOrder = 90;
		col.enabled = false;
		MoveY(-66f - base.transform.position.y, properties.moleAttackSpeed);
		while (movingY)
		{
			yield return new WaitForFixedUpdate();
		}
		bool[] leftOfPlayer = new bool[2];
		bool firstCheck = true;
		while (true)
		{
			bool shouldAttack = false;
			while (!shouldAttack)
			{
				base.transform.AddPosition((float)((direction != Direction.Left) ? 1 : (-1)) * properties.moleSpeed * CupheadTime.FixedDelta);
				if ((direction == Direction.Left && base.transform.position.x < -200f) || (direction == Direction.Right && base.transform.position.x > 200f))
				{
					direction = ((direction == Direction.Left) ? Direction.Right : Direction.Left);
				}
				for (int i = 0; i < 2; i++)
				{
					ArcadePlayerController arcadePlayerController = ((i != 0) ? PlayerManager.GetPlayer(PlayerId.PlayerTwo) : PlayerManager.GetPlayer(PlayerId.PlayerOne)) as ArcadePlayerController;
					if (!(arcadePlayerController == null))
					{
						bool flag = leftOfPlayer[i];
						leftOfPlayer[i] = arcadePlayerController.center.x < base.transform.position.x;
						if (leftOfPlayer[i] != flag && Mathf.Abs(base.transform.position.x) < 150f && arcadePlayerController.motor.Grounded && !firstCheck)
						{
							shouldAttack = true;
						}
					}
				}
				firstCheck = false;
				yield return new WaitForFixedUpdate();
			}
			yield return CupheadTime.WaitForSeconds(this, properties.moleWarningDelay);
			MoveY(-167f - base.transform.position.y, properties.moleAttackSpeed);
			while (movingY)
			{
				yield return new WaitForFixedUpdate();
			}
			sprite.sortingOrder = 200;
			col.enabled = true;
			MoveY(-114f - base.transform.position.y, properties.moleAttackSpeed);
			while (movingY)
			{
				yield return new WaitForFixedUpdate();
			}
			MoveY(-167f - base.transform.position.y, properties.moleAttackSpeed);
			while (movingY)
			{
				yield return new WaitForFixedUpdate();
			}
			sprite.sortingOrder = 90;
			col.enabled = false;
			MoveY(-66f - base.transform.position.y, properties.moleAttackSpeed);
			while (movingY)
			{
				yield return new WaitForFixedUpdate();
			}
			firstCheck = true;
		}
	}

	public void OnWaveEnd()
	{
		StopAllCoroutines();
		StartCoroutine(moveOffscreen_cr());
	}

	private IEnumerator moveOffscreen_cr()
	{
		MoveY(-167f - base.transform.position.y, properties.moleAttackSpeed);
		while (movingY)
		{
			yield return new WaitForFixedUpdate();
		}
		Object.Destroy(base.gameObject);
	}
}
