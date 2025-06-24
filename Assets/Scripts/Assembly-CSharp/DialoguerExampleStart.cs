using UnityEngine;

public class DialoguerExampleStart : MonoBehaviour
{
	private void Awake()
	{
		Dialoguer.Initialize();
	}

	private void OnGUI()
	{
		if (GUI.Button(new Rect(10f, 10f, 100f, 30f), "Start!"))
		{
			Dialoguer.StartDialogue(3);
		}
		string text = "Open this file (DialoguerExampleStart.cs) to see how to start using Dialoguer";
		GUI.Label(new Rect(10f, 50f, 500f, 500f), text);
	}
}
