using UnityEngine;

public class ScrollingGameObject : AbstractMonoBehaviour
{
	public enum Axis
	{
		X = 0,
		Y = 1
	}

	public Axis axis;

	[SerializeField]
	private bool negativeDirection;

	[Range(0f, 500f)]
	[SerializeField]
	private float speed;

	[SerializeField]
	private int size = 1280;

	[SerializeField]
	private bool resetTransforms = true;

	protected override void Awake()
	{
		base.Awake();
		GameObject gameObject = new GameObject("Container");
		gameObject.transform.SetParent(base.transform);
		if (resetTransforms)
		{
			base.transform.ResetLocalTransforms();
		}
		foreach (Transform item in base.transform)
		{
			item.SetParent(gameObject.transform);
		}
		GameObject gameObject2 = Object.Instantiate(gameObject.gameObject);
		gameObject2.transform.SetParent(base.transform);
		gameObject2.transform.ResetLocalTransforms();
		gameObject2.transform.SetLocalPosition(size, 0f, 0f);
		GameObject gameObject3 = Object.Instantiate(gameObject2);
		gameObject3.transform.SetParent(base.transform);
		gameObject3.transform.SetLocalPosition(-size, 0f, 0f);
	}

	protected override void Update()
	{
		base.Update();
		Vector3 localPosition = base.transform.localPosition;
		if (localPosition.x <= (float)(-size))
		{
			localPosition.x += size;
		}
		if (localPosition.x >= 1280f)
		{
			localPosition.x -= size;
		}
		localPosition.x -= (float)((!negativeDirection) ? 1 : (-1)) * speed * (float)CupheadTime.Delta;
		base.transform.localPosition = localPosition;
	}
}
