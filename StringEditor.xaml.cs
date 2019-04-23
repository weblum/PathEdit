//---------------------------------------------------------------------------
// FILE NAME: StringEditor.xaml.cs
// DATE:      Sunday, September 27, 2015   3 pm
// WEATHER:   Not available.
// Programmer's Cuvee XLI
// Copyright (C) 2015 William E. Blum.  All rights reserved.
//---------------------------------------------------------------------------

using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace PathEdit
{
	public partial class StringEditor
	{
		private const int GWL_STYLE = -16;

		private const int WS_MAXIMIZEBOX = 0x10000;

		private const int WS_MINIMIZEBOX = 0x20000;

		private const int WS_SIZEBOXES = WS_MAXIMIZEBOX | WS_MINIMIZEBOX;

		public StringEditor()
		{
			InitializeComponent();
		}

		public string PathString
		{
			get { return TextBox.Text; }
			set { TextBox.Text = value; }
		}

		public bool IsNew
		{
			set
			{
				InstructionLabel.Content = value
											   ? "Enter new path string"
											   : "Modify path string";
			}
		}

		protected override void OnSourceInitialized(EventArgs e)
		{
			base.OnSourceInitialized(e);
			IntPtr hwnd = new WindowInteropHelper(this).Handle;
			int value = GetWindowLong(hwnd, GWL_STYLE);
			SetWindowLong(hwnd, GWL_STYLE, value & ~WS_SIZEBOXES);
		}

		private void OkButton_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
		}

		private void BrowseButton_Click(object sender, RoutedEventArgs e)
		{
			string folderFromUser = DirectoryBrowser.GetFolderFromUser();
			if (folderFromUser == null)
				return;
			TextBox.Text = folderFromUser;
		}

		[DllImport("user32.dll")]
		private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

		[DllImport("user32.dll")]
		private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
	}
}
