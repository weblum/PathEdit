//---------------------------------------------------------------------------
// FILE NAME: TestParser.cs
// DATE:      Saturday, April 20, 2019   8 pm
// WEATHER:   Mostly Cloudy with Haze, Temp 52°F, Pressure 29.97",
//            Humidity 77%, Wind 10.4 mph from the Southwest
// Programmer's Cuvee XLV
// Copyright (C) 2019 William E. Blum.  All rights reserved.
//---------------------------------------------------------------------------

using System;
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
			const string Item = "TestItem";
			string[] items =
			{
				$"+{Item}"
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

            var expected = new EditItem(Item, EditItem.Action.Add, DefaultHive, DefaultLocation);

			Assert.That(actual, Is.EqualTo(expected));
		}

		[Test]
		public void Parse_DeleteItem_CreatesDefaultDelete()
		{
            // This test has an unexpected outcome, but it is explained when you know
            // that the EditItem is defective in the case of a deletion. Namely, the
            // hive and location are indeterminate and unused in the update algorithm.
            // Hence, they should be ignored if one is not to be confused.

			// Arrange
			Parser sut = new Parser();
			const string Item = "TestItem";

            string[] items =
			{
				$"-{Item}"
			};

            // Act
			IEnumerable<EditItem> enumerable = sut.Parse(items);
			var actual = enumerable.First();

            // Assert
            // The defaults returned from the parser are not necessarily the
            // default values of the EditItem. Here are the defaults we
            // expect from the Parser, which are inexplicably different
            // depending on whether we are adding or deleting:
            const EditItem.Location DefaultLocation = EditItem.Location.Beginning;
            const Hive DefaultHive = Hive.System;

            var expected = new EditItem(Item, EditItem.Action.Delete, DefaultHive, DefaultLocation);

            Assert.That(actual, Is.EqualTo(expected));
		}

		[TestCase("b", Hive.User, EditItem.Location.Beginning)]
		[TestCase("e", Hive.User, EditItem.Location.End)]

        [TestCase("u,b", Hive.User, EditItem.Location.Beginning)]
        [TestCase("b,u", Hive.User, EditItem.Location.Beginning)]

        [TestCase("u,e", Hive.User, EditItem.Location.End)]
        [TestCase("e,u", Hive.User, EditItem.Location.End)]

        [TestCase("s,b", Hive.System, EditItem.Location.Beginning)]
        [TestCase("b,s", Hive.System, EditItem.Location.Beginning)]

        [TestCase("s,e", Hive.System, EditItem.Location.End)]
		[TestCase("e,s", Hive.System, EditItem.Location.End)]

        [TestCase("sierra,echo", Hive.System, EditItem.Location.End)]
        [TestCase("echo,sierra", Hive.System, EditItem.Location.End)]
		public void Parse_SpecifyHiveLoc_CreatesCorrespondingAdd(string tok, Hive hive, EditItem.Location loc)
		{
			// Arrange
			Parser sut = new Parser();
			const string Item = "TestItem";

            string[] items =
			{
                $"[{tok}]",
				$"+{Item}"
			};

            // Act
			IEnumerable<EditItem> enumerable = sut.Parse(items);
			var actual = enumerable.First();

            // Assert
            var expected = new EditItem(Item, EditItem.Action.Add, hive, loc);

            Assert.That(actual, Is.EqualTo(expected));
		}

        [Test]
        public void Parse_MissingOpenBracked_Throws()
        {
            Parser sut = new Parser();

            string[] items =
            {
                "e,s]"
            };

            // Act
            Assert.Throws<Exception>(() => sut.Parse(items));
        }

        [Test]
        public void Parse_MissingCloseBracked_Throws()
        {
            Parser sut = new Parser();

            string[] items =
            {
                "[e,s"
            };

            // Act
            Assert.Throws<Exception>(() => sut.Parse(items));
        }

        [Test]
        public void Parse_SpaceInToken_Throws()
        {
            // I am not sure why a space in the token is required to throw an
            // exception, but that is apparently the requirement.
            // Arrange
            Parser sut = new Parser();

            string[] items =
            {
                "[e, s]"
            };

            // Act
            Assert.Throws<Exception>(() => sut.Parse(items));
        }
    }
}