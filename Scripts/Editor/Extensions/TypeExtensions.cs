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
using System.Reflection;
using UnityEngine.Assertions;

namespace JCMG.Genesis.Editor
{
	/// <summary>
	/// Extension methods for <see cref="Type"/>.
	/// </summary>
	public static class TypeExtensions
	{
		/// <summary>
		/// Returns true if <typeparamref name="T"/> implements interface <paramref name="type"/>.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="type"></param>
		public static bool ImplementsInterface<T>(this Type type)
		{
			Assert.IsTrue(type.IsInterface);

			return type.GetInterface(typeof(T).FullName) != null;
		}

		/// <summary>
		/// Returns a list of <see cref="PublicMemberInfo"/> instances for all public members on this
		/// <paramref name="type"/>.
		/// </summary>
		/// <param name="type"></param>
		public static List<PublicMemberInfo> GetPublicMemberInfos(this Type type)
		{
			var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public);
			var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
			var publicMemberInfoList = new List<PublicMemberInfo>(fields.Length + properties.Length);
			for (var index = 0; index < fields.Length; ++index)
			{
				publicMemberInfoList.Add(new PublicMemberInfo(fields[index]));
			}

			for (var index = 0; index < properties.Length; ++index)
			{
				var info = properties[index];
				if (info.CanRead && info.CanWrite && info.GetIndexParameters().Length == 0)
				{
					publicMemberInfoList.Add(new PublicMemberInfo(info));
				}
			}

			return publicMemberInfoList;
		}
	}
}
