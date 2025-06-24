using UnityEngine;

public class DragonLevelLightning : AbstractPausableComponent
{
	private readonly int[] layerOrder = new int[3] { 91, 93, 95 };

	[SerializeField]
	private new Animator animator;

	[SerializeField]
	private SpriteRenderer spriteRenderer;

	public void PlayLightning()
	{
		int value = Random.Range(1, 11);
		animator.SetInteger("LightningID", value);
		animator.SetTrigger("Continue");
		value = Random.Range(0, layerOrder.Length);
		spriteRenderer.sortingOrder = layerOrder[value];
		AudioManager.Play("level_dragon_amb_thunder");
	}
}
