using System.IO;
using Genesis.Shared;
using UnityEngine;

namespace JCMG.Genesis.Editor
{
	/// <summary>
	/// Extension methods for <see cref="GenesisConfig"/>.
	/// </summary>
	internal static class GenesisConfigExtensions
	{
		/// <summary>
		/// Converts a <see cref="GenesisConfig"/> to a Json string.
		/// </summary>
		public static string ToJson(this GenesisConfig genesisConfig)
		{
			return JsonUtility.ToJson(genesisConfig);
		}

		/// <summary>
		/// Returns a <see cref="IGenesisConfig"/> instance from a Json <see cref="string"/>.
		/// </summary>
		public static IGenesisConfig FromJson(this string jsonConfig)
		{
			return JsonUtility.FromJson<GenesisConfig>(jsonConfig);
		}

		/// <summary>
		/// Returns a <see cref="IGenesisConfig"/> instance from a Json file at <paramref name="filePath"/>.
		/// </summary>
		public static IGenesisConfig FromFile(this string filePath)
		{
			var fileContents = File.ReadAllText(filePath);
			return fileContents.FromJson();
		}
	}
}
