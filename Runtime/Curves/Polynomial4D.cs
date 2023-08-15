// by Freya Holmér (https://github.com/FreyaHolmer/Mathfs)

using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Freya {

	public struct Polynomial4D : IPolynomialCubic<Polynomial4D, Vector4>, IParamCurve3Diff<Vector4> {

		const MethodImplOptions INLINE = MethodImplOptions.AggressiveInlining;

		/// <inheritdoc cref="Polynomial.NaN"/>
		public static readonly Polynomial4D NaN = new Polynomial4D { x = Polynomial.NaN, y = Polynomial.NaN, z = Polynomial.NaN, w = Polynomial.NaN };

		public Polynomial x, y, z, w;

		public Polynomial4D( Polynomial x, Polynomial y, Polynomial z, Polynomial w ) => ( this.x, this.y, this.z, this.w ) = ( x, y, z, w );

		/// <inheritdoc cref="Polynomial(float,float,float,float)"/>
		public Polynomial4D( Vector4 c0, Vector4 c1, Vector4 c2, Vector4 c3 ) {
			this.x = new Polynomial( c0.x, c1.x, c2.x, c3.x );
			this.y = new Polynomial( c0.y, c1.y, c2.y, c3.y );
			this.z = new Polynomial( c0.z, c1.z, c2.z, c3.z );
			this.w = new Polynomial( c0.w, c1.w, c2.w, c3.w );
		}

		/// <inheritdoc cref="Polynomial(float,float,float)"/>
		public Polynomial4D( Vector4 c0, Vector4 c1, Vector4 c2 ) {
			this.x = new Polynomial( c0.x, c1.x, c2.x, 0 );
			this.y = new Polynomial( c0.y, c1.y, c2.y, 0 );
			this.z = new Polynomial( c0.z, c1.z, c2.z, 0 );
			this.w = new Polynomial( c0.w, c1.w, c2.w, 0 );
		}

		/// <inheritdoc cref="Polynomial(float,float)"/>
		public Polynomial4D( Vector4 c0, Vector4 c1 ) {
			this.x = new Polynomial( c0.x, c1.x, 0, 0 );
			this.y = new Polynomial( c0.y, c1.y, 0, 0 );
			this.z = new Polynomial( c0.z, c1.z, 0, 0 );
			this.w = new Polynomial( c0.w, c1.w, 0, 0 );
		}

		/// <inheritdoc cref="Polynomial(Matrix4x1)"/>
		public Polynomial4D( Vector4Matrix4x1 coefficients ) => ( x, y, z, w ) = ( new Polynomial( coefficients.X ), new Polynomial( coefficients.Y ), new Polynomial( coefficients.Z ), new Polynomial( coefficients.W ) );

		/// <inheritdoc cref="Polynomial(Matrix4x1)"/>
		public Polynomial4D( Vector4Matrix3x1 coefficients ) => ( x, y, z, w ) = ( new Polynomial( coefficients.X ), new Polynomial( coefficients.Y ), new Polynomial( coefficients.Z ), new Polynomial( coefficients.W ) );

		#region IPolynomialCubic

		public Vector4 C0 {
			[MethodImpl( INLINE )] get => new(x.c0, y.c0, z.c0, w.c0);
			[MethodImpl( INLINE )] set => ( x.c0, y.c0, z.c0, w.c0 ) = ( value.x, value.y, value.z, value.w );
		}
		public Vector4 C1 {
			[MethodImpl( INLINE )] get => new(x.c1, y.c1, z.c1, w.c1);
			[MethodImpl( INLINE )] set => ( x.c1, y.c1, z.c1, w.c1 ) = ( value.x, value.y, value.z, value.w );
		}
		public Vector4 C2 {
			[MethodImpl( INLINE )] get => new(x.c2, y.c2, z.c2, w.c2);
			[MethodImpl( INLINE )] set => ( x.c2, y.c2, z.c2, w.c2 ) = ( value.x, value.y, value.z, value.w );
		}
		public Vector4 C3 {
			[MethodImpl( INLINE )] get => new(x.c3, y.c3, z.c3, w.c3);
			[MethodImpl( INLINE )] set => ( x.c3, y.c3, z.c3, w.c3 ) = ( value.x, value.y, value.z, value.w );
		}

		public Polynomial this[ int i ] {
			get => i switch { 0 => x, 1 => y, 2 => z, 4 => w, _ => throw new IndexOutOfRangeException( "Polynomial4D component index has to be either 0, 1, 2, or 3" ) };
			set => _ = i switch { 0 => x = value, 1 => y = value, 2 => z = value, 3 => w = value, _ => throw new IndexOutOfRangeException() };
		}

		[MethodImpl( INLINE )] public Vector4 GetCoefficient( int degree ) =>
			degree switch {
				0 => C0,
				1 => C1,
				2 => C2,
				3 => C3,
				_ => throw new IndexOutOfRangeException( "Polynomial coefficient degree/index has to be between 0 and 3" )
			};

		[MethodImpl( INLINE )] public void SetCoefficient( int degree, Vector4 value ) {
			_ = degree switch {
				0 => C0 = value,
				1 => C1 = value,
				2 => C2 = value,
				3 => C3 = value,
				_ => throw new IndexOutOfRangeException( "Polynomial coefficient degree/index has to be between 0 and 3" )
			};
		}

		public Vector4 Eval( float t ) {
			float t2 = t * t;
			float t3 = t2 * t;
			return new Vector4(
				x.c3 * t3 + x.c2 * t2 + x.c1 * t + x.c0,
				y.c3 * t3 + y.c2 * t2 + y.c1 * t + y.c0,
				z.c3 * t3 + z.c2 * t2 + z.c1 * t + z.c0,
				w.c3 * t3 + w.c2 * t2 + w.c1 * t + w.c0
			);
		}

		[MethodImpl( INLINE )] public Vector4 Eval( float t, int n ) => Differentiate( n ).Eval( t );

		[MethodImpl( INLINE )] public Polynomial4D Differentiate( int n = 1 ) => new(x.Differentiate( n ), y.Differentiate( n ), z.Differentiate( n ), w.Differentiate( n ));

		public Polynomial4D ScaleParameterSpace( float factor ) {
			// ReSharper disable once CompareOfFloatsByEqualityOperator
			if( factor == 1f )
				return this;
			float factor2 = factor * factor;
			float factor3 = factor2 * factor;
			return new Polynomial4D(
				new Polynomial( x.c0, x.c1 / factor, x.c2 / factor2, x.c3 / factor3 ),
				new Polynomial( y.c0, y.c1 / factor, y.c2 / factor2, y.c3 / factor3 ),
				new Polynomial( z.c0, z.c1 / factor, z.c2 / factor2, z.c3 / factor3 ),
				new Polynomial( w.c0, w.c1 / factor, w.c2 / factor2, w.c3 / factor3 )
			);
		}

		public Polynomial4D Compose( float g0, float g1 ) => new(x.Compose( g0, g1 ), y.Compose( g0, g1 ), z.Compose( g0, g1 ), w.Compose( g0, g1 ));

		#endregion

		public static Polynomial4D FitCubicFrom0( float x1, float x2, float x3, Vector4 y0, Vector4 y1, Vector4 y2, Vector4 y3 ) {
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
			Vector4 scl0 = y0 / -( x1 * x2 * x3 );
			Vector4 scl1 = y1 / +( x1 * i12 * i13 );
			Vector4 scl2 = y2 / -( x2 * i12 * i23 );
			Vector4 scl3 = y3 / +( x3 * i13 * i23 );

			// polynomial form
			Vector4 c0 = new(
				-( scl0.x * x1x2x3 ),
				-( scl0.y * x1x2x3 ),
				-( scl0.z * x1x2x3 ),
				-( scl0.w * x1x2x3 )
			);
			Vector4 c1 = new(
				scl0.x * x1x2plusx1x3plusx2x3 + scl1.x * x2x3 + scl2.x * x1x3 + scl3.x * x1x2,
				scl0.y * x1x2plusx1x3plusx2x3 + scl1.y * x2x3 + scl2.y * x1x3 + scl3.y * x1x2,
				scl0.z * x1x2plusx1x3plusx2x3 + scl1.z * x2x3 + scl2.z * x1x3 + scl3.z * x1x2,
				scl0.w * x1x2plusx1x3plusx2x3 + scl1.w * x2x3 + scl2.w * x1x3 + scl3.w * x1x2
			);
			Vector4 c2 = new(
				-( scl0.x * x1plusx2plusx3 + scl1.x * x2plusx3 + scl2.x * x0plusx1plusx3 + scl3.x * x0plusx1plusx2 ),
				-( scl0.y * x1plusx2plusx3 + scl1.y * x2plusx3 + scl2.y * x0plusx1plusx3 + scl3.y * x0plusx1plusx2 ),
				-( scl0.z * x1plusx2plusx3 + scl1.z * x2plusx3 + scl2.z * x0plusx1plusx3 + scl3.z * x0plusx1plusx2 ),
				-( scl0.w * x1plusx2plusx3 + scl1.w * x2plusx3 + scl2.w * x0plusx1plusx3 + scl3.w * x0plusx1plusx2 )
			);
			Vector4 c3 = new(
				scl0.x + scl1.x + scl2.x + scl3.x,
				scl0.y + scl1.y + scl2.y + scl3.y,
				scl0.z + scl1.z + scl2.z + scl3.z,
				scl0.w + scl1.w + scl2.w + scl3.w
			);

			return new Polynomial4D( c0, c1, c2, c3 );
		}

		/// <inheritdoc cref="Polynomial2D.GetBounds01"/>
		public (FloatRange x, FloatRange y, FloatRange z, FloatRange w) GetBounds01() => ( x.OutputRange01, y.OutputRange01, z.OutputRange01, w.OutputRange01 );

		/// <inheritdoc cref="Polynomial.Split01"/>
		public (Polynomial4D pre, Polynomial4D post) Split01( float u ) {
			( Polynomial xPre, Polynomial xPost ) = x.Split01( u );
			( Polynomial yPre, Polynomial yPost ) = y.Split01( u );
			( Polynomial zPre, Polynomial zPost ) = z.Split01( u );
			( Polynomial wPre, Polynomial wPost ) = w.Split01( u );
			return ( new Polynomial4D( xPre, yPre, zPre, wPre ), new Polynomial4D( xPost, yPost, zPost, wPost ) );
		}

		#region IParamCurve3Diff interface implementations

		public int Degree => Mathfs.Max( x.Degree, y.Degree, z.Degree, w.Degree );
		public Vector4 EvalDerivative( float t ) => Differentiate().Eval( t );
		public Vector4 EvalSecondDerivative( float t ) => Differentiate( 2 ).Eval( t );
		public Vector4 EvalThirdDerivative( float t = 0 ) => Differentiate( 3 ).Eval( 0 );

		#endregion

		#region Project Point

		/// <inheritdoc cref="Polynomial2D.ProjectPoint(Vector2,int,int)"/>
		public Vector4 ProjectPoint( Vector4 point, int initialSubdivisions = 16, int refinementIterations = 4 ) => ProjectPoint( point, out _, initialSubdivisions, refinementIterations );

		struct PointProjectSample {
			public float t;
			public float distDeltaSq;
			public Vector4 f;
			public Vector4 fp;
		}

		static PointProjectSample[] pointProjectGuesses = { default, default, default };

		/// <inheritdoc cref="Polynomial2D.ProjectPoint(Vector2,out float,int,int)"/>
		public Vector4 ProjectPoint( Vector4 point, out float t, int initialSubdivisions = 16, int refinementIterations = 4 ) {
			// define a bezier relative to the test point
			Polynomial4D curve = this;
			curve.x.c0 -= point.x; // constant coefficient defines the start position
			curve.y.c0 -= point.y;
			curve.z.c0 -= point.z;
			Vector4 curveStart = curve.Eval( 0 );
			Vector4 curveEnd = curve.Eval( 1 );

			PointProjectSample SampleDistSqDelta( float tSmp ) {
				PointProjectSample s = new PointProjectSample { t = tSmp };
				( s.f, s.fp ) = ( curve.Eval( tSmp ), curve.EvalDerivative( tSmp ) );
				s.distDeltaSq = Vector4.Dot( s.f, s.fp );
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
				Vector4 fpp = curve.EvalSecondDerivative( smp.t );
				float tNew = smp.t - Vector4.Dot( smp.f, smp.fp ) / ( Vector4.Dot( smp.f, fpp ) + Vector4.Dot( smp.fp, smp.fp ) );
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
			Vector4 ptClosest = ( firstClosest ? curveStart : curveEnd ) + point;
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

		#region Typecasting & Operators

		public static Polynomial4D operator /( Polynomial4D p, float v ) => new(p.C0 / v, p.C1 / v, p.C2 / v, p.C3 / v);
		public static Polynomial4D operator *( Polynomial4D p, float v ) => new(p.C0 * v, p.C1 * v, p.C2 * v, p.C3 * v);
		public static Polynomial4D operator *( float v, Polynomial4D p ) => p * v;

		public static explicit operator Vector4Matrix3x1( Polynomial4D poly ) => new(poly.C0, poly.C1, poly.C2);
		public static explicit operator Vector4Matrix4x1( Polynomial4D poly ) => new(poly.C0, poly.C1, poly.C2, poly.C3);
		public static explicit operator BezierQuad4D( Polynomial4D poly ) => poly.Degree < 3 ? new BezierQuad4D( CharMatrix.quadraticBezierInverse * (Vector4Matrix3x1)poly ) : throw new InvalidCastException( "Cannot cast a cubic polynomial to a quadratic curve" );
		public static explicit operator BezierCubic4D( Polynomial4D poly ) => new(CharMatrix.cubicBezierInverse * (Vector4Matrix4x1)poly);
		public static explicit operator CatRomCubic4D( Polynomial4D poly ) => new(CharMatrix.cubicCatmullRomInverse * (Vector4Matrix4x1)poly);
		public static explicit operator HermiteCubic4D( Polynomial4D poly ) => new(CharMatrix.cubicHermiteInverse * (Vector4Matrix4x1)poly);
		public static explicit operator UBSCubic4D( Polynomial4D poly ) => new(CharMatrix.cubicUniformBsplineInverse * (Vector4Matrix4x1)poly);

		#endregion

	}

}