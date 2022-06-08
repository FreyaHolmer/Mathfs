// by Freya Holmér (https://github.com/FreyaHolmer/Mathfs)

using System;
using UnityEngine;

namespace Freya {

	/// <summary>A 3x1 column matrix with Vector2 values</summary>
	public readonly struct Vector2Matrix3x1 {

		public readonly Vector2 m0, m1, m2;

		public Vector2Matrix3x1( Vector2 m0, Vector2 m1, Vector2 m2 ) => ( this.m0, this.m1, this.m2 ) = ( m0, m1, m2 );

		public Vector2Matrix3x1( Matrix3x1 x, Matrix3x1 y ) {
			m0 = new Vector2( x.m0, y.m0 );
			m1 = new Vector2( x.m1, y.m1 );
			m2 = new Vector2( x.m2, y.m2 );
		}

		public Vector2 this[ int column ] =>
			column switch {
				0 => m0, 1 => m1, 2 => m2,
				_ => throw new IndexOutOfRangeException( $"Matrix column index has to be from 0 to 3, got: {column}" )
			};

		public Matrix3x1 X => new(m0.x, m1.x, m2.x);
		public Matrix3x1 Y => new(m0.y, m1.y, m2.y);

	}

}