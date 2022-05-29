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

		readonly Vector3[] ptEvalBuffer;

		/// <inheritdoc cref="Bezier2D.Count"/>
		public int Count {
			[MethodImpl( INLINE )] get => points.Length;
		}

		/// <inheritdoc cref="Bezier2D(Vector2[])"/>
		public Bezier3D( Vector3[] points ) {
			this.points = points;
			if( points == null || points.Length <= 1 )
				throw new ArgumentException( "Bézier curves require at least two points" );
			ptEvalBuffer = new Vector3[points.Length - 1];
		}

		/// <inheritdoc cref="Bezier2D.this[int]"/>
		public Vector3 this[ int i ] {
			[MethodImpl( INLINE )] get => points[i];
			[MethodImpl( INLINE )] set => points[i] = value;
		}

		#region Core IParamCurve Implementations

		/// <inheritdoc cref="Bezier2D.Degree"/>
		public int Degree {
			[MethodImpl( INLINE )] get => points.Length - 1;
		}

		public Vector3 Eval( float t ) {
			float n = Count - 1;
			for( int i = 0; i < n; i++ )
				ptEvalBuffer[i] = Vector3.LerpUnclamped( points[i], points[i + 1], t );
			while( n > 1 ) {
				n--;
				for( int i = 0; i < n; i++ )
					ptEvalBuffer[i] = Vector3.LerpUnclamped( ptEvalBuffer[i], ptEvalBuffer[i + 1], t );
			}

			return ptEvalBuffer[0];
		}

		#endregion

		/// <inheritdoc cref="Bezier2D.Differentiate"/>
		public Bezier3D Differentiate() {
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