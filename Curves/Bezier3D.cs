// by Freya Holmér (https://github.com/FreyaHolmer/Mathfs)
// a lot of stuff here made possible by this excellent writeup on bezier curves: https://pomax.github.io/bezierinfo/

using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Freya {

	/// <summary>A generalized 3D bezier curve, with an arbitrary number of control points. If you intend to only draw cubic bezier curves, consider using BezierCubic3D instead</summary>
	[Serializable] public class Bezier3D {

		const MethodImplOptions INLINE = MethodImplOptions.AggressiveInlining;

		/// <inheritdoc cref="Bezier2D.points"/>
		public readonly Vector3[] points;

		/// <inheritdoc cref="Bezier2D.Count"/>
		public int Count {
			[MethodImpl( INLINE )] get => points.Length;
		}

		/// <inheritdoc cref="Bezier2D.Degree"/>
		public int Degree {
			[MethodImpl( INLINE )] get => points.Length - 1;
		}

		/// <inheritdoc cref="Bezier2D(Vector2[])"/>
		public Bezier3D( Vector3[] points ) => this.points = points;

		/// <inheritdoc cref="Bezier2D.this[int]"/>
		public Vector3 this[ int i ] {
			[MethodImpl( INLINE )] get => points[i];
			[MethodImpl( INLINE )] set => points[i] = value;
		}

		/// <inheritdoc cref="Bezier2D.GetPoint(float)"/>
		public Vector3 GetPoint( float t ) {
			return B( Degree, 0 );

			Vector3 B( int k, int i ) {
				if( k == 0 ) return points[i];
				return Vector3.LerpUnclamped( B( k - 1, i ), B( k - 1, i + 1 ), t );
			}
		}

		/// <inheritdoc cref="Bezier2D.GetDerivative()"/>
		public Bezier3D GetDerivative() {
			int n = Count - 1;
			if( n == 0 )
				return null; // no derivative
			int d = Degree;
			Vector3[] deltaPts = new Vector3[n];
			for( int i = 0; i < n; i++ )
				deltaPts[i] = d * ( this[i + 1] - this[i] );
			return new Bezier3D( deltaPts );
		}
	}

}