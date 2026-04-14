// by Freya Holmér (https://github.com/FreyaHolmer/Mathfs)

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.Mathematics;
using UnityEditor;

namespace Freya {

	public static class MathfsCodegen {

		static GUID guidRuntimeAsm = new("6071c9f2ce0a4407c93af459fa416e54"); // Mathfs.asmdef
		static string PathRuntime => Path.GetDirectoryName( AssetDatabase.GUIDToAssetPath( guidRuntimeAsm ) );
		public static string PathSpline => $"{PathRuntime}/Splines";
		public static string PathNumerics => $"{PathRuntime}/Numerics";
		public static string PathStaticAccess => $"{PathRuntime}/Generated static functions";
		public static string PathMatrices => PathNumerics;

		static Dictionary<string, StaticAccessInterfaceGenerator> iDefCodegens = new();

		static StaticAccessInterfaceGenerator GetStaticCodegen( Type iType ) {
			// Type iDef = iType.IsGenericType ? iType.GetGenericTypeDefinition() : iType;
			string iName = iType.Name.Split( "`" )[0];
			if( iDefCodegens.TryGetValue( iName, out StaticAccessInterfaceGenerator code ) == false )
				iDefCodegens.Add( iName, code = new StaticAccessInterfaceGenerator( iType ) );
			return code;
		}

		[MenuItem( "Assets/Run Mathfs Codegen" )]
		public static void Regenerate() {
			SplineCodegen.GenerateUniformSplines( PathSpline );
			MatrixCodegen.GenerateMatrices( PathMatrices );

			GenerateStaticAccess( PathStaticAccess );

			// Debug.Log( "Type: " + typeof(rat2).inter );
		}

		/// <summary>Interfaces ignored by the codegen</summary>
		static Type[] interfaceBlacklist = new[] {
			typeof(IEquatable<>),
			typeof(IComparable<>),
		};

		static bool IsValidInterface( Type type ) {
			if( type.IsGenericType )
				type = type.GetGenericTypeDefinition();
			return interfaceBlacklist.Contains( type ) == false;
		}

		public static Type[] numericTypes = new[] {
			typeof(rat),
			typeof(rat2),
			typeof(inth),
			typeof(inth2),
		};
		public static Type[] externalTypes = new[] {
			typeof(int),
			typeof(int2),
			typeof(int3),
			typeof(int4),
		};


		static void GenerateStaticAccess( string pathStaticAccess ) {
			iDefCodegens.Clear();

			// collect all statics
			foreach( Type type in numericTypes ) {
				GenerateStaticAccessForInterface( type );
			}

			// extension interfaces/statics for external types
			foreach( ( Type nType, NumericTypeInfo info ) in StaticAccessExtensions.numericInfo ) {
				if( info.IsExternalType && ( info.numType is NumType.Float32 or NumType.Half16 or NumType.Double64 or NumType.Int32 ) ) {
					foreach( Type iType in StaticAccessExtensions.InterfacesOfExternalType( nType ) ) {
						GetStaticCodegen( iType ).GenerateSpecificsForType( nType, iType );
					}
				}
			}

			// finalize static codegen
			foreach( StaticAccessInterfaceGenerator gen in iDefCodegens.Values ) {
				gen.GenerateCode( pathStaticAccess );
			}
		}

		static void GenerateStaticAccessForInterface( Type type ) {
			foreach( Type iType in type.GetInterfaces().Where( IsValidInterface ) )
				GetStaticCodegen( iType ).GenerateSpecificsForType( type, iType );
		}


	}


}