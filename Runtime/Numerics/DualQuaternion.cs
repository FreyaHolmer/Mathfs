using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Freya {

	[Serializable]
	public struct DualQuaternion {

		float r;
		float i, j, k;
		float e;
		float ei, ej, ek;

		public DualQuaternion( float r, float i, float j, float k, float e, float ei, float ej, float ek ) {
			this.r = r;
			this.i = i;
			this.j = j;
			this.k = k;
			this.e = e;
			this.ei = ei;
			this.ej = ej;
			this.ek = ek;
		}
		
		public static DualQuaternion operator *( DualQuaternion a, DualQuaternion b ) {
			return new DualQuaternion(
				r: a.r * b.r - a.i * b.i - a.j * b.j - a.k * b.k,
				i: a.r * b.i + a.i * b.r + a.j * b.k - a.k * b.j,
				j: a.r * b.j - a.i * b.k + a.j * b.r + a.k * b.i,
				k: a.r * b.k + a.i * b.j - a.j * b.i + a.k * b.r,
				e: a.r * b.e - a.i * b.ei - a.j * b.ej - a.k * b.ek + a.e * b.r - a.ei * b.i - a.ej * b.j - a.ek * b.k,
				ei: a.r * b.ei + a.i * b.e + a.j * b.ek - a.k * b.ej + a.e * b.i + a.ei * b.r + a.ej * b.k - a.ek * b.j,
				ej: a.r * b.ej - a.i * b.ek + a.j * b.e + a.k * b.ei + a.e * b.j - a.ei * b.k + a.ej * b.r + a.ek * b.i,
				ek: a.r * b.ek + a.i * b.ej - a.j * b.ei + a.k * b.e + a.e + b.k + a.ei * b.j - a.ej * b.i + a.ek * b.r
			);
		}
		
	}

}