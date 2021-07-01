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
using System.Linq;
using NUnit.Framework;
// ReSharper disable All
#pragma warning disable

namespace Genesis.Plugin.Tests
{
	[TestFixture]
	[Category(TestConstants.CATEGORY_CODE_ANALYSIS)]
	public class ITypeSymbolExtensionsTests
	{
		[Test]
		public void FullTypeNameCanBeFoundForSimpleType()
		{
			var typeSymbol = TestTools.GetSimpleTypeSymbol();

			Assert.AreEqual("Fixtures.CreatureBehaviour", typeSymbol.GetFullTypeName());
		}

		[Test]
		public void HumanReadableNameCanBeFoundForSimpleType()
		{
			var typeSymbol = TestTools.GetSimpleTypeSymbol();

			Assert.AreEqual("CreatureBehaviour", typeSymbol.GetHumanReadableName());
		}

		[Test]
		public void FullTypeNameCanBeFoundForGenericType()
		{
			var typeSymbol = TestTools.GetOpenGenericTypeSymbol();

			Assert.AreEqual("Fixtures.GenericBehaviour<T>", typeSymbol.GetFullTypeName());
		}

		[Test]
		public void FullTypeNameCanBeFoundForGlobalNamespaceType()
		{
			var typeSymbol = TestTools.GetGlobalNamespaceTypeSymbol();

			Assert.AreEqual("CSharpAssemblyClass", typeSymbol.GetFullTypeName());
		}

		[Test]
		public void FullTypeNameCanBeFoundForArrayType()
		{
			var typeSymbol = TestTools.GetArrayTypeSymbol();

			Assert.AreEqual("UnityEngine.GameObject[]", typeSymbol.GetFullTypeName());
		}

		[Test]
		public void FullTypeNameCanBeFoundForMultiDimArrayType()
		{
			var typeSymbol = TestTools.GetMultiDimArrayTypeSymbol();

			Assert.AreEqual("UnityEngine.GameObject[,]", typeSymbol.GetFullTypeName());
		}

		[Test]
		public void FullTypeNameCanBeFoundForMultipleGenericType()
		{
			var typeSymbol = TestTools.GetCompilableStringGenericSymbol();
			var compilableString = typeSymbol.GetFullTypeName();

			Assert.AreEqual(
				"Fixtures.GenericBehaviour<System.Collections.Generic.List<UnityEngine.ScriptableObject>>",
				compilableString);
		}

		[Test]
		public void HumanReadableNameIsCorrectForOpenGenericType()
		{
			var typeSymbol = TestTools.GetOpenGenericTypeSymbol();

			Assert.AreEqual("GenericBehaviourT", typeSymbol.GetHumanReadableName());
		}

		[Test]
		public void HumanReadableNameIsCorrectForClosedGenericType()
		{
			var typeSymbol = TestTools.GetGenericCollectionTypeSymbol();

			Assert.AreEqual("ListGameObject", typeSymbol.GetHumanReadableName());
		}

		[Test]
		public void HumanReadableNameIsCorrectForArrayType()
		{
			var typeSymbol = TestTools.GetArrayTypeSymbol();

			Assert.AreEqual("GameObjectArray", typeSymbol.GetHumanReadableName());
		}

		[Test]
		public void IsArrayTypeWorksCorrectly()
		{
			var normalTypeSymbol = TestTools.GetSimpleTypeSymbol();

			Assert.IsFalse(normalTypeSymbol.IsArrayType());

			var arrayTypeSymbol = TestTools.GetArrayTypeSymbol();

			Assert.IsTrue(arrayTypeSymbol.IsArrayType());
		}

		[Test]
		public void IsGenericTypeWorksCorrectly()
		{
			var normalTypeSymbol = TestTools.GetSimpleTypeSymbol();

			Assert.IsFalse(normalTypeSymbol.IsGenericType());

			var genericTypeSymbol = TestTools.GetOpenGenericTypeSymbol();

			Assert.IsTrue(genericTypeSymbol.IsGenericType());
		}

		[Test]
		public void ImplementsInterfaceCanDetectInterface()
		{
			var typeSymbol = TestTools.GetOpenGenericTypeSymbol();

			Assert.IsTrue(typeSymbol.ImplementsInterface("IFooA"));
			Assert.IsFalse(typeSymbol.ImplementsInterface("IFooB"));
		}

