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

namespace Bamboo.Prevalence.Indexing
{
	#region SearchHit class
	/// <summary>
	/// A single item returned from a search.
	/// </summary>
	public class SearchHit
	{
		IRecord _record;

		/// <summary>
		/// creates a new object for the
		/// record passed as argument.
		/// </summary>
		/// <param name="record">a record</param>
		public SearchHit(IRecord record)
		{
			_record = record;
		}

		/// <summary>
		/// combines two search hits that refer
		/// to the same record. all the extended
		/// properties such as ranking and index specific
		/// information should be combined.
		/// </summary>
		/// <param name="other">the SearchHit that 
		/// should be combined to this one</param>
		public void Combine(SearchHit other)
		{
		}

		/// <summary>
		/// The record.
		/// </summary>
		public IRecord Record
		{
			get
			{
				return _record;
			}
		}

		/// <summary>
		/// Creates a clone from this object.
		/// </summary>
		/// <returns>a clone</returns>
		public SearchHit Clone()
		{
			return new SearchHit(_record);
		}
	}
	#endregion

	#region SearchResult class
	/// <summary>
	/// Accumulates the results of a search.
	/// </summary>
	public class SearchResult : IEnumerable
	{
		#region RecordFieldComparer (used by SortByField)
		/// <summary>
		/// IComparer implementation for IRecord fields.
		/// </summary>
		public class RecordFieldComparer : System.Collections.IComparer
		{
			string _field;

			/// <summary>
			/// Creates a new RecordFieldComparer for
			/// a specific field.
			/// </summary>
			/// <param name="field">field that should be used
			/// in comparisons</param>
			public RecordFieldComparer(string field)
			{
				_field = field;
			}

			#region Implementation of IComparer
			/// <summary>
			/// See <see cref="IComparer.Compare"/> for details.
			/// </summary>
			/// <param name="x"></param>
			/// <param name="y"></param>
			/// <returns></returns>
			public int Compare(object x, object y)
			{
				IRecord lhs = ((SearchHit)x).Record;
				IRecord rhs = ((SearchHit)y).Record;
				object lhsField = lhs[_field];
				object rhsField = rhs[_field];
				return ((IComparable)lhsField).CompareTo(rhsField);
			}
			#endregion
		}
		#endregion

		#region SearchHitRecordEnumerator (used by GetRecordEnumerator())
		/// <summary>
		/// Enumerates the records in a SearchResult.
		/// </summary>
		public class SearchHitRecordEnumerator : IEnumerator, IEnumerable
		{
			IEnumerator _hits;

			internal SearchHitRecordEnumerator(IEnumerator hits)
			{
				_hits = hits;
			}

			#region Implementation of IEnumerator

			/// <summary>
			/// See <see cref="IEnumerator.Reset"/> for details.
			/// </summary>
			public void Reset()
			{
				_hits.Reset();
			}

			/// <summary>
			/// See <see cref="IEnumerator.MoveNext"/> for details.
			/// </summary>
			/// <returns></returns>
			public bool MoveNext()
			{
				return _hits.MoveNext();
			}

			/// <summary>
			/// The current record.
			/// </summary>
			public IRecord Current
			{
				get
				{
					return ((SearchHit)_hits.Current).Record;
				}
			}

			object IEnumerator.Current
			{
				get
				{
					return ((SearchHit)_hits.Current).Record;
				}
			}
			#endregion

			#region Implementation of IEnumerable
			System.Collections.IEnumerator IEnumerable.GetEnumerator()
			{
				return this;
			}
			#endregion
		}
		#endregion

		long _elapsedTime;

		ArrayList _hits;
		
		/// <summary>
		/// Creates an empty SearchResult object.
		/// </summary>
		public SearchResult()
		{
			_hits = new ArrayList();
		}

		
		/// <summary>
		/// Number of items returned by the search.
		/// </summary>
		public int Count
		{
			get
			{
				return _hits.Count;
			}
		}


		/// <summary>
		/// Checks if the specified record was returned
		/// by the search.
		/// </summary>
		/// <param name="record">record to be checked</param>
		/// <returns>true if the record was indeed returned
		/// by the search</returns>
		/// <remarks>reference comparison is always used</remarks>
		public bool Contains(IRecord record)
		{
			return FindSearchHit(record) != null;
		}


