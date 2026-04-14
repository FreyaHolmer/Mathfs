using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Freya {

	public static class SplineCodegen {

		#region Type Definitions

		static SplineType typeBezier = new SplineType( 3, "Bezier", "Bézier", "cubicBezier", CharMatrix.cubicBezier,
			new[] { "p0", "p1", "p2", "p3" },
			new[] {
				"The starting point of the curve",
				"The second control point of the curve, sometimes called the start tangent point",
				"The third control point of the curve, sometimes called the end tangent point",
				"The end point of the curve"
			}
		);

		static SplineType typeBezierQuad = new SplineType( 2, "Bezier", "Bézier", "quadraticBezier", (RationalMatrix4x4)CharMatrix.quadraticBezier,
			new[] { "p0", "p1", "p2" },
			new[] {
				"The starting point of the curve",
				"The middle control point of the curve, sometimes called a tangent point",
				"The end point of the curve"
			}
		);

		static SplineType typeHermite = new SplineType( 3, "Hermite", "Hermite", "cubicHermite", CharMatrix.cubicHermite,
			new[] { "p0", "v0", "p1", "v1" },
			new[] {
				"The starting point of the curve",
				"The rate of change (velocity) at the start of the curve",
				"The end point of the curve",
				"The rate of change (velocity) at the end of the curve"
			}
		);

		static SplineType typeBspline = new SplineType( 3, "UBS", "B-Spline", "cubicUniformBspline", CharMatrix.cubicUniformBspline,
			new[] { "p0", "p1", "p2", "p3" },
			new[] {
				"The first point of the B-spline hull",
				"The second point of the B-spline hull",
				"The third point of the B-spline hull",
				"The fourth point of the B-spline hull"
			}
		);

		static SplineType typeCatRom = new SplineType( 3, "CatRom", "Catmull-Rom", "cubicCatmullRom", CharMatrix.cubicCatmullRom,
			new[] { "p0", "p1", "p2", "p3" },
			new[] {
				"The first control point of the catmull-rom curve. Note that this point is not included in the curve itself, and only helps to shape it",
				"The second control point, and the start of the catmull-rom curve",
				"The third control point, and the end of the catmull-rom curve",
				"The last control point of the catmull-rom curve. Note that this point is not included in the curve itself, and only helps to shape it"
			}
		);

		static SplineType[] allSplineTypes = { typeBezier, typeBezierQuad, typeHermite, typeBspline, typeCatRom };

		#endregion

		public static void GenerateUniformSplines( string splinesPath ) {
			string pathUniformSplines = $"{splinesPath}/Uniform Spline Segments";
			for( int dim = 1; dim < 5; dim++ ) { // 1D, 2D, 3D, 4D
				GenerateUniformSplineType( pathUniformSplines, typeBezier, dim );
				GenerateUniformSplineType( pathUniformSplines, typeBezierQuad, dim );
				GenerateUniformSplineType( pathUniformSplines, typeHermite, dim );
				GenerateUniformSplineType( pathUniformSplines, typeBspline, dim );
				GenerateUniformSplineType( pathUniformSplines, typeCatRom, dim );
			}
		}

		// [MenuItem( "Assets/Port Spline Data" )]
		public static void PortSplineData() {
			string replacements = "";
			int replacementCount = 0;
			GameObject[] gos = SceneManager.GetActiveScene().GetRootGameObjects();
			foreach( GameObject go in gos ) {
				foreach( Component c in go.GetComponentsInChildren<Component>( true ) ) {
					SerializedObject so = new SerializedObject( c ); // can actually find null components??
					so.Update();
					bool madeChanges = false;
					SerializedProperty prop = so.GetIterator();
					while( prop.Next( true ) ) {
						if( prop.isArray == false && IsSplineType( prop.type, out SplineType type, out int dim ) ) {
							SerializedProperty ptMtx = prop.FindPropertyRelative( "pointMatrix" );
							try {
								if( dim == 1 )
									for( int i = 0; i < type.paramNames.Length; i++ )
										ptMtx.FindPropertyRelative( $"m{i}" ).floatValue = prop.FindPropertyRelative( type.paramNames[i] ).floatValue;
								else if( dim == 2 )
									for( int i = 0; i < type.paramNames.Length; i++ )
										ptMtx.FindPropertyRelative( $"m{i}" ).vector2Value = prop.FindPropertyRelative( type.paramNames[i] ).vector2Value;
								else if( dim == 3 )
									for( int i = 0; i < type.paramNames.Length; i++ )
										ptMtx.FindPropertyRelative( $"m{i}" ).vector3Value = prop.FindPropertyRelative( type.paramNames[i] ).vector3Value;
								else if( dim == 4 )
									for( int i = 0; i < type.paramNames.Length; i++ )
										ptMtx.FindPropertyRelative( $"m{i}" ).vector4Value = prop.FindPropertyRelative( type.paramNames[i] ).vector4Value;
							} catch {
								Debug.LogError( $"Null thing in {go.name}/{c.GetType().Name}/{prop.propertyPath} of type {type.className} mtx: {type.matrixName}" );
							}

							madeChanges = true;
							replacements += $"Replaced: {go.name}/{c.GetType().Name}: {prop.displayName}\n";
							replacementCount++;
						}
					}

					if( madeChanges )
						so.ApplyModifiedProperties();
				}
			}

			Debug.Log( $"{replacementCount} replacements:\n{replacements}" );
		}

		static bool IsSplineType( string name, out SplineType type, out int dim ) {
			foreach( SplineType spline in allSplineTypes ) {
				for( int d = 1; d <= 3; d++ ) {
					if( name == $"{spline.className}{GetDegreeName( spline.degree, true )}{d}D" ) {
						dim = d;
						type = spline;
						return true;
					}
				}
			}

			( type, dim ) = ( default, default );
			return false;
		}


		static void GenerateUniformSplineType( string uniformSplinePath, SplineType type, int dim ) {
			int degree = type.degree;
			string dataType = dim == 1 ? "float" : $"Vector{dim}";
			string polynomType = dim == 1 ? "Polynomial" : $"Polynomial{dim}D";
			int ptCount = degree + 1;
			string degFullLower = GetDegreeName( degree, false );
			string degShortCapital = GetDegreeName( degree, true );
			string structName = $"{type.className}{degShortCapital}{dim}D";
			string[] points = type.paramNames;
			int[] ptRange = Enumerable.Range( 0, ptCount ).ToArray();
			string[] pointDescs = type.paramDescs;
			string lerpName = GetLerpName( (ElemType)dim );
			string pointMatrixType = $"{( dim == 1 ? "" : dataType )}Matrix{ptCount}x1";

			string JoinRange( string separator, Func<int, string> elem ) => string.Join( separator, ptRange.Select( elem ) );
			string JoinRangeStr( string separator, Func<string, string> elem ) => string.Join( separator, points.Select( elem ) );

			string ctorParams = JoinRange( ", ", i => $"{dataType} {type.paramNames[i]}" );
			string csPoints = JoinRange( ", ", i => $"{type.paramNames[i]}" );

			CodeGenerator code = new CodeGenerator();
			code.AppendHeader();
			code.Using( "System" );
			code.Using( "System.Runtime.CompilerServices" );
			code.Using( "UnityEngine" );
			code.LineBreak();

			using( code.BracketScope( "namespace Freya" ) ) {
				code.LineBreak();

				// type definition
				code.Summary( $"An optimized uniform {dim}D {degFullLower} {type.prettyNameLower} segment, with {ptCount} control points" );
				using( code.BracketScope( $"[Serializable] public struct {structName} : IParamSplineSegment<{polynomType},{pointMatrixType}>" ) ) { // intentionally always Cubic right now
					code.LineBreak();
					code.AppendLine( "const MethodImplOptions INLINE = MethodImplOptions.AggressiveInlining;" );
					code.LineBreak();

					// fields
					code.AppendLine( $"[SerializeField] {pointMatrixType} pointMatrix;" );
					code.AppendLine( $"[NonSerialized] {polynomType} curve;" );
					code.AppendLine( "[NonSerialized] bool validCoefficients;" );
					code.LineBreak();

					// constructors
					string ctorSummary = $"Creates a uniform {dim}D {degFullLower} {type.prettyNameLower} segment, from {ptCount} control points";
					code.Summary( ctorSummary );
					for( int i = 0; i < ptCount; i++ )
						type.AppendParamStrings( code, degree, i );
					code.AppendLine( $"public {structName}( {ctorParams} ) : this(new {pointMatrixType}({csPoints})){{}}" );

					code.Summary( ctorSummary );
					code.Param( "pointMatrix", "The matrix containing the control points of this spline" );
					code.AppendLine( $"public {structName}( {pointMatrixType} pointMatrix ) => (this.pointMatrix,curve,validCoefficients) = (pointMatrix,default,false);" );

					code.LineBreak();

					// properties
					using( code.BracketScope( $"public {polynomType} Curve" ) ) {
						using( code.BracketScope( $"get" ) ) {
							using( code.Scope( "if( validCoefficients )" ) )
								code.AppendLine( "return curve; // no need to update" );
							code.AppendLine( "validCoefficients = true;" );
							using( code.Scope( $"return curve = new {polynomType}(" ) ) {
								for( int icRow = 0; icRow < ptCount; icRow++ ) {
									MathSum sum = new MathSum();
									for( int ip = 0; ip < ptCount; ip++ )
										sum.AddTerm( type.charMatrix[icRow, ip], $"{type.paramNames[ip].ToUpperInvariant()}" );
									code.AppendLine( $"{sum}{( icRow < ptCount - 1 ? "," : "" )}" );
								}
							}

							code.AppendLine( ");" );
						}
						// todo: set would be possible! setting the points based on a curve
					}

					code.AppendLine( $"public {pointMatrixType} PointMatrix {{[MethodImpl( INLINE )] get => pointMatrix; [MethodImpl( INLINE )] set => _ = ( pointMatrix = value, validCoefficients = false ); }}" );
					for( int i = 0; i < ptCount; i++ ) {
						code.Summary( pointDescs[i] );
						code.AppendLine( $"public {dataType} {points[i].ToUpperInvariant()}{{ [MethodImpl( INLINE )] get => pointMatrix.m{i}; [MethodImpl( INLINE )] set => _ = ( pointMatrix.m{i} = value, validCoefficients = false ); }}" );
					}

					code.Summary( $"Get or set a control point position by index. Valid indices from 0 to {degree}" );
					using( code.BracketScope( $"public {dataType} this[ int i ]" ) ) {
						string indexException = $"throw new ArgumentOutOfRangeException( nameof(i), $\"Index has to be in the 0 to {degree} range, and I think {{i}} is outside that range you know\" )";
						code.AppendLine( $"get => i switch {{ {JoinRange( ", ", i => $"{i} => {points[i].ToUpperInvariant()}" )}, _ => {indexException} }};" );
						code.AppendLine( $"set {{ switch( i ){{ {JoinRange( " ", i => $"case {i}: {points[i].ToUpperInvariant()} = value; break;" )} default: {indexException}; }}}}" );
					}

					// equality checks
					string compEquals = JoinRangeStr( " && ", p => $"{p.ToUpperInvariant()}.Equals( other.{p.ToUpperInvariant()} )" );
					string toStringParams = JoinRange( ", ", i => $"{{pointMatrix.m{i}}}" );
					code.AppendLine( $"public static bool operator ==( {structName} a, {structName} b ) => a.pointMatrix == b.pointMatrix;" );
					code.AppendLine( $"public static bool operator !=( {structName} a, {structName} b ) => !( a == b );" );
					code.AppendLine( $"public bool Equals( {structName} other ) => {compEquals};" );
					code.AppendLine( $"public override bool Equals( object obj ) => obj is {structName} other && pointMatrix.Equals( other.pointMatrix );" );
					code.AppendLine( $"public override int GetHashCode() => pointMatrix.GetHashCode();" );
					code.AppendLine( $"public override string ToString() => $\"({toStringParams})\";" );
					code.LineBreak();

					// typecasting
					if( dim is 2 or 3 && degree is 3 ) {
						if( dim == 2 ) {
							// Typecast to 3D where z = 0
							string structName3D = $"{type.className}{degShortCapital}3D";
							code.Summary( "Returns this spline segment in 3D, where z = 0" );
							code.Param( "curve2D", "The 2D curve to cast to 3D" );
							string inParams = JoinRangeStr( ", ", p => $"curve2D.{p.ToUpperInvariant()}" );
							code.AppendLine( $"public static explicit operator {structName3D}( {structName} curve2D ) => new {structName3D}( {inParams} );" );
						}

						if( dim == 3 ) {
							// typecast to 2D where z is omitted
							string structName2D = $"{type.className}{degShortCapital}2D";
							code.Summary( "Returns this curve flattened to 2D. Effectively setting z = 0" );
							code.Param( "curve3D", "The 3D curve to flatten to the Z plane" );
							string inParams = JoinRangeStr( ", ", p => $"curve3D.{p.ToUpperInvariant()}" );
							code.AppendLine( $"public static explicit operator {structName2D}( {structName} curve3D ) => new {structName2D}( {inParams} );" );
						}
					}

					// converting between spline types
					if( degree == 3 ) {
						string[] cubicSplineTypeNames = {
							nameof(BezierCubic1D).Replace( "1D", $"{dim}D" ),
							nameof(HermiteCubic1D).Replace( "1D", $"{dim}D" ),
							nameof(CatRomCubic1D).Replace( "1D", $"{dim}D" ),
							nameof(UBSCubic1D).Replace( "1D", $"{dim}D" )
						};
						RationalMatrix4x4[] typeMatrices = {
							CharMatrix.cubicBezier,
							CharMatrix.cubicHermite,
							CharMatrix.cubicCatmullRom,
							CharMatrix.cubicUniformBspline
						};

						// Conversion to other cubic splines
						for( int i = 0; i < 4; i++ ) {
							string targetType = cubicSplineTypeNames[i];
							if( targetType == structName )
								continue; // don't convert to self
							RationalMatrix4x4 C = CharMatrix.GetConversionMatrix( type.charMatrix, typeMatrices[i] );

							using( code.Scope( $"public static explicit operator {targetType}( {structName} s ) =>" ) ) {
								using( code.Scope( $"new {targetType}(" ) ) {
									for( int oPt = 0; oPt < 4; oPt++ ) {
										MathSum sum = new();
										for( int iPt = 0; iPt < 4; iPt++ )
											sum.AddTerm( C[oPt, iPt], $"s.{type.paramNames[iPt].ToUpperInvariant()}" );
										code.AppendLine( $"{sum}{( oPt < 3 ? "," : "" )}" );
									}
								}

								code.AppendLine( ");" );
							}
						}
					}

					// Interpolation
					code.Summary( $"Returns a linear blend between two {type.prettyNameLower} curves" );
					code.Param( "a", "The first spline segment" );
					code.Param( "b", "The second spline segment" );
					code.Param( "t", "A value from 0 to 1 to blend between <c>a</c> and <c>b</c>" );
					using( code.Scope( $"public static {structName} Lerp( {structName} a, {structName} b, float t ) =>" ) ) {
						using( code.Scope( "new(" ) ) {
							for( int i = 0; i < ptCount; i++ ) {
								code.AppendLine( $"{lerpName}( a.{points[i].ToUpperInvariant()}, b.{points[i].ToUpperInvariant()}, t )" + ( i == ptCount - 1 ? "" : "," ) );
							}
						}

						code.AppendLine( ");" );
					}


					// special case slerps for cubic beziers in 2D and 3D
					if( dim is 2 or 3 && type == typeBezier ) {
						// todo: hermite slerp
						string slerpCast = dim == 2 ? "(Vector2)" : "";
						code.LineBreak();
						code.Summary( $"Returns a linear blend between two {type.prettyNameLower} curves, where the tangent directions are spherically interpolated" );
						code.Param( "a", "The first spline segment" );
						code.Param( "b", "The second spline segment" );
						code.Param( "t", "A value from 0 to 1 to blend between <c>a</c> and <c>b</c>" );
						using( code.BracketScope( $"public static {structName} Slerp( {structName} a, {structName} b, float t )" ) ) {
							code.AppendLine( $"{dataType} P0 = {lerpName}( a.P0, b.P0, t );" );
							code.AppendLine( $"{dataType} P3 = {lerpName}( a.P3, b.P3, t );" );
							using( code.Scope( $"return new {structName}(" ) ) {
								code.AppendLine( $"P0," );
								code.AppendLine( $"P0 + {slerpCast}Vector3.SlerpUnclamped( a.P1 - a.P0, b.P1 - b.P0, t )," );
								code.AppendLine( $"P3 + {slerpCast}Vector3.SlerpUnclamped( a.P2 - a.P3, b.P2 - b.P3, t )," );
								code.AppendLine( $"P3" );
							}

							code.AppendLine( ");" );
						}
					}

					// special case splits
					bool hasSplit = type == typeBezier || type == typeBezierQuad;
					if( hasSplit ) {
						code.Summary( "Splits this curve at the given t-value, into two curves that together form the exact same shape" );
						code.Param( "t", "The t-value to split at" );
						using( code.BracketScope( $"public ({structName} pre, {structName} post) Split( float t )" ) ) {
							AppendBezierSplit( code, structName, dataType, degree, dim );
						}
					}
				}
			}

			File.WriteAllLines( $"{uniformSplinePath}/{structName}.cs", code.content );
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

		class MathSum {

			rat globalScale = rat.one;
			List<(rat coeff, string var)> terms = new List<(rat coeff, string var)>();

			public void AddTerm( rat coeff, string var ) {
				if( coeff != 0 )
					terms.Add( ( coeff, var ) );
			}

			void TryOptimize() {
				if( terms.Count < 2 )
					return; // can't optimize 0 or 1 terms

				rat coeff0 = terms[0].coeff.abs;
				if( terms.TrueForAll( t => t.coeff.abs == coeff0 ) ) {
					globalScale = coeff0;
					for( int i = 0; i < terms.Count; i++ )
						terms[i] = ( terms[i].coeff / coeff0, terms[i].var );
				}
			}

			public override string ToString() {
				if( terms.Count == 0 )
					return "0";

				TryOptimize();

				string line = "";
				for( int i = 0; i < terms.Count; i++ )
					line += FormatTerm( i );

				if( globalScale != 1 ) {
					if( globalScale.n == 1 ) {
						line = $"({line})/{globalScale.d}";
					} else {
						line = $"{FormatRational( globalScale )}*({line})";
					}
				}

				return line;
			}

			string FormatRational( rat v ) => v.isInteger ? $"{v.n}" : $"({v}f)";

			string FormatTerm( int i ) {
				rat value = terms[i].coeff;
				string sign = i > 0 && value >= 0 ? "+" : "";
				string valueStr;
				string op = "";
				if( value == rat.one )
					valueStr = "";
				else if( value == -rat.one )
					valueStr = "-";
				else if( value > 0 ) {
					valueStr = FormatRational( value );
					op = "*";
				} else { // value < 0
					valueStr = FormatRational( -value );
					sign = "-";
					op = "*";
				}

				return $"{sign}{valueStr}{op}{terms[i].var}";
			}

		}

		static readonly string[] comp = { "x", "y", "z", "w" };

		static void AppendBezierSplit( CodeGenerator code, string structName, string dataType, int degree, int dim ) {
			string LerpStr( string A, string B, int c ) => $"{A}.{comp[c]} + ( {B}.{comp[c]} - {A}.{comp[c]} ) * t";

			void AppendLerps( string varName, string A, string B ) {
				if( dim > 1 ) {
					using( code.Scope( $"{dataType} {varName} = new {dataType}(" ) ) {
						for( int c = 0; c < dim; c++ ) {
							string end = c == dim - 1 ? " );" : ",";
							code.AppendLine( $"{LerpStr( A, B, c )}{end}" );
						}
					}
				} else { // floats
					code.AppendLine( $"{dataType} {varName} = {A} + ( {B} - {A} ) * t;" );
				}
			}

			AppendLerps( "a", "P0", "P1" );
			AppendLerps( "b", "P1", "P2" ); // this could be unrolled/optimized for the cubic case, as b is never used for the output
			if( degree == 3 ) {
				AppendLerps( "c", "P2", "P3" );
				AppendLerps( "d", "a", "b" );
				AppendLerps( "e", "b", "c" );
				AppendLerps( "p", "d", "e" );
				code.AppendLine( $"return ( new {structName}( P0, a, d, p ), new {structName}( p, e, c, P3 ) );" );
			} else if( degree == 2 ) {
				AppendLerps( "p", "a", "b" );
				code.AppendLine( $"return ( new {structName}( P0, a, p ), new {structName}( p, b, P2 ) );" );
			}
		}

		static string GetDegreeName( int d, bool shortName ) {
			return d switch {
				1 => "Linear",
				2 => shortName ? "Quad" : "Quadratic",
				3 => "Cubic",
				4 => "Quartic",
				5 => "Quintic",
				_ => throw new IndexOutOfRangeException()
			};
		}


	}

}