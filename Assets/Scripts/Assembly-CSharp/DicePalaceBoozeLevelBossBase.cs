using System.Collections;
using UnityEngine;

public class DicePalaceBoozeLevelBossBase : LevelProperties.DicePalaceBooze.Entity
{
	protected float health;

	public bool isDead { get; private set; }

	public static int DEATH_COUNTER { get; private set; }

	public static float ATTACK_DELAY { get; private set; }

	protected override void Start()
	{
		base.Start();
		isDead = false;
		DEATH_COUNTER = 0;
		ATTACK_DELAY = 0f;
	}

	public override void LevelInit(LevelProperties.DicePalaceBooze properties)
	{
		base.LevelInit(properties);
	}

	protected virtual void StartDying()
	{
		isDead = true;
		DEATH_COUNTER++;
		if (DEATH_COUNTER >= 3)
		{
			AllDead();
			return;
		}
		ATTACK_DELAY += base.properties.CurrentState.main.delaySubstractAmount;
		Dying();
	}

	private void Dying()
	{
		StopAllCoroutines();
		StartCoroutine(dying_cr());
	}

	private IEnumerator dying_cr()
	{
		base.animator.SetTrigger("OnDeath");
		GetComponent<DamageReceiver>().enabled = false;
		Object.Destroy(GetComponent<Rigidbody2D>());
		GetComponent<LevelBossDeathExploder>().StartExplosion();
		yield return CupheadTime.WaitForSeconds(this, 1.5f);
		GetComponent<LevelBossDeathExploder>().StopExplosions();
		yield return null;
	}

	private void AllDead()
	{
		StopAllCoroutines();
		base.animator.SetTrigger("OnDeath");
		base.properties.DealDamageToNextNamedState();
	}

	protected virtual void HandleDead()
	{
		GetComponent<Collider2D>().enabled = false;
	}
}
