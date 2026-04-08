using System.Collections.Generic;

namespace Freya {

	public static class EnumerationExtensions {

		public static IEnumerable<(T a, T b)> Pairs<T>( this IEnumerable<T> items, bool loop ) {
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
			if( loop && hasFoundFirst )
				yield return ( prev, first );
		}

	}

}