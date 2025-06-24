using System.Collections;
using UnityEngine;

public class ClownLevelHorseshoe : AbstractProjectile
{
	[SerializeField]
	private Effect greenSparkle;

	[SerializeField]
	private Effect pinkSparkle;

	[SerializeField]
	private Effect yellowSparkle;

	[SerializeField]
	private Effect deathPoof;

	private Effect selectedSparkle;

	private LevelProperties.Clown.Horse properties;

	private float velocityX;

	private float velocityY;

	private bool onRight;

	private float durationBeforeDrop;

	private bool isSparkling = true;

	public void Init(Vector2 pos, float velocityX, float velocityY, bool onRight, float durationBeforeDrop, LevelProperties.Clown.Horse properties, ClownLevelClownHorse.HorseType horseType)
	{
		base.transform.position = pos;
		this.velocityX = velocityX;
		this.velocityY = velocityY;
		this.properties = properties;
		this.onRight = onRight;
		this.durationBeforeDrop = durationBeforeDrop;
		switch (horseType)
		{
		case ClownLevelClownHorse.HorseType.Wave:
			StartCoroutine(wave_cr());
			if (base.CanParry)
			{
				animator.SetInteger("type", 2);
				selectedSparkle = pinkSparkle;
			}
			else
			{
				animator.SetInteger("type", 1);
				selectedSparkle = greenSparkle;
			}
			break;
		case ClownLevelClownHorse.HorseType.Drop:
			StartCoroutine(move_to_drop_point_cr());
			selectedSparkle = yellowSparkle;
			animator.SetInteger("type", 0);
			break;
		default:
			Debug.Log("Invalid number");
			break;
		}
		StartCoroutine(spawn_sparkle_cr());
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionPlayer(hit, phase);
		base.damageDealer.DealDamage(hit);
	}

	protected override void Update()
	{
		base.Update();
		if (base.damageDealer != null)
		{
			base.damageDealer.Update();
		}
	}

	private IEnumerator wave_cr()
	{
		float angle = 0f;
		float speed = 0f;
		float loopSize = 0f;
		Vector3 moveX = base.transform.position;
		float edge = ((!onRight) ? 690f : (-690f));
		speed = ((!onRight) ? velocityX : (0f - velocityX));
		while ((!onRight) ? (base.transform.position.x < edge) : (base.transform.position.x > edge))
		{
			loopSize = ((!(velocityY < 0f)) ? properties.WaveBulletAmount : (0f - properties.WaveBulletAmount));
			angle += velocityY * (float)CupheadTime.Delta;
			Vector3 moveY = new Vector3(0f, Mathf.Sin(angle + properties.WaveBulletAmount) * (float)CupheadTime.Delta * 60f * loopSize / 2f);
			moveX = base.transform.right * speed * CupheadTime.Delta;
			base.transform.position += moveX + moveY;
			yield return null;
		}
		Die();
		yield return null;
	}

	private IEnumerator move_to_drop_point_cr()
	{
		Vector3 pos = base.transform.position;
		if (onRight)
		{
			float leavePos = -740f;
			while (base.transform.position.x > leavePos)
			{
				base.transform.AddPosition((0f - velocityX) * (float)CupheadTime.Delta);
				yield return null;
			}
			pos.x = -740f;
		}
		else
		{
			float leavePos = 740f;
			while (base.transform.position.x < leavePos)
			{
				base.transform.AddPosition(velocityX * (float)CupheadTime.Delta);
				yield return null;
			}
			pos.x = 740f;
		}
		pos.y = 260f;
		base.transform.position = pos;
		float dropPos = (onRight ? (640f - velocityY) : (-640f + velocityY));
		animator.SetTrigger("onTop");
		yield return CupheadTime.WaitForSeconds(this, properties.DropBulletDelay);
		while (base.transform.position.x != dropPos)
		{
			pos.x = Mathf.MoveTowards(base.transform.position.x, dropPos, velocityX * (float)CupheadTime.Delta);
			base.transform.position = pos;
			yield return null;
		}
		isSparkling = false;
		yield return CupheadTime.WaitForSeconds(this, durationBeforeDrop);
		isSparkling = true;
		animator.SetTrigger("down");
		AudioManager.Play("clown_horseshoe_drop");
		emitAudioFromObject.Add("clown_horseshoe_drop");
		while (base.transform.position.y > (float)Level.Current.Ground)
		{
			pos.y -= properties.DropBulletSpeedDown * (float)CupheadTime.Delta;
			base.transform.position = pos;
			yield return null;
		}
		AudioManager.Play("clown_horseshoe_land");
		emitAudioFromObject.Add("clown_horseshoe_land");
		animator.SetTrigger("dead");
		deathPoof.Create(base.transform.position);
		yield return null;
	}

	public IEnumerator drop_cr()
	{
		Vector3 pos = base.transform.position;
		yield return null;
	}

	private IEnumerator simple_cr()
	{
		float speed = 0f;
		float edge = ((!onRight) ? 640 : (-640));
		speed = ((!onRight) ? velocityX : (0f - velocityX));
		while (base.transform.position.x != edge)
		{
			base.transform.AddPosition(speed * (float)CupheadTime.Delta);
			yield return null;
		}
		yield return null;
	}

	private IEnumerator spawn_sparkle_cr()
	{
		while (true)
		{
			yield return CupheadTime.WaitForSeconds(this, 0.1f);
			if (isSparkling)
			{
				selectedSparkle.Create(base.transform.position);
			}
			yield return null;
		}
	}

	protected override void Die()
	{
		StopAllCoroutines();
		base.transform.GetComponent<SpriteRenderer>().enabled = false;
		base.Die();
	}
}
