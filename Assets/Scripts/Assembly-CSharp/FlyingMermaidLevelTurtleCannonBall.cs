using System.Collections;
using UnityEngine;

public class FlyingMermaidLevelTurtleCannonBall : AbstractProjectile
{
	[SerializeField]
	private FlyingMermaidLevelTurtleSpiralProjectile spreadshotPrefab;

	[SerializeField]
	private Effect explodeEffectPrefab;

	private string explodePattern;

	private LevelProperties.FlyingMermaid.Turtle properties;

	public FlyingMermaidLevelTurtleCannonBall Create(Vector2 pos, string explodePattern, LevelProperties.FlyingMermaid.Turtle properties)
	{
		FlyingMermaidLevelTurtleCannonBall flyingMermaidLevelTurtleCannonBall = base.Create() as FlyingMermaidLevelTurtleCannonBall;
		flyingMermaidLevelTurtleCannonBall.properties = properties;
		flyingMermaidLevelTurtleCannonBall.explodePattern = explodePattern;
		flyingMermaidLevelTurtleCannonBall.transform.position = pos;
		return flyingMermaidLevelTurtleCannonBall;
	}

	protected override void Start()
	{
		base.Start();
		StartCoroutine(loop_cr());
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionPlayer(hit, phase);
		base.damageDealer.DealDamage(hit);
	}

	private IEnumerator loop_cr()
	{
		AudioManager.Play("level_mermaid_turtle_cannon");
		float t = 0f;
		float bulletTime = properties.bulletTimeToExplode.RandomFloat();
		float targetDistance = properties.bulletSpeed * bulletTime;
		float apex = targetDistance + 50f;
		float launchSpeed = Mathf.Sqrt(4000f * apex);
		float timeToApex = launchSpeed / 2000f;
		float launchY = base.transform.position.y;
		while (t < timeToApex || base.transform.position.y > targetDistance + launchY)
		{
			t += CupheadTime.FixedDelta;
			float y = launchY + launchSpeed * t - 1000f * t * t;
			base.transform.SetPosition(null, y);
			yield return new WaitForFixedUpdate();
		}
		string[] array = explodePattern.Split('-');
		foreach (string s in array)
		{
			float result = 0f;
			float.TryParse(s, out result);
			spreadshotPrefab.Create(base.transform.position, result, properties.spreadshotBulletSpeed, properties.spiralRate);
		}
		explodeEffectPrefab.Create(base.transform.position);
		Die();
	}

	protected override void Die()
	{
		base.Die();
		Object.Destroy(base.gameObject);
	}
}
