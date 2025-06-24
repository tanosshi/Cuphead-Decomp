using UnityEngine;

public class PlmManager
{
	private static PlmManager instance;

	public static PlmManager Instance
	{
		get
		{
			if (instance == null)
			{
				instance = new PlmManager();
			}
			return instance;
		}
	}

	public PlmInterface Interface { get; private set; }

	public void Init()
	{
		Interface = new DummyPlmInterface();
		Interface.Init();
		Interface.OnSuspend += OnSuspend;
		Interface.OnResume += OnResume;
		Interface.OnConstrained += OnConstrained;
		Interface.OnUnconstrained += OnUnconstrained;
	}

	private void OnSuspend()
	{
		Debug.Log("OnSuspend()");
	}

	private void OnResume()
	{
		Debug.Log("OnResume()");
	}

	private void OnConstrained()
	{
		Debug.Log("OnConstrained()");
	}

	private void OnUnconstrained()
	{
		Debug.Log("OnUnconstrained()");
	}
}
