using System.Collections.Generic;

namespace Genesis.Shared
{
	/// <summary>
	/// Represents a serializable config made up of key-value pairs.
	/// </summary>
	public interface IGenesisConfig
	{
		/// <summary>
		/// The human-readable name of this configuration.
		/// </summary>
		string Name { get; }

		/// <summary>
		/// Returns true if an existing KeyValuePair exists for <paramref name="key"/>, otherwise false.
		/// </summary>
		bool HasValue(string key);

		/// <summary>
		/// Gets the value for existing KeyValuePair with <paramref name="key"/>. If none is found a new KeyValuePair
		/// instance is created using the <paramref name="defaultValue"/> and returns it.
		/// </summary>
		string GetOrSetValue(string key, string defaultValue = "");

		/// <summary>
		/// Creates or updates an existing KeyValuePair.
		/// </summary>
		void SetValue(string key, string value);

		/// <summary>
		/// Creates an existing KeyValuePair if one is not already present.
		/// </summary>
		void SetIfNotPresent(Dictionary<string, string> defaultValues);

		/// <summary>
		/// Creates an existing KeyValuePair if one is not already present.
		/// </summary>
		void SetIfNotPresent(string key, string value);

		/// <summary>
		/// Creates a new instance of <typeparamref name="T"/> and configures it with <see cref="KeyValuePair"/>
		/// instances from this <see cref="IGenesisConfig"/>.
		/// </summary>
		T CreateAndConfigure<T>()
			where T : IConfigurable, new();
	}
}
