using UnityEngine;

public class DicePalaceDominoLevelFlooarDomino : MonoBehaviour
{
	[SerializeField]
	public float speed = 300f;

	public float resetPositionX = 2808f;

	private void Update()
	{
		Vector3 position = base.transform.position;
		position.x -= speed * (float)CupheadTime.Delta;
		base.transform.position = position;
		if (base.transform.position.x <= 0f)
		{
			position.x += resetPositionX;
			base.transform.position = position;
		}
	}
}
