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

}