using System;
using System.Collections.Generic;

namespace TMPro
{
	[Serializable]
	public class LineBreakingTable
	{
		public Dictionary<int, char> leadingCharacters;

		public Dictionary<int, char> followingCharacters;

		public LineBreakingTable()
		{
			leadingCharacters = new Dictionary<int, char>();
			followingCharacters = new Dictionary<int, char>();
		}
	}
}
