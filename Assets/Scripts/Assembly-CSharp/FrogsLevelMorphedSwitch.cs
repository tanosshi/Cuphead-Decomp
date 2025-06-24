using UnityEngine;

public class FrogsLevelMorphedSwitch : ParrySwitch
{
	private Transform target;

	public new bool enabled
	{
		get
		{
			return GetComponent<Collider2D>().enabled;
		}
		set
		{
			GetComponent<Collider2D>().enabled = value;
		}
	}

	public static FrogsLevelMorphedSwitch Create(FrogsLevelMorphed parent)
	{
		GameObject gameObject = new GameObject("Frogs_Morphed_Handle");
		FrogsLevelMorphedSwitch frogsLevelMorphedSwitch = gameObject.AddComponent<FrogsLevelMorphedSwitch>();
		frogsLevelMorphedSwitch.target = parent.switchRoot;
		return frogsLevelMorphedSwitch;
	}

	protected override void Awake()
	{
		base.Awake();
		CircleCollider2D circleCollider2D = base.gameObject.AddComponent<CircleCollider2D>();
		circleCollider2D.radius = 50f;
		circleCollider2D.isTrigger = true;
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
