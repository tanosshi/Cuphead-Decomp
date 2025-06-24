using System.Collections;
using UnityEngine;

public class MountainPlatformingLevelDragon : PlatformingLevelShootingEnemy
{
	private Vector3 endPos;

	private Vector3 startPos;

	protected override void Start()
	{
		base.Start();
		AudioManager.Play("castle_dragon_spawn");
		emitAudioFromObject.Add("castle_dragon_spawn");
	}

	public void Init(Vector3 startPos, Vector3 endPos)
	{
		this.startPos = startPos;
		base.transform.position = startPos;
		this.endPos = endPos;
		StartCoroutine(move_to_pos_cr());
		StartCoroutine(check_cr());
	}

	private IEnumerator move_to_pos_cr()
	{
		float t = 0f;
		float time = base.Properties.dragonTimeIn;
		Vector2 start = base.transform.position;
		_target = PlayerManager.GetNext();
		while (t < time)
		{
			float val = EaseUtils.Ease(EaseUtils.EaseType.easeInOutSine, 0f, 1f, t / time);
			base.transform.position = Vector2.Lerp(start, endPos, val);
			t += (float)CupheadTime.Delta;
			yield return null;
		}
		StartShoot();
		yield return null;
	}

	private IEnumerator check_cr()
	{
		while (MountainPlatformingLevelElevatorHandler.elevatorIsMoving)
		{
			yield return null;
		}
		Die();
	}

	protected override void Shoot()
	{
		base.Shoot();
		AudioManager.Play("castle_dragon_attack");
		emitAudioFromObject.Add("castle_dragon_attack");
		StartCoroutine(leave_cr());
	}

	protected override void SpawnShootEffect()
	{
		if (base.transform.localScale.x < 0f)
		{
			_effectRoot.localEulerAngles = new Vector3(0f, 0f, _effectRoot.localEulerAngles.z - 180f);
		}
		if (_shootEffect != null)
		{
			Effect effect = _shootEffect.Create(_effectRoot.position);
			effect.transform.rotation = _effectRoot.rotation;
		}
	}

	private IEnumerator leave_cr()
	{
		float t = 0f;
		float time = base.Properties.dragonTimeOut;
		base.transform.position = endPos;
		Vector2 start = base.transform.position;
		yield return CupheadTime.WaitForSeconds(this, base.Properties.dragonLeaveDelay);
		while (t < time)
		{
			float val = EaseUtils.Ease(EaseUtils.EaseType.easeInOutSine, 0f, 1f, t / time);
			base.transform.position = Vector2.Lerp(start, startPos, val);
			t += (float)CupheadTime.Delta;
			yield return null;
		}
		Die();
		yield return null;
	}

	protected override void Die()
	{
		AudioManager.Play("castle_dragon_death");
		emitAudioFromObject.Add("castle_dragon_death");
		base.Die();
	}

	private void ActivateTail()
	{
		base.animator.Play("Tail", 1);
	}
}
