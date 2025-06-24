using UnityEngine;

public class PlaneLevelEffect : Effect
{
	public const float SPEED = 300f;

	[Range(0f, 2f)]
	public float speed = 1f;

	protected override void Update()
	{
		base.Update();
		base.transform.AddPosition(-300f * (float)CupheadTime.Delta * speed);
	}
}
