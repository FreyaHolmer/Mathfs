// by Freya Holmér (https://github.com/FreyaHolmer/Mathfs)

using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Freya {

	/// <summary>Similar to Unity's Ray2D, except this one allows you to not normalize the direction
	/// which saves performance as well as allows you to work at different scales</summary>
	[Serializable] public struct Ray2D : ILinear2D {

		const MethodImplOptions INLINE = MethodImplOptions.AggressiveInlining;

		/// <summary>The origin of the ray</summary>
		public Vector2 origin;

		/// <summary>The direction of the ray. Note: Ray2D allows non-normalized direction vectors</summary>
		public Vector2 dir;

		/// <summary>Returns a normalized version of this ray. Normalized rays ensure t-values correspond to distance</summary>
		public Line2D Normalized => new Line2D( origin, dir );

		/// <summary>Creates a 2D Ray. Note: direction does not have to be normalized, but if it is, the t-value will correspond to distance along the ray</summary>
		/// <param name="origin">The origin of the ray</param>
		/// <param name="dir">The direction of the ray. It does not have to be normalized, but if it is, the t-value when sampling will correspond to distance along the ray</param>
		public Ray2D( Vector2 origin, Vector2 dir ) => ( this.origin, this.dir ) = ( origin, dir );

		/// <summary>Implicitly casts a Unity ray to a Mathfs ray</summary>
		/// <param name="ray">The ray to cast to a Unity ray</param>
		public static implicit operator Ray2D( Ray ray ) => new Ray2D( ray.origin, ray.direction );

		/// <summary>Implicitly casts a Mathfs ray to a Unity ray</summary>
		/// <param name="ray">The ray to cast to a Mathfs ray</param>
		public static implicit operator UnityEngine.Ray2D( Ray2D ray ) => new UnityEngine.Ray2D( ray.origin, ray.dir );

		#region Internal interface stuff for generic line tests

		[MethodImpl( INLINE )] bool ILinear2D.IsValidTValue( float t ) => t >= 0;
		[MethodImpl( INLINE )] float ILinear2D.ClampTValue( float t ) => t < 0 ? 0 : t;
		Vector2 ILinear2D.Origin {
			[MethodImpl( INLINE )] get => origin;
		}
		Vector2 ILinear2D.Dir {
			[MethodImpl( INLINE )] get => dir;
		}

		#endregion

	}

}