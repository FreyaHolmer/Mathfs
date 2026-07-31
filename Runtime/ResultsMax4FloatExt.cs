using System;

namespace Freya {

	public static class ResultsMax4FloatExt {
		public static ResultsMax4<float> InsertSorted( this ResultsMax4<float> r, float value ) {
			switch( r.count ) {
				case 0: return new ResultsMax4<float>( value );
				case 1:
					if( value < r.a )
						return new ResultsMax4<float>( value, r.a );
					return new ResultsMax4<float>( r.a, value );
				case 2:
					if( value < r.a )
						return new ResultsMax4<float>( value, r.a, r.b );
					if( value < r.b )
						return new ResultsMax4<float>( r.a, value, r.b );
					return new ResultsMax4<float>( r.a, r.b, value );
				case 3:
					if( value < r.a )
						return new ResultsMax4<float>( value, r.a, r.b, r.c );
					if( value < r.b )
						return new ResultsMax4<float>( r.a, value, r.b, r.c );
					if( value < r.c )
						return new ResultsMax4<float>( r.a, r.b, value, r.c );
					return new ResultsMax4<float>( r.a, r.b, r.c, value );
				default: throw new IndexOutOfRangeException( "Can't add more than four values to ResultsMax4" );
			}
		}

		public static ResultsMax3<float> InsertSorted( this ResultsMax3<float> r, float value ) {
			switch( r.count ) {
				case 0: return new ResultsMax3<float>( value );
				case 1:
					if( value < r.a )
						return new ResultsMax3<float>( value, r.a );
					return new ResultsMax3<float>( r.a, value );
				case 2:
					if( value < r.a )
						return new ResultsMax3<float>( value, r.a, r.b );
					if( value < r.b )
						return new ResultsMax3<float>( r.a, value, r.b );
					return new ResultsMax3<float>( r.a, r.b, value );
				default: throw new IndexOutOfRangeException( "Can't add more than three values to ResultsMax3" );
			}
		}
	}

}