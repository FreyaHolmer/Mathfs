// by Freya Holmér (https://github.com/FreyaHolmer/Mathfs)

using UnityEngine;

namespace Freya {

	/// <summary>A helper class to let you sample a bezier curve by uniform T-values or by distance</summary>
	public class BezierSampler {

		/// <summary>The number of distance samples used when calculating the cumulative distance table</summary>
		public readonly int resolution;

		/// <summary>Cumulative distance samples, where the first element is 0 and the last element is the total length of the curve</summary>
		public readonly float[] cumulativeDistances;

		/// <summary>Returns the approximate length of this curve. This property doesn't have to recalculate length, since it's already stored in the cumulative distances array</summary>
		public float CurveLength => cumulativeDistances[resolution - 1];

		// The way the point recalculation works right now is pretty naive, and doesn't handle extreme acceleration very well.
		// Right now you need around 30 samples to properly sample, which, is a lot

		#region Constructors & Point recalc

		/// <summary>Creates a sampler that can be used to sample a bezier curve by distance or by uniform t-values.
		/// You'll need to call sampler.Recalculate(bezier) to recalculate if the curve changes shape after this points.
		/// Recommended resolution for animation: [8-16]
		/// Recommended resolution for even point spacing: [16-50]</summary>
		/// <param name="bezier">The curve to use when sampling</param>
		/// <param name="resolution">The accuracy of this sampler.
		/// Higher values are more accurate, but are more costly to calculate for every new bezier shape</param>
		public BezierSampler( BezierCubic2D bezier, int resolution = 12 ) {
			this.resolution = resolution;
			cumulativeDistances = new float[resolution];
			Recalculate( bezier );
		}

		/// <summary>Creates a sampler that can be used to sample a bezier curve by distance or by uniform t-values.
		/// You'll need to call sampler.Recalculate(bezier) to recalculate if the curve changes shape after this points.
		/// Recommended resolution for animation: [8-16]
		/// Recommended resolution for even point spacing: [16-50]</summary>
		/// <param name="bezier">The curve to use when sampling</param>
		/// <param name="resolution">The accuracy of this sampler.
		/// Higher values are more accurate, but are more costly to calculate for every new bezier shape</param>
		public BezierSampler( BezierCubic3D bezier, int resolution = 12 ) {
			this.resolution = resolution;
			cumulativeDistances = new float[resolution];
			Recalculate( bezier );
		}

		/// <summary>Recalculates the internal lookup table so that the bezier can be sampled by distance or by uniform t-values.
		/// Only call this before sampling a different curve, or if the curve has changed shape since last time it was calculated</summary>
		/// <param name="bezier">The curve to use when sampling</param>
		public void Recalculate( BezierCubic2D bezier ) {
			float cumulativeLength = 0;
			Vector2 prevPt = bezier.P0;
			cumulativeDistances[0] = 0;
			for( int i = 1; i < resolution; i++ ) { // todo: could optimize by moving all points so that p0 = (0,0)
				Vector2 pt = bezier.GetPoint( i / ( resolution - 1f ) );
				cumulativeLength += Vector2.Distance( prevPt, pt );
				cumulativeDistances[i] = cumulativeLength;
				prevPt = pt;
			}
		}

		/// <summary>Recalculates the internal lookup table so that the bezier can be sampled by distance or by uniform t-values.
		/// Only call this before sampling a different curve, or if the curve has changed shape since last time it was calculated</summary>
		/// <param name="bezier">The curve to use when sampling</param>
		public void Recalculate( BezierCubic3D bezier ) {
			float cumulativeLength = 0;
			Vector3 prevPt = bezier.P0;
			cumulativeDistances[0] = 0;
			for( int i = 1; i < resolution; i++ ) { // todo: could optimize by moving all points so that p0 = (0,0)
				Vector3 pt = bezier.GetPoint( i / ( resolution - 1f ) );
				cumulativeLength += Vector3.Distance( prevPt, pt );
				cumulativeDistances[i] = cumulativeLength;
				prevPt = pt;
			}
		}

		#endregion

		/// <summary>Converts a uniform t-value to a t-value. Useful to uniformly sample a bezier curve</summary>
		/// <param name="tUniform">A value from 0 to 1 representing uniform distance along the spline</param>
		public float UniformToT( float tUniform ) => DistanceToT( tUniform * CurveLength );

		/// <summary>Converts a distance value to a t-value. Useful to sample a bezier curve by distance</summary>
		/// <param name="distance">The distance along the bezier curve at which you'd like to get the t-value for</param>
		public float DistanceToT( float distance ) {
			// check if the value is within the length of the curve
			if( distance.Between( 0, CurveLength ) ) {
				// find which two distance values our input distance lies between
				for( int i = 0; i < resolution - 1; i++ ) {
					float distPrev = cumulativeDistances[i];
					float distNext = cumulativeDistances[i + 1];
					if( distance.Within( distPrev, distNext ) ) { // check if our input distance lies between the two distances
						// get t-values at the samples
						float tPrev = i / ( resolution - 1f );
						float tNext = ( i + 1 ) / ( resolution - 1f );
						// remap the distance range to the t-value range
						return distance.Remap( distPrev, distNext, tPrev, tNext );
					}
				}
			}

			// distance is outside the length of the curve - extrapolate values outside
			return distance / CurveLength;
		}


	}

}