using System.Collections;
using UnityEngine;

public class MountainPlatformingLevelPickaxeProjectile : AbstractProjectile
{
	private Vector3 minerPosition;

	private Vector3 targetPos;

	private MountainPlatformingLevelMiner miner;

	private float speed;

	private bool towardsPlayer;

	private const float MAX_DIST = 5f;

	protected override void Start()
	{
		towardsPlayer = true;
		base.Start();
		StartCoroutine(throw_pickaxe_cr());
	}

	protected override void Update()
	{
		base.Update();
		if (base.damageDealer != null)
		{
			base.damageDealer.Update();
		}
	}

	public MountainPlatformingLevelPickaxeProjectile Create(Vector2 pos, float rotation, float speed, MountainPlatformingLevelMiner miner, Vector3 targetPos, Vector3 minerPos)
	{
		MountainPlatformingLevelPickaxeProjectile mountainPlatformingLevelPickaxeProjectile = base.Create(pos, rotation) as MountainPlatformingLevelPickaxeProjectile;
		mountainPlatformingLevelPickaxeProjectile.miner = miner;
		mountainPlatformingLevelPickaxeProjectile.speed = speed;
		mountainPlatformingLevelPickaxeProjectile.minerPosition = minerPos;
		mountainPlatformingLevelPickaxeProjectile.targetPos = targetPos;
		return mountainPlatformingLevelPickaxeProjectile;
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionPlayer(hit, phase);
		base.damageDealer.DealDamage(hit);
	}

	private IEnumerator throw_pickaxe_cr()
	{
		YieldInstruction wait = new WaitForFixedUpdate();
		Vector3 startPos = base.transform.position;
		float time = Vector3.Distance(base.transform.position, targetPos) / speed;
		float t = 0f;
		while (t < time)
		{
			t += CupheadTime.FixedDelta;
			float val = EaseUtils.Ease(EaseUtils.EaseType.easeOutSine, 0f, 1f, t / time);
			base.transform.position = Vector3.Lerp(startPos, targetPos, val);
			yield return wait;
		}
		yield return wait;
		base.transform.position = targetPos;
		t = 0f;
		Vector3 dir = startPos - targetPos;
		while (true)
		{
			if (miner != null)
			{
				if (!(t < time))
				{
					break;
				}
				t += CupheadTime.FixedDelta;
				float t2 = EaseUtils.Ease(EaseUtils.EaseType.easeInSine, 0f, 1f, t / time);
				base.transform.position = Vector3.Lerp(targetPos, minerPosition, t2);
			}
			else
			{
				base.transform.position += dir.normalized * speed * CupheadTime.FixedDelta;
			}
			yield return wait;
		}
		Object.Destroy(base.gameObject);
	}
}