		[Test]
		public void HasAttributeCanDetectAttribute()
		{
			var hasAttrTypeSymbol = TestTools.GetGlobalNamespaceTypeSymbol();
			var hasNoAttrTypeSymbol = TestTools.GetOpenGenericTypeSymbol();

			Assert.IsTrue(hasAttrTypeSymbol.HasAttribute("ExampleAttribute"));
			Assert.IsFalse(hasNoAttrTypeSymbol.HasAttribute("ExampleAttribute"));
		}

		[Test]
		public static void AllClassMembersCanBeFound()
		{
			var genericTypeSymbol = TestTools.GetClosedGenericTypeSymbol();
			var members = genericTypeSymbol.GetAllMembers();

			Assert.True(members.Any(x => x.Name == "FooA"));
			Assert.True(members.Any(x => x.Name == "FooB"));
			Assert.True(members.Any(x => x.Name == "Bar"));
		}

		[Test]
		public static void CanFindAssemblyName()
		{
			var typeSymbol = TestTools.GetRuntimeTypeSymbol();

			Assert.AreEqual("Runtime", typeSymbol.GetAssemblyName());

			var editorTypeSymbol = TestTools.GetEditorTypeSymbol();

			Assert.AreEqual("Editor", editorTypeSymbol.GetAssemblyName());
		}

		[Test]
		public static void CanFindAllInterfaces()
		{
			var openGenericType = TestTools.GetOpenGenericTypeSymbol();
			var openInterfaces = openGenericType.GetAllInterfacesIncludingThis();

			Assert.IsTrue(openInterfaces.Any(x => x.Name == "IFooA"));
			Assert.IsFalse(openInterfaces.Any(x => x.Name == "IFooB"));

			var closedGenericType = TestTools.GetClosedGenericTypeSymbol();
			var closedInterfaces = closedGenericType.GetAllInterfacesIncludingThis();

			Assert.True(closedInterfaces.Any(x => x.Name == "IFooA"));
			Assert.True(closedInterfaces.Any(x => x.Name == "IFooB"));
		}

		[Test]
		public static void CanDetermineIfTypeImplementsInterface()
		{
			var openGenericType = TestTools.GetOpenGenericTypeSymbol();

			Assert.IsTrue(openGenericType.ImplementsInterface("IFooA"));
			Assert.IsFalse(openGenericType.ImplementsInterface("IFooB"));

			var closedGenericType = TestTools.GetClosedGenericTypeSymbol();

			Assert.IsTrue(closedGenericType.ImplementsInterface("IFooA"));
			Assert.IsTrue(closedGenericType.ImplementsInterface("IFooB"));
		}

		[Test]
		public static void CanDetectAbstractClassType()
		{
			var openGenericType = TestTools.GetOpenGenericTypeSymbol();

			Assert.IsTrue(openGenericType.IsAbstractClass());

			var closedGenericType = TestTools.GetClosedGenericTypeSymbol();

			Assert.IsFalse(closedGenericType.IsAbstractClass());
		}

		[Test]
		public static void CanDetectNullableType()
		{
			var classMemberTypeSymbol = TestTools.GetClassMembersTypeSymbol();
			var allMembers = classMemberTypeSymbol.GetMembers();
			var nullableInt1Field = allMembers.First(x => x.Name == "nullableInt1");
			var nullableInt2Field = allMembers.First(x => x.Name == "nullableInt2");

			Assert.IsTrue(nullableInt1Field.IsNullableType());
			Assert.IsTrue(nullableInt2Field.IsNullableType());

			var nullableInt1Property = allMembers.First(x => x.Name == "NullableInt1");
			var nullableInt2Property = allMembers.First(x => x.Name == "NullableInt2");

			Assert.IsTrue(nullableInt1Property.IsNullableType());
			Assert.IsTrue(nullableInt2Property.IsNullableType());

			var nullableIntMethod = allMembers.First(x => x.Name == "NullableMethod");

			Assert.IsTrue(nullableIntMethod.IsNullableType());
		}

