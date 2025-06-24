using System;
using System.Collections;
using UnityEngine;

public class TrainLevelLollipopGhoul : LevelProperties.Train.Entity
{
	public enum State
	{
		Init = 0,
		Ready = 1,
		Attacking = 2,
		Dead = 3
	}

	public delegate void OnDamageTakenHandler(float damage);

	[SerializeField]
	private Transform head;

	[SerializeField]
	private Transform lightningRoot;

	[Space(10f)]
	[SerializeField]
	private TrainLevelLollipopGhoulLightning lightningPrefab;

	private float health;

	private DamageReceiver damageReceiver;

	private TrainLevelLollipopGhoulLightning currentLightning;

	public State state { get; private set; }

	public event OnDamageTakenHandler OnDamageTakenEvent;

	public event Action OnDeathEvent;

	protected override void Awake()
	{
		base.Awake();
		damageReceiver = GetComponent<DamageReceiver>();
		damageReceiver.OnDamageTaken += OnDamageTaken;
	}

	private void OnDamageTaken(DamageDealer.DamageInfo info)
	{
		if (!(health <= 0f))
		{
			if (this.OnDamageTakenEvent != null)
			{
				this.OnDamageTakenEvent(info.damage);
			}
			health -= info.damage;
			if (health <= 0f)
			{
				Die();
			}
		}
	}

	public override void LevelInit(LevelProperties.Train properties)
	{
		base.LevelInit(properties);
		health = properties.CurrentState.lollipopGhouls.health;
	}

	private void Die()
	{
		if (this.OnDeathEvent != null)
		{
			this.OnDeathEvent();
		}
		this.OnDeathEvent = null;
		StopAllCoroutines();
		StartCoroutine(die_cr());
	}

	private void DeathAnimComplete()
	{
		Debug.Log("Ghoul Dead");
		StopAllCoroutines();
		UnityEngine.Object.Destroy(base.gameObject);
	}

	public void AnimateIn()
	{
		base.animator.Play("Intro");
		state = State.Ready;
	}

	public void Attack()
	{
		state = State.Attacking;
		StartCoroutine(attack_cr());
	}

	private void StartLightning()
	{
		if (currentLightning != null)
		{
			UnityEngine.Object.Destroy(currentLightning);
		}
		currentLightning = UnityEngine.Object.Instantiate(lightningPrefab);
		currentLightning.transform.SetParent(lightningRoot);
		currentLightning.transform.ResetLocalTransforms();
	}

	private void EndLightning()
	{
		if (!(currentLightning == null))
		{
			currentLightning.End();
			currentLightning = null;
		}
	}

	private IEnumerator attack_cr()
	{
		yield return null;
		base.animator.ResetTrigger("Continue");
		base.animator.SetTrigger("OnAttack");
		yield return base.animator.WaitForAnimationToStart(this, "Attack_Charge");
		AudioManager.Play("train_lollipop_ghoul_attack_start");
		emitAudioFromObject.Add("train_lollipop_ghoul_attack_start");
		yield return CupheadTime.WaitForSeconds(this, base.properties.CurrentState.lollipopGhouls.warningTime);
		base.animator.SetTrigger("Continue");
		yield return base.animator.WaitForAnimationToStart(this, "Attack_Loop");
		AudioManager.PlayLoop("train_lollipop_ghoul_attack_loop");
		emitAudioFromObject.Add("train_lollipop_ghoul_attack_loop");
		StartLightning();
		yield return StartCoroutine(head_cr());
		EndLightning();
		AudioManager.Stop("train_lollipop_ghoul_attack_loop");
		AudioManager.Play("train_lollipop_ghoul_attack_end");
		yield return null;
		base.animator.SetTrigger("Continue");
		state = State.Ready;
	}

	private IEnumerator head_cr()
	{
		float t = 0f;
		float time = base.properties.CurrentState.lollipopGhouls.moveTime;
		EaseUtils.EaseType ease = EaseUtils.EaseType.easeInOutSine;
		Vector3 start = Vector3.zero;
		Vector3 end = new Vector3(base.properties.CurrentState.lollipopGhouls.moveDistance, 0f, 0f);
		head.localPosition = start;
		while (t < time)
		{
			float val = EaseUtils.Ease(ease, 0f, 1f, t / time);
			head.localPosition = Vector3.Lerp(start, end, val);
			t += (float)CupheadTime.Delta;
			yield return null;
		}
		head.localPosition = end;
		t = 0f;
		while (t < time)
		{
			float val2 = EaseUtils.Ease(ease, 0f, 1f, t / time);
			head.localPosition = Vector3.Lerp(end, start, val2);
			t += (float)CupheadTime.Delta;
			yield return null;
		}
		head.localPosition = start;
	}

	private IEnumerator die_cr()
	{
		AudioManager.Stop("train_lollipop_ghoul_attack_loop");
		AudioManager.Play("train_lollipop_ghoul_die");
		emitAudioFromObject.Add("train_lollipop_ghoul_die");
		state = State.Dead;
		yield return CupheadTime.WaitForSeconds(this, 0.3f);
		if (currentLightning != null)
		{
			EndLightning();
		}
		base.animator.Play("Die");
	}
}
