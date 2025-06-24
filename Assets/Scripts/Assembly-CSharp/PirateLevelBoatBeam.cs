using UnityEngine;

public class PirateLevelBoatBeam : ParrySwitch
{
	private DamageDealer damageDealer;

	private new Animator animator;

	protected override void Awake()
	{
		base.Awake();
		damageDealer = new DamageDealer(1f, 0.1f, DamageDealer.DamageSource.Enemy, true, false, false);
		damageDealer.SetDirection(DamageDealer.Direction.Left, base.transform);
		animator = GetComponent<Animator>();
	}

	protected override void Update()
	{
		base.Update();
		damageDealer.Update();
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		if (phase != CollisionPhase.Exit)
		{
			LevelPlayerController component = hit.GetComponent<LevelPlayerController>();
			if (!(component == null) && !component.Ducking)
			{
				damageDealer.DealDamage(hit);
			}
		}
	}

	public PirateLevelBoatBeam Create(Transform parent)
	{
		PirateLevelBoatBeam pirateLevelBoatBeam = InstantiatePrefab<PirateLevelBoatBeam>();
		pirateLevelBoatBeam.Init(parent);
		return pirateLevelBoatBeam;
	}

	private void Init(Transform parent)
	{
		AudioManager.Play("level_pirate_ship_beam_fire");
		base.transform.SetParent(parent);
		base.transform.ResetLocalPosition();
		base.transform.ResetLocalRotation();
	}

	public void StartBeam()
	{
	}

	public void EndBeam()
	{
		animator.SetTrigger("OnEnd");
	}

	public override void OnParryPrePause(AbstractPlayerController player)
	{
		base.OnParryPrePause(player);
		player.stats.ParryOneQuarter();
	}

	private void OnEndAnimComplete()
	{
		Object.Destroy(base.gameObject);
	}
}
