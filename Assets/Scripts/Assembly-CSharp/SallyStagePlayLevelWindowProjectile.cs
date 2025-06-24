using System.Collections;
using UnityEngine;

public class SallyStagePlayLevelWindowProjectile : BasicProjectile
{
	[SerializeField]
	private CollisionChild child;

	public SallyStagePlayLevelWindowProjectile Create(Vector2 pos, float rotation, float speed, SallyStagePlayLevel parent)
	{
		return base.Create(pos, rotation, speed) as SallyStagePlayLevelWindowProjectile;
	}

	protected override void Start()
	{
		base.Start();
		child.transform.SetEulerAngles(null, null, 0f);
		child.OnPlayerCollision += OnCollisionPlayer;
		StartCoroutine(on_ground_hit_cr());
	}

	private void OnPhase3()
	{
		Object.Destroy(base.gameObject);
	}

	protected override void Die()
	{
		base.Die();
	}

	private IEnumerator on_ground_hit_cr()
	{
		while (base.transform.position.y > (float)Level.Current.Ground)
		{
			yield return null;
		}
		move = false;
		animator.SetTrigger("OnSmash");
		AudioManager.Play("sally_bottle_smash");
		emitAudioFromObject.Add("sally_bottle_smash");
		yield return null;
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
	}
}
