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

namespace JCMG.Genesis.Editor
{
	/// <summary>
	/// General purpose properties for Genesis
	/// </summary>
	internal sealed class CodeGeneratorConfig : AbstractConfigurableConfig
	{
		public string[] SearchPaths
		{
			get { return _settings.GetOrSetValue(SEARCH_PATHS_KEY).ArrayFromCSV(); }
			set { _settings.SetValue(SEARCH_PATHS_KEY, value.ToCSV()); }
		}

		public string[] Plugins
		{
			get { return _settings.GetOrSetValue(PLUGINS_PATHS_KEY).ArrayFromCSV(); }
			set { _settings.SetValue(PLUGINS_PATHS_KEY, value.ToCSV()); }
		}

		public string[] PreProcessors
		{
			get { return _settings.GetOrSetValue(PRE_PROCESSORS_KEY).ArrayFromCSV(); }
			set { _settings.SetValue(PRE_PROCESSORS_KEY, value.ToCSV()); }
		}

		public string[] DataProviders
		{
			get { return _settings.GetOrSetValue(DATA_PROVIDERS_KEY).ArrayFromCSV(); }
			set { _settings.SetValue(DATA_PROVIDERS_KEY, value.ToCSV()); }
		}

		public string[] CodeGenerators
		{
			get { return _settings.GetOrSetValue(CODE_GENERATORS_KEY).ArrayFromCSV(); }
			set { _settings.SetValue(CODE_GENERATORS_KEY, value.ToCSV()); }
		}

		public string[] PostProcessors
		{
			get { return _settings.GetOrSetValue(POST_PROCESSORS_KEY).ArrayFromCSV(); }
			set { _settings.SetValue(POST_PROCESSORS_KEY, value.ToCSV()); }
		}

		private static readonly Dictionary<string, string> DEFAULT_VALUES;

		private const string SEARCH_PATHS_KEY = "Genesis.SearchPaths";
		private const string PLUGINS_PATHS_KEY = "Genesis.Plugins";
		private const string PRE_PROCESSORS_KEY = "Genesis.PreProcessors";
		private const string DATA_PROVIDERS_KEY = "Genesis.DataProviders";
		private const string CODE_GENERATORS_KEY = "Genesis.CodeGenerators";
		private const string POST_PROCESSORS_KEY = "Genesis.PostProcessors";

		static CodeGeneratorConfig()
		{
			DEFAULT_VALUES = new Dictionary<string, string>
			{
				{
					SEARCH_PATHS_KEY, string.Empty
				},
				{
					PLUGINS_PATHS_KEY, string.Empty
				},
				{
					PRE_PROCESSORS_KEY, string.Empty
				},
				{
					DATA_PROVIDERS_KEY, string.Empty
				},
				{
					CODE_GENERATORS_KEY, string.Empty
				},
				{
					POST_PROCESSORS_KEY, string.Empty
				}
			};
		}

		/// <summary>
		/// Configures preferences
		/// </summary>
		/// <param name="settings"></param>
		public override void Configure(GenesisSettings settings)
		{
			base.Configure(settings);

			settings.SetIfNotPresent(DEFAULT_VALUES);
		}
	}
}
