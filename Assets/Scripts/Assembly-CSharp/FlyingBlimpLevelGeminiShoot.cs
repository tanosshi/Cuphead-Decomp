using System.Collections;
using UnityEngine;

public class FlyingBlimpLevelGeminiShoot : AbstractCollidableObject
{
	private LevelProperties.FlyingBlimp.Gemini properties;

	[SerializeField]
	private GameObject smallFX;

	[SerializeField]
	private Transform projectileRootUp;

	[SerializeField]
	private Transform projectileRootDown;

	[SerializeField]
	private BasicProjectile projectilePrefab;

	private float smallRadius;

	private Transform projectileRoot;

	private Vector3 target;

	private Quaternion startRotation;

	private float speed;

	private float rotationTime;

	private float delayTime;

	private bool pointingUp;

	private bool smallFXSpawning;

	private bool halfWay;

	public void Init(LevelProperties.FlyingBlimp.Gemini properties, Vector2 pos, float speed)
	{
		this.properties = properties;
		base.transform.position = pos;
		this.speed = speed;
		smallRadius = GetComponent<CircleCollider2D>().radius;
		float num = Random.Range(0, 2);
		pointingUp = num == 0f;
		if (pointingUp)
		{
			projectileRoot = projectileRootUp;
		}
		else
		{
			projectileRoot = projectileRootDown;
		}
		StartCoroutine(rotate_cr());
	}

	protected override void Update()
	{
		base.Update();
	}

	private IEnumerator rotate_cr()
	{
		YieldInstruction wait = new WaitForFixedUpdate();
		AudioManager.Play("level_flying_blimp_wheel_start");
		yield return CupheadTime.WaitForSeconds(this, 1f);
		base.animator.SetBool("Attack", true);
		smallFXSpawning = true;
		StartCoroutine(spawn_small_fx_cr());
		AudioManager.PlayLoop("level_flying_blimp_gemini_sphere_attack");
		float pct = 0f;
		float startRotation = ((!Rand.Bool()) ? (-360) : 360);
		while (pct <= 1f)
		{
			base.transform.SetEulerAngles(null, null, startRotation * pct);
			pct += CupheadTime.FixedDelta * properties.rotationSpeed;
			ShootBullet();
			yield return wait;
		}
		base.transform.SetEulerAngles(null, null, (startRotation != 360f) ? 360 : (-360));
		smallFXSpawning = false;
		base.animator.SetBool("Attack", false);
		base.animator.SetTrigger("Leave");
		AudioManager.Stop("level_flying_blimp_gemini_sphere_attack");
		AudioManager.Play("level_flying_blimp_wheel_end");
	}

	private void ShootBullet()
	{
		float x = projectileRoot.position.x - base.transform.position.x;
		float y = projectileRoot.position.y - base.transform.position.y;
		float rotation = Mathf.Atan2(y, x) * 57.29578f;
		if (delayTime < properties.bulletDelay)
		{
			delayTime += 1f;
			return;
		}
		projectilePrefab.Create(projectileRoot.position, rotation, properties.bulletSpeed);
		delayTime = 0f;
	}

	private IEnumerator spawn_small_fx_cr()
	{
		while (smallFXSpawning)
		{
			GameObject small = Object.Instantiate(smallFX);
			Vector3 scale = new Vector3(1f, 1f, 1f);
			scale.x = ((!Rand.Bool()) ? (0f - scale.x) : scale.x);
			scale.y = ((!Rand.Bool()) ? (0f - scale.y) : scale.y);
			small.transform.SetScale(scale.x, scale.y, 1f);
			small.transform.eulerAngles = new Vector3(0f, 0f, Random.Range(0f, 360f));
			small.GetComponent<SpriteRenderer>().sortingOrder = Random.Range(0, 3);
			small.transform.position = GetRandomPoint();
			StartCoroutine(delete_small_fx(small));
			yield return CupheadTime.WaitForSeconds(this, 0.1f);
		}
	}

	private Vector2 GetRandomPoint()
	{
		Vector2 vector = base.transform.position;
		Vector2 vector2 = new Vector2(Random.Range(-1, 1), Random.Range(-1, 1)).normalized * (smallRadius * Random.value) * 2f;
		return vector + vector2;
	}

	private IEnumerator delete_small_fx(GameObject smallFX)
	{
		yield return smallFX.GetComponent<Animator>().WaitForAnimationToEnd(this, "SmallFX");
		Object.Destroy(smallFX);
	}

	private void Die()
	{
		AudioManager.Play("level_flying_blimp_gemini_sphere_leave");
		GetComponent<Collider2D>().enabled = false;
		StopAllCoroutines();
		Object.Destroy(base.gameObject);
	}
}
