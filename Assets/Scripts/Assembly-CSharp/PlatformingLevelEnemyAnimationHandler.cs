using UnityEngine;

public class PlatformingLevelEnemyAnimationHandler : AbstractPausableComponent
{
	[SerializeField]
	private int numOfTypes;

	[SerializeField]
	private int secondaryTypes;

	private int index1;

	private int index2;

	private const string LETTERS = "A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z";

	public void SelectAnimation(string type1)
	{
		for (int i = 0; i < numOfTypes; i++)
		{
			if (type1.Substring(0, 1) == "A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z".Split(',')[i])
			{
				index1 = i;
			}
			if (secondaryTypes > 0 && type1.Substring(1, 1) == "A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z".Split(',')[i])
			{
				index2 = i + 1;
			}
		}
		SpriteRenderer[] componentsInChildren = GetComponentsInChildren<SpriteRenderer>();
		foreach (SpriteRenderer spriteRenderer in componentsInChildren)
		{
			spriteRenderer.enabled = false;
		}
		GetComponentsInChildren<SpriteRenderer>()[index1].enabled = true;
		if (secondaryTypes > 0)
		{
			GetComponent<Animator>().SetInteger("type", index2);
		}
	}
}
