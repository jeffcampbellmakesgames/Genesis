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
