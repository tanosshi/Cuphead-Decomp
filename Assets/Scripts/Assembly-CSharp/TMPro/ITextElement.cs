using UnityEngine;
using UnityEngine.UI;

namespace TMPro
{
	public interface ITextElement
	{
		Material sharedMaterial { get; }

		void Rebuild(CanvasUpdate update);

		int GetInstanceID();
	}
}
