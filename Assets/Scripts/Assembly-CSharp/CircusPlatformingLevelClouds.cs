using System;
using System.Collections;
using UnityEngine;

public class CircusPlatformingLevelClouds : AbstractPausableComponent
{
	[Serializable]
	public class CloudPiece
	{
		public Transform cloud;

		public float cloudEndY;

		public float cameraRelativePosX;

		public float speedMultiplyAmount;

		private float currentRelativePosX;

		public void UpdateCurrentRelativePos(float pos)
		{
			currentRelativePosX = pos;
		}

		public float CurrentRelativePosX()
		{
			return currentRelativePosX;
		}
	}

	[SerializeField]
	private CloudPiece[] cloudPieces;

	private Vector3 lastPosition;

	[SerializeField]
	private float incrementAmount = 2f;

	protected override void Start()
	{
		base.Start();
		StartCoroutine(change_y_axis());
	}

	private IEnumerator change_y_axis()
	{
		float[] cloudStartPositionsX = new float[cloudPieces.Length];
		float[] cloudStartSpeedX = new float[cloudPieces.Length];
		for (int i = 0; i < cloudPieces.Length; i++)
		{
			cloudStartPositionsX[i] = cloudPieces[i].cloud.position.y;
			ScrollingSprite[] componentsInChildren = cloudPieces[i].cloud.GetComponentsInChildren<ScrollingSprite>();
			foreach (ScrollingSprite scrollingSprite in componentsInChildren)
			{
				cloudStartSpeedX[i] = scrollingSprite.speed;
			}
		}
		while (true)
		{
			for (int k = 0; k < cloudPieces.Length; k++)
			{
				cloudPieces[k].cloud.SetPosition(null, Mathf.Lerp(cloudStartPositionsX[k], cloudPieces[k].cloudEndY, RelativePosition(cloudPieces[k].cameraRelativePosX)));
				if (CupheadLevelCamera.Current.transform.position != lastPosition)
				{
					if ((bool)cloudPieces[k].cloud.GetComponent<PlatformingLevelParallax>())
					{
						cloudPieces[k].cloud.GetComponent<PlatformingLevelParallax>().enabled = false;
					}
					ScrollingSprite[] componentsInChildren2 = cloudPieces[k].cloud.GetComponentsInChildren<ScrollingSprite>();
					foreach (ScrollingSprite scrollingSprite2 in componentsInChildren2)
					{
						if (CupheadLevelCamera.Current.transform.position.x < lastPosition.x)
						{
							if (scrollingSprite2.speed > cloudStartSpeedX[k])
							{
								scrollingSprite2.speed -= incrementAmount;
							}
						}
						else if (scrollingSprite2.speed < cloudStartSpeedX[k] * cloudPieces[k].speedMultiplyAmount)
						{
							scrollingSprite2.speed += incrementAmount;
						}
					}
					cloudPieces[k].UpdateCurrentRelativePos(RelativePosition(cloudPieces[k].cameraRelativePosX));
					lastPosition = CupheadLevelCamera.Current.transform.position;
					yield return null;
					continue;
				}
				if ((bool)cloudPieces[k].cloud.GetComponent<PlatformingLevelParallax>())
				{
					cloudPieces[k].cloud.GetComponent<PlatformingLevelParallax>().enabled = true;
					cloudPieces[k].cloud.GetComponent<PlatformingLevelParallax>().UpdateBasePosition();
				}
				ScrollingSprite[] componentsInChildren3 = cloudPieces[k].cloud.GetComponentsInChildren<ScrollingSprite>();
				foreach (ScrollingSprite scrollingSprite3 in componentsInChildren3)
				{
					if (scrollingSprite3.speed > cloudStartSpeedX[k])
					{
						scrollingSprite3.speed -= incrementAmount;
					}
					else
					{
						scrollingSprite3.speed = cloudStartSpeedX[k];
					}
				}
				yield return null;
			}
			yield return null;
		}
	}

	private float RelativePosition(float relativePosX)
	{
		float num = relativePosX - (float)Level.Current.Left;
		float num2 = CupheadLevelCamera.Current.transform.position.x - (float)Level.Current.Left;
		return num2 / num;
	}
}
