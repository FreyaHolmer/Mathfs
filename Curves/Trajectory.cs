// collected and expended upon by Freya Holmér (https://github.com/FreyaHolmer/Mathfs)

using UnityEngine;
using static Freya.Mathfs;

namespace Freya {

	// Trajectory math
	/// <summary>Various trajectory methods to calculate displacement, angles, max ranges, and more</summary>
	public static class Trajectory {

		/// <summary>Outputs the launch speed required to traverse a given lateral distance when launched at a given angle, if one exists</summary>
		/// <param name="gravity">Gravitational acceleration in meters per second</param>
		/// <param name="lateralDistance">Target lateral distance in meters</param>
		/// <param name="angle">Launch angle in radians (0 = flat)</param>
		/// <param name="speed">Launch speed in meters per second</param>
		/// <returns>Whether or not there is a valid launch speed</returns>
		public static bool TryGetLaunchSpeed( float gravity, float lateralDistance, float angle, out float speed ) {
			float num = lateralDistance * gravity;
			float den = Sin( 2 * angle );
			if( Abs( den ) < 0.00001f ) {
				speed = default;
				return false; // direction is parallel, no speed would get you there
			}

			float speedSquared = num / den;
			if( speedSquared < 0 ) {
				speed = 0;
				return false; // can't reach destination because you're going the wrong way
			}

			speed = Sqrt( speedSquared );
			return true;
		}

		/// <summary>Outputs the two launch angles given a lateral distance and launch speed, if they exist</summary>
		/// <param name="gravity">Gravitational acceleration in meters per second</param>
		/// <param name="lateralDistance">Target lateral distance in meters</param>
		/// <param name="speed">Launch speed in meters per second</param>
		/// <param name="angleLow">The low launch angle in radians</param>
		/// <param name="angleHigh">The high launch angle in radians</param>
		/// <returns>Whether or not valid launch angles exist</returns>
		public static bool TryGetLaunchAngles( float gravity, float lateralDistance, float speed, out float angleLow, out float angleHigh ) {
			if( speed == 0 ) {
				angleLow = angleHigh = default;
				return false; // can't reach anything without speed
			}

			float asinContent = ( lateralDistance * gravity ) / ( speed * speed );
			if( asinContent.Within( -1, 1 ) == false ) {
				angleLow = angleHigh = default;
				return false; // can't reach no matter what angle is used
			}

			angleLow = Asin( asinContent ) / 2;
			angleHigh = ( -angleLow + TAU / 4 );
			return true;
		}

		/// <summary>Returns the maximum lateral range a trajectory could reach, in meters, when launched at the optimal angle of 45°</summary>
		/// <param name="gravity">Gravitational acceleration in meters per second</param>
		/// <param name="speed">Launch speed in meters per second</param>
		public static float GetMaxRange( float gravity, float speed ) => speed * speed / gravity;

		/// <summary>Returns the displacement given a launch speed, launch angle and a traversal time</summary>
		/// <param name="gravity">Gravitational acceleration in meters per second</param>
		/// <param name="speed">Launch speed in meters per second</param>
		/// <param name="angle">Launch angle in radians (0 = flat)</param>
		/// <param name="time">Traversal time in seconds</param>
		/// <returns>Displacement, where x = lateral displacement and y = vertical displacement</returns>
		public static Vector2 GetDisplacement( float gravity, float speed, float angle, float time ) {
			float xDisp = speed * time * Cos( angle );
			float yDisp = speed * time * Sin( angle ) - .5f * gravity * time * time;
			return new Vector2( xDisp, yDisp );
		}

		/// <summary>Returns the maximum height that can possibly be reached if speed was redirected upwards, given a current height and speed</summary>
		/// <param name="gravity">Gravitational acceleration in meters per second</param>
		/// <param name="currentHeight">Current height in meters</param>
		/// <param name="speed">Launch speed in meters per second</param>
		/// <returns>Potential height in meters</returns>
		public static float GetHeightPotential( float gravity, float currentHeight, float speed ) {
			return currentHeight + ( speed * speed ) / ( 2 * -gravity );
		}

		/// <summary>Outputs the speed of an object with a given height potential and current height, if it exists</summary>
		/// <param name="gravity">Gravitational acceleration in meters per second</param>
		/// <param name="currentHeight">Current height in meters</param>
		/// <param name="heightPotential">Potential height in meters</param>
		/// <param name="speed">Speed in meters per second</param>
		/// <returns>Whether or not there is a valid speed</returns>
		public static bool TryGetSpeedFromHeightPotential( float gravity, float currentHeight, float heightPotential, out float speed ) {
			float speedSq = ( heightPotential - currentHeight ) * -2 * gravity;
			if( speedSq <= 0 ) {
				speed = default; // Imaginary speed :sparkles:
				return false;
			}

			speed = Sqrt( speedSq );
			return true;
		}

	}

}