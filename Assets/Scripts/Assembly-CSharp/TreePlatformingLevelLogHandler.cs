using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreePlatformingLevelLogHandler : AbstractPausableComponent
{
	private enum LogTypes
	{
		A = 0,
		B = 1,
		C = 2,
		D = 3,
		E = 4,
		F = 5
	}

	[SerializeField]
	private float maxHP;

	private float HP;

	private float amountToKillLog;

	[SerializeField]
	private bool facingRight;

	[Header("SET UP LOGS HERE:")]
	[SerializeField]
	private LogTypes[] logOrder;

	[Header("DON'T TOUCH THIS:")]
	[SerializeField]
	private TreePlatformingLevelLog[] logPrefabs;

	[SerializeField]
	private Effect effect;

	private List<TreePlatformingLevelLog> logs;

	private Vector3 dropPosition;

	private float offset = 50f;

	private int shootableLogs;

	private int logsKilled;

	private List<bool> checkedLogs;

	private DamageDealer damageDealer;

	private DamageReceiver damageReceiver;

	protected override void Start()
	{
		dropPosition = base.transform.position;
		SetupLogs();
		HP = maxHP;
		damageReceiver = GetComponent<DamageReceiver>();
		damageReceiver.OnDamageTaken += OnDamageTaken;
		base.Start();
	}

	private void SetupLogs()
	{
		int num = 0;
		logs = new List<TreePlatformingLevelLog>();
		TreePlatformingLevelLog treePlatformingLevelLog = null;
		checkedLogs = new List<bool>(logOrder.Length);
		GetComponent<HitFlash>().otherRenderers = new SpriteRenderer[logOrder.Length];
		for (int i = 0; i < logOrder.Length; i++)
		{
			treePlatformingLevelLog = Object.Instantiate(logPrefabs[(int)logOrder[i]]);
			treePlatformingLevelLog.transform.position = new Vector3(base.transform.position.x, (float)Level.Current.Ceiling + 300f);
			treePlatformingLevelLog.transform.parent = base.transform;
			treePlatformingLevelLog.animator.SetBool("hasLegs", i == 0);
			treePlatformingLevelLog.GetComponent<SpriteRenderer>().sortingOrder = num++;
			treePlatformingLevelLog.GetComponent<DamageReceiver>().enabled = false;
			treePlatformingLevelLog.SetDirection(facingRight);
			logs.Add(treePlatformingLevelLog);
			checkedLogs.Add(false);
			GetComponent<HitFlash>().otherRenderers[i] = treePlatformingLevelLog.GetComponent<SpriteRenderer>();
			if (treePlatformingLevelLog.CanShoot)
			{
				shootableLogs++;
			}
		}
		amountToKillLog = maxHP / (float)logs.Count;
		StartCoroutine(check_to_start_cr());
	}

	private void OnDamageTaken(DamageDealer.DamageInfo info)
	{
		HP -= info.damage;
		if (!(HP < amountToKillLog * (float)logs.Count))
		{
			return;
		}
		if (HP > 0f)
		{
			KillLog();
			return;
		}
		if (logs[0] != null)
		{
			logs[0].KillLog();
		}
		Object.Destroy(base.gameObject);
	}

	private void KillLog()
	{
		if (logs.Count - 1 > 0)
		{
			logs[logs.Count - 1].KillLog();
			logs.RemoveAt(logs.Count - 1);
			checkedLogs.RemoveAt(logs.Count - 1);
		}
		GetComponent<BoxCollider2D>().offset = new Vector2(0f, logs[(logs.Count - 1) / 2].transform.localPosition.y);
		GetComponent<BoxCollider2D>().size = new Vector2(logs[0].GetComponent<BoxCollider2D>().bounds.size.x, logs[0].GetComponent<BoxCollider2D>().bounds.size.y * (float)logs.Count);
	}

	private IEnumerator check_to_start_cr()
	{
		StartCoroutine(drop_logs_cr());
		yield return null;
	}

	private IEnumerator drop_logs_cr()
	{
		float t = 0f;
		float time = 0.4f;
		YieldInstruction wait = new WaitForFixedUpdate();
		for (int i = 0; i < logs.Count; i++)
		{
			float offset = ((i != 0) ? (logs[i].GetComponent<BoxCollider2D>().bounds.size.y / 2f + logs[i - 1].GetComponent<BoxCollider2D>().bounds.size.y / 2f) : 0f);
			float start = CupheadLevelCamera.Current.Bounds.yMax + 300f;
			float end = dropPosition.y + offset;
			while (t < time)
			{
				if (logs[i] != null)
				{
					t += CupheadTime.FixedDelta;
					float t2 = EaseUtils.Ease(EaseUtils.EaseType.punch, 0f, 1f, t / time);
					logs[i].transform.SetPosition(null, Mathf.Lerp(start, end, t2));
				}
				yield return wait;
			}
			effect.Create(new Vector3(logs[i].transform.position.x, logs[i].transform.position.y - logs[i].GetComponent<BoxCollider2D>().bounds.size.y / 2f));
			t = 0f;
			dropPosition = logs[i].transform.position;
			logs[i].start = logs[i].transform.position.y;
			yield return wait;
		}
		foreach (TreePlatformingLevelLog log in logs)
		{
			log.GetComponent<DamageReceiver>().enabled = true;
		}
		GetComponent<BoxCollider2D>().offset = new Vector2(0f, logs[(logs.Count - 1) / 2].transform.localPosition.y);
		GetComponent<BoxCollider2D>().size = new Vector2(logs[0].GetComponent<BoxCollider2D>().bounds.size.x, logs[0].GetComponent<BoxCollider2D>().bounds.size.y * (float)logs.Count);
		StartCoroutine(shoot_cr());
	}

	private IEnumerator check_to_slide_cr()
	{
		float amount = 0f;
		int indexToSlide = 1000;
		while (true)
		{
			bool hasRemoved = false;
			int i = 0;
			while (i < logs.Count)
			{
				if (logs[i].isDying && !hasRemoved)
				{
					amount = logs[i].GetComponent<BoxCollider2D>().bounds.size.y;
					if (logs[i].CanShoot)
					{
						shootableLogs--;
					}
					logs.RemoveAt(i);
					checkedLogs.RemoveAt(i);
					indexToSlide = i;
					hasRemoved = true;
					continue;
				}
				if (i >= indexToSlide)
				{
					while (logs[i].isSliding)
					{
						yield return null;
					}
					if (logs[i] != null)
					{
						logs[i].SlideDown(amount);
					}
				}
				if (i == logs.Count - 1)
				{
					indexToSlide = 1000;
				}
				i++;
			}
			yield return null;
		}
	}

	private IEnumerator shoot_cr()
	{
		int rand = 0;
		while (shootableLogs > 0 && logs.Count > 0)
		{
			yield return CupheadTime.WaitForSeconds(this, logs[rand].ShootDelay);
			for (int i = 0; i < checkedLogs.Count; i++)
			{
				checkedLogs[i] = false;
			}
			while (checkedLogs.Contains(false))
			{
				rand = Random.Range(0, checkedLogs.Count);
				if (logs[rand].CanShoot && !checkedLogs[rand])
				{
					AudioManager.Play("level_platform_logface_attack");
					emitAudioFromObject.Add("level_platform_logface_attack");
					logs[rand].OnShoot();
					break;
				}
				checkedLogs[rand] = true;
			}
			yield return null;
		}
	}

	protected override void OnDrawGizmos()
	{
		base.OnDrawGizmos();
		float num = 1000f;
		Gizmos.color = Color.green;
		Gizmos.DrawWireCube(new Vector3(base.transform.position.x, base.transform.position.y + num / 2f), new Vector3(logPrefabs[0].GetComponent<SpriteRenderer>().bounds.size.x, num, 0f));
	}
}
