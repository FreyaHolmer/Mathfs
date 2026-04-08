using Unity.Mathematics;

namespace Freya {

	public interface IVec1<V, C, D, W, M> : IVec<V, C, D, W> {
		/// <summary>The X component of this vector</summary>
		public C X { get; }
		/// <summary>This vector with a reversed X component</summary>
		public V flipX { get; }
		/// <summary>This vector with a zeroed-out X component</summary>
		public V zeroX { get; }
	}

	// X component boilerplate
	public static partial class mathfs {

		/// <inheritdoc cref="IVec1{V, C, D, W, M}.X" />
		public static C X<V, C, D, W, M>( V v ) where V : IVec1<V, C, D, W, M> => v.X;

		/// <inheritdoc cref="IVec1{V, C, D, W, M}.X" />
		public static rat X( rat2 v ) => v.X;

		/// <inheritdoc cref="IVec1{V, C, D, W, M}.X" />
		public static inth X( inth2 v ) => v.X;

		/// <inheritdoc cref="IVec1{V, C, D, W, M}.X" />
		public static int X( this int2 v ) => v.x;

		/// <inheritdoc cref="IVec1{V, C, D, W, M}.X" />
		public static float X( this float2 v ) => v.x;

		/// <inheritdoc cref="IVec1{V, C, D, W, M}.X" />
		public static double X( this double2 v ) => v.x;

		/// <inheritdoc cref="IVec1{V, C, D, W, M}.X" />
		public static int X( this int3 v ) => v.x;

		/// <inheritdoc cref="IVec1{V, C, D, W, M}.X" />
		public static float X( this float3 v ) => v.x;

		/// <inheritdoc cref="IVec1{V, C, D, W, M}.X" />
		public static double X( this double3 v ) => v.x;

		/// <inheritdoc cref="IVec1{V, C, D, W, M}.X" />
		public static int X( this int4 v ) => v.x;

		/// <inheritdoc cref="IVec1{V, C, D, W, M}.X" />
		public static float X( this float4 v ) => v.x;

		/// <inheritdoc cref="IVec1{V, C, D, W, M}.X" />
		public static double X( this double4 v ) => v.x;


		/// <inheritdoc cref="IVec1{V, C, D, W, M}.flipX" />
		public static V flipX<V, C, D, W, M>( V v ) where V : IVec1<V, C, D, W, M> => v.flipX;

		/// <inheritdoc cref="IVec1{V, C, D, W, M}.flipX" />
		public static rat2 flipX( rat2 v ) => v.flipX;

		/// <inheritdoc cref="IVec1{V, C, D, W, M}.flipX" />
		public static inth2 flipX( inth2 v ) => v.flipX;

		/// <inheritdoc cref="IVec1{V, C, D, W, M}.flipX" />
		public static int2 flipX( this int2 v ) => new(-v.x, v.y);

		/// <inheritdoc cref="IVec1{V, C, D, W, M}.flipX" />
		public static float2 flipX( this float2 v ) => new(-v.x, v.y);

		/// <inheritdoc cref="IVec1{V, C, D, W, M}.flipX" />
		public static double2 flipX( this double2 v ) => new(-v.x, v.y);

		/// <inheritdoc cref="IVec1{V, C, D, W, M}.flipX" />
		public static int3 flipX( this int3 v ) => new(-v.x, v.y, v.z);

		/// <inheritdoc cref="IVec1{V, C, D, W, M}.flipX" />
		public static float3 flipX( this float3 v ) => new(-v.x, v.y, v.z);

		/// <inheritdoc cref="IVec1{V, C, D, W, M}.flipX" />
		public static double3 flipX( this double3 v ) => new(-v.x, v.y, v.z);

		/// <inheritdoc cref="IVec1{V, C, D, W, M}.flipX" />
		public static int4 flipX( this int4 v ) => new(-v.x, v.y, v.z, v.w);

		/// <inheritdoc cref="IVec1{V, C, D, W, M}.flipX" />
		public static float4 flipX( this float4 v ) => new(-v.x, v.y, v.z, v.w);

		/// <inheritdoc cref="IVec1{V, C, D, W, M}.flipX" />
		public static double4 flipX( this double4 v ) => new(-v.x, v.y, v.z, v.w);


		/// <inheritdoc cref="IVec1{V, C, D, W, M}.zeroX" />
		public static V zeroX<V, C, D, W, M>( V v ) where V : IVec1<V, C, D, W, M> => v.zeroX;

		/// <inheritdoc cref="IVec1{V, C, D, W, M}.zeroX" />
		public static rat2 zeroX( rat2 v ) => v.zeroX;

		/// <inheritdoc cref="IVec1{V, C, D, W, M}.zeroX" />
		public static inth2 zeroX( inth2 v ) => v.zeroX;

		/// <inheritdoc cref="IVec1{V, C, D, W, M}.zeroX" />
		public static int2 zeroX( this int2 v ) => new(0, v.y);

		/// <inheritdoc cref="IVec1{V, C, D, W, M}.zeroX" />
		public static float2 zeroX( this float2 v ) => new(0, v.y);

		/// <inheritdoc cref="IVec1{V, C, D, W, M}.zeroX" />
		public static double2 zeroX( this double2 v ) => new(0, v.y);

		/// <inheritdoc cref="IVec1{V, C, D, W, M}.zeroX" />
		public static int3 zeroX( this int3 v ) => new(0, v.y, v.z);

		/// <inheritdoc cref="IVec1{V, C, D, W, M}.zeroX" />
		public static float3 zeroX( this float3 v ) => new(0, v.y, v.z);

		/// <inheritdoc cref="IVec1{V, C, D, W, M}.zeroX" />
		public static double3 zeroX( this double3 v ) => new(0, v.y, v.z);

		/// <inheritdoc cref="IVec1{V, C, D, W, M}.zeroX" />
		public static int4 zeroX( this int4 v ) => new(0, v.y, v.z, v.w);

		/// <inheritdoc cref="IVec1{V, C, D, W, M}.zeroX" />
		public static float4 zeroX( this float4 v ) => new(0, v.y, v.z, v.w);

		/// <inheritdoc cref="IVec1{V, C, D, W, M}.zeroX" />
		public static double4 zeroX( this double4 v ) => new(0, v.y, v.z, v.w);

	}

}