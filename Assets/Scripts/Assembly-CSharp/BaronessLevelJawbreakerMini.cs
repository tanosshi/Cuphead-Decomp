using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaronessLevelJawbreakerMini : BaronessLevelMiniBossBase
{
	public enum State
	{
		Unspawned = 0,
		Spawned = 1,
		Dying = 2
	}

	private const float POSITION_FRAME_TIME = 1f / 12f;

	[SerializeField]
	private Transform sprite;

	private float speed;

	private float rotationSpeed;

	private float pathLength;

	private int layerNum = 3;

	private int positionsInList = 12;

	private int index;

	private bool rotateDeath;

	private bool lookingLeft = true;

	private bool isTurning;

	private DamageDealer damageDealer;

	private LevelProperties.Baroness.Jawbreaker properties;

	private Vector3 currentPos;

	private Transform targetPosition;

	private Transform aim;

	private List<Vector3> bigPath;

	private Quaternion rotate;

	public State state { get; private set; }

	protected override void Awake()
	{
		base.Awake();
		damageDealer = DamageDealer.NewEnemy();
		aim = base.transform;
		rotateDeath = false;
		bigPath = new List<Vector3>();
	}

	protected override void Start()
	{
		base.Start();
		layerSwitch = 3;
		fadeTime = 2f;
		sprite.GetComponent<SpriteRenderer>().sortingLayerName = SpriteLayer.Background.ToString();
		sprite.GetComponent<SpriteRenderer>().sortingOrder = 150;
		StartCoroutine(check_rotation_cr());
		StartCoroutine(switch_cr());
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionPlayer(hit, phase);
		damageDealer.DealDamage(hit);
	}

	public void Init(LevelProperties.Baroness.Jawbreaker properties, Vector2 pos, Transform targetPos, float speed, float rotationSpeed)
	{
		this.properties = properties;
		this.speed = speed;
		this.rotationSpeed = rotationSpeed;
		base.transform.position = pos;
		targetPosition = targetPos;
		state = State.Spawned;
		StartCoroutine(blink_cr());
		StartCoroutine(calculate_path_cr());
		StartCoroutine(move_cr());
	}

	protected override void Update()
	{
		base.Update();
		if (damageDealer != null)
		{
			damageDealer.Update();
		}
	}

	protected override void FixedUpdate()
	{
		base.FixedUpdate();
		if (state != State.Spawned)
		{
			return;
		}
		if (bigPath.Count != 0)
		{
			float num = pathLength / properties.jawbreakerMiniSpace;
			base.transform.position -= base.transform.right * (num * properties.jawbreakerHomingSpeed) * CupheadTime.FixedDelta;
			pathLength = 0f;
			aim.LookAt2D(2f * base.transform.position - bigPath[0]);
			base.transform.rotation = Quaternion.Slerp(base.transform.rotation, aim.rotation, rotationSpeed * CupheadTime.FixedDelta);
			sprite.transform.SetEulerAngles(0f, 0f, 0f);
			float num2 = Vector3.Distance(base.transform.position, bigPath[0]);
			if (num2 < properties.jawbreakerHomingSpeed / 4f)
			{
				bigPath.Remove(bigPath[0]);
			}
		}
		if (state == State.Dying && rotateDeath)
		{
			RotateExplode();
			rotateDeath = false;
		}
	}

	private IEnumerator switch_cr()
	{
		StartCoroutine(fade_color_cr());
		yield return CupheadTime.WaitForSeconds(this, 3f);
		sprite.GetComponent<SpriteRenderer>().sortingLayerName = SpriteLayer.Enemies.ToString();
		sprite.GetComponent<SpriteRenderer>().sortingOrder = 250;
	}

	private void Turn()
	{
		sprite.transform.SetScale(0f - sprite.transform.localScale.x, 1f, 1f);
	}

	private IEnumerator check_rotation_cr()
	{
		while (true)
		{
			if (((targetPosition.transform.position.x < base.transform.position.x && !lookingLeft) || (targetPosition.transform.position.x > base.transform.position.x && lookingLeft)) && !isTurning)
			{
				isTurning = true;
				base.animator.SetTrigger("Turn");
				yield return base.animator.WaitForAnimationToEnd(this, "Turn");
				lookingLeft = !lookingLeft;
				isTurning = false;
			}
			yield return null;
		}
	}

	private IEnumerator move_cr()
	{
		while (true)
		{
			pathLength = 0f;
			for (int i = 0; i < bigPath.Count - 1; i++)
			{
				pathLength += Vector3.Distance(bigPath[i], bigPath[i + 1]);
				if ((float)CupheadTime.Delta == 0f)
				{
					yield return null;
				}
			}
			yield return null;
		}
	}

	private IEnumerator calculate_path_cr()
	{
		float speed = properties.jawbreakerHomingSpeed;
		float space = properties.jawbreakerMiniSpace;
		float spawnTime = space / speed;
		while (true)
		{
			for (int i = 0; i < positionsInList; i++)
			{
				if ((float)CupheadTime.Delta != 0f)
				{
					bigPath.Add(targetPosition.transform.position);
				}
				yield return null;
				yield return null;
			}
		}
	}

	public void Stop()
	{
		state = State.Dying;
		StopCoroutine(blink_cr());
		StopCoroutine(calculate_path_cr());
	}

	public void StartDying()
	{
		StartCoroutine(dying_cr());
	}

	private void RotateExplode()
	{
		float z = Random.Range(0, 360);
		base.transform.rotation = Quaternion.Euler(0f, 0f, z);
	}

	private IEnumerator dying_cr()
	{
		Collider2D collider = GetComponent<Collider2D>();
		collider.enabled = false;
		yield return CupheadTime.WaitForSeconds(this, 1f);
		base.animator.SetTrigger("Dead");
		yield return CupheadTime.WaitForSeconds(this, 0.5f);
		rotateDeath = true;
		yield return base.animator.WaitForAnimationToEnd(this, "Death_End");
		Die();
	}

	private IEnumerator blink_cr()
	{
		while (state == State.Spawned)
		{
			base.animator.SetTrigger("Blink");
			int timeBetweenNext = Random.Range(2, 4);
			yield return CupheadTime.WaitForSeconds(this, timeBetweenNext);
		}
	}

	private new void Die()
	{
		state = State.Unspawned;
		Collider2D component = GetComponent<Collider2D>();
		component.enabled = false;
		StopAllCoroutines();
		Object.Destroy(base.gameObject);
	}
}
