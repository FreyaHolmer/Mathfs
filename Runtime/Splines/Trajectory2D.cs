// by Freya Holmér (https://github.com/FreyaHolmer/Mathfs)

using UnityEngine;
using static Freya.Mathfs;

namespace Freya {

	// Trajectory math
	/// <summary>Various trajectory methods to calculate displacement, angles, max ranges, and more</summary>
	public struct Trajectory2D {

		public Vector2 position;
		public Vector2 velocity;
		public Vector2 acceleration;

		/// <summary>Constructs a trajectory using initial velocity</summary>
		/// <param name="position">The initial position at time 0</param>
		/// <param name="velocity">The initial velocity at time 0</param>
		/// <param name="accelerationOrGravityVector">The constant acceleration or gravity vector</param>
		public Trajectory2D( Vector2 position, Vector2 velocity, Vector2 accelerationOrGravityVector ) {
			this.position = position;
			this.velocity = velocity;
			this.acceleration = accelerationOrGravityVector;
		}

		/// <summary>Constructs a simple trajectory using angle, speed and the downwards gravitational constant</summary>
		/// <param name="position">The initial position at time 0</param>
		/// <param name="angle"></param>
		/// <param name="speed"></param>
		/// <param name="gravity">The downwards gravitational force constant. Positive values will gravitate downwards</param>
		public Trajectory2D( Vector2 position, float angle, float speed, float gravity ) {
			this.position = position;
			this.velocity = AngToDir( angle ) * speed;
			this.acceleration = new Vector2( 0, -gravity );
		}

		/// <summary>Returns the position at time t</summary>
		public Vector2 GetPosition( float t ) => position + t * velocity + 0.5f * t * t * acceleration;

		/// <summary>Returns the velocity at time t</summary>
		public Vector2 GetVelocity( float t ) => velocity + t * acceleration;

		/// <summary>Returns the acceleration, which is the same regardless of time</summary>
		public Vector2 GetAcceleration() => acceleration;

		public float TimeAtApex => -( velocity.y / acceleration.y );
		public Vector2 Apex => GetPosition( TimeAtApex );

		/// <summary>Returns the time when this trajectory hits the ground, assuming this trajectory starts at ground level</summary>
		public float GetLandingTime() => TimeAtApex * 2;

		/// <summary>Returns the landing position of this trajectory, assuming this trajectory starts at ground level</summary>
		public Vector2 GetLandingPosition() => new Vector2( position.x + GetLandingOffset(), position.y );

		/// <summary>Returns the horizontal displacement from the starting position this trajectory lands, assuming this trajectory starts and ends at ground level</summary>
		public float GetLandingOffset() => ( -2 * velocity.x * velocity.y ) / acceleration.y;

		/// <summary>Returns the two time values when passing by a specific height. Note that this may return either 0, 1 or 2 results. The time values may also be negative</summary>
		/// <param name="height">The height to get time values of</param>
		public ResultsMax2<float> GetTimesAtHeight( float height ) {
			float discriminant = velocity.y * velocity.y - 2 * acceleration.y * ( position.y - height );
			if( Approximately( discriminant, 0 ) ) {
				// 1 solution
				return new ResultsMax2<float>( TimeAtApex );
			} else if( discriminant < 0 ) {
				// 0 solutions
				return default;
			} else {
				// 2 solutions
				float sqrt = Sqrt( discriminant );
				float mvy = -velocity.y;
				return new ResultsMax2<float>(
					( mvy + sqrt ) / acceleration.y,
					( mvy - sqrt ) / acceleration.y
				);
			}
		}

		/// <summary>Returns either 0, 1 or 2 angles that would make a trajectory reach a given distance, when launched at the given speed</summary>
		/// <param name="xStart">The x coordinate of the start of this trajectory</param>
		/// <param name="speed">The initial speed of the trajectory</param>
		/// <param name="g">The downwards gravitational constant. Positive values means it's falling down</param>
		/// <param name="xTarget">The x coordinate of the target this trajectory should attempt to hit</param>
		public static ResultsMax2<float> GetAnglesToReachCoordinateAtSameHeight( float xStart, float speed, float g, float xTarget ) {
			float x = xTarget - xStart;
			float content = ( g * x ) / ( speed * speed );
			if( content.Within( -1, 1 ) ) {
				float angSteep = 0.5f * Acos( content ) + TAU / 8;
				float angShallow = Sign( x ) * TAU / 4 - angSteep;
				if( Approximately( angSteep, TAU / 8 ) ) {
					// 1 solution - both angles converged to the same angle of 45°
					return new ResultsMax2<float>( angSteep );
				} else {
					// 2 solutions
					return new ResultsMax2<float>(
						angShallow,
						angSteep
					);
				}
			} else {
				// 0 solutions
				return default;
			}
		}


		/// <summary>Returns either 0, 1 or 2 angles that would make a trajectory reach a given distance, when launched at the given speed</summary>
		/// <param name="startPosition">The start position of this trajectory</param>
		/// <param name="speed">The initial speed of the trajectory</param>
		/// <param name="g">The downwards gravitational constant. Positive values means it's falling down</param>
		/// <param name="target">The target coordinate this trajectory should attempt to hit</param>
		public static ResultsMax2<float> GetAnglesToReachCoordinate( Vector2 startPosition, float speed, float g, Vector2 target ) {
			float x = target.x - startPosition.x;
			float y = target.y - startPosition.y;
			float s2 = speed * speed;
			float s4 = s2 * s2;
			float discriminant = s4 - g * ( g * x * x + 2 * y * s2 );
			float flipOffset = x < 0 ? TAU / 2 : 0;

			if( Approximately( discriminant, 0 ) ) {
				// 1 solution
				return new ResultsMax2<float>( Atan( s2 / ( g * x ) ) + flipOffset );
			} else if( discriminant > 0 ) {
				// 2 solutions
				float sqrt = Sqrt( discriminant );
				float gx = g * x;
				return new ResultsMax2<float>(
					Atan( ( s2 + sqrt ) / gx ) + flipOffset,
					Atan( ( s2 - sqrt ) / gx ) + flipOffset
				);
			} else {
				// 0 solutions
				return default;
			}
		}


		/// <summary>Returns the position in a given trajectory, at the given time</summary>
		/// <param name="position">The initial position</param>
		/// <param name="velocity">The initial velocity</param>
		/// <param name="gravityOrAcceleration">The constant acceleration or gravity vector</param>
		/// <param name="time">The time to get the position at</param>
		public static Vector2 GetPosition( Vector2 position, Vector2 velocity, Vector2 gravityOrAcceleration, float time ) {
			return position + velocity * time + 0.5f * time * time * gravityOrAcceleration;
		}

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