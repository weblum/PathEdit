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
		public StringEditor()
		{
			InitializeComponent();
		}

		protected override void OnSourceInitialized(EventArgs e)
		{
			base.OnSourceInitialized(e);
			var hwnd = new WindowInteropHelper(this).Handle;
			var value = GetWindowLong(hwnd, GWL_STYLE);
			SetWindowLong(hwnd, GWL_STYLE, value & ~WS_SIZEBOXES);
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

		private void OkButton_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
		}

		[DllImport("user32.dll")]
		private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
		[DllImport("user32.dll")]
		private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

		private const int GWL_STYLE = -16;
		private const int WS_MAXIMIZEBOX = 0x10000;
		private const int WS_MINIMIZEBOX = 0x20000;
		private const int WS_SIZEBOXES = WS_MAXIMIZEBOX | WS_MINIMIZEBOX;
	}
}
