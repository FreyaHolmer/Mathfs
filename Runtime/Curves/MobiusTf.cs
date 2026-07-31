// by Freya Holmér (https://github.com/FreyaHolmer/Mathfs)

using System;
using UnityEngine.Assertions;

namespace Freya {

	/// <summary>The Möbius Transformation as an equation of the form f(x) = (ax+b)/(cx+d)</summary>
	[Serializable] public struct MobiusTf {

		public float a, b, c, d;

		public static readonly MobiusTf identity = new MobiusTf( 1, 1, 0, 0 );
		public bool IsIdentity => b == 0 && c == 0 && a == d;

		float Determinant => a * d - b * c;
		public bool IsValid => Determinant != 0;
		public bool IsAffine => c == 0;
		public MobiusTf Inverse => new(d, -b, -c, a);

		public MobiusTf( float a, float b, float c, float d ) {
			this.a = a;
			this.b = b;
			this.c = c;
			this.d = d;
			Assert.IsTrue( IsValid );
		}

		/// <summary>Evaluates the nth derivative of the mobius transform</summary>
		/// <param name="x">The parameter value to evaluate at</param>
		/// <param name="n">The nth derivative. 0 = evaluates the function. 1 = evaluates the first derivative, etc.</param>
		public float eval( float x, int n = 0 ) {
			float D = c * x + d;
			switch( n ) {
				case 0: return ( a * x + b ) / D;
				case 1: return Determinant / ( D * D );
				case 2: return -2 * Determinant / ( D * D * D );
				default:
					int scale = -Mathfs.Factorial( (uint)n ) * n.esign();
					float num = Determinant * c.pow( n - 1 );
					return scale * ( num / D.pow( n + 1 ) );
			}
		}

		/// <summary>Evaluates the definite integral from x0 to x1</summary>
		public float integrate( float x0, float x1 ) {
			float r = ( x1 - x0 ) / ( c * x0 + d );
			float rect = r * ( a * x0 + b );
			float curv = r * r * Determinant * logrem( c * r );
			return rect + curv;
		}

		/// <summary>A weird natural log remainder type thing (x-ln(x+1))/(x*x)</summary>
		static float logrem( float x ) {
			return x.abs() switch {
				0        => 0.5f, // singularity at 0
				< 0.001f => 0.5f - x / 3 + ( x * x ) / 4, // approximation near 0
				_        => ( x - MathF.Log( 1 + x ) ) / ( x * x )
			};
		}

	}

}