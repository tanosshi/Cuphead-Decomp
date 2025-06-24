using System.Collections;
using UnityEngine;

public class PlatformingLevelBigEnemy : PlatformingLevelShootingEnemy
{
	[SerializeField]
	private Transform passDistance;

	protected float LockDistance = 500f;

	protected bool bigEnemyCameraLock;

	protected bool isDead;

	private float dist;

	protected override void Start()
	{
		base.Start();
		FrameDelayedCallback(StartLockCheck, 1);
	}

	private void StartLockCheck()
	{
		StartCoroutine(camera_locking_cr());
	}

	private IEnumerator camera_locking_cr()
	{
		while (!isDead)
		{
			if (!bigEnemyCameraLock)
			{
				dist = PlayerManager.Center.x - base.transform.position.x;
				if (dist > 0f - LockDistance)
				{
					bigEnemyCameraLock = true;
					CupheadLevelCamera.Current.LockCamera(true);
					OnLock();
				}
			}
			else if (bigEnemyCameraLock)
			{
				dist = PlayerManager.Center.x - passDistance.transform.position.x;
				if (dist > 0f)
				{
					bigEnemyCameraLock = false;
					CupheadLevelCamera.Current.LockCamera(false);
					OnPass();
					break;
				}
			}
			yield return null;
		}
	}

	protected virtual void OnPass()
	{
	}

	protected virtual void OnLock()
	{
	}

	protected override void Die()
	{
		bigEnemyCameraLock = false;
		CupheadLevelCamera.Current.LockCamera(false);
		base.Die();
	}

	protected override void OnDrawGizmos()
	{
		base.OnDrawGizmos();
		Gizmos.color = Color.cyan;
		Gizmos.DrawLine(passDistance.transform.position, new Vector3(passDistance.transform.position.x, passDistance.transform.position.y - 1000f));
	}
}
