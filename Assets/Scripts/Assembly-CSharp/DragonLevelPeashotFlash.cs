using UnityEngine;

public class DragonLevelPeashotFlash : AbstractPausableComponent
{
	private new Animator animator;

	protected override void Awake()
	{
		base.Awake();
		animator = GetComponent<Animator>();
	}

	public void Flash()
	{
		base.transform.SetScale(1f, MathUtils.PlusOrMinus(), 1f);
		animator.SetInteger("i", Random.Range(0, 4));
		animator.SetTrigger("OnChange");
	}
}
