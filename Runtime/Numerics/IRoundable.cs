// by Freya Holmér (https://github.com/FreyaHolmer/Mathfs)

namespace Freya {

	/// <summary>Basically the same as C#'s <see cref="System.MidpointRounding"/>, but older .net versions don't have all the options</summary>
	public enum RoundingDirection {
		ToEven = 0,
		AwayFromZero = 1,
		ToZero = 2,
		ToNegativeInfinity = 3,
		ToPositiveInfinity = 4,
	}

	/// <summary>Objects that can be rounded to nearby values</summary>
	public interface IRoundable<out R> {
		public R round( RoundingDirection rounding = RoundingDirection.ToEven );
		public R floorToward0 { get; }
		public R ceilAwayFrom0 { get; }
		public R floor { get; }
		public R ceil { get; }
	}

}