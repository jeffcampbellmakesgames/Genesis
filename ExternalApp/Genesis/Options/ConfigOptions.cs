﻿/*

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

using CommandLine;

namespace Genesis.CLI
{
	/// <summary>
	/// Command-line options for manipulating or creating configs
	/// </summary>
	[Verb("config", HelpText = "Manipulates or creates a configuration file.")]
	internal sealed class ConfigOptions
	{
		[Option(
			longName:"create",
			SetName = "create",
			HelpText = "Specifies that a config file should be created and populated with all settings from " +
			           "available plugins.")]
		public bool DoCreate { get; set; }

		[Option(
			longName: "output-path",
			SetName = "create",
			HelpText = "The output file path where this config should be written. Depending on the type of file " +
			           "written the appropriate file extension will be used (.json or .properties).",
			Default = "Genesis.CLI")]
		public string CreatePath { get; set; }

		[Option(
			"plugin-path",
			HelpText = "The path to the plugin folder.",
			Default = "Plugins")]
		public string PluginPath { get; set; }

		[Option(
			"verbose",
			HelpText = "Sets the logging to be verbose if true, errors only if false.",
			Default = false)]
		public bool IsVerbose { get; set; }

		[Option(
			"load-unsafe",
			HelpText = "Forces out-of-date plugins to be loaded",
			Default = false)]
		public bool DoLoadUnsafe { get; set; }

		[Option(
			"use-properties-file",
			HelpText = "Creates a properties config file rather than a json file.",
			Default = false)]
		public bool UsePropertiesFile { get; set; }
	}
}
