// by Freya Holmér (https://github.com/FreyaHolmer/Mathfs)

namespace Freya {

	/// <summary>The three main types of catmull-rom curves</summary>
	public enum CatRomType {
		/// <summary>Uniform catmull-rom, which is the simplest form. Fast to evaluate but prone to overshooting and loops. Equivalent to an alpha value of 0</summary>
		Uniform,

		/// <summary>Centripetal catmull-rom, which follows the control points tightly, preventing cusps or loops, and generally doesn't overshoot as much as uniform catroms. Equivalent to an alpha value of 0.5</summary>
		Centripetal,

		/// <summary>Chordal catmull-rom, which follows the control points with wide arcs, for a very smooth path. Equivalent to an alpha value of 1</summary>
		Chordal
	}

	internal static class CatRomTypeExtensions {
		/// <summary>Returns the alpha value of a given catmull-rom type</summary>
		/// <param name="catRom">The type to get the alpha value from</param>
		public static float AlphaValue( this CatRomType catRom ) {
			if( catRom == CatRomType.Centripetal ) return 0.5f;
			if( catRom == CatRomType.Chordal ) return 1f;
			return 0f; // uniform
		}
	}

}