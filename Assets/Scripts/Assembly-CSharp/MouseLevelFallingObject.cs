using System.Collections;
using UnityEngine;

public class MouseLevelFallingObject : AbstractProjectile
{
	[SerializeField]
	private Effect explosionSmall;

	private float gravity;

	private float speed;

	public MouseLevelFallingObject Create(float xPos, LevelProperties.Mouse.Claw properties)
	{
		Vector2 vector = new Vector2(-600f, 50f);
		MouseLevelFallingObject mouseLevelFallingObject = InstantiatePrefab<MouseLevelFallingObject>();
		mouseLevelFallingObject.GetComponent<Animator>().SetInteger("Pick", Random.Range(0, 3));
		mouseLevelFallingObject.speed = properties.objectStartingFallSpeed;
		mouseLevelFallingObject.gravity = properties.objectGravity;
		mouseLevelFallingObject.transform.SetPosition(xPos + vector.x, (float)Level.Current.Ceiling + vector.y);
		mouseLevelFallingObject.StartCoroutine(mouseLevelFallingObject.move_cr());
		return mouseLevelFallingObject;
	}

	protected override void Update()
	{
		base.Update();
		if (base.damageDealer != null)
		{
			base.damageDealer.Update();
		}
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionPlayer(hit, phase);
		base.damageDealer.DealDamage(hit);
	}

	private IEnumerator move_cr()
	{
		while (base.transform.position.y > (float)Level.Current.Ground)
		{
			speed += gravity * CupheadTime.FixedDelta;
			base.transform.AddPosition(0f, (0f - speed) * CupheadTime.FixedDelta);
			yield return new WaitForFixedUpdate();
		}
		explosionSmall.Create(base.transform.position);
		animator.SetTrigger("Death");
		AudioManager.Play("level_mouse_debris_smash");
		emitAudioFromObject.Add("level_mouse_debris_smash");
	}

	private void DestroyWood()
	{
		Object.Destroy(base.gameObject);
	}
}
