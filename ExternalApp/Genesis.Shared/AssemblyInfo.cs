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

using System.Reflection;
using System.Runtime.CompilerServices;

// External Assemblies
[assembly: InternalsVisibleTo("Genesis.CLI")]
[assembly: InternalsVisibleTo("Genesis.Plugin")]
[assembly: InternalsVisibleTo("Genesis.Plugin.Tests")]

// Unity Assemblies
[assembly: InternalsVisibleTo("Genesis.Editor")]
[assembly: InternalsVisibleTo("Genesis.Editor.Tests")]

[assembly: AssemblyVersion("2.0.6.0")]
[assembly: AssemblyFileVersion("2.0.6")]
[assembly: AssemblyInformationalVersion("2.0.6-alpha.37+Branch.develop.Sha.c558a4635cffe9068e7c4319c1c15d3567298988")]
