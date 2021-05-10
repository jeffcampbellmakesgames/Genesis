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

using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Genesis.Plugin
{
	public static class ClassDeclarationSyntaxExtensions
	{
		public static IEnumerable<MemberDeclarationSyntax> GetMembersFromAllParts(this ClassDeclarationSyntax type,
			SemanticModel model)
		{
			var typeSymbol = model.GetDeclaredSymbol(type);
			if (typeSymbol.IsErrorType())
				return null;
			var allTypeDeclarations = typeSymbol.DeclaringSyntaxReferences
				.Select(sr => sr.GetSyntax())
				.OfType<ClassDeclarationSyntax>();
			if (allTypeDeclarations.Any())
				return allTypeDeclarations.SelectMany(t => t.Members);
			return null;
		}
	}
}
