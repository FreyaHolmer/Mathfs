// by Freya Holmér (https://github.com/FreyaHolmer/Mathfs)

using System;
using UnityEngine;

namespace Freya {

	public struct Polynomial3D : IParamCurve3Diff<Vector3> {

		public Polynomial x;
		public Polynomial y;
		public Polynomial z;

		public Vector3 C0 {
			get => new(x.c0, y.c0, z.c0);
			set => ( x.c0, y.c0, z.c0 ) = ( value.x, value.y, value.z );
		}
		public Vector3 C1 {
			get => new(x.c1, y.c1, z.c1);
			set => ( x.c1, y.c1, z.c1 ) = ( value.x, value.y, value.z );
		}
		public Vector3 C2 {
			get => new(x.c2, y.c2, z.c2);
			set => ( x.c2, y.c2, z.c2 ) = ( value.x, value.y, value.z );
		}
		public Vector3 C3 {
			get => new(x.c3, y.c3, z.c3);
			set => ( x.c3, y.c3, z.c3 ) = ( value.x, value.y, value.z );
		}

		public Polynomial this[ int i ] => i switch { 0 => x, 1 => y, 2 => z, _ => throw new IndexOutOfRangeException( "Polynomial3D component index has to be either 0, 1, or 2" ) };

		public Polynomial3D( Polynomial x, Polynomial y, Polynomial z ) => ( this.x, this.y, this.z ) = ( x, y, z );

		/// <inheritdoc cref="Polynomial(float,float,float,float)"/>
		public Polynomial3D( Vector3 c0, Vector3 c1, Vector3 c2, Vector3 c3 ) {
			this.x = new Polynomial( c0.x, c1.x, c2.x, c3.x );
			this.y = new Polynomial( c0.y, c1.y, c2.y, c3.y );
			this.z = new Polynomial( c0.z, c1.z, c2.z, c3.z );
		}

		/// <inheritdoc cref="Polynomial(float,float,float)"/>
		public Polynomial3D( Vector3 c0, Vector3 c1, Vector3 c2 ) {
			this.x = new Polynomial( c0.x, c1.x, c2.x, 0 );
			this.y = new Polynomial( c0.y, c1.y, c2.y, 0 );
			this.z = new Polynomial( c0.z, c1.z, c2.z, 0 );
		}

		/// <inheritdoc cref="Polynomial(Matrix4x1)"/>
		public Polynomial3D( Vector3Matrix4x1 coefficients ) => ( x, y, z ) = ( new Polynomial( coefficients.X ), new Polynomial( coefficients.Y ), new Polynomial( coefficients.Z ) );

		/// <inheritdoc cref="Polynomial(Matrix4x1)"/>
		public Polynomial3D( Vector3Matrix3x1 coefficients ) => ( x, y, z ) = ( new Polynomial( coefficients.X ), new Polynomial( coefficients.Y ), new Polynomial( coefficients.Z ) );

		/// <inheritdoc cref="Polynomial.Eval(float)"/>
		public Vector3 Eval( float t ) => new(x.Eval( t ), y.Eval( t ), z.Eval( t ));

		/// <inheritdoc cref="Polynomial.Differentiate(int)"/>
		public Polynomial3D Differentiate( int n = 1 ) => new(x.Differentiate( n ), y.Differentiate( n ), z.Differentiate( n ));

		/// <inheritdoc cref="Polynomial.Compose(float,float)"/>
		public Polynomial3D Compose( float g0, float g1 ) => new(x.Compose( g0, g1 ), y.Compose( g0, g1 ), z.Compose( g0, g1 ));

		/// <inheritdoc cref="Polynomial2D.GetBounds01"/>
		public Bounds GetBounds01() => FloatRange.ToBounds( x.OutputRange01, y.OutputRange01, z.OutputRange01 );

		/// <inheritdoc cref="Polynomial.Split01"/>
		public (Polynomial3D pre, Polynomial3D post) Split01( float u ) {
			( Polynomial xPre, Polynomial xPost ) = x.Split01( u );
			( Polynomial yPre, Polynomial yPost ) = y.Split01( u );
			( Polynomial zPre, Polynomial zPost ) = z.Split01( u );
			return ( new Polynomial3D( xPre, yPre, zPre ), new Polynomial3D( xPost, yPost, zPost ) );
		}

