using System.Collections;
using UnityEngine;

public class PirateLevelShark : LevelProperties.Pirate.Entity
{
	public enum State
	{
		Init = 0,
		Swim = 1,
		Attack = 2,
		Exit = 3,
		Exit_Shot = 4,
		Complete = 5
	}

	public const float SHOT_DELAY = 1f;

	public const float START_X = -950f;

	[SerializeField]
	private GameObject fin;

	[SerializeField]
	private GameObject shark;

	[SerializeField]
	private GameObject splash;

	private LevelProperties.Pirate.Shark sharkProperties;

	private new Animator animator;

	private DamageDealer damageDealer;

	private IEnumerator shotCoroutine;

	public State state { get; private set; }

	protected override void Awake()
	{
		base.Awake();
		GetComponent<DamageReceiver>().OnDamageTaken += OnDamageTaken;
		animator = GetComponent<Animator>();
		shark.SetActive(false);
		shark.transform.SetLocalPosition(-950f);
		splash.SetActive(false);
	}

	private void OnDamageTaken(DamageDealer.DamageInfo info)
	{
		if (state == State.Exit || state == State.Exit_Shot)
		{
			if (shotCoroutine != null)
			{
				StopCoroutine(shotCoroutine);
			}
			shotCoroutine = shot_cr();
			StartCoroutine(shotCoroutine);
		}
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionPlayer(hit, phase);
		damageDealer.DealDamage(hit);
	}

	public override void LevelInitWithGroup(AbstractLevelPropertyGroup propertyGroup)
	{
		base.LevelInitWithGroup(propertyGroup);
		sharkProperties = propertyGroup as LevelProperties.Pirate.Shark;
		StartCoroutine(shark_cr());
		StartCoroutine(collider_cr());
		state = State.Swim;
		damageDealer = new DamageDealer(1f, 1f);
		damageDealer.SetDirection(DamageDealer.Direction.Right, base.transform);
		Vector3 position = base.transform.position;
		position.x = sharkProperties.x;
		base.transform.position = position;
	}

	private void OnBiteAnimComplete()
	{
		state = State.Exit;
		StartCoroutine(exit_cr());
	}

	private void OnBiteAudio()
	{
	}

	private void OnBiteShake()
	{
		CupheadLevelCamera.Current.Shake(12f, 0.5f);
	}

	private void Splash()
	{
		splash.SetActive(true);
		animator.Play("Splash", 3);
	}

	private void End()
	{
		AudioManager.Stop("level_pirate_shark_exit_normal_loop");
		state = State.Complete;
		StopAllCoroutines();
		Object.Destroy(base.gameObject);
	}

	private IEnumerator shot_cr()
	{
		state = State.Exit_Shot;
		animator.SetLayerWeight(1, 1f);
		float t = 0f;
		while (t < 1f)
		{
			t += (float)CupheadTime.Delta;
			yield return null;
		}
		animator.SetLayerWeight(1, 0f);
		state = State.Exit;
	}

	private IEnumerator shark_cr()
	{
		AudioManager.Play("level_pirate_shark_warning");
		yield return StartCoroutine(fin_cr());
		shark.SetActive(true);
		state = State.Attack;
		animator.Play("Attack");
		AudioManager.Play("levels_pirate_shark_attack");
		emitAudioFromObject.Add("levels_pirate_shark_attack");
	}

	private IEnumerator exit_cr()
	{
		animator.Play("Exit");
		AudioManager.PlayLoop("level_pirate_shark_exit_normal_loop");
		emitAudioFromObject.Add("level_pirate_shark_exit_normal_loop");
		animator.Play("Exit", 1);
		while (true)
		{
			if (shark.transform.position.x < -950f)
			{
				End();
			}
			TransformExtensions.AddPosition(x: 0f - ((state != State.Exit) ? sharkProperties.shotExitSpeed : sharkProperties.exitSpeed) * (float)CupheadTime.Delta, transform: base.transform);
			yield return null;
		}
	}

	private IEnumerator fin_cr()
	{
		float t = 0f;
		float time = sharkProperties.finTime;
		int startX = 640;
		int endX = -740;
		while (t < time)
		{
			float val = t / time;
			TransformExtensions.SetPosition(x: Mathf.Lerp(startX, endX, val), transform: fin.transform);
			t += (float)CupheadTime.Delta;
			yield return null;
		}
		fin.transform.SetPosition(endX);
		yield return CupheadTime.WaitForSeconds(this, sharkProperties.attackDelay);
	}

	private IEnumerator collider_cr()
	{
		BoxCollider2D collider = GetComponent<Collider2D>() as BoxCollider2D;
		BoxCollider2D childCollider = shark.GetComponent<Collider2D>() as BoxCollider2D;
		while (true)
		{
			collider.offset = (Vector2)shark.transform.localPosition + childCollider.offset;
			collider.size = childCollider.size;
			yield return null;
		}
	}
}
