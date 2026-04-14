using Unity.Mathematics;

namespace Freya {

	/// <summary>Objects that can be treated like complex numbers</summary>
	public interface IComplex<V, M> {
		/// <summary>Multiplies as if they were complex numbers. The resulting vector is "rotated" by the other, and scaled by its magnitude.
		/// Note that this operation does not use any trigonometry or square roots, it's very cheap to use!</summary>
		[BinaryOp] public M complexMul( V other );

		/// <summary>The complex conjugate of this vector, if treated as a complex number. Which, in english, just means it negates the y component</summary>
		public V complexConj { get; }
	}

}