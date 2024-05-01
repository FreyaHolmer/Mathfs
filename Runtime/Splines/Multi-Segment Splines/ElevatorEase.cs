using System;
using Freya;

public class ElevatorEase {

	public enum Mode {
		FastStartPolynomial,
		Trigonometric
	}

	public readonly Mode mode;
	public readonly float maxDistance;
	public readonly float contractSpeed;
	public readonly float accDuration;

	public readonly float t1;
	public readonly float t2;
	public readonly float tEnd;
	public readonly float v;

	// linear section
	float g_c0, g_c1;

	// polynomial coefficients used by Mode.FastStartPolynomial
	float f_c3, f_c4, f_c5; // ease in
	float h_c4, h_c5; // ease out (relative)

	// constants used by Mode.Trigonometric
	float c_xsq, c_trigScale, c_trigInner;

	public ElevatorEase( Mode mode, float maxDistance, float contractSpeed, float accDuration ) {
		this.mode = mode;
		this.maxDistance = maxDistance;
		this.contractSpeed = contractSpeed;
		this.accDuration = accDuration;

		// derived things
		t1 = accDuration;
		t2 = maxDistance / contractSpeed;
		tEnd = t1 + t2;
		v = contractSpeed;
		float tHalf = tEnd / 2;
		if( accDuration >= tHalf ) {
			// no linear section - adjust accordingly
			t1 = t2 = tHalf;
			v = 2 * ( maxDistance / tEnd );
		}

		// calculate coefficients
		g_c1 = v;
		if( mode == Mode.FastStartPolynomial ) {
			g_c0 = -2 * v * t1 / 5;
			float t1_2 = t1 * t1;
			float t1_3 = t1_2 * t1;
			float t1_4 = t1_2 * t1_2;
			f_c3 = 2 * v / t1_2;
			f_c4 = 2 * v / -t1_3;
			f_c5 = 3 * v / ( 5 * t1_4 );
			h_c4 = v / -t1_3;
			h_c5 = -3 * v / ( 5 * t1_4 );
		} else if( mode == Mode.Trigonometric ) {
			g_c0 = -v * t1 / 2;
			c_xsq = v / ( 2 * t1 );
			c_trigScale = v * t1 / ( Mathfs.TAU * Mathfs.TAU );
			c_trigInner = Mathfs.TAU / t1;
		}
	}

	float EaseIn( float t ) {
		float x2 = t * t;

		if( mode == Mode.FastStartPolynomial ) {
			float x3 = x2 * t;
			float x4 = x2 * x2;
			float x5 = x3 * x2;
			return f_c3 * x3 + f_c4 * x4 + f_c5 * x5;
		} else if( mode == Mode.Trigonometric ) {
			return c_xsq * x2 + c_trigScale * ( MathF.Cos( c_trigInner * t ) - 1 );
		}
		throw new NotImplementedException();
	}

	float Linear( float t ) => g_c0 + g_c1 * t;

	float EaseOut( float t ) {
		if( mode == Mode.FastStartPolynomial ) {
			// this one is asymmetric, and needs special handling
			float x = t - tEnd; // remap to relative
			float x2 = x * x;
			float x4 = x2 * x2;
			float x5 = x4 * x;
			return h_c4 * x4 + h_c5 * x5 + maxDistance;
		}

		// same as ease-in but reversed
		return maxDistance - EaseIn( tEnd - t );
	}

	public float Eval( float t ) {
		// check extremes:
		if( t <= 0 )
			return 0;
		if( t >= tEnd )
			return maxDistance;
		// check functions:
		if( t < t1 )
			return EaseIn( t );
		if( t > t2 )
			return EaseOut( t );
		return Linear( t );
	}

}