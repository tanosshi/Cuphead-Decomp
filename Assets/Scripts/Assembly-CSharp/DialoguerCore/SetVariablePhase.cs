using System.Collections.Generic;
using DialoguerEditor;
using UnityEngine;

namespace DialoguerCore
{
	public class SetVariablePhase : AbstractDialoguePhase
	{
		public readonly VariableEditorScopes scope;

		public readonly VariableEditorTypes type;

		public readonly int variableId;

		public readonly VariableEditorSetEquation equation;

		public readonly string setValue;

		private bool _setBool;

		private float _setFloat;

		private string _setString;

		public SetVariablePhase(VariableEditorScopes scope, VariableEditorTypes type, int variableId, VariableEditorSetEquation equation, string setValue, List<int> outs)
			: base(outs)
		{
			this.scope = scope;
			this.type = type;
			this.variableId = variableId;
			this.equation = equation;
			this.setValue = setValue;
		}

		protected override void onStart()
		{
			bool flag = false;
			switch (type)
			{
			case VariableEditorTypes.Boolean:
				flag = bool.TryParse(setValue, out _setBool);
				switch (equation)
				{
				case VariableEditorSetEquation.Equals:
					if (scope == VariableEditorScopes.Local)
					{
						_localVariables.booleans[variableId] = _setBool;
					}
					else
					{
						Dialoguer.SetGlobalBoolean(variableId, _setBool);
					}
					break;
				case VariableEditorSetEquation.Toggle:
					if (scope == VariableEditorScopes.Local)
					{
						_localVariables.booleans[variableId] = !_localVariables.booleans[variableId];
					}
					else
					{
						Dialoguer.SetGlobalBoolean(variableId, !Dialoguer.GetGlobalBoolean(variableId));
					}
					flag = true;
					break;
				}
				break;
			case VariableEditorTypes.Float:
				flag = float.TryParse(setValue, out _setFloat);
				switch (equation)
				{
				case VariableEditorSetEquation.Equals:
					if (scope == VariableEditorScopes.Local)
					{
						_localVariables.floats[variableId] = _setFloat;
					}
					else
					{
						Dialoguer.SetGlobalFloat(variableId, _setFloat);
					}
					break;
				case VariableEditorSetEquation.Add:
					if (scope == VariableEditorScopes.Local)
					{
						_localVariables.floats[variableId] += _setFloat;
					}
					else
					{
						Dialoguer.SetGlobalFloat(variableId, Dialoguer.GetGlobalFloat(variableId) + _setFloat);
					}
					break;
				case VariableEditorSetEquation.Subtract:
					if (scope == VariableEditorScopes.Local)
					{
						_localVariables.floats[variableId] -= _setFloat;
					}
					else
					{
						Dialoguer.SetGlobalFloat(variableId, Dialoguer.GetGlobalFloat(variableId) - _setFloat);
					}
					break;
				case VariableEditorSetEquation.Multiply:
					if (scope == VariableEditorScopes.Local)
					{
						_localVariables.floats[variableId] *= _setFloat;
					}
					else
					{
						Dialoguer.SetGlobalFloat(variableId, Dialoguer.GetGlobalFloat(variableId) * _setFloat);
					}
					break;
				case VariableEditorSetEquation.Divide:
					if (scope == VariableEditorScopes.Local)
					{
						_localVariables.floats[variableId] /= _setFloat;
					}
					else
					{
						Dialoguer.SetGlobalFloat(variableId, Dialoguer.GetGlobalFloat(variableId) / _setFloat);
					}
					break;
				}
				break;
			case VariableEditorTypes.String:
				flag = true;
				_setString = setValue;
				switch (equation)
				{
				case VariableEditorSetEquation.Equals:
					if (scope == VariableEditorScopes.Local)
					{
						_localVariables.strings[variableId] = _setString;
					}
					else
					{
						Dialoguer.SetGlobalString(variableId, _setString);
					}
					break;
				case VariableEditorSetEquation.Add:
					if (scope == VariableEditorScopes.Local)
					{
						_localVariables.strings[variableId] += _setString;
					}
					else
					{
						Dialoguer.SetGlobalString(variableId, Dialoguer.GetGlobalString(variableId) + _setString);
					}
					break;
				}
				break;
			}
			if (!flag)
			{
				Debug.LogWarning("[SetVariablePhase] Could not parse setValue");
			}
			Continue(0);
			base.state = PhaseState.Complete;
		}

		public override string ToString()
		{
			return "Set Variable Phase\nScope: " + scope.ToString() + "\nType: " + type.ToString() + "\nVariable ID: " + variableId + "\nEquation: " + equation.ToString() + "\nSet Value: " + setValue + "\nOut: " + outs[0] + "\n";
		}
	}
}
