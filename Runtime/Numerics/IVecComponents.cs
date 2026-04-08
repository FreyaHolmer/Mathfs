using Unity.Mathematics;

namespace Freya {

	public interface IVecComponents<C> {
		// todo: coooould make a C Component<V,C>( V elem, int iAxis )
		/// <summary>Returns a component of this vector by index</summary>
		public C this[ int i ] { get; }
		/// <summary>The minimum of the components of this vector</summary>
		public C cmin { get; }
		/// <summary>The maximum of the components of this vector</summary>
		public C cmax { get; }
		/// <summary>The sum of the components of this vector</summary>
		public C csum { get; }
	}

	public static partial class mathfs {

		/// <inheritdoc cref="IVecComponents{C}.cmin" />
		public static C cmin<C>( C v ) where C : IVecComponents<C> => v.cmin;

		/// <inheritdoc cref="IVecComponents{C}.cmin" />
		public static rat cmin( rat v ) => v;

		/// <inheritdoc cref="IVecComponents{C}.cmin" />
		public static rat cmin( rat2 v ) => v.cmin;

		/// <inheritdoc cref="IVecComponents{C}.cmin" />
		public static inth cmin( inth v ) => v;

		/// <inheritdoc cref="IVecComponents{C}.cmin" />
		public static inth cmin( inth2 v ) => v.cmin;

		/// <inheritdoc cref="IVecComponents{C}.cmin" />
		public static int cmin( this int v ) => v;

		/// <inheritdoc cref="IVecComponents{C}.cmin" />
		public static int cmin( this int2 v ) => math.cmin( v );

		/// <inheritdoc cref="IVecComponents{C}.cmin" />
		public static int cmin( this int3 v ) => math.cmin( v );

		/// <inheritdoc cref="IVecComponents{C}.cmin" />
		public static int cmin( this int4 v ) => math.cmin( v );

		/// <inheritdoc cref="IVecComponents{C}.cmin" />
		public static float cmin( this float v ) => v;

		/// <inheritdoc cref="IVecComponents{C}.cmin" />
		public static float cmin( this float2 v ) => math.cmin( v );

		/// <inheritdoc cref="IVecComponents{C}.cmin" />
		public static float cmin( this float3 v ) => math.cmin( v );

		/// <inheritdoc cref="IVecComponents{C}.cmin" />
		public static float cmin( this float4 v ) => math.cmin( v );

		/// <inheritdoc cref="IVecComponents{C}.cmin" />
		public static double cmin( this double v ) => v;

		/// <inheritdoc cref="IVecComponents{C}.cmin" />
		public static double cmin( this double2 v ) => math.cmin( v );

		/// <inheritdoc cref="IVecComponents{C}.cmin" />
		public static double cmin( this double3 v ) => math.cmin( v );

		/// <inheritdoc cref="IVecComponents{C}.cmin" />
		public static double cmin( this double4 v ) => math.cmin( v );


		/// <inheritdoc cref="IVecComponents{C}.cmax" />
		public static C cmax<C>( C v ) where C : IVecComponents<C> => v.cmax;

		/// <inheritdoc cref="IVecComponents{C}.cmax" />
		public static rat cmax( rat v ) => v;

		/// <inheritdoc cref="IVecComponents{C}.cmax" />
		public static rat cmax( rat2 v ) => v.cmax;

		/// <inheritdoc cref="IVecComponents{C}.cmax" />
		public static inth cmax( inth v ) => v;

		/// <inheritdoc cref="IVecComponents{C}.cmax" />
		public static inth cmax( inth2 v ) => v.cmax;

		/// <inheritdoc cref="IVecComponents{C}.cmax" />
		public static int cmax( this int v ) => v;

		/// <inheritdoc cref="IVecComponents{C}.cmax" />
		public static int cmax( this int2 v ) => math.cmax( v );

		/// <inheritdoc cref="IVecComponents{C}.cmax" />
		public static int cmax( this int3 v ) => math.cmax( v );

