using System;
using UnityEngine;

namespace Freya {

	public class BSpline2D {

		public Vector2[] points;
		public float[] knots;
		public int degree;

		/// <summary>The Order of this curve (degree+1)</summary>
		public int Order => degree + 1;

		/// <summary>The number of control points in the B-spline hull</summary>
		public int PointCount => points.Length;

		/// <summary>Creates a B-spline of the given degree, from a set of points and a knot vector</summary>
		/// <param name="points">The B-spline control points</param>
		/// <param name="knots">The knot vector defining the parameter space of this B-spline. Note: the number of knots has to be exactly degree+pointCount+1</param>
		/// <param name="degree">The degree of the spline</param>
		public BSpline2D( Vector2[] points, float[] knots, int degree = 3 ) {
			this.points = points;
			this.knots = knots;
			this.degree = degree;
			this.evalBuffer = new Vector2[degree + 1];
			int expectedKnotCount = SplineUtils.BSplineKnotCount( this.points.Length, this.degree );
			if( knots.Length != expectedKnotCount )
				throw new ArgumentException( $"The knots array has to be of length (degree+pointCount+1). Got an array of {knots.Length} knots, expected {expectedKnotCount}", nameof(knots) );
		}

		/// <summary>Creates a uniform B-spline of the given degree, automatically configuring the knot vector to be uniform</summary>
		/// <param name="points">The B-spline control points</param>
		/// <param name="degree">The degree of the curve</param>
		/// <param name="open">Whether or not it should be open. Open means the curve passes through its endpoints</param>
		public BSpline2D( Vector2[] points, int degree = 3, bool open = false ) {
			this.points = points;
			this.degree = degree.AtLeast( 1 );
			this.evalBuffer = new Vector2[degree + 1];
			this.knots = SplineUtils.GenerateUniformKnots( this.degree, this.points.Length, open );
		}

		#region Parameter space knot stuff

		/// <summary>The number of knots in the B-spline parameter space</summary>
		public int KnotCount => knots.Length;

		/// <summary>The number of curve segments. Note: some of these curve segments may have a length of 0 depending on knot multiplicity</summary>
		public int SegmentCount => InternalKnotCount - 1;

		/// <summary>The first knot index of the internal parameter space</summary>
		public int InternalKnotIndexStart => degree;

		/// <summary>The last knot index of the internal parameter space</summary>
		public int InternalKnotIndexEnd => KnotCount - degree - 1;

		/// <summary>The parameter space knot value at the start of the internal parameter space</summary>
		public float InternalKnotValueStart => knots[InternalKnotIndexStart];

		/// <summary>The parameter space knot value at the end of the internal parameter space</summary>
		public float InternalKnotValueEnd => knots[InternalKnotIndexEnd];

		/// <summary>The number of knots in the internal parameter space</summary>
		public int InternalKnotCount => KnotCount - degree * 2;

		/// <summary>Returns the parameter space knot u-value, given a t-value along the whole spline</summary>
		/// <param name="t">A value from 0-1, representing a percentage along the whole spline</param>
		public float GetKnotValueAt( float t ) => Mathfs.Lerp( knots[degree], knots[knots.Length - degree - 1], t );

		/// <summary>Returns whether or not this B-spline is open, which means it will pass through its endpoints.
		/// A B-spline is open if the first degree+1 knots are equal, and the last degree+1 knots are equal</summary>
		public bool Open {
			get {
				int kc = KnotCount;
				for( int i = 0; i < degree; i++ ) {
					if( knots[i] != knots[i + 1] )
						return false;
					if( knots[kc - 1 - i] != knots[kc - i - 2] )
						return false;
				}

				return true;
			}
		}

		#endregion

		#region Differentiation

