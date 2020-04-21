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

namespace JCMG.Genesis.Editor
{
	/// <summary>
	/// A logger that outputs its content to the Unity Console window.
	/// </summary>
	internal sealed class UnityLogger : ILogger
	{
		public string Name { get; }

		public UnityLogger(string name)
		{
			Name = name;
		}

		public void Trace(string message)
		{
			Log(LogLevel.Trace, message);
		}

		public void Debug(string message)
		{
			Log(LogLevel.Debug, message);
		}

		public void Info(string message)
		{
			Log(LogLevel.Info, message);
		}

		public void Warn(string message)
		{
			Log(LogLevel.Warn, message);
		}

		public void Error(string message)
		{
			Log(LogLevel.Error, message);
		}

		public void Error(Exception exception, string message)
		{
			Log(LogLevel.Error, message, exception);
		}

		public void Fatal(string message)
		{
			Log(LogLevel.Fatal, message);
		}

		public void Fatal(Exception exception, string message)
		{
			Log(LogLevel.Fatal, message, exception);
		}

		public void Log(LogLevel logLvl, string message, Exception exception = null)
		{
			if (GenesisPreferences.PreferredLogLevel > logLvl)
			{
				return;
			}

			switch (logLvl)
			{
				case LogLevel.On:
				case LogLevel.Trace:
				case LogLevel.Debug:
				case LogLevel.Info:
					UnityEngine.Debug.LogFormat(
						EditorConstants.CODE_GENERATION_UPDATE,
						Name,
						message);
					break;

				case LogLevel.Warn:
					UnityEngine.Debug.LogWarningFormat(
						EditorConstants.CODE_GENERATION_UPDATE,
						Name,
						message);
					break;

				case LogLevel.Error:
				case LogLevel.Fatal:
					if (exception == null)
					{
						UnityEngine.Debug.LogErrorFormat(
							EditorConstants.CODE_GENERATION_UPDATE,
							Name,
							message);
					}
					else
					{
						UnityEngine.Debug.LogErrorFormat(
							EditorConstants.CODE_GENERATION_UPDATE,
							Name,
							message + "\n\n" + exception.ToString());
					}

					break;

				case LogLevel.Off:
					// No-Op
					break;

				default:
					throw new ArgumentOutOfRangeException(nameof(logLvl), logLvl, null);
			}
		}
	}
}