		[Test]
		public static void CanDetectDelegateType()
		{
			var delegateTypeSymbol = TestTools.GetDelegateTypeSymbol();

			Assert.IsTrue(delegateTypeSymbol.IsDelegateType());
		}

		[Test]
		public static void CanDetectNestedType()
		{
			var nestedTypeSymbol = TestTools.GetNestedTypeSymbol();

			Assert.IsTrue(nestedTypeSymbol.IsNested());
		}

		[Test]
		public static void CanGetAllContainingTypesAndThis()
		{
			var nestedTypeSymbol = TestTools.GetNestedTypeSymbol();
			var containingTypesAndThis = nestedTypeSymbol.GetContainingTypesAndThis().ToArray();

			Assert.IsTrue(containingTypesAndThis.Any(x => x.Name == "NestedClassType"));
			Assert.IsTrue(containingTypesAndThis.Any(x => x.Name == "OuterClassType"));
		}

		[Test]
		public static void CanDetectTypeInheritanceForGeneric()
		{
			var closedGenericTypeSymbol = TestTools.GetClosedGenericTypeSymbol();

			Assert.IsTrue(closedGenericTypeSymbol.InheritsFrom("UnityEngine.MonoBehaviour"));
			Assert.IsTrue(closedGenericTypeSymbol.InheritsFrom("Fixtures.GenericBehaviour<int>"));
			Assert.IsFalse(closedGenericTypeSymbol.InheritsFrom("Fixtures.ClosedGenericBehaviour"));
		}

		[Test]
		public static void CanDetectTypeInheritanceOsIsForGeneric()
		{
			var closedGenericTypeSymbol = TestTools.GetClosedGenericTypeSymbol();

			Assert.IsTrue(closedGenericTypeSymbol.InheritsFromOrIs("UnityEngine.MonoBehaviour"));
			Assert.IsTrue(closedGenericTypeSymbol.InheritsFromOrIs("Fixtures.GenericBehaviour<int>"));
			Assert.IsTrue(closedGenericTypeSymbol.InheritsFromOrIs("Fixtures.ClosedGenericBehaviour"));
		}

		[Test]
		public static void CanDetectListType()
		{
			var arrayTypeSymbol = TestTools.GetArrayTypeSymbol();
			var listTypeSymbol = TestTools.GetGenericListTypeSymbol();

			Assert.IsFalse(arrayTypeSymbol.IsList(out var nonListElementTypeSymbol));
			Assert.IsTrue(listTypeSymbol.IsList(out var elementTypeSymbol));
			Assert.AreEqual("UnityEngine.GameObject", elementTypeSymbol.GetFullTypeName());
		}

		[Test]
		public static void CanDetectDictionaryType()
		{
			var arrayTypeSymbol = TestTools.GetArrayTypeSymbol();
			var dictionaryTypeSymbol = TestTools.GetGenericDictionaryTypeSymbol();

			Assert.IsFalse(arrayTypeSymbol.IsDictionary(out var nonKeyTypeSymbol, out var nonValueTypeSymbol));
			Assert.IsTrue(dictionaryTypeSymbol.IsDictionary(out var keyTypeSymbol, out var valueTypeSymbol));
			Assert.AreEqual("int", keyTypeSymbol.GetFullTypeName());
			Assert.AreEqual("UnityEngine.GameObject", valueTypeSymbol.GetFullTypeName());
		}

		[Test]
		public static void CanDetectArrayType()
		{
			var arrayTypeSymbol = TestTools.GetArrayTypeSymbol();
			var dictionaryTypeSymbol = TestTools.GetGenericDictionaryTypeSymbol();
			Assert.IsFalse(dictionaryTypeSymbol.IsArray(out var nonElementTypeSymbol));
			Assert.IsTrue(arrayTypeSymbol.IsArray(out var elementTypeSymbol));
			Assert.AreEqual("UnityEngine.GameObject", elementTypeSymbol.GetFullTypeName());
		}

		[Test]
		public static void CanDetectCloneableImplementation()
		{
			var cloneableTypeSymbol = TestTools.GetCloneableClassTypeSymbol();

			Assert.IsTrue(cloneableTypeSymbol.IsCloneable());
		}

