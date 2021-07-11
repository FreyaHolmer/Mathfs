// by Freya Holmér (https://github.com/FreyaHolmer/Mathfs)

namespace Freya {

	/// <summary>The type/degree of a polynomial</summary>
	public enum PolynomialType {

		/// <summary>A polynomial that is just a, straight up constant value</summary>
		Constant,

		/// <summary>A polynomial of the form ax+b</summary>
		Linear,

		/// <summary>A polynomial of the form ax²+bx+c</summary>
		Quadratic,

		/// <summary>A polynomial of the form ax³+bx²+cx+d</summary>
		Cubic
	}

}