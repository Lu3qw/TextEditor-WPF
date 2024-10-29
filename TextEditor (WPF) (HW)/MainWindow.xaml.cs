using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Animation;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Linq;
using FontFamily = System.Windows.Media.FontFamily;
using System.Globalization;

namespace TextEditor__WPF___HW_
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{

		string path = String.Empty;
		string filter = "txt|*.txt|xml|*.xml|json|*.json";
		bool isEdit = false;



		public MainWindow()
		{
			//CultureInfo.CurrentUICulture = new CultureInfo("en-US");

			Closing += Window_Closing;
			InitializeComponent();

		
			App.Current.Resources = new ResourceDictionary() { Source = new Uri("Themes/LightThemeDictionary.xaml", UriKind.RelativeOrAbsolute) };

		}

		private void OpenFileDialog_FileOk(object? sender, System.ComponentModel.CancelEventArgs e)
		{
			if (sender is System.Windows.Forms.OpenFileDialog openFileDialog)
			{
				path = openFileDialog.FileName;
				using (StreamReader reader = new StreamReader(openFileDialog.FileName))
				{
					mainTextBox.Text = reader.ReadToEnd();
				}
			}
			isEdit = false;
		}
		private void SaveFileDialog_FileOk(object? sender, System.ComponentModel.CancelEventArgs e)
		{
			if (sender is System.Windows.Forms.SaveFileDialog saveFileDialog)
			{
				path = saveFileDialog.FileName;
				using (StreamWriter writer = new StreamWriter(saveFileDialog.FileName))
				{
					writer.Write(mainTextBox.Text);
				}
			}
			isEdit = false;
		}

		private void Save()
		{
			using (StreamWriter writer = new StreamWriter(path))
			{
				writer.Write(mainTextBox.Text);
			}
		}

		private void SaveAs()
		{
			System.Windows.Forms.SaveFileDialog saveFileDialog = new System.Windows.Forms.SaveFileDialog();
			saveFileDialog.Filter = filter;
			saveFileDialog.FileOk += SaveFileDialog_FileOk;
			saveFileDialog.ShowDialog();
		}
		private int CountLetters(string text) => text.Count(symbol => char.IsLetter(symbol));


		private void openFileCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog();
			openFileDialog.Filter = filter;
			openFileDialog.FileOk += OpenFileDialog_FileOk;
			openFileDialog.ShowDialog();
		}
		private void saveFileCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			if (string.IsNullOrEmpty(path))
				SaveAs();
			else
				Save();
			isEdit = false;
		}
		private void saveFileCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = isEdit;
		private void saveAsFileCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e) => SaveAs();


		private void mainTextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			isEdit = true;
			lineLabel.Content = mainTextBox.LineCount.ToString() + ",";
			charLabel.Content = CountLetters(mainTextBox.Text).ToString();
		}


		private void ExitMenuItem_Click(object sender, RoutedEventArgs e) => this.Close();

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (!isEdit)
				return;

			MessageBoxResult result = System.Windows.MessageBox.Show(Resource.saveChangesWindowText, Resource.saveChangesWindow, MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
			if (result == MessageBoxResult.Yes)
			{
				if (string.IsNullOrEmpty(path))
					SaveAs();
				else
					Save();
			}
			else if (result == MessageBoxResult.Cancel)
			{
				e.Cancel = true;
			}
		}


		private void leftAlignCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e) => mainTextBox.TextAlignment = TextAlignment.Left;
		private void centerAlignCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e) => mainTextBox.TextAlignment = TextAlignment.Center;
		private void rightAlignCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e) => mainTextBox.TextAlignment = TextAlignment.Right;

		private void toggleItalicCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			if (mainTextBox.FontStyle == FontStyles.Italic)
				mainTextBox.FontStyle = FontStyles.Normal;
			else
				mainTextBox.FontStyle = FontStyles.Italic;
		}

		private void toggleUnderlineCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			if (mainTextBox.TextDecorations == TextDecorations.Underline)
				mainTextBox.TextDecorations = null;
			else
				mainTextBox.TextDecorations = TextDecorations.Underline;
		}

		private void toggleBoldCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			if (mainTextBox.FontWeight == FontWeights.Bold)
				mainTextBox.FontWeight = FontWeights.Regular;
			else
				mainTextBox.FontWeight = FontWeights.Bold;
		}

		private void fontDialogControlButton_Click(object sender, RoutedEventArgs e)
		{
			FontDialog fontDialog = new FontDialog();
			if (fontDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				mainTextBox.FontFamily =  new FontFamily(fontDialog.Font.Name);
				mainTextBox.FontSize = fontDialog.Font.Size;
				mainTextBox.FontWeight = fontDialog.Font.Bold ? FontWeights.Bold : FontWeights.Regular;
				mainTextBox.FontStyle = fontDialog.Font.Italic ? FontStyles.Italic : FontStyles.Normal;
			}
		}

		private void ThemeMenuItem_Click(object sender, RoutedEventArgs e)
		{
			if (sender is MenuItem menuItem)
				if ((menuItem.Tag as string) != null)
				{
					string path = "Themes/" + (menuItem.Tag).ToString() + ".xaml";
					App.Current.Resources = new ResourceDictionary() { Source = new Uri(path, UriKind.RelativeOrAbsolute) };
				}
			
		}
	}
}
