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

		readonly Vector2[] ptEvalBuffer;

		/// <summary>The number of control points in this curve</summary>
		public int Count {
			[MethodImpl( INLINE )] get => points.Length;
		}

		/// <summary>Creates a general bezier curve, from any number of control points</summary>
		/// <param name="points">The control points of this curve</param>
		public Bezier2D( params Vector2[] points ) {
			this.points = points;
			if( points == null || points.Length <= 1 )
				throw new ArgumentException( "Bézier curves require at least two points" );
			ptEvalBuffer = new Vector2[points.Length - 1];
		}

		/// <summary>Get or set a control point position by index</summary>
		public Vector2 this[ int i ] {
			[MethodImpl( INLINE )] get => points[i];
			[MethodImpl( INLINE )] set => points[i] = value;
		}

		#region Core IParamCurve Implementations

		/// <summary>The degree of the curve, equal to the number of control points minus 1. 2 points = degree 1 (linear), 3 points = degree 2 (quadratic), 4 points = degree 3 (cubic)</summary>
		public int Degree {
			[MethodImpl( INLINE )] get => points.Length - 1;
		}

		public Vector2 Eval( float t ) {
			float n = Count - 1;
			for( int i = 0; i < n; i++ )
				ptEvalBuffer[i] = Vector2.LerpUnclamped( points[i], points[i + 1], t );
			while( n > 1 ) {
				n--;
				for( int i = 0; i < n; i++ )
					ptEvalBuffer[i] = Vector2.LerpUnclamped( ptEvalBuffer[i], ptEvalBuffer[i + 1], t );
			}

			return ptEvalBuffer[0];
		}

		#endregion

		/// <summary>Returns the derivative bezier curve if possible, otherwise returns null</summary>
		public Bezier2D Differentiate() {
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