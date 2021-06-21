// Internal utility types, like non-allocating multi-return values.
// Lots of boring boilerplate in here~
// by Freya Holmér (https://github.com/FreyaHolmer/Mathfs)

using System;

namespace Freya {

	/// <summary>Contains either 0, 1, 2 or 3 valid return values</summary>
	public readonly struct ResultsMax3<T> where T : struct {

		/// <summary>The number of valid values</summary>
		public readonly int count;

		/// <summary>The first value. This may or may not be set/defined - use .count to see how many are valid</summary>
		public readonly T a;

		/// <summary>The second value. This may or may not be set/defined - use .count to see how many are valid</summary>
		public readonly T b;

		/// <summary>The third value. This may or may not be set/defined - use .count to see how many are valid</summary>
		public readonly T c;

		public ResultsMax3( T a, T b, T c ) {
			( this.a, this.b, this.c ) = ( a, b, c );
			count = 3;
		}

		public ResultsMax3( T a, T b ) {
			( this.a, this.b, this.c ) = ( a, b, default );
			count = 2;
		}

		public ResultsMax3( T a ) {
			( this.a, this.b, this.c ) = ( a, default, default );
			count = 1;
		}

		/// <summary>Returns the valid values at index i. Will throw an index out of range exception for invalid values. Use toghether with .count to ensure you don't get invalid values</summary>
		public T this[ int i ] {
			get {
				if( i >= 0 && i < count ) {
					switch( i ) {
						case 0: return a;
						case 1: return b;
						case 2: return c;
					}
				}

				throw new IndexOutOfRangeException();
			}
		}

		/// <summary>Returns a version of these results with one more element added to it. Note: this does not mutate the original struct</summary>
		/// <param name="value">The value to add</param>
		public ResultsMax3<T> Add( T value ) {
			switch( count ) {
				case 0: return new ResultsMax3<T>( value );
				case 1: return new ResultsMax3<T>( a, value );
				case 2: return new ResultsMax3<T>( a, b, value );
				default: throw new IndexOutOfRangeException("Can't add more than three values to ResultsMax3");
			}
		}

		public ResultsMax3<T> Where( Func<T, bool> validate ) {
			int validCount = 0;
			bool aIsValid = count > 0 && validate( a );
			if( aIsValid ) validCount++;
			bool bIsValid = count > 1 && validate( b );
			if( bIsValid ) validCount++;
			bool cIsValid = count > 2 && validate( c );
			if( cIsValid ) validCount++;

			switch( validCount ) {
				case 1:  return new ResultsMax3<T>( aIsValid ? a : bIsValid ? b : c );
				case 2:  return new ResultsMax3<T>( aIsValid ? a : b, bIsValid ? b : c );
				case 3:  return this; // all three passed, no change
				default: return default;
			}
		}

		public static implicit operator ResultsMax3<T>( (T?, T?, T?) tuple ) {
			int validCount = 0;
			(T? a, T? b, T? c) = tuple;
			bool aIsValid = a.HasValue;
			if( aIsValid ) validCount++;
			bool bIsValid = b.HasValue;
			if( bIsValid ) validCount++;
			bool cIsValid = c.HasValue;
			if( cIsValid ) validCount++;

			switch( validCount ) {
				case 1:  return new ResultsMax3<T>( aIsValid ? a.Value : bIsValid ? b.Value : c.Value );
				case 2:  return new ResultsMax3<T>( aIsValid ? a.Value : b.Value, bIsValid ? b.Value : c.Value );
				case 3:  return new ResultsMax3<T>( a.Value, b.Value, c.Value ); // all three passed, no change
				default: return default;
			}
		}

		public static implicit operator ResultsMax3<T>( T v ) => new ResultsMax3<T>( v );

		public static implicit operator ResultsMax3<T>( ResultsMax2<T> m2 ) {
			switch( m2.count ) {
				case 0: return default;
				case 1: return new ResultsMax3<T>( m2.a );
				case 2: return new ResultsMax3<T>( m2.a, m2.b );
			}

			throw new InvalidCastException( "Failed to cast ResultsMax2 to ResultsMax3" );
		}

	}

	/// <summary>Contains either 0, 1 or 2 valid return values</summary>
	public readonly struct ResultsMax2<T> where T : struct {

		/// <summary>The number of valid values</summary>
		public readonly int count;

		/// <summary>The first value. This may or may not be set/defined - use .count to see how many are valid</summary>
		public readonly T a;

		/// <summary>The second value. This may or may not be set/defined - use .count to see how many are valid</summary>
		public readonly T b;

		public ResultsMax2( T a, T b ) {
			( this.a, this.b ) = ( a, b );
			count = 2;
		}

		public ResultsMax2( T a ) {
			( this.a, this.b ) = ( a, default );
			count = 1;
		}

		/// <summary>Returns the valid values at index i. Will throw an index out of range exception for invalid values.
		/// Use toghether with .count to ensure you don't get invalid values</summary>
		public T this[ int i ] {
			get {
				if( i >= 0 && i < count ) {
					switch( i ) {
						case 0: return a;
						case 1: return b;
					}
				}

				throw new IndexOutOfRangeException();
			}
		}
		
		/// <summary>Returns a version of these results with one more element added to it. Note: this does not mutate the original struct</summary>
		/// <param name="value">The value to add</param>
		public ResultsMax2<T> Add( T value ) {
			switch( count ) {
				case 0:  return new ResultsMax2<T>( value );
				case 1:  return new ResultsMax2<T>( a, value );
				default: throw new IndexOutOfRangeException("Can't add more than two values to ResultsMax2");
			}
		}


	}

}