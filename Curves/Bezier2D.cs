// by Freya Holmér (https://github.com/FreyaHolmer/Mathfs)
// a lot of stuff here made possible by this excellent writeup on bezier curves: https://pomax.github.io/bezierinfo/

using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Freya {

	/// <summary>A generalized 2D bezier curve, with an arbitrary number of control points. If you intend to only draw cubic bezier curves, consider using BezierCubic2D instead</summary>
	[Serializable] public class Bezier2D : IParamCurve<Vector2> {

		const MethodImplOptions INLINE = MethodImplOptions.AggressiveInlining;

		/// <summary>The control points of the curve</summary>
		public readonly Vector2[] points;

		/// <summary>Creates a general bezier curve, from any number of control points</summary>
		/// <param name="points">The control points of this curve</param>
		public Bezier2D( Vector2[] points ) => this.points = points;

		/// <summary>Get or set a control point position by index</summary>
		public Vector2 this[ int i ] {
			[MethodImpl( INLINE )] get => points[i];
			[MethodImpl( INLINE )] set => points[i] = value;
		}

		#region Core IParamCurve Implementations

		public Vector2 GetStartPoint() => points[0];
		public Vector2 GetEndPoint() => points[Count - 1];

		public int Count {
			[MethodImpl( INLINE )] get => points.Length;
		}

		/// <summary>The degree of the curve, equal to the number of control points minus 1. 2 points = degree 1 (linear), 3 points = degree 2 (quadratic), 4 points = degree 3 (cubic)</summary>
		public int Degree {
			[MethodImpl( INLINE )] get => points.Length - 1;
		}

		public Vector2 GetPoint( float t ) {
			return B( Degree, 0 );

			Vector2 B( int k, int i ) {
				if( k == 0 ) return points[i];
				return Vector2.LerpUnclamped( B( k - 1, i ), B( k - 1, i + 1 ), t ); // todo: optimize
			}
		}

		#endregion

		/// <summary>Returns the derivative bezier curve if possible, otherwise returns null</summary>
		public Bezier2D GetDerivative() {
			int n = Count - 1;
			if( n == 0 )
				return null; // no derivative
			int d = Degree;
			Vector2[] deltaPts = new Vector2[n];
			for( int i = 0; i < n; i++ )
				deltaPts[i] = d * ( this[i + 1] - this[i] );
			return new Bezier2D( deltaPts );
		}

	}

}