// by Freya Holmér (https://github.com/FreyaHolmer/Mathfs)

using System;

/// <summary>A 3x1 column matrix with float values</summary>
public readonly struct Matrix3x1 {

	public readonly float m0, m1, m2;

	public Matrix3x1( float m0, float m1, float m2 ) => ( this.m0, this.m1, this.m2 ) = ( m0, m1, m2 );

	public float this[ int column ] =>
		column switch {
			0 => m0, 1 => m1, 2 => m2,
			_ => throw new IndexOutOfRangeException( $"Matrix column index has to be from 0 to 3, got: {column}" )
		};

		public static bool operator ==( Matrix3x1 a, Matrix3x1 b ) => a.m0 == b.m0 && a.m1 == b.m1 && a.m2 == b.m2;
		public static bool operator !=( Matrix3x1 a, Matrix3x1 b ) => !( a == b );
		public bool Equals( Matrix3x1 other ) => m0.Equals( other.m0 ) && m1.Equals( other.m1 ) && m2.Equals( other.m2 );
		public override bool Equals( object obj ) => obj is Matrix3x1 other && Equals( other );
		public override int GetHashCode() => HashCode.Combine( m0, m1, m2 );

	}

}