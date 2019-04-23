using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PathEdit;

namespace UnitTest
{
	[TestFixture]
	public class TestEditItem
	{
		// This is the original list of paths at the beginning of each test.
		private static readonly string[] Original = {"The", ToDelete, "In", "Spain"};

		// This is the modifiable list constructed at the beginning of each
		// test.
		private List<string> pathList;

		// This is the new path that is supposed to be added by each test.
		private const string TestPath = "TestPath";

		// This is the old path that is supposed to be deleted.
		private const string ToDelete = "Rain";

		[SetUp]
		public void MakePathList()
		{
			// Our setup consists of being sure the modifiable list of paths
			// starts out the same at the beginning of each test.
			pathList = new List<string>(Original);
		}

		[Test]
		public void Execute_Delete_DeletesFromUser()
		{
			// Arrange
			EditItem sut = new EditItem(ToDelete, EditItem.Action.Delete);

			// Act
			sut.Execute(Hive.User, pathList);
			
			// Assert
			string[] expected = {Original[0], Original[2], Original[3]};
			CollectionAssert.AreEqual(expected, pathList);
		}

		[Test]
		public void Execute_Delete_DeletesFromSystem()
		{
			// Arrange
			EditItem sut = new EditItem(ToDelete, EditItem.Action.Delete);

			// Act
			sut.Execute(Hive.System, pathList);
			
			// Assert
			string[] expected = {Original[0], Original[2], Original[3]};
			CollectionAssert.AreEqual(expected, pathList);
		}

		[Test]
		public void Execute_AddUserBegin_AddsToUser()
		{
			// Verify that the SUT adds the path to the beginning of the User
			// hive and does not remove or change the order of what was
			// there.

			// Arrange
			EditItem sut = new EditItem(TestPath, EditItem.Action.Add, 
				Hive.User, EditItem.Location.Beginning);

			// Act
			sut.Execute(Hive.User, pathList);

			// Assert
			string first = pathList.First();
			Assert.That(first, Is.EqualTo(TestPath));
			IEnumerable<string> theRest = pathList.Skip(1);
			CollectionAssert.AreEqual(Original, theRest);
		}

		[Test]
		public void Execute_AddUserBegin_NoAddToSystem()
		{
			// Verify that the SUT does not molest the hive it is supposed to
			// leave alone.

			// Arrange
			EditItem sut = new EditItem(TestPath, EditItem.Action.Add,
				Hive.User, EditItem.Location.Beginning);

			// Act
			sut.Execute(Hive.System, pathList);

			// Assert
			CollectionAssert.AreEqual(Original, pathList);
		}

		[Test]
		public void Execute_AddUserEnd_AddsToUser()
		{
			// Verify that the SUT adds the path to the end of the User hive
			// and does not remove or change the order of what was there.

			// Arrange
			EditItem sut = new EditItem(TestPath, EditItem.Action.Add,
				Hive.User, EditItem.Location.End);

			// Act
			sut.Execute(Hive.User, pathList);

			// Assert
			string last = pathList.Last();
			Assert.That(last, Is.EqualTo(TestPath));
			int allButLast = pathList.Count - 1;
			IEnumerable<string> theRest = pathList.Take(allButLast);
			CollectionAssert.AreEqual(Original, theRest);
		}

		[Test]
		public void Execute_AddUserEnd_NoAddToSystem()
		{
			// Verify that the SUT does not molest the hive it is supposed to
			// leave alone.

			// Arrange
			EditItem sut = new EditItem(TestPath, EditItem.Action.Add,
				Hive.User, EditItem.Location.End);

			// Act
			sut.Execute(Hive.System, pathList);

			// Assert
			CollectionAssert.AreEqual(Original, pathList);
		}

		[Test]
		public void Execute_AddSystemBegin_AddsToSystem()
		{
			// Verify that the SUT adds the path to the beginning of the
			// System hive and does not remove or change the order of what
			// was there.

			// Arrange
			EditItem sut = new EditItem(TestPath, EditItem.Action.Add,
				Hive.System, EditItem.Location.Beginning);

			// Act
			sut.Execute(Hive.System, pathList);

			// Assert
			string first = pathList.First();
			Assert.That(first, Is.EqualTo(TestPath));
			IEnumerable<string> theRest = pathList.Skip(1);
			CollectionAssert.AreEqual(Original, theRest);
		}

		[Test]
		public void Execute_AddSystemBegin_NoAddToUser()
		{
			// Verify that the SUT does not molest the hive it is supposed to
			// leave alone.

			// Arrange
			EditItem sut = new EditItem(TestPath, EditItem.Action.Add,
				Hive.System, EditItem.Location.Beginning);

			// Act
			sut.Execute(Hive.User, pathList);

			// Assert
			CollectionAssert.AreEqual(Original, pathList);
		}

		[Test]
		public void Execute_AddSystemEnd_AddsToSystem()
		{
			// Verify that the SUT adds the path to the end of the System
			// hive and does not remove or change the order of what was
			// there.

			// Arrange
			EditItem sut = new EditItem(TestPath, EditItem.Action.Add,
				Hive.System, EditItem.Location.End);

			// Act
			sut.Execute(Hive.System, pathList);

			// Assert
			string last = pathList.Last();
			Assert.That(last, Is.EqualTo(TestPath));
			int allButLast = pathList.Count - 1;
			IEnumerable<string> theRest = pathList.Take(allButLast);
			CollectionAssert.AreEqual(Original, theRest);
		}

		[Test]
		public void Execute_AddSystemEnd_NoAddToUser()
		{
			// Verify that the SUT does not molest the hive it is supposed to
			// leave alone.

			// Arrange
			EditItem sut = new EditItem(TestPath, EditItem.Action.Add,
				Hive.System, EditItem.Location.End);

			// Act
			sut.Execute(Hive.User, pathList);

			// Assert
			CollectionAssert.AreEqual(Original, pathList);
		}
	}
}
