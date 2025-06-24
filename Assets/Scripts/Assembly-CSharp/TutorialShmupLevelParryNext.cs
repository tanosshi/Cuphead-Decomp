using UnityEngine;
using UnityEngine.UI;

public class TutorialShmupLevelParryNext : AbstractCollidableObject
{
	[SerializeField]
	private TutorialShmupLevelParryNext nextSphere;

	[SerializeField]
	private Image image;

	[SerializeField]
	private bool startAsParry;

	[SerializeField]
	private ParrySwitch parrySwitch;

	private AbstractPlayerController lastPlayerController;

	protected override void Start()
	{
		base.Start();
		parrySwitch.OnActivate += SetNextParry;
		if (startAsParry)
		{
			image.enabled = true;
			parrySwitch.enabled = true;
		}
		else
		{
			image.enabled = false;
			parrySwitch.enabled = false;
		}
	}

	private void SetNextParry()
	{
		nextSphere.parrySwitch.enabled = true;
		parrySwitch.enabled = false;
		if (lastPlayerController != null)
		{
			lastPlayerController.stats.OnParry();
			lastPlayerController = null;
		}
		image.enabled = false;
		nextSphere.image.enabled = true;
	}

	protected override void OnCollisionOther(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionOther(hit, phase);
		if ((bool)hit.transform && (bool)hit.transform.parent)
		{
			lastPlayerController = hit.transform.parent.GetComponent<AbstractPlayerController>();
		}
	}
}
