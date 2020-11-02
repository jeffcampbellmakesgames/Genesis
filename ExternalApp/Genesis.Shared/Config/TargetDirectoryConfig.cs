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

namespace Genesis.Shared
{
	/// <summary>
	/// A configuration definition for a target directory to write code-generation output to.
	/// </summary>
	public class TargetDirectoryConfig : AbstractConfigurableConfig
	{
		public string TargetDirectory
		{
			get => _genesisConfig.GetOrSetValue(TARGET_DIRECTORY_KEY, ASSETS_FOLDER).ToSafeDirectory();
			set => _genesisConfig.SetValue(TARGET_DIRECTORY_KEY, value.ToSafeDirectory());
		}

		private const string TARGET_DIRECTORY_KEY = "Genesis.TargetDirectory";
		private const string ASSETS_FOLDER = "Assets";

		/// <summary>
		/// Attempts to set defaults for the <paramref name="genesisConfig"/> and initializes any local state.
		/// </summary>
		public override void Configure(IGenesisConfig genesisConfig)
		{
			base.Configure(genesisConfig);

			genesisConfig.SetIfNotPresent(TARGET_DIRECTORY_KEY, ASSETS_FOLDER);
		}
	}
}
