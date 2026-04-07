// by Freya Holmér (https://github.com/FreyaHolmer/Mathfs)

using Unity.Mathematics;

namespace Freya {

	public interface IVec<V, C, D, W> : IDotProduct<V, D>, IWedgeProduct<V, W> {
		/// <summary>Returns a component of this vector by index</summary>
		public C this[ int i ] { get; }
		/// <summary>Returns whether this lies flat along at least one axis</summary>
		public bool isOrthogonal { get; }
		/// <summary>Returns whether this vector is the zero vector</summary>
		public bool isZero { get; }

		/// <summary>The vector from this point to the target. Equivalent to <c>target - this</c></summary>
		public V to( V target );

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

		/// <summary>The minimum of the components of this vector</summary>
		public C cmin { get; }
		/// <summary>The maximum of the components of this vector</summary>
		public C cmax { get; }
		/// <summary>The sum of the components of this vector</summary>
		public C csum { get; }

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

	/// <summary>Objects that implement a dot product</summary>
	public interface IDotProduct<V, D> {
		/// <summary>The dot product between two vectors. This is the sum of the product of each respective component</summary>
		public D dot( V other );
	}

	/// <summary>Objects that implement the wedge product</summary>
	public interface IWedgeProduct<V, W> {
		/// <summary>The wedge product between two vectors. This is a generalized form of the cross product.
		/// <ul><li>In 2D, this returns a scalar, and is sometimes called the perpendicular dot product.</li>
		/// <li>In 3D, this returns a vector, and is effectively the same as the cross product (technically it's a bivector but whatever)</li>
		/// </ul></summary>
		public W wedge( V other );
	}

	/// <summary>Objects that reside within four quadrants in 2D</summary>
	public interface IQuadrant2D {
		/// <summary>The index of the quadrant containing this position, indexed from 0 to 3, starting from 0 in the top right,
		/// increasing in the positive rotation direction/counter-clockwise.
		/// Ambiguous positions pick the quadrant in the positive rotation direction.</summary>
		public int quadrant { get; }
		/// <summary>The signed of the quadrant containing this position, indexed from 0 to 3, starting from 0 in the top right,
		/// increasing in the positive rotation direction/counter-clockwise.
		/// Ambiguous positions pick the quadrant in the positive rotation direction.</summary>
		public int signedQuadrant { get; }
		/// <summary>The X-axis of the basis within the current quadrant.
		/// Ambiguous positions pick the quadrant in the positive rotation direction. Zero-vectors return <c>(1,0)</c></summary>
		public int2 quadrantBasisX { get; }
		/// <summary>Returns the two basis vectors of the quadrant that contains this position.
		/// Ambiguous positions pick the quadrant in the positive rotation direction</summary>
		public (int2 x, int2 y) quadrantBasis { get; }
	}

	/// <summary>Objects that can be treated like complex numbers</summary>
	public interface IComplex<V, M> {
		/// <summary>Multiplies as if they were complex numbers. The resulting vector is "rotated" by the other, and scaled by its magnitude.
		/// Note that this operation does not use any trigonometry or square roots, it's very cheap to use!</summary>
		public M complexMul( V other );

		/// <summary>The complex conjugate of this vector, if treated as a complex number. Which, in english, just means it negates the y component</summary>
		public V complexConj { get; }
	}

	public interface IVec2<V, C, D, W, M> : IVec<V, C, D, W>, IQuadrant2D, IComplex<V, M> {
		/// <summary>The X component of this vector</summary>
		public C X { get; }
		/// <summary>The Y component of this vector</summary>
		public C Y { get; }


		/// <summary>Rotates this vector in the positive rotation direction by 90 degrees. This is usually a counter-clockwise/left turn"</summary>
		public V rot90 { get; }
		/// <summary>Rotates this vector in the positive rotation direction by 90 degrees. This is usually a clockwise/right turn"</summary>
		public V rotNeg90 { get; }
		/// <summary>Rotates this vector by 180 degrees. Equivalent to negating this vector</summary>
		public V rot180 { get; }


		// public V rot45chebyshev { get; }
		// public V FromVector2( Vector2 v ); // should only happen for coarse things like inthalf2 and int. rational ones are messy here
	}

}