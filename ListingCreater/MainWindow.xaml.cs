using ListingAutoCreater.Models;
using ListingCreater.Logic;
using Microsoft.WindowsAPICodePack.Dialogs;
using ServiceStationBusinessLogic.OfficePackage;
using ServiceStationBusinessLogic.OfficePackage.Implements;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ListingCreater
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly AbstractSaveToWord _saveToWord;
        private readonly FileFinder _finder;

        public MainWindow()
        {
            InitializeComponent();
            _saveToWord = new SaveToWord();
            _finder = new FileFinder();
        }

        private void ButtonAddExtention_Click(object sender, RoutedEventArgs e)
        {
            string text = extentionInput.Text;

            if (!string.IsNullOrEmpty(text))
            {
                extentionList.Items.Add($".{text}");
                extentionInput.Text = "";
            }
        }

        private void extentionList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var element = extentionList.SelectedItem;
            if(MessageBox.Show($"Вы уверены, что хотите удалить расширение {element}","Подтверждение удаления.",MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                extentionList.Items.Remove(element);
            }
        }

        private void ButtonAddIgnoreF_Click(object sender, RoutedEventArgs e)
        {
            string text = ignoreFInput.Text;

            if (!string.IsNullOrEmpty(text))
            {
                ignoreFList.Items.Add($"{text}");
                ignoreFInput.Text = "";
            }
        }

        private void ignoreFList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var element = ignoreFList.SelectedItem;
            if (MessageBox.Show($"Вы уверены, что хотите удалить игнорируему папку {element}", "Подтверждение удаления.", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                ignoreFList.Items.Remove(element);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            CommonFileDialogResult result = dialog.ShowDialog();

            if(result == CommonFileDialogResult.Ok)
            {
                projectFolderText.Text = dialog.FileName;
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.SaveFileDialog();
            dialog.FileName = "Листинг"; // Default file name
            dialog.DefaultExt = ".docx"; // Default file extension
            dialog.Filter = "Word Document (.docx)|*.docx"; // Filter files by extension

            bool? res = dialog.ShowDialog();
            if (res != null && res.Value && !string.IsNullOrEmpty(projectFolderText.Text) && extentionList.Items.Count>0)
            {
                try
                {
                    string filename = dialog.FileName;

                    var data = _finder.GetAllFileInFolder(new FileSearchModel
                    {
                        FolderPath = projectFolderText.Text,
                        RequredExtentions = extentionList.Items.OfType<string>().ToList(),
                        IgnoredFolderName = ignoreFList.Items.OfType<string>().ToList(),
                    });


                    int cc = 3;
                    int.TryParse(columnCount.Text, out cc);
                    int tiS = 7;
                    int.TryParse(titleSize.Text, out tiS);
                    int tS = 6;
                    int.TryParse(textSize.Text, out tS);

                    MemoryStream file = _saveToWord.CreateFaultListDoc(new DocumentInfo
                    {
                        FileName = filename,
                        Files = data,
                        HomeProjectDirectory = projectFolderText.Text+"//",
                        OutputTabAndReturns = !tabRemove.IsChecked.Value,
                        ColumnsCount = cc,
                        ListingTitleTextSoze = tiS,
                        ListingTextSize= tS
                    });

                    using var fileStream = new FileStream(filename, FileMode.OpenOrCreate, FileAccess.Write);
                    file.WriteTo(fileStream);

                    MessageBox.Show("Листинг создан!", "Результат работы", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch(Exception ex)                 
                {
                    MessageBox.Show($"Ошибка при создании листинга:{ex.Message}", "Результат работы", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Введите данные для создания листинга", "Результат работы", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
