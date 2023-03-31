// by Freya Holmér (https://github.com/FreyaHolmer/Mathfs)

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
		CatenaryToPoint cat2D; // also stores arc length
		Plane2DIn3D space; // stores p0 and slack direction
		Evaluability evaluability;

		public float Length {
			get => cat2D.Length;
			set => cat2D.Length = value; // does not change evaluability of this type, since space hasn't changed
		}
		public Vector3 P0 {
			get => space.origin;
			set {
				if( value != space.origin )
					( space.origin, evaluability ) = ( value, Evaluability.NotReady );
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
			get => -space.axisY;
			set {
				if( value != SlackDirection )
					( space.axisY, evaluability ) = ( -value, Evaluability.NotReady );
			}
		}

		/// <summary>Creates a catenary curve between two points, given an arc length <c>s</c> and a slack direction</summary>
		/// <param name="p0">The start of the curve</param>
		/// <param name="p1">The end of the curve</param>
		/// <param name="length">The length of the curve. note: has to be equal or longer than the distance between the points</param>
		/// <param name="slackDirection">The direction of "gravity" for the arc</param>
		public Catenary3D( Vector3 p0, Vector3 p1, float length, Vector3 slackDirection ) {
			cat2D = new CatenaryToPoint( p1 - p0, length );
			space.axisX = default; // set on first evaluation by RotateAroundYToInclude
			( space.origin, space.axisY, this.p1 ) = ( p0, -slackDirection, p1 );
			evaluability = Evaluability.NotReady;
		}

		/// <inheritdoc cref="CatenaryToPoint.Eval(float,int)"/>
		public Vector3 Eval( float sEval, int n = 1 ) {
			ReadyForEvaluation();
			return n switch {
				0 => space.TransformPoint( cat2D.Eval( sEval, 0 ) ),
				_ => space.TransformVector( cat2D.Eval( sEval, n ) )
			};
		}

		// ensures the space transformation is ready
		void ReadyForEvaluation() {
			if( evaluability == Evaluability.Ready )
				return;
			// ready the embedded plane of the catenary and assign the 2D endpoint
			space.RotateAroundYToInclude( P1, out Vector2 p1Local );
			cat2D.P = p1Local;
			evaluability = Evaluability.Ready;
		}

	}

}