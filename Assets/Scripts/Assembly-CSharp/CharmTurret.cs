using System.Collections;
using UnityEngine;

public class CharmTurret : AbstractCollidableObject
{
	[SerializeField]
	private BasicProjectile projectile;

	private float circleSpeed;

	private float projectileSpeed;

	private float delay;

	private float angle;

	private float loopSize = 200f;

	private GameObject rootObject;

	public void Init(GameObject rootObject, float circleSpeed, float projectileSpeed, float delay)
	{
		base.transform.position = rootObject.transform.position;
		this.rootObject = rootObject;
		this.circleSpeed = circleSpeed;
		this.projectileSpeed = projectileSpeed;
		this.delay = delay;
		StartCoroutine(move_cr());
		StartCoroutine(shoot_cr());
	}

	protected override void Start()
	{
		base.Start();
	}

	private IEnumerator move_cr()
	{
		while (true)
		{
			angle += circleSpeed * CupheadTime.FixedDelta;
			Vector3 handleRotationX = new Vector3((0f - Mathf.Sin(angle)) * loopSize, 0f, 0f);
			Vector3 handleRotationY = new Vector3(0f, Mathf.Cos(angle) * loopSize, 0f);
			base.transform.position = rootObject.transform.position;
			base.transform.position += handleRotationX + handleRotationY;
			yield return null;
		}
	}

	private IEnumerator shoot_cr()
	{
		while (true)
		{
			yield return CupheadTime.WaitForSeconds(this, delay);
			projectile.Create(base.transform.position, 0f, projectileSpeed);
			yield return null;
		}
	}
}
