using UnityEngine;

public class TutorialLevelParryNext : AbstractCollidableObject
{
	[SerializeField]
	private TutorialLevelParryNext nextSphere;

	[SerializeField]
	private SpriteRenderer spriteRenderer;

	[SerializeField]
	private Sprite normalSprite;

	[SerializeField]
	private Sprite parrySprite;

	[SerializeField]
	private Material normalMaterial;

	[SerializeField]
	private Material parryMaterial;

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
			spriteRenderer.sprite = parrySprite;
			spriteRenderer.sharedMaterial = parryMaterial;
			parrySwitch.enabled = true;
		}
		else
		{
			spriteRenderer.sprite = normalSprite;
			spriteRenderer.sharedMaterial = normalMaterial;
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
		spriteRenderer.sprite = normalSprite;
		spriteRenderer.sharedMaterial = normalMaterial;
		nextSphere.spriteRenderer.sprite = nextSphere.parrySprite;
		nextSphere.spriteRenderer.sharedMaterial = parryMaterial;
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
