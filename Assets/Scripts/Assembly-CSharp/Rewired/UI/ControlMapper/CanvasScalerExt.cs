using UnityEngine;
using UnityEngine.UI;

namespace Rewired.UI.ControlMapper
{
	[AddComponentMenu("")]
	public class CanvasScalerExt : CanvasScaler
	{
		public void ForceRefresh()
		{
			Handle();
		}
	}
}
