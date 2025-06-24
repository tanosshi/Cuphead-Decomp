public class ClassStringAssembler
{
	private int indents;

	public string value = string.Empty;

	public ClassStringAssembler(int indent = 0)
	{
		indents = indent;
	}

	public void Add(string s)
	{
		value += s;
	}

	public void AddLine(string s)
	{
		int index = 0;
		if (s.Length > 0)
		{
			index = s.Length - 1;
		}
		if (s.Length > 0 && (s[0] == '}' || s[0] == ')' || s[0] == ']'))
		{
			indents--;
		}
		Add("\n" + PreIndent() + s);
		if (s.Length > 0 && (s[index] == '{' || s[index] == '(' || s[index] == '['))
		{
			indents++;
		}
	}

	public void Break()
	{
		value += "\n";
	}

	public void Indent()
	{
		indents++;
	}

	public void Undent()
	{
		indents--;
	}

	private string PreIndent()
	{
		string text = string.Empty;
		for (int i = 0; i < indents; i++)
		{
			text += "\t";
		}
		return text;
	}
}
