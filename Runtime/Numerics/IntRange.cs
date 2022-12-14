// by Freya Holmér (https://github.com/FreyaHolmer/Mathfs)

using System.Text;

namespace Freya {

	/// <summary>An integer range</summary>
	public readonly struct IntRange {
		public readonly int start;
		public readonly int count;

		/// <summary>The last integer in the range</summary>
		public int End => start + count - 1;

		/// <summary>The distance from first to last integer. Equivalent to <c>count-1</c></summary>
		public int Distance => count - 1;

		/// <summary>Creates a new integer range, given a start integer and how many more integers to include</summary>
		/// <param name="start">The first integer</param>
		/// <param name="count">How many integers to include in the range</param>
		public IntRange( int start, int count ) {
			this.start = start;
			this.count = count;
		}

		/// <summary>Whether or not this range contains a given value (inclusive)</summary>
		/// <param name="value">The value to check if it's inside, or equal to the start or end</param>
		public bool Contains( int value ) => value >= start && value <= End;

		/// <summary>Create an integer range from start to end (inclusive)</summary>
		/// <param name="start">The first integer</param>
		/// <param name="end">The last integer (inclusive)</param>
		public static IntRange StartEnd( int start, int end ) => new IntRange( start, end - start + 1 );
		
		static readonly StringBuilder toStrBuilder = new StringBuilder();
		public override string ToString() {
			toStrBuilder.Clear();
			toStrBuilder.Append( "{ " );
			int last = End;
			for( int i = start; i <= last; i++ ) {
				toStrBuilder.Append( i );
				if( i != last )
					toStrBuilder.Append( ", " );
			}
			toStrBuilder.Append( " }" );
			return toStrBuilder.ToString();
		}

	}

}