using System.Collections;
using UnityEngine;

public class FlyingMermaidLevelFishSpreadProjectile : BasicProjectile
{
	[SerializeField]
	private Effect smokeEffectPrefab;

	[SerializeField]
	private Transform smokeEffectRoot;

	protected override void Awake()
	{
		base.Awake();
		StartCoroutine(smoke_cr());
		StartCoroutine(layer_change_cr());
	}

	private IEnumerator smoke_cr()
	{
		yield return CupheadTime.WaitForSeconds(this, Random.Range(0.25f, 0.5f));
		SpriteRenderer sprite = GetComponentInChildren<SpriteRenderer>();
		while (!base.dead)
		{
			Effect smoke = smokeEffectPrefab.Create(smokeEffectRoot.position + (Vector3)MathUtils.RandomPointInUnitCircle() * 15f);
			SpriteRenderer smokeSprite = smoke.GetComponentInChildren<SpriteRenderer>();
			smokeSprite.sortingLayerID = sprite.sortingLayerID;
			smokeSprite.sortingOrder = sprite.sortingOrder - 1;
			yield return CupheadTime.WaitForSeconds(this, Random.Range(0.1f, 0.2f));
		}
	}

	private IEnumerator layer_change_cr()
	{
		yield return CupheadTime.WaitForSeconds(this, 0.2f);
		SpriteRenderer sprite = GetComponentInChildren<SpriteRenderer>();
		sprite.sortingLayerName = "Foreground";
		sprite.sortingOrder = 30;
	}
}
