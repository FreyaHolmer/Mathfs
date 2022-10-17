using System.Runtime.CompilerServices;
using UnityEngine;
using static Freya.Mathfs;

namespace Freya {

	/// <summary>An interface representing a parametric curve</summary>
	/// <typeparam name="V">The vector type of the curve</typeparam>
	public interface IParamCurve<out V> where V : struct {

		/// <summary>Returns the degree of this curve. Quadratic = 2, Cubic = 3, etc</summary>
		int Degree { get; }

		/// <summary>The number of control points in this curve</summary>
		int Count { get; }

		/// <summary>Returns the point at the given t-value on the curve</summary>
		/// <param name="t">The t-value along the curve to sample</param>
		V GetPoint( float t );

		/// <summary>Returns the starting point of this curve, where t = 0</summary>
		V GetStartPoint();

		/// <summary>Returns the end point of this curve, where t = 1</summary>
		V GetEndPoint();

	}

	/// <summary>An interface representing a parametric curve of degree 1 or higher</summary>
	/// <typeparam name="V">The vector type of the curve</typeparam>
	public interface IParamCurve1Diff<V> : IParamCurve<V> where V : struct {
		/// <summary>Returns the derivative at the given t-value on the curve. Loosely analogous to "velocity" of the point along the curve</summary>
		/// <param name="t">The t-value along the curve to sample</param>
		V GetDerivative( float t );
	}

	/// <summary>An interface representing a parametric curve of degree 2 or higher</summary>
	/// <typeparam name="V">The vector type of the curve</typeparam>
	public interface IParamCurve2Diff<V> : IParamCurve1Diff<V> where V : struct {
		/// <summary>Returns the second derivative at the given t-value on the curve. Loosely analogous to "acceleration" of the point along the curve</summary>
		/// <param name="t">The t-value along the curve to sample</param>
		V GetSecondDerivative( float t );
	}

	/// <summary>An interface representing a parametric curve of degree 3 or higher</summary>
	/// <typeparam name="V">The vector type of the curve</typeparam>
	public interface IParamCurve3Diff<V> : IParamCurve2Diff<V> where V : struct {
		/// <summary>Returns the third derivative of the curve. Loosely analogous to "jerk/jolt" (rate of change of acceleration) of the point along the curve</summary>
		V GetThirdDerivative( float t );
	}

	/// <summary>Shared functionality for all 2D parametric curves</summary>
	public static class IParamCurveExt2D {

		const MethodImplOptions INLINE = MethodImplOptions.AggressiveInlining;

		/// <summary>Returns the approximate length of the curve</summary>
		/// <param name="accuracy">The number of subdivisions to approximate the length with. Higher values are more accurate, but more expensive to calculate</param>
		public static float GetLength<T>( this T curve, int accuracy = 8 ) where T : IParamCurve<Vector2> {
			if( accuracy <= 2 )
				return ( curve.GetStartPoint() - curve.GetEndPoint() ).magnitude;

			float totalDist = 0;
			Vector2 prev = curve.GetStartPoint();
			for( int i = 1; i < accuracy; i++ ) {
				float t = i / ( accuracy - 1f );
				Vector2 p = curve.GetPoint( t );
				float dx = p.x - prev.x;
				float dy = p.y - prev.y;
				totalDist += Mathf.Sqrt( dx * dx + dy * dy );
				prev = p;
			}

			return totalDist;
		}

	}

	/// <summary>Shared functionality for all 3D parametric curves</summary>
	public static class IParamCurveExt3D {

		const MethodImplOptions INLINE = MethodImplOptions.AggressiveInlining;

		/// <summary>Returns the approximate length of the curve</summary>
		/// <param name="accuracy">The number of subdivisions to approximate the length with. Higher values are more accurate, but more expensive to calculate</param>
		public static float GetLength<T>( this T curve, int accuracy = 8 ) where T : IParamCurve<Vector3> {
			if( accuracy <= 2 )
				return ( curve.GetStartPoint() - curve.GetEndPoint() ).magnitude;

			float totalDist = 0;
			Vector3 prev = curve.GetStartPoint();
			for( int i = 1; i < accuracy; i++ ) {
				float t = i / ( accuracy - 1f );
				Vector3 p = curve.GetPoint( t );
				float dx = p.x - prev.x;
				float dy = p.y - prev.y;
				totalDist += Mathf.Sqrt( dx * dx + dy * dy );
				prev = p;
			}

			return totalDist;
		}

	}

