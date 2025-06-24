using System;
using System.Collections;
using UnityEngine;

public class GroundHomingMovement : AbstractPausableComponent
{
	public enum Direction
	{
		Left = 0,
		Right = 1
	}

	public bool startOnAwake;

	public float maxSpeed;

	public float acceleration;

	public float bounceRatio;

	public bool bounceEnabled;

	public float leftPadding;

	public float rightPadding;

	public bool destroyOffScreen;

	public bool enableRadishRot;

	private float velocityX;

	public AbstractPlayerController TrackingPlayer { get; set; }

	public bool EnableHoming { get; set; }

	public Direction MoveDirection { get; set; }

	protected override void Awake()
	{
		base.Awake();
		StartCoroutine(loop_cr());
		if (startOnAwake)
		{
			EnableHoming = true;
		}
	}

	private float hitPauseCoefficient()
	{
		DamageReceiver component = GetComponent<DamageReceiver>();
		if (component == null)
		{
			return 1f;
		}
		return (!component.IsHitPaused) ? 1f : 0f;
	}

	private IEnumerator loop_cr()
	{
		Quaternion radishRot = base.transform.localRotation;
		while (TrackingPlayer == null)
		{
			yield return null;
		}
		while (true)
		{
			if (!base.enabled)
			{
				yield return null;
				continue;
			}
			if (TrackingPlayer == null || TrackingPlayer.IsDead)
			{
				TrackingPlayer = PlayerManager.GetNext();
			}
			if (EnableHoming)
			{
				if (TrackingPlayer.transform.position.x > base.transform.position.x)
				{
					MoveDirection = Direction.Right;
					if (radishRot.z < (float)Math.PI / 60f)
					{
						radishRot.z += 0.01f;
					}
				}
				else
				{
					MoveDirection = Direction.Left;
					if (radishRot.z > -(float)Math.PI / 60f)
					{
						radishRot.z -= 0.01f;
					}
				}
			}
			if (MoveDirection == Direction.Right)
			{
				velocityX += acceleration * (float)CupheadTime.Delta * hitPauseCoefficient();
			}
			else
			{
				velocityX -= acceleration * (float)CupheadTime.Delta * hitPauseCoefficient();
			}
			velocityX = Mathf.Clamp(velocityX, 0f - maxSpeed, maxSpeed);
			Vector2 position = base.transform.localPosition;
			position.x += velocityX * (float)CupheadTime.Delta * hitPauseCoefficient();
			if (bounceEnabled)
			{
				if (position.x < (float)Level.Current.Left + leftPadding)
				{
					position.x = (float)Level.Current.Left + leftPadding;
					velocityX *= 0f - bounceRatio;
				}
				if (position.x > (float)Level.Current.Right - rightPadding)
				{
					position.x = (float)Level.Current.Right - rightPadding;
					velocityX *= 0f - bounceRatio;
				}
			}
			if (destroyOffScreen)
			{
				SpriteRenderer component = GetComponent<SpriteRenderer>();
				if (position.x < (float)Level.Current.Left - component.bounds.size.x / 2f || position.x > (float)Level.Current.Right + component.bounds.size.x / 2f)
				{
					UnityEngine.Object.Destroy(base.gameObject);
				}
			}
			base.transform.localPosition = position;
			if (enableRadishRot)
			{
				base.transform.localRotation = radishRot;
			}
			yield return null;
		}
	}
}
