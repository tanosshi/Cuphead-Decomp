using System.Collections;
using UnityEngine;

public class DicePalaceCigarLevelBackground : AbstractPausableComponent
{
	[SerializeField]
	private Transform foregroundFireSprite;

	[SerializeField]
	private Transform firePivot;

	private bool isA = true;

	protected override void Start()
	{
		base.Start();
	}

	private IEnumerator circulate_fire_cr()
	{
		float loopSize = 6f;
		float angle = 0f;
		while (true)
		{
			angle += 0.5f * (float)CupheadTime.Delta;
			Vector3 handleRotationX = new Vector3((0f - Mathf.Sin(angle)) * loopSize, 0f, 0f);
			Vector3 handleRotationY = new Vector3(0f, Mathf.Cos(angle) * loopSize, 0f);
			foregroundFireSprite.position = firePivot.position;
			foregroundFireSprite.position += handleRotationX + handleRotationY;
			yield return null;
		}
	}
}
