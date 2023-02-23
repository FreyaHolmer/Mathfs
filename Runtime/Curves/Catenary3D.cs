// by Freya Holmér (https://github.com/FreyaHolmer/Mathfs)

using System;
using UnityEngine;

namespace Freya {

	/// <summary>A catenary curve passing through two points with a given an arc length</summary>
	public struct Catenary3D {

		enum Evaluability {
			NotReady,
			Ready
		}

		// data
		Vector3 p1;
		Catenary2D cat2D; // also stores arc length
		Plane2DIn3D plane; // also stores p0
		Evaluability evaluability;

		public float Length {
			get => cat2D.Length;
			set => cat2D.Length = value; // does not change evaluability of this type, since space hasn't changed
		}
		public Vector3 P0 {
			get => plane.origin;
			set {
				if( value != plane.origin )
					( plane.origin, evaluability ) = ( value, Evaluability.NotReady );
			}
		}
		public Vector3 P1 {
			get => p1;
			set {
				if( value != p1 )
					( p1, evaluability ) = ( value, Evaluability.NotReady );
			}
		}
		public Vector3 SlackDirection {
			get => -plane.axisY;
			set {
				if( value != SlackDirection )
					( plane.axisY, evaluability ) = ( -value, Evaluability.NotReady );
			}
		}

		/// <summary>Creates a catenary curve between two points, given an arc length <c>s</c> and a slack direction</summary>
		/// <param name="p0">The start of the curve</param>
		/// <param name="p1">The end of the curve</param>
		/// <param name="length">The length of the curve. note: has to be equal or longer than the distance between the points</param>
		/// <param name="slackDirection">The direction of "gravity" for the arc. (0,-1,0) would create a hanging chain like arc</param>
		public Catenary3D( Vector3 p0, Vector3 p1, float length, Vector3 slackDirection ) {
			cat2D = new Catenary2D( default, default, length );
			( plane.origin, plane.axisY, this.p1 ) = ( p0, -slackDirection, p1 );
			evaluability = Evaluability.NotReady;
			plane = default;
		}

		/// <summary>Creates a catenary curve between two points, given an arc length <c>s</c>, with slack/gravity direction pointing down</summary>
		/// <param name="p0">The start of the curve</param>
		/// <param name="p1">The end of the curve</param>
		/// <param name="length">The length of the curve. note: has to be equal or longer than the distance between the points</param>
		public Catenary3D( Vector3 p0, Vector3 p1, float length ) : this( p0, p1, length, Vector3.down ) {}

		/// <inheritdoc cref="Catenary2D.EvalByTValue"/>
		public Vector3 Eval( float t ) => EvalDerivativeByArcLength( t * Length, n: 0 );

		/// <inheritdoc cref="Catenary2D.Eval(float,int)"/>
		public Vector3 EvalByArcLength( float sEval ) => EvalDerivativeByArcLength( sEval, n: 0 );

		/// <inheritdoc cref="Catenary2D.Eval(float,int)"/>
		public Vector3 EvalDerivativeByArcLength( float sEval, int n = 1 ) {
			ReadyForEvaluation();
			return n switch {
				0 => plane.TransformPoint( cat2D.Eval( sEval, 0 ) ),
				_ => plane.TransformVector( cat2D.Eval( sEval, n ) )
			};
		}

		// calculates p, a, delta, arcLenSampleOffset, and which evaluation method to use
		void ReadyForEvaluation() {
			if( evaluability == Evaluability.Ready )
				return;
			// ready the embedded plane of the catenary and assign the 2D endpoint
			plane.RotateAroundYToInclude( P1, out Vector2 p1Local );
			cat2D.P1 = p1Local;
			evaluability = Evaluability.Ready;
		}

	}

}