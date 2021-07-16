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

	#region Vertices

	public partial struct Box2D {
		/// <summary>Returns a vertex of this box by index</summary>
		/// <param name="index">The index of the vertex to retrieve</param>
		public Vector2 GetVertex( int index ) {
			switch( index ) {
				case 0:  return new Vector2( center.x - extents.x, center.y - extents.y );
				case 1:  return new Vector2( center.x - extents.x, center.y + extents.y );
				case 2:  return new Vector2( center.x + extents.x, center.y - extents.y );
				case 3:  return new Vector2( center.x + extents.x, center.y + extents.y );
				default: throw new ArgumentOutOfRangeException( nameof(index), $"Invalid index: {index}. Valid vertex indices range from 0 to {VERTEX_COUNT - 1}" );
			}
		}
	}

	public partial struct Box3D {
		/// <inheritdoc cref="Box2D.GetVertex"/>
		public Vector2 GetVertex( int index ) {
			switch( index ) {
				case 0:  return new Vector3( center.x - extents.x, center.y - extents.y, center.z - extents.z );
				case 1:  return new Vector3( center.x - extents.x, center.y - extents.y, center.z + extents.z );
				case 2:  return new Vector3( center.x - extents.x, center.y + extents.y, center.z - extents.z );
				case 3:  return new Vector3( center.x - extents.x, center.y + extents.y, center.z + extents.z );
				case 4:  return new Vector3( center.x + extents.x, center.y - extents.y, center.z - extents.z );
				case 5:  return new Vector3( center.x + extents.x, center.y - extents.y, center.z + extents.z );
				case 6:  return new Vector3( center.x + extents.x, center.y + extents.y, center.z - extents.z );
				case 7:  return new Vector3( center.x + extents.x, center.y + extents.y, center.z + extents.z );
				default: throw new ArgumentOutOfRangeException( nameof(index), $"Invalid index: {index}. Valid vertex indices range from 0 to {VERTEX_COUNT - 1}" );
			}
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

	#region Contains

	public partial struct Box2D {
		/// <summary>Returns whether or not a point is inside this box</summary>
		/// <param name="point">The point to test if it's inside</param>
		[MethodImpl( INLINE )] public bool Contains( Vector2 point ) => Abs( point.x ) - extents.x <= 0 && Abs( point.y ) - extents.y <= 0;
	}

	public partial struct Box3D {
		/// <inheritdoc cref="Box2D.Contains"/>
		[MethodImpl( INLINE )] public bool Contains( Vector3 point ) => Abs( point.x ) - extents.x <= 0 && Abs( point.y ) - extents.y <= 0 && Abs( point.z ) - extents.z <= 0;
	}

	#endregion

	#region Encapsulate

	public partial struct Box2D {
		/// <summary>Extends the boundary of this box to encapsulate a point</summary>
		/// <param name="point">The point to encapsulate</param>
		public void Encapsulate( Vector2 point ) {
			float minX = Min( center.x - extents.x, point.x );
			float minY = Min( center.y - extents.y, point.y );
			float maxX = Max( center.x + extents.x, point.x );
			float maxY = Max( center.y + extents.y, point.y );
			center.x = ( maxX + minX ) / 2;
			center.y = ( maxY + minY ) / 2;
			extents.x = ( maxX - minX ) / 2;
			extents.y = ( maxY - minY ) / 2;
		}
	}

	public partial struct Box3D {
		/// <inheritdoc cref="Box2D.Encapsulate"/>
		public void Encapsulate( Vector3 point ) {
			float minX = Min( center.x - extents.x, point.x );
			float minY = Min( center.y - extents.y, point.y );
			float minZ = Min( center.z - extents.z, point.z );
			float maxX = Max( center.x + extents.x, point.x );
			float maxY = Max( center.y + extents.y, point.y );
			float maxZ = Max( center.z + extents.z, point.z );
			center.x = ( maxX + minX ) / 2;
			center.y = ( maxY + minY ) / 2;
			center.z = ( maxZ + minZ ) / 2;
			extents.x = ( maxX - minX ) / 2;
			extents.y = ( maxY - minY ) / 2;
			extents.z = ( maxZ - minZ ) / 2;
		}
	}

	#endregion
}