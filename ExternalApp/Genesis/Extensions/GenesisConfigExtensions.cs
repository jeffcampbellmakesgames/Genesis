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
