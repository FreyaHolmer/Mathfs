// by Freya Holmér (https://github.com/FreyaHolmer/Mathfs)

using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using static Freya.Mathfs;

namespace Freya {

	/// <summary>A 2D rectangular box</summary>
	[Serializable] public partial struct Box2D {

		const MethodImplOptions INLINE = MethodImplOptions.AggressiveInlining;

		/// <summary>The number of vertices in a rectangle</summary>
		public const int VERTEX_COUNT = 4;

		/// <summary>The center of this rectangle</summary>
		public Vector2 center;

		/// <summary>The extents of this rectangle (distance from the center to the edge) per axis</summary>
		public Vector2 extents;

	}

	/// <summary>A 3D rectangular box, also known as a cuboid</summary>
	[Serializable] public partial struct Box3D {

		const MethodImplOptions INLINE = MethodImplOptions.AggressiveInlining;

		/// <summary>The number of vertices in a cuboid</summary>
		public const int VERTEX_COUNT = 8;

		/// <summary>The center of this cuboid</summary>
		public Vector3 center;

		/// <summary>The extents of this cuboid (distance from the center to the edge) per axis</summary>
		public Vector3 extents;

	}

	#region Size

	public partial struct Box2D {
		/// <summary>The size of this box</summary>
		public Vector2 Size {
			[MethodImpl( INLINE )] get => extents * 2;
		}

		/// <summary>The width of this rectangle</summary>
		public float Width {
			[MethodImpl( INLINE )] get => extents.x * 2;
		}

		/// <summary>The height of this rectangle</summary>
		public float Height {
			[MethodImpl( INLINE )] get => extents.y * 2;
		}
	}

	public partial struct Box3D {
		/// <inheritdoc cref="Box2D.Size"/>
		public Vector3 Size {
			[MethodImpl( INLINE )] get => extents * 2;
		}

		/// <inheritdoc cref="Box2D.Width"/>
		public float Width {
			[MethodImpl( INLINE )] get => extents.x * 2;
		}

		/// <inheritdoc cref="Box2D.Height"/>
		public float Height {
			[MethodImpl( INLINE )] get => extents.y * 2;
		}

		/// <summary>The depth of this box</summary>
		public float Depth {
			[MethodImpl( INLINE )] get => extents.z * 2;
		}
	}

	#endregion

	#region Min/Max

	public partial struct Box2D {
		/// <summary>The minimum coordinates inside the box, per axis</summary>
		public Vector2 Min {
			[MethodImpl( INLINE )] get => new Vector2( center.x - extents.x, center.y - extents.y );
		}
		/// <summary>The maximum coordinates inside the box, per axis</summary>
		public Vector2 Max {
			[MethodImpl( INLINE )] get => new Vector2( center.x + extents.x, center.y + extents.y );
		}
	}

	public partial struct Box3D {
		/// <inheritdoc cref="Box2D.Min"/>
		public Vector3 Min {
			[MethodImpl( INLINE )] get => new Vector3( center.x - extents.x, center.y - extents.y, center.z - extents.z );
		}
		/// <inheritdoc cref="Box2D.Max"/>
		public Vector3 Max {
			[MethodImpl( INLINE )] get => new Vector3( center.x + extents.x, center.y + extents.y, center.z + extents.z );
		}
	}

	#endregion
	#region Volume

	public partial struct Box2D {
		/// <summary>The area of this rectangle</summary>
		public float Area {
			[MethodImpl( INLINE )] get => ( extents.x * extents.y ) * 4;
		}
	}

	public partial struct Box3D {
		/// <summary>The volume of this cuboid</summary>
		public float Volume {
			[MethodImpl( INLINE )] get => ( extents.x * extents.y * extents.z ) * 8;
		}
	}

	#endregion

	#region Surface

	public partial struct Box2D {
		/// <summary>The total perimeter length of this rectangle</summary>
		public float Perimeter {
			[MethodImpl( INLINE )] get => 4 * ( extents.x + extents.y );
		}
	}

	public partial struct Box3D {
		/// <summary>The total surface area of this cuboid</summary>
		public float SurfaceArea {
			[MethodImpl( INLINE )] get => 4 * ( extents.y * ( extents.x + extents.z ) + extents.z * extents.x );
		}
	}

	#endregion
}