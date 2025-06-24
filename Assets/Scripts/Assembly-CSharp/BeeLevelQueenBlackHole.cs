using UnityEngine;

public class BeeLevelQueenBlackHole : AbstractProjectile
{
	[HideInInspector]
	public float health;

	[HideInInspector]
	public float speed;

	[HideInInspector]
	public float childDelay;

	[HideInInspector]
	public float childSpeed;

	[SerializeField]
	private BasicProjectile childPrefab;

	private int direction;

	private float timer;

	protected override void Start()
	{
		base.Start();
		direction = ((base.transform.position.x < 0f) ? 1 : (-1));
	}

	protected override void Update()
	{
		base.Update();
		base.transform.AddPosition(speed * (float)direction * (float)CupheadTime.Delta);
		timer += CupheadTime.Delta;
		if (timer >= childDelay)
		{
			childPrefab.Create(base.transform.position, 90f, childSpeed);
			childPrefab.Create(base.transform.position, -90f, childSpeed).GetComponent<Animator>().Play("Reverse");
			timer = 0f;
		}
	}
}