	/// <summary>Shared functionality for 2D parametric curves of degree 1 or higher</summary>
	public static class IParamCurve1DiffExt2D {

		const MethodImplOptions INLINE = MethodImplOptions.AggressiveInlining;

		/// <summary>Returns the normalized tangent direction at the given t-value on the curve</summary>
		/// <param name="t">The t-value along the curve to sample</param>
		[MethodImpl( INLINE )] public static Vector2 GetTangent<T>( this T curve, float t ) where T : IParamCurve1Diff<Vector2> => curve.GetDerivative( t ).normalized;

		/// <summary>Returns the normal direction at the given t-value on the curve.
		/// This normal will point to the inner arc of the current curvature</summary>
		/// <param name="t">The t-value along the curve to sample</param>
		[MethodImpl( INLINE )] public static Vector2 GetNormal<T>( this T curve, float t ) where T : IParamCurve1Diff<Vector2> => curve.GetTangent( t ).Rotate90CCW();

		/// <summary>Returns the 2D angle of the direction of the curve at the given point, in radians</summary>
		/// <param name="t">The t-value along the curve to sample</param>
		[MethodImpl( INLINE )] public static float GetAngle<T>( this T curve, float t ) where T : IParamCurve1Diff<Vector2> => DirToAng( curve.GetDerivative( t ) );

		/// <summary>Returns the orientation at the given point t, where the X axis is tangent to the curve</summary>
		/// <param name="t">The t-value along the curve to sample</param>
		[MethodImpl( INLINE )] public static Quaternion GetOrientation<T>( this T curve, float t ) where T : IParamCurve1Diff<Vector2> => DirToOrientation( curve.GetDerivative( t ) );

		/// <summary>Returns the position and orientation at the given t-value on the curve</summary>
		/// <param name="t">The t-value along the curve to sample</param>
		[MethodImpl( INLINE )] public static Pose GetPose<T>( this T curve, float t ) where T : IParamCurve1Diff<Vector2> => PointDirToPose( curve.GetPoint( t ), curve.GetTangent( t ) );

		/// <summary>Returns the position and orientation at the given t-value on the curve, expressed as a matrix</summary>
		/// <param name="t">The t-value along the curve to sample</param>
		[MethodImpl( INLINE )] public static Matrix4x4 GetMatrix<T>( this T curve, float t ) where T : IParamCurve1Diff<Vector2> => GetMatrixFrom2DPointDir( curve.GetPoint( t ), curve.GetTangent( t ) );

	}

	/// <summary>Shared functionality for 3D parametric curves of degree 1 or higher</summary>
	public static class IParamCurve1DiffExt3D {
		const MethodImplOptions INLINE = MethodImplOptions.AggressiveInlining;

		/// <inheritdoc cref="IParamCurve1DiffExt2D.GetTangent{T}(T,float)"/>
		[MethodImpl( INLINE )] public static Vector3 GetTangent<T>( this T curve, float t ) where T : IParamCurve1Diff<Vector3> => curve.GetDerivative( t ).normalized;

		/// <summary>Returns a normal of the curve given a reference up vector and t-value on the curve.
		/// The normal will be perpendicular to both the supplied up vector and the curve</summary>
		/// <param name="t">The t-value along the curve to sample</param>
		/// <param name="up">The reference up vector. The normal will be perpendicular to both the supplied up vector and the curve</param>
		[MethodImpl( INLINE )] public static Vector3 GetNormal<T>( this T curve, float t, Vector3 up ) where T : IParamCurve1Diff<Vector3> => GetNormalFromLookTangent( curve.GetDerivative( t ), up );

		/// <summary>Returns the binormal of the curve given a reference up vector and t-value on the curve.
		/// The binormal will attempt to be as aligned with the reference vector as possible,
		/// while still being perpendicular to the curve</summary>
		/// <param name="t">The t-value along the curve to sample</param>
		/// <param name="up">The reference up vector. The binormal will attempt to be as aligned with the reference vector as possible, while still being perpendicular to the curve</param>
		[MethodImpl( INLINE )] public static Vector3 GetBinormal<T>( this T curve, float t, Vector3 up ) where T : IParamCurve1Diff<Vector3> => GetBinormalFromLookTangent( curve.GetDerivative( t ), up );

