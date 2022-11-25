// by Freya Holmér (https://github.com/FreyaHolmer/Mathfs)

namespace Freya {

	public enum RotationSpace {
		/// <summary>An intrinsic rotation around its own local axes, usually called "local" or "self" space. Equivalent to <c>q*rotation</c></summary>
		Self,
		/// <summary>Rotation around its pre-rotation axes, usually "world" space. Equivalent to <c>rotation*q</c></summary>
		Extrinsic
	}

}