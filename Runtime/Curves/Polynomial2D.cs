// by Freya Holmér (https://github.com/FreyaHolmer/Mathfs)

using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Freya {

	[Serializable]
	public struct Polynomial2D : IPolynomialCubic<Polynomial2D, Vector2>, IParamCurve3Diff<Vector2> {

		const MethodImplOptions INLINE = MethodImplOptions.AggressiveInlining;

		/// <inheritdoc cref="Polynomial.NaN"/>
		public static readonly Polynomial2D NaN = new Polynomial2D { x = Polynomial.NaN, y = Polynomial.NaN };

		public Polynomial x, y;

		public Polynomial2D( Polynomial x, Polynomial y ) => ( this.x, this.y ) = ( x, y );

		/// <inheritdoc cref="Polynomial(float,float,float,float)"/>
		public Polynomial2D( Vector2 c0, Vector2 c1, Vector2 c2, Vector2 c3 ) {
			this.x = new Polynomial( c0.x, c1.x, c2.x, c3.x );
			this.y = new Polynomial( c0.y, c1.y, c2.y, c3.y );
		}

		/// <inheritdoc cref="Polynomial(float,float,float)"/>
		public Polynomial2D( Vector2 c0, Vector2 c1, Vector2 c2 ) {
			this.x = new Polynomial( c0.x, c1.x, c2.x );
			this.y = new Polynomial( c0.y, c1.y, c2.y );
		}

		/// <inheritdoc cref="Polynomial(float,float)"/>
		public Polynomial2D( Vector2 c0, Vector2 c1 ) {
			this.x = new Polynomial( c0.x, c1.x, 0, 0 );
			this.y = new Polynomial( c0.y, c1.y, 0, 0 );
		}

		/// <inheritdoc cref="Polynomial(Matrix4x1)"/>
		public Polynomial2D( Vector2Matrix4x1 coefficients ) => ( x, y ) = ( new Polynomial( coefficients.X ), new Polynomial( coefficients.Y ) );

		/// <inheritdoc cref="Polynomial(Matrix4x1)"/>
		public Polynomial2D( Vector2Matrix3x1 coefficients ) => ( x, y ) = ( new Polynomial( coefficients.X ), new Polynomial( coefficients.Y ) );

		#region IPolynomialCubic

		public Vector2 C0 {
			[MethodImpl( INLINE )] get => new(x.c0, y.c0);
			[MethodImpl( INLINE )] set => ( x.c0, y.c0 ) = ( value.x, value.y );
		}
		public Vector2 C1 {
			[MethodImpl( INLINE )] get => new(x.c1, y.c1);
			[MethodImpl( INLINE )] set => ( x.c1, y.c1 ) = ( value.x, value.y );
		}
		public Vector2 C2 {
			[MethodImpl( INLINE )] get => new(x.c2, y.c2);
			[MethodImpl( INLINE )] set => ( x.c2, y.c2 ) = ( value.x, value.y );
		}
		public Vector2 C3 {
			[MethodImpl( INLINE )] get => new(x.c3, y.c3);
			[MethodImpl( INLINE )] set => ( x.c3, y.c3 ) = ( value.x, value.y );
		}

		public Polynomial this[ int i ] {
			get { return i switch { 0 => x, 1         => y, _         => throw new IndexOutOfRangeException( "Polynomial2D component index has to be either 0 or 1" ) }; }
			set => _ = i switch { 0   => x = value, 1 => y = value, _ => throw new IndexOutOfRangeException() };
		}

		[MethodImpl( INLINE )] public Vector2 GetCoefficient( int degree ) =>
			degree switch {
				0 => C0,
				1 => C1,
				2 => C2,
				3 => C3,
				_ => throw new IndexOutOfRangeException( "Polynomial coefficient degree/index has to be between 0 and 3" )
			};

		[MethodImpl( INLINE )] public void SetCoefficient( int degree, Vector2 value ) {
			_ = degree switch {
				0 => C0 = value,
				1 => C1 = value,
				2 => C2 = value,
				3 => C3 = value,
				_ => throw new IndexOutOfRangeException( "Polynomial coefficient degree/index has to be between 0 and 3" )
			};
		}

		public Vector2 Eval( float t ) {
			float t2 = t * t;
			float t3 = t2 * t;
			return new Vector2(
				x.c3 * t3 + x.c2 * t2 + x.c1 * t + x.c0,
				y.c3 * t3 + y.c2 * t2 + y.c1 * t + y.c0
			);
		}

		[MethodImpl( INLINE )] public Vector2 Eval( float t, int n ) => Differentiate( n ).Eval( t );

		[MethodImpl( INLINE )] public Polynomial2D Differentiate( int n = 1 ) => new(x.Differentiate( n ), y.Differentiate( n ));

		public Polynomial2D ScaleParameterSpace( float factor ) {
			// ReSharper disable once CompareOfFloatsByEqualityOperator
			if( factor == 1f )
				return this;
			float factor2 = factor * factor;
			float factor3 = factor2 * factor;
			return new Polynomial2D(
				new Polynomial( x.c0, x.c1 / factor, x.c2 / factor2, x.c3 / factor3 ),
				new Polynomial( y.c0, y.c1 / factor, y.c2 / factor2, y.c3 / factor3 )
			);
		}

		public Polynomial2D Compose( float g0, float g1 ) => new(x.Compose( g0, g1 ), y.Compose( g0, g1 ));

		#endregion

		/// <inheritdoc cref="Polynomial.FitCubicFrom0(float,float,float,float,float,float,float)"/>
		public static Polynomial2D FitCubicFrom0( float x1, float x2, float x3, Vector2 y0, Vector2 y1, Vector2 y2, Vector2 y3 ) {
			// precalcs
			float i12 = x2 - x1;
			float i13 = x3 - x1;
			float i23 = x3 - x2;
			float x1x2 = x1 * x2;
			float x1x3 = x1 * x3;
			float x2x3 = x2 * x3;
			float x1x2x3 = x1 * x2x3;
			float x0plusx1plusx2 = x1 + x2;
			float x0plusx1plusx3 = x1 + x3;
			float x2plusx3 = x2 + x3;
			float x1plusx2plusx3 = x1 + x2plusx3;
			float x1x2plusx1x3plusx2x3 = ( x1x2 + x1x3 + x2x3 );

			// scale factors
			Vector2 scl0 = y0 / -( x1 * x2 * x3 );
			Vector2 scl1 = y1 / +( x1 * i12 * i13 );
			Vector2 scl2 = y2 / -( x2 * i12 * i23 );
			Vector2 scl3 = y3 / +( x3 * i13 * i23 );

			// polynomial form
			Vector2 c0 = new(
				-( scl0.x * x1x2x3 ),
				-( scl0.y * x1x2x3 )
			);
			Vector2 c1 = new(
				scl0.x * x1x2plusx1x3plusx2x3 + scl1.x * x2x3 + scl2.x * x1x3 + scl3.x * x1x2,
				scl0.y * x1x2plusx1x3plusx2x3 + scl1.y * x2x3 + scl2.y * x1x3 + scl3.y * x1x2
			);
			Vector2 c2 = new(
				-( scl0.x * x1plusx2plusx3 + scl1.x * x2plusx3 + scl2.x * x0plusx1plusx3 + scl3.x * x0plusx1plusx2 ),
				-( scl0.y * x1plusx2plusx3 + scl1.y * x2plusx3 + scl2.y * x0plusx1plusx3 + scl3.y * x0plusx1plusx2 )
			);
			Vector2 c3 = new(
				scl0.x + scl1.x + scl2.x + scl3.x,
				scl0.y + scl1.y + scl2.y + scl3.y
			);

			return new Polynomial2D( c0, c1, c2, c3 );
		}


		/// <summary>Returns the tight axis-aligned bounds of the curve in the unit interval</summary>
		public Rect GetBounds01() => FloatRange.ToRect( x.OutputRange01, y.OutputRange01 );

		/// <inheritdoc cref="Polynomial.Split01"/>
		public (Polynomial2D pre, Polynomial2D post) Split01( float u ) {
			( Polynomial xPre, Polynomial xPost ) = x.Split01( u );
			( Polynomial yPre, Polynomial yPost ) = y.Split01( u );
			return ( new Polynomial2D( xPre, yPre ), new Polynomial2D( xPost, yPost ) );
		}

		#region IParamCurve3Diff interface implementations

		public int Degree => Mathfs.Max( x.Degree, y.Degree );
		public Vector2 EvalDerivative( float t ) => Differentiate().Eval( t );
		public Vector2 EvalSecondDerivative( float t ) => Differentiate( 2 ).Eval( t );
		public Vector2 EvalThirdDerivative( float t = 0 ) => Differentiate( 3 ).Eval( 0 );

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
			// define a curve relative to the test point
			Polynomial2D curve = this;
			curve.x.c0 -= point.x; // constant coefficient defines the start position
			curve.y.c0 -= point.y;
			Polynomial2D vel = curve.Differentiate();
			Polynomial2D acc = vel.Differentiate();
			Vector2 curveStart = curve.Eval( 0 );
			Vector2 curveEnd = curve.Eval( 1 );

			PointProjectSample SampleDistSqDelta( float tSmp ) {
				PointProjectSample s = new PointProjectSample {
					t = tSmp,
					f = curve.Eval( tSmp ),
					fp = vel.Eval( tSmp )
				};
				s.distDeltaSq = Vector2.Dot( s.f, s.fp );
				return s;
			}

			// find initial candidates
			int candidatesFound = 0;
			PointProjectSample prevSmp = SampleDistSqDelta( 0 );

			for( int i = 1; i < initialSubdivisions; i++ ) {
				float ti = i / ( initialSubdivisions - 1f );
				PointProjectSample smp = SampleDistSqDelta( ti );
				if( Mathfs.SignAsInt( smp.distDeltaSq ) != Mathfs.SignAsInt( prevSmp.distDeltaSq ) ) {
					pointProjectGuesses[candidatesFound++] = SampleDistSqDelta( ( prevSmp.t + smp.t ) / 2 );
					if( candidatesFound == 3 ) break; // no more than three possible candidates because of the polynomial degree
				}

				prevSmp = smp;
			}

			// refine each guess w. Newton-Raphson iterations
			void Refine( ref PointProjectSample smp ) {
				Vector2 fpp = acc.Eval( smp.t );
				float tNew = smp.t - Vector2.Dot( smp.f, smp.fp ) / ( Vector2.Dot( smp.f, fpp ) + Vector2.Dot( smp.fp, smp.fp ) );
				smp = SampleDistSqDelta( tNew );
			}

			for( int p = 0; p < candidatesFound; p++ )
				for( int i = 0; i < refinementIterations; i++ )
					Refine( ref pointProjectGuesses[p] );

			// Now find closest. First include the endpoints
			float sqDist0 = curveStart.sqrMagnitude; // include endpoints
			float sqDist1 = curveEnd.sqrMagnitude;
			bool firstClosest = sqDist0 < sqDist1;
			float tClosest = firstClosest ? 0 : 1;
			Vector2 ptClosest = ( firstClosest ? curveStart : curveEnd ) + point;
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
			Polynomial2D rel = this;
			rel.C0 -= origin;
			float y0 = Mathfs.Determinant( rel.C0, direction ); // transform polynomial into the line space y components
			float y1 = Mathfs.Determinant( rel.C1, direction );
			float y2 = Mathfs.Determinant( rel.C2, direction );
			float y3 = Mathfs.Determinant( rel.C3, direction );
			Polynomial polynomY = new Polynomial( y0, y1, y2, y3 );
			ResultsMax3<float> roots = polynomY.Roots; // t values of the function

			Polynomial polynomX = default;
			if( rangeLimited ) {
				// if we're range limited, we need to verify position along the ray/line/lineSegment
				// and if we do, we need to be able to go from t -> x coord
				float x0 = Vector2.Dot( rel.C0, direction ); // transform into the line space x components
				float x1 = Vector2.Dot( rel.C1, direction );
				float x2 = Vector2.Dot( rel.C2, direction );
				float x3 = Vector2.Dot( rel.C3, direction );
				polynomX = new Polynomial( x0, x1, x2, x3 );
			}

			float CurveTtoRayT( float t ) => polynomX.Eval( t );

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
				pts = pts.Add( Eval( tVals[i] ) );
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

		/// <summary>Returns the polynomial, rotated around the origin</summary>
		/// <param name="poly">The polynomial to rotate</param>
		/// <param name="angle">The angle to rotate by (in radians)</param>
		public static Polynomial2D Rotate( Polynomial2D poly, float angle ) =>
			new(
				poly.C0.Rotate( angle ),
				poly.C1.Rotate( angle ),
				poly.C2.Rotate( angle ),
				poly.C3.Rotate( angle )
			);

		#region Typecasting & Operators

		public static Polynomial2D operator /( Polynomial2D p, float v ) => new(p.C0 / v, p.C1 / v, p.C2 / v, p.C3 / v);
		public static Polynomial2D operator *( Polynomial2D p, float v ) => new(p.C0 * v, p.C1 * v, p.C2 * v, p.C3 * v);
		public static Polynomial2D operator *( float v, Polynomial2D p ) => p * v;

		public static explicit operator Vector2Matrix3x1( Polynomial2D poly ) => new(poly.C0, poly.C1, poly.C2);
		public static explicit operator Vector2Matrix4x1( Polynomial2D poly ) => new(poly.C0, poly.C1, poly.C2, poly.C3);
		public static explicit operator BezierQuad2D( Polynomial2D poly ) => poly.Degree < 3 ? new BezierQuad2D( CharMatrix.quadraticBezierInverse * (Vector2Matrix3x1)poly ) : throw new InvalidCastException( "Cannot cast a cubic polynomial to a quadratic curve" );
		public static explicit operator BezierCubic2D( Polynomial2D poly ) => new(CharMatrix.cubicBezierInverse * (Vector2Matrix4x1)poly);
		public static explicit operator CatRomCubic2D( Polynomial2D poly ) => new(CharMatrix.cubicCatmullRomInverse * (Vector2Matrix4x1)poly);
		public static explicit operator HermiteCubic2D( Polynomial2D poly ) => new(CharMatrix.cubicHermiteInverse * (Vector2Matrix4x1)poly);
		public static explicit operator UBSCubic2D( Polynomial2D poly ) => new(CharMatrix.cubicUniformBsplineInverse * (Vector2Matrix4x1)poly);

		#endregion

	}

}