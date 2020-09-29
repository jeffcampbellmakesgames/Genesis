using Genesis.Plugin;
using Genesis.Shared;

namespace Genesis.Core.Plugin
{
	/// <summary>
	/// A <see cref="IPostProcessor"/> that converts line endings in all <see cref="CodeGenFile"/> instances to either
	/// Windows or Linux style.
	/// </summary>
	internal sealed class NewLinePostProcessor : IPostProcessor,
	                                             IConfigurable
	{
		public string Name => NAME;

		public int Priority => 95;

		public bool RunInDryMode => true;

		private readonly LineEndingConfig _lineEndingConfig = new LineEndingConfig();

		private const string NAME = "Convert Line Endings";

		/// <summary>
		/// Configures preferences
		/// </summary>
		/// <param name="genesisConfig"></param>
		public void Configure(IGenesisConfig genesisConfig)
		{
			_lineEndingConfig.Configure(genesisConfig);
		}

		public CodeGenFile[] PostProcess(CodeGenFile[] files)
		{
			foreach (var file in files)
			{
				file.FileContent = _lineEndingConfig.LineEnding == LineEndingMode.Unix
					? file.FileContent.ToUnixLineEndings()
					: file.FileContent.ToWindowsLineEndings();
			}

			return files;
		}
	}
}
