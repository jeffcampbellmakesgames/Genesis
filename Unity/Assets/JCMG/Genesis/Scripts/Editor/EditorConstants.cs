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

		// General Logging
		public const string LOG_PREFIX = "[Genesis] ";
		public const string CODE_GENERATION_UPDATE_ERROR = LOG_PREFIX + "An unexpected error occured. \n\n{0}";
		public const string DOTNET_COMMAND_EXECUTION_FORMAT = LOG_PREFIX + "{0} {1}";

		// Code Generation Logging
		public const string STARTED_CODE_GENERATION = LOG_PREFIX + "Code generation started.";
		public const string CODE_GENERATION_SUCCESS = LOG_PREFIX + "Code generation successful!";
		public const string CODE_GENERATION_FAILURE = LOG_PREFIX + "Code generation failed, process exited with code {0}.";
		public const string CODE_GENERATION_UPDATE = LOG_PREFIX + "{0}";
		public const string CODE_GENERATION_UPDATE_ERROR_FORMAT =
			LOG_PREFIX + "An unexpected error occured during code generation for GenesisSettings asset [{0}].";

		// Config Generation Logging
		public const string STARTED_CONFIG_GENERATION = LOG_PREFIX + "Config generation started.";
		public const string CONFIG_GENERATION_SUCCESS = LOG_PREFIX + "Config generation successful!";
		public const string CONFIG_GENERATION_FAILURE = LOG_PREFIX + "Config generation failed, process exited with code {0}.";

		// Dotnet Core and Genesis Assemblies
		public const string DOTNET_EXE = "dotnet";
		public const string GENESIS_EXECUTABLE = "Genesis.CLI.dll";

		// General file and path
		public const string DLL_EXTENSION = ".dll";
		public const string BACKSLASH_STR = "\"";
		public const string SPACE_STR = " ";
		public const string QUOTE_STR = "\"";
		public const string COMMA_STR = ",";
		public const string EQUALS_STR = "=";

		// Config files
		public const string TEMP_CONFIG_FILE_PATH = "./unity_temp_config.json";

		// Search filters
		public const string WILDCARD_ALL_DLLS = "*.dll";
		public const string WILDCARD_ALL_SLNS = "*.sln";

		// General Dialog
		public const string DIALOG_OK = "OK";

		// Solution Dialog
		public const string SOLUTION_ISSUE_TITLE = "Visual Studio Solution Issue";
		public const string SOLUTION_MESSAGE = "A single Visual Studio solution cannot be found in the project folder " +
		                                       "[{0}]. Please ensure that Unity has generated this solution.";
	}
}
