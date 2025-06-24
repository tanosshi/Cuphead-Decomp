using UnityEngine;

public class GameObjectHelperGO : MonoBehaviour
{
	public delegate void OnUpdateHandler();

	public delegate void OnFixedUpdateHandler();

	public delegate void OnLateUpdateHandler();

	public delegate void OnDrawGizmosHandler();

	public delegate void OnDestroyHandler();

	public event OnUpdateHandler onUpdate;

	public event OnFixedUpdateHandler onFixedUpdate;

	public event OnLateUpdateHandler onLateUpdate;

	public event OnDrawGizmosHandler onDrawGizmos;

	public event OnDestroyHandler onDestroy;

	protected void Update()
	{
		if (this.onUpdate != null)
		{
			this.onUpdate();
		}
	}

	protected void FixedUpdate()
	{
		if (this.onFixedUpdate != null)
		{
			this.onFixedUpdate();
		}
	}

	protected void LateUpdate()
	{
		if (this.onLateUpdate != null)
		{
			this.onLateUpdate();
		}
	}

	protected void OnDrawGizmos()
	{
		if (this.onDrawGizmos != null)
		{
			this.onDrawGizmos();
		}
	}

	protected void OnDestroy()
	{
		if (this.onDestroy != null)
		{
			this.onDestroy();
		}
		clear();
	}

	private void clear()
	{
		this.onUpdate = null;
		this.onFixedUpdate = null;
		this.onLateUpdate = null;
		this.onDrawGizmos = null;
	}
}
