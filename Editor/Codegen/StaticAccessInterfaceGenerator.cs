using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Freya {

	public class StaticAccessInterfaceGenerator {

		const string TSelf = "Self";
		const string StaticAccessClassName = nameof(mathfs);
		const string StaticAccessClassNameGenerics = StaticAccessClassName + "_generics";
		const string Namespace = nameof(Freya);

		public Type iTypeDef;
		public List<string> generics = new();
		public List<string> specifics = new();

		string FileName => $"{IName}_static.cs";
		bool IsGenericInterface => iTypeDef.IsGenericType;
		string IName => IsGenericInterface ? iTypeDef.Name.Split( "`" )[0] : iTypeDef.Name;
		IEnumerable<string> GenericTypeDefNames => iTypeDef.GetGenericArguments().Select( x => x.Name );
		string GenericsDefCsv => string.Join( ",", GenericTypeDefNames );
		string GenericsDefWithSelfCsv => string.Join( ",", GenericTypeDefNames.Append( TSelf ) );
		string INameInheritDoc => IName + ( IsGenericInterface ? $"{{{GenericsDefCsv}}}" : "" );
		string INameWithGenerics => IName + ( IsGenericInterface ? $"<{GenericsDefCsv}>" : "" );
		// string INameWithGenericsAndSelf => IName + $"<{GenericsDefWithSelfCsv}>";

		public void GenerateCode( string targetFolder ) {
			if( generics.Count == 0 && specifics.Count == 0 )
				return; // nothing to generate
			CodeGenerator code = new();
			code.AppendHeader();
			code.AppendLine( "using System;" );
			code.AppendLine( "using Unity.Mathematics;" );
			code.AppendLine( "using UnityEngine;" );
			using( code.BracketScope( $"namespace {Namespace}" ) ) {
				if( generics.Count > 0 )
					using( code.BracketScope( $"public static partial class {StaticAccessClassNameGenerics}" ) )
						generics.ForEach( code.AppendLine );
				if( specifics.Count > 0 )
					using( code.BracketScope( $"public static partial class {StaticAccessClassName}" ) )
						specifics.ForEach( code.AppendLine );
			}
			Debug.Log( $"{FileName}:\n{string.Join( "\n", code.content )}" );
			File.WriteAllLines( $"{targetFolder}/{FileName}", code.content );
		}

		public bool PropertyIsValid( PropertyInfo p ) => p.IsSpecialName == false && p.GetIndexParameters().Length == 0;
		public bool MethodIsValid( MethodInfo p ) => p.IsSpecialName == false;

		public IEnumerable<PropertyInfo> PropertiesITypeDef => iTypeDef.GetProperties().Where( PropertyIsValid );
		public IEnumerable<MethodInfo> MethodsITypeDef => iTypeDef.GetMethods().Where( MethodIsValid );

		public StaticAccessInterfaceGenerator( Type iType ) {
			iTypeDef = iType.IsGenericType ? iType.GetGenericTypeDefinition() : iType;

			// Generic implementations of each property, eg:
			// /// <inheritdoc cref="INumber.isInteger" />
			// public static bool isInteger<T>( T v ) where T : INumber => v.isInteger;

			// but also, sometimes the Self type is already in the return type
			// /// <inheritdoc cref="INumber{N}.abs" />
			// public static T abs<T>( T x ) where T : INumber<T> => x.abs;
			// incorrectly generates as:
			// public static N abs<N,Self>(Self v) where Self : INumber<N> => v.abs;


			// GENERIC definitions
			foreach( PropertyInfo prop in PropertiesITypeDef ) {
				string memName = prop.Name;
				string access = "public static";
				string retType = GetReturnTypeName( prop );
				string fName = $"{memName}<{GenericsDefWithSelfCsv}>";
				string constraints = $"where {TSelf} : {INameWithGenerics}";
				string parms = $"{TSelf} v";
				string body = $"v.{memName};";
				generics.Add( CodeGenerator.GetInheritdocString( $"{INameInheritDoc}.{memName}" ) );
				generics.Add( $"{access} {retType} {fName}({parms}) {constraints} => {body}" );
			}

			foreach( MethodInfo meth in MethodsITypeDef ) {
				CreateStaticAccessForMethod( meth, isGenericDef: true );
			}
		}


		static void CsvAppend( ref string s, string appendage ) {
			s = string.IsNullOrEmpty( s ) ? appendage : $"{s}, {appendage}";
		}

		string GetMethodParams( MethodInfo method ) {
			return string.Join( ", ", method.GetParameters().Select( p => p.ParameterType.Name + " " + p.Name ) );
		}

		public void GenerateSpecificsForType( Type vType, Type typeImpl ) {
			NumericTypeInfo info = StaticAccessExtensions.numericInfo[vType];
			bool addAsExtension = info.IsExternalType;
			string param0prefix = addAsExtension ? "this " : "";

			// /// <inheritdoc cref="INumber.isZero" />
			// public static bool isZero( rat v ) => v.isZero;
			string vTypeName = vType.Name;

			// Specific PROPERTIES
			foreach( PropertyInfo prop in typeImpl.GetProperties().Where( PropertyIsValid ) ) {
				string memName = prop.Name;
				string retTypeName = GetReturnTypeName( prop );
				string paramsInvoc = addAsExtension ? "()" : "";

				string paramName = "v";

				string body;
				if( addAsExtension ) {
					string strToFormat = StaticAccessExtensions.GetCustomImplementation( typeImpl, vType, memName );
					try {
						body = string.Format( strToFormat, paramName );
					} catch {
						body = "default";
						Debug.LogError( strToFormat );
					}
				} else {
					body = $"{paramName}.{memName}{paramsInvoc}";
				}

				specifics.Add( CodeGenerator.GetInheritdocString( $"{INameInheritDoc}.{memName}" ) );
				specifics.Add( $"public static {retTypeName} {memName}({param0prefix}{vTypeName} {paramName}) => {body};" );
			}

			// Specific METHODS
			foreach( MethodInfo meth in typeImpl.GetMethods().Where( MethodIsValid ) ) {
				CreateStaticAccessForMethod( meth, isGenericDef: false, vType, typeImpl );
			}
		}

		void CreateStaticAccessForMethod( MethodInfo meth, bool isGenericDef, Type numTypeSelf = null, Type iTypeImpl = null ) {
			// binary means we want to name the two input parameters (a,b)
			NumericTypeInfo info = numTypeSelf != null ? StaticAccessExtensions.numericInfo[numTypeSelf] : default;
			bool addAsExtension = numTypeSelf != null && info.IsExternalType;
			string param0prefix = addAsExtension ? "this " : "";
			bool isBinaryOp = meth.CustomAttributes.Any( x => x.AttributeType == typeof(BinaryOpAttribute) );
			string memName = meth.Name;
			string access = "public static";
			string retType = GetReturnTypeName( meth, isGenericDef );
			string fName = isGenericDef ? $"{memName}<{GenericsDefWithSelfCsv}>" : memName;
			string tSelf = isGenericDef ? TSelf : NameOf( numTypeSelf, isGenericDef );
			if( string.IsNullOrEmpty( tSelf ) )
				Debug.LogWarning( $"Empty param name: {numTypeSelf} in {meth.Name}" );
			string constraints = isGenericDef ? $" where {tSelf} : {INameWithGenerics}" : "";
			string selfParamName = isBinaryOp ? "a" : "v";
			string paramsBody = "";
			string paramsDecl = $"{param0prefix}{tSelf} {selfParamName}";
			List<string> paramNames = new();
			paramNames.Add( selfParamName );
			if( isBinaryOp ) { // f<T>(T a,T b) => a.f(b);
				string bTypeName = NameOf( meth.GetParameters().First().ParameterType, isGenericDef );
				if( string.IsNullOrEmpty( bTypeName ) )
					Debug.LogWarning( $"Empty param name: {meth.GetParameters().First()}/{meth.GetParameters().First().ParameterType.Name}/gen: {meth.GetParameters().First().ParameterType.IsGenericType} in meth {meth}" );
				string bName = "b";
				CsvAppend( ref paramsDecl, $"{bTypeName} {bName}" );
				CsvAppend( ref paramsBody, bName );
				paramNames.Add( bName );
			} else {
				foreach( ParameterInfo p in meth.GetParameters() ) {
					string paramTypeName = NameOf( p.ParameterType, isGenericDef );
					if( string.IsNullOrEmpty( paramTypeName ) )
						Debug.LogWarning( $"Empty param type name: {p.Name} in {meth.Name}" );
					CsvAppend( ref paramsDecl, paramTypeName + " " + p.Name );
					CsvAppend( ref paramsBody, p.Name );
					paramNames.Add( p.Name );
				}
			}
			string body;
			if( addAsExtension ) {
				body = StaticAccessExtensions.GetCustomImplementation( iTypeImpl, numTypeSelf, memName );
				if( paramNames.Count > 0 )
					body = string.Format( body, paramNames.ToArray() );
			} else
				body = $"{selfParamName}.{memName}({paramsBody})";
			List<string> targetList = isGenericDef ? generics : specifics;
			targetList.Add( CodeGenerator.GetInheritdocString( $"{INameInheritDoc}.{memName}" ) );
			targetList.Add( $"{access} {retType} {fName}({paramsDecl}){constraints} => {body};" );
		}

		string NameOf( Type type, bool isGenericDef ) => type.Name; // isGenericDef ? type.Name : type.FullName;

		string GetReturnTypeName( MethodInfo meth, bool isGenericDef ) {
			// if( meth.ReturnType.IsGenericParameter )
			return meth.ReturnType.Name;
			// return meth.ReturnType.FullName;
		}

		string GetReturnTypeName( PropertyInfo prop ) {
			Type type = prop.PropertyType;
			if( type.IsGenericType && type.GetGenericTypeDefinition() == typeof(ValueTuple<,>) ) {
				// type.custom
				Type[] args = type.GetGenericArguments();
				TupleElementNamesAttribute attr = prop.GetMethod.ReturnParameter.GetCustomAttribute<TupleElementNamesAttribute>();
				return "(" + string.Join( ",", args.Zip( attr.TransformNames, ( pType, label ) => label != null ? $"{pType} {label}" : pType.Name ) ) + ")";
			}

			return prop.PropertyType.Name;

			// Debug.Log( $"prop.PropertyType.FullName {prop.PropertyType.FullName}" );
			// Debug.Log( $"prop.GetMethod.ReturnType.FullName {prop.GetMethod.ReturnType.FullName}" );
			// if( prop.IsGenericMethod )
			// 	Debug.Log( $"prop.GetMethod.GetGenericMethodDefinition().ReturnType {prop.GetMethod.GetGenericMethodDefinition().ReturnType}" );


			// prop.GetMethod.gene

			// if(prop.typ)

			// return prop.GetMethod.IsGenericMethod ? prop.GetMethod.GetGenericMethodDefinition().ReturnType.FullName : type.FullName;
		}


	}

}