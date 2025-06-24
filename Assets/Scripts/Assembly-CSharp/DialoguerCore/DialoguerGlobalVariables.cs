using System;
using System.Collections.Generic;

namespace DialoguerCore
{
	[Serializable]
	public class DialoguerGlobalVariables
	{
		public List<bool> booleans;

		public List<float> floats;

		public List<string> strings;

		public DialoguerGlobalVariables()
		{
			booleans = new List<bool>();
			floats = new List<float>();
			strings = new List<string>();
		}

		public DialoguerGlobalVariables(List<bool> booleans, List<float> floats, List<string> strings)
		{
			this.booleans = booleans;
			this.floats = floats;
			this.strings = strings;
		}
	}
}
