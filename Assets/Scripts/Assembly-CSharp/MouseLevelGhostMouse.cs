using System.Collections;
using UnityEngine;

public class MouseLevelGhostMouse : AbstractCollidableObject
{
	public enum State
	{
		Unspawned = 0,
		Intro = 1,
		Idle = 2,
		Attack = 3,
		Dying = 4
	}

	private const float heightVariation = 35f;

	private const float spawnXRatio = 0.125f;

	private Vector2 basePos;

	private LevelProperties.Mouse properties;

	private float hp;

	[SerializeField]
	private MouseLevelGhostMouseBall blueBallPrefab;

	[SerializeField]
	private MouseLevelGhostMouseBall pinkBallPrefab;

	[SerializeField]
	private Transform projectileRoot;

	public State state { get; private set; }

	protected override void Awake()
	{
		base.Awake();
		basePos = base.transform.localPosition;
		GetComponent<DamageReceiver>().OnDamageTaken += OnDamageTaken;
	}

	private void OnDamageTaken(DamageDealer.DamageInfo info)
	{
		hp -= info.damage;
		if (hp < 0f && state != State.Dying)
		{
			Die();
		}
	}

	public void Spawn(LevelProperties.Mouse properties)
	{
		this.properties = properties;
		if (state == State.Unspawned)
		{
			StopAllCoroutines();
			state = State.Intro;
			base.animator.ResetTrigger("AttackBlue");
			base.animator.ResetTrigger("AttackPink");
			base.animator.ResetTrigger("Continue");
			StartCoroutine(spawn_cr());
		}
	}

	private IEnumerator spawn_cr()
	{
		float spawnOffset = 150f * base.transform.localScale.x;
		float yPos = basePos.y + Random.Range(-35f, 35f);
		Vector2 start = new Vector2(basePos.x * 0.125f + spawnOffset, yPos);
		hp = properties.CurrentState.ghostMouse.hp;
		base.transform.SetEulerAngles(0f, 0f, 0f);
		base.animator.SetTrigger("Spawn");
		float t = 0f;
		while (t < 1.083f)
		{
			base.transform.SetLocalPosition(EaseUtils.Ease(EaseUtils.EaseType.easeInOutSine, start.x, basePos.x, t / 1.083f), yPos);
			t += (float)CupheadTime.Delta;
			yield return null;
		}
		base.transform.SetLocalPosition(basePos.x, yPos);
		yield return base.animator.WaitForAnimationToStart(this, "Idle_A");
		state = State.Idle;
	}

	public void Attack(bool pink)
	{
		state = State.Attack;
		StartCoroutine(attack_cr(pink));
	}

	private IEnumerator attack_cr(bool pink)
	{
		base.animator.SetTrigger((!pink) ? "AttackBlue" : "AttackPink");
		yield return base.animator.WaitForAnimationToStart(this, (!pink) ? "Attack_Blue_Loop" : "Attack_Pink_Loop");
		yield return CupheadTime.WaitForSeconds(this, properties.CurrentState.ghostMouse.attackAnticipation);
		base.animator.SetTrigger("Continue");
		yield return base.animator.WaitForAnimationToStart(this, "Idle_B");
		state = State.Idle;
	}

	private void FireBlue()
	{
		blueBallPrefab.Create(projectileRoot.position, properties.CurrentState.ghostMouse.ballSpeed, properties.CurrentState.ghostMouse.splitSpeed);
	}

	private void FirePink()
	{
		pinkBallPrefab.Create(projectileRoot.position, properties.CurrentState.ghostMouse.ballSpeed, properties.CurrentState.ghostMouse.splitSpeed);
	}

	public void Die()
	{
		if (state != State.Unspawned && state != State.Dying)
		{
			state = State.Dying;
			StopAllCoroutines();
			StartCoroutine(death_cr());
		}
	}

	private IEnumerator death_cr()
	{
		while (state == State.Intro)
		{
			yield return null;
		}
		base.animator.SetTrigger("Die");
		base.transform.Rotate(0f, 0f, Random.Range(-16, 16));
		yield return base.animator.WaitForAnimationToEnd(this, "Death");
		state = State.Unspawned;
	}

	private void SoundMouseGhostWail()
	{
		AudioManager.Play("level_mouse_ghost_mouse_wail");
		emitAudioFromObject.Add("level_mouse_ghost_mouse_wail");
	}

	private void SoundMouseGhostLaugh()
	{
		AudioManager.Play("level_mouse_ghost_mouse_laugh");
		emitAudioFromObject.Add("level_mouse_ghost_mouse_laugh");
	}

	private void SoundMouseGhostAttack()
	{
		AudioManager.Play("level_mouse_ghost_attack");
		emitAudioFromObject.Add("level_mouse_ghost_attack");
	}

	private void SoundMouseGhostDeath()
	{
		AudioManager.Play("level_mouse_ghost_death");
		emitAudioFromObject.Add("level_mouse_ghost_death");
	}

	private void SoundMouseGhostDeathStart()
	{
		AudioManager.Play("level_mouse_ghost_death_start");
		emitAudioFromObject.Add("level_mouse_ghost_death_start");
	}
}
