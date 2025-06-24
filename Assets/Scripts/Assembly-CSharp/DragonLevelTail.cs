using System.Collections;
using UnityEngine;

public class DragonLevelTail : LevelProperties.Dragon.Entity
{
	public enum State
	{
		Idle = 0,
		Tail = 1
	}

	public const float OUT_Y = -1210f;

	public const float IN_Y = -465f;

	public const float START_Y = -1045f;

	public const float START_TIME = 0.3f;

	[SerializeField]
	private CollisionChild childCollider;

	private DamageDealer damageDealer;

	public State state { get; private set; }

	protected override void Awake()
	{
		base.Awake();
		RegisterCollisionChild(childCollider);
		base.transform.SetPosition(null, -1210f);
		damageDealer = new DamageDealer(1f, 0.1f, true, false, false);
	}

	public override void LevelInit(LevelProperties.Dragon properties)
	{
		base.LevelInit(properties);
	}

	protected override void Update()
	{
		base.Update();
		if (damageDealer != null)
		{
			damageDealer.Update();
		}
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionPlayer(hit, phase);
		if (damageDealer != null)
		{
			damageDealer.DealDamage(hit);
		}
	}

	public void TailStart(float warningTime, float inTime, float holdTime, float outTime)
	{
		StartCoroutine(go_cr(warningTime, inTime, holdTime, outTime));
	}

	private IEnumerator go_cr(float warningTime, float inTime, float holdTime, float outTime)
	{
		state = State.Tail;
		base.transform.SetPosition(PlayerManager.GetNext().transform.position.x);
		AudioManager.Play("level_dragon_left_dragon_tail_appear");
		emitAudioFromObject.Add("level_dragon_left_dragon_tail_appear");
		yield return TweenPositionY(-1210f, -1045f, 0.3f, EaseUtils.EaseType.easeOutSine);
		yield return CupheadTime.WaitForSeconds(this, warningTime);
		AudioManager.Play("level_dragon_left_dragon_tail_attack");
		emitAudioFromObject.Add("level_dragon_left_dragon_tail_attack");
		yield return TweenPositionY(-1045f, -465f, inTime, EaseUtils.EaseType.easeInSine);
		CupheadLevelCamera.Current.Shake(20f, 0.4f);
		yield return CupheadTime.WaitForSeconds(this, holdTime);
		yield return TweenPositionY(-465f, -1210f, outTime, EaseUtils.EaseType.easeInSine);
		state = State.Idle;
	}
}
