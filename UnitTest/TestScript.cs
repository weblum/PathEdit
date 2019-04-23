//---------------------------------------------------------------------------
// FILE NAME: TestScript.cs
// GREETING:  Happy Earth Day
// DATE:      Monday, April 22, 2019   9 pm
// WEATHER:   Fair, Temp 67°F, Pressure 29.99",
//            Humidity 59%, Wind calm
// Programmer's Cuvee XLV
// Copyright (C) 2019 William E. Blum.  All rights reserved.
//---------------------------------------------------------------------------

using System.Collections.Generic;
using NUnit.Framework;
using PathEdit;

namespace UnitTest
{
	[TestFixture]
	public class TestScript
	{
		private const string TestPath = "TestPath";
		private static readonly string[] SysOrig = {"SysAlpha", "SysBeta"};
		private static readonly string[] UseOrig = {"UseAlpha", "UseBeta"};

		private FakeRegistryEditor editor;
		private Script sut;

		[Test]
		public void Execute_AddToBothHives_BothHivesGetNew()
		{
			// Arrange
			editor = new FakeRegistryEditor();
			editor.SetSysStrings(SysOrig);
			editor.SetUseStrings(UseOrig);
			sut = new Script(editor);
			EditItem ed1 = new EditItem(TestPath, EditItem.Action.Add, Hive.System, EditItem.Location.Beginning);
			EditItem ed2 = new EditItem(TestPath, EditItem.Action.Add, Hive.User, EditItem.Location.Beginning);
			EditItem[] items = {ed1, ed2};

			// Act
			sut.Execute(items);

			// Assert
			var expectedSys = new List<string> {TestPath};
			expectedSys.AddRange(SysOrig);

			var expectedUser = new List<string> {TestPath};
			expectedUser.AddRange(UseOrig);

			var actualSys = editor.ReceivedSystem;
			var actualUser = editor.ReceivedUser;

			CollectionAssert.AreEqual(expectedSys, actualSys);
			CollectionAssert.AreEqual(expectedUser, actualUser);
		}

		[Test]
		public void Execute_DeleteFromBoth_RemovedFromEach()
		{
			// Arrange
			editor = new FakeRegistryEditor();
			editor.SetSysStrings(new []{"aa", "kk", "bb"});
			editor.SetUseStrings(new []{"aa", "kk", "bb"});
			sut = new Script(editor);
			EditItem item = new EditItem("kk", EditItem.Action.Delete);
			EditItem[] items = {item};

			// Act
			sut.Execute(items);

			// Assert
			var expected = new[] {"aa", "bb"};
			var actual1 = editor.ReceivedSystem;
			var actual2 = editor.ReceivedUser;
			CollectionAssert.AreEqual(expected, actual1);
			CollectionAssert.AreEqual(expected, actual2);
		}
	}
}