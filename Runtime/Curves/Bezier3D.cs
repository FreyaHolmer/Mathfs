// by Freya Holmér (https://github.com/FreyaHolmer/Mathfs)
// a lot of stuff here made possible by this excellent writeup on bezier curves: https://pomax.github.io/bezierinfo/

using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Freya {

	/// <summary>A generalized 3D bezier curve, with an arbitrary number of control points. If you intend to only draw cubic bezier curves, consider using BezierCubic3D instead</summary>
	[Serializable] public class Bezier3D : IParamCurve<Vector3> {

		const MethodImplOptions INLINE = MethodImplOptions.AggressiveInlining;

		/// <inheritdoc cref="Bezier2D.points"/>
		public readonly Vector3[] points;

		/// <inheritdoc cref="Bezier2D(Vector2[])"/>
		public Bezier3D( Vector3[] points ) => this.points = points;

		/// <inheritdoc cref="Bezier2D.this[int]"/>
		public Vector3 this[ int i ] {
			[MethodImpl( INLINE )] get => points[i];
			[MethodImpl( INLINE )] set => points[i] = value;
		}

		#region Core IParamCurve Implementations

		public Vector3 GetStartPoint() => points[0];
		public Vector3 GetEndPoint() => points[Count - 1];

		public int Count {
			[MethodImpl( INLINE )] get => points.Length;
		}

		/// <summary>The degree of the curve, equal to the number of control points minus 1. 2 points = degree 1 (linear), 3 points = degree 2 (quadratic), 4 points = degree 3 (cubic)</summary>
		public int Degree {
			[MethodImpl( INLINE )] get => points.Length - 1;
		}

		public Vector3 GetPoint( float t ) {
			return B( Degree, 0 );

			Vector3 B( int k, int i ) {
				if( k == 0 ) return points[i];
				return Vector3.LerpUnclamped( B( k - 1, i ), B( k - 1, i + 1 ), t ); // todo: optimize
			}
		}

		#endregion

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