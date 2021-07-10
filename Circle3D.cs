// by Freya Holmér (https://github.com/FreyaHolmer/Mathfs)

using System.Runtime.CompilerServices;
using UnityEngine;

namespace Freya {

	/// <summary>A 3D circle with a centerpoint, radius, and a normal/axis</summary>
	public struct Circle3D {

		const MethodImplOptions INLINE = MethodImplOptions.AggressiveInlining;

		/// <summary>The center of the circle</summary>
		public Vector3 center;

		/// <summary>The normal/axis of the circle</summary>
		public Vector3 normal;

		/// <summary>The radius of the circle</summary>
		public float radius;

		/// <summary>Creates a 3D Circle</summary>
		/// <param name="center">The center of the circle</param>
		/// <param name="normal">The normal/axis of the circle</param>
		/// <param name="radius">The radius of the circle</param>
		public Circle3D( Vector3 center, Vector3 normal, float radius ) {
			this.center = center;
			this.normal = normal;
			this.radius = radius;
		}

		/// <summary>Get or set the area of this circle</summary>
		public float Area {
			[MethodImpl( INLINE )] get => Circle2D.RadiusToArea( radius );
			[MethodImpl( INLINE )] set => radius = Circle2D.AreaToRadius( value );
		}

		/// <summary>Get or set the circumference of this circle</summary>
		public float Circumference {
			[MethodImpl( INLINE )] get => Circle2D.RadiusToCircumference( radius );
			[MethodImpl( INLINE )] set => radius = Circle2D.CircumferenceToRadius( value );
		}

	}

}