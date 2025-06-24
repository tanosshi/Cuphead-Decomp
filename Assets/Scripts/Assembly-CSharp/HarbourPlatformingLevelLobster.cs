using System.Collections;
using UnityEngine;

public class HarbourPlatformingLevelLobster : PlatformingLevelShootingEnemy
{
	private new enum Direction
	{
		Left = 0,
		Right = 1
	}

	[SerializeField]
	private Transform main;

	[SerializeField]
	private Transform onTrigger;

	[SerializeField]
	private Transform offTrigger;

	[SerializeField]
	private Transform leftBoundary;

	[SerializeField]
	private Transform rightBoundary;

	[SerializeField]
	private LevelBossDeathExploder exploder;

	[SerializeField]
	private GameObject splashPrefab;

	[SerializeField]
	private Transform splashTransform;

	private Vector3 localPosition;

	private bool poppedUp;

	private bool isGone;

	private float dist = 1000f;

	private float startPositionY;

	private const float OffScreenPadding = 350f;

	private const float attackPadding = -250f;

	private Coroutine mainCoroutine;

	private float previousY;

	private Direction direction = Direction.Right;

	protected override void Start()
	{
		base.Start();
		startPositionY = base.transform.position.y;
		StartCoroutine(start_trigger_cr());
		previousY = base.transform.position.y;
		exploder = GetComponent<LevelBossDeathExploder>();
	}

	protected override void OnDrawGizmos()
	{
		base.OnDrawGizmos();
		Gizmos.color = new Color(0f, 0f, 1f, 1f);
		Gizmos.DrawLine(offTrigger.transform.position, new Vector3(offTrigger.transform.position.x, 5000f, 0f));
		Gizmos.DrawLine(onTrigger.transform.position, new Vector3(onTrigger.transform.position.x, 5000f, 0f));
		Gizmos.color = new Color(0f, 1f, 0f, 0.5f);
		Gizmos.DrawLine(leftBoundary.transform.position, new Vector3(leftBoundary.transform.position.x, 5000f, 0f));
		Gizmos.DrawLine(rightBoundary.transform.position, new Vector3(rightBoundary.transform.position.x, 5000f, 0f));
	}

	private IEnumerator attack_cr()
	{
		base.animator.SetTrigger("OnAttackStart");
		yield return base.animator.WaitForAnimationToStart(this, "Warning_Loop");
		yield return CupheadTime.WaitForSeconds(this, base.Properties.lobsterWarningTime);
		base.animator.SetTrigger("Attack");
		AttackSFX();
		yield return null;
		yield return base.animator.WaitForAnimationToStart(this, "Attack_Trans_Idle");
	}

	private IEnumerator start_trigger_cr()
	{
		while (_target == null)
		{
			yield return null;
		}
		dist = _target.transform.position.x - onTrigger.transform.position.x;
		while (dist < 0f)
		{
			dist = _target.transform.position.x - onTrigger.transform.position.x;
			yield return null;
		}
		mainCoroutine = StartCoroutine(main_cr());
		dist = _target.transform.position.x - offTrigger.transform.position.x;
		while (dist < 0f)
		{
			dist = _target.transform.position.x - offTrigger.transform.position.x;
			yield return null;
		}
		isGone = true;
		yield return null;
	}

	private IEnumerator main_cr()
	{
		while (!isGone)
		{
			direction = ((direction != Direction.Right) ? Direction.Right : Direction.Left);
			base.transform.localScale = new Vector3((direction != Direction.Left) ? 1 : (-1), 1f, 1f);
			base.transform.SetPosition((direction != Direction.Left) ? (CupheadLevelCamera.Current.Bounds.left - 350f) : (CupheadLevelCamera.Current.Bounds.right + 350f), base.Properties.lobsterY);
			if ((direction != Direction.Left) ? (base.transform.position.x > rightBoundary.position.x) : (base.transform.position.x < leftBoundary.position.x))
			{
				base.transform.SetPosition(null, -5000f);
				yield return CupheadTime.WaitForSeconds(this, base.Properties.lobsterOffscreenTime);
				continue;
			}
			if ((direction != Direction.Left) ? (base.transform.position.x < leftBoundary.position.x) : (base.transform.position.x > rightBoundary.position.x))
			{
				base.transform.SetPosition((direction != Direction.Left) ? leftBoundary.position.x : rightBoundary.position.x);
				yield return StartCoroutine(pop_up_cr());
			}
			else
			{
				base.animator.Play("Idle");
				IdleSFX();
				poppedUp = true;
			}
			while ((direction != Direction.Left) ? (base.transform.position.x < CupheadLevelCamera.Current.Bounds.right + -250f) : (base.transform.position.x > CupheadLevelCamera.Current.Bounds.left - -250f))
			{
				base.transform.AddPosition(base.Properties.lobsterSpeed * (float)CupheadTime.Delta * (float)((direction != Direction.Left) ? 1 : (-1)));
				if ((direction != Direction.Left) ? (base.transform.position.x > rightBoundary.position.x) : (base.transform.position.x < leftBoundary.position.x))
				{
					Popdown(false);
					yield break;
				}
				yield return null;
			}
			yield return StartCoroutine(attack_cr());
			while ((direction != Direction.Left) ? (base.transform.position.x < CupheadLevelCamera.Current.Bounds.right + 350f) : (base.transform.position.x > CupheadLevelCamera.Current.Bounds.left - 350f))
			{
				base.transform.AddPosition(base.Properties.lobsterSpeed * (float)CupheadTime.Delta * (float)((direction != Direction.Left) ? 1 : (-1)));
				if ((direction != Direction.Left) ? (base.transform.position.x > rightBoundary.position.x) : (base.transform.position.x < leftBoundary.position.x))
				{
					Popdown(false);
					yield break;
				}
				yield return null;
			}
			base.transform.SetPosition(null, -5000f);
			yield return CupheadTime.WaitForSeconds(this, base.Properties.lobsterOffscreenTime);
		}
		Object.Destroy(main.gameObject);
	}

