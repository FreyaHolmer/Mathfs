// by Freya Holmér (https://github.com/FreyaHolmer/Mathfs)

using System;
using UnityEngine;

namespace Freya {

	/// <summary>A catenary curve passing through two points with a given an arc length</summary>
	public struct Catenary3D {

		// data
		Vector3 p0, p1;

		// cached state
		Catenary2D cat2D;

		public float Length {
			get => cat2D.Length;
			set => cat2D.Length = value;
		}
		public Vector3 P0 {
			get => p0;
			set {
				// todo
				throw new NotImplementedException();
			}
		}
		public Vector3 P1 {
			get => p1;
			set {
				// todo
				throw new NotImplementedException();
			}
		}

		/// <inheritdoc cref="Catenary2D(Vector2,Vector2,float)"/>
		public Catenary3D( Vector3 p0, Vector3 p1, float s ) {
			cat2D = new Catenary2D( default, default, s );
			( this.p0, this.p1 ) = ( p0, p1 );
		}

		public Vector3 TransformPoint( Vector2 pt ) {
			// 2D to 3D
			// todo
			throw new NotImplementedException();
		}

		public Vector3 TransformVector( Vector2 pt ) {
			// 2D to 3D
			// todo
			throw new NotImplementedException();
		}

		/// <inheritdoc cref="Catenary2D.Eval(float)"/>
		public Vector3 Eval( float t ) => EvalDerivativeByArcLength( t * Length, n: 0 );

		/// <inheritdoc cref="Catenary2D.EvalByArcLength(float)"/>
		public Vector3 EvalByArcLength( float sEval ) => EvalDerivativeByArcLength( sEval, n: 0 );

		/// <inheritdoc cref="Catenary2D.EvalDerivativeByArcLength(float,int)"/>
		public Vector3 EvalDerivativeByArcLength( float sEval, int n = 1 ) {
			return n switch {
				0 => TransformPoint( cat2D.EvalByArcLength( sEval ) ),
				_ => TransformVector( cat2D.EvalCatDerivByArcLength( sEval ) )
			};
		}

		// calculates p, a, delta, arcLenSampleOffset, and which evaluation method to use
		void ReadyForEvaluation() {
			// todo: space transform cache?
		}

	}

}