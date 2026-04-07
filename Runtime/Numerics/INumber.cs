// by Freya Holmér (https://github.com/FreyaHolmer/Mathfs)

using Unity.Mathematics;
using UnityEngine;

namespace Freya {
	
	public interface INumber {
		public bool isInteger { get; }
	}

	public interface ISignedNumber<out R> : INumber {
		public R sign { get; }
	}

	public interface INumber<N, out R> : INumber {
		/// <summary>Returns the absolute value of this number</summary>
		public N abs { get; }
		public N max( N other );
		public N min( N other );

		// I can't do this bc Unity uses older versions of C#:
		// public static abstract R zero { get; }
		// public static abstract R one { get; }
	}

	public interface IHalfNumber<out F> {
		/// <summary>Multiplies this by 2 and returns an integer value</summary>
		public F times2 { get; }
	}

}