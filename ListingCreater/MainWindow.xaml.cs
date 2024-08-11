using ListingAutoCreater.Models;
using ListingCreater.Logic;
using ListingCreater.Logic.Handlers;
using ListingCreater.Models;
using ListingCreater.Models.Internal;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;
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
        private readonly IMediator _mediator;
        private readonly ILogger _logger;
        private readonly Microsoft.Win32.SaveFileDialog _saveListingDialog;
        private readonly CommonOpenFileDialog _projectDirDialogDialog;

        public MainWindow(ILogger<MainWindow> logger, IMediator mediator)
        {
            InitializeComponent();
            _mediator = mediator;
            _logger = logger;
            _saveListingDialog = new SaveFileDialog();
            _projectDirDialogDialog = new CommonOpenFileDialog();
        }

        private void ButtonAddExtention_Click(object sender, RoutedEventArgs e)
        {
            SafeExecute(() =>
            {
                var res = _mediator.Send(new AddExtentionCommand()
                {
                    ExtentionText = extentionInput.Text,
                    ExtentionInput = extentionInput,
                    ExtentionList = extentionList,
                }).Result;

                if (!res)
                {
                    MessageBox.Show("Не удалось добавить расширение. Возможно оно уже есть", "Ошибка добавления расширения");
                }
            });
        }

        private void extentionList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            SafeExecute(() =>
            {
                var element = extentionList.SelectedItem;
                if (MessageBox.Show($"Вы уверены, что хотите удалить расширение {element}", "Подтверждение удаления.", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    var res = _mediator.Send(new RemoveExtentionCommand()
                    {
                        SelectedItem = element,
                        ExtentionList = extentionList,
                    }).Result;

                    if (!res)
                    {
                        MessageBox.Show("Не удалось удалить расширение.", "Ошибка удаления расширения");
                    }
                }
            });
        }

        private void ButtonAddIgnoreF_Click(object sender, RoutedEventArgs e)
        {
            SafeExecute(() =>
            {
                var res = _mediator.Send(new AddIgnoreDirPatternCommand()
                {
                    IgnoreDirText = ignoreFInput.Text,
                    IgnoreDirInput = ignoreFInput,
                    IgnoreDirList = ignoreFList,
                }).Result;

                if (!res)
                {
                    MessageBox.Show("Не удалось добавить паттерн игнорирования. Возможно он уже есть", "Ошибка добавления паттерна");
                }
            });
        }

        private void ignoreFList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            SafeExecute(() =>
            {
                var element = ignoreFList.SelectedItem;
                if (MessageBox.Show($"Вы уверены, что хотите удалить паттерн игнора {element}", "Подтверждение удаления.", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    var res = _mediator.Send(new RemoveIgnoreDirPatternCommand()
                    {
                        SelectedItem = element,
                        IgnoreDirList = ignoreFList,
                    }).Result;

                    if (!res)
                    {
                        MessageBox.Show("Не удалось удалить паттерн игнорирования.", "Ошибка удаления паттерна игнорирования");
                    }
                }
            });
        }

        private void SelectProjectDirButton_Click(object sender, RoutedEventArgs e)
        {
            SafeExecute(() =>
            {
                var res = _mediator.Send(new SetProjectDirCommand()
                {
                    SelectDialog = _projectDirDialogDialog,
                    ProjectFolderInput = projectFolderText

                }).Result;

                if (string.IsNullOrEmpty(res))
                {
                    MessageBox.Show("Не удалось выбрать папку проекта.", "Ошибка выбора папки проекта");
                }
            });
        }

        private void CreateListingButton_Click(object sender, RoutedEventArgs e)
        {

            SafeExecute(() =>
            {
                var filepath = _mediator.Send(new SelectListingDestonationQuery()
                {
                    SelectDialog = _saveListingDialog,
                }).Result;

                if (string.IsNullOrEmpty(filepath))
                {
                    MessageBox.Show("Не удалось выбрать папку проекта.", "Ошибка выбора папки проекта");
                    return;
                }

                if(string.IsNullOrEmpty(projectFolderText.Text) || extentionList.Items.Count == 0)
                {
                    MessageBox.Show("Введите данные для создания листинга", "Результат работы", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                _mediator.Send(new SaveCurrentConfigCommand()
                {
                    ColumnsCountText = columnCount,
                    TitleSizeText = titleSize,
                    TextSizeText = textSize,
                    TabRemoveBox = tabRemove
                }).Wait();

                _mediator.Send(new CreateListingDocumentCommand()
                {
                    DestonationFilename = filepath,
                }).Wait();

                MessageBox.Show("Листинг создан!", "Результат работы", MessageBoxButton.OK, MessageBoxImage.Information);

            });
        }

        private void MenuFileSave_Click(object sender, RoutedEventArgs e)
        {
            SafeExecute(() =>
            {
                _mediator.Send(new SaveCurrentConfigCommand()
                {
                    ColumnsCountText = columnCount,
                    TitleSizeText = titleSize,
                    TextSizeText = textSize,
                    TabRemoveBox = tabRemove
                }).Wait();

                var filename = _mediator.Send(new SelectConfigSaveFileQuery()
                {
                    SaveFileDialog = new SaveFileDialog()
                }).Result;

                if (string.IsNullOrEmpty(filename))
                    throw new Exception("Не выбран файл для сохранения");

                _mediator.Send(new SaveConfigurationToFileCommand()
                {
                    Filepath = filename
                }).Wait();

                MessageBox.Show("Конфигурация сохранена", "Результат работы", MessageBoxButton.OK, MessageBoxImage.Information);
            });
        }

        private void MenuFileLoad_Click(object sender, RoutedEventArgs e)
        {
            SafeExecute(() =>
            {
                var filename = _mediator.Send(new SelectConfigSaveFileQuery()
                {
                    SaveFileDialog = new OpenFileDialog()
                }).Result;

                if (string.IsNullOrEmpty(filename))
                    throw new Exception("Не выбран файл для загрузки");

                _mediator.Send(new LoadConfigurationFromFileCommand()
                {
                    Filename = filename,
                    TextSizeText= textSize,
                    TitleSizeText= titleSize,
                    TabRemoveBox = tabRemove,
                    ColumnsCountText = columnCount,
                    ExtentionList = extentionList,
                    IgnoreList = ignoreFList
                }).Wait();

                MessageBox.Show("Конфигурация загружена", "Результат работы", MessageBoxButton.OK, MessageBoxImage.Information);
            });
        }

        private void extentionInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (sender is TextBox textBox && textBox.IsFocused && e.Key == Key.Enter)
            {
                ButtonAddExtention_Click(this, new RoutedEventArgs());
            }
        }

        private void ignoreFInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (sender is TextBox textBox && textBox.IsFocused && e.Key == Key.Enter)
            {
                ButtonAddIgnoreF_Click(this, new RoutedEventArgs());
            }
        }



        private XElement? LoadData(string filename) => XDocument.Load(filename)?.Root;

        private void SaveData(ListingConfiguration data, string filename, string xmlNodeName)
        {
            new XDocument(new XElement(xmlNodeName, data.GetXElement)).Save(filename);
        }


        private void SafeExecute(Action action)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error on action");
                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButton.OK);
            }
        }


    }
}
