using System;
using System.IO;
using System.Linq;

namespace Freya {

	public static class MatrixCodegen {
		public static void GenerateMatrices( string pathMatrices ) {
			for( int dim = 1; dim < 5; dim++ ) { // 1D, 2D, 3D, 4D
				GenerateMatrixNx1( pathMatrices, 3, GetVectorOfDim( dim ) );
				GenerateMatrixNx1( pathMatrices, 4, GetVectorOfDim( dim ) );
			}
			GenerateMatrixNx1( pathMatrices, 4, ElemType.Quat );
		}

		static void GenerateMatrixNx1( string path, int count, ElemType dim ) {
			const string vCompStr = "xyzw";
			const string vCompStrUp = "XYZW";
			int elemCompCount = ( (int)dim ).AtMost( 4 ); // quats also have 4
			int[] elemRange = Enumerable.Range( 0, count ).ToArray();
			int[] compRange = Enumerable.Range( 0, elemCompCount ).ToArray();
			string[] compRangeStr = compRange.Select( c => vCompStr[c].ToString() ).ToArray();
			string JoinRange( string separator, Func<int, string> elem ) => string.Join( separator, elemRange.Select( elem ) );
			string elemType = dim switch {
				ElemType._1D  => "float",
				ElemType.Quat => "Quaternion",
				_             => $"Vector{elemCompCount}"
			};
			string typePrefix = dim == ElemType._1D ? "" : elemType;
			string lerpName = GetLerpName( dim );
			string typeName = $"{typePrefix}Matrix{count}x1";
			string csParams = JoinRange( ", ", i => $"m{i}" );
			string csParamsThis = JoinRange( ", ", i => $"this.m{i}" );
			string ctorParams = JoinRange( ", ", i => $"{elemType} m{i}" );
			string indexerException = $"throw new IndexOutOfRangeException( $\"Matrix row index has to be from 0 to {count - 1}, got: {{row}}\" )";
			string indexerGetterCases = JoinRange( ", ", i => $"{i} => m{i}" ) + $", _ => {indexerException}";
			string equalsCompare = JoinRange( " && ", i => $"m{i}.Equals( other.m{i} )" );
			string equalsOpCompare = JoinRange( " && ", i => $"a.m{i} == b.m{i}" );
			string lerpAtoB = JoinRange( ", ", i => $"{lerpName}( a.m{i}, b.m{i}, t )" );
			bool isMultiComponentVector = dim != ElemType._1D && dim != ElemType.Quat;

			// generate content
			CodeGenerator code = new CodeGenerator();
			code.AppendHeader();
			code.AppendLine( "using System;" );
			if( dim != ElemType._1D ) // for Vector2/3
				code.AppendLine( "using UnityEngine;" );

			using( code.BracketScope( "namespace Freya" ) ) {
				code.Summary( $"A {count}x1 column matrix with {elemType} values" );
				using( code.BracketScope( $"[Serializable] public struct {typeName}" ) ) {
					// fields
					code.AppendLine( $"public {elemType} {csParams};" );

					// constructors
					code.AppendLine( $"public {typeName}({ctorParams}) => ({csParamsThis}) = ({csParams});" );
					if( isMultiComponentVector ) { // compose from float matrices
						string s = $"public {typeName}({string.Join( ", ", compRangeStr.Select( c => $"Matrix{count}x1 {c}" ) )}) => ";
						s += $"({csParams}) = ({JoinRange( ", ", i => $"new {elemType}({string.Join( ", ", compRangeStr.Select( c => $"{c}.m{i}" ) )})" )});";
						code.AppendLine( s );
					}

					// indexer
					using( code.BracketScope( $"public {elemType} this[int row]" ) ) {
						code.AppendLine( $"get => row switch{{{indexerGetterCases}}};" );
						using( code.BracketScope( "set" ) ) {
							using( code.BracketScope( "switch(row)" ) ) {
								code.AppendLine( JoinRange( " ", i => $"case {i}: m{i} = value; break;" ) );
								code.AppendLine( $"default: {indexerException};" );
							}
						}
					}

					// component extraction for vector-valued matrices
					if( isMultiComponentVector ) {
						for( int c = 0; c < elemCompCount; c++ ) {
							int cc = c;
							string parameters = JoinRange( ", ", i => $"m{i}.{vCompStr[cc]}" );
							code.AppendLine( $"public Matrix{count}x1 {vCompStrUp[c]} => new({parameters});" );
						}
					}

					// interpolation
					code.Summary( "Linearly interpolates between two matrices, based on a value <c>t</c>" );
					code.Param( "t", "The value to blend by" );
					string interpName = dim == ElemType.Quat ? "Slerp" : "Lerp";
					code.AppendLine( $"public static {typeName} {interpName}( {typeName} a, {typeName} b, float t ) => new {typeName}({lerpAtoB});" );

					// comparison/operators
					code.AppendLine( $"public static bool operator ==( {typeName} a, {typeName} b ) => {equalsOpCompare};" );
					code.AppendLine( $"public static bool operator !=( {typeName} a, {typeName} b ) => !( a == b );" );
					code.AppendLine( $"public bool Equals( {typeName} other ) => {equalsCompare};" );
					code.AppendLine( $"public override bool Equals( object obj ) => obj is {typeName} other && Equals( other );" );
					code.AppendLine( $"public override int GetHashCode() => HashCode.Combine( {csParams} );" );
					string stringPrint = JoinRange( "\\n", i => $"[{{m{i}}}]" );
					code.AppendLine( $"public override string ToString() => $\"{stringPrint}\";" );
				}
			}

			// save/finalize
			File.WriteAllLines( $"{path}/{typeName}.cs", code.content );
		}

		static string GetLerpName( ElemType dim ) {
			return dim switch {
				ElemType._1D  => "Mathfs.Lerp",
				ElemType._2D  => "Vector2.LerpUnclamped",
				ElemType._3D  => "Vector3.LerpUnclamped",
				ElemType._4D  => "Vector4.LerpUnclamped",
				ElemType.Quat => "Quaternion.SlerpUnclamped",
				_             => throw new IndexOutOfRangeException()
			};
		}

		public static ElemType GetVectorOfDim( int dim ) => (ElemType)dim;

	}

}