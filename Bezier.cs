// by Freya Holmér (https://github.com/FreyaHolmer/Mathfs)
// a lot of stuff here made possible by this excellent writeup on bezier curves: https://pomax.github.io/bezierinfo/

using System;
using UnityEngine;
using static Freya.Mathfs;

namespace Freya {

	// Bezier math
	// A lot of the following code is unrolled into floats and components for performance reasons.
	// It's much faster than keeping the more readable function calls and vector types unfortunately
	[Serializable] public partial struct BezierCubic2D {
		public Vector2 p0, p1, p2, p3;
		public BezierCubic2D( Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3 ) => ( this.p0, this.p1, this.p2, this.p3 ) = ( p0, p1, p2, p3 );
		public Vector2 this[ int i ] {
			get {
				switch( i ) {
					case 0:  return p0;
					case 1:  return p1;
					case 2:  return p2;
					case 3:  return p3;
					default: throw new ArgumentOutOfRangeException( nameof(i), $"Index has to be in the 0 to 3 range, and I think {i} is outside that range you know" );
				}
			}
			set {
				switch( i ) {
					case 0:
						p0 = value;
						break;
					case 1:
						p1 = value;
						break;
					case 2:
						p2 = value;
						break;
					case 3:
						p3 = value;
						break;
					default: throw new ArgumentOutOfRangeException( nameof(i), $"Index has to be in the 0 to 3 range, and I think {i} is outside that range you know" );
				}
			}

		}
	}

	[Serializable] public partial struct BezierCubic3D {
		public Vector3 p0, p1, p2, p3;
		public BezierCubic3D( Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3 ) => ( this.p0, this.p1, this.p2, this.p3 ) = ( p0, p1, p2, p3 );
		public Vector3 this[ int i ] {
			get {
				switch( i ) {
					case 0:  return p0;
					case 1:  return p1;
					case 2:  return p2;
					case 3:  return p3;
					default: throw new IndexOutOfRangeException();
				}
			}
			set {
				switch( i ) {
					case 0:
						p0 = value;
						break;
					case 1:
						p1 = value;
						break;
					case 2:
						p2 = value;
						break;
					case 3:
						p3 = value;
						break;
					default: throw new IndexOutOfRangeException();
				}
			}

		}
	}

}