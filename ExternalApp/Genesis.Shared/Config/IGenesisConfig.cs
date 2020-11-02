/*

MIT License

Copyright (c) Jeff Campbell

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/

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
