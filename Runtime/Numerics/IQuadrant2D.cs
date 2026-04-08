using Unity.Mathematics;

namespace Freya {

	/// <summary>Objects that reside within four quadrants in 2D</summary>
	public interface IQuadrant2D {
		/// <summary>The index of the quadrant containing this position, indexed from 0 to 3, starting from 0 in the top right,
		/// increasing in the positive rotation direction/counter-clockwise.
		/// Ambiguous positions pick the quadrant in the positive rotation direction.<br/><br/>
		/// Quadrant layout:
		/// <list type="table"><item>
		///   <term>1</term>
		///   <description>0</description>
		/// </item>
		/// <item>
		///   <term>2</term>
		///   <description>3</description>
		/// </item></list></summary>
		public int quadrant { get; }
		/// <summary>The signed of the quadrant containing this position, indexed from 0 to 3, starting from 0 in the top right,
		/// increasing in the positive rotation direction/counter-clockwise.
		/// Ambiguous positions pick the quadrant in the positive rotation direction.<br/><br/>
		/// Quadrant layout:
		/// <list type="table"><item>
		///   <term>1</term>
		///   <description>0</description>
		/// </item>
		/// <item>
		///   <term>-2</term>
		///   <description>-1</description>
		/// </item></list></summary>
		public int signedQuadrant { get; }
		/// <summary>The X-axis of the basis within the current quadrant.
		/// Ambiguous positions pick the quadrant in the positive rotation direction. Zero-vectors return <c>(1,0)</c></summary>
		public int2 quadrantBasisX { get; }
		/// <summary>Returns the two basis vectors of the quadrant that contains this position.
		/// Ambiguous positions pick the quadrant in the positive rotation direction</summary>
		public (int2 x, int2 y) quadrantBasis { get; }
	}

	public static partial class mathfs {
		/// <inheritdoc cref="IQuadrant2D.quadrant" />
		public static int quadrant<V>( V v ) where V : IQuadrant2D => v.quadrant;

		/// <inheritdoc cref="IQuadrant2D.quadrant" />
		public static int quadrant( rat2 v ) => v.quadrant;

		/// <inheritdoc cref="IQuadrant2D.quadrant" />
		public static int quadrant( inth2 v ) => v.quadrant;

		/// <inheritdoc cref="IQuadrant2D.quadrant" />
		public static int quadrant( this int2 v ) =>
			v.y switch {
				> 00 when v.x <= 0 => 1,
				<= 0 when v.x < 00 => 2,
				< 00 when v.x >= 0 => 3,
				_                  => 0
			};

		/// <inheritdoc cref="IQuadrant2D.quadrant" />
		public static int quadrant( this float2 v ) =>
			v.y switch {
				> 00 when v.x <= 0 => 1,
				<= 0 when v.x < 00 => 2,
				< 00 when v.x >= 0 => 3,
				_                  => 0
			};

		/// <inheritdoc cref="IQuadrant2D.quadrant" />
		public static int quadrant( this double2 v ) =>
			v.y switch {
				> 00 when v.x <= 0 => 1,
				<= 0 when v.x < 00 => 2,
				< 00 when v.x >= 0 => 3,
				_                  => 0
			};


		/// <inheritdoc cref="IQuadrant2D.quadrantBasisX" />
		public static int2 quadrantBasisX<V>( V v ) where V : IQuadrant2D => v.quadrantBasisX;

		/// <inheritdoc cref="IQuadrant2D.quadrantBasisX" />
		public static int2 quadrantBasisX( rat2 v ) => v.quadrantBasisX;

		/// <inheritdoc cref="IQuadrant2D.quadrantBasisX" />
		public static int2 quadrantBasisX( inth2 v ) => v.quadrantBasisX;

		/// <inheritdoc cref="IQuadrant2D.quadrantBasisX" />
		public static int2 quadrantBasisX( this int2 v ) => quadrantToBasisX( v.quadrant() );

		/// <inheritdoc cref="IQuadrant2D.quadrantBasisX" />
		public static int2 quadrantBasisX( this float2 v ) => quadrantToBasisX( v.quadrant() );

		/// <inheritdoc cref="IQuadrant2D.quadrantBasisX" />
		public static int2 quadrantBasisX( this double2 v ) => quadrantToBasisX( v.quadrant() );


		/// <inheritdoc cref="IQuadrant2D.quadrantBasis" />
		public static (int2 x, int2 y) quadrantBasis<V>( V v ) where V : IQuadrant2D => v.quadrantBasis;

		/// <inheritdoc cref="IQuadrant2D.quadrantBasis" />
		public static (int2 x, int2 y) quadrantBasis( rat2 v ) => v.quadrantBasis;

		/// <inheritdoc cref="IQuadrant2D.quadrantBasis" />
		public static (int2 x, int2 y) quadrantBasis( inth2 v ) => v.quadrantBasis;

		/// <inheritdoc cref="IQuadrant2D.quadrantBasis" />
		public static (int2 x, int2 y) quadrantBasis( this int2 v ) => quadrantToBasis( v.quadrant() );

		/// <inheritdoc cref="IQuadrant2D.quadrantBasis" />
		public static (int2 x, int2 y) quadrantBasis( this float2 v ) => quadrantToBasis( v.quadrant() );

		/// <inheritdoc cref="IQuadrant2D.quadrantBasis" />
		public static (int2 x, int2 y) quadrantBasis( this double2 v ) => quadrantToBasis( v.quadrant() );
	}


}