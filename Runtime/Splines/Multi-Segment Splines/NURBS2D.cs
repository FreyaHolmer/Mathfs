using System;
using UnityEngine;

namespace Freya {

	public class NURBS2D {

		public Vector2[] points;
		public float[] knots;
		public float[] weights = null;

		public int degree;

		public bool Rational => weights != null;
		public int PointCount => points.Length;
		public int Order => degree + 1;
		public int KnotCount => degree + PointCount + 1;
		public int SegmentCount => KnotCount - degree * 2 - 1;

		public static NURBS2D GetUniformBSpline( Vector2[] points, int degree = 3, bool open = true ) {
			int ptCount = points.Length;
			float[] knots = SplineUtils.GenerateUniformKnots( degree, ptCount, open );
			return new NURBS2D( points, knots, null, degree );
		}

		public static float[] GetUnweightedWeights( int count ) {
			float[] weights = new float[count];
			for( int i = 0; i < count; i++ )
				weights[i] = 1;
			return weights;
		}


		public NURBS2D( Vector2[] points, float[] knots, float[] weights, int degree = 3 ) {
			this.points = points;
			this.knots = knots;
			this.degree = degree;
			if( weights != null && weights.Length != points.Length )
				throw new ArgumentException( $"The weights array has to match the number of points. Got an array of {weights.Length} weights, expected ${points.Length}", nameof(weights) );
			this.weights = weights;
			if( knots.Length != KnotCount )
				throw new ArgumentException( $"The knots array has to be of length (degree+pointCount+1). Got an array of {knots.Length} knots, expected ${KnotCount}", nameof(knots) );
			this.knots = knots;
		}

		public Vector2 GetPointByKnotValue( float t ) {
			bool weighted = Rational;
			Vector2 sum = default;

			float norm = 0;
			for( int i = 0; i < PointCount; i++ ) { // todo: unnecessary, don't do all points
				float basis = Basis( i, Order, t );
				if( weighted ) norm += ( basis *= weights[i] );
				sum += points[i] * basis;
			}

			if( weighted )
				sum /= norm;

			return sum;
		}

		public Vector2 GetPoint( float t ) {
			t = Mathfs.Lerp( knots[degree], knots[knots.Length - degree - 1], t );
			return GetPointByKnotValue( t );
		}

		float W( int i, int k, float t ) {
			float den = knots[i + k] - knots[i];
			if( den == 0 )
				return 0;
			return ( t - knots[i] ) / den;
		}

		public float Basis( int i, int k, float t ) {
			k--;
			if( k == 0 ) {
				if( i == knots.Length - 2 ) // weird exception
					return knots[i] <= t && t <= knots[i + 1] ? 1 : 0;
				return knots[i] <= t && t < knots[i + 1] ? 1 : 0;
			}

			return W( i, k, t ) * Basis( i, k, t ) + ( 1f - W( i + 1, k, t ) ) * Basis( i + 1, k, t );
		}

		// todo: wasteful as heck
		public float WeightedBasis( int i, int k, float t ) {
			bool weighted = Rational;
			float norm = 0;
			float targetBasis = 0;
			for( int j = 0; j < PointCount; j++ ) {
				float basis = Basis( j, Order, t );
				if( weighted )
					norm += ( basis *= weights[j] );
				if( j == i )
					targetBasis = basis;
			}

			return targetBasis / norm;
		}


	}

}