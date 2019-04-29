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
		private static readonly string[] Original = {"The", Existing, "In", "Spain"};

		// This is the modifiable list constructed at the beginning of each
		// test.
		private List<string> pathList;

		// This is the new path that is supposed to be added by each test.
		private const string TestPath = "TestPath";

		// This is a path that is already on the list, and which is either to
		// be deleted or is there to check that duplicates are not added.
		private const string Existing = "Rain";

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
			// Verify that the SUT deletes the requested item from somewhere
			// in the middle of the User hive.

			// Arrange
			EditItem sut = new EditItem(Existing, EditItem.Action.Delete);

			// Act
			sut.Execute(Hive.User, pathList);
			
			// Assert
			string[] expected = {Original[0], Original[2], Original[3]};
			CollectionAssert.AreEqual(expected, pathList);
		}

		[Test]
		public void Execute_Delete_DeletesFromSystem()
		{
			// Verify that the SUT deletes the requested item from somewhere
			// in the middle of the System hive.

			// Arrange
			EditItem sut = new EditItem(Existing, EditItem.Action.Delete);

			// Act
			sut.Execute(Hive.System, pathList);
			
			// Assert
			string[] expected = {Original[0], Original[2], Original[3]};
			CollectionAssert.AreEqual(expected, pathList);
		}

		[Test]
		public void Execute_DeleteNullPath_DeletesNothing()
		{
			// Verify that a null entry in a hive is not deleted.

			// Arrange
			EditItem sut = new EditItem(Existing, EditItem.Action.Delete);

			// Act
			pathList[1] = null;
			sut.Execute(Hive.System, pathList);

			// Assert
			string[] expected = { Original[0], null, Original[2], Original[3] };
			CollectionAssert.AreEqual(expected, pathList);
		}

		[Test]
		public void Execute_DeleteNullItem_DeletesNothing()
		{
			// Verify that a command to delete a null entry in a hive deletes nothing.

			// Arrange
			EditItem sut = new EditItem(path: null, EditItem.Action.Delete);

			// Act
			sut.Execute(Hive.System, pathList);

			// Assert
			CollectionAssert.AreEqual(Original, pathList);
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

		[Test]
		public void Execute_AddSystemBeginAlready_DoesNotAdd()
		{
			// Verify that the SUT does not add a string to the beginning of
			// system the hive if it is already in the hive.

			// Arrange
			EditItem sut = new EditItem(Existing, EditItem.Action.Add,
				Hive.System, EditItem.Location.Beginning);

			// Act
			sut.Execute(Hive.System, pathList);

			// Assert
			CollectionAssert.AreEqual(Original, pathList);
		}

		[Test]
		public void Execute_AddSystemEndAlready_DoesNotAdd()
		{
			// Verify that the SUT does not add a string to the end of the
			// system hive if it is already in the hive.

			// Arrange
			EditItem sut = new EditItem(Existing, EditItem.Action.Add,
				Hive.System, EditItem.Location.End);

			// Act
			sut.Execute(Hive.System, pathList);

			// Assert
			CollectionAssert.AreEqual(Original, pathList);
		}

		[Test]
		public void Execute_AddUserBeginAlready_DoesNotAdd()
		{
			// Verify that the SUT does not add a string to the beginning of
			// the user hive if it is already in the hive.

			// Arrange
			EditItem sut = new EditItem(Existing, EditItem.Action.Add,
				Hive.User, EditItem.Location.Beginning);

			// Act
			sut.Execute(Hive.User, pathList);

			// Assert
			CollectionAssert.AreEqual(Original, pathList);
		}

		[Test]
		public void Execute_AddUserEndAlready_DoesNotAdd()
		{
			// Verify that the SUT does not add a string to the end of
			// the user hive if it is already in the hive.

			// Arrange
			EditItem sut = new EditItem(Existing, EditItem.Action.Add,
				Hive.User, EditItem.Location.End);

			// Act
			sut.Execute(Hive.User, pathList);

			// Assert
			CollectionAssert.AreEqual(Original, pathList);
		}
	}
}
