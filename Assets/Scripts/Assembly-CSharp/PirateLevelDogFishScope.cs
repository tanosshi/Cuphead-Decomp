using UnityEngine;

public class PirateLevelDogFishScope : AbstractMonoBehaviour
{
	private new Animator animator;

	protected override void Awake()
	{
		base.Awake();
		animator = GetComponent<Animator>();
	}

	public void In()
	{
		animator.Play("In");
	}

	private void SoundDogfishPeriStart()
	{
		AudioManager.Play("level_pirate_periscope_warning");
	}
}
