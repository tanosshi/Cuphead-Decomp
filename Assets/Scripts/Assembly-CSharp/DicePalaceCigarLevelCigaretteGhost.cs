using System.Collections;
using UnityEngine;

public class DicePalaceCigarLevelCigaretteGhost : AbstractProjectile
{
	[SerializeField]
	private Transform root;

	[SerializeField]
	private Effect fx;

	private float time;

	private Vector3 centerPoint;

	private LevelProperties.DicePalaceCigar properties;

	public void InitGhost(LevelProperties.DicePalaceCigar properties)
	{
		this.properties = properties;
		time = 0f;
		StartCoroutine(move_cr());
	}

	private IEnumerator move_cr()
	{
		StartCoroutine(spawn_fx_cr());
		YieldInstruction wait = new WaitForFixedUpdate();
		while (base.transform.position.y < 560f)
		{
			base.transform.AddPosition(0f, properties.CurrentState.cigaretteGhost.verticalSpeed * CupheadTime.FixedDelta);
			yield return wait;
		}
		Die();
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		base.damageDealer.DealDamage(hit);
		base.OnCollisionPlayer(hit, phase);
	}

	protected override void OnDestroy()
	{
		StopAllCoroutines();
		base.OnDestroy();
	}

	protected override void Die()
	{
		base.Die();
		StopAllCoroutines();
	}

	private IEnumerator spawn_fx_cr()
	{
		bool isVal1 = Rand.Bool();
		while (true)
		{
			float value1 = Random.Range(0.4f, 0.6f);
			float value2 = Random.Range(0.2f, 0.3f);
			float chosenVal = ((!isVal1) ? value2 : value1);
			yield return CupheadTime.WaitForSeconds(this, chosenVal);
			float t = 0f;
			float time = chosenVal;
			while (t < time)
			{
				t += (float)CupheadTime.Delta;
				fx.Create(root.transform.position);
				yield return CupheadTime.WaitForSeconds(this, 0.1f);
				yield return null;
			}
			yield return CupheadTime.WaitForSeconds(this, Random.Range(0.25f, 0.45f));
			yield return null;
		}
	}
}
