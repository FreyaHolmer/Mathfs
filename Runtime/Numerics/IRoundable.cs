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

		/// <inheritdoc cref="IRoundable{R}.round(RoundingDirection)" />
		public static int round( this float v, RoundingDirection rounding = RoundingDirection.ToEven ) => (int)MathF.Round( v, (MidpointRounding)rounding );

		/// <inheritdoc cref="IRoundable{R}.round(RoundingDirection)" />
		public static int round( this double v, RoundingDirection rounding = RoundingDirection.ToEven ) => (int)Math.Round( v, (MidpointRounding)rounding );

		/// <inheritdoc cref="IRoundable{R}.floorToward0" />
		public static R floorToward0<R, V>( V v ) where V : IRoundable<R> => v.floorToward0;

		/// <inheritdoc cref="IRoundable{R}.ceilAwayFrom0" />
		public static R ceilAwayFrom0<R, V>( V v ) where V : IRoundable<R> => v.ceilAwayFrom0;

		/// <inheritdoc cref="IRoundable{R}.floor" />
		public static R floor<R, V>( V v ) where V : IRoundable<R> => v.floor;

		/// <inheritdoc cref="IRoundable{R}.ceil" />
		public static R ceil<R, V>( V v ) where V : IRoundable<R> => v.ceil;
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