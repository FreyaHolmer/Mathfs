namespace Freya {

	using UnityEngine;

	/// <summary>Restricts this integer to a range of 1 and above</summary>
	public class PositiveIntegerAttribute : PropertyAttribute {}

	/// <summary>Restricts this integer to a range of -1 and below</summary>
	public class NegativeIntegerAttribute : PropertyAttribute {}

	/// <summary>Restricts this integer to a range of 0 and above</summary>
	public class NonNegativeIntegerAttribute : PropertyAttribute {}

	/// <summary>Restricts this integer to a range of 0 and below</summary>
	public class NonPositiveIntegerAttribute : PropertyAttribute {}

	/// <summary>Restricts this integer to be non-zero</summary>
	public class NonZeroIntegerAttribute : PropertyAttribute {}

}