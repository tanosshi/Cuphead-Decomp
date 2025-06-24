using System;
using System.Collections.Generic;

[Serializable]
public class GitLevelProperties
{
	[Serializable]
	public class GitLevel
	{
		public string name;

		public string levelClassPath;

		public string levelObjectPath;
	}

	public const string UNITY_PATH = "/_CUPHEAD/_Generated/git_data.xml";

	public const string GIT_TOOLS_PATH = "Assets/_CUPHEAD/_Generated/git_data.xml";

	public List<GitLevel> levels;

	public GitLevelProperties()
	{
		levels = new List<GitLevel>();
	}
}
