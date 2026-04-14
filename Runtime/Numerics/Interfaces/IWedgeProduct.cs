using Unity.Mathematics;

namespace Freya {

	/// <summary>Objects that implement the wedge product</summary>
	public interface IWedgeProduct<V, W> {
		/// <summary>The wedge product between two vectors. This is a generalized form of the cross product.
		/// <ul><li>In 2D, this returns a scalar, and is sometimes called the perpendicular dot product.</li>
		/// <li>In 3D, this returns a vector, and is effectively the same as the cross product
		/// (technically it's a bivector but whatever)</li>
		/// </ul></summary>
		[BinaryOp] public W wedge( V other );
	}

	public static partial class mathfs {
		/// <inheritdoc cref="IWedgeProduct{V,W}.wedge(V)" />
		public static W wedge<V, W>( V a, V b ) where V : IWedgeProduct<V, W> => a.wedge( b );
	}

}