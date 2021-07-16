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

}