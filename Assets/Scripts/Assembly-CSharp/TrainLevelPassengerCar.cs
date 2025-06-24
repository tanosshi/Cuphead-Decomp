using UnityEngine;

public class TrainLevelPassengerCar : AbstractTrainLevelTrainCar
{
	[SerializeField]
	private Effect[] explosionEffects;

	public void Explode(int i)
	{
		base.animator.SetInteger("State", i);
		base.animator.SetTrigger("OnDamaged");
		switch (i)
		{
		case 0:
			explosionEffects[0].Create(base.transform.position);
			break;
		case 1:
			explosionEffects[1].Create(base.transform.position);
			break;
		}
	}
}
