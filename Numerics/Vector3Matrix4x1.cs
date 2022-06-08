// by Freya Holmér (https://github.com/FreyaHolmer/Mathfs)

using System;
using UnityEngine;

namespace Freya {

	/// <summary>A 4x1 column matrix with Vector3 values</summary>
	public readonly struct Vector3Matrix4x1 {

		public readonly Vector3 m0, m1, m2, m3;

		public Vector3Matrix4x1( Vector3 m0, Vector3 m1, Vector3 m2, Vector3 m3 ) => ( this.m0, this.m1, this.m2, this.m3 ) = ( m0, m1, m2, m3 );

		public Vector3Matrix4x1( Matrix4x1 x, Matrix4x1 y, Matrix4x1 z ) {
			m0 = new Vector3( x.m0, y.m0, z.m0 );
			m1 = new Vector3( x.m1, y.m1, z.m1 );
			m2 = new Vector3( x.m2, y.m2, z.m2 );
			m3 = new Vector3( x.m3, y.m3, z.m3 );
		}

		public Vector3 this[ int column ] =>
			column switch {
				0 => m0, 1 => m1, 2 => m2, 3 => m3,
				_ => throw new IndexOutOfRangeException( $"Matrix column index has to be from 0 to 3, got: {column}" )
			};

		public Matrix4x1 X => new(m0.x, m1.x, m2.x, m3.x);
		public Matrix4x1 Y => new(m0.y, m1.y, m2.y, m3.y);
		public Matrix4x1 Z => new(m0.z, m1.z, m2.z, m3.z);

	}

}