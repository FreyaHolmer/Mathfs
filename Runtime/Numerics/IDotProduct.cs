using Unity.Mathematics;

namespace Freya {

	/// <summary>Objects that implement a dot product</summary>
	public interface IDotProduct<B, D> {
		/// <summary>The dot product between two vectors. This is the sum of the product of each respective component</summary>
		public D dot( B other );
	}

	public static partial class mathfs {
		/// <inheritdoc cref="IDotProduct{B,D}.dot" />
		public static D dot<A, B, D>( A a, B b ) where A : IDotProduct<B, D> => a.dot( b );

		/// <inheritdoc cref="IDotProduct{B,D}.dot" />
		public static rat dot( rat2 a, rat2 b ) => a.dot( b );

		/// <inheritdoc cref="IDotProduct{B,D}.dot" />
		public static rat dot( inth2 a, inth2 b ) => a.dot( b );

		/// <inheritdoc cref="IDotProduct{B,D}.dot" />
		public static rat dot( rat2 a, int2 b ) => a.dot( b );

		/// <inheritdoc cref="IDotProduct{B,D}.dot" />
		public static rat dot( int2 a, rat2 b ) => b.dot( b );

		/// <inheritdoc cref="IDotProduct{B,D}.dot" />
		public static int dot( this int2 a, int2 b ) => math.dot( a, b );

		/// <inheritdoc cref="IDotProduct{B,D}.dot" />
		public static float dot( this float2 a, float2 b ) => math.dot( a, b );

		/// <inheritdoc cref="IDotProduct{B,D}.dot" />
		public static double dot( this double2 a, double2 b ) => math.dot( a, b );
	}

}