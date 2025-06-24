using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class MountainPlatformingLevelWall : AbstractPlatformingLevelEnemy
{
	[SerializeField]
	private Transform groundPosY;

	[SerializeField]
	private Transform platform;

	[SerializeField]
	private SpriteRenderer foreground1;

	[SerializeField]
	private SpriteRenderer foreground2;

	[SerializeField]
	private SpriteRenderer shield;

	[SerializeField]
	private Transform head;

	[SerializeField]
	private Transform startTrigger;

	[SerializeField]
	private Transform projectileRoot;

	[SerializeField]
	private Effect projectileEffect;

	[SerializeField]
	private Effect projectilePinkEffect;

	[SerializeField]
	private MountainPlatformingLevelWallProjectile bouncyProjectile;

	[SerializeField]
	private MountainPlatformingLevelWallProjectile bouncyPinkProjectile;

	private int projectileCount;

	private bool isDead;

	protected override void OnStart()
	{
	}

	protected override void Start()
	{
		base.Start();
		StartCoroutine(move_cr());
		GetComponent<Collider2D>().enabled = false;
		head.GetComponent<Collider2D>().enabled = false;
		platform.gameObject.SetActive(false);
		GetComponent<DamageReceiver>().enabled = false;
		head.GetComponent<DamageReceiver>().OnDamageTaken += OnDamageTaken;
		head.gameObject.tag = Tags.Enemy.ToString();
	}

	private void FaceOn()
	{
		base.animator.Play("Face_Idle");
		base.animator.Play("Shield_Idle");
	}

	private IEnumerator move_cr()
	{
		yield return CupheadTime.WaitForSeconds(this, 0.2f);
		AbstractPlayerController player = PlayerManager.GetNext();
		while (player.transform.position.x < startTrigger.transform.position.x)
		{
			yield return null;
			if (player == null || player.IsDead)
			{
				player = PlayerManager.GetNext();
			}
		}
		GetComponent<Collider2D>().enabled = true;
		head.GetComponent<Collider2D>().enabled = true;
		platform.gameObject.SetActive(true);
		base.animator.SetTrigger("OnIntro");
		yield return base.animator.WaitForAnimationToEnd(this, "Wall_Intro");
		StartCoroutine(shoot_cr());
		float t = 0f;
		float time = base.Properties.wallFaceTravelTime;
		bool movingUp = false;
		float top = head.transform.position.y + 100f;
		float bottom = head.transform.position.y - 100f;
		float start = head.transform.position.y;
		float end = 0f;
		while (true)
		{
			start = head.transform.position.y;
			end = ((!movingUp) ? bottom : top);
			while (t < time)
			{
				TransformExtensions.SetPosition(y: EaseUtils.Ease(EaseUtils.EaseType.easeInOutSine, start, end, t / time), transform: head.transform);
				t += (float)CupheadTime.Delta;
				yield return null;
			}
			head.transform.SetPosition(null, end);
			movingUp = !movingUp;
			t = 0f;
			yield return null;
		}
	}

	private IEnumerator shoot_cr()
	{
		while (true)
		{
			yield return CupheadTime.WaitForSeconds(this, base.Properties.wallAttackDelay.RandomFloat());
			base.animator.SetTrigger("Attack");
			yield return null;
		}
	}

	private void ShootProjectileEffect()
	{
		if (projectileCount == 2)
		{
			projectilePinkEffect.Create(new Vector3(projectileRoot.transform.position.x - 20f, projectileRoot.transform.position.y));
		}
		else
		{
			projectileEffect.Create(new Vector3(projectileRoot.transform.position.x - 20f, projectileRoot.transform.position.y));
		}
	}

	private void ShootProjectile()
	{
		if (projectileCount == 2)
		{
			projectileCount = 0;
			bouncyPinkProjectile.Create(projectileRoot.position, 0f, new Vector2(0f - base.Properties.wallProjectileXSpeed, base.Properties.wallProjectileYSpeed), base.Properties.wallProjectileGravity, groundPosY.position.y);
		}
		else
		{
			projectileCount++;
			bouncyProjectile.Create(projectileRoot.position, 0f, new Vector2(0f - base.Properties.wallProjectileXSpeed, base.Properties.wallProjectileYSpeed), base.Properties.wallProjectileGravity, groundPosY.position.y);
		}
	}

	protected override void OnDrawGizmos()
	{
		base.OnDrawGizmos();
		Gizmos.color = new Color(0f, 0f, 1f, 1f);
		Gizmos.DrawLine(startTrigger.transform.position, new Vector3(startTrigger.transform.position.x, 5000f, 0f));
	}

	protected override void Die()
	{
		StopAllCoroutines();
		head.GetComponent<Collider2D>().enabled = false;
		base.animator.SetTrigger("OnDeath");
		StartCoroutine(dying_cr());
		StartCoroutine(death_shake_cr());
		StartCoroutine(create_explosions_cr());
	}

	private void FaceDead()
	{
		base.animator.Play("Face_Death_Loop");
	}

	private IEnumerator death_shake_cr()
	{
		bool movingUp = false;
		float top = base.transform.position.y + 4f;
		float bottom = base.transform.position.y - 4f;
		float start = base.transform.position.y;
		float end = 0f;
		float t = 0f;
		float time = 0.01f;
		while (!isDead)
		{
			start = base.transform.position.y;
			end = ((!movingUp) ? bottom : top);
			while (t < time)
			{
				TransformExtensions.SetPosition(y: EaseUtils.Ease(EaseUtils.EaseType.easeInOutBounce, start, end, t / time), transform: base.transform);
				t += (float)CupheadTime.Delta;
				yield return null;
			}
			base.transform.SetPosition(null, end);
			movingUp = !movingUp;
			t = 0f;
			yield return null;
		}
		yield return null;
	}

	private IEnumerator create_explosions_cr()
	{
		while (!isDead)
		{
			GetComponent<EffectRadius>().CreateInRadius();
			yield return CupheadTime.WaitForSeconds(this, Random.Range(0.2f, 0.4f));
			yield return null;
		}
	}

	private IEnumerator dying_cr()
	{
		AudioManager.Play("castle_mountain_wall_death");
		emitAudioFromObject.Add("castle_mountain_wall_death");
		yield return base.animator.WaitForAnimationToEnd(this, "Wall_Death");
		yield return CupheadTime.WaitForSeconds(this, 1.67f);
		float t = 0f;
		float time = 0.65f;
		while (t < time)
		{
			GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f - t / time);
			head.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f - t / time);
			shield.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f - t / time);
			foreground1.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f - t / time);
			foreground2.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f - t / time);
			t += (float)CupheadTime.Delta;
			yield return null;
		}
		isDead = true;
		_003CDie_003E__BaseCallProxy0();
		yield return null;
	}

	private void SoundMountainWallShoot()
	{
		AudioManager.Play("castle_mountain_wall_attack");
		emitAudioFromObject.Add("castle_mountain_wall_attack");
	}

	private void SoundMountainWallIntro()
	{
		AudioManager.Play("castle_mountain_wall_spawn");
		emitAudioFromObject.Add("castle_mountain_wall_spawn");
	}

	[CompilerGenerated]
	[DebuggerHidden]
	private void _003CDie_003E__BaseCallProxy0()
	{
		base.Die();
	}
}
