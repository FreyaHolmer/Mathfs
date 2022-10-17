// by Freya Holmér (https://github.com/FreyaHolmer/Mathfs)
// a lot of stuff here made possible by this excellent writeup on bezier curves: https://pomax.github.io/bezierinfo/

using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using static Freya.Mathfs;

namespace Freya {

	// Bezier math
	// A lot of the following code is unrolled into floats and components for performance reasons.
	// It's much faster than keeping the more readable function calls and vector types unfortunately

	/// <summary>An optimized 2D cubic bezier curve, with 4 control points</summary>
	[Serializable] public struct BezierCubic2D : IParamCurve3Diff<Vector2> {

		const MethodImplOptions INLINE = MethodImplOptions.AggressiveInlining;

		/// <summary>Creates a cubic bezier curve, from 4 control points</summary>
		/// <param name="p0">The starting point of the curve</param>
		/// <param name="p1">The second control point of the curve, sometimes called the start tangent point</param>
		/// <param name="p2">The third control point of the curve, sometimes called the end tangent point</param>
		/// <param name="p3">The end point of the curve</param>
		public BezierCubic2D( Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3 ) {
			( this.p0, this.p1, this.p2, this.p3 ) = ( p0, p1, p2, p3 );
			validCoefficients = false;
			c3 = c2 = c1 = default;
		}

		#region Control Points

		[SerializeField] Vector2 p0, p1, p2, p3; // the points of the curve

		/// <summary>The starting point of the curve</summary>
		public Vector2 P0 {
			[MethodImpl( INLINE )] get => p0;
			[MethodImpl( INLINE )] set => _ = ( p0 = value, validCoefficients = false );
		}

		/// <summary>The second control point of the curve, sometimes called the start tangent point</summary>
		public Vector2 P1 {
			[MethodImpl( INLINE )] get => p1;
			[MethodImpl( INLINE )] set => _ = ( p1 = value, validCoefficients = false );
		}

		/// <summary>The third control point of the curve, sometimes called the end tangent point</summary>
		public Vector2 P2 {
			[MethodImpl( INLINE )] get => p2;
			[MethodImpl( INLINE )] set => _ = ( p2 = value, validCoefficients = false );
		}

		/// <summary>The end point of the curve</summary>
		public Vector2 P3 {
			[MethodImpl( INLINE )] get => p3;
			[MethodImpl( INLINE )] set => _ = ( p3 = value, validCoefficients = false );
		}

		/// <summary>Get or set a control point position by index. Valid indices: 0, 1, 2 or 3</summary>
		public Vector2 this[ int i ] {
			get {
				switch( i ) {
					case 0:  return P0;
					case 1:  return P1;
					case 2:  return P2;
					case 3:  return P3;
					default: throw new ArgumentOutOfRangeException( nameof(i), $"Index has to be in the 0 to 3 range, and I think {i} is outside that range you know" );
				}
			}
			set {
				switch( i ) {
					case 0:
						P0 = value;
						break;
					case 1:
						P1 = value;
						break;
					case 2:
						P2 = value;
						break;
					case 3:
						P3 = value;
						break;
					default: throw new ArgumentOutOfRangeException( nameof(i), $"Index has to be in the 0 to 3 range, and I think {i} is outside that range you know" );
				}
			}
		}

		#endregion

		#region Coefficients

		[NonSerialized] bool validCoefficients; // inverted isDirty flag (can't default to true in structs)
		[NonSerialized] Vector2 c3, c2, c1; // cached coefficients for fast evaluation. c0 = p0

		// Coefficient Calculation
		[MethodImpl( INLINE )] void ReadyCoefficients() {
			if( validCoefficients )
				return; // no need to update
			validCoefficients = true;
			c3.x = 3 * ( p1.x - p2.x ) + ( p3.x - p0.x );
			c2.x = 3 * ( p0.x - p1.x + p2.x - p1.x );
			c1.x = 3 * ( p1.x - p0.x );
			c3.y = 3 * ( p1.y - p2.y ) + ( p3.y - p0.y );
			c2.y = 3 * ( p0.y - p1.y + p2.y - p1.y );
			c1.y = 3 * ( p1.y - p0.y );
		}

		/// <summary>The constant coefficient when evaluating this curve in the form C3*t³ + C2*t² + C1*t + C0</summary>
		public Vector2 C0 {
			[MethodImpl( INLINE )] get => p0;
		}

		/// <summary>The linear coefficient when evaluating this curve in the form C3*t³ + C2*t² + C1*t + C0</summary>
		public Vector2 C1 {
			[MethodImpl( INLINE )] get {
				ReadyCoefficients();
				return c1;
			}
		}

		/// <summary>The quadratic coefficient when evaluating this curve in the form C3*t³ + C2*t² + C1*t + C0</summary>
		public Vector2 C2 {
			[MethodImpl( INLINE )] get {
				ReadyCoefficients();
				return c2;
			}
		}

		/// <summary>The cubic coefficient when evaluating this curve in the form C3*t³ + C2*t² + C1*t + C0</summary>
		public Vector2 C3 {
			[MethodImpl( INLINE )] get {
				ReadyCoefficients();
				return c3;
			}
		}

		/// <summary>The polynomial coefficients in the form c3*t³ + c2*t² + c1*t + c0</summary>
		[MethodImpl( INLINE )] public (Vector2 c3, Vector2 c2, Vector2 c1, Vector2 c0) GetCoefficients() {
			ReadyCoefficients();
			return ( c3, c2, c1, p0 );
		}

		#endregion

		// Object comparison stuff

		#region Object Comparison & ToString

		public static bool operator ==( BezierCubic2D a, BezierCubic2D b ) => a.P0 == b.P0 && a.P1 == b.P1 && a.P2 == b.P2 && a.P3 == b.P3;
		public static bool operator !=( BezierCubic2D a, BezierCubic2D b ) => !( a == b );
		public bool Equals( BezierCubic2D other ) => P0.Equals( other.P0 ) && P1.Equals( other.P1 ) && P2.Equals( other.P2 ) && P3.Equals( other.P3 );
		public override bool Equals( object obj ) => obj is BezierCubic2D other && Equals( other );

		public override int GetHashCode() {
			unchecked {
				int hashCode = P0.GetHashCode();
				hashCode = ( hashCode * 397 ) ^ P1.GetHashCode();
				hashCode = ( hashCode * 397 ) ^ P2.GetHashCode();
				hashCode = ( hashCode * 397 ) ^ P3.GetHashCode();
				return hashCode;
			}
		}

		public override string ToString() => $"{P0}, {P1}, {P2}, {P3}";

		#endregion

		#region Type Casting

		/// <summary>Returns this bezier curve in 3D, where z = 0</summary>
		/// <param name="bezierCubic2D">The 2D curve to cast</param>
		public static explicit operator BezierCubic3D( BezierCubic2D bezierCubic2D ) {
			return new BezierCubic3D( bezierCubic2D.P0, bezierCubic2D.P1, bezierCubic2D.P2, bezierCubic2D.P3 );
		}

		#endregion

		// Base properties - Points, Derivatives & Tangents

		#region Core IParamCurve Implementations

		public int Degree {
			[MethodImpl( INLINE )] get => 3;
		}
		public int Count {
			[MethodImpl( INLINE )] get => 4;
		}

		[MethodImpl( INLINE )] public Vector2 GetStartPoint() => p0;
		[MethodImpl( INLINE )] public Vector2 GetEndPoint() => p3;

		[MethodImpl( INLINE )] public Vector2 GetPoint( float t ) {
			ReadyCoefficients();
			float t2 = t * t;
			float t3 = t2 * t;
			return new Vector2( t3 * c3.x + t2 * c2.x + t * c1.x + p0.x, t3 * c3.y + t2 * c2.y + t * c1.y + p0.y );
		}

		[MethodImpl( INLINE )] public Vector2 GetDerivative( float t ) {
			ReadyCoefficients();
			float t2 = t * t;
			return new Vector2( 3 * t2 * c3.x + 2 * t * c2.x + c1.x, 3 * t2 * c3.y + 2 * t * c2.y + c1.y );
		}

		[MethodImpl( INLINE )] public Vector2 GetSecondDerivative( float t ) {
			ReadyCoefficients();
			return new Vector2( 6 * t * c3.x + 2 * c2.x, 6 * t * c3.y + 2 * c2.y );
		}

		[MethodImpl( INLINE )] public Vector2 GetThirdDerivative( float t = 0 ) {
			ReadyCoefficients();
			return new Vector2( 6 * c3.x, 6 * c3.y );
		}

		#endregion

		#region Point Components

		/// <summary>Returns the X coordinate at the given t-value on the curve</summary>
		/// <param name="t">The t-value along the curve to sample</param>
		[MethodImpl( INLINE )] public float GetPointX( float t ) {
			ReadyCoefficients();
			float t2 = t * t;
			float t3 = t2 * t;
			return t3 * c3.x + t2 * c2.x + t * c1.x + p0.x;
		}

		/// <summary>Returns the Y coordinate at the given t-value on the curve</summary>
		/// <param name="t">The t-value along the curve to sample</param>
		[MethodImpl( INLINE )] public float GetPointY( float t ) {
			ReadyCoefficients();
			float t2 = t * t;
			float t3 = t2 * t;
			return t3 * c3.y + t2 * c2.y + t * c1.y + p0.y;
		}

		/// <summary>Returns a component of the coordinate at the given t-value on the curve</summary>
		/// <param name="component">Which component of the coordinate to return. 0 is X, 1 is Y</param>
		/// <param name="t">The t-value along the curve to sample</param>
		public float GetPointComponent( int component, float t ) {
			switch( component ) {
				case 0:  return GetPointX( t );
				case 1:  return GetPointY( t );
				default: throw new ArgumentOutOfRangeException( nameof(component), "component has to be either 0 or 1" );
			}
		}

		#endregion

		// Whole-curve properties & functions

		#region Interpolation

		/// <summary>Returns linear blend between two bézier curves</summary>
		/// <param name="a">The first curve</param>
		/// <param name="b">The second curve</param>
		/// <param name="t">A value from 0 to 1 to blend between <c>a</c> and <c>b</c></param>
		public static BezierCubic2D Lerp( BezierCubic2D a, BezierCubic2D b, float t ) {
			return new BezierCubic2D(
				Vector2.LerpUnclamped( a.p0, b.p0, t ),
				Vector2.LerpUnclamped( a.p1, b.p1, t ),
				Vector2.LerpUnclamped( a.p2, b.p2, t ),
				Vector2.LerpUnclamped( a.p3, b.p3, t )
			);
		}

		/// <summary>Returns blend between two bézier curves,
		/// where the endpoints are linearly interpolated,
		/// while the tangents are spherically interpolated relative to their corresponding endpoint</summary>
		/// <param name="a">The first curve</param>
		/// <param name="b">The second curve</param>
		/// <param name="t">A value from 0 to 1 to blend between <c>a</c> and <c>b</c></param>
		public static BezierCubic2D Slerp( BezierCubic2D a, BezierCubic2D b, float t ) {
			Vector2 p0 = Vector2.LerpUnclamped( a.p0, b.p0, t );
			Vector2 p3 = Vector2.LerpUnclamped( a.p3, b.p3, t );
			return new BezierCubic2D(
				p0,
				p0 + (Vector2)Vector3.SlerpUnclamped( a.p1 - a.p0, b.p1 - b.p0, t ),
				p3 + (Vector2)Vector3.SlerpUnclamped( a.p2 - a.p3, b.p2 - b.p3, t ),
				p3
			);
		}

		#endregion

		#region Splitting

		/// <summary>Splits this curve at the given t-value, into two curves of the exact same shape</summary>
		/// <param name="t">The t-value along the curve to sample</param>
		public (BezierCubic2D pre, BezierCubic2D post) Split( float t ) {
			Vector2 a = new Vector2(
				P0.x + ( P1.x - P0.x ) * t,
				P0.y + ( P1.y - P0.y ) * t );
			float bx = P1.x + ( P2.x - P1.x ) * t;
			float by = P1.y + ( P2.y - P1.y ) * t;
			Vector2 c = new Vector2(
				P2.x + ( P3.x - P2.x ) * t,
				P2.y + ( P3.y - P2.y ) * t );
			Vector2 d = new Vector2(
				a.x + ( bx - a.x ) * t,
				a.y + ( by - a.y ) * t );
			Vector2 e = new Vector2(
				bx + ( c.x - bx ) * t,
				by + ( c.y - by ) * t );
			Vector2 p = new Vector2(
				d.x + ( e.x - d.x ) * t,
				d.y + ( e.y - d.y ) * t );
			return ( new BezierCubic2D( P0, a, d, p ), new BezierCubic2D( p, e, c, P3 ) );
		}

		#endregion

		#region Bounds

		/// <summary>Returns the tight axis-aligned bounds of the curve</summary>
		public Rect GetBounds() {
			// first and last points are always included
			Vector2 min = Vector2.Min( P0, P3 );
			Vector2 max = Vector2.Max( P0, P3 );

			void Encapsulate( int axis, float value ) {
				min[axis] = Min( min[axis], value );
				max[axis] = Max( max[axis], value );
			}

			for( int i = 0; i < 2; i++ ) {
				ResultsMax2<float> extrema = GetLocalExtremaPoints( i );
				for( int j = 0; j < extrema.count; j++ )
					Encapsulate( i, extrema[j] );
			}

			return new Rect( min.x, min.y, max.x - min.x, max.y - min.y );
		}

		#endregion

		#region Project Point

		/// <summary>Returns the (approximate) point on the curve closest to the input point</summary>
		/// <param name="point">The point to project against the curve</param>
		/// <param name="initialSubdivisions">Recommended range: [8-32]. More subdivisions will be more accurate, but more expensive.
		/// This is how many subdivisions to split the curve into, to find candidates for the closest point.
		/// If your curves are complex, you might need to use around 16 subdivisions.
		/// If they are usually very simple, then around 8 subdivisions is likely fine</param>
		/// <param name="refinementIterations">Recommended range: [3-6]. More iterations will be more accurate, but more expensive.
		/// This is how many times to refine the initial guesses, using Newton's method. This converges rapidly, so high numbers are generally not necessary</param>
		public Vector2 ProjectPoint( Vector2 point, int initialSubdivisions = 16, int refinementIterations = 4 ) => ProjectPoint( point, out _, initialSubdivisions, refinementIterations );

		struct PointProjectSample {
			public float t;
			public float distDeltaSq;
			public Vector2 f;
			public Vector2 fp;
		}

		static PointProjectSample[] pointProjectGuesses = { default, default, default };

		/// <summary>Returns the (approximate) point on the curve closest to the input point</summary>
		/// <param name="point">The point to project against the curve</param>
		/// <param name="t">The t-value at the projected point on the curve</param>
		/// <param name="initialSubdivisions">Recommended range: [8-32]. More subdivisions will be more accurate, but more expensive.
		/// This is how many subdivisions to split the curve into, to find candidates for the closest point.
		/// If your curves are complex, you might need to use around 16 subdivisions.
		/// If they are usually very simple, then around 8 subdivisions is likely fine</param>
		/// <param name="refinementIterations">Recommended range: [3-6]. More iterations will be more accurate, but more expensive.
		/// This is how many times to refine the initial guesses, using Newton's method. This converges rapidly, so high numbers are generally not necessary</param>
		public Vector2 ProjectPoint( Vector2 point, out float t, int initialSubdivisions = 16, int refinementIterations = 4 ) {
			// define a bezier relative to the test point
			BezierCubic2D bez = new BezierCubic2D( P0 - point, P1 - point, P2 - point, P3 - point );

			PointProjectSample SampleDistSqDelta( float tSmp ) {
				PointProjectSample s = new PointProjectSample { t = tSmp };
				s.f = bez.GetPoint( tSmp );
				s.fp = bez.GetDerivative( tSmp );
				s.distDeltaSq = Vector2.Dot( s.f, s.fp );
				return s;
			}

			// find initial candidates
			int candidatesFound = 0;
			PointProjectSample prevSmp = SampleDistSqDelta( 0 );

			for( int i = 1; i < initialSubdivisions; i++ ) {
				float ti = i / ( initialSubdivisions - 1f );
				PointProjectSample smp = SampleDistSqDelta( ti );
				if( SignAsInt( smp.distDeltaSq ) != SignAsInt( prevSmp.distDeltaSq ) ) {
					pointProjectGuesses[candidatesFound++] = SampleDistSqDelta( ( prevSmp.t + smp.t ) / 2 );
					if( candidatesFound == 3 ) break; // no more than three possible candidates because of the polynomial degree
				}

				prevSmp = smp;
			}

			// refine each guess w. Newton-Raphson iterations
			void Refine( ref PointProjectSample smp ) {
				Vector2 fpp = bez.GetSecondDerivative( smp.t );
				float tNew = smp.t - Vector2.Dot( smp.f, smp.fp ) / ( Vector2.Dot( smp.f, fpp ) + Vector2.Dot( smp.fp, smp.fp ) );
				smp = SampleDistSqDelta( tNew );
			}

			for( int p = 0; p < candidatesFound; p++ )
				for( int i = 0; i < refinementIterations; i++ )
					Refine( ref pointProjectGuesses[p] );

			// Now find closest. First include the endpoints
			float sqDist0 = bez.P0.sqrMagnitude; // include endpoints
			float sqDist1 = bez.P3.sqrMagnitude;
			bool firstClosest = sqDist0 < sqDist1;
			float tClosest = firstClosest ? 0 : 1;
			Vector2 ptClosest = firstClosest ? P0 : P3;
			float distSqClosest = firstClosest ? sqDist0 : sqDist1;

			// then check internal roots
			for( int i = 0; i < candidatesFound; i++ ) {
				float pSqmag = pointProjectGuesses[i].f.sqrMagnitude;
				if( pSqmag < distSqClosest ) {
					distSqClosest = pSqmag;
					tClosest = pointProjectGuesses[i].t;
					ptClosest = pointProjectGuesses[i].f + point;
				}
			}

			t = tClosest;
			return ptClosest;
		}

		#endregion

		#region Intersection Tests

		// Internal - used by all other intersections
		private ResultsMax3<float> Intersect( Vector2 origin, Vector2 direction, bool rangeLimited = false, float minRayT = float.NaN, float maxRayT = float.NaN ) {
			Vector2 p0rel = this.P0 - origin;
			Vector2 p1rel = this.P1 - origin;
			Vector2 p2rel = this.P2 - origin;
			Vector2 p3rel = this.P3 - origin;
			float y0 = Determinant( p0rel, direction ); // transform bezier point components into the line space y components
			float y1 = Determinant( p1rel, direction );
			float y2 = Determinant( p2rel, direction );
			float y3 = Determinant( p3rel, direction );
			Polynomial polynomY = SplineUtils.GetCubicPolynomial( y0, y1, y2, y3 );
			ResultsMax3<float> roots = polynomY.Roots; // t values of the function


			Polynomial polynomX = default;
			if( rangeLimited ) {
				// if we're range limited, we need to verify position along the ray/line/lineSegment
				// and if we do, we need to be able to go from t -> x coord
				float x0 = Vector2.Dot( p0rel, direction ); // transform bezier point components into the line space x components
				float x1 = Vector2.Dot( p1rel, direction );
				float x2 = Vector2.Dot( p2rel, direction );
				float x3 = Vector2.Dot( p3rel, direction );
				polynomX = SplineUtils.GetCubicPolynomial( x0, x1, x2, x3 );
			}

			float CurveTtoRayT( float t ) => polynomX.Sample( t );

			ResultsMax3<float> returnVals = default;

			for( int i = 0; i < roots.count; i++ ) {
				if( roots[i].Between( 0, 1 ) && ( rangeLimited == false || CurveTtoRayT( roots[i] ).Within( minRayT, maxRayT ) ) )
					returnVals = returnVals.Add( roots[i] );
			}

			return returnVals;
		}

		// Internal - to unpack from curve t values to points
		private ResultsMax3<Vector2> TtoPoints( ResultsMax3<float> tVals ) {
			ResultsMax3<Vector2> pts = default;
			for( int i = 0; i < tVals.count; i++ )
				pts = pts.Add( GetPoint( tVals[i] ) );
			return pts;
		}

		/// <summary>Returns the t-values at which the given line intersects with the curve</summary>
		/// <param name="line">The line to test intersection against</param>
		public ResultsMax3<float> Intersect( Line2D line ) => Intersect( line.origin, line.dir );

		/// <summary>Returns the t-values at which the given ray intersects with the curve</summary>
		/// <param name="ray">The ray to test intersection against</param>
		public ResultsMax3<float> Intersect( Ray2D ray ) => Intersect( ray.origin, ray.dir, rangeLimited: true, 0, float.MaxValue );

		/// <summary>Returns the t-values at which the given line segment intersects with the curve</summary>
		/// <param name="lineSegment">The line segment to test intersection against</param>
		public ResultsMax3<float> Intersect( LineSegment2D lineSegment ) => Intersect( lineSegment.start, lineSegment.end - lineSegment.start, rangeLimited: true, 0, lineSegment.LengthSquared );

		/// <summary>Returns the points at which the given line intersects with the curve</summary>
		/// <param name="line">The line to test intersection against</param>
		public ResultsMax3<Vector2> IntersectionPoints( Line2D line ) => TtoPoints( Intersect( line.origin, line.dir ) );

		/// <summary>Returns the points at which the given ray intersects with the curve</summary>
		/// <param name="ray">The ray to test intersection against</param>
		public ResultsMax3<Vector2> IntersectionPoints( Ray2D ray ) => TtoPoints( Intersect( ray.origin, ray.dir, rangeLimited: true, 0, float.MaxValue ) );

		/// <summary>Returns the points at which the given line segment intersects with the curve</summary>
		/// <param name="lineSegment">The line segment to test intersection against</param>
		public ResultsMax3<Vector2> IntersectionPoints( LineSegment2D lineSegment ) => TtoPoints( Intersect( lineSegment.start, lineSegment.end - lineSegment.start, rangeLimited: true, 0, lineSegment.LengthSquared ) );

		/// <summary>Raycasts and returns whether or not it hit, along with the closest hit point</summary>
		/// <param name="ray">The ray to use when raycasting</param>
		/// <param name="hitPoint">The closest point on the curve the ray hit</param>
		/// <param name="maxDist">The maximum length of the ray</param>
		public bool Raycast( Ray2D ray, out Vector2 hitPoint, float maxDist = float.MaxValue ) => Raycast( ray, out hitPoint, out _, maxDist );

		/// <summary>Raycasts and returns whether or not it hit, along with the closest hit point and the t-value on the curve</summary>
		/// <param name="ray">The ray to use when raycasting</param>
		/// <param name="hitPoint">The closest point on the curve the ray hit</param>
		/// <param name="t">The t-value of the curve at the point the ray hit</param>
		/// <param name="maxDist">The maximum length of the ray</param>
		public bool Raycast( Ray2D ray, out Vector2 hitPoint, out float t, float maxDist = float.MaxValue ) {
			float closestDist = float.MaxValue;
			ResultsMax3<float> tPts = Intersect( ray );
			ResultsMax3<Vector2> pts = TtoPoints( tPts );

			// find closest point
			bool didHit = false;
			hitPoint = default;
			t = default;
			for( int i = 0; i < pts.count; i++ ) {
				Vector2 pt = pts[i];
				float dist = Vector2.Dot( ray.dir, pt - ray.origin );
				if( dist < closestDist && dist <= maxDist ) {
					closestDist = dist;
					hitPoint = pt;
					t = tPts[i];
					didHit = true;
				}
			}

			return didHit;
		}

		#endregion

		// Esoteric math stuff

		#region Polynomial Factors

		/// <summary>Returns the factors of the derivative polynomials, per-component, in the form at²+bt+c</summary>
		public (Vector2 a, Vector2 b, Vector2 c) GetDerivativeFactors() {
			ReadyCoefficients();
			return ( new Vector2( 3 * c3.x, 3 * c3.y ), new Vector2( 2 * c2.x, 2 * c2.y ), c1 );
		}

		/// <summary>Returns the factors of the second derivative polynomials, per-component, in the form at+b</summary>
		public (Vector2 a, Vector2 b) GetSecondDerivativeFactors() {
			ReadyCoefficients();
			return ( new Vector2( 6 * c3.x, 6 * c3.y ), new Vector2( 2 * c2.x, 2 * c2.y ) );
		}

		#endregion

		#region Local Extrema

		/// <summary>Returns the t values of extrema (local minima/maxima) on a given axis in the 0 &lt; t &lt; 1 range</summary>
		/// <param name="axis">Either 0 (X) or 1 (Y)</param>
		public ResultsMax2<float> GetLocalExtrema( int axis ) {
			if( axis < 0 || axis > 1 )
				throw new ArgumentOutOfRangeException( nameof(axis), "axis has to be either 0 or 1" );
			Polynomial polynom;
			if( axis == 0 ) // a little silly but the vec[] indexers are kinda expensive
				polynom = SplineUtils.GetCubicPolynomialDerivative( P0.x, P1.x, P2.x, P3.x );
			else
				polynom = SplineUtils.GetCubicPolynomialDerivative( P0.y, P1.y, P2.y, P3.y );
			ResultsMax3<float> roots = polynom.Roots;
			ResultsMax2<float> outPts = default;
			for( int i = 0; i < roots.count; i++ ) {
				float t = roots[i];
				if( t.Between( 0, 1 ) )
					outPts = outPts.Add( t );
			}

			return outPts;
		}

		/// <summary>Returns the extrema points (local minima/maxima points) on a given axis in the 0 &lt; t &lt; 1 range</summary>
		/// <param name="axis">Either 0 (X) or 1 (Y)</param>
		public ResultsMax2<float> GetLocalExtremaPoints( int axis ) {
			ResultsMax2<float> t = GetLocalExtrema( axis );
			ResultsMax2<float> pts = default;
			for( int i = 0; i < t.count; i++ )
				pts = pts.Add( GetPointComponent( axis, t[i] ) );
			return pts;
		}

		#endregion

	}

}

