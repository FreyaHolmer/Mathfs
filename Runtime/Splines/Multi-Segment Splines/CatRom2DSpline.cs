using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Freya {

	[Serializable]
	public class CatRom2DSpline {

		const MethodImplOptions INLINE = MethodImplOptions.AggressiveInlining;

		public enum EndpointMode {
			None,
			Extrapolate,
			Collapse
		}

		public struct Node {
			public Vector2 pos;
			public float knot;
			public Polynomial2D curve;

			public Vector2 EvalPoint( float u ) {
				u -= knot; // transform to local knot value
				return curve.Eval( u );
			}

			public Vector2 EvalDerivative( float u ) {
				u -= knot; // transform to local knot value
				return curve.EvalDerivative( u );
			}

			public Vector2 EvalSecondDerivative( float u ) {
				u -= knot; // transform to local knot value
				return curve.EvalSecondDerivative( u );
			}

			public Vector2 EvalThirdDerivative() => curve.EvalThirdDerivative();
		}

		public List<Node> nodes;
		[SerializeField] [Range( 0, 1 )] float alpha;
		[SerializeField] bool autoCalculateKnots;
		[SerializeField] public EndpointMode endpointMode;
		[NonSerialized] bool isDirty;

		#region Properties

		bool IncludeEndpoints {
			[MethodImpl( INLINE )] get => endpointMode != EndpointMode.None;
		}

		/// <inheritdoc cref="NUCatRomCubic2D.Alpha"/>
		public float Alpha {
			[MethodImpl( INLINE )] get => alpha;
			[MethodImpl( INLINE )] set {
				isDirty = true;
				alpha = value;
			}
		}

		/// <summary>Whether or not to calculate knots based on the <c>alpha</c> value</summary>
		public bool AutoCalculateKnots {
			[MethodImpl( INLINE )] get => autoCalculateKnots;
			[MethodImpl( INLINE )] set {
				isDirty = true;
				autoCalculateKnots = value;
			}
		}

		int IndexSplineStart {
			[MethodImpl( INLINE )] get => IncludeEndpoints ? 0 : 1;
		}
		int IndexSplineEnd {
			[MethodImpl( INLINE )] get => ControlPointCount - ( IncludeEndpoints ? 1 : 2 );
		}

		/// <summary>The number of control points in this spline</summary>
		public int ControlPointCount {
			[MethodImpl( INLINE )] get => nodes.Count;
		}

		/// <summary>The number of curves in this spline</summary>
		public int CurveCount {
			[MethodImpl( INLINE )] get => ControlPointCount - ( IncludeEndpoints ? 1 : 2 );
		}

		/// <summary>The knot value at the start of the spline</summary>
		public float KnotStart {
			[MethodImpl( INLINE )] get => GetKnot( IndexSplineStart );
		}

		/// <summary>The knot value at the end of the spline</summary>
		public float KnotEnd {
			[MethodImpl( INLINE )] get => GetKnot( IndexSplineEnd );
		}
		/// <summary>The knot range of the spline from start to end</summary>
		public float KnotRange {
			[MethodImpl( INLINE )] get => KnotEnd - KnotStart;
		}

		/// <summary>The starting point of this spline</summary>
		public Vector2 StartPoint {
			[MethodImpl( INLINE )] get => nodes[IndexSplineStart].pos;
		}

		/// <summary>The endpoint of this spline</summary>
		public Vector2 EndPoint {
			[MethodImpl( INLINE )] get => nodes[IndexSplineEnd].pos;
		}

		#endregion

		#region Constructors

		/// <summary>Creates a cubic catmull-rom spline, given a set of control points</summary>
		/// <param name="points">The control points of the spline</param>
		/// <param name="knots">The knot values of the spline</param>
		/// <param name="endpointMode">Whether or not the spline should reach the endpoints, and how</param>
		public CatRom2DSpline( IReadOnlyCollection<Vector2> points, IReadOnlyCollection<float> knots, EndpointMode endpointMode = EndpointMode.None ) {
			if( points == null || knots == null )
				throw new NullReferenceException( $"{GetType().Name} requires non-null inputs" );
			if( points.Count != knots.Count )
				throw new Exception( $"{GetType().Name} points[{points.Count}] and knots[{knots.Count}] have to have the same count" );
			if( points.Count < 2 )
				throw new Exception( $"{GetType().Name} requires at least 2 points" );
			nodes = points.Zip( knots, ( p, k ) => new Node { pos = p, knot = k } ).ToList();
			isDirty = true;
			autoCalculateKnots = false;
			this.endpointMode = endpointMode;
		}

		/// <summary>Creates a cubic catmull-rom spline, given a set of control points</summary>
		/// <param name="points">The control points of the spline</param>
		/// <param name="alpha">The alpha parameter controls how much the length of each segment should influence the shape of the curve.
		/// A value of 0 is called a uniform catrom, and is fast to evaluate but has a tendency to overshoot.
		/// A value of 0.5 is a centripetal catrom, which follows points very tightly, and prevents cusps and loops.
		/// A value of 1 is a chordal catrom, which follows the points very smoothly with wide arcs</param>
		/// <param name="endpointMode">Whether or not the spline should reach the endpoints, and how</param>
		public CatRom2DSpline( IReadOnlyCollection<Vector2> points, float alpha, EndpointMode endpointMode = EndpointMode.None ) {
			if( points == null )
				throw new NullReferenceException( $"{GetType().Name} requires non-null points" );
			if( points.Count < 2 )
				throw new Exception( $"{GetType().Name} requires at least 2 points" );
			nodes = points.Select( p => new Node { pos = p } ).ToList();
			this.alpha = alpha;
			this.isDirty = true;
			this.autoCalculateKnots = true;
			this.endpointMode = endpointMode;
		}

		/// <summary>Creates a cubic catmull-rom spline, given a set of control points</summary>
		/// <param name="points">The control points of the spline</param>
		/// <param name="type">The type of catrom curve to use. This will internally determine the value of the <c>alpha</c> parameter</param>
		/// <param name="endpointMode">Whether or not the spline should reach the endpoints, and how</param>
		public CatRom2DSpline( IReadOnlyCollection<Vector2> points, CatRomType type, EndpointMode endpointMode = EndpointMode.None ) {
			if( points == null )
				throw new NullReferenceException( $"{GetType().Name} requires non-null points" );
			if( points.Count < 2 )
				throw new Exception( $"{GetType().Name} requires at least 2 points" );
			nodes = points.Select( p => new Node { pos = p } ).ToList();
			this.alpha = type.AlphaValue();
			this.isDirty = true;
			this.autoCalculateKnots = true;
			this.endpointMode = endpointMode;
		}

		#endregion

		#region Points & Derivatives

		/// <summary>Returns the point at parameter value <c>u</c></summary>
		/// <param name="u">The parameter space position to sample the point at</param>
		[MethodImpl( INLINE )] public Vector2 GetPoint( float u ) {
			u = ReadyAndClampU( u );
			return GetPointInternal( GetIntervalIndexForKnotValue( u ), u );
		}

		/// <summary>Returns the derivative with respect to <c>u</c> at the input parameter value</summary>
		/// <param name="u">The parameter space position to sample the derivative at</param>
		[MethodImpl( INLINE )] public Vector2 GetDerivative( float u ) {
			u = ReadyAndClampU( u );
			return GetDerivativeInternal( GetIntervalIndexForKnotValue( u ), u );
		}

		/// <summary>Returns the second derivative with respect to <c>u</c> at the input parameter value</summary>
		/// <param name="u">The parameter space position to sample the second derivative at</param>
		[MethodImpl( INLINE )] public Vector2 GetSecondDerivative( float u ) {
			u = ReadyAndClampU( u );
			return GetSecondDerivativeInternal( GetIntervalIndexForKnotValue( u ), u );
		}

		/// <summary>Returns the third derivative with respect to <c>u</c> at the input parameter value</summary>
		/// <param name="u">The parameter space position to sample the third derivative at</param>
		[MethodImpl( INLINE )] public Vector2 GetThirdDerivative( float u ) {
			return GetThirdDerivativeInternal( GetIntervalIndexForKnotValue( ReadyAndClampU( u ) ) );
		}

		[MethodImpl( INLINE )] float ReadyAndClampU( float u ) {
			ReadyKnotsAndCoefficients();
			return ClampToKnotRange( u );
		}

		#endregion

		#region Recalculations

		/// <summary>Ensures the knot vector is ready (if <c>autoCalculateKnots</c> is on) and the coefficients are up to date</summary>
		public void ReadyKnotsAndCoefficients() {
			if( isDirty ) {
				isDirty = false;
				if( autoCalculateKnots )
					RecalculateKnots();
				for( int i = 0; i < ControlPointCount - 1; i++ ) {
					Node n = nodes[i];
					// knot parameters are local to each spline's main (second) knot
					Vector2Matrix4x1 pts = new(GetControlPoint( i - 1 ), n.pos, GetControlPoint( i + 1 ), GetControlPoint( i + 2 ));
					Matrix4x1 knots = new(GetKnot( i - 1 ) - n.knot, 0, GetKnot( i + 1 ) - n.knot, GetKnot( i + 2 ) - n.knot);
					n.curve = SplineUtils.CalculateCatRomCurve( pts, knots );
					nodes[i] = n;
				}
			}
		}

		/// <summary>Recalculates the knot vector based on alpha and the point distances</summary>
		public void RecalculateKnots() {
			if( alpha == 0 ) { // uniform catrom
				for( int i = 0; i < ControlPointCount; i++ )
					SetKnotInternal( i, i );
			} else { // non-uniform
				SetKnotInternal( 0, 0 ); // first knot is 0
				// todo: it's possible to cache and optimize these distance checks
				// todo: by caching distances and only recalculating the necessary ones
				for( int i = 1; i < ControlPointCount; i++ ) {
					float sqDist = Vector2.SqrMagnitude( nodes[i - 1].pos - nodes[i].pos );
					SetKnotInternal( i, SplineUtils.CalcCatRomKnot( nodes[i - 1].knot, sqDist, alpha, true ) );
				}
			}
		}

		#endregion

		#region Point/Knot getter/setters

		/// <summary>Get the position of a control point by index</summary>
		/// <param name="index">The index of the point</param>
		public Vector2 GetControlPoint( int index ) {
			if( endpointMode == EndpointMode.Collapse )
				index = index.Clamp( 0, ControlPointCount - 1 );

			if( index == -1 ) // extrapolate at the ends
				return Vector2.LerpUnclamped( nodes[1].pos, nodes[0].pos, 2 );
			if( index == ControlPointCount )
				return Vector2.LerpUnclamped( nodes[ControlPointCount - 2].pos, nodes[ControlPointCount - 1].pos, 2 );

			return nodes[index].pos;
		}

		/// <summary>Set the position of a control point by index</summary>
		/// <param name="index">The index of the knot</param>
		/// <param name="position">The position to assign to the control point</param>
		public void SetControlPoint( int index, Vector2 position ) {
			isDirty = true;
			Node n = nodes[index];
			n.pos = position;
			nodes[index] = n;
		}

		/// <summary>Get the value of knot by index</summary>
		/// <param name="index">The index of the knot</param>
		public float GetKnot( int index ) {
			ReadyKnotsAndCoefficients();
			if( index == -1 ) // extrapolate at the ends
				return Mathfs.Lerp( nodes[1].knot, nodes[0].knot, 2 );
			if( index == ControlPointCount )
				return Mathfs.Lerp( nodes[ControlPointCount - 2].knot, nodes[ControlPointCount - 1].knot, 2 );
			return nodes[index].knot;
		}

		/// <summary>Get the knot value at a given t-value along the whole spline</summary>
		/// <param name="t">The percentage along the spline from 0 to 1</param>
		public float GetKnotValue( float t ) => Mathfs.LerpClamped( KnotStart, KnotEnd, t );

		/// <summary>Sets the given knot to a specific value</summary>
		/// <param name="index">The index of the knot to edit</param>
		/// <param name="value">The value to assign to the knot</param>
		[MethodImpl( INLINE )] public void SetKnot( int index, float value ) {
			isDirty = true;
			SetKnotInternal( index, value );
		}

		[MethodImpl( INLINE )] void SetKnotInternal( int index, float value ) {
			Node n = nodes[index];
			n.knot = value;
			nodes[index] = n;
		}

		#endregion

		#region Get point/derivative by curve index

		/// <summary>Returns a point along a curve by index</summary>
		/// <param name="curve">The index of the curve to sample</param>
		/// <param name="t">The fraction along this segment from 0 to 1</param>
		[MethodImpl( INLINE )] public Vector2 GetPoint( int curve, float t ) => GetPointInternal( curve, RangeCheckAndGetU( curve, t ) );

		/// <summary>Returns the derivative with respect to <c>u</c> along a curve by index</summary>
		/// <param name="curve">The index of the curve to sample</param>
		/// <param name="t">The fraction along this segment from 0 to 1</param>
		[MethodImpl( INLINE )] public Vector2 GetDerivative( int curve, float t ) => GetDerivativeInternal( curve, RangeCheckAndGetU( curve, t ) );

		/// <summary>Returns the second derivative with respect to <c>u</c> along a curve by index</summary>
		/// <param name="curve">The index of the curve to sample</param>
		/// <param name="t">The fraction along this segment from 0 to 1</param>
		[MethodImpl( INLINE )] public Vector2 GetSecondDerivative( int curve, float t ) => GetSecondDerivativeInternal( curve, RangeCheckAndGetU( curve, t ) );

		/// <summary>Returns the third derivative with respect to <c>u</c> of a curve by index</summary>
		/// <param name="curve">The index of the curve to sample</param>
		[MethodImpl( INLINE )] public Vector2 GetThirdDerivative( int curve ) => GetThirdDerivativeInternal( curve );

		float RangeCheckAndGetU( int curve, float t ) {
			if( curve < 0 || curve >= CurveCount )
				throw new IndexOutOfRangeException( $"Curve index {curve} is out of the range 0 to {CurveCount - 1}" );
			ReadyKnotsAndCoefficients();
			return Mathfs.Lerp( nodes[curve].knot, nodes[curve + 1].knot, t );
		}

		[MethodImpl( INLINE )] Vector2 GetPointInternal( int curve, float u ) => nodes[curve].EvalPoint( u );
		[MethodImpl( INLINE )] Vector2 GetDerivativeInternal( int curve, float u ) => nodes[curve].EvalDerivative( u );
		[MethodImpl( INLINE )] Vector2 GetSecondDerivativeInternal( int curve, float u ) => nodes[curve].EvalSecondDerivative( u );
		[MethodImpl( INLINE )] Vector2 GetThirdDerivativeInternal( int curve ) => nodes[curve].EvalThirdDerivative();

		#endregion

		#region Knot Utilities

		/// <summary>Clamps the input value <c>u</c> to the range of this spline</summary>
		/// <param name="u">The parameter space value to clamp</param>
		public float ClampToKnotRange( float u ) => u.Clamp( KnotStart, KnotEnd );

		/// <summary>Returns the index of the curve containing knot value <c>u</c></summary>
		/// <param name="u">The knot value to get the curve of</param>
		public int GetIntervalIndexForKnotValue( float u ) {
			ReadyKnotsAndCoefficients();
			if( u <= KnotStart )
				return 0;
			if( u >= GetKnot( ControlPointCount - 2 ) )
				return ControlPointCount - 2; // very last knot is never used as an interval (fencepost issue |-|-|)
			// todo: linear search, but, might want to use binary search if more than ~30 nodes
			for( int i = 0; i < ControlPointCount - 1; i++ ) {
				if( u < GetKnot( i + 1 ) )
					return i;
			}

			throw new Exception( $"Failed to get spline interval for knot value {u} in the range {KnotStart} to {KnotEnd}" );
		}

		#endregion

	}

}