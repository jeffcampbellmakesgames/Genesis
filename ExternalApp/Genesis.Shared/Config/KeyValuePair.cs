using System;

namespace Genesis.Shared
{
	/// <summary>
	/// A serializable key-value pair for configuration.
	/// </summary>
	[Serializable]
	internal class KeyValuePair
	{
		public string key;
		public string value;
	}
}
