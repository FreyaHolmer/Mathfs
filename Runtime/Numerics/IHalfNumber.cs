using Unity.Mathematics;

namespace Freya {

	public interface IHalfNumber<out F> {
		/// <summary>Multiplies this by 2 and returns an integer value</summary>
		public F times2 { get; }
	}

	public static partial class mathfs {
		/// <inheritdoc cref="IHalfNumber{T}.times2" />
		public static T times2<T>( T v ) where T : IHalfNumber<T> => v.times2;

		/// <inheritdoc cref="IHalfNumber{T}.times2" />
		public static int times2( inth v ) => v.times2;

		/// <inheritdoc cref="IHalfNumber{T}.times2" />
		public static int2 times2( inth2 v ) => v.times2;
	}

}