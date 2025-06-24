using System.Collections;
using UnityEngine;

public class TestLevelShootableTimer : AbstractCollidableObject
{
	[SerializeField]
	private float maxTime = 3f;

	[SerializeField]
	private DamageReceiver child;

	private DamageReceiver damageReceiver;

	private float damageTaken;

	private bool timerStarted;

	protected override void Start()
	{
		base.Start();
		damageReceiver = GetComponent<DamageReceiver>();
		damageReceiver.OnDamageTaken += OnDamageTaken;
		child.OnDamageTaken += OnDamageTaken;
		StartCoroutine(timer_cr());
	}

	protected override void Update()
	{
		base.Update();
		if (Input.GetKeyDown(KeyCode.T))
		{
			timerStarted = true;
		}
		Debug.Log("Damage Taken: " + damageTaken);
	}

	private void OnDamageTaken(DamageDealer.DamageInfo info)
	{
		if (timerStarted)
		{
			damageTaken += info.damage;
		}
	}

	private IEnumerator timer_cr()
	{
		while (true)
		{
			float t = 0f;
			if (timerStarted)
			{
				Debug.Log("TIMER ON");
				while (t < maxTime)
				{
					t += (float)CupheadTime.Delta;
					yield return null;
				}
				yield return null;
				Debug.Log("TIMES UP");
				damageTaken = 0f;
				timerStarted = false;
			}
			yield return null;
		}
	}
}
