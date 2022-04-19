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

using Genesis.Plugin;
using Genesis.Shared;
using JavaPropertiesParser;
using JavaPropertiesParser.Expressions;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using static JavaPropertiesParser.Build;

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
		/// Returns a <see cref="IGenesisConfig"/> instance from a Java-styled properties file <see cref="string"/>.
		/// </summary>
		public static IGenesisConfig LoadGenesisConfigFromProperties(this string propertiesConfig)
		{
			var properties = Parser.Parse(propertiesConfig);
			var config = new GenesisConfig();

			foreach (var expression in properties.Expressions.OfType<KeyValuePairExpression>())
			{
				var key = expression.Key?.Text?.LogicalValue ?? string.Empty;
				var value = expression.Value?.Text?.LogicalValue ?? string.Empty;

				config.SetValue(key, value);
			}

			return config;
		}

		/// <summary>
		/// Returns a <see cref="IGenesisConfig"/> instance from a file at <paramref name="filePath"/>. Loads the file
		/// as a .properties file if <paramref name="filePath"/> ends in ".properties", otherwise loads it as JSON.
		/// </summary>
		public static IGenesisConfig LoadGenesisConfigFromFile(this string filePath)
		{
			var fileContents = File.ReadAllText(filePath);

			if (filePath.EndsWith(".properties"))
			{
				return fileContents.LoadGenesisConfigFromProperties();
			}

			return fileContents.LoadGenesisConfigFromJson();
		}

		/// <summary>
		/// Returns a Json string representing the serialized form of this config.
		/// </summary>
		public static string ConvertToJson(this GenesisConfig genesisConfig)
		{
			return JsonConvert.SerializeObject(genesisConfig, Formatting.Indented);
		}

		/// <summary>
		/// Returns a string representing the serialized java properties form of this config.
		/// </summary>
		public static string ConvertToPropertiesFile(this GenesisConfig genesisConfig)
		{
			var stringBuilder = new StringBuilder();
			var separator = Separator("=");
			var expressions = new List<ITopLevelExpression>();
			foreach (var kvp in genesisConfig.keyValuePairs)
			{
				ValueExpression value;
				var rawValue = kvp.value;

				// If the config value is an array, attempt to split it out over multiple lines
				var arrayRawValue = rawValue.Split(",");
				if (arrayRawValue.Length > 1)
				{
					stringBuilder.Clear();
					stringBuilder.AppendLine(" /");
					for (var i = 0; i < arrayRawValue.Length; i++)
					{
						var str = arrayRawValue[i];
						stringBuilder.Append($"    {str.Trim()} /");

						if (i < arrayRawValue.Length - 1)
						{
							stringBuilder.AppendLine();
						}
					}

					value = Value(stringBuilder.ToString());
				}
				else
				{
					value = Value(kvp.value);
				}

				var key = Key(kvp.key);
				expressions.Add(Pair(key, separator, value));
				expressions.Add(Whitespace("\n"));
			}

			var properties = Doc(expressions.ToArray());
			return properties.ToString().ToUnixLineEndings();
		}
	}
}
