using Genesis.Plugin;

namespace Genesis.Core.Plugin
{
	/// <summary>
	/// A <see cref="IPostProcessor"/> that adds a file header to the top of every code-generated file.
	/// </summary>
	internal sealed class AddFileHeaderPostProcessor : IPostProcessor
	{
		public string Name => NAME;

		public int Priority => 0;

		public bool RunInDryMode => true;

		private const string NAME = "Add File Header";

		public CodeGenFile[] PostProcess(CodeGenFile[] files)
		{
			foreach (var file in files)
			{
				file.FileContent = CommentTools.GetCommentHeaderWithVersion() + file.FileContent;
			}

			return files;
		}
	}
}
