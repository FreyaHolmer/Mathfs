using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace Freya {

	public enum TypeSource {
		CsNative,
		UnityMath,
		Unity,
		Mathfs
	}

	public enum NumType {
		Bool2,
		Byte8,
		SByte8,
		Short16,
		UShort16,
		Int32,
		UInt32,
		Long64,
		ULong64,
		IntHalf,
		Rational,
		Half16,
		Float32,
		Double64,
	}

	public static class StaticAccessExtensions {


		public static readonly Dictionary<Type, NumericTypeInfo> numericInfo = new() {
			{ typeof(rat), new(typeof(rat), NumType.Rational, dims: 1, TypeSource.Mathfs) },
			{ typeof(rat2), new(typeof(rat2), NumType.Rational, dims: 2, TypeSource.Mathfs) },
			// { typeof(rat3), new(typeof(rat3), NumType.Rational, dims: 3, TypeSource.Mathfs) }, // todo
			// { typeof(rat4), new(typeof(rat4), NumType.Rational, dims: 4, TypeSource.Mathfs) },

			{ typeof(inth), new(typeof(inth), NumType.IntHalf, dims: 1, TypeSource.Mathfs) },
			{ typeof(inth2), new(typeof(inth2), NumType.IntHalf, dims: 2, TypeSource.Mathfs) },
			// { typeof(inth3), new(typeof(inth3), NumType.IntHalf, dims: 3, TypeSource.Mathfs) }, // todo
			// { typeof(inth4), new(typeof(inth4), NumType.IntHalf, dims: 4, TypeSource.Mathfs) },

			{ typeof(bool), new(typeof(bool), NumType.Bool2, dims: 1, TypeSource.CsNative) },
			{ typeof(bool2), new(typeof(bool2), NumType.Bool2, dims: 2, TypeSource.UnityMath) },
			{ typeof(bool3), new(typeof(bool3), NumType.Bool2, dims: 3, TypeSource.UnityMath) },
			{ typeof(bool4), new(typeof(bool4), NumType.Bool2, dims: 4, TypeSource.UnityMath) },

			{ typeof(byte), new(typeof(byte), NumType.Byte8, dims: 1, TypeSource.CsNative) },
			{ typeof(sbyte), new(typeof(sbyte), NumType.SByte8, dims: 1, TypeSource.CsNative) },
			{ typeof(short), new(typeof(short), NumType.Short16, dims: 1, TypeSource.CsNative) },
			{ typeof(ushort), new(typeof(ushort), NumType.UShort16, dims: 1, TypeSource.CsNative) },
			{ typeof(long), new(typeof(long), NumType.Long64, dims: 1, TypeSource.CsNative) },
			{ typeof(ulong), new(typeof(ulong), NumType.ULong64, dims: 1, TypeSource.CsNative) },

			{ typeof(uint), new(typeof(uint), NumType.UInt32, dims: 1, TypeSource.CsNative) },
			{ typeof(uint2), new(typeof(uint2), NumType.UInt32, dims: 2, TypeSource.UnityMath) },
			{ typeof(uint3), new(typeof(uint3), NumType.UInt32, dims: 3, TypeSource.UnityMath) },
			{ typeof(uint4), new(typeof(uint4), NumType.UInt32, dims: 4, TypeSource.UnityMath) },

			{ typeof(int), new(typeof(int), NumType.Int32, dims: 1, TypeSource.CsNative) },
			{ typeof(int2), new(typeof(int2), NumType.Int32, dims: 2, TypeSource.UnityMath) },
			{ typeof(Vector2Int), new(typeof(Vector2Int), NumType.Int32, dims: 2, TypeSource.Unity) }, // legacy unity int2
			{ typeof(int3), new(typeof(int3), NumType.Int32, dims: 3, TypeSource.UnityMath) },
			{ typeof(Vector3Int), new(typeof(Vector3Int), NumType.Int32, dims: 3, TypeSource.Unity) }, // legacy unity int3
			{ typeof(int4), new(typeof(int4), NumType.Int32, dims: 4, TypeSource.UnityMath) },
			// { typeof(Vector4Int), new(typeof(Vector4Int), NumType.Int32, dims: 4, TypeSource.Unity) }, // Unity doesn't define this

			// todo: Colors have a bunch of exceptions, disabling for now
			// { typeof(Color32), new(typeof(Color32), NumType.Int32, dims: 4, TypeSource.Unity) }, // basically a byte4 type
			// { typeof(Color), new(typeof(Color), NumType.Float32, dims: 4, TypeSource.Unity) },

			// todo: Unity's half types become floats under basic operations, so they require a lot of exceptions in the codegen. Disabling for now
			// { typeof(half), new(typeof(half), NumType.Half16, dims: 1, TypeSource.UnityMath) }, // exception: unity defines this
			// { typeof(half2), new(typeof(half2), NumType.Half16, dims: 2, TypeSource.UnityMath) },
			// { typeof(half3), new(typeof(half3), NumType.Half16, dims: 3, TypeSource.UnityMath) },
			// { typeof(half4), new(typeof(half4), NumType.Half16, dims: 4, TypeSource.UnityMath) },

			{ typeof(float), new(typeof(float), NumType.Float32, dims: 1, TypeSource.CsNative) },
			{ typeof(float2), new(typeof(float2), NumType.Float32, dims: 2, TypeSource.UnityMath) },
			{ typeof(Vector2), new(typeof(Vector2), NumType.Float32, dims: 2, TypeSource.Unity) }, // legacy unity float2
			{ typeof(float3), new(typeof(float3), NumType.Float32, dims: 3, TypeSource.UnityMath) },
			{ typeof(Vector3), new(typeof(Vector3), NumType.Float32, dims: 3, TypeSource.Unity) }, // legacy unity float3
			{ typeof(float4), new(typeof(float4), NumType.Float32, dims: 4, TypeSource.UnityMath) },
			{ typeof(Vector4), new(typeof(Vector4), NumType.Float32, dims: 4, TypeSource.Unity) }, // legacy unity float4

			{ typeof(double), new(typeof(double), NumType.Double64, dims: 1, TypeSource.CsNative) },
			{ typeof(double2), new(typeof(double2), NumType.Double64, dims: 2, TypeSource.UnityMath) },
			{ typeof(double3), new(typeof(double3), NumType.Double64, dims: 3, TypeSource.UnityMath) },
			{ typeof(double4), new(typeof(double4), NumType.Double64, dims: 4, TypeSource.UnityMath) },
		};

		// public static IEnumerable<(Type nType,Type iType)> ExternalTypes


		// v.abs().csum() // taxicab / L1
		// v.abs().cmax() // chebyshev / max norm

		public static IEnumerable<Type> InterfacesOfExternalType( Type nType ) {
			HashSet<Type> interfaces = new HashSet<Type>();

			NumericTypeInfo info = numericInfo[nType];
			Type V = nType;
			Type R = info.TypeAfterRounding; // rounded type
			Type C = info.ComponentType; // component type
			Type D = C; // dot product result type
			Type M = V; // complex multiplication result type


			interfaces.UnionWith( InterfacesOf( typeof(ISignedNumber<>).MakeGenericType( R ) ) );
			if( info.numType is NumType.Double64 or NumType.Float32 or NumType.Half16 or NumType.Rational or NumType.IntHalf )
				interfaces.UnionWith( InterfacesOf( typeof(IRoundable<>).MakeGenericType( R ) ) );

			switch( info.dims ) {
				case 1:
					interfaces.UnionWith( InterfacesOf( typeof(INumber<>).MakeGenericType( V ) ) );
					break;
				case 2:
					Type W2 = D; // wedge product result type
					Type ProjSc2 = info.ScalarProjectionType;
					interfaces.UnionWith( InterfacesOf( typeof(IVec2<,,,,,>).MakeGenericType( V, C, D, W2, M, ProjSc2 ) ) );
					break;
				case 3:
					Type W3 = V; // wedge product result type
					Type ProjSc3 = info.ScalarProjectionType;
					interfaces.UnionWith( InterfacesOf( typeof(IVec3<,,,>).MakeGenericType( V, C, D, W3 ) ) );
					break;
				case 4:
					interfaces.UnionWith( InterfacesOf( typeof(IVec4<,,>).MakeGenericType( V, C, D ) ) );
					break;
			}

			// todo: only 2D vectors so far
			// if( info.dims == 2 ) // typeof(IVec2<V, C, D, W, M>)
			// 	else


			return interfaces;
		}

		static IEnumerable<Type> InterfacesOf( Type t ) {
			if( t == null )
				yield break;
			if( t.IsInterface )
				yield return t;
			foreach( Type iType in t.GetInterfaces() )
				yield return iType;
		}

		public static string GetCustomImplementation( Type iType, Type nType, string member ) {
			Type iTypeDef = iType.IsGenericType ? iType.GetGenericTypeDefinition() : iType;
			if( numericInfo.TryGetValue( nType, out NumericTypeInfo info ) == false ) {
				Debug.LogWarning( $"Missing custom implementation for nType {nType} with iType {iType} member {member} itypedef: {iTypeDef}" );
				return "default";
			}

			if( iTypeDef == typeof(INumberBase) ) {
				if( member == nameof(INumberBase.isInteger) ) {
					if( info.IsAlwaysIntegerValue )
						return "true";
					if( info.IsScalar )
						return $"{{0}} == {CsMathClass( nType )}.Truncate( {{0}} )";
					return info.JoinComponents( " && ", ( i, c ) => $"{{0}}.{c}.isInteger()" );
				}
				if( member == nameof(INumberBase.isZero) ) {
					if( info.IsScalar )
						return "{0} == 0";
					if( info.source == TypeSource.UnityMath )
						return "math.all( {0} == 0 )";
					return info.JoinComponents( " && ", ( i, c ) => $"{{0}}.{c} == 0" );
				}
				if( member == nameof(INumberBase.isOrthogonal) ) {
					if( info.IsScalar )
						return "true";
					string value = "{0}";
					if( info.source == TypeSource.Unity && info.numType == NumType.Int32 ) {
						// Vector2Int and Vector3Int requires a cast
						value = $"new int{info.dims}({info.JoinComponents( ", ", ( i, c ) => $"{{0}}.{c}" )})";
					}
					string ceilOptionally = info.IsAlwaysIntegerValue ? "" : ".ceilAwayFrom0()";
					return $"({value}{ceilOptionally}.abs() > 0).csum() <= 1";
				}
			} else if( iTypeDef == typeof(INumber<>) ) {
				switch( member ) {
					case nameof(INumber<object>.abs): return StandardMathOp( info, "math.abs({0})", "Math.Abs({0})", "abs()" );
					case nameof(INumber<object>.min): return StandardMathOp( info, "math.min({0},{1})", "Math.Min({0},{1})", "min({0})" );
					case nameof(INumber<object>.max): return StandardMathOp( info, "math.max({0},{1})", "Math.Max({0},{1})", "max({0})" );
					case nameof(INumber<object>.to):  return "{1} - {0}"; // b - a
				}
			} else if( iTypeDef == typeof(IComplex<,>) ) {
				switch( member ) {
					case nameof(IComplex<object, object>.complexMul):  return "new({0}.x*{1}.x-{0}.y*{1}.y, {0}.x*{1}.y+{0}.y*{1}.x)";
					case nameof(IComplex<object, object>.complexConj): return "new({0}.x, -{0}.y)";
				}
			} else if( iTypeDef == typeof(IDotProduct<,>) ) {
				switch( member ) {
					case nameof(IDotProduct<object, object>.dot):
						// unity.mathematics does not implement dot() for the half type 
						bool isNotHalf = info.ComponentType != typeof(half);
						if( info.source == TypeSource.UnityMath && isNotHalf )
							return "math.dot( {0}, {1} )";
						return info.CompSum( ( i, c ) => $"{{0}}.{c}*{{1}}.{c}" );
				}
			} else if( iTypeDef == typeof(IWedgeProduct<,>) ) {
				switch( member ) {
					case nameof(IWedgeProduct<object, object>.wedge):
						return info.dims switch {
							2 => "{0}.x*{1}.y - {0}.y*{1}.x",
							3 => "new(" +
								 "{0}.y * {1}.z - {0}.z * {1}.y," +
								 "{0}.z * {1}.x - {0}.x * {1}.z," +
								 "{0}.x * {1}.y - {0}.y * {1}.x" +
								 ")",
							_ => throw new NotImplementedException( $"Wedge product not implemented for elements of dimension {info.dims}" )
						};
				}
			} else if( iTypeDef == typeof(ISqrMag<>) ) {
				switch( member ) {
					case nameof(IDotProduct<object, object>.magSq):
						// unity.mathematics does not implement dot() for the half type 
						bool isNotHalf = info.ComponentType != typeof(half);
						if( info.source == TypeSource.UnityMath && isNotHalf )
							return "math.dot( {0}, {0} )";
						return info.CompSum( ( i, c ) => $"{{0}}.{c}*{{0}}.{c}" );
				}
			} else if( iTypeDef == typeof(ISignedNumber<>) ) {
				if( member == nameof(ISignedNumber<object>.sign) ) {
					if( info.source == TypeSource.CsNative )
						return "Math.Sign({0})";
					if( info.source == TypeSource.UnityMath && info.numType is NumType.Float32 or NumType.Double64 )
						return $"(int{info.dims})math.sign({{0}})"; // floats/doubles 
					return info.NewFromComps( ( i, c ) => $"{{0}}.{c}.sign()" );
				}
			} else if( iTypeDef == typeof(IVec2<,,,,,>) ) {
				if( member == nameof(IVec2<object, object, object, object, object, object>.rot90) ) {
					return "new(-{0}.y,{0}.x)";
				} else if( member == nameof(IVec2<object, object, object, object, object, object>.rotNeg90) ) {
					return "new({0}.y,-{0}.x)";
				} else if( member == nameof(IVec2<object, object, object, object, object, object>.rot180) ) {
					return "new(-{0}.x,-{0}.y)";
				}
			} else if( iTypeDef == typeof(IRoundable<>) ) {
				switch( member ) {
					case nameof(IRoundable<object>.floorToward0):
						if( info.source == TypeSource.CsNative && info.IsScalar )
							return "(int)({0}<0?math.ceil({0}):math.floor({0}))";
						return info.NewFromComps( ( i, c ) => $"{{0}}.{c}.floorToward0()" );
					case nameof(IRoundable<object>.ceilAwayFrom0):
						if( info.source == TypeSource.CsNative && info.IsScalar )
							return "(int)({0}<0?math.floor({0}):math.ceil({0}))";
						return info.NewFromComps( ( i, c ) => $"{{0}}.{c}.ceilAwayFrom0()" );
					case nameof(IRoundable<object>.floor):
						if( info.source == TypeSource.CsNative && info.IsScalar )
							return info.numType switch {
								NumType.Double64 => "(int)Math.Floor({0})",
								NumType.Float32  => "(int)MathF.Floor({0})",
								_                => throw new NotImplementedException( info.numType.ToString() )
							};
						else if( info.source == TypeSource.UnityMath )
							return $"(int{info.dims})math.floor({{0}})";
						return info.NewFromComps( ( i, c ) => $"{{0}}.{c}.floor()" );
					case nameof(IRoundable<object>.ceil):
						if( info.source == TypeSource.CsNative && info.IsScalar )
							return info.numType switch {
								NumType.Double64 => "(int)Math.Ceiling({0})",
								NumType.Float32  => "(int)MathF.Ceiling({0})",
								_                => throw new NotImplementedException( info.numType.ToString() )
							};
						else if( info.source == TypeSource.UnityMath )
							return $"(int{info.dims})math.ceil({{0}})";
						return info.NewFromComps( ( i, c ) => $"{{0}}.{c}.ceil()" );
					case nameof(IRoundable<object>.round):
						if( info.source == TypeSource.CsNative && info.IsScalar )
							return info.numType switch {
								NumType.Double64 => "(int)Math.Round( {0}, (MidpointRounding)rounding )",
								NumType.Float32  => "(int)MathF.Round( {0}, (MidpointRounding)rounding )",
								_                => throw new NotImplementedException( info.numType.ToString() )
							};
						return info.NewFromComps( ( i, c ) => $"{{0}}.{c}.round(rounding)" );
				}
			} else if( iTypeDef == typeof(IVecComponents<>) ) {
				switch( member ) {
					case "cmin": return UnityMathVecsElseComponentWise( info, "math.cmin({0})", info.CompAggrInstanceFuncs( "min" ) );
					case "cmax": return UnityMathVecsElseComponentWise( info, "math.cmax({0})", info.CompAggrInstanceFuncs( "max" ) );
					case "csum": return UnityMathVecsElseComponentWise( info, "math.csum({0})", info.CompSum( ( i, c ) => $"{{0}}.{c}" ) );
				}
			} else if( iTypeDef == typeof(IVec1Base<,,>) && IVecNBase( info, member, 1, out string str1 ) ) {
				return str1;
			} else if( iTypeDef == typeof(IVec2Base<,,>) && IVecNBase( info, member, 2, out string str2 ) ) {
				return str2;
			} else if( iTypeDef == typeof(IVec3Base<,,>) && IVecNBase( info, member, 3, out string str3 ) ) {
				return str3;
			} else if( iTypeDef == typeof(IVec4Base<,,>) && IVecNBase( info, member, 4, out string str4 ) ) {
				return str4;
			} else if( iTypeDef == typeof(IVec<,,>) ) {
				switch( member ) {
					case nameof(IVec<object, object, object>.magChebyshev):     return "{0}.abs().cmax()";
					case nameof(IVec<object, object, object>.magTaxicab):       return "{0}.abs().csum()";
					case nameof(IVec<object, object, object>.pointSideOfPlane): return "({0}-{1}).dot({2}).sign()";
				}
			} else if( iTypeDef == typeof(IQuadrant2D) ) {
				string rounding = info.numType is NumType.Float32 or NumType.Double64 ? $".{nameof(IRoundable<object>.ceilAwayFrom0)}()" : "";
				return member switch {
					nameof(IQuadrant2D.quadrant) => "{0}.y switch {{" +
													"> 00 when {0}.x <= 0 => 1," +
													"<= 0 when {0}.x < 00 => 2," +
													"< 00 when {0}.x >= 0 => 3," +
													"_ => 0 }}",
					nameof(IQuadrant2D.quadrantBasisX) => $"mathfs.{nameof(mathfs.quadrantToBasisX)}({{0}}{rounding}.quadrant())",
					nameof(IQuadrant2D.quadrantBasis)  => $"mathfs.{nameof(mathfs.quadrantToBasis)}({{0}}{rounding}.quadrant())",
					nameof(IQuadrant2D.signedQuadrant) => $"mathfs.{nameof(mathfs.quadrantToSignedQuadrant)}({{0}}{rounding}.quadrant())",
					_                                  => throw new NotImplementedException()
				};
			} else if( iTypeDef == typeof(IVecProjections<,>) ) {
				switch( member ) {
					case nameof(IVecProjections<object, object>.projTValue): return "{0}.dot({1})/{1}.dot({1})";
				}
			}

			Debug.LogWarning( $"Missing custom implementation for nType {nType} with iType {iType} member {member}" );
			return "default";
		}

		public static bool IVecNBase( NumericTypeInfo info, string member, int dim, out string str ) {
			int comp = member[^1] switch {
				'X' => 0,
				'Y' => 1,
				'Z' => 2,
				'W' => 3,
				_   => 0,
			};

			if( member is "X" or "Y" or "Z" or "W" ) {
				str = $"{{0}}.{member.ToLower()}";
				return true;
			}
			if( member is "flipX" or "flipY" or "flipZ" or "flipW" ) {
				str = info.NewFromComps( ( i, c ) => $"{( i == comp ? "-" : "+" )}{{0}}.{c}" );
				return true;
			}
			if( member is "zeroX" or "zeroY" or "zeroZ" or "zeroW" ) {
				str = info.NewFromComps( ( i, c ) => i == comp ? "0" : $"{{0}}.{c}" );
				return true;
			}

			str = default;
			return false;
		}

		public static string UnityMathVecsElseComponentWise( NumericTypeInfo info, string funcMathematics, string perComp ) {
			if( info.source == TypeSource.UnityMath && info.IsScalar == false )
				return funcMathematics;
			return perComp;
			// if( info.source == TypeSource.CsNative )
			// 	return $"{funcNative}";
			// return info.NewFromComponents( ( i, c ) => $"{{0}}.{c}.{string.Format( funcCompExtension, $"{{1}}.{c}" )}" );
		}

		public static string StandardMathOp( NumericTypeInfo info, string funcMathematics, string funcNative, string funcCompExtension ) {
			if( info.source == TypeSource.UnityMath )
				return $"{funcMathematics}";
			if( info.source == TypeSource.CsNative )
				return $"{funcNative}";
			return info.NewFromComps( ( i, c ) => $"{{0}}.{c}.{string.Format( funcCompExtension, $"{{1}}.{c}" )}" );
		}

		public static string CsMathClass( Type type ) {
			if( type == typeof(float) )
				return nameof(MathF);
			if( type == typeof(double) )
				return nameof(Math);
			throw new NotImplementedException( type.FullName );
		}


	}

}