using System;
using Unity.Mathematics;

namespace Freya {

	public interface ISignedNumber<out R> : INumberBase {
		/// <summary>Returns the sign of this number. Either -1, 0, or 1</summary>
		public R sign { get; }
	}

	public static partial class mathfs {
		/// <inheritdoc cref="ISignedNumber{T}.sign" />
		public static T sign<T>( T v ) where T : ISignedNumber<T> => v.sign;
	}

}