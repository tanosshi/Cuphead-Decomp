using UnityEngine;

public class GameObjectHelper
{
	private GameObject _gameObject;

	public GameObjectHelperGO events { get; private set; }

	public GameObjectHelper(string name)
	{
		_gameObject = new GameObject("[Helper] " + name);
		events = _gameObject.AddComponent<GameObjectHelperGO>();
	}

	public void Destroy()
	{
		Object.Destroy(_gameObject);
	}

	public void DontDestroyOnLoad()
	{
		Object.DontDestroyOnLoad(_gameObject);
	}
}