		/// <summary>Returns the derivative of this B-spline, which is a B-spline in and of itself</summary>
		public BSpline2D Differentiate() {
			float[] dKnots = new float[KnotCount];
			knots.CopyTo( dKnots, 0 );

			// one point less
			Vector2[] dPts = new Vector2[PointCount - 1];
			for( int i = 0; i < dPts.Length; i++ ) {
				Vector2 num = points[i + 1] - points[i];
				float den = knots[i + degree + 1] - knots[i + 1];
				float scale = den == 0 ? 0 : degree / den;
				dPts[i] = num * scale;
			}

			return new BSpline2D( dPts, dKnots, degree - 1 );
		}

		#endregion

		#region Point Evaluation

		/// <summary>Returns the point at the given t-value of a specific B-spline segment, by index</summary>
		/// <param name="segment">The segment to get a point from</param>
		/// <param name="t">The t-value along the segment to evaluate</param>
		public Vector2 GetSegmentPoint( int segment, float t ) {
			if( segment < 0 || segment >= SegmentCount )
				throw new IndexOutOfRangeException( $"B-Spline segment index {segment} is out of range. Valid indices: 0 to {SegmentCount - 1}" );

			float knotMin = knots[degree + segment];
			float knotMax = knots[degree + segment + 1];
			float u = Mathfs.Lerp( knotMin, knotMax, t );
			return Eval( degree + segment, u );
		}

		/// <summary>Returns the point at the given t-value in the spline</summary>
		/// <param name="t">A value from 0-1, representing a percentage along the whole spline</param>
		public Vector2 GetPoint( float t ) {
			float u = GetKnotValueAt( t ); // remap 0-1 to knot space
			return GetPointByKnotValue( u );
		}

		/// <summary>Returns the point at the given knot by index</summary>
		/// <param name="i">The index of the knot to get the position of</param>
		public Vector2 GetPointByKnotIndex( int i ) {
			int kRef = i.Clamp( InternalKnotIndexStart, InternalKnotIndexEnd - 1 );
			return Eval( kRef, knots[i] );
		}

		/// <summary>Returns the point at the given prameter space u-value of the spline</summary>
		/// <param name="u">A value in parameter space. Note: this value has to be within the internal knot interval</param>
		public Vector2 GetPointByKnotValue( float u ) {
			int i = 0;
			if( u >= knots[InternalKnotIndexEnd] ) {
				i = InternalKnotIndexEnd - 1; // to handle the t = 1 special case 
			} else {
				for( int j = 0; j < knots.Length; j++ ) { // find relevant interval
					if( knots[j] <= u && u < knots[j + 1] ) {
						i = j;
						break;
					}
				}
			}

			return Eval( i, u );
		}

		[NonSerialized] Vector2[] evalBuffer;

		// based on https://en.wikipedia.org/wiki/De_Boor%27s_algorithm
		/// <summary>Returns the point at the given De-Boor recursion depth, knot interval and parameter space u-value</summary>
		/// <param name="k">The index of the knot interval our u-value is inside</param>
		/// <param name="u">A value in parameter space. Note: this value has to be within the internal knot interval</param>
		public Vector2 Eval( int k, float u ) {
			// make sure our buffer is ready
			if( evalBuffer == null || evalBuffer.Length != degree + 1 )
				evalBuffer = new Vector2[degree + 1];

			// populate points in the buffer
			for( int i = 0; i < degree + 1; i++ )
				evalBuffer[i] = points[i + k - degree];

			// calculate each layer until we've got only one point left
			for( int r = 1; r < degree + 1; r++ ) {
				for( int j = degree; j > r - 1; j-- ) {
					float alpha = Mathfs.InverseLerpSafe( knots[j + k - degree], knots[j + 1 + k - r], u );
					evalBuffer[j] = Vector2.LerpUnclamped( evalBuffer[j - 1], evalBuffer[j], alpha );
				}
			}

			return evalBuffer[degree];
		}

		#endregion

		#region Basis Curves

		/// <summary>Returns the basis curve of a given point (by index), at the given parameter space u-value</summary>
		/// <param name="point">The point to get the basis curve of</param>
		/// <param name="u">A value in parameter space. Note: this value has to be within the internal knot interval</param>
		public float GetPointWeightAtKnotValue( int point, float u ) => EvalBasis( point, Order, u );

