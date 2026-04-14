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

}