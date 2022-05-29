// by Freya Holmér (https://github.com/FreyaHolmer/Mathfs)

using UnityEngine;

namespace Freya {

	/// <summary>A helper class to let you sample a curve by uniform parameter values, t-values or by distance</summary>
	public class UniformCurveSampler {

		/// <summary>The number of distance samples used when calculating the cumulative distance table</summary>
		public readonly int resolution;

		/// <summary>Cumulative distance samples, where the first element is 0 and the last element is the total length of the curve</summary>
		public readonly float[] cumulativeDistances;

		/// <summary>Returns the approximate length of this curve. This property doesn't have to recalculate length, since it's already stored in the cumulative distances array</summary>
		public float CurveIntervalLength => cumulativeDistances[resolution - 1];

		FloatRange paramInterval;

		/// <summary>The t-value range in which we've calculated the uniform sampler for</summary>
		public FloatRange ParamInterval => paramInterval;

		// The way the point recalculation works right now is pretty naive, and doesn't handle extreme acceleration very well.
		// Right now you need around 30 samples to properly sample, which, is a lot

		#region Constructors & Point recalc

		/// <summary>Creates a sampler that can be used to sample a curve by distance or by uniform t-values.
		/// You'll need to call sampler.Recalculate(curve) to recalculate if the curve changes shape after this points.
		/// Recommended resolution for animation: [8-16]
		/// Recommended resolution for even point spacing: [16-50]</summary>
		/// <param name="curve">The curve to use when sampling</param>
		/// <param name="interval">The interval you want to uniformly sample within</param>
		/// <param name="resolution">The accuracy of this sampler.
		/// Higher values are more accurate, but are more costly to calculate for every new curve shape</param>
		public UniformCurveSampler( Polynomial2D curve, FloatRange interval, int resolution = 12 ) {
			this.resolution = resolution;
			cumulativeDistances = new float[resolution];
			Recalculate( curve, interval );
		}

		/// <inheritdoc cref="UniformCurveSampler(Polynomial2D,FloatRange,int)"/>
		public UniformCurveSampler( Polynomial2D curve, int resolution = 12 ) : this( curve, FloatRange.unit, resolution ) {
		}

		/// <inheritdoc cref="UniformCurveSampler(Polynomial2D,FloatRange,int)"/>
		public UniformCurveSampler( Polynomial3D curve, FloatRange interval, int resolution = 12 ) {
			this.resolution = resolution;
			cumulativeDistances = new float[resolution];
			Recalculate( curve, interval );
		}

		/// <inheritdoc cref="UniformCurveSampler(Polynomial3D,FloatRange,int)"/>
		public UniformCurveSampler( Polynomial3D curve, int resolution = 12 ) : this( curve, FloatRange.unit, resolution ) {
		}

		/// <summary>Recalculates the internal lookup table so that the curve can be sampled by distance or by uniform t-values.
		/// Only call this before sampling a different curve, or if the curve has changed shape since last time it was calculated</summary>
		/// <param name="curve">The curve to use when sampling</param>
		/// <param name="interval">The interval you want to uniformly sample within</param>
		public void Recalculate( Polynomial2D curve, FloatRange interval ) {
			this.paramInterval = interval;
			float cumulativeLength = 0;
			Vector2 prevPt = curve.Eval( paramInterval.a );
			cumulativeDistances[0] = 0;
			for( int i = 1; i < resolution; i++ ) {
				Vector2 pt = curve.Eval( paramInterval.Lerp( i / ( resolution - 1f ) ) );
				cumulativeLength += Vector2.Distance( prevPt, pt );
				cumulativeDistances[i] = cumulativeLength;
				prevPt = pt;
			}
		}

		/// <inheritdoc cref="Recalculate(Polynomial2D,FloatRange)"/>
		public void Recalculate( Polynomial2D curve ) => Recalculate( curve, FloatRange.unit );

		/// <inheritdoc cref="Recalculate(Polynomial2D,FloatRange)"/>
		public void Recalculate( Polynomial3D curve, FloatRange interval ) {
			this.paramInterval = interval;
			float cumulativeLength = 0;
			Vector3 prevPt = curve.Eval( paramInterval.a );
			cumulativeDistances[0] = 0;
			for( int i = 1; i < resolution; i++ ) {
				Vector3 pt = curve.Eval( paramInterval.Lerp( i / ( resolution - 1f ) ) );
				cumulativeLength += Vector3.Distance( prevPt, pt );
				cumulativeDistances[i] = cumulativeLength;
				prevPt = pt;
			}
		}

		/// <inheritdoc cref="Recalculate(Polynomial3D,FloatRange)"/>
		public void Recalculate( Polynomial3D curve ) => Recalculate( curve, FloatRange.unit );

		#endregion

		float GetParamAtSegmentTValue( float t ) => paramInterval.InverseLerp( t );

		/// <summary>Converts a t-value along the segment to a raw parameter value. Useful to uniformly sample a curve</summary>
		/// <param name="t">A value from 0 to 1 representing uniform position along the curve interval</param>
		public float UniformTToParam( float t ) => DistanceToParam( t * CurveIntervalLength );
		
		/// <summary>Converts a uniform parameter value to a raw parameter value. Useful to uniformly sample a curve</summary>
		/// <param name="u">The input parameter for uniform position along the curve</param>
		public float UniformParamToParam( float u ) => DistanceToParam( paramInterval.InverseLerp( u ) * CurveIntervalLength );

		/// <summary>Converts a distance value (relative to the start of the interval) to a parameter value. Useful to sample a curve by distance</summary>
		/// <param name="distance">The distance along the curve segment parameter interval at which you'd like to get the parameter value for</param>
		public float DistanceToParam( float distance ) {
			// check if the value is within the length of the curve
			if( distance.Between( 0, CurveIntervalLength ) ) {
				// find which two distance values our input distance lies between
				for( int i = 0; i < resolution - 1; i++ ) {
					float distPrev = cumulativeDistances[i];
					float distNext = cumulativeDistances[i + 1];
					if( distance.Within( distPrev, distNext ) ) { // check if our input distance lies between the two distances
						// get t-values at the samples
						float tPrev = i / ( resolution - 1f );
						float tNext = ( i + 1 ) / ( resolution - 1f );
						// remap the distance range to the t-value range
						float tLocal = distance.Remap( distPrev, distNext, tPrev, tNext );
						return paramInterval.Lerp( tLocal );
					}
				}
			}

			// distance is outside the length of the curve - extrapolate values outside
			return paramInterval.Lerp( distance / CurveIntervalLength );
		}


	}

}