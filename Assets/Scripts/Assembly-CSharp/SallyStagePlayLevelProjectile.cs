using System.Collections;
using UnityEngine;

public class SallyStagePlayLevelProjectile : AbstractCollidableObject
{
	[SerializeField]
	private Transform sprite;

	private float speed;

	private float rotation;

	private DamageDealer damageDealer;

	private LevelProperties.SallyStagePlay.Projectile properties;

	protected override void Awake()
	{
		damageDealer = DamageDealer.NewEnemy();
		base.Awake();
	}

	protected override void Update()
	{
		base.Update();
		sprite.SetEulerAngles(0f, 0f, 0f);
		if (damageDealer != null)
		{
			damageDealer.Update();
		}
	}

	public void Init(Vector2 pos, float rotation, LevelProperties.SallyStagePlay.Projectile properties)
	{
		base.transform.position = pos;
		this.properties = properties;
		speed = properties.projectileSpeed;
		this.rotation = rotation;
		base.transform.SetEulerAngles(null, null, rotation);
		StartCoroutine(move_cr());
		AudioManager.Play("sally_fan_shoot");
		emitAudioFromObject.Add("sally_fan_shoot");
	}

	private IEnumerator move_cr()
	{
		AudioManager.PlayLoop("sally_fan_shoot_loop");
		emitAudioFromObject.Add("sally_fan_shoot_loop");
		while (base.transform.position.y > (float)Level.Current.Ground)
		{
			base.transform.position += base.transform.right * speed * CupheadTime.Delta;
			yield return null;
		}
		base.animator.SetTrigger("OnLand");
		AudioManager.Play("sally_fan_stick");
		emitAudioFromObject.Add("sally_fan_stick");
		AudioManager.Stop("sally_fan_shoot_loop");
		yield return CupheadTime.WaitForSeconds(this, properties.groundDuration);
		base.animator.SetTrigger("OnDeath");
	}

	private void Die()
	{
		StopAllCoroutines();
		AudioManager.Play("sally_fan_dissappear");
		emitAudioFromObject.Add("sally_fan_dissappear");
		Object.Destroy(base.gameObject);
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		damageDealer.DealDamage(hit);
		base.OnCollisionPlayer(hit, phase);
	}
}
