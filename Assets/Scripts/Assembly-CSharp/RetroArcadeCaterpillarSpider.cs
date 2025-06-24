using System.Collections;
using UnityEngine;

public class RetroArcadeCaterpillarSpider : RetroArcadeEnemy
{
	public enum Direction
	{
		Left = 0,
		Right = 1
	}

	public enum State
	{
		Entering = 0,
		ZigZagDown = 1,
		ZigZagUp = 2,
		Leaving = 3
	}

	private const float OFFSCREEN_Y = 300f;

	private const float MOVE_OFFSCREEN_SPEED = 500f;

	public const float MAX_X = 320f;

	private LevelProperties.RetroArcade.Caterpillar properties;

	private Direction direction;

	private Vector2 targetPos;

	private State state;

	private int numZigZags;

	public RetroArcadeCaterpillarSpider Create(Direction direction, LevelProperties.RetroArcade.Caterpillar properties)
	{
		RetroArcadeCaterpillarSpider retroArcadeCaterpillarSpider = InstantiatePrefab<RetroArcadeCaterpillarSpider>();
		retroArcadeCaterpillarSpider.transform.SetPosition((direction != Direction.Right) ? 320f : (-320f), 300f);
		retroArcadeCaterpillarSpider.direction = direction;
		retroArcadeCaterpillarSpider.properties = properties;
		retroArcadeCaterpillarSpider.targetPos = new Vector2(retroArcadeCaterpillarSpider.transform.position.x, properties.spiderPathY.max);
		retroArcadeCaterpillarSpider.state = State.Entering;
		retroArcadeCaterpillarSpider.hp = 1f;
		return retroArcadeCaterpillarSpider;
	}

	protected override void FixedUpdate()
	{
		base.FixedUpdate();
		if (base.IsDead)
		{
			return;
		}
		float num = properties.spiderSpeed * CupheadTime.FixedDelta;
		float magnitude = (targetPos - (Vector2)base.transform.position).magnitude;
		if (magnitude > num)
		{
			move(num);
			return;
		}
		base.transform.position = targetPos;
		switch (state)
		{
		case State.Entering:
			state = State.ZigZagDown;
			break;
		case State.ZigZagDown:
		case State.ZigZagUp:
			if (numZigZags >= properties.spiderNumZigZags)
			{
				state = State.Leaving;
				break;
			}
			state = ((state == State.ZigZagUp) ? State.ZigZagDown : State.ZigZagUp);
			numZigZags++;
			break;
		case State.Leaving:
			Object.Destroy(base.gameObject);
			return;
		}
		switch (state)
		{
		case State.ZigZagDown:
		case State.ZigZagUp:
			targetPos.x = (float)((direction == Direction.Right) ? 1 : (-1)) * Mathf.Lerp(-320f, 320f, (float)numZigZags / (float)properties.spiderNumZigZags);
			targetPos.y = ((state != State.ZigZagUp) ? properties.spiderPathY.min : properties.spiderPathY.max);
			break;
		case State.Leaving:
			targetPos.y = 300f;
			break;
		}
		move(num - magnitude);
	}

	private void move(float distance)
	{
		base.transform.position = (Vector2)base.transform.position + (targetPos - (Vector2)base.transform.position).normalized * distance;
	}

	public override void Die()
	{
		base.Die();
		StartCoroutine(moveOffscreen_cr());
	}

	private IEnumerator moveOffscreen_cr()
	{
		MoveY(300f - base.transform.position.y, 500f);
		while (movingY)
		{
			yield return null;
		}
		Object.Destroy(base.gameObject);
	}
}