		/// <summary>
		/// Adds a new item to the collection of items
		/// returned by the search. If the hit
		/// represents an existing record it
		/// will be combined to the existing hit instead.
		/// </summary>
		/// <param name="hit">the hit to be added or
		/// combined to a existing hit</param>
		public void Add(SearchHit hit)
		{
			SearchHit existing = FindSearchHit(hit.Record);
			if (null != existing)
			{
				existing.Combine(hit);
			}
			else
			{
				_hits.Add(hit);
			}
		}


		/// <summary>
		/// How long the search took
		/// </summary>
		public long ElapsedTime
		{
			get
			{
				return _elapsedTime;
			}

			set
			{
				_elapsedTime = value;
			}
		}


		/// <summary>
		/// Returns an item by its position
		/// </summary>
		public SearchHit this[int index]
		{
			get
			{
				return (SearchHit)_hits[index];
			}
		}


		/// <summary>
		/// Set intersection operation. Creates
		/// a new SearchResult with all the records
		/// that exist in both SearchResult objects.
		/// </summary>
		/// <param name="other"></param>
		/// <returns>a SearchResult representing the
		/// intersection between the this and other objects
		/// </returns>
		/// <remarks>all the SearchHit objects in
		/// the resulting SearchResult are clones from
		/// the original ones combined to the ones in
		/// other</remarks>
		public SearchResult Intersect(SearchResult other)
		{
			SearchResult result = new SearchResult();
			foreach (SearchHit hit in _hits)
			{
				SearchHit otherHit = other.FindSearchHit(hit.Record);
				if (null != otherHit)
				{
					SearchHit resultingHit = hit.Clone();
					resultingHit.Combine(otherHit);
					result.Add(resultingHit);
				}
			}
			return result;
		}

		/// <summary>
		/// Build a new SearchResult object including
		/// only those elements for which the 
		/// filter returns true.
		/// </summary>
		/// <param name="filter">filter</param>
		/// <returns>a new SearchResult containing all the elements for which 
		/// <see cref="ISearchHitFilter.Test"/> returned true</returns>
		public SearchResult Filter(ISearchHitFilter filter)
		{
			SearchResult result = new SearchResult();
			foreach (SearchHit hit in _hits)
			{
				if (filter.Test(hit))
				{
					result.Add(hit);
				}
			}			
			return result;
		}


		/// <summary>
		/// Sorts the result by a specific record field.
		/// </summary>
		/// <param name="field">the field to be used in the sort</param>
		public void SortByField(string field)
		{
			_hits.Sort(new RecordFieldComparer(field));
		}


		/// <summary>
		/// Copies all the records to an array. The
		/// order is mantained so that
		/// this[N].Record == resultingArray[N] is
		/// valid for every 0 &lt;= N &lt; this.Count.
		/// </summary>
		/// <param name="recordType">array element type</param>
		/// <returns>the resulting array.</returns>
		public object[] ToRecordArray(Type recordType)
		{			
			object[] records = (object[])Array.CreateInstance(recordType, _hits.Count);
			for (int i=0; i<records.Length; ++i)
			{				
				records[i] = _hits[i];
			}
			return records;
		}


		/// <summary>
		/// Returns an enumerator for all the records
		/// in this object.
		/// </summary>
		/// <returns></returns>
		public SearchHitRecordEnumerator GetRecordEnumerator()
		{
			return new SearchHitRecordEnumerator(_hits.GetEnumerator());
		}

		/// <summary>
		/// Finds a SearchHit that represents a specific
		/// record.
		/// </summary>
		/// <param name="record">the record to search for</param>
		/// <returns>the found SearchHit or null</returns>
		protected SearchHit FindSearchHit(IRecord record)
		{
			foreach (SearchHit hit in _hits)
			{
				if (hit.Record == record)
				{
					return hit;
				}
			}
			return null;
		}

		
		#region Implementation of IEnumerable
		/// <summary>
		/// See <see cref="IEnumerable.GetEnumerator"/> for details
		/// </summary>
		/// <returns></returns>
		public IEnumerator GetEnumerator()
		{
			return _hits.GetEnumerator();
		}
		#endregion
	}
	#endregion
}
