// by Freya Holmér (https://github.com/FreyaHolmer/Mathfs)

using Unity.Mathematics;

namespace Freya {


	public interface IVec<V, C, D, W> : IDotProduct<V, D>, IWedgeProduct<V, W>, IVecComponents<C> {

		/// <summary>The squared magnitude of this vector</summary>
		public D magSq { get; }

		/// <summary>The chebyshev magnitude of this vector.
		/// In chebyshev distance, diagonal distances are treated the same as orthogonal distances.
		/// This means the magnitude of (1,1) is 1, the magnitude of (2,2) is 2</summary>
		public C magChebyshev { get; }
		/// <summary>The taxicab magnitude of this vector.
		/// In taxicab distance, diagonal distances are treated as if you can only measure orthogonally.
		/// This means the magnitude of (1,1) is 2, the magnitude of (2,2) is 4</summary>
		public C magTaxicab { get; }


		/// <summary>Returns whether this point is in front of or behind a plane.
		/// <ul>
		/// <li>returns <c>+1</c> when in front of the plane</li>
		/// <li>returns <c> 0</c> when inside the plane</li>
		/// <li>returns <c>-1</c> when behind the plane</li>
		/// </ul>
		/// </summary>
		/// <param name="planePos">A point inside the plane</param>
		/// <param name="planeNormal">The normal direction of the plane</param>
		public int pointSideOfPlane( V planePos, V planeNormal );
	}

	public static partial class mathfs {
		// todo
	}


}