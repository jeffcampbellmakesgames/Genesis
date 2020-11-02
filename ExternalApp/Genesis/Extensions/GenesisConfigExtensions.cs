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

using Newtonsoft.Json;
using System.IO;
using Genesis.Shared;

namespace Genesis.CLI
{
	/// <summary>
	/// Helper methods for <see cref="IGenesisConfig"/>.
	/// </summary>
	internal static class GenesisConfigExtensions
	{
		/// <summary>
		/// Returns a <see cref="IGenesisConfig"/> instance from a base64-encoded Json <see cref="string"/>.
		/// </summary>
		public static IGenesisConfig LoadGenesisConfigFromBase64(this string base64Config)
		{
			return base64Config.ConvertFromBase64().LoadGenesisConfigFromJson();
		}

		/// <summary>
		/// Returns a <see cref="IGenesisConfig"/> instance from a Json <see cref="string"/>.
		/// </summary>
		public static IGenesisConfig LoadGenesisConfigFromJson(this string jsonConfig)
		{
			return JsonConvert.DeserializeObject<GenesisConfig>(jsonConfig);
		}

		/// <summary>
		/// Returns a <see cref="IGenesisConfig"/> instance from a Json file at <paramref name="filePath"/>.
		/// </summary>
		public static IGenesisConfig LoadGenesisConfigFromFile(this string filePath)
		{
			var fileContents = File.ReadAllText(filePath);
			return fileContents.LoadGenesisConfigFromJson();
		}

		/// <summary>
		/// Returns a Json string representing the serialized form of this config.
		/// </summary>
		public static string ConvertToJson(this GenesisConfig genesisConfig)
		{
			return JsonConvert.SerializeObject(genesisConfig, Formatting.Indented);
		}
	}
}
