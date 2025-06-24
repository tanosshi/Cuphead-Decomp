using UnityEngine;

public class DicePalaceRouletteLevelMarble : BasicProjectile
{
	private const string FallState = "Fall";

	private const string GroundParameterName = "Ground";

	private const string VariationParameterName = "Variation";

	private const int VariationCount = 3;

	protected override void Start()
	{
		base.Start();
		animator.Play("Fall", 0, Random.value);
	}

	protected override void OnCollisionGround(GameObject hit, CollisionPhase phase)
	{
		if (phase == CollisionPhase.Enter)
		{
			animator.SetFloat("Variation", (float)Random.Range(0, 3) / 2f);
			animator.SetTrigger("Ground");
			Speed = 0f;
		}
	}

	public void OnAnimEnd()
	{
		AudioManager.Play("dice_palace_roulette_balls_splat");
		emitAudioFromObject.Add("dice_palace_roulette_balls_splat");
		Object.Destroy(base.gameObject);
	}
}
