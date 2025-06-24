using System.Collections;
using UnityEngine;

public class MountainPlatformingLevelCyclopsRocks : AbstractPausableComponent
{
	private enum CyclopsState
	{
		UnSpawned = 0,
		Spawned = 1,
		Turning = 2,
		Attacking = 3,
		Dead = 4
	}

	[SerializeField]
	private float walkSpeed;

	[SerializeField]
	private MinMax attackDelayRange;

	[SerializeField]
	private Transform onTrigger;

	[SerializeField]
	private Transform offTrigger;

	[SerializeField]
	private MountainPlatformingLevelCyclopsBG cyclopsBG;

	[SerializeField]
	private float cyclopsStopOffset;

	private AbstractPlayerController player;

	private CyclopsState cyclopsState;

	private bool IsIdle;

	private bool playerInTrigger;

	private bool facingLeft;

	private Animator cyclopsAnimator;

	protected override void Start()
	{
		base.Start();
		cyclopsState = CyclopsState.UnSpawned;
		StartCoroutine(start_trigger_cr());
		cyclopsAnimator = cyclopsBG.GetComponent<Animator>();
	}

	private IEnumerator start_trigger_cr()
	{
		yield return CupheadTime.WaitForSeconds(this, 0.2f);
		player = PlayerManager.GetNext();
		while (player.transform.position.x < onTrigger.transform.position.x)
		{
			yield return null;
			if (player == null || player.IsDead)
			{
				player = PlayerManager.GetNext();
			}
		}
		StartCyclops();
		while (cyclopsState != CyclopsState.Dead)
		{
			if (player == null || player.IsDead)
			{
				player = PlayerManager.GetNext();
			}
			playerInTrigger = ((player.transform.position.x > onTrigger.transform.position.x) ? true : false);
			if (player.transform.position.x > offTrigger.transform.position.x)
			{
				cyclopsBG.isDead = true;
				cyclopsState = CyclopsState.Dead;
				break;
			}
			yield return null;
		}
		yield return null;
	}

	private void StartCyclops()
	{
		IsIdle = true;
		playerInTrigger = true;
		cyclopsBG.start = cyclopsBG.transform.position;
		cyclopsAnimator.SetTrigger("StartCyclops");
		cyclopsAnimator.SetBool("isIdle", IsIdle);
		cyclopsState = CyclopsState.Spawned;
		StartCoroutine(walk_and_idle_cr());
		StartCoroutine(attack_cr());
		StartCoroutine(move_cr());
	}

	protected override void Update()
	{
		base.Update();
	}

	private IEnumerator turn_cyclops_cr()
	{
		if (cyclopsState != CyclopsState.Turning)
		{
			string ani = (IsIdle ? "Turn" : "Turn_To_Walk");
			cyclopsAnimator.SetTrigger("OnTurn");
			cyclopsState = CyclopsState.Turning;
			yield return cyclopsAnimator.WaitForAnimationToEnd(this, ani);
			facingLeft = cyclopsBG.transform.localScale.x == 1f;
			cyclopsState = CyclopsState.Spawned;
		}
	}

	private IEnumerator walk_and_idle_cr()
	{
		facingLeft = true;
		float t = 0f;
		float timer = 1f;
		while (cyclopsState != CyclopsState.Dead)
		{
			if (cyclopsState == CyclopsState.Spawned)
			{
				if (IsIdle)
				{
					if (player.transform.position.x < cyclopsBG.transform.position.x + cyclopsStopOffset && player.transform.position.x > cyclopsBG.transform.position.x - cyclopsStopOffset)
					{
						if (player.transform.position.x < cyclopsBG.transform.position.x && !facingLeft)
						{
							yield return StartCoroutine(turn_cyclops_cr());
						}
						else if (player.transform.position.x > cyclopsBG.transform.position.x && facingLeft)
						{
							yield return StartCoroutine(turn_cyclops_cr());
						}
					}
					else
					{
						IsIdle = false;
						cyclopsAnimator.SetBool("isIdle", IsIdle);
						yield return cyclopsAnimator.WaitForAnimationToEnd(this, "Idle_To_Walk");
					}
				}
				else if (player.transform.position.x < cyclopsBG.transform.position.x - cyclopsStopOffset && !facingLeft)
				{
					yield return StartCoroutine(turn_cyclops_cr());
				}
				else if (player.transform.position.x > cyclopsBG.transform.position.x + cyclopsStopOffset && facingLeft)
				{
					yield return StartCoroutine(turn_cyclops_cr());
				}
				else if (t < timer)
				{
					t += (float)CupheadTime.Delta;
				}
				else
				{
					IsIdle = true;
					cyclopsAnimator.SetBool("isIdle", IsIdle);
					yield return cyclopsAnimator.WaitForAnimationToEnd(this, "Walk_To_Idle");
					t = 0f;
				}
			}
			yield return null;
		}
	}

	private IEnumerator move_cr()
	{
		while (cyclopsBG != null)
		{
			if (cyclopsBG.isWalking)
			{
				cyclopsBG.transform.AddPosition(((!facingLeft) ? walkSpeed : (0f - walkSpeed)) * (float)CupheadTime.Delta);
			}
			yield return null;
		}
	}

	private IEnumerator attack_cr()
	{
		while (cyclopsState != CyclopsState.Dead)
		{
			yield return CupheadTime.WaitForSeconds(this, attackDelayRange.RandomFloat());
			while (!playerInTrigger || cyclopsState == CyclopsState.Turning)
			{
				yield return null;
			}
			cyclopsBG.GetPlayer(player);
			cyclopsAnimator.SetTrigger("OnAttack");
			yield return cyclopsAnimator.WaitForAnimationToStart(this, "Attack_Start");
			cyclopsState = CyclopsState.Attacking;
			if (IsIdle)
			{
				yield return cyclopsAnimator.WaitForAnimationToEnd(this, "Attack_To_Idle");
			}
			else
			{
				yield return cyclopsAnimator.WaitForAnimationToEnd(this, "Attack_To_Walk");
			}
			if (player.transform.position.x < cyclopsBG.transform.position.x && !facingLeft)
			{
				yield return StartCoroutine(turn_cyclops_cr());
			}
			else if (player.transform.position.x > cyclopsBG.transform.position.x && facingLeft)
			{
				yield return StartCoroutine(turn_cyclops_cr());
			}
			else
			{
				cyclopsState = CyclopsState.Spawned;
			}
			yield return CupheadTime.WaitForSeconds(this, 0.1f);
		}
		cyclopsAnimator.SetTrigger("OnAttack");
		yield return null;
	}

	protected override void OnDrawGizmos()
	{
		base.OnDrawGizmos();
		Gizmos.color = new Color(0f, 0f, 1f, 1f);
		Gizmos.DrawLine(offTrigger.transform.position, new Vector3(offTrigger.transform.position.x, 5000f, 0f));
		Gizmos.DrawLine(onTrigger.transform.position, new Vector3(onTrigger.transform.position.x, 5000f, 0f));
		Gizmos.color = new Color(1f, 0f, 1f, 1f);
		if ((bool)cyclopsBG)
		{
			Gizmos.DrawLine(new Vector3(cyclopsBG.transform.position.x + cyclopsStopOffset, cyclopsBG.transform.position.y), new Vector3(cyclopsBG.transform.position.x - cyclopsStopOffset, cyclopsBG.transform.position.y));
		}
	}
}
