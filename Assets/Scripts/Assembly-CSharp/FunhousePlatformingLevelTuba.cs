using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class FunhousePlatformingLevelTuba : AbstractPlatformingLevelEnemy
{
	[SerializeField]
	private Transform root;

	[SerializeField]
	private BasicProjectile projectile;

	[SerializeField]
	private Transform startPos;

	[SerializeField]
	private Transform endPos;

	private float offset = 50f;

	private bool projectilesOn;

	private Vector2 start;

	private Vector2 end;

	private List<GameObject> bwaaList = new List<GameObject>();

	protected override void Start()
	{
		base.Start();
		base.transform.position = startPos.transform.position;
		start = startPos.transform.position;
		end = endPos.transform.position;
		StartCoroutine(check_to_start_cr());
	}

	protected override void OnStart()
	{
		StartCoroutine(attack_cr());
	}

	private IEnumerator check_to_start_cr()
	{
		while (base.transform.position.x > CupheadLevelCamera.Current.Bounds.xMax + offset)
		{
			yield return null;
		}
		OnStart();
		yield return null;
	}

	private IEnumerator attack_cr()
	{
		float time = base.Properties.MoveSpeed;
		float t = 0f;
		yield return CupheadTime.WaitForSeconds(this, base.Properties.tubaInitialDelay);
		while (true)
		{
			if (base.transform.position.x > CupheadLevelCamera.Current.Bounds.xMax + offset || base.transform.position.x < CupheadLevelCamera.Current.Bounds.xMin - offset)
			{
				yield return null;
				continue;
			}
			t = 0f;
			base.animator.SetBool("isAttacking", true);
			yield return base.animator.WaitForAnimationToEnd(this, "Tuba_Anti");
			base.animator.Play("Attack_" + ((!Rand.Bool()) ? "B" : "A"), 1);
			StartCoroutine(shoot_cr());
			while (t < time)
			{
				t += (float)CupheadTime.Delta;
				Vector2 pos = base.transform.position;
				pos.y = Mathf.Lerp(base.transform.position.y, end.y, t / time);
				base.transform.position = pos;
				yield return null;
			}
			t = 0f;
			while (t < time)
			{
				t += (float)CupheadTime.Delta;
				Vector2 pos2 = base.transform.position;
				pos2.y = Mathf.Lerp(base.transform.position.y, start.y, t / time);
				base.transform.position = pos2;
				yield return null;
			}
			yield return CupheadTime.WaitForSeconds(this, base.Properties.tubaMainDelayRange.RandomFloat());
			yield return null;
		}
	}

	private IEnumerator shoot_cr()
	{
		AudioManager.Play("funhouse_jackbox_attack");
		emitAudioFromObject.Add("funhouse_jackbox_attack");
		float delay = 0f;
		BasicProjectile p = projectile.Create(root.transform.position, 180f, base.Properties.ProjectileSpeed);
		p.animator.Play("BW");
		p.transform.parent = base.transform;
		p.OnDie += OnBwaaDie;
		bwaaList.Add(p.gameObject);
		delay = p.transform.GetComponent<SpriteRenderer>().bounds.size.x / 1.4f / base.Properties.ProjectileSpeed;
		yield return CupheadTime.WaitForSeconds(this, delay);
		for (int i = 0; i < base.Properties.tubaACount; i++)
		{
			p = projectile.Create(root.transform.position, 180f, base.Properties.ProjectileSpeed);
			p.animator.Play("A" + Random.Range(1, 4));
			p.transform.parent = base.transform;
			p.OnDie += OnBwaaDie;
			bwaaList.Add(p.gameObject);
			delay = p.transform.GetComponent<SpriteRenderer>().bounds.size.x / 2f / base.Properties.ProjectileSpeed;
			yield return CupheadTime.WaitForSeconds(this, delay);
		}
		p = projectile.Create(root.transform.position, 180f, base.Properties.ProjectileSpeed);
		p.animator.Play("EXCLAIM");
		p.transform.parent = base.transform;
		p.OnDie += OnBwaaDie;
		bwaaList.Add(p.gameObject);
		yield return CupheadTime.WaitForSeconds(this, delay);
		base.animator.SetBool("isAttacking", false);
		AudioManager.Stop("tuba_temp");
		emitAudioFromObject.Add("tuba_temp");
		yield return null;
	}

	private void OnBwaaDie(AbstractProjectile p)
	{
		p.OnDie -= OnBwaaDie;
		if (bwaaList != null)
		{
			bwaaList.Remove(p.gameObject);
		}
	}

	protected override void OnDrawGizmos()
	{
		base.OnDrawGizmos();
		Gizmos.color = new Color(0f, 1f, 0f, 1f);
		Gizmos.DrawLine(startPos.transform.position, endPos.transform.position);
		Gizmos.color = new Color(1f, 0f, 0f, 1f);
		Gizmos.DrawWireSphere(startPos.transform.position, 10f);
		Gizmos.DrawWireSphere(endPos.transform.position, 10f);
	}

	protected override void Die()
	{
		StopAllCoroutines();
		base.animator.SetTrigger("OnDeath");
		StartCoroutine(slide_off_cr());
	}

	private IEnumerator slide_off_cr()
	{
		for (int i = 0; i < bwaaList.Count; i++)
		{
			if (bwaaList[i] != null)
			{
				bwaaList[i].transform.SetParent(null);
			}
		}
		float t = 0f;
		float time = 3f;
		float start = base.transform.position.y;
		YieldInstruction wait = new WaitForFixedUpdate();
		while (t < time)
		{
			t += CupheadTime.FixedDelta;
			if (base.transform.localScale.y > 0f)
			{
				base.transform.SetPosition(null, Mathf.Lerp(start, -860f, t / time));
			}
			else
			{
				base.transform.SetPosition(null, Mathf.Lerp(start, 1220f, t / time));
			}
			yield return wait;
		}
		_003CDie_003E__BaseCallProxy0();
		yield return null;
	}

	private void SoundTubaAnti()
	{
		AudioManager.Play("funhouse_jackbox_anti");
		emitAudioFromObject.Add("funhouse_jackbox_anti");
	}

	private void SoundTubaDeath()
	{
		AudioManager.Play("funhouse_jackbox_death");
		emitAudioFromObject.Add("funhouse_jackbox_death");
	}

	[CompilerGenerated]
	[DebuggerHidden]
	private void _003CDie_003E__BaseCallProxy0()
	{
		base.Die();
	}
}
