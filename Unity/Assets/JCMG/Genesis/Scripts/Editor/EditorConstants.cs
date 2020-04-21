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

namespace JCMG.Genesis.Editor
{
	/// <summary>
	/// Constant values for editor usage.
	/// </summary>
	internal static class EditorConstants
	{
		// Library Name
		public const string SDK_NAME = "Genesis";

		// Logging
		public const string LOG_PREFIX = "[Genesis] ";
		public const string STARTED_CODE_GENERATION = LOG_PREFIX + "Code generation started.";
		public const string CODE_GENERATION_SUCCESS = LOG_PREFIX + "Code generation successful!";
		public const string CODE_GENERATION_FAILURE = LOG_PREFIX + "Code generation failed, process exited with code {0}.";
		public const string CODE_GENERATION_UPDATE = LOG_PREFIX + "[{0}] {1}";
		public const string CODE_GENERATION_UPDATE_ERROR = LOG_PREFIX + "An unexpected error occured during code generation: {0}";
		public const string DOTNET_COMMAND_EXECUTION_FORMAT = LOG_PREFIX + "{0} {1}";

		// Dotnet Core and Genesis Assemblies
		public const string DOTNET_EXE = "dotnet";
		public const string GENESIS_EXE = "Genesis.CLI.dll";
		public const string GENESIS_SHARED_ASSEMBLY_NAME = "Genesis.Attributes";

		// General file and path
		public const string DLL_EXTENSION = ".dll";
		public const string BACKSLASH_STR = "\"";
		public const string SPACE_STR = " ";

		// Search filters
		public const string WILDCARD_ALL_DLLS = "*.dll";
	}
}
