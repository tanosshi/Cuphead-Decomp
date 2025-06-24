using System.Collections;
using UnityEngine;

public class TrainLevelTrain : LevelProperties.Train.Entity
{
	public enum State
	{
		BlindSpecter = 0,
		Skeleton = 1,
		LollipopGhouls = 2,
		Engine = 3
	}

	public const float TRAIN_MOVE_TIME = 2.5f;

	public const float START_X = 455f;

	public const float SKELETON_X = -960f;

	public const float GHOUL_X = -2358f;

	public const float ENGINE_MID_X = -4816f;

	public const float ENGINE_X = -6016f;

	[SerializeField]
	private TrainLevelSkeleton skeleton;

	[SerializeField]
	private TrainLevelPassengerCar[] skeletonCars;

	[Space(10f)]
	[SerializeField]
	private TrainLevelLollipopGhoulsManager ghouls;

	[Space(10f)]
	[SerializeField]
	private TrainLevelEngineCar engineCar;

	[SerializeField]
	private TrainLevelEngineBoss engineBoss;

	public State state { get; private set; }

	protected override void Awake()
	{
		base.Awake();
		base.transform.SetPosition(455f);
	}

	public void OnBlindSpectreDeath()
	{
		StartCoroutine(blindSpectreDeath_cr());
	}

	public void OnSkeletonDeath()
	{
		StartCoroutine(skeletonDeath_cr());
	}

	public void OnLollipopsDeath()
	{
		StartCoroutine(lollipopsDeath_cr());
	}

	private IEnumerator blindSpectreDeath_cr()
	{
		state = State.Skeleton;
		yield return CupheadTime.WaitForSeconds(this, 1f);
		yield return TweenPositionX(base.transform.position.x, -960f, 2.5f, EaseUtils.EaseType.easeInOutSine);
		yield return CupheadTime.WaitForSeconds(this, 1f);
		for (int i = 0; i < skeletonCars.Length; i++)
		{
			int i2 = ((i == 1) ? 1 : 0);
			skeletonCars[i].Explode(i2);
		}
		AudioManager.Play(Sfx.Level_Train_Top_Explode);
		skeleton.StartSkeleton();
	}

	private IEnumerator skeletonDeath_cr()
	{
		state = State.LollipopGhouls;
		ghouls.Setup();
		yield return CupheadTime.WaitForSeconds(this, 1f);
		yield return TweenPositionX(base.transform.position.x, -2358f, 2.5f, EaseUtils.EaseType.easeInOutSine);
		yield return CupheadTime.WaitForSeconds(this, 1f);
		ghouls.StartGhouls();
	}

	private IEnumerator lollipopsDeath_cr()
	{
		yield return CupheadTime.WaitForSeconds(this, 2f);
		yield return TweenPositionX(base.transform.position.x, -4816f, 2.5f, EaseUtils.EaseType.easeInSine);
		engineCar.PlayRage();
		yield return TweenPositionX(base.transform.position.x, -6016f, 2.5f, EaseUtils.EaseType.linear);
		engineCar.End();
		yield return CupheadTime.WaitForSeconds(this, 2f);
		engineBoss.StartBoss();
		base.gameObject.SetActive(false);
	}
}
