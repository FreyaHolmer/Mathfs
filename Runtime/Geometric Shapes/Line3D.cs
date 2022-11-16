// by Freya Holmér (https://github.com/FreyaHolmer/Mathfs)

using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using static Freya.Mathfs;

namespace Freya {

	// 3D line math
	/// <summary>A structure representing an infinitely long 3D line</summary>
	[Serializable] public struct Line3D : ILinear3D {

		const MethodImplOptions INLINE = MethodImplOptions.AggressiveInlining;

		/// <summary>The origin of this line</summary>
		public Vector3 origin;

		/// <summary>The direction of the ray. Note: Line3D allows non-normalized direction vectors</summary>
		public Vector3 dir;

		/// <summary>Returns a normalized version of this line. Normalized lines ensure t-values correspond to distance</summary>
		public Line3D Normalized => new Line3D( origin, dir );

		/// <summary>Creates an infinitely long 3D line, given an origin and a direction</summary>
		/// <param name="origin">The origin of the line</param>
		/// <param name="dir">The direction of the line. It does not have to be normalized, but if it is, the t-value when sampling will correspond to distance along the ray</param>
		public Line3D( Vector3 origin, Vector3 dir ) => ( this.origin, this.dir ) = ( origin, dir );

		/// <summary>The signed distance from this line to a point. Points to the left of this line are positive</summary>
		/// <param name="point">The point to check the signed distance to</param>
		[MethodImpl( INLINE )] public float SignedDistance( Vector3 point ) => Determinant( dir.normalized, point - origin );

		#region Interface stuff for generic line tests

		[MethodImpl( INLINE )] bool ILinear3D.IsValidTValue( float t ) => true; // just always valid uwu
		[MethodImpl( INLINE )] float ILinear3D.ClampTValue( float t ) => t; // :)
		Vector3 ILinear3D.Origin {
			[MethodImpl( INLINE )] get => origin;
		}
		Vector3 ILinear3D.Dir {
			[MethodImpl( INLINE )] get => dir;
		}

		#endregion

		#region Statics (general linear 3D methods)

		/// <summary>Projects a point onto an infinite line, returning the t-value along the line</summary>
		/// <param name="lineOrigin">Line origin</param>
		/// <param name="lineDir">Line direction (does not have to be normalized)</param>
		/// <param name="point">The point to project onto the line</param>
		[MethodImpl( INLINE )] public static float ProjectPointToLineTValue( Vector3 lineOrigin, Vector3 lineDir, Vector3 point ) {
			return Vector3.Dot( lineDir, point - lineOrigin ) / Vector3.Dot( lineDir, lineDir );
		}

		/// <summary>Gets the t-values of the closest point between two infinite lines, returning the two t-values along the line</summary>
		/// <param name="aOrigin">Line A origin</param>
		/// <param name="aDir">Line A direction (does not have to be normalized)</param>
		/// <param name="bOrigin">Line B origin</param>
		/// <param name="bDir">Line B direction (does not have to be normalized)</param>
		[MethodImpl( INLINE )] public static (float tA, float tB) ClosestPointBetweenLinesTValues( Vector3 aOrigin, Vector3 aDir, Vector3 bOrigin, Vector3 bDir ) {
			// source: https://math.stackexchange.com/questions/2213165/find-shortest-distance-between-lines-in-3d
			Vector3 a = aOrigin;
			Vector3 b = aDir;
			Vector3 c = bOrigin;
			Vector3 d = bDir;
			Vector3 e = a - c;
			float be = Vector3.Dot( b, e );
			float de = Vector3.Dot( d, e ); 
			float bd = Vector3.Dot( b, d );
			float b2 = Vector3.Dot( b, b );
			float d2 = Vector3.Dot( d, d );
			float A = -b2 * d2 + bd * bd;

			float s = ( -b2 * de + be * bd ) / A;
			float t = ( d2 * be - de * bd ) / A;

			return ( t, s );

			// Vector3 n = Vector3.Cross( aDir, bDir );
			// float nMag = n.magnitude;
			// float dist = Vector3.Dot( n, aOrigin - bOrigin ) / nMag;
		}

		/// <summary>Projects a point onto an infinite line</summary>
		/// <param name="lineOrigin">Line origin</param>
		/// <param name="lineDir">Line direction (does not have to be normalized)</param>
		/// <param name="point">The point to project onto the line</param>
		[MethodImpl( INLINE )] public static Vector3 ProjectPointToLine( Vector3 lineOrigin, Vector3 lineDir, Vector3 point ) {
			return lineOrigin + lineDir * ProjectPointToLineTValue( lineOrigin, lineDir, point );
		}

		/// <summary>Projects a point onto an infinite line</summary>
		/// <param name="line">Line to project onto</param>
		/// <param name="point">The point to project onto the line</param>
		[MethodImpl( INLINE )] public static Vector3 ProjectPointToLine( Line3D line, Vector3 point ) => ProjectPointToLine( line.origin, line.dir, point );

		/// <summary>Returns the signed distance to a 3D plane</summary>
		/// <param name="planeOrigin">Plane origin</param>
		/// <param name="planeNormal">Plane normal (has to be normalized for a true distance)</param>
		/// <param name="point">The point to use when checking distance to the plane</param>
		[MethodImpl( INLINE )] public static float PointToPlaneSignedDistance( Vector3 planeOrigin, Vector3 planeNormal, Vector3 point ) {
			return Vector3.Dot( point - planeOrigin, planeNormal );
		}

		/// <summary>Returns the distance to a 3D plane</summary>
		/// <param name="planeOrigin">Plane origin</param>
		/// <param name="planeNormal">Plane normal (has to be normalized for a true distance)</param>
		/// <param name="point">The point to use when checking distance to the plane</param>
		[MethodImpl( INLINE )] public static float PointToPlaneDistance( Vector3 planeOrigin, Vector3 planeNormal, Vector3 point ) => Abs( PointToPlaneSignedDistance( planeOrigin, planeNormal, point ) );

		#endregion


	}

}