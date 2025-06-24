using System;
using System.Collections;
using UnityEngine;

public class DicePalaceMainLevelDice : ParrySwitch
{
	public enum Roll
	{
		One = 0,
		Two = 1,
		Three = 2
	}

	public Roll roll;

	public bool waitingToRoll;

	private LevelProperties.DicePalaceMain.Dice properties;

	private Transform pivotPoint;

	private bool reverse;

	public void Init(Vector2 pos, LevelProperties.DicePalaceMain.Dice properties, Transform pivotPoint)
	{
		base.transform.position = pos;
		this.pivotPoint = pivotPoint;
		this.properties = properties;
		GetComponent<Collider2D>().enabled = false;
		waitingToRoll = true;
		StartCoroutine(move_cr());
	}

	public void StartRoll()
	{
		base.animator.SetTrigger("StartRoll");
		waitingToRoll = true;
		base.animator.SetBool("Reverse", Rand.Bool());
		GetComponent<Collider2D>().enabled = true;
	}

	public void RollOne()
	{
		roll = Roll.One;
		PostRoll();
	}

	public void RollTwo()
	{
		roll = Roll.Two;
		PostRoll();
	}

	public void RollThree()
	{
		roll = Roll.Three;
		PostRoll();
	}

	private void PostRoll()
	{
		waitingToRoll = false;
		DicePalaceMainLevelGameInfo.TURN_COUNTER++;
	}

	private IEnumerator move_cr()
	{
		float loopSize = 20f;
		float speed = properties.movementSpeed;
		float angle = 0f;
		while (true)
		{
			Vector3 pivotOffset = Vector3.left * 2f * loopSize;
			angle += speed * (float)CupheadTime.Delta;
			if (angle > (float)Math.PI * 2f)
			{
				reverse = !reverse;
				angle -= (float)Math.PI * 2f;
			}
			if (angle < 0f)
			{
				angle += (float)Math.PI * 2f;
			}
			float value;
			if (reverse)
			{
				base.transform.position = pivotPoint.position + pivotOffset;
				value = 1f;
			}
			else
			{
				base.transform.position = pivotPoint.position;
				value = -1f;
			}
			Vector3 handleRotationX = new Vector3(Mathf.Cos(angle) * value * loopSize, 0f, 0f);
			Vector3 handleRotationY = new Vector3(0f, Mathf.Sin(angle) * loopSize, 0f);
			base.transform.position += handleRotationX + handleRotationY;
			yield return null;
		}
	}

	public override void OnParryPostPause(AbstractPlayerController player)
	{
		base.OnParryPostPause(player);
		base.animator.SetTrigger("Hit");
		GetComponent<Collider2D>().enabled = false;
	}
}
