#region License
// Bamboo.Prevalence - a .NET object prevalence engine
// Copyright (C) 2002 Rodrigo B. de Oliveira
//
// Based on the original concept and implementation of Prevayler (TM)
// by Klaus Wuestefeld. Visit http://www.prevayler.org for details.
//
// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation; either version 2
// of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
//
// As a special exception, if you link this library with other files to
// produce an executable, this library does not by itself cause the
// resulting executable to be covered by the GNU General Public License.
// This exception does not however invalidate any other reasons why the
// executable file might be covered by the GNU General Public License.
//
// Contact Information
//
// http://bbooprevalence.sourceforge.net
// mailto:rodrigobamboo@users.sourceforge.net
#endregion

using System;
using System.Collections;
using System.Text;

namespace Bamboo.Prevalence.Indexing.FullText
{
	/// <summary>
	/// A collection of Posting objects.
	/// </summary>
	[Serializable]
	public class Postings : System.Collections.IEnumerable
	{
		Hashtable _postings;

		string _term;

		public Postings(string term)
		{
			_term = term;
			_postings = new Hashtable();
		}

		public string Term
		{
			get
			{
				return _term;
			}
		}

		public IRecord[] Records
		{
			get
			{
				IRecord[] records = new IRecord[_postings.Count];
				_postings.Keys.CopyTo(records, 0);
				return records;
			}
		}

		public void Add(IRecord record, IndexedField field, int termIndex)
		{
			Posting posting = _postings[record] as Posting;
			if (null == posting)
			{
				posting = new Posting(record);
				_postings[record] = posting;
			}
			posting.Occurrences.Add(field, termIndex);
		}

		public void Remove(IRecord record)
		{
			_postings.Remove(record);
		}

		public System.Collections.IEnumerator GetEnumerator()
		{
			return _postings.Values.GetEnumerator();
		}

		public override string ToString()
		{
			StringBuilder builder = new StringBuilder();
			builder.Append(_term);
			builder.Append(" => [");			
			foreach (Posting posting in _postings.Values)
			{
				builder.Append(posting.ToString());
				builder.Append(", ");
			}
			if (builder.Length > 1)
			{
				builder.Remove(builder.Length-2, 2);
			}
			builder.Append("]");
			return builder.ToString();
		}
	}
}
