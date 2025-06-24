using System.Collections;
using UnityEngine;

public class FlyingMermaidLevelHomingProjectile : HomingProjectile
{
	[SerializeField]
	private SpriteRenderer mainSprite;

	private LevelProperties.FlyingMermaid.HomerFish properties;

	public FlyingMermaidLevelHomingProjectile Create(Vector3 pos, float rotation, AbstractPlayerController player, LevelProperties.FlyingMermaid.HomerFish properties)
	{
		FlyingMermaidLevelHomingProjectile flyingMermaidLevelHomingProjectile = Create(pos, rotation, properties.initSpeed, properties.bulletSpeed, properties.rotationSpeed, properties.timeBeforeDeath, properties.timeBeforeHoming, player) as FlyingMermaidLevelHomingProjectile;
		flyingMermaidLevelHomingProjectile.properties = properties;
		flyingMermaidLevelHomingProjectile.transform.position = pos;
		return flyingMermaidLevelHomingProjectile;
	}

	protected override void Start()
	{
		base.Start();
		StartCoroutine(timer_cr());
	}

	private IEnumerator timer_cr()
	{
		yield return CupheadTime.WaitForSeconds(this, 0.2f);
		mainSprite.sortingLayerName = "Foreground";
		mainSprite.sortingOrder = 30;
		yield return CupheadTime.WaitForSeconds(this, properties.timeBeforeDeath - 0.2f);
		base.HomingEnabled = false;
		animator.SetTrigger("StopTracking");
		while (true)
		{
			base.transform.position += (Vector3)velocity.normalized * properties.bulletSpeed * 1.4f * CupheadTime.Delta;
			base.transform.SetEulerAngles(0f, 0f, MathUtils.DirectionToAngle(velocity) + 180f);
			yield return null;
		}
	}
}
