using System;

namespace Freya {

	[AttributeUsage( AttributeTargets.Method | AttributeTargets.Property )]
	public class BinaryOpAttribute : Attribute {}

}