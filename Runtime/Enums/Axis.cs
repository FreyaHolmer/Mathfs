// by Freya Holmér (https://github.com/FreyaHolmer/Mathfs)

using Unity.Mathematics;

namespace Freya {

	public enum Axis {
		X = 0,
		Y = 1,
		Z = 2
	}

	public static class AxisExtensions {
		public static float3 asFloat3( this Axis a ) => new() { [(int)a] = 1 };
	}

}