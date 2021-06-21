// Collected and expanded upon to by Freya Holmér (https://github.com/FreyaHolmer/Mathfs)

using UnityEngine;
using static Freya.Mathfs;

namespace Freya {

	// Circle math
	public struct Circle3D {
		public Vector3 center;
		public Vector3 normal;
		public float radius;

		public Circle3D( Vector3 center, Vector3 normal, float radius ) {
			this.center = center;
			this.normal = normal;
			this.radius = radius;
		}

		public float Area => Circle.RadiusToArea( radius );
		public float Circumference => Circle.RadiusToCircumference( radius );

	}

}