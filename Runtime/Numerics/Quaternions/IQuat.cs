using Unity.Mathematics;

namespace Freya {

	public interface IQuat {
		public float Real { get; }
		public quatPure Im { get; }
		public bool IsUnit { get; }
		public bool IsPure { get; }
		public float SqMag { get; }
		public float Mag { get; }
		public float this[ int i ] { get; }
		
		// swizzling
		public float2 xy { get; }
		public float2 yz { get; }
		public float2 zx { get; }
		public float2 yx { get; }
		public float2 zy { get; }
		public float2 xz { get; }
		public float3 xyz { get; }
		public float4 xyzw { get; }
		
	}
	
	public interface IQuat<out QSelf> : IQuat {
		public QSelf Conj { get; }
		public QSelf Inverse { get; }
	}

}