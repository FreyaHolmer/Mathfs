// Internal utility types, like non-allocating multi-return values.
// Lots of boring boilerplate in here~
// by Freya Holmér (https://github.com/FreyaHolmer/Mathfs)

using System;
using System.Collections;
using System.Collections.Generic;

namespace Freya {

	/// <summary>Contains either 0, 1, 2 or 3 valid return values</summary>
	public readonly struct ResultsMax3<T> : IEnumerable<T> where T : struct {

		/// <summary>The number of valid values</summary>
		public readonly int count;

		/// <summary>The first value. This may or may not be set/defined - use .count to see how many are valid</summary>
		public readonly T a;

		/// <summary>The second value. This may or may not be set/defined - use .count to see how many are valid</summary>
		public readonly T b;

		/// <summary>The third value. This may or may not be set/defined - use .count to see how many are valid</summary>
		public readonly T c;

		/// <summary>Creates a result with three values</summary>
		/// <param name="a">The first value</param>
		/// <param name="b">The second value</param>
		/// <param name="c">The third value</param>
		public ResultsMax3( T a, T b, T c ) {
			( this.a, this.b, this.c ) = ( a, b, c );
			count = 3;
		}

		/// <summary>Creates a result with two values</summary>
		/// <param name="a">The first value</param>
		/// <param name="b">The second value</param>
		public ResultsMax3( T a, T b ) {
			( this.a, this.b, this.c ) = ( a, b, default );
			count = 2;
		}

		/// <summary>Creates a result with one value</summary>
		/// <param name="a">The one value</param>
		public ResultsMax3( T a ) {
			( this.a, this.b, this.c ) = ( a, default, default );
			count = 1;
		}

		/// <summary>Returns the valid values at index i. Will throw an index out of range exception for invalid values. Use toghether with .count to ensure you don't get invalid values</summary>
		/// <param name="i">The index of the result to get</param>
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
				case 0:  return new ResultsMax3<T>( value );
				case 1:  return new ResultsMax3<T>( a, value );
				case 2:  return new ResultsMax3<T>( a, b, value );
				default: throw new IndexOutOfRangeException( "Can't add more than three values to ResultsMax3" );
			}
		}

		/// <summary>Implicitly casts from a tuple with nullables to a results structure</summary>
		/// <param name="tuple">The tuple to cast</param>
		public static implicit operator ResultsMax3<T>( (T?, T?, T?) tuple ) {
			int validCount = 0;
			( T? a, T? b, T? c ) = tuple;
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

		/// <summary>Implicitly casts a value to a results structure</summary>
		/// <param name="v">The value to cast</param>
		public static implicit operator ResultsMax3<T>( T v ) => new ResultsMax3<T>( v );

		/// <summary>Implicitly casts ResultsMax2 to ResultsMax3</summary>
		/// <param name="m2">The results to cast</param>
		public static implicit operator ResultsMax3<T>( ResultsMax2<T> m2 ) {
			switch( m2.count ) {
				case 0: return default;
				case 1: return new ResultsMax3<T>( m2.a );
				case 2: return new ResultsMax3<T>( m2.a, m2.b );
			}

			throw new InvalidCastException( "Failed to cast ResultsMax2 to ResultsMax3" );
		}


		IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public struct ResultsMax3Enumerator : IEnumerator<T> {

			int currentIndex;
			readonly ResultsMax3<T> value;

			public ResultsMax3Enumerator( ResultsMax3<T> value ) {
				this.value = value;
				currentIndex = -1;
			}

			public bool MoveNext() => ++currentIndex < value.count;
			public void Reset() => currentIndex = -1;
			public T Current => value[currentIndex];
			object IEnumerator.Current => Current;
			public void Dispose() => _ = 0;
		}

		public ResultsMax3Enumerator GetEnumerator() => new ResultsMax3Enumerator( this );

	}

	/// <summary>Contains either 0, 1 or 2 valid return values</summary>
	public readonly struct ResultsMax2<T> : IEnumerable<T> where T : struct {

		/// <inheritdoc cref="Freya.ResultsMax3{T}.count"/>
		public readonly int count;

		/// <inheritdoc cref="Freya.ResultsMax3{T}.a"/>
		public readonly T a;

		/// <inheritdoc cref="Freya.ResultsMax3{T}.b"/>
		public readonly T b;

		/// <inheritdoc cref="Freya.ResultsMax3{T}(T,T)"/>
		public ResultsMax2( T a, T b ) {
			( this.a, this.b ) = ( a, b );
			count = 2;
		}

		/// <inheritdoc cref="Freya.ResultsMax3{T}(T)"/>
		public ResultsMax2( T a ) {
			( this.a, this.b ) = ( a, default );
			count = 1;
		}

		/// <inheritdoc cref="ResultsMax3{T}.this"/>
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

		/// <inheritdoc cref="Freya.ResultsMax3{T}.Add(T)"/>
		public ResultsMax2<T> Add( T value ) {
			switch( count ) {
				case 0:  return new ResultsMax2<T>( value );
				case 1:  return new ResultsMax2<T>( a, value );
				default: throw new IndexOutOfRangeException( "Can't add more than two values to ResultsMax2" );
			}
		}


		IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public struct ResultsMax2Enumerator : IEnumerator<T> {

			int currentIndex;
			readonly ResultsMax2<T> value;

			public ResultsMax2Enumerator( ResultsMax2<T> value ) {
				this.value = value;
				currentIndex = -1;
			}

			public bool MoveNext() => ++currentIndex < value.count;
			public void Reset() => currentIndex = -1;
			public T Current => value[currentIndex];
			object IEnumerator.Current => Current;
			public void Dispose() => _ = 0;
		}

		public ResultsMax2Enumerator GetEnumerator() => new ResultsMax2Enumerator( this );
	}

}