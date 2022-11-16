// by Freya Holmér (https://github.com/FreyaHolmer/Mathfs)

using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using Plane = System.Numerics.Plane;

namespace Freya {

	/// <summary>Represents a line segment, similar to a line but with a defined start and end</summary>
	[Serializable] public struct LineSegment3D : ILinear3D {

		const MethodImplOptions INLINE = MethodImplOptions.AggressiveInlining;

		/// <summary>The start point of the line segment</summary>
		public Vector3 start;

		/// <summary>The end point of the line segment</summary>
		public Vector3 end;

		/// <summary>Creates a line segment with a defined start and end point</summary>
		/// <param name="start">The start point of the line segment</param>
		/// <param name="end">The end point of the line segment</param>
		public LineSegment3D( Vector3 start, Vector3 end ) => ( this.start, this.end ) = ( start, end );

		/// <summary>Returns the displacement vector from start to end of this line. Equivalent to <c>end-start</c></summary>
		public Vector3 Displacement {
			[MethodImpl( INLINE )] get => end - start;
		}

		/// <summary>Returns the normalized direction of this line. Equivalent to <c>(end-start).normalized</c></summary>
		public Vector3 Direction {
			[MethodImpl( INLINE )] get => Displacement.normalized;
		}

		/// <summary>Calculates the length of the line segment</summary>
		public float Length {
			[MethodImpl( INLINE )] get {
				float dx = end.x - start.x;
				float dy = end.y - start.y;
				float dz = end.z - start.z;
				return (float)Math.Sqrt( dx * dx + dy * dy + dz * dz );
			}
		}

		/// <summary>Calculates the length squared (faster than calculating the actual length)</summary>
		public float LengthSquared {
			[MethodImpl( INLINE )] get {
				float dx = end.x - start.x;
				float dy = end.y - start.y;
				float dz = end.z - start.z;
				return dx * dx + dy * dy + dz * dz;
			}
		}

		/// <summary>Returns the perpendicular bisector. Note: the returned normal is not normalized to save performance. Use <c>Bisector.Normalized</c> if you want to make sure it is normalized</summary>
		public Plane3D Bisector {
			[MethodImpl( INLINE )] get => GetBisector( start, end );
		}

		/// <summary>Returns the perpendicular bisector of the input line segment</summary>
		/// <param name="startPoint">Starting point of the line segment</param>
		/// <param name="endPoint">Endpoint of the line segment</param>
		[MethodImpl( INLINE )] public static Plane3D GetBisector( Vector3 startPoint, Vector3 endPoint ) => new Plane3D( ( endPoint - startPoint ).normalized, ( endPoint + startPoint ) / 2 );

		#region Interface stuff for generic line tests

		[MethodImpl( INLINE )] bool ILinear3D.IsValidTValue( float t ) => t >= 0 && t <= 1;
		[MethodImpl( INLINE )] float ILinear3D.ClampTValue( float t ) => t < 0 ? 0 : t > 1 ? 1 : t;
		Vector3 ILinear3D.Origin {
			[MethodImpl( INLINE )] get => start;
		}
		Vector3 ILinear3D.Dir {
			[MethodImpl( INLINE )] get => end - start;
		}

		#endregion

	}

}