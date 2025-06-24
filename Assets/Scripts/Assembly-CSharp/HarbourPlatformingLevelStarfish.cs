using System;
using System.Collections;
using UnityEngine;

public class HarbourPlatformingLevelStarfish : AbstractPlatformingLevelEnemy
{
	[SerializeField]
	private Effect normalBubbles;

	[SerializeField]
	private Effect pinkBubbles;

	private GameObject pivotPoint;

	private float angle;

	private float figureEightSpeed;

	private float movementSpeed;

	private float loopSize;

	private string type;

	private Vector3 pivotOffset;

	private Effect bubbles;

	public void Init(float rotation, float speedX, float speedY, float loopSize, string type)
	{
		base.transform.SetEulerAngles(null, null, rotation - 90f);
		figureEightSpeed = speedX;
		movementSpeed = speedY;
		this.loopSize = loopSize;
		this.type = type;
	}

	protected override void OnStart()
	{
	}

	protected override void Start()
	{
		base.Start();
		pivotOffset = Vector3.up * 2f * loopSize;
		pivotPoint = new GameObject("PivotPoint");
		pivotPoint.transform.position = base.transform.position;
		if (type == "A")
		{
			_canParry = true;
			bubbles = pinkBubbles;
		}
		else
		{
			bubbles = normalBubbles;
		}
		GetComponent<PlatformingLevelEnemyAnimationHandler>().SelectAnimation(type);
		StartCoroutine(spawn_bubbles_cr());
		StartCoroutine(move_cr());
		StartCoroutine(figure_eight_cr());
	}

	private IEnumerator move_cr()
	{
		while (true)
		{
			pivotPoint.transform.position += base.transform.up * movementSpeed * CupheadTime.FixedDelta;
			if (base.transform.position.y > CupheadLevelCamera.Current.Bounds.yMax + 200f)
			{
				break;
			}
			yield return null;
		}
		Die();
		yield return null;
	}

	private IEnumerator figure_eight_cr()
	{
		bool invert = false;
		while (true)
		{
			angle += figureEightSpeed * (float)CupheadTime.Delta;
			if (angle > (float)Math.PI * 2f)
			{
				invert = !invert;
				angle -= (float)Math.PI * 2f;
			}
			if (angle < 0f)
			{
				angle += (float)Math.PI * 2f;
			}
			float value;
			if (invert)
			{
				base.transform.position = pivotPoint.transform.position + pivotOffset;
				value = -1f;
			}
			else
			{
				base.transform.position = pivotPoint.transform.position;
				value = 1f;
			}
			Vector3 handleRotation = new Vector3((0f - Mathf.Sin(angle)) * loopSize, Mathf.Cos(angle) * value * loopSize, 0f);
			base.transform.position += handleRotation;
			yield return null;
		}
	}

	private IEnumerator spawn_bubbles_cr()
	{
		string bubbleTypes = "A,B,C,D";
		while (true)
		{
			string bubbleType = bubbleTypes.Split(',')[UnityEngine.Random.Range(0, bubbleTypes.Split(',').Length)];
			bubbles.Create(base.transform.position).GetComponent<PlatformingLevelEnemyAnimationHandler>().SelectAnimation(bubbleType);
			yield return CupheadTime.WaitForSeconds(this, 1f);
			yield return null;
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		UnityEngine.Object.Destroy(pivotPoint.gameObject);
	}
}
