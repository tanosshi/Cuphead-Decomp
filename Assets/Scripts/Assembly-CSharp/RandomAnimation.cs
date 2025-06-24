using UnityEngine;

public class RandomAnimation : AbstractPausableComponent
{
	[SerializeField]
	[Range(0f, 1f)]
	private float randomSpeed = 0.1f;

	protected override void Awake()
	{
		base.Awake();
		base.animator.SetInteger("Animation", Random.Range(0, base.animator.GetInteger("Count")));
		base.animator.speed += Random.Range(0f - randomSpeed, randomSpeed);
	}
}
