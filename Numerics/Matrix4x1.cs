// by Freya Holmér (https://github.com/FreyaHolmer/Mathfs)

using System;

/// <summary>A 4x1 column matrix with float values</summary>
public readonly struct Matrix4x1 {

	public readonly float m0, m1, m2, m3;

	public Matrix4x1( float m0, float m1, float m2, float m3 ) => ( this.m0, this.m1, this.m2, this.m3 ) = ( m0, m1, m2, m3 );

	public float this[ int column ] =>
		column switch {
			0 => m0, 1 => m1, 2 => m2, 3 => m3,
			_ => throw new IndexOutOfRangeException( $"Matrix column index has to be from 0 to 3, got: {column}" )
		};
		public static bool operator ==( Matrix4x1 a, Matrix4x1 b ) => a.m0 == b.m0 && a.m1 == b.m1 && a.m2 == b.m2 && a.m3 == b.m3;
		public static bool operator !=( Matrix4x1 a, Matrix4x1 b ) => !( a == b );
		public bool Equals( Matrix4x1 other ) => m0.Equals( other.m0 ) && m1.Equals( other.m1 ) && m2.Equals( other.m2 ) && m3.Equals( other.m3 );
		public override bool Equals( object obj ) => obj is Matrix4x1 other && Equals( other );
		public override int GetHashCode() => HashCode.Combine( m0, m1, m2, m3 );

	}

}