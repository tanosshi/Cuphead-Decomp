using UnityEngine;

public class BeeLevelHoneyDrip : AbstractMonoBehaviour
{
	private const float START_Y = 415f;

	private const int MAX = 5;

	private int i;

	public BeeLevelHoneyDrip Create()
	{
		return Object.Instantiate(this);
	}

	private BeeLevelHoneyDrip Create(int number)
	{
		BeeLevelHoneyDrip beeLevelHoneyDrip = Create();
		beeLevelHoneyDrip.i = number;
		return beeLevelHoneyDrip;
	}

	protected override void Awake()
	{
		base.Awake();
		GetComponent<Animator>().SetInteger("I", Random.Range(0, 6));
		base.transform.SetParent(Camera.main.transform);
		base.transform.SetLocalPosition(Random.Range(-540, 540), 415f, 100f);
		base.transform.SetParent(null);
		AudioManager.Play("bee_honey_glug_sweet");
	}

	private void OnAnimationEnd()
	{
		if (i < 4 && Random.value < 0.5f)
		{
			Create(i + 1);
		}
		Object.Destroy(base.gameObject);
	}
}
