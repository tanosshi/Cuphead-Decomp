using UnityEngine;

public class TrainLevelEngineCar : AbstractPausableComponent
{
	[SerializeField]
	private Transform steamRoot;

	[SerializeField]
	private Effect steamEffect;

	public void PlayRage()
	{
		AudioManager.Play("train_engine_car_rage_loop");
		emitAudioFromObject.Add("train_engine_car_rage_loop");
		base.animator.Play("Rage");
	}

	public void End()
	{
		base.animator.Play("Idle");
	}

	private void SteamEffect()
	{
		steamEffect.Create(steamRoot.position);
	}
}
