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
using System.Text;

namespace Bamboo.Prevalence.Indexing.FullText
{
	/// <summary>
	/// 
	/// </summary>
	[Serializable]
	public class TermOccurrence
	{
		IndexedField _field;
		
		int[] _positions;
		
		public TermOccurrence(IndexedField field, int position)
		{
			_field = field;
			_positions = new int[] { position };
		}

		public IndexedField Field
		{
			get
			{
				return _field;
			}
		}
	
		public int[] Positions
		{
			get
			{
				return _positions;
			}
		}

		internal void Add(int position)
		{
			int[] newPositions = new int[_positions.Length + 1];
			Array.Copy(_positions, newPositions, _positions.Length);
			newPositions[_positions.Length] = position;
		}

		public override string ToString()
		{
			StringBuilder builder = new StringBuilder();
			builder.Append("<");
			builder.Append(_field.ToString());
			builder.Append(" => ");
			builder.Append("[");
			for (int i=0; i<_positions.Length; ++i)
			{
				builder.Append(_positions[i]);
				builder.Append(", ");
			}
			builder.Append("]");
			return builder.ToString();
		}
	}
}