		/// <summary>Returns the orientation at the given point t, where the Z direction is tangent to the curve.
		/// The Y axis will attempt to align with the supplied up vector</summary>
		/// <param name="t">The t-value along the curve to sample</param>
		/// <param name="up">The reference up vector. The Y axis will attempt to be as aligned with this vector as much as possible</param>
		[MethodImpl( INLINE )] public static Quaternion GetOrientation<T>( this T curve, float t, Vector3 up ) where T : IParamCurve1Diff<Vector3> => Quaternion.LookRotation( curve.GetDerivative( t ), up );

		/// <summary>Returns the position and orientation of curve at the given point t, where the Z direction is tangent to the curve.
		/// The Y axis will attempt to align with the supplied up vector</summary>
		/// <param name="t">The t-value along the curve to sample</param>
		/// <param name="up">The reference up vector. The Y axis will attempt to be as aligned with this vector as much as possible</param>
		[MethodImpl( INLINE )] public static Pose GetPose<T>( this T curve, float t, Vector3 up ) where T : IParamCurve1Diff<Vector3> => new Pose( curve.GetPoint( t ), Quaternion.LookRotation( curve.GetDerivative( t ), up ) );

		/// <summary>Returns the position and orientation of curve at the given point t, expressed as a matrix, where the Z direction is tangent to the curve.
		/// The Y axis will attempt to align with the supplied up vector</summary>
		/// <param name="t">The t-value along the curve to sample</param>
		/// <param name="up">The reference up vector. The Y axis will attempt to be as aligned with this vector as much as possible</param>
		public static Matrix4x4 GetMatrix<T>( this T curve, float t, Vector3 up ) where T : IParamCurve1Diff<Vector3> {
			( Vector3 Pt, Vector3 Tn ) = ( curve.GetPoint( t ), curve.GetTangent( t ) );
			Vector3 Nm = Vector3.Cross( up, Tn ).normalized; // X axis
			Vector3 Bn = Vector3.Cross( Tn, Nm ); // Y axis
			return new Matrix4x4(
				new Vector4( Nm.x, Nm.y, Nm.z, 0 ),
				new Vector4( Bn.x, Bn.y, Bn.z, 0 ),
				new Vector4( Tn.x, Tn.y, Tn.z, 0 ),
				new Vector4( Pt.x, Pt.y, Pt.z, 1 )
			);
		}
	}

	/// <summary>Shared functionality for 2D parametric curves of degree 2 or higher</summary>
	public static class IParamCurve2DiffExt2D {

		const MethodImplOptions INLINE = MethodImplOptions.AggressiveInlining;

		/// <summary>Returns the signed curvature at the given t-value on the curve, in radians per distance unit (equivalent to the reciprocal radius of the osculating circle)</summary>
		/// <param name="t">The t-value along the curve to sample</param>
		[MethodImpl( INLINE )] public static float GetCurvature<T>( this T curve, float t ) where T : IParamCurve2Diff<Vector2> => Mathfs.GetCurvature( curve.GetDerivative( t ), curve.GetSecondDerivative( t ) );

		/// <summary>Returns the osculating circle at the given t-value in the curve, if possible. Osculating circles are defined everywhere except on inflection points, where curvature is 0</summary>
		/// <param name="t">The t-value along the curve to sample</param>
		[MethodImpl( INLINE )] public static Circle2D GetOsculatingCircle<T>( this T curve, float t ) where T : IParamCurve2Diff<Vector2> => Circle2D.GetOsculatingCircle( curve.GetPoint( t ), curve.GetDerivative( t ), curve.GetSecondDerivative( t ) );

	}

	/// <summary>Shared functionality for 3D parametric curves of degree 2 or higher</summary>
	public static class IParamCurve2DiffExt3D {
		const MethodImplOptions INLINE = MethodImplOptions.AggressiveInlining;

		/// <summary>Returns a pseudovector at the given t-value on the curve, where the magnitude is the curvature in radians per distance unit, and the direction is the axis of curvature</summary>
		/// <param name="t">The t-value along the curve to sample</param>
		[MethodImpl( INLINE )] public static Vector3 GetCurvature<T>( this T curve, float t ) where T : IParamCurve2Diff<Vector3> => Mathfs.GetCurvature( curve.GetDerivative( t ), curve.GetSecondDerivative( t ) );