// code graveyard - kept just in case I want to bring any of it back:
/*
/// <summary>Returns the point and the derivative at the given t-value on the curve. This is more performant than calling GetPoint and GetDerivative separately</summary>
/// <param name="t">The t-value along the curve to sample</param>
public (Vector2, Vector2) GetPointAndDerivative( float t ) {
	ReadyCoefficients();
	float t2 = t * t;
	float tx2 = t * 2;
	float t2x3 = t2 * 3;
	float t3 = t2 * t;
	return (
		new Vector2( t3 * c3.x + t2 * c2.x + t * c1.x + p0.x, t3 * c3.y + t2 * c2.y + t * c1.y + p0.y ),
		new Vector2( t2x3 * c3.x + tx2 * c2.x + c1.x, t2x3 * c3.y + tx2 * c2.y + c1.y )
	);
}

/// <summary>Returns all three derivatives at the given t-value on the curve</summary>
/// <param name="t">The t-value along the curve to sample</param>
public (Vector2, Vector2, Vector2) GetAllThreeDerivatives( float t ) {
	ReadyCoefficients();
	float t2x3 = 3 * t * t;
	float tx2 = 2 * t;
	float tx6 = 6 * t;
	return (
		new Vector2( t2x3 * c3.x + tx2 * c2.x + c1.x, t2x3 * c3.y + tx2 * c2.y + c1.y ),
		new Vector2( tx6 * c3.x + 2 * c2.x, tx6 * c3.y + 2 * c2.y ),
		new Vector2( 6 * c3.x, 6 * c3.y )
	);
}

[MethodImpl( INLINE )] public (Vector2, Vector2) GetFirstTwoDerivatives( float t ) {
	ReadyCoefficients();
	float t2x3 = 3 * t * t;
	float tx2 = 2 * t;
	float tx6 = 6 * t;
	return (
		new Vector2( t2x3 * c3.x + tx2 * c2.x + c1.x, t2x3 * c3.y + tx2 * c2.y + c1.y ),
		new Vector2( tx6 * c3.x + 2 * c2.x, tx6 * c3.y + 2 * c2.y )
	);
}

public (Vector2, Vector2, Vector2) GetPointAndFirstTwoDerivatives( float t ) {
	ReadyCoefficients();
	float t2 = t * t;
	float tx2 = t * 2;
	float tx6 = t * 6;
	float t2x3 = t2 * 3;
	float t3 = t2 * t;
	return (
		new Vector2( t3 * c3.x + t2 * c2.x + t * c1.x + p0.x, t3 * c3.y + t2 * c2.y + t * c1.y + p0.y ),
		new Vector2( t2x3 * c3.x + tx2 * c2.x + c1.x, t2x3 * c3.y + tx2 * c2.y + c1.y ),
		new Vector2( tx6 * c3.x + 2 * c2.x, tx6 * c3.y + 2 * c2.y )
	);
}
*/