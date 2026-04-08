using Unity.Mathematics;

namespace Freya {

	public interface IVec2<V, C, D, W, M> : IVec1<V, C, D, W, M>, IQuadrant2D, IComplex<V, M> {
		/// <summary>The Y component of this vector</summary>
		public C Y { get; }
		/// <summary>This vector with a reversed Y component</summary>
		public V flipY { get; }
		/// <summary>This vector with a zeroed-out Y component</summary>
		public V zeroY { get; }

		/// <summary>Rotates this vector in the positive rotation direction by 90 degrees. This is usually a counter-clockwise/left turn</summary>
		public V rot90 { get; }
		/// <summary>Rotates this vector in the negative rotation direction by 90 degrees. This is usually a clockwise/right turn</summary>
		public V rotNeg90 { get; }
		/// <summary>Rotates this vector by 180 degrees. Equivalent to negating this vector</summary>
		public V rot180 { get; }

		// public V rot45chebyshev { get; }
		// public V FromVector2( Vector2 v ); // should only happen for coarse things like inthalf2 and int. rational ones are messy here
	}

	public static partial class mathfs {
		/// <inheritdoc cref="IVec2{V, C, D, W, M}.rot90" />
		public static V rot90<V, C, D, W, M>( V v ) where V : IVec2<V, C, D, W, M> => v.rot90;

		/// <inheritdoc cref="IVec2{V, C, D, W, M}.rot90" />
		public static rat2 rot90( rat2 v ) => v.rot90;

		/// <inheritdoc cref="IVec2{V, C, D, W, M}.rot90" />
		public static inth2 rot90( inth2 v ) => v.rot90;

		/// <inheritdoc cref="IVec2{V, C, D, W, M}.rot90" />
		public static int2 rot90( this int2 v ) => new(-v.y, v.x);

		/// <inheritdoc cref="IVec2{V, C, D, W, M}.rot90" />
		public static float2 rot90( this float2 v ) => new(-v.y, v.x);

		/// <inheritdoc cref="IVec2{V, C, D, W, M}.rot90" />
		public static double2 rot90( this double2 v ) => new(-v.y, v.x);


		/// <inheritdoc cref="IVec2{V, C, D, W, M}.rotNeg90" />
		public static V rotNeg90<V, C, D, W, M>( V v ) where V : IVec2<V, C, D, W, M> => v.rotNeg90;

		/// <inheritdoc cref="IVec2{V, C, D, W, M}.rotNeg90" />
		public static rat2 rotNeg90( rat2 v ) => v.rotNeg90;

		/// <inheritdoc cref="IVec2{V, C, D, W, M}.rotNeg90" />
		public static inth2 rotNeg90( inth2 v ) => v.rotNeg90;

		/// <inheritdoc cref="IVec2{V, C, D, W, M}.rotNeg90" />
		public static int2 rotNeg90( this int2 v ) => new(v.y, -v.x);

		/// <inheritdoc cref="IVec2{V, C, D, W, M}.rotNeg90" />
		public static float2 rotNeg90( this float2 v ) => new(v.y, -v.x);

		/// <inheritdoc cref="IVec2{V, C, D, W, M}.rotNeg90" />
		public static double2 rotNeg90( this double2 v ) => new(v.y, -v.x);


		/// <inheritdoc cref="IVec2{V, C, D, W, M}.rot180" />
		public static V rot180<V, C, D, W, M>( V v ) where V : IVec2<V, C, D, W, M> => v.rot180;

		/// <inheritdoc cref="IVec2{V, C, D, W, M}.rot180" />
		public static rat2 rot180( rat2 v ) => -v;

		/// <inheritdoc cref="IVec2{V, C, D, W, M}.rot180" />
		public static inth2 rot180( inth2 v ) => -v;

		/// <inheritdoc cref="IVec2{V, C, D, W, M}.rot180" />
		public static int2 rot180( this int2 v ) => -v;

		/// <inheritdoc cref="IVec2{V, C, D, W, M}.rot180" />
		public static float2 rot180( this float2 v ) => -v;

		/// <inheritdoc cref="IVec2{V, C, D, W, M}.rot180" />
		public static double2 rot180( this double2 v ) => -v;

	}

	// Y component boilerplate
	public static partial class mathfs {
		/// <inheritdoc cref="IVec2{V, C, D, W, M}.Y" />
		public static C Y<V, C, D, W, M>( V v ) where V : IVec2<V, C, D, W, M> => v.Y;

		/// <inheritdoc cref="IVec2{V, C, D, W, M}.Y" />
		public static rat Y( rat2 v ) => v.Y;

		/// <inheritdoc cref="IVec2{V, C, D, W, M}.Y" />
		public static inth Y( inth2 v ) => v.Y;

		/// <inheritdoc cref="IVec2{V, C, D, W, M}.Y" />
		public static int Y( this int2 v ) => v.y;

		/// <inheritdoc cref="IVec2{V, C, D, W, M}.Y" />
		public static float Y( this float2 v ) => v.y;

		/// <inheritdoc cref="IVec2{V, C, D, W, M}.Y" />
		public static double Y( this double2 v ) => v.y;

