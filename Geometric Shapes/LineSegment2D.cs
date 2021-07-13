// by Freya Holmér (https://github.com/FreyaHolmer/Mathfs)

using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Freya {

	/// <summary>Represents a line segment, similar to a line but with a defined start and end</summary>
	[Serializable] public struct LineSegment2D : ILinear2D {

		const MethodImplOptions INLINE = MethodImplOptions.AggressiveInlining;

		/// <summary>The start point of the line segment</summary>
		public Vector2 start;

		/// <summary>The end point of the line segment</summary>
		public Vector2 end;

		/// <summary>Creates a line segment with a defined start and end point</summary>
		/// <param name="start">The start point of the line segment</param>
		/// <param name="end">The end point of the line segment</param>
		public LineSegment2D( Vector2 start, Vector2 end ) => ( this.start, this.end ) = ( start, end );

		/// <summary>Returns the displacement vector from start to end of this line. Equivalent to <c>end-start</c></summary>
		public Vector2 Displacement {
			[MethodImpl( INLINE )] get => end - start;
		}

		/// <summary>Returns the normalized direction of this line. Equivalent to <c>(end-start).normalized</c></summary>
		public Vector2 Direction {
			[MethodImpl( INLINE )] get => Displacement.normalized;
		}

		/// <summary>Calculates the length of the line segment</summary>
		public float Length {
			[MethodImpl( INLINE )] get {
				float dx = end.x - start.x;
				float dy = end.y - start.y;
				return (float)Math.Sqrt( dx * dx + dy * dy );
			}
		}

		/// <summary>Calculates the length squared (faster than calculating the actual length)</summary>
		public float LengthSquared {
			[MethodImpl( INLINE )] get {
				float dx = end.x - start.x;
				float dy = end.y - start.y;
				return dx * dx + dy * dy;
			}
		}

		/// <summary>Returns the perpendicular bisector. Note: the returned normal is not normalized to save performance. Use <c>Bisector.Normalized</c> if you want to make sure it is normalized</summary>
		public Line2D Bisector {
			[MethodImpl( INLINE )] get => GetBisector( start, end );
		}

		/// <summary>Returns the perpendicular bisector of the input line segment. Note: the returned line is not normalized to save performance. Use <c>GetBisector().Normalized</c> if you want to make sure it is normalized</summary>
		/// <param name="startPoint">Starting point of the line segment</param>
		/// <param name="endPoint">Endpoint of the line segment</param>
		[MethodImpl( INLINE )] public static Line2D GetBisector( Vector2 startPoint, Vector2 endPoint ) => new Line2D( ( startPoint + endPoint ) * 0.5f, ( startPoint - endPoint ).Rotate90CCW() );

		#region Interface stuff for generic line tests

		[MethodImpl( INLINE )] bool ILinear2D.IsValidTValue( float t ) => t >= 0 && t <= 1;
		[MethodImpl( INLINE )] float ILinear2D.ClampTValue( float t ) => t < 0 ? 0 : t > 1 ? 1 : t;
		Vector2 ILinear2D.Origin {
			[MethodImpl( INLINE )] get => start;
		}
		Vector2 ILinear2D.Dir {
			[MethodImpl( INLINE )] get => end - start;
		}

		#endregion

	}

}