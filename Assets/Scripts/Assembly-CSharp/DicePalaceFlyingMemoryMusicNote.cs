using System.Collections;
using UnityEngine;

public class DicePalaceFlyingMemoryMusicNote : BasicProjectile
{
	[SerializeField]
	private Transform sprite;

	private float deathTimer;

	public DicePalaceFlyingMemoryMusicNote Create(Vector3 pos, float rotation, float speed, float deathTimer)
	{
		DicePalaceFlyingMemoryMusicNote dicePalaceFlyingMemoryMusicNote = base.Create(pos, rotation, speed) as DicePalaceFlyingMemoryMusicNote;
		dicePalaceFlyingMemoryMusicNote.deathTimer = deathTimer;
		return dicePalaceFlyingMemoryMusicNote;
	}

	protected override void Start()
	{
		base.Start();
		StartCoroutine(death_timer_cr());
	}

	protected override void FixedUpdate()
	{
		base.FixedUpdate();
		sprite.transform.SetEulerAngles(null, null, 0f);
	}

	private IEnumerator death_timer_cr()
	{
		yield return CupheadTime.WaitForSeconds(this, deathTimer);
		animator.SetTrigger("OnDeath");
		yield return animator.WaitForAnimationToEnd(this, "Note_Death");
		move = false;
		yield return null;
	}

	private void Kill()
	{
		Object.Destroy(base.gameObject);
	}
}
