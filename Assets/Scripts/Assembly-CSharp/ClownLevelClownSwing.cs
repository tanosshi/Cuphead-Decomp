using System;
using System.Collections;
using UnityEngine;

public class ClownLevelClownSwing : LevelProperties.Clown.Entity
{
	public enum State
	{
		Intro = 0,
		Idle = 1,
		Enemies = 2,
		Death = 3
	}

	public const int NumOfSwings = 6;

	[SerializeField]
	private ClownLevelCoasterHandler coasterHandler;

	[SerializeField]
	private GameObject umbrella;

	[SerializeField]
	private GameObject topper;

	[SerializeField]
	private ClownLevelEnemy enemy;

	[SerializeField]
	private ClownLevelSwings swingFrontPrefab;

	[SerializeField]
	private ClownLevelSwings swingBackPrefab;

	[SerializeField]
	private BasicProjectile clownBullet;

	[SerializeField]
	private Transform swingStopPosition;

	private DamageReceiver damageReceiver;

	private bool moveUp;

	private int eyeMainIndex;

	public Action OnDeath;

	public State state { get; private set; }

	protected override void Awake()
	{
		base.Awake();
		damageReceiver = GetComponent<DamageReceiver>();
		damageReceiver.OnDamageTaken += OnDamageTaken;
		state = State.Intro;
	}

	private void OnDamageTaken(DamageDealer.DamageInfo info)
	{
		base.properties.DealDamage(info.damage);
		if (base.properties.CurrentHealth <= 0f && state != State.Death)
		{
			state = State.Death;
			StartDeath();
		}
	}

	public override void LevelInit(LevelProperties.Clown properties)
	{
		base.LevelInit(properties);
		eyeMainIndex = UnityEngine.Random.Range(0, properties.CurrentState.swing.positionString.Length);
	}

	public void StartSwing()
	{
		AudioManager.Play("clown_swing_face_intro");
		emitAudioFromObject.Add("clown_swing_face_intro");
		StartCoroutine(swing_intro_cr());
	}

	private IEnumerator swing_intro_cr()
	{
		float t = 0f;
		float time = 5f;
		Vector2 start = base.transform.position;
		while (t < time)
		{
			float val = EaseUtils.Ease(EaseUtils.EaseType.easeInOutSine, 0f, 1f, t / time);
			base.transform.position = Vector2.Lerp(start, swingStopPosition.position, val);
			t += (float)CupheadTime.Delta;
			yield return null;
		}
		base.transform.position = swingStopPosition.position;
		t = 0f;
		time = 0.5f;
		start = umbrella.transform.position;
		Vector2 end = new Vector3(umbrella.transform.position.x, umbrella.transform.position.y - 30f);
		while (t < time)
		{
			float val2 = EaseUtils.Ease(EaseUtils.EaseType.easeInOutSine, 0f, 1f, t / time);
			umbrella.transform.position = Vector2.Lerp(start, end, val2);
			t += (float)CupheadTime.Delta;
			yield return null;
		}
		umbrella.transform.position = start;
		umbrella.GetComponent<SpriteRenderer>().sortingLayerName = "Player";
		umbrella.GetComponent<SpriteRenderer>().sortingOrder = 200;
		state = State.Idle;
		StartCoroutine(swing_cr());
		coasterHandler.OnCoasterLeave += StartEnemies;
		yield return null;
	}

	private IEnumerator swing_cr()
	{
		LevelProperties.Clown.Swing p = base.properties.CurrentState.swing;
		float spacingFront = swingFrontPrefab.GetComponent<Renderer>().bounds.size.x + p.swingSpacing;
		float spacingBack = swingBackPrefab.GetComponent<Renderer>().bounds.size.x + p.swingSpacing;
		int numOfSwings = 6;
		AudioManager.Play("clown_swing_open");
		emitAudioFromObject.Add("clown_swing_open");
		base.animator.SetTrigger("Continue");
		for (int i = 0; i < numOfSwings; i++)
		{
			Vector3 pos = new Vector3(-640f - spacingFront + spacingFront * (float)i, 360f, 0f);
			ClownLevelSwings clownLevelSwings = UnityEngine.Object.Instantiate(swingFrontPrefab);
			clownLevelSwings.Init(pos, base.properties.CurrentState.swing, spacingFront, i);
		}
		for (int j = 0; j < numOfSwings; j++)
		{
			Vector3 pos2 = new Vector3(640f + spacingBack - spacingBack * (float)j, 360f, 0f);
			ClownLevelSwings clownLevelSwings2 = UnityEngine.Object.Instantiate(swingBackPrefab);
			clownLevelSwings2.Init(pos2, base.properties.CurrentState.swing, spacingBack, j);
		}
		yield return null;
	}

	private void StartBottom()
	{
		base.animator.Play("Swing_Bottom_Idle");
	}

	private void StartDeath()
	{
		if (OnDeath != null)
		{
			OnDeath();
		}
		StopAllCoroutines();
		AudioManager.Play("clown_swing_death");
		emitAudioFromObject.Add("clown_swing_death");
		base.animator.Play("Swing_Death");
		base.animator.Play("Swing_Bottom_Death");
		base.animator.Play("Face_Death");
		ClownLevelSwings.moveSpeed = base.properties.CurrentState.swing.swingSpeed * 2f;
		GetComponent<Collider2D>().enabled = false;
	}

	private void SetMoveDirection(int set)
	{
		if (set == 1)
		{
			moveUp = true;
		}
		else
		{
			moveUp = false;
		}
	}

	private IEnumerator move_topper_cr()
	{
		float speed = 60f;
		while (true)
		{
			if (moveUp)
			{
				topper.transform.position += Vector3.up * speed * CupheadTime.Delta;
			}
			else
			{
				topper.transform.position -= Vector3.up * speed * CupheadTime.Delta;
			}
			yield return null;
		}
	}

	private void StartEnemies()
	{
		base.animator.SetBool("IsAttacking", true);
		StartCoroutine(enemies_cr());
	}

	private IEnumerator enemies_cr()
	{
		LevelProperties.Clown.Swing p = base.properties.CurrentState.swing;
		string[] enemyPosString = p.positionString[eyeMainIndex].Split(',');
		Vector3 endPos = new Vector3(base.transform.position.x, Level.Current.Ground);
		state = State.Enemies;
		AudioManager.Play("clown_swing_face_attack_intro");
		emitAudioFromObject.Add("clown_swing_face_attack_intro");
		yield return base.animator.WaitForAnimationToEnd(this, "Face_Attack_Intro", 1);
		for (int i = 0; i < enemyPosString.Length; i++)
		{
			string[] enemyPos = enemyPosString[i].Split('-');
			string[] array = enemyPos;
			foreach (string pos in array)
			{
				float targetX = 0f;
				float.TryParse(pos, out targetX);
				enemy.Create(base.transform.position, targetX, p.HP, p, this);
				yield return CupheadTime.WaitForSeconds(this, p.spawnDelay);
			}
		}
		eyeMainIndex = (eyeMainIndex + 1) % p.positionString.Length;
		base.animator.SetBool("IsAttacking", false);
		AudioManager.Play("clown_swing_face_attack_outro");
		emitAudioFromObject.Add("clown_swing_face_attack_outro");
		yield return null;
	}
}
