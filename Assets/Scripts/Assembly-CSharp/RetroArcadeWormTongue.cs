using System.Collections;
using UnityEngine;

public class RetroArcadeWormTongue : AbstractCollidableObject
{
	private enum Direction
	{
		Clockwise = 0,
		Counterclockwise = 1
	}

	private const float RETRACTED_Y_OFFSET = -250f;

	private const float EXTENDED_Y_OFFSET = 195f;

	private const float EXTEND_MOVE_SPEED = 100f;

	private LevelProperties.RetroArcade.Worm properties;

	[SerializeField]
	private Transform tongueSpinner;

	[SerializeField]
	private RetroArcadeWorm parent;

	private Direction rotationDirection;

	private DamageDealer damageDealer;

	public float TileRotationSpeed { get; private set; }

	protected override void Awake()
	{
		base.Awake();
		damageDealer = DamageDealer.NewEnemy();
		GetComponentInChildren<Collider2D>().enabled = false;
	}

	protected override void Update()
	{
		base.Update();
		if (damageDealer != null)
		{
			damageDealer.Update();
		}
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionPlayer(hit, phase);
		damageDealer.DealDamage(hit);
	}

	public void Init(LevelProperties.RetroArcade.Worm properties)
	{
		this.properties = properties;
	}

	public void Extend()
	{
		StartCoroutine(main_cr());
	}

	public void Retract()
	{
		StopAllCoroutines();
		StartCoroutine(retract_cr());
	}

	private IEnumerator main_cr()
	{
		float extendTime = 4.45f;
		float t = 0f;
		while (t < extendTime)
		{
			base.transform.SetPosition(parent.transform.position.x, parent.transform.position.y + Mathf.Lerp(-250f, 195f, t / extendTime));
			t += CupheadTime.FixedDelta;
			yield return new WaitForFixedUpdate();
		}
		GetComponentInChildren<Collider2D>().enabled = true;
		while (true)
		{
			TransformExtensions.SetEulerAngles(z: tongueSpinner.eulerAngles.z + properties.tongueRotateSpeed * CupheadTime.FixedDelta * (float)((rotationDirection == Direction.Counterclockwise) ? 1 : (-1)), transform: tongueSpinner, x: 0f, y: 0f);
			base.transform.SetPosition(parent.transform.position.x, parent.transform.position.y + 195f);
			yield return new WaitForFixedUpdate();
		}
	}

	private IEnumerator retract_cr()
	{
		float retractTime = 4.45f;
		GetComponentInChildren<Collider2D>().enabled = false;
		float t = 0f;
		while (t < retractTime)
		{
			base.transform.SetPosition(parent.transform.position.x, parent.transform.position.y + Mathf.Lerp(195f, -250f, t / retractTime));
			t += CupheadTime.FixedDelta;
			yield return new WaitForFixedUpdate();
		}
	}
}
