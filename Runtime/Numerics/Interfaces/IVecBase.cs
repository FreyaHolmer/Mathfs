namespace Freya {

	public interface IVec1Base<V, C, D> : IVec<V, C, D> {
		/// <summary>The X component of this vector</summary>
		public C X { get; }
		/// <summary>This vector with a reversed X component</summary>
		public V flipX { get; }
		/// <summary>This vector with a zeroed-out X component</summary>
		public V zeroX { get; }
	}
	
	/// <summary>Operations for vectors 2D and above</summary>
	public interface IVec2Base<V, C, D> : IVec1Base<V, C, D> {
		/// <summary>The Y component of this vector</summary>
		public C Y { get; }
		/// <summary>This vector with a reversed Y component</summary>
		public V flipY { get; }
		/// <summary>This vector with a zeroed-out Y component</summary>
		public V zeroY { get; }
	}

	/// <summary>Operations for vectors 3D and above</summary>
	public interface IVec3Base<V, C, D> : IVec2Base<V, C, D> {
		/// <summary>The Z component of this vector</summary>
		public C Z { get; }
		/// <summary>This vector with a reversed Z component</summary>
		public V flipZ { get; }
		/// <summary>This vector with a zeroed-out Z component</summary>
		public V zeroZ { get; }
	}

	/// <summary>Operations for vectors 4D and above</summary>
	public interface IVec4Base<V, C, D> : IVec3Base<V, C, D> {
		/// <summary>The W component of this vector</summary>
		public C W { get; }
		/// <summary>This vector with a reversed W component</summary>
		public V flipW { get; }
		/// <summary>This vector with a zeroed-out W component</summary>
		public V zeroW { get; }
	}

}