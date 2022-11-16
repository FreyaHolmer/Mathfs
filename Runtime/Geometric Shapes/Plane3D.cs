// by Freya Holmér (https://github.com/FreyaHolmer/Mathfs)

using UnityEngine;

namespace Freya {

	/// <summary>A mathematical plane in 3D space</summary>
	public struct Plane3D {

		/// <summary>The normal of the plane. Note that this type lets you assign non-normalized vectors</summary>
		public Vector3 normal;

		/// <summary>The signed distance from the world origin</summary>
		public float distance;

		public Vector3 PointClosestToOrigin => normal * distance;

		public Plane3D( Vector3 normal, float distance ) {
			this.normal = normal;
			this.distance = distance;
		}

		public Plane3D( Vector3 normal, Vector3 point ) {
			this.normal = normal;
			this.distance = Vector3.Dot( normal, point );
		}

		// public static Line3D Intersect( Plane3D a, Plane3D b ) {
		// 	return default;
		// }


	}

}