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

using System;
using Genesis.Shared;

namespace Genesis.Core
{
	/// <summary>
	/// A configuration definition for the line-endings code-generation output should have.
	/// </summary>
	internal sealed class LineEndingConfig : AbstractConfigurableConfig
	{
		/// <summary>
		/// The <see cref="LineEndingMode"/> that should be used in code-generated files (Windows or Linux).
		/// </summary>
		public LineEndingMode LineEnding
		{
			get
			{
				var enumStr = _genesisConfig.GetOrSetValue(LINE_ENDINGS_KEY, LineEndingMode.Unix.ToString());
				if (Enum.TryParse<LineEndingMode>(enumStr, true, out var mode))
				{
					return mode;
				}

				_genesisConfig.SetValue(LINE_ENDINGS_KEY, LineEndingMode.Unix.ToString());

				return LineEndingMode.Unix;
			}
			set => _genesisConfig.SetValue(LINE_ENDINGS_KEY, value.ToString());
		}

		private const string LINE_ENDINGS_KEY = "Genesis.LineEndings";

		/// <summary>
		/// Attempts to set defaults for the <paramref name="genesisConfig"/> and initializes any local state.
		/// </summary>
		public override void Configure(IGenesisConfig genesisConfig)
		{
			base.Configure(genesisConfig);

			genesisConfig.SetIfNotPresent(LINE_ENDINGS_KEY, LineEndingMode.Unix.ToString());
		}
	}
}
