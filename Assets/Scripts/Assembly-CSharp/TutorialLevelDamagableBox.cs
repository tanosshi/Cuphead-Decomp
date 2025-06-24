using UnityEngine;

public class TutorialLevelDamagableBox : AbstractCollidableObject
{
	[SerializeField]
	private float boxHealth = 20f;

	[SerializeField]
	private GameObject toDisable;

	[SerializeField]
	private GameObject toEnable;

	[SerializeField]
	private PlatformingLevelGenericExplosion explosionPrefab;

	[SerializeField]
	private Vector3 explosionPosition;

	protected override void Start()
	{
		base.Start();
		GetComponent<DamageReceiver>().OnDamageTaken += OnDamageTaken;
	}

	protected override void OnDrawGizmosSelected()
	{
		base.OnDrawGizmosSelected();
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(explosionPosition + base.transform.position, 10f);
	}

	private void OnDamageTaken(DamageDealer.DamageInfo info)
	{
		boxHealth -= info.damage;
		if (boxHealth <= 0f)
		{
			toEnable.SetActive(true);
			toDisable.SetActive(false);
			explosionPrefab.Create(explosionPosition + base.transform.position);
			AudioManager.Play("sfx_object_explode");
			emitAudioFromObject.Add("sfx_object_explode");
			Object.Destroy(base.gameObject);
		}
	}
}
