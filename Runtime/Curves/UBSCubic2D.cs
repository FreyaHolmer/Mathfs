using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Freya {

	/// <summary>An optimized 2D uniform B-spline segment</summary>
	[Serializable] public struct UBSCubic2D : IParamCurve3Diff<Vector2> {

		const MethodImplOptions INLINE = MethodImplOptions.AggressiveInlining;

		/// <summary>Creates a uniform cubic B-spline segment, given 4 control points</summary>
		/// <param name="p0">The first point of the B-spline hull</param>
		/// <param name="p1">The second point of the B-spline hull</param>
		/// <param name="p2">The third point of the B-spline hull</param>
		/// <param name="p3">The fourth point of the B-spline hull</param>
		public UBSCubic2D( Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3 ) {
			( this.p0, this.p1, this.p2, this.p3 ) = ( p0, p1, p2, p3 );
			validCoefficients = false;
			c3 = c2 = c1 = c0 = default;
		}

		#region Control Points

		[SerializeField] Vector2 p0, p1, p2, p3; // the points of the B-spline hull

		/// <summary>The first point of the B-spline hull</summary>
		public Vector2 P0 {
			[MethodImpl( INLINE )] get => p0;
			[MethodImpl( INLINE )] set => _ = ( p0 = value, validCoefficients = false );
		}

		/// <summary>The second point of the B-spline hull</summary>
		public Vector2 P1 {
			[MethodImpl( INLINE )] get => p1;
			[MethodImpl( INLINE )] set => _ = ( p1 = value, validCoefficients = false );
		}

		/// <summary>The third point of the B-spline hull</summary>
		public Vector2 P2 {
			[MethodImpl( INLINE )] get => p2;
			[MethodImpl( INLINE )] set => _ = ( p2 = value, validCoefficients = false );
		}

		/// <summary>The fourth point of the B-spline hull</summary>
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
		[NonSerialized] Vector2 c3, c2, c1, c0; // cached coefficients for fast evaluation

		// Coefficient Calculation
		[MethodImpl( INLINE )] void ReadyCoefficients() {
			if( validCoefficients )
				return; // no need to update
			validCoefficients = true;
			const float _6th = 1 / 6f;
			c3.x = _6th * ( -p0.x + 3 * ( p1.x - p2.x ) + p3.x );
			c2.x = 0.5f * ( p0.x - 2 * p1.x + p2.x );
			c1.x = 0.5f * ( -p0.x + p2.x );
			c0.x = _6th * ( p0.x + 4 * p1.x + p2.x );
			
			c3.y = _6th * ( -p0.y + 3 * ( p1.y - p2.y ) + p3.y );
			c2.y = 0.5f * ( p0.y - 2 * p1.y + p2.y );
			c1.y = 0.5f * ( -p0.y + p2.y );
			c0.y = _6th * ( p0.y + 4 * p1.y + p2.y );

		}

		/// <summary>The constant coefficient when evaluating this curve in the form C3*t³ + C2*t² + C1*t + C0</summary>
		public Vector2 C0 {
			[MethodImpl( INLINE )] get {
				ReadyCoefficients();
				return c0;
			}
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
			return ( c3, c2, c1, c0 );
		}

		#endregion

		#region Core IParamCurve Implementations

		public int Degree {
			[MethodImpl( INLINE )] get => 3;
		}
		public int Count {
			[MethodImpl( INLINE )] get => 4;
		}

		[MethodImpl( INLINE )] public Vector2 GetStartPoint() => C0;

		[MethodImpl( INLINE )] public Vector2 GetEndPoint() {
			ReadyCoefficients();
			float x = c0.x + c1.x + c2.x + c3.x;
			float y = c0.y + c1.y + c2.y + c3.y;
			return new Vector2( x, y );
		}

		[MethodImpl( INLINE )] public Vector2 GetPoint( float t ) {
			ReadyCoefficients();
			float t2 = t * t;
			float t3 = t2 * t;
			return new Vector2( t3 * c3.x + t2 * c2.x + t * c1.x + c0.x, t3 * c3.y + t2 * c2.y + t * c1.y + c0.y );
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

		/// <summary>Returns the exact cubic bézier representation of this segment</summary>
		public BezierCubic2D ToBezier() {
			float ax = p0.x + ( 2f / 3f ) * ( p1.x - p0.x );
			float bx = p1.x + ( 1f / 3f ) * ( p2.x - p1.x );
			float cx = p1.x + ( 2f / 3f ) * ( p2.x - p1.x );
			float dx = p2.x + ( 1f / 3f ) * ( p3.x - p2.x );
			float ay = p0.y + ( 2f / 3f ) * ( p1.y - p0.y );
			float by = p1.y + ( 1f / 3f ) * ( p2.y - p1.y );
			float cy = p1.y + ( 2f / 3f ) * ( p2.y - p1.y );
			float dy = p2.y + ( 1f / 3f ) * ( p3.y - p2.y );
			return new BezierCubic2D(
				new Vector2( 0.5f * ( ax + bx ), 0.5f * ( ay + by ) ),
				new Vector2( bx, by ),
				new Vector2( cx, cy ),
				new Vector2( 0.5f * ( cx + dx ), 0.5f * ( cy + dy ) )
			);
		}

	}

}