		/// <inheritdoc cref="IVecComponents{C}.cmax" />
		public static int cmax( this int4 v ) => math.cmax( v );

		/// <inheritdoc cref="IVecComponents{C}.cmax" />
		public static float cmax( this float v ) => v;

		/// <inheritdoc cref="IVecComponents{C}.cmax" />
		public static float cmax( this float2 v ) => math.cmax( v );

		/// <inheritdoc cref="IVecComponents{C}.cmax" />
		public static float cmax( this float3 v ) => math.cmax( v );

		/// <inheritdoc cref="IVecComponents{C}.cmax" />
		public static float cmax( this float4 v ) => math.cmax( v );

		/// <inheritdoc cref="IVecComponents{C}.cmax" />
		public static double cmax( this double v ) => v;

		/// <inheritdoc cref="IVecComponents{C}.cmax" />
		public static double cmax( this double2 v ) => math.cmax( v );

		/// <inheritdoc cref="IVecComponents{C}.cmax" />
		public static double cmax( this double3 v ) => math.cmax( v );

		/// <inheritdoc cref="IVecComponents{C}.cmax" />
		public static double cmax( this double4 v ) => math.cmax( v );


		/// <inheritdoc cref="IVecComponents{C}.csum" />
		public static C csum<C>( C v ) where C : IVecComponents<C> => v.csum;

		/// <inheritdoc cref="IVecComponents{C}.csum" />
		public static rat csum( rat v ) => v;

		/// <inheritdoc cref="IVecComponents{C}.csum" />
		public static rat csum( rat2 v ) => v.csum;

		/// <inheritdoc cref="IVecComponents{C}.csum" />
		public static inth csum( inth v ) => v;

		/// <inheritdoc cref="IVecComponents{C}.csum" />
		public static inth csum( inth2 v ) => v.csum;

		/// <inheritdoc cref="IVecComponents{C}.csum" />
		public static int csum( this int v ) => v;

		/// <inheritdoc cref="IVecComponents{C}.csum" />
		public static int csum( this int2 v ) => math.csum( v );

		/// <inheritdoc cref="IVecComponents{C}.csum" />
		public static int csum( this int3 v ) => math.csum( v );

		/// <inheritdoc cref="IVecComponents{C}.csum" />
		public static int csum( this int4 v ) => math.csum( v );

		/// <inheritdoc cref="IVecComponents{C}.csum" />
		public static float csum( this float v ) => v;

		/// <inheritdoc cref="IVecComponents{C}.csum" />
		public static float csum( this float2 v ) => math.csum( v );

		/// <inheritdoc cref="IVecComponents{C}.csum" />
		public static float csum( this float3 v ) => math.csum( v );

		/// <inheritdoc cref="IVecComponents{C}.csum" />
		public static float csum( this float4 v ) => math.csum( v );

		/// <inheritdoc cref="IVecComponents{C}.csum" />
		public static double csum( this double v ) => v;

		/// <inheritdoc cref="IVecComponents{C}.csum" />
		public static double csum( this double2 v ) => math.csum( v );

		/// <inheritdoc cref="IVecComponents{C}.csum" />
		public static double csum( this double3 v ) => math.csum( v );

		/// <inheritdoc cref="IVecComponents{C}.csum" />
		public static double csum( this double4 v ) => math.csum( v );

		/// <inheritdoc cref="IVecComponents{C}.csum" />
		public static int csum( this bool b ) => b ? 1 : 0;

		/// <inheritdoc cref="IVecComponents{C}.csum" />
		public static int csum( this bool2 b ) => math.csum( (int2)b );

		/// <inheritdoc cref="IVecComponents{C}.csum" />
		public static int csum( this bool3 b ) => math.csum( (int3)b );

		/// <inheritdoc cref="IVecComponents{C}.csum" />
		public static int csum( this bool4 b ) => math.csum( (int4)b );
	}

}