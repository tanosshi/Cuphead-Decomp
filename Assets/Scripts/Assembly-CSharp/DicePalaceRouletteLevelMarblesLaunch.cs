using UnityEngine;

public class DicePalaceRouletteLevelMarblesLaunch : AbstractMonoBehaviour
{
	private const string IsFirstTimeParameterName = "IsFirstTime";

	public bool IsFirstTime
	{
		set
		{
			base.animator.SetBool("IsFirstTime", value);
		}
	}

	public void KillMarblesLaunch()
	{
		Object.Destroy(base.gameObject);
	}
}
