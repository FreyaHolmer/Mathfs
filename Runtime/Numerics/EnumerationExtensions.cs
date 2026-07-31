// by Freya Holmér (https://github.com/FreyaHolmer/Mathfs)

using System;
using System.Collections.Generic;
using System.Linq;

namespace Freya {

	public static class EnumerationExtensions {


		/// <summary>Enumerates each item pair as tuples</summary>
		/// <param name="items">The items to enumerate</param>
		/// <param name="cyclic">Whether to include a last pair formed by the last element and the first element</param>
		/// <returns>Given items <c>[a,b,c,d]</c>, this returns:
		/// <ul>
		/// <li><c>[(a,b),(b,c),(c,d)]</c> if <c>cyclic == false</c></li>
		/// <li><c>[(a,b),(b,c),(c,d),(d,a)]</c> if <c>cyclic == true</c></li>
		/// </ul></returns>
		public static IEnumerable<(T a, T b)> Pairs<T>( this IEnumerable<T> items, bool cyclic ) {
			bool hasFoundFirst = false;
			T first = default;
			T prev = default;
			foreach( T item in items ) {
				if( hasFoundFirst == false ) {
					hasFoundFirst = true;
					first = item;
				} else {
					yield return ( prev, item );
				}
				prev = item;
			}
			if( cyclic && hasFoundFirst )
				yield return ( prev, first );
		}

		/// <summary>A shorthand for selecting the out parameters of bool functions returning true</summary>
		/// <param name="items">The items to enumerate</param>
		/// <param name="predicate">Predicate selecting tuples with the boolean return value, and the out parameter value</param>
		/// <example><code>items.SelectOutParamsWhereTrue( x => (x.TryThing(out y), y) )</code></example>
		public static IEnumerable<O> SelectOutParamsWhereTrue<T, O>( this IEnumerable<T> items, Func<T, (bool, O)> predicate ) {
			foreach( T item in items ) {
				( bool valid, O value ) = predicate( item );
				if( valid )
					yield return value;
			}
		}

		/// <summary>Tries to select the minimum value. Returns <c>false</c> with a minimum value of <c>int.MaxValue</c> if there are no items</summary>
		/// <param name="items">The items to enumerate</param>
		/// <param name="predicate">The selector for the minimum value</param>
		/// <param name="minimum">The minimum value found</param>
		public static bool TryMin<T>( this IEnumerable<T> items, Func<T, int> predicate, out int minimum ) => items.Select( predicate ).TryMin( out minimum );

		/// <summary>Tries to select the minimum value. Returns <c>false</c> with a minimum value of <c>int.MaxValue</c> if there are no items</summary>
		/// <param name="items">The items to enumerate</param>
		/// <param name="minimum">The minimum value found</param>
		public static bool TryMin( this IEnumerable<int> items, out int minimum ) => ( minimum = items.Aggregate( int.MaxValue, Math.Min ) ) != int.MaxValue;

	}

}