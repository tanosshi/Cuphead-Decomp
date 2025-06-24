using System;
using System.Collections.Generic;

namespace DialoguerCore
{
	[Serializable]
	public class DialoguerVariables
	{
		public readonly List<bool> booleans;

		public readonly List<float> floats;

		public readonly List<string> strings;

		public DialoguerVariables(List<bool> booleans, List<float> floats, List<string> strings)
		{
			this.booleans = booleans;
			this.floats = floats;
			this.strings = strings;
		}

		public DialoguerVariables Clone()
		{
			List<bool> list = new List<bool>();
			for (int i = 0; i < booleans.Count; i++)
			{
				list.Add(booleans[i]);
			}
			List<float> list2 = new List<float>();
			for (int j = 0; j < floats.Count; j++)
			{
				list2.Add(floats[j]);
			}
			List<string> list3 = new List<string>();
			for (int k = 0; k < strings.Count; k++)
			{
				list3.Add(strings[k]);
			}
			return new DialoguerVariables(list, list2, list3);
		}
	}
}
