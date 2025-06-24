using System.Collections;
using UnityEngine;

public class DicePalaceDominoLevelDominoSwing : AbstractCollidableObject
{
	[SerializeField]
	private DicePalaceDominoLevelDomino domino;

	private float speed;

	private float strength;

	private Vector3 origin;

	private LevelProperties.DicePalaceDomino properties;

	protected override void Awake()
	{
		base.Awake();
	}

	public void InitSwing(LevelProperties.DicePalaceDomino properties)
	{
		this.properties = properties;
		speed = properties.CurrentState.domino.swingSpeed;
		strength = properties.CurrentState.domino.swingDistance;
		origin = new Vector3(base.transform.position.x, properties.CurrentState.domino.swingPosY);
		base.transform.position = origin;
		StartCoroutine(move_cr());
	}

	protected virtual float hitPauseCoefficient()
	{
		return (!GetComponentInChildren<DamageReceiver>().IsHitPaused) ? 1f : 0f;
	}

	private IEnumerator move_cr()
	{
		yield return domino.GetComponent<Animator>().WaitForAnimationToEnd(this, "Intro");
		float angle = 0f;
		while (true)
		{
			angle += speed * (float)CupheadTime.Delta * hitPauseCoefficient();
			base.transform.position = origin + Vector3.up * (Mathf.Sin(angle) * strength);
			yield return null;
		}
	}
}
