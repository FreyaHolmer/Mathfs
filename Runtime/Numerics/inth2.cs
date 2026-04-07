// by Freya Holmér (https://github.com/FreyaHolmer/Mathfs)

using System;
using Unity.Mathematics;
using UnityEngine;
using static Freya.mathfs;

namespace Freya {

	/// <summary>A fixed precision data type for half-integers, using a single backing interger. For numbers like: 0, 0.5, 1, 1.5, 2, etc.</summary>
	[Serializable] public struct inth2 :
		IEquatable<inth2>,
		IVec2<inth2, inth, rat, rat, rat2>,
		INumber<inth2, int2>,
		ISignedNumber<int2>,
		IHalfNumber<int2>,
		IRoundable<int2> {

		/// <summary>The number of halves in the x axis</summary>
		[SerializeField] public inth x;
		/// <summary>The number of halves in the y axis</summary>
		[SerializeField] public inth y;

		public inth X => x;
		public inth Y => y;
		public inth this[ int i ] => i switch { 0 => x, 1 => y, _ => throw new IndexOutOfRangeException( i.ToString() ) };
		public bool isOrthogonal => abs.cmin == 0;
		public bool isZero => x == 0 && y == 0;

		public inth2( inth x, inth y ) => ( this.x, this.y ) = ( x, y );

		public bool isInteger => x.isInteger && y.isInteger;

		public inth2 rot90 => new(-y, x);
		public inth2 rotNeg90 => new(y, -x);
		public inth2 rot180 => -this;

		public inth2 FromVector2( Vector2 v, RoundingDirection rounding = RoundingDirection.ToEven ) =>
			new() {
				x = inth.fromFloat( v.x, rounding ),
				y = inth.fromFloat( v.y, rounding )
			};

		public int2 times2 => new(x.times2, y.times2);
		public inth cmin => x.min( y );
		public inth cmax => x.max( y );
		public inth csum => x + y;
		public rat dot( inth2 other ) => ( (rat2)this ).dot( other );
		public rat wedge( inth2 other ) => ( (rat2)this ).wedge( other );
		public inth2 to( inth2 target ) => target - this;
		public rat magSq => this.dot( this );
		public inth magChebyshev => this.abs.cmax;
		public inth magTaxicab => this.abs.csum;
		public inth2 abs => new(x.abs, y.abs);
		public inth2 max( inth2 other ) => new(x.max( other.x ), y.max( other.y ));
		public inth2 min( inth2 other ) => new(x.min( other.x ), y.min( other.y ));
		public int2 sign => new(x.sign, y.sign);
		public int2 round( RoundingDirection rounding = RoundingDirection.ToEven ) => new(x.round( rounding ), y.round( rounding ));
		public int2 floorToward0 => new(x.floorToward0, y.floorToward0);
		public int2 ceilAwayFrom0 => new(x.ceilAwayFrom0, y.ceilAwayFrom0);
		public int2 floor => new(x.floor, y.floor);
		public int2 ceil => new(x.ceil, y.ceil);
		public int quadrant => ceilAwayFrom0.quadrant();
		public int signedQuadrant => ceilAwayFrom0.signedQuadrant();
		public int2 quadrantBasisX => ceilAwayFrom0.quadrantBasisX();
		public (int2 x, int2 y) quadrantBasis => ceilAwayFrom0.quadrantBasis();
		public int pointSideOfPlane( inth2 planePos, inth2 planeNormal ) => this.times2.pointSideOfPlane( planePos.times2, planeNormal.times2 );

		public rat2 complexMul( inth2 other ) => ( (rat2)times2.complexMul( other.times2 ) ) / 4;
		public inth2 complexConj => new(x, -y);

		public override string ToString() => $"( {x}, {y} )";

		public static implicit operator rat2( inth2 ih ) => new(ih.x, ih.y);
		public static implicit operator inth2( int2 i ) => new(i.x, i.y);

		public static explicit operator int2( inth2 ih ) => new((int)ih.x, (int)ih.y);
		public static explicit operator float2( inth2 ih ) => new((float)ih.x, (float)ih.y);
		public static explicit operator Vector2( inth2 ih ) => new((float)ih.x, (float)ih.y);
		public static explicit operator double2( inth2 ih ) => new((double)ih.x, (double)ih.y);
		public static explicit operator Vector3( inth2 ih ) => new((float)ih.x, (float)ih.y, 0);
		public static explicit operator double3( inth2 ih ) => new((double)ih.x, (double)ih.y, 0);

		public static int2 zero => new(0, 0);
		public static inth2 half => new(inth.half, inth.half);
		public static int2 one => new(1, 1);

		// unary operations
		public static inth2 operator -( inth2 ih ) => new(-ih.x, -ih.y);
		public static inth2 operator +( inth2 ih ) => ih;

		// binary operations
		public static inth2 operator +( inth2 a, inth2 b ) => new(a.x + b.x, a.y + b.y);
		public static inth2 operator +( inth2 a, int2 b ) => new(a.x + b.x, a.y + b.y);
		public static inth2 operator +( int2 a, inth2 b ) => new(a.x + b.x, a.y + b.y);
		public static inth2 operator -( inth2 a, inth2 b ) => new(a.x - b.x, a.y - b.y);
		public static inth2 operator -( inth2 a, int2 b ) => new(a.x - b.x, a.y - b.y);
		public static inth2 operator -( int2 a, inth2 b ) => new(a.x - b.x, a.y - b.y);
		public static inth2 operator *( inth2 a, int2 b ) => new(a.x * b.x, a.y * b.y);
		public static inth2 operator *( int2 a, inth2 b ) => new(a.x * b.x, a.y * b.y);
		public static rat2 operator *( inth2 a, rat b ) => new(a.x * b, a.y * b);
		public static rat2 operator *( rat a, inth2 b ) => new(a * b.x, a * b.y);

		// comparison operators
		public static bool2 operator ==( inth2 a, inth2 b ) => new(a.x == b.x, a.y == b.y);
		public static bool2 operator !=( inth2 a, inth2 b ) => new(a.x != b.x, a.y != b.y);
		public static bool2 operator <( inth2 a, inth2 b ) => new(a.x < b.x, a.y < b.y);
		public static bool2 operator >( inth2 a, inth2 b ) => new(a.x > b.x, a.y > b.y);
		public static bool2 operator <=( inth2 a, inth2 b ) => new(a.x <= b.x, a.y <= b.y);
		public static bool2 operator >=( inth2 a, inth2 b ) => new(a.x >= b.x, a.y >= b.y);

		// comparison functions
		public bool Equals( inth2 other ) => x == other.x && y == other.y;
		public override bool Equals( object obj ) => obj is inth2 other && Equals( other );
		public override int GetHashCode() => HashCode.Combine( x, y );


	}

}