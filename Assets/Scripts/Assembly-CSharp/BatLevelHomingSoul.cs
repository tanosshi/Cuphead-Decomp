using System.Collections;
using UnityEngine;

public class BatLevelHomingSoul : AbstractCollidableObject
{
	private LevelProperties.Bat.WolfSoul properties;

	private AbstractPlayerController player;

	private DamageDealer damageDealer;

	private int durationIndex;

	private Transform aim;

	private Transform targetPos;

	private float maxDist = 100f;

	private bool isHoming;

	private string[] durationString;

	public void Init(Vector2 pos, AbstractPlayerController player, LevelProperties.Bat.WolfSoul properties)
	{
		aim = new GameObject("Aim").transform;
		aim.SetParent(base.transform);
		aim.ResetLocalTransforms();
		this.properties = properties;
		this.player = player;
		durationString = properties.floatUpDuration.GetRandom().Split(',');
	}

	protected override void Awake()
	{
		base.Awake();
		GetComponent<Collider2D>().enabled = false;
		isHoming = true;
		damageDealer = DamageDealer.NewEnemy();
	}

	protected override void Update()
	{
		base.Update();
		if (damageDealer != null)
		{
			damageDealer.Update();
		}
		if (!(aim == null) && !(player == null) && isHoming)
		{
			float f = Vector3.Distance(base.transform.position, player.transform.position);
			base.transform.position -= base.transform.right * properties.homingSpeed * CupheadTime.Delta;
			aim.LookAt2D(2f * base.transform.position - player.center);
			base.transform.rotation = Quaternion.Slerp(base.transform.rotation, aim.rotation, properties.homingRotation * (float)CupheadTime.Delta);
			if (Mathf.Abs(f) < maxDist && isHoming)
			{
				StartCoroutine(attack_cr());
				isHoming = false;
			}
		}
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionPlayer(hit, phase);
		damageDealer.DealDamage(hit);
	}

	private IEnumerator attack_cr()
	{
		base.animator.SetTrigger("Warning");
		yield return CupheadTime.WaitForSeconds(this, properties.floatWarningDuration);
		base.animator.SetTrigger("Attack");
		GetComponent<Collider2D>().enabled = true;
		yield return CupheadTime.WaitForSeconds(this, properties.attackDuration);
		base.animator.SetTrigger("End");
		GetComponent<Collider2D>().enabled = false;
		StartCoroutine(float_up_cr());
		yield return null;
	}

	private IEnumerator float_up_cr()
	{
		float t = 0f;
		float duration = 0f;
		float.TryParse(durationString[durationIndex], out duration);
		player = PlayerManager.GetNext();
		while (t < duration)
		{
			base.transform.AddPosition(0f, properties.floatSpeed * (float)CupheadTime.Delta);
			t += (float)CupheadTime.Delta;
			yield return null;
		}
		isHoming = true;
		durationIndex %= durationString.Length;
		yield return null;
	}
}