		[Test]
		public static void CanDetectDefaultConstructor()
		{
			var defaultConstructorClassTypeSymbol = TestTools.GetCloneableClassTypeSymbol();
			var noDefaultConstructorClassTypeSymbol = TestTools.GetClassTypeSymbolWithNoDefaultConstructor();

			Assert.IsTrue(defaultConstructorClassTypeSymbol.HasDefaultConstructor());
			Assert.IsFalse(noDefaultConstructorClassTypeSymbol.HasDefaultConstructor());
		}

		[Test]
		public static void CanDetectCopyConstructor()
		{
			var copyConstructorClassTypeSymbol = TestTools.GetClassTypeSymbolWithNoDefaultConstructor();
			var defaultConstructorClassTypeSymbol = TestTools.GetCloneableClassTypeSymbol();

			Assert.IsTrue(copyConstructorClassTypeSymbol.HasCopyConstructor());
			Assert.IsFalse(defaultConstructorClassTypeSymbol.HasCopyConstructor());
		}

		[Test]
		public static void CanDetectMutableReferenceType()
		{
			var stringTypeSymbol = TestTools.GetStringTypeSymbol();
			var arrayTypeSymbol = TestTools.GetArrayTypeSymbol();
			var listTypeSymbol = TestTools.GetGenericListTypeSymbol();

			Assert.IsFalse(stringTypeSymbol.IsMutableReferenceType());
			Assert.IsTrue(arrayTypeSymbol.IsMutableReferenceType());
			Assert.IsTrue(listTypeSymbol.IsMutableReferenceType());
		}

		[Test]
		public static void CanGetAllContainingTypes()
		{
			var nestedTypeSymbol = TestTools.GetNestedTypeSymbol();
			var containingTypesAndThis = nestedTypeSymbol.GetContainingTypes().ToArray();

			Assert.IsFalse(containingTypesAndThis.Any(x => x.Name == "NestedClassType"));
			Assert.IsTrue(containingTypesAndThis.Any(x => x.Name == "OuterClassType"));
		}

		[Test]
		public static void CanDetectAttributeType()
		{
			var nestedTypeSymbol = TestTools.GetAttributeTypeSymbol();

			Assert.IsTrue(nestedTypeSymbol.IsAttribute());
		}

		[Test]
		public static void CanDetectAttributeTypeThatIsInherited()
		{
			var typeSymbol = TestTools.GetDerivedAttributeClassTypeSymbol();

			Assert.IsFalse(typeSymbol.HasAttribute("BaseAttribute"));
			Assert.IsTrue(typeSymbol.HasAttribute("BaseAttribute", canInherit:true));
		}

		[Test]
		public static void CanGetAttributeTypeThatIsInherited()
		{
			var typeSymbol = TestTools.GetDerivedAttributeClassTypeSymbol();

			Assert.IsEmpty(typeSymbol.GetAttributes("BaseAttribute"));

			var attrData = typeSymbol.GetAttributes("BaseAttribute", canInherit: true);
			Assert.IsNotEmpty(attrData);

			var firstAttr = attrData.Single();

			Assert.AreEqual("FooDerivedAttribute", firstAttr.AttributeClass.Name);
		}

		[Test]
		public static void CanDetectAndFilterAttributesByTypeName()
		{
			var enumTypeSymbol = TestTools.GetEnumTypeSymbol();
			var allFactoryAttributes = enumTypeSymbol.GetAttributes("FactoryKeyEnumFor");

			Assert.IsTrue(allFactoryAttributes.All(x => x.AttributeClass.Name == "FactoryKeyEnumFor"));
		}

		[Test]
		public static void CanGetAllBaseTypes()
		{
			var typeSymbol = TestTools.GetSimpleTypeSymbol();
			var baseTypesAndThis = typeSymbol.GetBaseTypesAndThis();

			Assert.IsTrue(baseTypesAndThis.Any(x => x.Name == "CreatureBehaviour"));
			Assert.IsTrue(baseTypesAndThis.Any(x => x.Name == "MonoBehaviour"));
			Assert.IsTrue(baseTypesAndThis.Any(x => x.Name == "Behaviour"));
			Assert.IsTrue(baseTypesAndThis.Any(x => x.Name == "Component"));
			Assert.IsTrue(baseTypesAndThis.Any(x => x.Name == "Object"));
		}
	}
}
#pragma warning restore