		#region IParamCurve3Diff interface implementations

		public int Degree => Mathf.Max( (int)x.Degree, (int)y.Degree, (int)z.Degree );
		public Vector3 EvalDerivative( float t ) => Differentiate().Eval( t );
		public Vector3 EvalSecondDerivative( float t ) => Differentiate( 2 ).Eval( t );
		public Vector3 EvalThirdDerivative( float t = 0 ) => Differentiate( 3 ).Eval( 0 );

		#endregion

		#region Project Point

		/// <inheritdoc cref="Polynomial2D.ProjectPoint(Vector2,int,int)"/>
		public Vector3 ProjectPoint( Vector3 point, int initialSubdivisions = 16, int refinementIterations = 4 ) => ProjectPoint( point, out _, initialSubdivisions, refinementIterations );

		struct PointProjectSample {
			public float t;
			public float distDeltaSq;
			public Vector3 f;
			public Vector3 fp;
		}

		static PointProjectSample[] pointProjectGuesses = { default, default, default };

		/// <inheritdoc cref="Polynomial2D.ProjectPoint(Vector2,out float,int,int)"/>
		public Vector3 ProjectPoint( Vector3 point, out float t, int initialSubdivisions = 16, int refinementIterations = 4 ) {
			// define a bezier relative to the test point
			Polynomial3D curve = this;
			curve.x.c0 -= point.x; // constant coefficient defines the start position
			curve.y.c0 -= point.y;
			curve.z.c0 -= point.z;
			Vector3 curveStart = curve.Eval( 0 );
			Vector3 curveEnd = curve.Eval( 1 );

			PointProjectSample SampleDistSqDelta( float tSmp ) {
				PointProjectSample s = new PointProjectSample { t = tSmp };
				( s.f, s.fp ) = ( curve.Eval( tSmp ), curve.EvalDerivative( tSmp ) );
				s.distDeltaSq = Vector3.Dot( s.f, s.fp );
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
				Vector3 fpp = curve.EvalSecondDerivative( smp.t );
				float tNew = smp.t - Vector3.Dot( smp.f, smp.fp ) / ( Vector3.Dot( smp.f, fpp ) + Vector3.Dot( smp.fp, smp.fp ) );
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
			Vector3 ptClosest = ( firstClosest ? curveStart : curveEnd ) + point;
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

		public static Polynomial3D operator /( Polynomial3D p, float v ) => new(p.C0 / v, p.C1 / v, p.C2 / v, p.C3 / v);
		public static Polynomial3D operator *( Polynomial3D p, float v ) => new(p.C0 * v, p.C1 * v, p.C2 * v, p.C3 * v);
		public static Polynomial3D operator *( float v, Polynomial3D p ) => p * v;

		public static explicit operator Polynomial2D( Polynomial3D p ) => new(p.x, p.y);
		public static explicit operator Vector3Matrix3x1( Polynomial3D poly ) => new(poly.C0, poly.C1, poly.C2);
		public static explicit operator Vector3Matrix4x1( Polynomial3D poly ) => new(poly.C0, poly.C1, poly.C2, poly.C3);
		public static explicit operator BezierQuad3D( Polynomial3D poly ) => poly.Degree < 3 ? new BezierQuad3D( CharMatrix.quadraticBezierInverse * (Vector3Matrix3x1)poly ) : throw new InvalidCastException( "Cannot cast a cubic polynomial to a quadratic curve" );
		public static explicit operator BezierCubic3D( Polynomial3D poly ) => new(CharMatrix.cubicBezierInverse * (Vector3Matrix4x1)poly);
		public static explicit operator CatRomCubic3D( Polynomial3D poly ) => new(CharMatrix.cubicCatmullRomInverse * (Vector3Matrix4x1)poly);
		public static explicit operator HermiteCubic3D( Polynomial3D poly ) => new(CharMatrix.cubicHermiteInverse * (Vector3Matrix4x1)poly);
		public static explicit operator UBSCubic3D( Polynomial3D poly ) => new(CharMatrix.cubicUniformBsplineInverse * (Vector3Matrix4x1)poly);

		#endregion

	}

}