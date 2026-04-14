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

}