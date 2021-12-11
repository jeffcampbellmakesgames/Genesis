namespace Genesis.Unity.Factory
{
	/// <summary>
	/// Represents an object that can be uniquely identified by it's <see cref="Symbol"/>.
	/// </summary>
	public interface ISymbolObject
	{
		/// <summary>
		/// Represents a unique identifier for this object.
		/// </summary>
		string Symbol { get; }
	}
}
