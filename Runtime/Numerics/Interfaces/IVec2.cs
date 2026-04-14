using Unity.Mathematics;

namespace Freya {

	/// <summary>Operations that are unique to 2D vectors</summary>
	public interface IVec2<V, C, D, W, M> : IVec2Base<V, C, D>, IWedgeProduct<V, W>, IQuadrant2D, IComplex<V, M> {
		/// <summary>Rotates this vector in the positive rotation direction by 90 degrees. This is usually a counter-clockwise/left turn</summary>
		public V rot90 { get; }
		/// <summary>Rotates this vector in the negative rotation direction by 90 degrees. This is usually a clockwise/right turn</summary>
		public V rotNeg90 { get; }
		/// <summary>Rotates this vector by 180 degrees. Equivalent to negating this vector</summary>
		public V rot180 { get; }
	}

}