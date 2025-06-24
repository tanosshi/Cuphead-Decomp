using System.Collections;
using UnityEngine;

public class FlyingGenieLevelGoop : LevelProperties.FlyingGenie.Entity
{
	[SerializeField]
	private Transform topRoot;

	[SerializeField]
	private Transform bottomRoot;

	[SerializeField]
	private FlyingGenieLevelHelixProjectile projectile;

	[SerializeField]
	private Transform endRoot;

	private bool moving;

	private float yMax = 60f;

	private float yMin = -260f;

	private DamageReceiver damageReceiver;

	public override void LevelInit(LevelProperties.FlyingGenie properties)
	{
		base.LevelInit(properties);
		damageReceiver = GetComponent<DamageReceiver>();
		damageReceiver.OnDamageTaken += OnDamageTaken;
	}

	public void ActivateGoop()
	{
		base.animator.SetTrigger("OnStartGoop");
		GetComponent<Collider2D>().enabled = true;
		GetComponent<SpriteRenderer>().enabled = true;
		StartCoroutine(move_cr());
		StartCoroutine(shoot_cr());
	}

	private void DeactivateGoop()
	{
		moving = false;
		GetComponent<Collider2D>().enabled = true;
		GetComponent<SpriteRenderer>().enabled = true;
		StopAllCoroutines();
		base.animator.Play("Off");
	}

	private void OnDamageTaken(DamageDealer.DamageInfo info)
	{
		base.properties.DealDamage(info.damage);
	}

	public IEnumerator move_cr()
	{
		LevelProperties.FlyingGenie.Coffin p = base.properties.CurrentState.coffin;
		bool goingUp = false;
		moving = true;
		yield return base.animator.WaitForAnimationToEnd(this, "Intro");
		while (true)
		{
			if (moving)
			{
				if (goingUp)
				{
					while (base.transform.position.y < yMax)
					{
						base.transform.AddPosition(0f, p.heartMovement * (float)CupheadTime.Delta);
						yield return null;
					}
					goingUp = !goingUp;
				}
				else
				{
					while (base.transform.position.y > yMin)
					{
						base.transform.AddPosition(0f, (0f - p.heartMovement) * (float)CupheadTime.Delta);
						yield return null;
					}
					goingUp = !goingUp;
				}
			}
			yield return null;
		}
	}

	private IEnumerator shoot_cr()
	{
		yield return base.animator.WaitForAnimationToEnd(this, "Intro");
		while (true)
		{
			if (moving)
			{
				yield return CupheadTime.WaitForSeconds(this, base.properties.CurrentState.coffin.heartShotDelayRange.RandomFloat());
				base.animator.SetTrigger("OnAttack");
				yield return base.animator.WaitForAnimationToEnd(this, "Attack");
			}
			yield return null;
		}
	}

	private void ShootProjectiles()
	{
		AudioManager.Play("genie_sarcophagus_eye_plop");
		emitAudioFromObject.Add("genie_sarcophagus_eye_plop");
		projectile.Create(topRoot.position, base.properties.CurrentState.coffin, true);
		projectile.Create(bottomRoot.position, base.properties.CurrentState.coffin, false);
	}

	public void StartDeath()
	{
		StopAllCoroutines();
		base.animator.SetTrigger("OnDeath");
		StartCoroutine(death_cr());
		AudioManager.Play("genie_goop_voice_exit");
		emitAudioFromObject.Add("genie_goop_voice_exit");
	}

	private IEnumerator death_cr()
	{
		float moveSpeed = 50f;
		yield return CupheadTime.WaitForSeconds(this, 0.5f);
		while (base.transform.localPosition.x < endRoot.localPosition.x)
		{
			base.transform.localPosition += base.transform.right * moveSpeed;
			yield return null;
		}
		GetComponent<SpriteRenderer>().enabled = false;
		DeactivateGoop();
		yield return null;
	}

	private void SoundGenieGoopIntro()
	{
		AudioManager.Play("genie_goop_voice_enter");
		emitAudioFromObject.Add("genie_goop_voice_enter");
	}

	private void SoundGenieGoopAttackPre()
	{
		AudioManager.Play("genie_goop_attack_pre");
		emitAudioFromObject.Add("genie_goop_attack_pre");
	}

	private void SoundGenieGoopAttack()
	{
		AudioManager.Play("gene_goop_voice_attack");
		emitAudioFromObject.Add("gene_goop_voice_attack");
	}
}
