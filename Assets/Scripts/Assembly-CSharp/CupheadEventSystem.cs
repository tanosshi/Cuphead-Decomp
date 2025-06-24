using UnityEngine;

public class CupheadEventSystem : AbstractMonoBehaviour
{
	private const string PATH = "EventSystems/CupheadEventSystem";

	private static CupheadEventSystem _instance;

	public static void Init()
	{
		if (!(_instance != null))
		{
			_instance = (Object.Instantiate(Resources.Load("EventSystems/CupheadEventSystem")) as GameObject).GetComponent<CupheadEventSystem>();
		}
	}

	protected override void Awake()
	{
		base.Awake();
		if (_instance == null)
		{
			_instance = this;
			base.gameObject.name = base.gameObject.name.Replace("(Clone)", string.Empty);
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
	}
}
