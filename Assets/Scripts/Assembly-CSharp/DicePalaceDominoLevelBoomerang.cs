using System.Collections;
using UnityEngine;

public class DicePalaceDominoLevelBoomerang : AbstractProjectile
{
	[SerializeField]
	private Effect deathPoof;

	private LevelProperties.DicePalaceDomino.Boomerang properties;

	private DamageReceiver damageReceiver;

	private float returnHeight;

	private float speed;

	private float HP;

	private bool isDead;

	public DicePalaceDominoLevelBoomerang Create(Vector2 pos, float speed, float hp, LevelProperties.DicePalaceDomino.Boomerang properties)
	{
		DicePalaceDominoLevelBoomerang dicePalaceDominoLevelBoomerang = base.Create(pos) as DicePalaceDominoLevelBoomerang;
		dicePalaceDominoLevelBoomerang.properties = properties;
		dicePalaceDominoLevelBoomerang.speed = speed;
		dicePalaceDominoLevelBoomerang.HP = hp;
		return dicePalaceDominoLevelBoomerang;
	}

	protected override void Start()
	{
		damageReceiver = GetComponent<DamageReceiver>();
		damageReceiver.OnDamageTaken += OnDamageTaken;
		returnHeight = Level.Current.Ground + 50;
		StartCoroutine(move_cr());
		base.Start();
	}

	private void OnDamageTaken(DamageDealer.DamageInfo info)
	{
		HP -= info.damage;
		if (HP <= 0f && !isDead)
		{
			isDead = true;
			Killed();
		}
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
		base.damageDealer.DealDamage(hit);
		base.OnCollisionPlayer(hit, phase);
	}

	private IEnumerator move_cr()
	{
		float dropPoint = (float)Level.Current.Ground + GetComponent<Collider2D>().bounds.size.y;
		float goToPos = -440f;
		while (base.transform.position.x > goToPos)
		{
			base.transform.position += Vector3.left * speed * CupheadTime.Delta;
			yield return null;
		}
		animator.SetTrigger("OnDrop");
		yield return animator.WaitForAnimationToStart(this, "Fly_Drop_Start");
		AudioManager.Play("dice_palace_domino_bird_dive");
		emitAudioFromObject.Add("dice_palace_domino_bird_dive");
		while (base.transform.position.y > dropPoint)
		{
			base.transform.position += Vector3.down * speed * CupheadTime.Delta;
			yield return null;
		}
		animator.SetTrigger("OnStop");
		yield return null;
	}

	private void ChangeDirection()
	{
		StopAllCoroutines();
		StartCoroutine(fly_right_cr());
	}

	private IEnumerator fly_right_cr()
	{
		while (base.transform.position.x < 840f)
		{
			base.transform.position += Vector3.right * speed * CupheadTime.Delta;
			yield return null;
		}
		Die();
		yield return null;
	}

	private void WingFlapSFX()
	{
		AudioManager.Play("bird_bird_flap");
		emitAudioFromObject.Add("bird_bird_flap");
	}

	private void Killed()
	{
		StopAllCoroutines();
		GetComponent<SpriteRenderer>().enabled = false;
		Die();
		AudioManager.Play("dice_bird_die");
		emitAudioFromObject.Add("dice_bird_die");
		animator.SetTrigger("OnDeath");
	}

	private void SpawnEffect()
	{
		GetComponent<Collider2D>().enabled = false;
		deathPoof.Create(base.transform.position);
	}
}
