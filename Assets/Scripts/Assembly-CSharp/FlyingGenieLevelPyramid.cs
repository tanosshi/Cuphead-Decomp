using System.Collections;
using UnityEngine;

public class FlyingGenieLevelPyramid : AbstractCollidableObject
{
	public int number;

	public bool finishedATK;

	[SerializeField]
	private GameObject[] beams;

	private LevelProperties.FlyingGenie.Pyramids properties;

	private DamageDealer damageDealer;

	private DamageReceiver damageReceiver;

	private Transform pivotPoint;

	private float speed;

	private float angle;

	private bool isClockwise;

	protected override void Awake()
	{
		base.Awake();
		damageDealer = DamageDealer.NewEnemy();
		GameObject[] array = beams;
		foreach (GameObject gameObject in array)
		{
			gameObject.GetComponent<CollisionChild>().OnPlayerCollision += OnCollisionPlayer;
		}
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionPlayer(hit, phase);
		damageDealer.DealDamage(hit);
	}

	protected override void Update()
	{
		base.Update();
		if (damageDealer != null)
		{
			damageDealer.Update();
		}
	}

	public void Init(LevelProperties.FlyingGenie.Pyramids properties, Vector2 startPos, float startAngle, float speed, Transform pivot, int number, bool isClockWise)
	{
		base.transform.position = startPos;
		angle = startAngle;
		this.speed = speed;
		pivotPoint = pivot;
		this.number = number;
		this.properties = properties;
		isClockwise = isClockWise;
		StartCoroutine(path_cr());
	}

	public IEnumerator beam_cr()
	{
		finishedATK = false;
		AudioManager.Play("genie_pyramid_attack");
		emitAudioFromObject.Add("genie_pyramid_attack");
		base.animator.SetTrigger("OnStartAttack");
		yield return base.animator.WaitForAnimationToEnd(this, "Open_Start");
		yield return CupheadTime.WaitForSeconds(this, properties.warningDuration);
		base.animator.SetTrigger("OnShoot");
		yield return base.animator.WaitForAnimationToEnd(this, "Shoot_Start");
		GameObject[] array = beams;
		foreach (GameObject gameObject in array)
		{
			gameObject.GetComponent<Animator>().SetBool("IsAttacking", true);
		}
		yield return CupheadTime.WaitForSeconds(this, properties.beamDuration);
		base.animator.SetTrigger("OnEnd");
		GameObject[] array2 = beams;
		foreach (GameObject gameObject2 in array2)
		{
			gameObject2.GetComponent<Animator>().SetBool("IsAttacking", false);
		}
		finishedATK = true;
	}

	private IEnumerator path_cr()
	{
		while (true)
		{
			PathMovement();
			yield return null;
		}
	}

	private void PathMovement()
	{
		angle += speed * (float)CupheadTime.Delta;
		Vector3 vector = ((!isClockwise) ? new Vector3((0f - Mathf.Sin(angle)) * properties.pyramidLoopSize, 0f, 0f) : new Vector3(Mathf.Sin(angle) * properties.pyramidLoopSize, 0f, 0f));
		Vector3 vector2 = new Vector3(0f, Mathf.Cos(angle) * properties.pyramidLoopSize, 0f);
		base.transform.position = pivotPoint.position;
		base.transform.position += vector + vector2;
	}
}
