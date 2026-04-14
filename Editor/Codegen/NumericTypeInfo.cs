using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

namespace Freya {

	public struct NumericTypeInfo {
		public Type nType;
		public int dims;
		public TypeSource source;
		public NumType numType;

		public NumericTypeInfo( Type nType, NumType numType, int dims, TypeSource source ) {
			this.nType = nType;
			this.numType = numType;
			this.dims = dims;
			this.source = source;
		}

		public bool IsScalar => dims == 1;
		public bool IsExternalType => source != TypeSource.Mathfs;
		public IEnumerable<int> Components() => Enumerable.Range( 0, dims );
		public IEnumerable<T> Components<T>( Func<int, char, T> selector ) => Components().Select( i => selector( i, "xyzw"[i] ) );
		public IEnumerable<(int i, char c)> ComponentsTuples => Components().Select( i => ( i, "xyzw"[i] ) );
		public string JoinComponents<T>( string separator, Func<int, char, T> selector ) => string.Join( separator, Components( selector ) );
		public string CompSum<T>( Func<int, char, T> selector ) => string.Join( "+", Components( selector ) );
		public string NewFromComps<T>( Func<int, char, T> selector ) => $"new({string.Join( ", ", Components( selector ) )})";

		public string CompAggrInstanceFuncs( string funcName ) {
			if( dims == 1 )
				return "{0}";
			return ComponentsTuples.Skip( 1 ).Aggregate( "{0}.x", ( acc, item ) => $"{{0}}.{item.c}.{funcName}({acc})" );
		}
		// 	.Aggregate( "", (agg,elem) => $"{prev}.{funcName}"  );


		// x
		// x.max(y)
		// x.max(y.max(z))
		// x.max(y.max(z.max(w)))

		// max( x, y )
		// max( x, max( y, z ) )
		// max( x, max( y, max( z, w ) ) )

		public bool IsAlwaysIntegerValue =>
			numType is
				NumType.Bool2 or
				NumType.Byte8 or NumType.SByte8 or
				NumType.Short16 or NumType.UShort16 or
				NumType.Int32 or NumType.UInt32 or
				NumType.Long64 or NumType.ULong64;

		public bool IsFloatingPoint => numType is NumType.Float32 or NumType.Double64;
		public bool IsHalfInteger => numType is NumType.IntHalf;
		public bool IsRational => numType is NumType.Rational;

		public Type TypeAfterRounding =>
			dims switch {
				1 => typeof(int),
				2 => typeof(int2),
				3 => typeof(int3),
				4 => typeof(int4),
				_ => throw new IndexOutOfRangeException()
			};
		public Type ComponentType =>
			numType switch {
				NumType.Bool2    => typeof(bool),
				NumType.Byte8    => typeof(byte),
				NumType.SByte8   => typeof(sbyte),
				NumType.Short16  => typeof(short),
				NumType.UShort16 => typeof(ushort),
				NumType.Int32    => typeof(int),
				NumType.UInt32   => typeof(uint),
				NumType.Long64   => typeof(long),
				NumType.ULong64  => typeof(ulong),
				NumType.IntHalf  => typeof(inth),
				NumType.Rational => typeof(rat),
				NumType.Half16   => typeof(half),
				NumType.Float32  => typeof(float),
				NumType.Double64 => typeof(double),
				_                => throw new IndexOutOfRangeException()
			};

		public Type ScalarProjectionType {
			get {
				if( IsAlwaysIntegerValue || IsRational || IsHalfInteger )
					return typeof(rat);
				if( IsFloatingPoint )
					return ComponentType; // double/float
				throw new NotImplementedException();
			}
		}
		public Type VectorProjectionType {
			get {
				if( IsAlwaysIntegerValue || IsRational || IsHalfInteger )
					return GetTypeMatching( dims, typeof(rat) ); // integers and half-integers get type promoted to rational
				return nType;
			}
		}

		public static Type GetTypeMatching( int dims, Type componentType ) {
			if( componentType == typeof(bool) )
				switch( dims ) {
					case 1: return typeof(bool);
					case 2: return typeof(bool2);
					case 3: return typeof(bool3);
					case 4: return typeof(bool4);
				}
			if( componentType == typeof(int) )
				switch( dims ) {
					case 1: return typeof(int);
					case 2: return typeof(int2);
					case 3: return typeof(int3);
					case 4: return typeof(int4);
				}
			if( componentType == typeof(float) )
				switch( dims ) {
					case 1: return typeof(float);
					case 2: return typeof(float2);
					case 3: return typeof(float3);
					case 4: return typeof(float4);
				}
			if( componentType == typeof(double) )
				switch( dims ) {
					case 1: return typeof(double);
					case 2: return typeof(double2);
					case 3: return typeof(double3);
					case 4: return typeof(double4);
				}
			if( componentType == typeof(half) )
				switch( dims ) {
					case 1: return typeof(half);
					case 2: return typeof(half2);
					case 3: return typeof(half3);
					case 4: return typeof(half4);
				}
			if( componentType == typeof(byte) )
				switch( dims ) {
					case 1: return typeof(byte);
					// case 2: return typeof(byte2);
					// case 3: return typeof(byte3);
					// case 4: return typeof(byte4);
				}
			if( componentType == typeof(sbyte) )
				switch( dims ) {
					case 1: return typeof(sbyte);
					// case 2: return typeof(sbyte2);
					// case 3: return typeof(sbyte3);
					// case 4: return typeof(sbyte4);
				}
			if( componentType == typeof(short) )
				switch( dims ) {
					case 1: return typeof(short);
					// case 2: return typeof(short2);
					// case 3: return typeof(short3);
					// case 4: return typeof(short4);
				}
			if( componentType == typeof(ushort) )
				switch( dims ) {
					case 1: return typeof(ushort);
					// case 2: return typeof(ushort2);
					// case 3: return typeof(ushort3);
					// case 4: return typeof(ushort4);
				}
			if( componentType == typeof(uint) )
				switch( dims ) {
					case 1: return typeof(uint);
					case 2: return typeof(uint2);
					case 3: return typeof(uint3);
					case 4: return typeof(uint4);
				}
			if( componentType == typeof(long) )
				switch( dims ) {
					case 1: return typeof(long);
					// case 2: return typeof(long2);
					// case 3: return typeof(long3);
					// case 4: return typeof(long4);
				}
			if( componentType == typeof(ulong) )
				switch( dims ) {
					case 1: return typeof(ulong);
					// case 2: return typeof(ulong2);
					// case 3: return typeof(ulong3);
					// case 4: return typeof(ulong4);
				}
			if( componentType == typeof(inth) )
				switch( dims ) {
					case 1: return typeof(inth);
					case 2: return typeof(inth2);
					// case 3: return typeof(inth3);
					// case 4: return typeof(inth4);
				}
			if( componentType == typeof(rat) )
				switch( dims ) {
					case 1: return typeof(rat);
					case 2: return typeof(rat2);
					// case 3: return typeof(rat3);
					// case 4: return typeof(rat4);
				}
			throw new NotImplementedException( $"Missing implementation of a {dims}D {componentType.Name} vector" );
		}


	}

}