	private void Popup()
	{
		StartCoroutine(pop_up_cr());
	}

	private void Popdown(bool dead)
	{
		StartCoroutine(pop_down_cr(dead));
	}

	private IEnumerator pop_up_cr()
	{
		base.animator.SetTrigger("OnEmerge");
		EmergeSFX();
		localPosition = base.transform.localPosition;
		float t = 0f;
		float time = 0.6f;
		float endY = base.Properties.lobsterY;
		Vector2 end = new Vector2(base.transform.position.x, endY);
		while (t < time)
		{
			Vector2 start = base.transform.position;
			end = new Vector2(base.transform.position.x, endY);
			float val = EaseUtils.Ease(EaseUtils.EaseType.easeInOutSine, 0f, 1f, t / time);
			base.transform.position = Vector2.Lerp(start, end, val);
			t += (float)CupheadTime.Delta;
			yield return null;
		}
		base.transform.position = end;
		poppedUp = true;
	}

	private IEnumerator pop_down_cr(bool dead)
	{
		if (!poppedUp)
		{
			yield break;
		}
		poppedUp = false;
		if (dead && mainCoroutine != null)
		{
			StopCoroutine(mainCoroutine);
		}
		if (isGone)
		{
			base.transform.parent = null;
			base.animator.SetTrigger("OnTuck");
		}
		else
		{
			base.animator.Play("Tuck");
			SinkSFX();
		}
		if (dead)
		{
			exploder.StartExplosion();
			yield return new WaitForSeconds(1f);
			exploder.StopExplosions();
		}
		float t = 0f;
		float time = 1.5f;
		Vector2 end = new Vector2(base.transform.position.x, startPositionY);
		float splashDepth = -1200f;
		while (t < time)
		{
			Vector2 start = base.transform.position;
			end = new Vector2(base.transform.position.x, startPositionY);
			float val = EaseUtils.Ease(EaseUtils.EaseType.easeInSine, 0f, 1f, t / time);
			base.transform.position = Vector2.Lerp(start, end, val);
			if (base.transform.position.y <= splashDepth && previousY > splashDepth)
			{
				GameObject gameObject = Object.Instantiate(splashPrefab, new Vector3(base.transform.position.x, splashDepth, base.transform.position.z), Quaternion.identity);
				gameObject.transform.SetParent(null);
				Object.Destroy(gameObject, 10f);
			}
			t += (float)CupheadTime.Delta;
			previousY = base.transform.position.y;
			yield return null;
		}
		base.transform.position = end;
		if (isGone)
		{
			Object.Destroy(main.gameObject);
		}
		else
		{
			StartCoroutine(delay_cr(dead));
		}
		yield return null;
	}

	protected override void Die()
	{
		Popdown(true);
	}

	private IEnumerator delay_cr(bool dead)
	{
		yield return CupheadTime.WaitForSeconds(this, (!dead) ? base.Properties.lobsterOffscreenTime : base.Properties.lobsterTuckTime);
		if (isGone)
		{
			Object.Destroy(main.gameObject);
			yield break;
		}
		base.Health = base.Properties.Health;
		mainCoroutine = StartCoroutine(main_cr());
	}

	private void EmergeSFX()
	{
		AudioManager.Play("harbour_lobster_emerge");
		emitAudioFromObject.Add("harbour_lobster_emerge");
	}

	private void SinkSFX()
	{
		AudioManager.Stop("harbour_lobster_idle");
		AudioManager.Play("harbour_lobster_sink");
		emitAudioFromObject.Add("harbour_lobster_sink");
	}

	private void AttackSFX()
	{
		AudioManager.Play("harbour_lobster_attack");
		emitAudioFromObject.Add("harbour_lobster_attack");
	}

	private void IdleSFX()
	{
		AudioManager.PlayLoop("harbour_lobster_idle");
		emitAudioFromObject.Add("harbour_lobster_idle");
	}
}
