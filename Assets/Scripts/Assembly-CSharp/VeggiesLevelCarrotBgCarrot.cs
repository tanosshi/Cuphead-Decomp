using System.Collections;
using UnityEngine;

public class VeggiesLevelCarrotBgCarrot : AbstractMonoBehaviour
{
	private const float RANGE_MIN = 150f;

	private const float RANGE_MAX = 600f;

	private VeggiesLevelCarrot parentCarrot;

	public VeggiesLevelCarrotBgCarrot Create(int side, float speed, VeggiesLevelCarrot parentCarrot)
	{
		VeggiesLevelCarrotBgCarrot veggiesLevelCarrotBgCarrot = InstantiatePrefab<VeggiesLevelCarrotBgCarrot>();
		veggiesLevelCarrotBgCarrot.Init(side, speed, parentCarrot);
		return veggiesLevelCarrotBgCarrot;
	}

	private void Init(int side, float speed, VeggiesLevelCarrot parentCarrot)
	{
		base.transform.SetPosition(Random.Range(150f, 600f) * (float)side, -360f, 0f);
		this.parentCarrot = parentCarrot;
		StartCoroutine(float_cr(speed));
	}

	private void End()
	{
		StopAllCoroutines();
		Object.Destroy(base.gameObject);
	}

	private IEnumerator float_cr(float speed)
	{
		YieldInstruction wait = new WaitForFixedUpdate();
		while (true)
		{
			if (base.transform.position.y > 720f)
			{
				parentCarrot.ShootHoming();
				End();
			}
			base.transform.AddPosition(0f, speed * CupheadTime.FixedDelta);
			yield return wait;
		}
	}
}
