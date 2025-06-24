using UnityEngine;

public class SpriteOrderChanger : AbstractMonoBehaviour
{
	public int change = 1;

	public int frameDelay = 2;

	private SpriteRenderer renderer;

	private int t;

	protected override void Awake()
	{
		base.Awake();
		renderer = GetComponent<SpriteRenderer>();
	}

	protected override void Update()
	{
		base.Update();
		if (t >= frameDelay)
		{
			t = 0;
			renderer.sortingOrder += change;
		}
		t++;
	}
}
