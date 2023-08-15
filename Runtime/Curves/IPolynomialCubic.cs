namespace Freya {

	public interface IPolynomialCubic<P, V> {

		/// <summary>The constant coefficient</summary>
		public V C0 { get; set; }
		/// <summary>The linear coefficient</summary>
		public V C1 { get; set; }
		/// <summary>The quadratic coefficient</summary>
		public V C2 { get; set; }
		/// <summary>The cubic coefficient</summary>
		public V C3 { get; set; }

		/// <summary>The degree of the polynomial</summary>
		public int Degree { get; }
		
		/// <summary>Returns the component polynomial of the given axis/dimension</summary>
		/// <param name="i">Index of axis/dimension. 0 = x. 1 = y, etc</param>
		public Polynomial this[ int i ] { get; set; }

		/// <summary>Gets the coefficient of the given degree</summary>
		/// <param name="degree">The degree of the coefficient you want to get. For example, 0 will return the constant coefficient, 3 will return the cubic coefficient</param>
		V GetCoefficient( int degree );

		/// <summary>Sets the coefficient of the given degree</summary>
		/// <param name="degree">The degree of the coefficient you want to set. For example, 0 will return the constant coefficient, 3 will return the cubic coefficient</param>
		/// <param name="value">The value to set it to</param>
		public void SetCoefficient( int degree, V value );

		/// <summary>Evaluates the polynomial at the given value <c>t</c></summary>
		/// <param name="t">The value to sample at</param>
		public V Eval( float t );

		/// <summary>Evaluates the <c>n</c>:th derivative of the polynomial at the given value <c>t</c></summary>
		/// <param name="t">The value to sample at</param>
		/// <param name="n">The derivative to evaluate</param>
		public V Eval( float t, int n );

		/// <summary>Differentiates this function, returning the n-th derivative of this polynomial</summary>
		/// <param name="n">The number of times to differentiate this function. 0 returns the function itself, 1 returns the first derivative</param>
		public P Differentiate( int n = 1 );

		/// <summary>Scales the parameter space by a factor. For example, the output of the polynomial in the input interval [0 to 1] will now be in the range [0 to factor]</summary>
		/// <param name="factor">The factor to scale the input parameters by</param>
		public P ScaleParameterSpace( float factor );

		/// <summary>Given an inner function g(x), returns f(g(x))</summary>
		/// <param name="g0">The constant coefficient of the inner function g(x)</param>
		/// <param name="g1">The linear coefficient of the inner function g(x)</param>
		public P Compose( float g0, float g1 );

	}

}