		/// <inheritdoc cref="IVec2{V, C, D, W, M}.Y" />
		public static int Y( this int3 v ) => v.y;

		/// <inheritdoc cref="IVec2{V, C, D, W, M}.Y" />
		public static float Y( this float3 v ) => v.y;

		/// <inheritdoc cref="IVec2{V, C, D, W, M}.Y" />
		public static double Y( this double3 v ) => v.y;

		/// <inheritdoc cref="IVec2{V, C, D, W, M}.Y" />
		public static int Y( this int4 v ) => v.y;

		/// <inheritdoc cref="IVec2{V, C, D, W, M}.Y" />
		public static float Y( this float4 v ) => v.y;

		/// <inheritdoc cref="IVec2{V, C, D, W, M}.Y" />
		public static double Y( this double4 v ) => v.y;


		/// <inheritdoc cref="IVec2{V, C, D, W, M}.flipY" />
		public static V flipY<V, C, D, W, M>( V v ) where V : IVec2<V, C, D, W, M> => v.flipY;

		/// <inheritdoc cref="IVec2{V, C, D, W, M}.flipY" />
		public static rat2 flipY( rat2 v ) => v.flipY;

		/// <inheritdoc cref="IVec2{V, C, D, W, M}.flipY" />
		public static inth2 flipY( inth2 v ) => v.flipY;

		/// <inheritdoc cref="IVec2{V, C, D, W, M}.flipY" />
		public static int2 flipY( this int2 v ) => new(v.x, -v.y);

		/// <inheritdoc cref="IVec2{V, C, D, W, M}.flipY" />
		public static float2 flipY( this float2 v ) => new(v.x, -v.y);

		/// <inheritdoc cref="IVec2{V, C, D, W, M}.flipY" />
		public static double2 flipY( this double2 v ) => new(v.x, -v.y);

		/// <inheritdoc cref="IVec2{V, C, D, W, M}.flipY" />
		public static int3 flipY( this int3 v ) => new(v.x, -v.y, v.z);

		/// <inheritdoc cref="IVec2{V, C, D, W, M}.flipY" />
		public static float3 flipY( this float3 v ) => new(v.x, -v.y, v.z);

		/// <inheritdoc cref="IVec2{V, C, D, W, M}.flipY" />
		public static double3 flipY( this double3 v ) => new(v.x, -v.y, v.z);

		/// <inheritdoc cref="IVec2{V, C, D, W, M}.flipY" />
		public static int4 flipY( this int4 v ) => new(v.x, -v.y, v.z, v.w);

		/// <inheritdoc cref="IVec2{V, C, D, W, M}.flipY" />
		public static float4 flipY( this float4 v ) => new(v.x, -v.y, v.z, v.w);

		/// <inheritdoc cref="IVec2{V, C, D, W, M}.flipY" />
		public static double4 flipY( this double4 v ) => new(v.x, -v.y, v.z, v.w);


		/// <inheritdoc cref="IVec2{V, C, D, W, M}.zeroY" />
		public static V zeroY<V, C, D, W, M>( V v ) where V : IVec2<V, C, D, W, M> => v.zeroY;

		/// <inheritdoc cref="IVec2{V, C, D, W, M}.zeroY" />
		public static rat2 zeroY( rat2 v ) => v.zeroY;

		/// <inheritdoc cref="IVec2{V, C, D, W, M}.zeroY" />
		public static inth2 zeroY( inth2 v ) => v.zeroY;

		/// <inheritdoc cref="IVec2{V, C, D, W, M}.zeroY" />
		public static int2 zeroY( this int2 v ) => new(v.x, 0);

		/// <inheritdoc cref="IVec2{V, C, D, W, M}.zeroY" />
		public static float2 zeroY( this float2 v ) => new(v.x, 0);

		/// <inheritdoc cref="IVec2{V, C, D, W, M}.zeroY" />
		public static double2 zeroY( this double2 v ) => new(v.x, 0);

		/// <inheritdoc cref="IVec2{V, C, D, W, M}.zeroY" />
		public static int3 zeroY( this int3 v ) => new(v.x, 0, v.z);

		/// <inheritdoc cref="IVec2{V, C, D, W, M}.zeroY" />
		public static float3 zeroY( this float3 v ) => new(v.x, 0, v.z);

		/// <inheritdoc cref="IVec2{V, C, D, W, M}.zeroY" />
		public static double3 zeroY( this double3 v ) => new(v.x, 0, v.z);

		/// <inheritdoc cref="IVec2{V, C, D, W, M}.zeroY" />
		public static int4 zeroY( this int4 v ) => new(v.x, 0, v.z, v.w);

		/// <inheritdoc cref="IVec2{V, C, D, W, M}.zeroY" />
		public static float4 zeroY( this float4 v ) => new(v.x, 0, v.z, v.w);

		/// <inheritdoc cref="IVec2{V, C, D, W, M}.zeroY" />
		public static double4 zeroY( this double4 v ) => new(v.x, 0, v.z, v.w);
	}

}