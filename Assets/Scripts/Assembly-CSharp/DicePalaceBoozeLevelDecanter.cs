using System.Collections;
using UnityEngine;

public class DicePalaceBoozeLevelDecanter : DicePalaceBoozeLevelBossBase
{
	[SerializeField]
	private Transform sprayYRoot;

	[SerializeField]
	private GameObject sprayPrefab;

	private bool attacking;

	private int attackDelayIndex;

	private PlayerId nextPlayerTarget;

	private Vector3 dropPosition;

	private DamageDealer damageDealer;

	private DamageReceiver damageReceiver;

	protected override void Awake()
	{
		damageDealer = DamageDealer.NewEnemy();
		damageReceiver = GetComponent<DamageReceiver>();
		damageReceiver.OnDamageTaken += OnDamageTaken;
		base.Awake();
	}

	protected override void Update()
	{
		damageDealer.Update();
		base.Update();
	}

	private void OnDamageTaken(DamageDealer.DamageInfo info)
	{
		health -= info.damage;
		if (health < 0f && !base.isDead)
		{
			StartDying();
		}
	}

	public override void LevelInit(LevelProperties.DicePalaceBooze properties)
	{
		dropPosition.z = 0f;
		dropPosition.y = sprayYRoot.position.y;
		attackDelayIndex = Random.Range(0, properties.CurrentState.decanter.attackDelayString.Split(',').Length);
		attacking = false;
		nextPlayerTarget = PlayerId.PlayerOne;
		Level.Current.OnIntroEvent += OnIntroEnd;
		Level.Current.OnWinEvent += HandleDead;
		health = properties.CurrentState.decanter.decanterHP;
		AudioManager.Play("booze_decanter_intro");
		emitAudioFromObject.Add("booze_decanter_intro");
		base.LevelInit(properties);
	}

	private void OnIntroEnd()
	{
		StartCoroutine(attack_cr());
	}

	private IEnumerator attack_cr()
	{
		while (true)
		{
			yield return CupheadTime.WaitForSeconds(this, float.Parse(base.properties.CurrentState.decanter.attackDelayString.Split(',')[attackDelayIndex]) - DicePalaceBoozeLevelBossBase.ATTACK_DELAY);
			base.animator.SetTrigger("OnAttack");
			yield return base.animator.WaitForAnimationToStart(this, "Attack");
			AudioManager.Play("booze_decanter_attack");
			emitAudioFromObject.Add("booze_decanter_attack");
			StartCoroutine(spray_cr());
			attackDelayIndex++;
			if (attackDelayIndex >= base.properties.CurrentState.decanter.attackDelayString.Split(',').Length)
			{
				attackDelayIndex = 0;
			}
			if (nextPlayerTarget == PlayerId.PlayerOne)
			{
				if (PlayerManager.GetPlayer(PlayerId.PlayerTwo) != null)
				{
					nextPlayerTarget = PlayerId.PlayerTwo;
				}
			}
			else
			{
				nextPlayerTarget = PlayerId.PlayerOne;
			}
			while (attacking)
			{
				yield return null;
			}
		}
	}

	private IEnumerator spray_cr()
	{
		attacking = true;
		yield return CupheadTime.WaitForSeconds(this, base.properties.CurrentState.decanter.beamAppearDelayRange.RandomFloat());
		AudioManager.Play("booze_decanter_spray_down");
		emitAudioFromObject.Add("booze_decanter_spray_down");
		GameObject spray = Object.Instantiate(sprayPrefab, dropPosition, Quaternion.identity);
		attacking = false;
		dropPosition.x = PlayerManager.GetPlayer(nextPlayerTarget).center.x;
		Vector3 pos = spray.transform.position;
		pos.x = dropPosition.x;
		spray.transform.position = pos;
		yield return spray.GetComponent<Animator>().WaitForAnimationToEnd(this, "Spray");
		Object.Destroy(spray);
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		damageDealer.DealDamage(hit);
		base.OnCollisionPlayer(hit, phase);
	}
}
