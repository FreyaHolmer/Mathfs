namespace Freya {

	public interface IHalfNumber<out F> {
		/// <summary>Multiplies this by 2 and returns an integer value</summary>
		public F times2 { get; }
	}

}