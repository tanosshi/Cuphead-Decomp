using UnityEngine;

public class TrainLevelGhostCannons : LevelProperties.Train.Entity
{
	[SerializeField]
	private Effect cannonSmoke;

	[SerializeField]
	private Transform[] cannonRoots;

	[SerializeField]
	private TrainLevelGhostCannonGhost ghostPrefab;

	private int cannon;

	private bool shooting = true;

	public void Shoot(int cannon)
	{
		if (shooting)
		{
			this.cannon = cannon;
			base.animator.SetInteger("Cannon", cannon);
			base.animator.SetTrigger("OnShoot");
		}
	}

	public void End()
	{
		shooting = false;
	}

	private void ShootAnim()
	{
		if (shooting)
		{
			AudioManager.Play("train_cannon_shoot");
			emitAudioFromObject.Add("train_cannon_shoot");
			cannonSmoke.Create(cannonRoots[cannon].position);
			ghostPrefab.Create(cannonRoots[cannon].position, base.properties.CurrentState.lollipopGhouls.ghostDelay, base.properties.CurrentState.lollipopGhouls.ghostSpeed, base.properties.CurrentState.lollipopGhouls.ghostAimSpeed, base.properties.CurrentState.lollipopGhouls.ghostHealth, base.properties.CurrentState.lollipopGhouls.skullSpeed);
		}
	}
}
