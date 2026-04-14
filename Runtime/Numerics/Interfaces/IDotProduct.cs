using Unity.Mathematics;

namespace Freya {

	/// <summary>Objects that implement a dot product</summary>
	public interface IDotProduct<B, D> : ISqrMag<D> {
		/// <summary>The dot product between two vectors. This is the sum of the product of each respective component</summary>
		[BinaryOp] public D dot( B other );
	}

	public static partial class mathfs {
		// todo: special case
		/// <inheritdoc cref="IDotProduct{B,D}.dot" />
		public static rat dot( int2 a, rat2 b ) => b.dot( b );
	}

}