using UnityEngine;

public class DicePalaceDominoLevelRandomSpike : AbstractMonoBehaviour
{
	[SerializeField]
	private string[] states;

	private bool melt;

	protected override void Start()
	{
		base.Start();
		ChangeSpikes();
	}

	public void ChangeSpikes()
	{
		base.animator.SetTrigger(states[Random.Range(0, states.Length)]);
		melt = false;
	}

	protected override void Update()
	{
		base.Update();
		if (!melt && base.transform.position.x <= -410f)
		{
			melt = true;
			base.animator.SetTrigger("Melt");
		}
	}
}
