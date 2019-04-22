//---------------------------------------------------------------------------
// FILE NAME: TestParser.cs
// DATE:      Saturday, April 20, 2019   8 pm
// WEATHER:   Mostly Cloudy with Haze, Temp 52°F, Pressure 29.97",
//            Humidity 77%, Wind 10.4 mph from the Southwest
// Programmer's Cuvee XLV
// Copyright (C) 2019 William E. Blum.  All rights reserved.
//---------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PathEdit;

namespace UnitTest
{
	[TestFixture]
	public class TestParser
	{
		[Test]
		public void Parse_AddItem_CreatesDefaultAdd()
		{
			// Arrange
			Parser sut = new Parser();
			const string item = "TestItem";
			string[] items =
				{
					$"+{item}"
				};

			// Act
			IEnumerable<EditItem> enumerable = sut.Parse(items);
			var actual = enumerable.First();

            // Assert
            // The defaults returned from the parser are not necessarily the
            // default values of the EditItem. Here are the defaults we
            // expect from the Parser:
            const EditItem.Location DefaultLocation = EditItem.Location.Beginning;
            const Hive DefaultHive = Hive.User;

            var expected = new EditItem(item, EditItem.Action.Add, DefaultHive, DefaultLocation);

			Assert.That(actual, Is.EqualTo(expected));
		}

		[Test]
		public void Parse_DeleteItem_CreatesDefaultDelete()
		{
			Assert.Inconclusive();
			// Arrange
			// Act
			// Assert
		}

	}
}