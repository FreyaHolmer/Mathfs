// by Freya Holmér (https://github.com/FreyaHolmer/Mathfs)

using System;

namespace Freya {

	/// <summary>Objects that can be rounded to nearby values</summary>
	public interface IRoundable<out R> {

		/// <summary>Rounds to the nearest integer</summary>
		/// <param name="rounding">The rounding method to use</param>
		public R round( RoundingDirection rounding = RoundingDirection.ToEven );

		/// <summary>Rounds to the nearest integer towards 0</summary>
		public R floorToward0 { get; }

		/// <summary>Rounds to the nearest integer away from 0</summary>
		public R ceilAwayFrom0 { get; }

		/// <summary>Rounds down to the nearest integer</summary>
		public R floor { get; }

		/// <summary>Rounds up to the nearest integer</summary>
		public R ceil { get; }
	}

	public static partial class mathfs {
		/// <inheritdoc cref="IRoundable{R}.round(RoundingDirection)" />
		public static R round<R, V>( V v, RoundingDirection rounding = RoundingDirection.ToEven ) where V : IRoundable<R> => v.round();
	}

	/// <summary>Basically the same as C#'s <see cref="System.MidpointRounding"/>, but older .net versions don't have all the options</summary>
	public enum RoundingDirection {
		ToEven = 0,
		AwayFromZero = 1,
		ToZero = 2,
		ToNegativeInfinity = 3,
		ToPositiveInfinity = 4,
	}

}