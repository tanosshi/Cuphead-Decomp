using System;
using UnityEngine;

namespace Rewired.UI.ControlMapper
{
	[RequireComponent(typeof(CanvasScalerExt))]
	public class CanvasScalerFitter : MonoBehaviour
	{
		[Serializable]
		private class BreakPoint
		{
			[SerializeField]
			public string name;

			[SerializeField]
			public float screenAspectRatio;

			[SerializeField]
			public Vector2 referenceResolution;
		}

		[SerializeField]
		private BreakPoint[] breakPoints;

		private CanvasScalerExt canvasScaler;

		private int screenWidth;

		private int screenHeight;

		private Action ScreenSizeChanged;

		private void OnEnable()
		{
			canvasScaler = GetComponent<CanvasScalerExt>();
			Update();
			canvasScaler.ForceRefresh();
		}

		private void Update()
		{
			if (Screen.width != screenWidth || Screen.height != screenHeight)
			{
				screenWidth = Screen.width;
				screenHeight = Screen.height;
				UpdateSize();
			}
		}

		private void UpdateSize()
		{
			float num = (float)Screen.width / (float)Screen.height;
			if (num <= 1.7677778f)
			{
				canvasScaler.referenceResolution = new Vector2(1885f, 600f * (1.7777778f / num));
				canvasScaler.ForceRefresh();
			}
			else
			{
				canvasScaler.referenceResolution = new Vector2(1885f, 600f);
				canvasScaler.ForceRefresh();
			}
		}
	}
}
