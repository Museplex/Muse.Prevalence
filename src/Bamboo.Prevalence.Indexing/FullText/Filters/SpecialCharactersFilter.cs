using System;
using System.Globalization;
using System.Text;
using Bamboo.Prevalence.Indexing.FullText;

namespace Bamboo.Prevalence.Indexing.FullText.Filters
{
	/// <summary>
	/// A filter that replaces special characters by
	/// their ASCII counterparts.
	/// </summary>
	[Serializable]
	public class SpecialCharactersFilter : ITokenFilter 
	{
		public SpecialCharactersFilter()
		{
		}

		public Token Filter(ITokenizer tokenizer)
		{
			Token token = tokenizer.NextToken();
			if (null != token)
			{
				token.Value = Filter(token.Value);
			}
			return token;
		}

		string Filter(string value)
		{			
			char[] mapped = new char[value.Length];
			for (int i=0; i<value.Length; ++i)
			{
				char c = value[i];

				if (Char.IsUpper(c))
				{
					c = Char.ToLower(c);
				}

				switch (c)
				{
					case '�':
						c = 'a';
						break;

					case '�':
						c = 'e';
						break;

					case '�':
						c = 'i';
						break;

					case '�':
						c = 'o';
						break;

					case '�':
						c = 'u';
						break;

					case '�':
						c = 'a';
						break;

					case '�':
						c = 'o';
						break;

					case '�':
						c = 'a';
						break;

					case '�':
						c = 'e';
						break;

					case '�':
						c = 'o';
						break;

					case '�':
						c = 'a';
						break;

					case '�':
						c = 'c';
						break;
				}
				mapped[i] = c;
			}
			return new string(mapped);
		}
	}
}
