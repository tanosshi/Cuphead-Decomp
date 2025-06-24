using System.IO;
using System.Xml.Serialization;
using DialoguerEditor;
using UnityEngine;

namespace DialoguerCore
{
	public class DialoguerDataManager
	{
		private static DialoguerData _data;

		public static void Initialize()
		{
			DialogueEditorMasterObject data = (Resources.Load("dialoguer_data_object") as DialogueEditorMasterObjectWrapper).data;
			_data = data.getDialoguerData();
		}

		public static string GetGlobalVariablesState()
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(DialoguerGlobalVariables));
			StringWriter stringWriter = new StringWriter();
			xmlSerializer.Serialize(stringWriter, _data.globalVariables);
			return stringWriter.ToString();
		}

		public static void LoadGlobalVariablesState(string globalVariablesXml)
		{
			_data.loadGlobalVariablesState(globalVariablesXml);
		}

		public static float GetGlobalFloat(int floatId)
		{
			return _data.globalVariables.floats[floatId];
		}

		public static void SetGlobalFloat(int floatId, float floatValue)
		{
			_data.globalVariables.floats[floatId] = floatValue;
		}

		public static bool GetGlobalBoolean(int booleanId)
		{
			return _data.globalVariables.booleans[booleanId];
		}

		public static void SetGlobalBoolean(int booleanId, bool booleanValue)
		{
			_data.globalVariables.booleans[booleanId] = booleanValue;
		}

		public static string GetGlobalString(int stringId)
		{
			return _data.globalVariables.strings[stringId];
		}

		public static void SetGlobalString(int stringId, string stringValue)
		{
			_data.globalVariables.strings[stringId] = stringValue;
		}

		public static DialoguerDialogue GetDialogueById(int dialogueId)
		{
			if (_data.dialogues.Count <= dialogueId)
			{
				Debug.LogWarning("Dialogue [" + dialogueId + "] does not exist.");
				return null;
			}
			return _data.dialogues[dialogueId];
		}
	}
}
