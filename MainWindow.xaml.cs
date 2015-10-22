//---------------------------------------------------------------------------
// FILE NAME: MainWindow.xaml.cs
// DATE:      Sunday, September 27, 2015   2 pm
// WEATHER:   Not available.
// Programmer's Cuvee XLI
// Copyright (C) 2015 William E. Blum.  All rights reserved.
//---------------------------------------------------------------------------

using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace PathEdit
{
	public partial class MainWindow
	{
		private const string Caption = "Unexpected Happening";
		private ObservableCollection<string> data;
		private Hive hive = Hive.User;
		private string originalTitle;

		public MainWindow()
		{
			InitializeComponent();
			originalTitle = Title;
			ReadCurrentValues();
			EnableButtons();
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

		private void OnSaveClick(object sender, RoutedEventArgs e)
		{
			try
			{
				var editor = new RegistryEditor();
				editor.SetPathStrings(hive, data);
				MessageBox.Show("The operation completed successfully.",
				                "Good News",
				                MessageBoxButton.OK,
				                MessageBoxImage.Information);
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

		#endregion

		#region Helpers

		private void ReadCurrentValues()
		{
			Title = string.Format("{0} - {1}", originalTitle, hive);

			try
			{
				var editor = new RegistryEditor();
				data = editor.GetPathStrings(hive);

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