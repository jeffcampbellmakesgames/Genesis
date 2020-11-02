using System.IO;
using Genesis.Plugin;
using Genesis.Shared;
using Serilog;

namespace Genesis.Core.Plugin
{
	/// <summary>
	/// An <see cref="IPostProcessor"/> for removing all files from the output folder for code-generated files.
	/// </summary>
	internal sealed class CleanTargetDirectoryPostProcessor : IPostProcessor,
	                                                          IConfigurable
	{
		public string Name => NAME;

		public int Priority => 94;

		public bool RunInDryMode => false;

		private readonly ILogger _logger = Log.ForContext<CleanTargetDirectoryPostProcessor>();
		private readonly TargetDirectoryConfig _targetDirectoryConfig = new TargetDirectoryConfig();

		private const string NAME = "Clean Target Directory";

		public void Configure(IGenesisConfig genesisConfig)
		{
			_targetDirectoryConfig.Configure(genesisConfig);
		}

		public CodeGenFile[] PostProcess(CodeGenFile[] files)
		{
			CleanDirectory();

			return files;
		}

		private void CleanDirectory()
		{
			if (Directory.Exists(_targetDirectoryConfig.TargetDirectory))
			{
				foreach (var file in new DirectoryInfo(_targetDirectoryConfig.TargetDirectory).GetFiles(
					"*.cs",
					SearchOption.AllDirectories))
				{
					try
					{
						File.Delete(file.FullName);
					}
					catch
					{
						_logger.Error("Could not delete file " + file);
					}
				}
			}
			else
			{
				Directory.CreateDirectory(_targetDirectoryConfig.TargetDirectory);
			}
		}
	}
}
