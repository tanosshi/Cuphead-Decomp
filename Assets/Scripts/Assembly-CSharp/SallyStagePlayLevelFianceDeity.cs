using System.Collections;
using UnityEngine;

public class SallyStagePlayLevelFianceDeity : AbstractCollidableObject
{
	[SerializeField]
	private BasicProjectile husbandProjectile;

	[SerializeField]
	private Transform husbandRoot;

	private bool isDead;

	private float health;

	private LevelProperties.SallyStagePlay.Husband properties;

	private DamageDealer damageDealer;

	private DamageReceiver damageReceiver;

	protected override void Start()
	{
		base.Start();
		damageDealer = DamageDealer.NewEnemy();
		damageReceiver = GetComponent<DamageReceiver>();
		damageReceiver.OnDamageTaken += OnDamageTaken;
	}

	public void Init(LevelProperties.SallyStagePlay.Husband properties)
	{
		this.properties = properties;
		health = properties.deityHP;
		StartCoroutine(attack_cr());
	}

	private void OnDamageTaken(DamageDealer.DamageInfo info)
	{
		health -= info.damage;
		if (health <= 0f && !isDead)
		{
			isDead = true;
			Dead();
		}
	}

	private IEnumerator attack_cr()
	{
		LevelProperties.SallyStagePlay.Husband p = properties;
		while (!isDead)
		{
			yield return CupheadTime.WaitForSeconds(this, p.shotDelayRange.RandomFloat());
			GetComponent<Animator>().SetBool("OnAttack", true);
			yield return GetComponent<Animator>().WaitForAnimationToEnd(this, "Puppet_Attack_Start");
			husbandProjectile.Create(husbandRoot.position, 0f, new Vector2(p.shotScale, p.shotScale), p.shotSpeed);
			GetComponent<Animator>().SetBool("OnAttack", false);
			yield return null;
		}
	}

	public void Dead()
	{
		StopAllCoroutines();
		damageReceiver.enabled = false;
		GetComponent<Animator>().SetTrigger("OnDeath");
	}

	public IEnumerator move_cr()
	{
		float t = 0f;
		float time = 2.5f;
		Vector3 endPos = new Vector3(-1140f, base.transform.position.y);
		Vector2 start = base.transform.position;
		while (t < time)
		{
			float val = EaseUtils.Ease(EaseUtils.EaseType.linear, 0f, 1f, t / time);
			base.transform.position = Vector2.Lerp(start, endPos, val);
			t += (float)CupheadTime.Delta;
			yield return null;
		}
		Object.Destroy(base.gameObject);
	}
}
