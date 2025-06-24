using UnityEngine;

public class MapNPCBarbershop : AbstractMonoBehaviour
{
	[SerializeField]
	private new Animator animator;

	[SerializeField]
	private RuntimeAnimatorController fourAnimatorController;

	[SerializeField]
	private Vector3 fourPosition;

	[SerializeField]
	protected MapNPCLostBarbershop mapNPCDistanceAnimator;

	public MapDialogueInteraction mapDialogueInteraction;

	[SerializeField]
	private int dialoguerVariableID = 10;

	private bool reunited;

	protected override void Start()
	{
		base.Start();
		if (Dialoguer.GetGlobalFloat(dialoguerVariableID) > 0f)
		{
			NowFour();
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
	}

	protected override void OnDrawGizmosSelected()
	{
		base.OnDrawGizmosSelected();
		Gizmos.DrawWireSphere(fourPosition + base.transform.parent.position, 1f);
	}

	public void NowFour()
	{
		animator.runtimeAnimatorController = fourAnimatorController;
		base.transform.localPosition = fourPosition;
		if ((bool)mapDialogueInteraction)
		{
			Object.Destroy(mapDialogueInteraction);
		}
		if ((bool)mapNPCDistanceAnimator)
		{
			Object.Destroy(mapNPCDistanceAnimator);
		}
	}

	private void SongLooped()
	{
	}

	private void Show()
	{
		animator.SetTrigger("show");
	}
}
