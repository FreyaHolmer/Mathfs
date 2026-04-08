using System;
using Unity.Mathematics;

namespace Freya {

	public interface ISignedNumber<out R> : INumber {
		/// <summary>Returns the sign of this number. Either -1, 0, or 1</summary>
		public R sign { get; }
	}

	public static partial class mathfs {
		/// <inheritdoc cref="ISignedNumber{T}.sign" />
		public static T sign<T>( T v ) where T : ISignedNumber<T> => v.sign;

		/// <inheritdoc cref="ISignedNumber{T}.sign" />
		public static int sign( this int i ) => Math.Sign( i );

		/// <inheritdoc cref="ISignedNumber{T}.sign" />
		public static int2 sign( this int2 i ) => new(Math.Sign( i.x ), Math.Sign( i.y ));

		/// <inheritdoc cref="ISignedNumber{T}.sign" />
		public static int sign( this float i ) => Math.Sign( i );

		/// <inheritdoc cref="ISignedNumber{T}.sign" />
		public static int sign( this double i ) => Math.Sign( i );
	}

}