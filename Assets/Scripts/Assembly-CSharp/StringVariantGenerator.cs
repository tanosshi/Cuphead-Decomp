using System.Collections.Generic;
using UnityEngine;

public class StringVariantGenerator
{
	private class CharacterGenerator
	{
		private int currentIndex;

		private string variants;

		private List<string> randomPrefixes;

		private List<string> randomSuffixes;

		public CharacterGenerator(string variants, List<string> randomPrefixes = null, List<string> randomSuffixes = null)
		{
			this.variants = variants;
			this.randomPrefixes = randomPrefixes;
			this.randomSuffixes = randomSuffixes;
		}

		public void Init()
		{
			currentIndex = Random.Range(0, variants.Length);
		}

		public string generate()
		{
			currentIndex = (currentIndex + 1) % variants.Length;
			string text = variants[currentIndex].ToString();
			if (randomPrefixes != null)
			{
				text = randomPrefixes.RandomChoice() + text;
			}
			if (randomSuffixes != null)
			{
				text += randomSuffixes.RandomChoice();
			}
			return text;
		}
	}

	private static StringVariantGenerator _instance;

	private Dictionary<char, CharacterGenerator> characterGenerators;

	public static StringVariantGenerator Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new StringVariantGenerator();
			}
			return _instance;
		}
	}

	private StringVariantGenerator()
	{
		characterGenerators = new Dictionary<char, CharacterGenerator>
		{
			{
				'a',
				new CharacterGenerator("aA*")
			},
			{
				'b',
				new CharacterGenerator("bB(")
			},
			{
				'c',
				new CharacterGenerator("cC)")
			},
			{
				'd',
				new CharacterGenerator("dD")
			},
			{
				'e',
				new CharacterGenerator("eE&")
			},
			{
				'f',
				new CharacterGenerator("fF")
			},
			{
				'g',
				new CharacterGenerator("gG")
			},
			{
				'h',
				new CharacterGenerator("hH-")
			},
			{
				'i',
				new CharacterGenerator("iI")
			},
			{
				'j',
				new CharacterGenerator("jJ")
			},
			{
				'k',
				new CharacterGenerator("kK")
			},
			{
				'l',
				new CharacterGenerator("lL%")
			},
			{
				'm',
				new CharacterGenerator("mM")
			},
			{
				'n',
				new CharacterGenerator("nN^")
			},
			{
				'o',
				new CharacterGenerator("oO+")
			},
			{
				'p',
				new CharacterGenerator("pP")
			},
			{
				'q',
				new CharacterGenerator("qQ")
			},
			{
				'r',
				new CharacterGenerator("rR@")
			},
			{
				's',
				new CharacterGenerator("sS#")
			},
			{
				't',
				new CharacterGenerator("tT$")
			},
			{
				'u',
				new CharacterGenerator("uU")
			},
			{
				'v',
				new CharacterGenerator("vV")
			},
			{
				'w',
				new CharacterGenerator("wW")
			},
			{
				'x',
				new CharacterGenerator("xX")
			},
			{
				'y',
				new CharacterGenerator("yY")
			},
			{
				'z',
				new CharacterGenerator("zZ")
			},
			{
				'-',
				new CharacterGenerator(":;", new List<string> { "[" }, new List<string> { "[" })
			},
			{
				'!',
				new CharacterGenerator("!1", new List<string> { "[", "{", "[[", "{{" })
			},
			{
				'~',
				new CharacterGenerator("~`", new List<string> { "[", "{", "[[", "{{" })
			},
			{
				'\'',
				new CharacterGenerator("'\"", new List<string>
				{
					"[",
					"{",
					string.Empty
				}, new List<string>
				{
					"[",
					"{",
					string.Empty
				})
			},
			{
				'.',
				new CharacterGenerator(".>", new List<string> { "[", "{", "[[", "{{" })
			},
			{
				',',
				new CharacterGenerator(",<", new List<string>
				{
					"[",
					"{",
					string.Empty
				}, new List<string>
				{
					"[",
					"{",
					string.Empty
				})
			},
			{
				'?',
				new CharacterGenerator("?/", new List<string> { "[", "{", "[[", "{{" })
			},
			{
				' ',
				new CharacterGenerator("   ]  }")
			}
		};
	}

	public string Generate(string input)
	{
		foreach (CharacterGenerator value in characterGenerators.Values)
		{
			value.Init();
		}
		string text = string.Empty;
		bool flag = false;
		input = input.Replace(" -", "-");
		input = input.Replace("- ", "-");
		string text2 = input;
		foreach (char c in text2)
		{
			if (c == '<')
			{
				flag = true;
			}
			if (c == '>')
			{
				flag = false;
			}
			text = ((flag || !characterGenerators.ContainsKey(c)) ? (text + c) : (text + characterGenerators[c].generate()));
		}
		return text;
	}
}
