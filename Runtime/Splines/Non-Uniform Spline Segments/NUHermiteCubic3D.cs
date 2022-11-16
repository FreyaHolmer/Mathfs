using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Freya {

	public class NUHermiteCubic3D : IParamSplineSegment<Polynomial3D, Vector3Matrix4x1> {

		const MethodImplOptions INLINE = MethodImplOptions.AggressiveInlining;

		/// <inheritdoc cref="NUCatRomCubic2D(Vector2Matrix4x1,Matrix4x1)"/>
		public NUHermiteCubic3D( Vector3Matrix4x1 pointMatrix, (float k0, float k1) knotVector ) {
			this.pointMatrix = pointMatrix;
			this.KnotVector = knotVector;
			validCoefficients = false;
			curve = default;
		}

		// serialized data
		[SerializeField] Vector3Matrix4x1 pointMatrix;
		public Vector3Matrix4x1 PointMatrix {
			get => pointMatrix;
			set => _ = ( pointMatrix = value, validCoefficients = false );
		}
		[SerializeField] float k0, k1;
		public (float k0, float k1) KnotVector {
			get => ( k0, k1 );
			set => _ = ( ( k0, k1 ) = value, validCoefficients = false );
		}

		Polynomial3D curve;
		public Polynomial3D Curve {
			get {
				ReadyCoefficients();
				return curve;
			}
		}

		// cached data to accelerate calculations
		[NonSerialized] bool validCoefficients; // inverted isDirty flag (can't default to true in structs)

		[MethodImpl( INLINE )] void ReadyCoefficients() {
			if( validCoefficients )
				return; // no need to update
			validCoefficients = true;
			curve = SplineUtils.CalculateHermiteCurve( pointMatrix, k0, k1 );
		}

	}

}