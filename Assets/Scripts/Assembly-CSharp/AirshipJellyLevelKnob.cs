using UnityEngine;

public class AirshipJellyLevelKnob : ParrySwitch
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

	public static AirshipJellyLevelKnob Create(AirshipJellyLevelJelly jelly)
	{
		GameObject gameObject = new GameObject("Airship_Jelly_Knob");
		AirshipJellyLevelKnob airshipJellyLevelKnob = gameObject.AddComponent<AirshipJellyLevelKnob>();
		airshipJellyLevelKnob.target = jelly.knobRoot;
		airshipJellyLevelKnob.tag = Tags.ParrySwitch.ToString();
		return airshipJellyLevelKnob;
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