		// Cox–de Boor recursion
		float EvalBasis( int p, int k, float u ) {
			// p = the point to get the basis curve for
			// k = depth of recursion, where 0 = base knots. generally you start with k = Order
			// u = knot value
			k--;
			if( k == 0 ) {
				if( p == knots.Length - 2 ) // todo: verify this, I just hacked it in, seems sus af
					return knots[p] <= u && u <= knots[p + 1] ? 1 : 0;
				return knots[p] <= u && u < knots[p + 1] ? 1 : 0;
			}

			return W( p, k, u ) * EvalBasis( p, k, u ) + ( 1f - W( p + 1, k, u ) ) * EvalBasis( p + 1, k, u );
		}

		float W( int i, int k, float t ) {
			float den = knots[i + k] - knots[i];
			if( den == 0 )
				return 0;
			return ( t - knots[i] ) / den;
		}

		#endregion

		#region esoteric trashmath

		// esoteric as heck:
		public Vector2 GetPointKnotAndWeightByLocalSpan( int point, float t ) {
			float knotStart = knots[point];
			float knotEnd = knots[point + degree + 1];
			float u = Mathfs.Lerp( knotStart, knotEnd, t );
			float v = EvalBasis( point, Order, u );
			return new Vector2( u, v );
		}

		/* old point eval:
		public Vector2 Eval( int r, int i, float u ) {
			if( r == 0 )
				return points[i];
			Vector2 pA = Eval( r - 1, i - 1, u );
			Vector2 pB = Eval( r - 1, i, u );
			return Vector2.LerpUnclamped( pA, pB, Alpha( i, i + Order - r, u ) );
		}

		float Alpha( int a, int b, float u ) {
			float den = knots[b] - knots[a];
			if( den == 0 )
				return 0;
			return ( u - knots[a] ) / den;
		}*/

		/*
		public Vector2 GetPointCollect( float t, ICollection<(Vector2 a, Vector2 b)> lines ) {
			// remap 0-1 to knot space
			float tKnot = Mathfs.Lerp( knots[degree], knots[knots.Length - degree - 1], t );

			int i = 0;
			if( tKnot >= knots.Length - degree - 1 ) {
				i = knots.Length - degree - 2; // to handle the t = 1 special case 
			} else {
				for( int j = 0; j < knots.Length; j++ ) { // find relevant interval
					if( knots[j] <= tKnot && tKnot < knots[j + 1] ) {
						i = j;
						break;
					}
				}
			}

			return EvalCollect( degree, i, tKnot, lines );
		}

		public Vector2 GetPointCollectByKnotIndex( int i, ICollection<(Vector2 a, Vector2 b)> lines ) {
			int kRef = i.Clamp( FirstInteriorKnotIndex, LastInteriorKnotIndex - 1 );
			return EvalCollect( degree, kRef, knots[i], lines );
		}

		Vector2 EvalCollect( int r, int i, float t, ICollection<(Vector2, Vector2)> lines ) {
			if( r == 0 )
				return points[i];
			Vector2 pA = EvalCollect( r - 1, i - 1, t, lines );
			Vector2 pB = EvalCollect( r - 1, i, t, lines );
			Vector2 pt = Vector2.Lerp( pA, pB, Alpha( i, r, t ) );
			if( r > 1 )
				lines.Add( ( pA, pB ) );
			return pt;
		}
		
		public Vector2 GetDerivativePoint(float t) {
			const float du = 0.0001f;
			float u = GetKnotValueAt( t );
			float ua = (u-du/2).AtLeast( InternalKnotValueStart );
			float ub = (u+du/2).AtMost( InternalKnotValueEnd );
			Vector2 a = GetPointByKnotValue( ua );
			Vector2 b = GetPointByKnotValue( ub );
			return ( b - a ) / (ub-ua);
		}
		*/

		#endregion


	}

}