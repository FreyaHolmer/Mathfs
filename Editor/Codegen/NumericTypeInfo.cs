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


	}

}