using System.Collections;
using UnityEngine;

public class MountainPlatformingLevelSatyr : PlatformingLevelGroundMovementEnemy
{
	protected override void Start()
	{
		GetComponent<Collider2D>().enabled = false;
		base.Start();
		StartCoroutine(satyr_land_cr());
	}

	public void Init(Direction direction, bool isForeground)
	{
		_direction = direction;
		GetComponent<SpriteRenderer>().sortingLayerName = SpriteLayer.Background.ToString();
	}

	private IEnumerator satyr_land_cr()
	{
		AudioManager.Play("castle_imp_spawn");
		emitAudioFromObject.Add("castle_imp_spawn");
		floating = false;
		Jump();
		StartCoroutine(change_layer_cr());
		while (!base.Grounded)
		{
			yield return null;
		}
		landing = true;
		AudioManager.Play("castle_imp_land");
		emitAudioFromObject.Add("castle_imp_land");
		base.animator.SetTrigger("Continue");
		yield return base.animator.WaitForAnimationToEnd(this, "Intro_Continue");
		landing = false;
		yield return null;
	}

	private IEnumerator change_layer_cr()
	{
		yield return CupheadTime.WaitForSeconds(this, 0.5f);
		GetComponent<SpriteRenderer>().sortingLayerName = SpriteLayer.Enemies.ToString();
		GetComponent<SpriteRenderer>().sortingOrder = 20;
		GetComponent<Collider2D>().enabled = true;
		yield return null;
	}

	protected override void OnCollision(GameObject hit, CollisionPhase phase)
	{
		base.OnCollision(hit, phase);
		if (phase == CollisionPhase.Enter && (bool)hit.GetComponent<MountainPlatformingLevelWall>())
		{
			Turn();
		}
	}

	protected override void Die()
	{
		AudioManager.Play("castle_generic_death_honk");
		emitAudioFromObject.Add("castle_generic_death_honk");
		base.Die();
	}
}
