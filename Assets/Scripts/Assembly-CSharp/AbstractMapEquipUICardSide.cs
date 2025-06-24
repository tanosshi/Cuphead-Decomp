using UnityEngine;

public class AbstractMapEquipUICardSide : AbstractMonoBehaviour
{
	protected CanvasGroup canvasGroup;

	protected PlayerId playerID { get; private set; }

	protected override void Awake()
	{
		base.Awake();
		canvasGroup = GetComponent<CanvasGroup>();
	}

	public virtual void Init(PlayerId playerID)
	{
		this.playerID = playerID;
	}

	public void SetActive(bool active)
	{
		canvasGroup.alpha = (active ? 1 : 0);
	}
}
