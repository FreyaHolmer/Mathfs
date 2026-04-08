// by Freya Holmér (https://github.com/FreyaHolmer/Mathfs)

using System.IO;
using UnityEditor;

namespace Freya {

	public static class MathfsCodegen {

		static GUID guidRuntimeAsm = new("6071c9f2ce0a4407c93af459fa416e54"); // Mathfs.asmdef
		static string PathRuntime => Path.GetDirectoryName( AssetDatabase.GUIDToAssetPath( guidRuntimeAsm ) );
		public static string PathSpline => $"{PathRuntime}/Splines";
		public static string PathNumerics => $"{PathRuntime}/Numerics";
		public static string PathMatrices => PathNumerics;

		[MenuItem( "Assets/Run Mathfs Codegen" )]
		public static void Regenerate() {
			SplineCodegen.GenerateUniformSplines( PathSpline );
			MatrixCodegen.GenerateMatrices( PathMatrices );
		}

	}

}