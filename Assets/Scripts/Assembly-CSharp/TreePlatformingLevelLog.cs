using System.Collections;
using UnityEngine;

public class TreePlatformingLevelLog : AbstractPlatformingLevelEnemy
{
	[SerializeField]
	private TreePlatformingLevelLogProjectile projectile;

	[SerializeField]
	private Transform root;

	[SerializeField]
	private float shootDelay;

	[SerializeField]
	private SpriteDeathParts[] parts;

	[SerializeField]
	private bool canShoot;

	[SerializeField]
	private string pinkString;

	[SerializeField]
	private Effect projectilePuff;

	private bool facingRight;

	private string[] pinkPattern;

	private int pinkIndex;

	public bool isDying;

	public bool isSliding;

	public float start;

	public bool CanShoot
	{
		get
		{
			return canShoot;
		}
	}

	public float ShootDelay
	{
		get
		{
			return shootDelay;
		}
	}

	protected override void Start()
	{
		base.Start();
		base._damageReceiver.enabled = false;
		pinkPattern = pinkString.Split(',');
		pinkIndex = Random.Range(0, pinkPattern.Length);
	}

	protected override void OnStart()
	{
	}

	public void SlideDown(float belowBoundsY)
	{
		StartCoroutine(slide_cr(belowBoundsY));
	}

	private IEnumerator slide_cr(float belowBoundsY)
	{
		isSliding = true;
		YieldInstruction wait = new WaitForFixedUpdate();
		while (base.transform.position.y > start - belowBoundsY)
		{
			base.transform.AddPosition(0f, (0f - base.Properties.MoveSpeed) * CupheadTime.FixedDelta);
			yield return wait;
		}
		start = base.transform.position.y;
		isSliding = false;
		yield return null;
	}

	protected override void Die()
	{
	}

	public void KillLog()
	{
		SpawnPieces();
		isDying = true;
		base.Die();
	}

	private void SpawnPieces()
	{
		AudioManager.Play("level_platform_logface_death");
		emitAudioFromObject.Add("level_platform_logface_death");
		SpriteDeathParts[] array = parts;
		foreach (SpriteDeathParts spriteDeathParts in array)
		{
			spriteDeathParts.CreatePart(base.transform.position);
		}
	}

	public void OnShoot()
	{
		if (canShoot)
		{
			base.animator.SetTrigger("OnShoot");
		}
	}

	private void Shoot()
	{
		float num = base.Properties.ProjectileSpeed;
		if (facingRight)
		{
			num *= -1f;
		}
		projectile.Create(root.transform.position, 180f, num, !facingRight, pinkPattern[pinkIndex][0] == 'P');
		pinkIndex = (pinkIndex + 1) % pinkPattern.Length;
		Effect effect = projectilePuff.Create(root.transform.position);
		effect.GetComponent<SpriteRenderer>().flipY = facingRight;
	}

	public void SetDirection(bool isRight)
	{
		facingRight = isRight;
		if (facingRight)
		{
			Vector3 localScale = base.transform.localScale;
			localScale.x *= -1f;
			base.transform.localScale = localScale;
		}
	}
}
