using UnityEngine;

public class TrainLevelEngineBossTail : ParrySwitch
{
	private CircleCollider2D collider;

	private Transform target;

	public bool tailEnabled
	{
		get
		{
			return !(collider == null) && collider.enabled;
		}
		set
		{
			if (collider != null)
			{
				collider.enabled = value;
			}
		}
	}

	public static TrainLevelEngineBossTail Create(Transform target)
	{
		GameObject gameObject = new GameObject("Engine_Boss_Tail");
		TrainLevelEngineBossTail trainLevelEngineBossTail = gameObject.AddComponent<TrainLevelEngineBossTail>();
		trainLevelEngineBossTail.target = target;
		trainLevelEngineBossTail.tag = Tags.ParrySwitch.ToString();
		return trainLevelEngineBossTail;
	}

	protected override void Awake()
	{
		base.Awake();
		collider = base.gameObject.AddComponent<CircleCollider2D>();
		collider.radius = 40f;
		collider.isTrigger = true;
	}

	protected override void Update()
	{
		base.Update();
		UpdateLocation();
	}

	protected override void LateUpdate()
	{
		base.LateUpdate();
		UpdateLocation();
	}

	private void UpdateLocation()
	{
		if (target != null)
		{
			base.transform.position = target.position;
		}
	}
}
