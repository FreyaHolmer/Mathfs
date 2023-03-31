// by Freya Holmér (https://github.com/FreyaHolmer/Mathfs)

using System.Text;

namespace Freya {

	/// <summary>An integer range</summary>
	public readonly struct IntRange {

		public static readonly IntRange empty = new IntRange( 0, 0 );

		public readonly int start;
		public readonly int count;

		public int this[ int i ] => start + i;

		/// <summary>The last integer in the range</summary>
		public int Last => start + count - 1;

		/// <summary>The distance from first to last integer. Equivalent to <c>count-1</c></summary>
		public int Distance => count - 1;

		/// <summary>Creates a new integer range, given a start integer and how many integers to include in total</summary>
		/// <param name="start">The first integer</param>
		/// <param name="count">How many integers to include in the full range</param>
		public IntRange( int start, int count ) {
			this.start = start;
			this.count = count;
		}

		/// <summary>Whether or not this range contains a given value (inclusive)</summary>
		/// <param name="value">The value to check if it's inside, or equal to the start or end</param>
		public bool Contains( int value ) => value >= start && value <= Last;

		/// <summary>Create an integer range from start to end (inclusive)</summary>
		/// <param name="first">The first integer</param>
		/// <param name="last">The last integer</param>
		public static IntRange FirstToLast( int first, int last ) => new IntRange( first, last - first + 1 );

		static readonly StringBuilder toStrBuilder = new StringBuilder();

		public override string ToString() {
			toStrBuilder.Clear();
			toStrBuilder.Append( "{ " );
			int last = Last;
			for( int i = start; i <= last; i++ ) {
				toStrBuilder.Append( i );
				if( i != last )
					toStrBuilder.Append( ", " );
			}
			toStrBuilder.Append( " }" );
			return toStrBuilder.ToString();
		}

		public IntRangeEnumerator GetEnumerator() => new IntRangeEnumerator( this );

		public struct IntRangeEnumerator /*: IEnumerator<int>*/ {
			readonly IntRange intRange;
			int currValue;
			public IntRangeEnumerator( IntRange range ) => ( this.intRange, currValue ) = ( range, range.start - 1 );
			public bool MoveNext() => ++currValue <= intRange.Last;
			public int Current => currValue;
		}

	}

}