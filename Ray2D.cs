// collected and expended upon by Freya Holmér (https://github.com/FreyaHolmer/Mathfs)

using UnityEngine;

namespace Freya {

	/// <summary>
	/// Similar to Unity's Ray2D, except this one allows you to not normalize the direction
	/// which saves performance as well as allows you to work at different scales
	/// </summary>
	public struct Ray2D {

		public Vector2 origin;
		public Vector2 dir;

		public Ray2D( Vector2 origin, Vector2 dir ) {
			this.origin = origin;
			this.dir = dir;
		}

		public Vector2 GetPoint( float distance ) => origin + dir * distance;

		public static implicit operator Ray2D( Ray ray ) => new Ray2D( ray.origin, ray.direction );
		public static implicit operator UnityEngine.Ray2D( Ray2D ray ) => new UnityEngine.Ray2D( ray.origin, ray.dir );

	}

}