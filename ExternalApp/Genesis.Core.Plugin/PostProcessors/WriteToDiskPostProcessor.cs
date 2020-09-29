using System.IO;
using Genesis.Plugin;
using Genesis.Shared;

namespace Genesis.Core.Plugin
{
	/// <summary>
	/// A <see cref="IPostProcessor"/> that writes all <see cref="CodeGenFile"/> instances to disk.
	/// </summary>
	internal sealed class WriteToDiskPostProcessor : IPostProcessor,
	                                                 IConfigurable
	{
		public string Name => NAME;

		public int Priority => 100;

		public bool RunInDryMode => false;

		private readonly TargetDirectoryConfig _targetDirectoryConfig = new TargetDirectoryConfig();

		private const string NAME = "Write Code Gen Files to Disk";

		public void Configure(IGenesisConfig genesisConfig)
		{
			_targetDirectoryConfig.Configure(genesisConfig);
		}

		public CodeGenFile[] PostProcess(CodeGenFile[] files)
		{
			foreach (var file in files)
			{
				var path = _targetDirectoryConfig.TargetDirectory + Path.DirectorySeparatorChar + file.FileName;
				var directoryName = Path.GetDirectoryName(path);
				if (!Directory.Exists(directoryName))
				{
					Directory.CreateDirectory(directoryName);
				}

				File.WriteAllText(path, file.FileContent);
			}

			return files;
		}
	}
}
