using System.Collections;
using UnityEngine;

public class SallyStagePlayLevelUmbrella : GroundHomingMovement
{
	[SerializeField]
	private Transform shadow;

	private float startPosX;

	private LevelProperties.SallyStagePlay properties;

	private Direction moveDir;

	protected override void Awake()
	{
		base.Awake();
		StartCoroutine(drop_cr());
	}

	public void GetProperties(LevelProperties.SallyStagePlay properties)
	{
		this.properties = properties;
	}

	protected override void FixedUpdate()
	{
		base.FixedUpdate();
		shadow.transform.SetLocalEulerAngles(null, null, 0f - base.transform.localEulerAngles.z);
	}

	private IEnumerator drop_cr()
	{
		AudioManager.PlayLoop("sally_umbrella_fall");
		emitAudioFromObject.Add("sally_umbrella_fall");
		YieldInstruction wait = new WaitForFixedUpdate();
		float speed = 200f;
		float t = 0f;
		float rotateAmount = 12f;
		float rotateT = 0f;
		float time = 0.2f;
		while (base.transform.position.y > (float)Level.Current.Ground + 100f)
		{
			t += CupheadTime.FixedDelta;
			base.transform.AddPosition(0f, (0f - speed) * CupheadTime.FixedDelta);
			rotateT += CupheadTime.FixedDelta;
			float phase = Mathf.Sin(rotateT / time);
			base.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, phase * rotateAmount));
			yield return wait;
		}
		maxSpeed = properties.CurrentState.umbrella.homingMaxSpeed;
		acceleration = properties.CurrentState.umbrella.homingAcceleration;
		bounceRatio = properties.CurrentState.umbrella.homingBounceRatio;
		Object.Destroy(GetComponent<LevelCharacterShadow>());
		AudioManager.Stop("sally_umbrella_fall");
		AudioManager.PlayLoop("sally_umbrella_spin_loop");
		emitAudioFromObject.Add("sally_umbrella_spin_loop");
		base.animator.SetTrigger("Land");
		base.EnableHoming = true;
		enableRadishRot = true;
		StartCoroutine(check_dir_change_cr());
		yield return null;
	}

	private IEnumerator check_dir_change_cr()
	{
		while (true)
		{
			if (base.MoveDirection != moveDir)
			{
				AudioManager.Play("sally_umbrella_change_direction");
				emitAudioFromObject.Add("sally_umbrella_change_direction");
				moveDir = base.MoveDirection;
			}
			yield return null;
		}
	}
}
