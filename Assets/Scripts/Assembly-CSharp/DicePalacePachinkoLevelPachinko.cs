using System.Collections;
using UnityEngine;

public class DicePalacePachinkoLevelPachinko : LevelProperties.DicePalacePachinko.Entity
{
	[SerializeField]
	private Animator fire;

	[SerializeField]
	private Transform[] lights;

	[SerializeField]
	private Sprite[] beamSprites;

	[SerializeField]
	private GameObject beam;

	private bool reversing;

	private int direction;

	private float baseSpeed;

	private float pct;

	private float initialHP;

	private DamageDealer damageDealer;

	private DamageReceiver damageReceiver;

	public bool attacking { get; private set; }

	protected override void Awake()
	{
		reversing = false;
		damageDealer = DamageDealer.NewEnemy();
		damageReceiver = GetComponent<DamageReceiver>();
		damageReceiver.OnDamageTaken += OnDamageTaken;
		Level.Current.OnWinEvent += OnDeath;
		base.Awake();
	}

	protected override void Update()
	{
		damageDealer.Update();
		base.Update();
	}

	private void OnDamageTaken(DamageDealer.DamageInfo info)
	{
		pct = 1f - base.properties.CurrentHealth / initialHP;
		base.properties.DealDamage(info.damage);
		Debug.Log(pct);
	}

	public override void LevelInit(LevelProperties.DicePalacePachinko properties)
	{
		Level.Current.OnIntroEvent += OnIntroEnd;
		base.LevelInit(properties);
		attacking = false;
		direction = 1;
		pct = 0f;
		initialHP = properties.CurrentHealth;
		baseSpeed = properties.CurrentState.boss.movementSpeed.min;
		StartCoroutine(intro_cr());
	}

	private IEnumerator intro_cr()
	{
		yield return CupheadTime.WaitForSeconds(this, 1.2f);
		base.animator.SetTrigger("Continue");
		AudioManager.Play("dice_palace_pachinko_intro");
		emitAudioFromObject.Add("dice_palace_pachinko_intro");
		yield return null;
	}

	private void OnIntroEnd()
	{
		StartCoroutine(move_cr());
		StartCoroutine(attack_cr());
		StartCoroutine(check_position_cr());
	}

	protected virtual float hitPauseCoefficient()
	{
		return (!GetComponent<DamageReceiver>().IsHitPaused) ? 1f : 0f;
	}

	private IEnumerator move_cr()
	{
		AudioManager.PlayLoop("dice_palace_pachinko_movement_loop");
		emitAudioFromObject.Add("dice_palace_pachinko_movement_loop");
		while (true)
		{
			float speed = baseSpeed + (base.properties.CurrentState.boss.movementSpeed.max - base.properties.CurrentState.boss.movementSpeed.min) * pct;
			base.transform.position += Vector3.right * speed * direction * CupheadTime.Delta * hitPauseCoefficient();
			yield return null;
		}
	}

	private IEnumerator check_position_cr()
	{
		while (true)
		{
			if ((base.transform.position.x < -640f + base.properties.CurrentState.boss.leftBoundaryOffset && direction == -1) || (base.transform.position.x > 640f - base.properties.CurrentState.boss.rightBoundaryOffset && direction == 1))
			{
				StartCoroutine(reverse_cr());
			}
			yield return null;
		}
	}

	private IEnumerator attack_cr()
	{
		yield return CupheadTime.WaitForSeconds(this, base.properties.CurrentState.boss.initialAttackDelay);
		while (true)
		{
			StartCoroutine(lights_cr());
			base.animator.SetTrigger("OnAttack");
			yield return base.animator.WaitForAnimationToEnd(this, "Attack_Warning_Start");
			BeamWarning();
			yield return CupheadTime.WaitForSeconds(this, base.properties.CurrentState.boss.warningDuration);
			base.animator.SetTrigger("Continue");
			AudioManager.Play("dice_palace_pachinko_warning_trans");
			emitAudioFromObject.Add("dice_palace_pachinko_warning_trans");
			attacking = true;
			BeamOn();
			yield return CupheadTime.WaitForSeconds(this, base.properties.CurrentState.boss.beamDuration);
			BeamOff();
			attacking = false;
			base.animator.SetTrigger("OnEnd");
			AudioManager.Play("dice_palace_pachinko_trans_out");
			emitAudioFromObject.Add("dice_palace_pachinko_trans_out");
			yield return base.animator.WaitForAnimationToEnd(this, "Attack_Trans_Out");
			yield return CupheadTime.WaitForSeconds(this, base.properties.CurrentState.boss.attackDelay.RandomFloat());
		}
	}

	private void BeamWarning()
	{
		beam.SetActive(true);
		beam.GetComponent<Collider2D>().enabled = false;
		beam.GetComponent<SpriteRenderer>().sprite = beamSprites[0];
	}

	private void BeamOn()
	{
		beam.SetActive(true);
		beam.GetComponent<Collider2D>().enabled = true;
		beam.GetComponent<SpriteRenderer>().sprite = beamSprites[1];
	}

	private void BeamOff()
	{
		beam.SetActive(false);
		beam.GetComponent<Collider2D>().enabled = false;
	}

	private IEnumerator lights_cr()
	{
		float fastSpeed = 6f;
		float fadeTime = 0.01f;
		Transform[] array = lights;
		foreach (Transform light in array)
		{
			light.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
			yield return CupheadTime.WaitForSeconds(this, base.properties.CurrentState.boss.warningDuration / (float)lights.Length);
		}
		bool fadingOut = false;
		while (!attacking)
		{
			yield return null;
		}
		fire.speed = fastSpeed;
		while (attacking)
		{
			Transform[] array2 = lights;
			foreach (Transform transform in array2)
			{
				if (fadingOut)
				{
					transform.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0f);
				}
				else
				{
					transform.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
				}
			}
			fadingOut = !fadingOut;
			yield return CupheadTime.WaitForSeconds(this, fadeTime);
			yield return null;
		}
		StartCoroutine(fire_speed_cr(fastSpeed));
		Transform[] array3 = lights;
		foreach (Transform transform2 in array3)
		{
			transform2.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
		}
		Transform[] array4 = lights;
		foreach (Transform light2 in array4)
		{
			light2.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0f);
			yield return CupheadTime.WaitForSeconds(this, base.properties.CurrentState.boss.warningDuration / (float)lights.Length);
		}
		yield return null;
	}

	private IEnumerator fire_speed_cr(float fastSpeed)
	{
		while (fastSpeed > 0f)
		{
			fastSpeed -= 0.1f;
			fire.speed = fastSpeed;
			yield return null;
		}
		yield return null;
	}

	private IEnumerator reverse_cr()
	{
		if (!reversing)
		{
			reversing = true;
			direction *= -1;
			yield return CupheadTime.WaitForSeconds(this, 0.1f);
			reversing = false;
		}
		yield return null;
	}

	private void OnDeath()
	{
		AudioManager.Stop("dice_palace_pachinko_movement_loop");
		AudioManager.Play("dice_palace_pachinko_death");
		StopAllCoroutines();
		GetComponent<Collider2D>().enabled = false;
		base.animator.SetTrigger("OnDeath");
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		damageDealer.DealDamage(hit);
		base.OnCollisionPlayer(hit, phase);
	}

	protected override void OnDestroy()
	{
		StopAllCoroutines();
		base.OnDestroy();
	}
}
