using Unity.Mathematics;

namespace Freya {

	/// <summary>Objects that can be treated like complex numbers</summary>
	public interface IComplex<V, M> {
		/// <summary>Multiplies as if they were complex numbers. The resulting vector is "rotated" by the other, and scaled by its magnitude.
		/// Note that this operation does not use any trigonometry or square roots, it's very cheap to use!</summary>
		public M complexMul( V other );

		/// <summary>The complex conjugate of this vector, if treated as a complex number. Which, in english, just means it negates the y component</summary>
		public V complexConj { get; }
	}

	public static partial class mathfs {
		/// <inheritdoc cref="IComplex{V,M}.complexMul(V)" />
		public static M complexMul<V, M>( V a, V b ) where V : IComplex<V, M> => a.complexMul( b );

		/// <inheritdoc cref="IComplex{V,M}.complexMul(V)" />
		public static rat2 complexMul( rat2 a, rat2 b ) => a.complexMul( b );

		/// <inheritdoc cref="IComplex{V,M}.complexMul(V)" />
		public static rat2 complexMul( inth2 a, inth2 b ) => a.complexMul( b );

		/// <inheritdoc cref="IComplex{V,M}.complexMul(V)" />
		public static int2 complexMul( this int2 a, int2 b ) => new(a.x * b.x - a.y * b.y, a.x * b.y + a.y * b.x);

		/// <inheritdoc cref="IComplex{V,M}.complexMul(V)" />
		public static float2 complexMul( this float2 a, float2 b ) => new(a.x * b.x - a.y * b.y, a.x * b.y + a.y * b.x);

		/// <inheritdoc cref="IComplex{V,M}.complexMul(V)" />
		public static double2 complexMul( this double2 a, double2 b ) => new(a.x * b.x - a.y * b.y, a.x * b.y + a.y * b.x);


		/// <inheritdoc cref="IComplex{V,M}.complexConj" />
		public static V complexConj<V, M>( V v ) where V : IComplex<V, M> => v.complexConj;

		/// <inheritdoc cref="IComplex{V,M}.complexConj" />
		public static rat2 complexConj( rat2 v ) => v.complexConj;

		/// <inheritdoc cref="IComplex{V,M}.complexConj" />
		public static inth2 complexConj( inth2 v ) => v.complexConj;

		/// <inheritdoc cref="IComplex{V,M}.complexConj" />
		public static int2 complexConj( this int2 v ) => new(v.x, -v.y);

		/// <inheritdoc cref="IComplex{V,M}.complexConj" />
		public static float2 complexConj( this float2 v ) => new(v.x, -v.y);

		/// <inheritdoc cref="IComplex{V,M}.complexConj" />
		public static double2 complexConj( this double2 v ) => new(v.x, -v.y);

	}

}