// by Freya Holmér (https://github.com/FreyaHolmer/Mathfs)

using UnityEngine;

namespace Freya {

	/// <summary>A shared interface between Ray, Line and LineSegment</summary>
	public interface ILinear2D {

		/// <summary>The origin of this linear 2D object</summary>
		Vector2 Origin { get; }

		/// <summary>The direction of this linear 2D object</summary>
		Vector2 Dir { get; }

		/// <summary>Returns a point along this linear 2D object</summary>
		/// <param name="t">The t-value along the linear 2D object</param>
		Vector2 GetPoint( float t );

		/// <summary>Returns whether or not this t-value is within this linear 2D object</summary>
		/// <param name="t">The t-value along the linear 2D object</param>
		bool IsValidTValue( float t );
	}

}