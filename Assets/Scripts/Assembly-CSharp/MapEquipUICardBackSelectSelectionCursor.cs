using UnityEngine;

public class MapEquipUICardBackSelectSelectionCursor : MapEquipUICursor
{
	public override void SetPosition(Vector3 position)
	{
		base.SetPosition(position);
		Show();
	}

	public new void Show()
	{
		image.enabled = true;
		base.animator.Play("Idle");
	}

	public void Select()
	{
		base.animator.Play("Select");
	}
}
