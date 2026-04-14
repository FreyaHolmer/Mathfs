namespace Freya {

	public interface IVecProjections<V, Proj> {
		/// <summary>Projects the vector onto a second vector, as a scalar t-value along the second vector.
		/// The second vector does not need to be normalized, but if it is, the t-value is also a distance along that vector</summary>
		public Proj projTValue( V n ); // => math.dot( v, n ) / math.dot( n, n );
	}

}