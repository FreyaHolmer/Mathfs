// by Freya Holmér (https://github.com/FreyaHolmer/Mathfs)

using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

namespace Freya {

	public interface INumberBase {
		/// <summary>Returns whether this number is an integer</summary>
		public bool isInteger { get; }

		/// <summary>Returns whether this vector is the zero vector</summary>
		public bool isZero { get; }

		/// <summary>Returns whether this is zero or lies along a single axis</summary>
		public bool isOrthogonal { get; }

	}

	public interface INumber<N> : INumberBase {
		/// <summary>Returns the absolute value of the number. Makes negative values positive</summary>
		public N abs { get; }

		/// <summary>Returns the minimum of two numbers</summary>
		[BinaryOp] public N min( N other );

		/// <summary>Returns the maximum of two numbers</summary>
		[BinaryOp] public N max( N other );

		/// <summary>The vector from this point to the target. Equivalent to <c>target - this</c></summary>
		public N to( N target );

		// I can't do this bc Unity uses older versions of C#:
		// public static abstract R zero { get; }
		// public static abstract R one { get; }
	}

}