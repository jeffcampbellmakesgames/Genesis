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
using System.Collections.Generic;

namespace JCMG.Genesis.Editor
{
	/// <summary>
	/// A static factory for creating <see cref="ILogger"/> instances.
	/// </summary>
	public static class Log
	{
		private static readonly Dictionary<string, ILogger> LOGGERS;

		static Log()
		{
			LOGGERS = new Dictionary<string, ILogger>();
		}

		/// <summary>
		/// Returns an <see cref="ILogger"/> instance with context set for <paramref name="type"/>.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static ILogger GetLogger(Type type)
		{
			return GetLogger(type.FullName);
		}

		/// <summary>
		/// Returns an <see cref="ILogger"/> instance with context set for <typeparamref name="T"/>.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static ILogger GetLogger<T>()
		{
			return GetLogger(typeof(T).FullName);
		}

		/// <summary>
		/// Returns an <see cref="ILogger"/> instance with context set for <paramref name="name"/>.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public static ILogger GetLogger(string name)
		{
			if (!LOGGERS.TryGetValue(name, out var logger))
			{
				logger = new UnityLogger(name);
				LOGGERS.Add(name, logger);
			}

			return logger;
		}

		/// <summary>
		/// Clears all cached instances of <see cref="ILogger"/>.
		/// </summary>
		public static void ResetLoggers()
		{
			LOGGERS.Clear();
		}
	}
}
