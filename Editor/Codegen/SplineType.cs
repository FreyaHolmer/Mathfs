namespace Freya {

	public class SplineType {
		public int degree;
		public string className;
		public string prettyName;
		public string prettyNameLower;
		public string[] paramNames;
		public string[] paramDescs;
		public string matrixName;
		public RationalMatrix4x4 charMatrix;

		public SplineType( int degree, string className, string prettyName, string matrixName, RationalMatrix4x4 charMatrix, string[] paramNames, string[] paramDescs, string[] paramDescsQuad = null ) {
			this.degree = degree;
			this.className = className;
			this.prettyName = prettyName;
			this.prettyNameLower = prettyName.ToLowerInvariant();
			this.paramDescs = paramDescs;
			this.matrixName = matrixName;
			this.paramNames = paramNames;
			this.charMatrix = charMatrix;
		}

		public void AppendParamStrings( CodeGenerator gen, int degree, int i ) {
			gen.Param( paramNames[i], paramDescs[i] );
		}
	}

}