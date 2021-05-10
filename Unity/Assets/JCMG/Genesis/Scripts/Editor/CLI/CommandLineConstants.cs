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
	/// Shared constant values for command line parameters
	/// </summary>
	internal static class CommandLineConstants
	{
		// Verbs
		public const string GENERATE_VERB_PARAM = "generate";
		public const string CONFIG_VERB_PARAM = "config";

		// General Options
		public const string VERBOSE_PARAM = "--verbose";

		// Code Generation Options
		public const string DRY_RUN_PARAM = "--dryRun";
		public const string CONFIG_BASE64_PARAM = "--config-base64";
		public const string CONFIG_FILE_PARAM = "--config-file";

		// Config Generation Options
		public const string CONFIG_CREATE_PARAM = "--create";
		public const string OUTPUT_PATH_PARAM = "--output-path";
		public const string PROJECT_PATH_PARAM = "--project-path";
		public const string SOLUTION_PATH_PARAM = "--solution-path";
	}
}
