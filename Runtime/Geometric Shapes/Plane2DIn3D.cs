using UnityEngine;

namespace Freya {

	/// <summary>An oriented 2D plane embedded in 3D space</summary>
	public struct Plane2DIn3D {

		public static readonly Plane2DIn3D XY = new(default, Vector3.right, Vector3.up);
		public static readonly Plane2DIn3D YZ = new(default, Vector3.up, Vector3.forward);
		public static readonly Plane2DIn3D ZX = new(default, Vector3.forward, Vector3.right);

		public Vector3 origin, axisX, axisY;

		/// <summary>Creates an oriented 2D plane embedded in 3D space</summary>
		/// <param name="origin">The origin of the plane</param>
		/// <param name="axisX">The x axis direction of the plane</param>
		/// <param name="axisY">The y axis direction of the plane</param>
		public Plane2DIn3D( Vector3 origin, Vector3 axisX, Vector3 axisY ) => ( this.origin, this.axisX, this.axisY ) = ( origin, axisX.normalized, axisY.normalized );

		/// <summary>Rotates this plane around the Y axis, setting the X axis,
		/// so that the given point <c>p</c> is in the plane where x > 0</summary>
		/// <param name="p">The point to include in the plane</param>
		/// <param name="pLocal">The included point in the 2D local space</param>
		public void RotateAroundYToInclude( Vector3 p, out Vector2 pLocal ) {
			Vector3 pRel = p - origin;
			float yProj = Vector3.Dot( axisY, pRel );
			axisX = ( pRel - axisY * yProj ).normalized;
			float xProj = Vector3.Dot( axisX, pRel );
			pLocal = new Vector2( xProj, yProj );
		}

		/// <summary>Transforms a local 2D point to a 3D world space point</summary>
		/// <param name="pt">The local space point to transform</param>
		public Vector3 TransformPoint( Vector2 pt ) {
			return new( // unrolled for performance
				origin.x + axisX.x * pt.x + axisY.x * pt.y,
				origin.y + axisX.y * pt.x + axisY.y * pt.y,
				origin.z + axisX.z * pt.x + axisY.z * pt.y
			);
		}

		/// <summary>Transforms a local 2D vector to a 3D world space vector, not taking position into account</summary>
		/// <param name="vec">The local space vector to transform</param>
		public Vector3 TransformVector( Vector2 vec ) {
			return new( // unrolled for performance
				axisX.x * vec.x + axisY.x * vec.y,
				axisX.y * vec.x + axisY.y * vec.y,
				axisX.z * vec.x + axisY.z * vec.y
			);
		}

		/// <summary>Transform a 3D world space point to a local 2D point</summary>
		/// <param name="pt">World space point</param>
		public Vector2 InverseTransformPoint( Vector3 pt ) {
			float rx = pt.x - origin.x;
			float ry = pt.y - origin.y;
			float rz = pt.z - origin.z;
			return new(
				axisX.x * rx + axisX.y * ry + axisX.z * rz,
				axisY.x * rx + axisY.y * ry + axisY.z * rz
			);
		}

		/// <summary>Transform a 3D world space vector to a local 2D vector</summary>
		/// <param name="vec">World space vector</param>
		public Vector2 InverseTransformVector( Vector3 vec ) {
			return new(
				axisX.x * vec.x + axisX.y * vec.y + axisX.z * vec.z,
				axisY.x * vec.x + axisY.y * vec.y + axisY.z * vec.z
			);
		}

	}

}