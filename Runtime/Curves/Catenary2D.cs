// by Freya Holmér (https://github.com/FreyaHolmer/Mathfs)

using UnityEngine;

namespace Freya {

	/// <summary>A catenary curve passing through two points with a given an arc length</summary>
	public struct Catenary2D {

		enum Evaluability {
			NotReady,
			Ready
		}

		// data
		Vector2 p1;
		CatenaryToPoint catenary; // stores arc length
		Transform2D space; // stores p0 and slack direction
		Evaluability evaluability;

		public float Length {
			get => catenary.Length;
			set => catenary.Length = value; // does not change evaluability of this type, since space hasn't changed
		}
		public Vector2 P0 {
			get => space.Origin;
			set {
				if( value != space.Origin )
					( space.Origin, evaluability ) = ( value, Evaluability.NotReady );
			}
		}
		public Vector2 P1 {
			get => p1;
			set {
				if( value != p1 )
					( p1, evaluability ) = ( value, Evaluability.NotReady );
			}
		}
		public Vector2 SlackDirection {
			get => -space.AxisY;
			set {
				if( value != SlackDirection )
					( space.AxisY, evaluability ) = ( -value, Evaluability.NotReady );
			}
		}

		/// <inheritdoc cref="Catenary3D(Vector3,Vector3,float,Vector3)"/>
		public Catenary2D( Vector2 p0, Vector2 p1, float length, Vector2 slackDirection ) {
			space = default;
			catenary = new CatenaryToPoint( p1 - p0, length );
			( space.Origin, this.p1 ) = ( p0, p1 );
			evaluability = Evaluability.NotReady;
		}

		/// <inheritdoc cref="CatenaryToPoint.Eval(float,int)"/>
		public Vector3 Eval( float sEval, int n = 1 ) {
			ReadyForEvaluation();
			return n switch {
				0 => space.TransformPoint( catenary.Eval( sEval, 0 ) ),
				_ => space.TransformVector( catenary.Eval( sEval, n ) )
			};
		}

		// ensures the space transformation is ready
		void ReadyForEvaluation() {
			if( evaluability == Evaluability.Ready )
				return;
			catenary.P = space.InverseTransformPoint( p1 );
			evaluability = Evaluability.Ready;
		}

	}

}