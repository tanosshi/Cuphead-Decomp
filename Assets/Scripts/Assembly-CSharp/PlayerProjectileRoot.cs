using UnityEngine;

public class PlayerProjectileRoot : AbstractMonoBehaviour
{
	public Vector2 Position
	{
		get
		{
			return base.transform.position;
		}
	}

	public float Rotation
	{
		get
		{
			return base.transform.eulerAngles.z;
		}
	}

	public Vector3 Scale
	{
		get
		{
			float y = 1f;
			return new Vector3(1f, y, 1f);
		}
	}
}
