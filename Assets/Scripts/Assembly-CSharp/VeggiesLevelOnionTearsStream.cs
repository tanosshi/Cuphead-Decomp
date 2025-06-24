using UnityEngine;

public class VeggiesLevelOnionTearsStream : AbstractMonoBehaviour
{
	private bool ending;

	public VeggiesLevelOnionTearsStream Create(Vector2 pos, int scale)
	{
		VeggiesLevelOnionTearsStream veggiesLevelOnionTearsStream = InstantiatePrefab<VeggiesLevelOnionTearsStream>();
		veggiesLevelOnionTearsStream.transform.SetScale(scale, 1f, 1f);
		veggiesLevelOnionTearsStream.transform.position = pos;
		return veggiesLevelOnionTearsStream;
	}

	public void End()
	{
		if (!ending)
		{
			ending = true;
			base.animator.Play("Out");
		}
	}

	private void OnAnimEnd()
	{
		Object.Destroy(base.gameObject);
	}
}
