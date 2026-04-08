using Unity.Mathematics;

namespace Freya {

	/// <summary>Objects that implement the wedge product</summary>
	public interface IWedgeProduct<V, W> {
		/// <summary>The wedge product between two vectors. This is a generalized form of the cross product.
		/// <ul><li>In 2D, this returns a scalar, and is sometimes called the perpendicular dot product.</li>
		/// <li>In 3D, this returns a vector, and is effectively the same as the cross product
		/// (technically it's a bivector but whatever)</li>
		/// </ul></summary>
		public W wedge( V other );
	}
	
	public static partial class mathfs {
		/// <inheritdoc cref="IWedgeProduct{V,W}.wedge(V)" />
		public static W wedge<V, W>( V a, V b ) where V : IWedgeProduct<V, W> => a.wedge( b );
		
		/// <inheritdoc cref="IWedgeProduct{V,W}.wedge(V)" />
		public static rat wedge( rat2 a, rat2 b ) => a.wedge( b );

		/// <inheritdoc cref="IWedgeProduct{V,W}.wedge(V)" />
		public static rat wedge( inth2 a, inth2 b ) => a.wedge( b );

		/// <inheritdoc cref="IWedgeProduct{V,W}.wedge(V)" />
		public static int wedge( this int2 a, int2 b ) => a.x * b.y - a.y * b.x;

		/// <inheritdoc cref="IWedgeProduct{V,W}.wedge(V)" />
		public static float wedge( this float2 a, float2 b ) => a.x * b.y - a.y * b.x;

		/// <inheritdoc cref="IWedgeProduct{V,W}.wedge(V)" />
		public static double wedge( this double2 a, double2 b ) => a.x * b.y - a.y * b.x;
	}

}