using ListingAutoCreater.Models;
using ListingCreater.Logic;
using ListingCreater.Models;
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
using System.Xml.Linq;

namespace ListingCreater
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly AbstractSaveToWord _saveToWord;
        private readonly FileFinder _finder;
        private ListingConfiguration configuration;

        public MainWindow()
        {
            InitializeComponent();
            _saveToWord = new SaveToWord();
            _finder = new FileFinder();
            configuration = new();
        }

        private void ButtonAddExtention_Click(object sender, RoutedEventArgs e)
        {
            string text = extentionInput.Text;

            if (!string.IsNullOrEmpty(text))
            {
                extentionList.Items.Add($".{text}");
                extentionInput.Text = "";
                configuration.Extentions.Add($".{text}");
            }
        }

        private void extentionList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var element = extentionList.SelectedItem;
            if(MessageBox.Show($"Вы уверены, что хотите удалить расширение {element}","Подтверждение удаления.",MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                extentionList.Items.Remove(element);
                configuration.Extentions.Remove(element.ToString());
            }
        }

        private void ButtonAddIgnoreF_Click(object sender, RoutedEventArgs e)
        {
            string text = ignoreFInput.Text;

            if (!string.IsNullOrEmpty(text))
            {
                ignoreFList.Items.Add($"{text}");
                ignoreFInput.Text = "";
                configuration.IgnoreFolder.Add($"{text}");
            }
        }

        private void ignoreFList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var element = ignoreFList.SelectedItem;
            if (MessageBox.Show($"Вы уверены, что хотите удалить игнорируему папку {element}", "Подтверждение удаления.", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                ignoreFList.Items.Remove(element);
                configuration.IgnoreFolder.Remove(element.ToString());
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
                configuration.ProjectDirectoryName = dialog.FileName;
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
                        FolderPath = configuration.ProjectDirectoryName,
                        RequredExtentions = configuration.Extentions,
                        IgnoredFolderName = configuration.IgnoreFolder,
                    });


                    int cc = configuration.ColumnsCount;
                    int.TryParse(columnCount.Text, out cc);
                    configuration.ColumnsCount = cc;

                    int tiS = configuration.ListingTitleTextSoze;
                    int.TryParse(titleSize.Text, out tiS);
                    configuration.ListingTitleTextSoze = tiS;

                    int tS = configuration.ListingTextSize;
                    int.TryParse(textSize.Text, out tS);
                    configuration.ListingTextSize = tiS;

                    configuration.OutputTabAndReturns = !tabRemove.IsChecked.Value;

                    MemoryStream file = _saveToWord.CreateFaultListDoc(new DocumentInfo
                    {
                        FileName = filename,
                        Files = data,
                        HomeProjectDirectory = configuration.ProjectDirectoryName+"\\",
                        OutputTabAndReturns = configuration.OutputTabAndReturns,
                        ColumnsCount = configuration.ColumnsCount,
                        ListingTitleTextSoze = configuration.ListingTitleTextSoze,
                        ListingTextSize= configuration.ListingTextSize
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

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.SaveFileDialog();
            dialog.FileName = "Конфигурация"; // Default file name
            dialog.DefaultExt = ".xml"; // Default file extension
            dialog.Filter = "XML Document (.xml)|*.xml"; // Filter files by extension

            bool? res = dialog.ShowDialog();
            if(res != null && res.Value)
            {
                try
                {
                    string filename = dialog.FileName;
                    SaveData(configuration, filename, "Configuration_v1.0");
                }
                catch(Exception ex)
                {
                    MessageBox.Show($"Ошибка при сохранении конфигурации:{ex.Message}", "Результат работы", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.FileName = "Конфигурация"; // Default file name
            dialog.DefaultExt = ".xml"; // Default file extension
            dialog.Filter = "XML Document (.xml)|*.xml"; // Filter files by extension

            bool? res = dialog.ShowDialog();
            if (res != null && res.Value)
            {
                string filename = dialog.FileName;
                if (MessageBox.Show($"Вы уверены, что хотите загрузить настройки из файлf {filename}", "Подтверждение загрузка.", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                {
                    return;
                }
                try
                {  
                    configuration = ListingConfiguration.Create(LoadData(filename));
                    extentionList.Items.Clear();
                    foreach (var item in configuration.Extentions)
                    {
                        extentionList.Items.Add(item);
                    }
                    ignoreFList.Items.Clear();
                    foreach (var item in configuration.IgnoreFolder)
                    {
                        ignoreFList.Items.Add(item);
                    }
                    columnCount.Text = configuration.ColumnsCount.ToString();
                    titleSize.Text = configuration.ListingTitleTextSoze.ToString();
                    textSize.Text = configuration.ListingTextSize.ToString();
                    tabRemove.IsChecked = !configuration.OutputTabAndReturns;
                    projectFolderText.Text = configuration.ProjectDirectoryName;

                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при сохранении конфигурации:{ex.Message}", "Результат работы", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private XElement? LoadData(string filename) => XDocument.Load(filename)?.Root;

        private void SaveData(ListingConfiguration data, string filename, string xmlNodeName)
        {
            new XDocument(new XElement(xmlNodeName, data.GetXElement)).Save(filename);
        }
    }
}
