// by Freya Holmér (https://github.com/FreyaHolmer/Mathfs)

using System;
using UnityEngine;
using static Freya.Mathfs;
using UnityRandom = UnityEngine.Random;

namespace Freya {

	/// <summary>Various methods for generating random stuff (like, actually things of the category randomization, not, "various items")</summary>
	public static class Random {

		// 1D
		/// <summary>Returns a random value between 0 and 1</summary>
		public static float Value => UnityRandom.value;

		/// <summary>Randomly returns either -1 or 1</summary>
		public static float Sign => Value > 0.5f ? 1f : -1f;

		/// <summary>Randomly returns either -1 or 1, equivalent to <c>Random.Sign</c></summary>
		public static float Direction1D => Sign;

		/// <summary>Randomly returns a value between <c>min</c> [inclusive] and <c>max</c> [inclusive]</summary>
		/// <param name="min">The minimum value [inclusive] </param>
		/// <param name="max">The maximum value [inclusive]</param>
		public static float Range( float min, float max ) => UnityRandom.Range( min, max );

		/// <summary>Randomly returns a value between <c>min</c> [inclusive] and <c>max</c> [exclusive]</summary>
		/// <param name="min">The minimum value [inclusive]</param>
		/// <param name="max">The maximum value [exclusive]</param>
		public static int Range( int min, int max ) => UnityRandom.Range( min, max );

		// 2D
		/// <summary>Returns a random point on the unit circle</summary>
		public static Vector2 OnUnitCircle => AngToDir( Value * TAU );

		/// <summary>Returns a random 2D direction, equivalent to <c>OnUnitCircle</c></summary>
		public static Vector2 Direction2D => OnUnitCircle;

		/// <summary>Returns a random point inside the unit circle</summary>
		public static Vector2 InUnitCircle => UnityRandom.insideUnitCircle;

		/// <summary>Returns a random point inside the unit square. Values are between 0 to 1</summary>
		public static Vector2 InUnitSquare => new Vector2( Value, Value );

		// 3D
		/// <summary>Returns a random point on the unit sphere</summary>
		public static Vector3 OnUnitSphere => UnityRandom.onUnitSphere;

		/// <summary>Returns a random 3D direction, equivalent to <c>OnUnitSphere</c></summary>
		public static Vector3 Direction3D => OnUnitSphere;

		/// <summary>Returns a random point inside the unit sphere</summary>
		public static Vector3 InUnitSphere => UnityRandom.insideUnitSphere;

		/// <summary>Returns a random point inside the unit cube. Values are between 0 to 1</summary>
		public static Vector3 InUnitCube => new Vector3( Value, Value, Value );

		// 2D orientation
		/// <summary>Returns a random angle in radians from 0 to TAU</summary>
		public static float Angle => Value * TAU;

		// 3D Orientation
		/// <summary>Returns a random uniformly distributed rotation</summary>
		public static Quaternion Rotation => UnityRandom.rotationUniform;

		// Normal distributions
		/// <summary>Returns random numbers with a gaussian distribution</summary>
		/// <param name="variance">The distance away from the mean that will contain 68.27% of all values</param>
		/// <param name="mean">The mean (average) value of the distribution</param>
		public static float Gaussian( float variance = 1, float mean = 0 ) {
			float u1 = UnityEngine.Random.value;
			float u2 = UnityEngine.Random.value;
			return MathF.Sqrt( -2 * MathF.Log( u1 ) ) * MathF.Cos( TAU * u2 ) * variance + mean;
		}

		/// <summary>Samples a gaussian with rejection sampling, retrying when samples end up outside the given range</summary>
		/// <param name="variance">The distance away from the mean that will contain 68.27% of all values</param>
		/// <param name="mean">The mean (average) value of the distribution</param>
		/// <param name="min">The minimum value of the range</param>
		/// <param name="max">The maximum value of the range</param>
		/// <param name="samplingMaxAttempts">The maximum number of times to attempt to get a value within the range, before giving up and clamping instead</param>
		public static float GaussianInRange( float variance = 1, float mean = 0, float min = -1, float max = 1, int samplingMaxAttempts = 3 ) {
			float x;
			int attempts = 0;
			do {
				x = Gaussian( variance, mean );
				attempts++;
			} while( attempts < samplingMaxAttempts && x.Within( min, max ) == false );
			return x.Clamp( min, max );
		}

		/// <summary>Returns a value between -1 and 1 with a variance of 1/4</summary>
		public static float GaussianNeg1to1 => GaussianInRange( 1 / 4f );
		/// <summary>Returns a value between 0 and 1 with a variance of 1/8</summary>
		public static float Gaussian01 => GaussianInRange( 1 / 8f, 1 / 2f, 0, 1 );
	}

}