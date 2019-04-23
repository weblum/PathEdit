//---------------------------------------------------------------------------
// FILE NAME: MainWindow.xaml.cs
// DATE:      Sunday, September 27, 2015   2 pm
// WEATHER:   Not available.
// Programmer's Cuvee XLI
// Copyright (C) 2015 William E. Blum.  All rights reserved.
//---------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace PathEdit
{
	public partial class MainWindow
	{
		private const string Caption = "Unexpected Happening";
		private readonly string originalTitle;
		private readonly DispatcherTimer timer = new DispatcherTimer();
		private List<string> cleanData;
		private ObservableCollection<string> data;
		private Hive hive = Hive.User;

		public MainWindow()
		{
			InitializeComponent();
			originalTitle = Title;
			ReadCurrentValues();
			EnableButtons();
			timer.Interval = TimeSpan.FromSeconds(2);
			timer.Tick += Timer_Tick;
			timer.Start();
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			if (UserDeclinesToAbandon())
				e.Cancel = true;

			base.OnClosing(e);
		}

		#region Button Handlers

		private void OnAddClick(object sender, RoutedEventArgs e)
		{
			var dlg = new StringEditor {IsNew = true, Owner = this};
			bool okay = dlg.ShowDialog().GetValueOrDefault();

			if (!okay)
				return;

			data.Add(dlg.PathString);
		}

		private void OnEditClick(object sender, RoutedEventArgs e)
		{
			int index = ListBox.SelectedIndex;

			if (index < 0)
			{
				MessageBox.Show("You must select an item to edit",
								"Selection Error", MessageBoxButton.OK,
								MessageBoxImage.Warning);
				return;
			}

			string itemToEdit = data[index];

			var dlg = new StringEditor
				{IsNew = false, Owner = this, PathString = itemToEdit};

			bool okay = dlg.ShowDialog().GetValueOrDefault();

			if (!okay)
				return;

			data[index] = dlg.PathString;
		}

		private void OnDeleteClick(object sender, RoutedEventArgs e)
		{
			int index = ListBox.SelectedIndex;

			if (index < 0)
			{
				MessageBox.Show("You must select an item to delete",
								"Selection Error", MessageBoxButton.OK,
								MessageBoxImage.Warning);
				return;
			}

			data.RemoveAt(index);
		}

		private void OnMoveUpClick(object sender, RoutedEventArgs e)
		{
			int index = ListBox.SelectedIndex;

			if (index < 0)
			{
				MessageBox.Show("You must select an item to move up",
								"Selection Error", MessageBoxButton.OK,
								MessageBoxImage.Warning);
				return;
			}

			if (index == 0)
				return; // because we are already at the top

			string s = data[index];
			data.RemoveAt(index);
			data.Insert(index - 1, s);
			ListBox.SelectedIndex = index - 1;
		}

		private void OnMoveDownClick(object sender, RoutedEventArgs e)
		{
			int index = ListBox.SelectedIndex;

			if (index < 0)
			{
				MessageBox.Show("You must select an item to move down",
								"Selection Error", MessageBoxButton.OK,
								MessageBoxImage.Warning);
				return;
			}

			if (index == data.Count - 1)
				return; // because we are already at the bottom

			string s = data[index];
			data.RemoveAt(index);
			data.Insert(index + 1, s);
			ListBox.SelectedIndex = index + 1;
		}

		private void OnSwitchClick(object sender, RoutedEventArgs e)
		{
			if (UserDeclinesToAbandon())
				return;

			switch (hive)
			{
				case Hive.System:
					hive = Hive.User;
					break;
				case Hive.User:
					hive = Hive.System;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			ReadCurrentValues();
		}
		private void OnMoveClick(object sender, RoutedEventArgs e)
		{
			int index = ListBox.SelectedIndex;

			if (index < 0)
			{
				MessageBox.Show("You must select an item to move",
					"Selection Error", MessageBoxButton.OK,
					MessageBoxImage.Warning);
				return;
			}

			string ItemToMove = data[index];
			data.RemoveAt(index);
			try
			{
				Save();
				OnSwitchClick(sender, e);
				data.Add(ItemToMove);

				StatusText.Text = "Entry moved";
				timer.Start();
				/*
				MessageBox.Show("The operation completed successfully.",
								"Good News",
								MessageBoxButton.OK,
								MessageBoxImage.Information);
				 */
			}
			catch (Exception x)
			{
				MessageBox.Show(x.Message, Caption,
					MessageBoxButton.OK, MessageBoxImage.Warning);
			}
		}


		private void OnSaveClick(object sender, RoutedEventArgs e)
		{
			try
			{
				Save();
				StatusText.Text = "Hive saved";
				timer.Start();
				/*
				MessageBox.Show("The operation completed successfully.",
								"Good News",
								MessageBoxButton.OK,
								MessageBoxImage.Information);
				 */
			}
			catch (Exception x)
			{
				MessageBox.Show(x.Message, Caption,
								MessageBoxButton.OK, MessageBoxImage.Warning);
			}
		}

		private void OnCancelClick(object sender, RoutedEventArgs e)
		{
			Close();
		}

		#endregion

		#region Other Event Handlers

		private void ListBox_SelectionChanged(object sender,
											  SelectionChangedEventArgs e)
		{
			EnableButtons();
		}

		private void Timer_Tick(object sender, EventArgs e)
		{
			timer.Stop();
			StatusText.Text = "";
		}

		#endregion

		#region Helpers

		private void ReadCurrentValues()
		{
			Title = string.Format("{0} - {1}", originalTitle, hive);

			try
			{
				data = ReadRegistry(hive);
				cleanData = CloneData(data);

				if (data.Count == 0)
					return;

				ListBox.DataContext = data;
			}
			catch (Exception x)
			{
				MessageBox.Show(x.Message, Caption,
								MessageBoxButton.OK, MessageBoxImage.Warning);
			}
		}

		private static ObservableCollection<string> ReadRegistry(Hive currentHive)
		{
			var editor = new RegistryEditor();
			IEnumerable<string> paths = editor.GetPathStrings(currentHive);
			var result = new ObservableCollection<string>(paths);
			return result;
		}

		private bool UserDeclinesToAbandon()
		{
			if (DataAreClean())
				return false;

			MessageBoxResult result =
				MessageBox.Show("Data have changed. Save first?",
								Title,
								MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);

			switch (result)
			{
				case MessageBoxResult.Cancel:
					return true;
				case MessageBoxResult.OK:
				case MessageBoxResult.Yes:
					return TriedToSaveButFailed();
				case MessageBoxResult.No:
					return false;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private bool DataAreClean()
		{
			if (data == null || cleanData == null)
				return true;
			if (data.Count != cleanData.Count)
				return false;
			bool k = data.Zip(cleanData, (a, b) => a == b).All(x => x);
			return k;
		}

		private bool TriedToSaveButFailed()
		{
			try
			{
				Save();
				return false;
			}
			catch (Exception x)
			{
				MessageBox.Show(x.Message, Caption,
								MessageBoxButton.OK, MessageBoxImage.Warning);
				return true;
			}
		}

		private void Save()
		{
			var editor = new RegistryEditor();
			editor.SetPathStrings(hive, data);
			cleanData = CloneData(data);
		}

		private static List<string> CloneData(IEnumerable<string> list)
		{
			return new List<string>(list);
		}

		private void EnableButtons()
		{
			int x = ListBox.SelectedIndex;
			bool isEnabled = x >= 0;

			EditButton.IsEnabled = isEnabled;
			DeleteButton.IsEnabled = isEnabled;
			MoveUpButton.IsEnabled = isEnabled;
			MoveDownButton.IsEnabled = isEnabled;
		}

		#endregion
	}
}
