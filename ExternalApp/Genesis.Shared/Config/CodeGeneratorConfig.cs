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
	/// General purpose properties for Genesis
	/// </summary>
	internal sealed class CodeGeneratorConfig : AbstractConfigurableConfig
	{
		public string[] SearchPaths
		{
			get => _genesisConfig.GetOrSetValue(SEARCH_PATHS_KEY).ArrayFromCSV();
			set => _genesisConfig.SetValue(SEARCH_PATHS_KEY, value.ToCSV());
		}

		public string[] Plugins
		{
			get => _genesisConfig.GetOrSetValue(PLUGINS_PATHS_KEY).ArrayFromCSV();
			set => _genesisConfig.SetValue(PLUGINS_PATHS_KEY, value.ToCSV());
		}

		public string[] EnabledPreProcessors
		{
			get => _genesisConfig.GetOrSetValue(PRE_PROCESSORS_KEY).ArrayFromCSV();
			set => _genesisConfig.SetValue(PRE_PROCESSORS_KEY, value.ToCSV());
		}

		public string[] EnabledDataProviders
		{
			get => _genesisConfig.GetOrSetValue(DATA_PROVIDERS_KEY).ArrayFromCSV();
			set => _genesisConfig.SetValue(DATA_PROVIDERS_KEY, value.ToCSV());
		}

		public string[] EnabledCodeGenerators
		{
			get => _genesisConfig.GetOrSetValue(CODE_GENERATORS_KEY).ArrayFromCSV();
			set => _genesisConfig.SetValue(CODE_GENERATORS_KEY, value.ToCSV());
		}

		public string[] EnabledPostProcessors
		{
			get => _genesisConfig.GetOrSetValue(POST_PROCESSORS_KEY).ArrayFromCSV();
			set => _genesisConfig.SetValue(POST_PROCESSORS_KEY, value.ToCSV());
		}

		public string[] AllPreProcessors
		{
			get => _genesisConfig.GetOrSetValue(ALL_PRE_PROCESSORS_KEY).ArrayFromCSV();
			set => _genesisConfig.SetValue(ALL_PRE_PROCESSORS_KEY, value.ToCSV());
		}

		public string[] AllDataProviders
		{
			get => _genesisConfig.GetOrSetValue(ALL_DATA_PROVIDERS_KEY).ArrayFromCSV();
			set => _genesisConfig.SetValue(ALL_DATA_PROVIDERS_KEY, value.ToCSV());
		}

		public string[] AllCodeGenerators
		{
			get => _genesisConfig.GetOrSetValue(ALL_CODE_GENERATORS_KEY).ArrayFromCSV();
			set => _genesisConfig.SetValue(ALL_CODE_GENERATORS_KEY, value.ToCSV());
		}

		public string[] AllPostProcessors
		{
			get => _genesisConfig.GetOrSetValue(ALL_POST_PROCESSORS_KEY).ArrayFromCSV();
			set => _genesisConfig.SetValue(ALL_POST_PROCESSORS_KEY, value.ToCSV());
		}

		private static readonly Dictionary<string, string> DEFAULT_VALUES;
		private static readonly string[] ALL_KEYS;

		private const string SEARCH_PATHS_KEY = "Genesis.SearchPaths";
		private const string PLUGINS_PATHS_KEY = "Genesis.Plugins";
		private const string PRE_PROCESSORS_KEY = "Genesis.PreProcessors";
		private const string DATA_PROVIDERS_KEY = "Genesis.DataProviders";
		private const string CODE_GENERATORS_KEY = "Genesis.CodeGenerators";
		private const string POST_PROCESSORS_KEY = "Genesis.PostProcessors";

		private const string ALL_PRE_PROCESSORS_KEY = "Genesis.AllPreProcessors";
		private const string ALL_DATA_PROVIDERS_KEY = "Genesis.AllDataProviders";
		private const string ALL_CODE_GENERATORS_KEY = "Genesis.AllCodeGenerators";
		private const string ALL_POST_PROCESSORS_KEY = "Genesis.AllPostProcessors";

		static CodeGeneratorConfig()
		{
			DEFAULT_VALUES = new Dictionary<string, string>
			{
				{ SEARCH_PATHS_KEY, string.Empty },
				{ PLUGINS_PATHS_KEY, string.Empty },
				{ PRE_PROCESSORS_KEY, string.Empty },
				{ DATA_PROVIDERS_KEY, string.Empty },
				{ CODE_GENERATORS_KEY, string.Empty },
				{ POST_PROCESSORS_KEY, string.Empty },
				{ ALL_PRE_PROCESSORS_KEY, string.Empty },
				{ ALL_DATA_PROVIDERS_KEY, string.Empty },
				{ ALL_CODE_GENERATORS_KEY, string.Empty },
				{ ALL_POST_PROCESSORS_KEY, string.Empty }
			};

			ALL_KEYS = new []
			{
				SEARCH_PATHS_KEY,
				PLUGINS_PATHS_KEY,
				PRE_PROCESSORS_KEY,
				DATA_PROVIDERS_KEY,
				CODE_GENERATORS_KEY,
				POST_PROCESSORS_KEY,
				ALL_PRE_PROCESSORS_KEY,
				ALL_DATA_PROVIDERS_KEY,
				ALL_CODE_GENERATORS_KEY,
				ALL_POST_PROCESSORS_KEY
			};
		}

		/// <summary>
		/// Attempts to set defaults for the <paramref name="genesisConfig"/> and initializes any local state.
		/// </summary>
		public override void Configure(IGenesisConfig genesisConfig)
		{
			base.Configure(genesisConfig);

			genesisConfig.SetIfNotPresent(DEFAULT_VALUES);
		}

		/// <summary>
		/// Overwrite the contents of this config from another config
		/// </summary>
		public void Overwrite(IGenesisConfig settings)
		{
			for (var i = 0; i < ALL_KEYS.Length; i++)
			{
				_genesisConfig.SetValue(ALL_KEYS[i], settings.GetOrSetValue(ALL_KEYS[i], string.Empty));
			}
		}
	}
}
