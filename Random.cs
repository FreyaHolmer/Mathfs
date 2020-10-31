// Collected and expanded upon to by Freya Holmér (https://github.com/FreyaHolmer/Mathfs)

using UnityEngine;
using static Freya.Mathfs;
using UnityRandom = UnityEngine.Random;

namespace Freya {

	// Random stuff (like, actually things of the category randomization, not, "various items")
	public static class Random {

		// 1D
		public static float Value => UnityRandom.value;
		public static float Sign => Value > 0.5f ? 1f : -1f;
		public static float Direction1D => Sign;
		public static float Range( float min, float max ) => UnityRandom.Range( min, max );

		// 2D
		public static Vector2 OnUnitCircle => AngToDir( Value * TAU );
		public static Vector2 Direction2D => OnUnitCircle;
		public static Vector2 InUnitCircle => UnityRandom.insideUnitCircle;
		public static Vector2 InUnitSquare => new Vector2( Value, Value );

		// 3D
		public static Vector3 OnUnitSphere => UnityRandom.onUnitSphere;
		public static Vector3 Direction3D => OnUnitSphere;
		public static Vector3 InUnitSphere => UnityRandom.insideUnitSphere;
		public static Vector3 InUnitCube => new Vector3( Value, Value, Value );

		// 2D orientation
		/// <summary>Returns a random angle in radians from 0 to TAU</summary>
		public static float Angle => Value * TAU;

		// 3D Orientation
		/// <summary>Returns a random uniformly distributed rotation</summary>
		public static Quaternion Rotation => UnityRandom.rotationUniform;
	}
}