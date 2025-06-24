using UnityEngine;
using UnityEngine.UI;

public class MapEquipUICursor : AbstractMonoBehaviour
{
	[SerializeField]
	private Image selectionCursor;

	[SerializeField]
	protected Image image;

	public virtual void SetPosition(Vector3 position)
	{
		base.transform.position = position;
	}

	public virtual void SelectIcon(bool onSame)
	{
		if (onSame)
		{
			base.animator.Play("Select_V2", 1);
		}
		else
		{
			base.animator.Play("Select", 1);
		}
	}

	public virtual void Hide()
	{
		image.enabled = false;
	}

	public virtual void Show()
	{
		image.enabled = true;
	}

	private void HideSelectionCursor()
	{
		selectionCursor.enabled = false;
	}

	private void ShowSelectionCursor()
	{
		selectionCursor.enabled = true;
	}
}
