using UnityEngine;

public class AirshipLevelKnob : ParrySwitch
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

	public static AirshipLevelKnob Create(Transform root)
	{
		GameObject gameObject = new GameObject("Airship_Knob");
		AirshipLevelKnob airshipLevelKnob = gameObject.AddComponent<AirshipLevelKnob>();
		airshipLevelKnob.target = root;
		airshipLevelKnob.tag = Tags.ParrySwitch.ToString();
		return airshipLevelKnob;
	}

	protected override void Awake()
	{
		base.Awake();
		CircleCollider2D circleCollider2D = base.gameObject.AddComponent<CircleCollider2D>();
		circleCollider2D.radius = 20f;
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