		/// <inheritdoc cref="IParamCurve2DiffExt2D.GetOsculatingCircle{T}(T,float)"/>
		[MethodImpl( INLINE )] public static Circle3D GetOsculatingCircle<T>( this T curve, float t ) where T : IParamCurve2Diff<Vector3> => Circle3D.GetOsculatingCircle( curve.GetPoint( t ), curve.GetDerivative( t ), curve.GetSecondDerivative( t ) );

		/// <summary>Returns the frenet-serret (curvature-based) normal direction at the given t-value on the curve</summary>
		/// <param name="t">The t-value along the curve to sample</param>
		[MethodImpl( INLINE )] public static Vector3 GetArcNormal<T>( this T curve, float t ) where T : IParamCurve2Diff<Vector3> => Mathfs.GetArcNormal( curve.GetDerivative( t ), curve.GetSecondDerivative( t ) );

		/// <summary>Returns the frenet-serret (curvature-based) binormal direction at the given t-value on the curve</summary>
		/// <param name="t">The t-value along the curve to sample</param>
		[MethodImpl( INLINE )] public static Vector3 GetArcBinormal<T>( this T curve, float t ) where T : IParamCurve2Diff<Vector3> => Mathfs.GetArcBinormal( curve.GetDerivative( t ), curve.GetSecondDerivative( t ) );

		/// <summary>Returns the frenet-serret (curvature-based) orientation of curve at the given point t, where the Z direction is tangent to the curve.
		/// The X axis will point to the inner arc of the current curvature</summary>
		/// <param name="t">The t-value along the curve to sample</param>
		[MethodImpl( INLINE )] public static Quaternion GetArcOrientation<T>( this T curve, float t ) where T : IParamCurve2Diff<Vector3> => Mathfs.GetArcOrientation( curve.GetDerivative( t ), curve.GetSecondDerivative( t ) );

		/// <summary>Returns the position and the frenet-serret (curvature-based) orientation of curve at the given point t, where the Z direction is tangent to the curve.
		/// The X axis will point to the inner arc of the current curvature</summary>
		/// <param name="t">The t-value along the curve to sample</param>
		[MethodImpl( INLINE )] public static Pose GetArcPose<T>( this T curve, float t ) where T : IParamCurve2Diff<Vector3> {
			( Vector3 pt, Vector3 vel, Vector3 acc ) = ( curve.GetPoint( t ), curve.GetDerivative( t ), curve.GetSecondDerivative( t ) );
			Vector3 binormal = Vector3.Cross( vel, acc );
			return new Pose( pt, Quaternion.LookRotation( vel, binormal ) );
		}

		/// <summary>Returns the position and the frenet-serret (curvature-based) orientation of curve at the given point t, expressed as a matrix, where the Z direction is tangent to the curve.
		/// The X axis will point to the inner arc of the current curvature</summary>
		/// <param name="t">The t-value along the curve to sample</param>
		public static Matrix4x4 GetArcMatrix<T>( this T curve, float t ) where T : IParamCurve2Diff<Vector3> {
			Vector3 P = curve.GetPoint( t );
			Vector3 vel = curve.GetDerivative( t );
			Vector3 acc = curve.GetSecondDerivative( t );
			Vector3 Tn = vel.normalized;
			Vector3 B = Vector3.Cross( vel, acc ).normalized;
			Vector3 N = Vector3.Cross( B, Tn );
			return new Matrix4x4(
				new Vector4( N.x, N.y, N.z, 0 ),
				new Vector4( B.x, B.y, B.z, 0 ),
				new Vector4( Tn.x, Tn.y, Tn.z, 0 ),
				new Vector4( P.x, P.y, P.z, 1 )
			);
		}

	}

	/// <summary>Shared functionality for 3D parametric curves of degree 3 or higher</summary>
	public static class IParamCurve3DiffExt3D {

		const MethodImplOptions INLINE = MethodImplOptions.AggressiveInlining;

		/// <summary>Returns the torsion at the given t-value on the curve, in radians per distance unit</summary>
		/// <param name="t">The t-value along the curve to sample</param>
		[MethodImpl( INLINE )] public static float GetTorsion<T>( this T curve, float t ) where T : IParamCurve3Diff<Vector3> => Mathfs.GetTorsion( curve.GetDerivative( t ), curve.GetSecondDerivative( t ), curve.GetThirdDerivative( t ) );
	}


}