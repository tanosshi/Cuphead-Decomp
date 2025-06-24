using UnityEngine;

public class PlayerScreenEffectController : AbstractMonoBehaviour
{
	[SerializeField]
	private bool dontCenter;

	[SerializeField]
	private SpriteRenderer[] spriteRenderers;

	protected override void Update()
	{
		base.Update();
		if (!dontCenter)
		{
			UpdateToCamera();
		}
	}

	protected override void LateUpdate()
	{
		base.LateUpdate();
		if (!dontCenter)
		{
			UpdateToCamera();
		}
	}

	private void UpdateToCamera()
	{
		base.transform.position = Camera.main.transform.position;
		base.transform.localScale = Vector3.one * (Camera.main.orthographicSize / 360f);
	}

	public void SetSpriteLayer(int index, SpriteLayer layer)
	{
		spriteRenderers[index].sortingLayerName = layer.ToString();
	}

	public void SetSpriteOrder(int index, int order)
	{
		spriteRenderers[index].sortingOrder = order;
	}

	public void ResetSprites()
	{
		for (int i = 0; i < spriteRenderers.Length; i++)
		{
			spriteRenderers[i].sortingOrder = -2010 - i;
			spriteRenderers[i].sortingLayerName = "Player";
			spriteRenderers[i].sprite = null;
		}
	}
}
