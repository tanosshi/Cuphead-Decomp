using UnityEngine;

public class FlyingMermaidLevelDebrisSpawner : ScrollingSpriteSpawner
{
	[SerializeField]
	private GameObject trackingWater;

	protected override void OnSpawn(GameObject obj)
	{
		base.OnSpawn(obj);
		FlyingMermaidLevelFloater component = obj.GetComponent<FlyingMermaidLevelFloater>();
		if (component != null)
		{
			component.trackingWater = trackingWater;
		}
	}
}
