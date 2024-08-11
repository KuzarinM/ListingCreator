using DocumentFormat.OpenXml.EMMA;
using ListingCreater.Models;
using ListingCreater.Models.Internal;
using ListingCreater.Storages.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using ServiceStationBusinessLogic.OfficePackage;
using ServiceStationBusinessLogic.OfficePackage.HelperModels.Word.Enums;
using ServiceStationBusinessLogic.OfficePackage.HelperModels.Word.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ListingCreater.Logic.Handlers
{
    public class CreateListingDocumentCommandHandler : IRequestHandler<CreateListingDocumentCommand>
    {
        private readonly ILogger _logger;
        private readonly IConfigurationStorage _configurationStorage;
        private readonly AbstractSaveToWord _abstractSaveToWord;
        private readonly IMediator _mediator;

        private SemaphoreSlim _semaphore = new(1,1);

        public CreateListingDocumentCommandHandler(
            ILogger<CreateListingDocumentCommandHandler> logger, 
            IConfigurationStorage configurationStorage, 
            AbstractSaveToWord abstractSaveToWord,
            IMediator mediator)
        {
            _logger = logger;
            _configurationStorage = configurationStorage;
            _abstractSaveToWord = abstractSaveToWord;
            _mediator = mediator;
        }

        public async Task Handle(CreateListingDocumentCommand request, CancellationToken cancellationToken)
        {
            var configuration = _configurationStorage.GetConfiguration(null);

            if (configuration == null)
                throw new Exception("Конфигурации нет");

            var projectFilenames = await _mediator.Send(new GetProjectRequredFilenamesQuery());

            _abstractSaveToWord.CreateWord();

            Task.WaitAll(projectFilenames.Select(x=>AddSingleFileListing(x, configuration)).ToArray());

            using var fileStream = new FileStream(request.DestonationFilename, FileMode.Create, FileAccess.Write);
            _abstractSaveToWord.SaveWord().WriteTo(fileStream);
        }

        private async Task AddSingleFileListing(string filename, ListingConfiguration configuration)
        {
            List<(string, WordTextProperties?)> texts = new();
            var fileText = GetFileText(filename);
            if (configuration.OutputTabAndReturns)
            {
                texts = fileText.Split("\n").Select(run => ($"{run}\n", (WordTextProperties?)null)).ToList();
            }
            else
            {
                texts = new() { (fileText.Replace("\t", "").Replace("\n", "").Replace("\r", ""), null) };
            }

            await _semaphore.WaitAsync();

            _abstractSaveToWord.CreateParagraph(new WordParagraph
            {
                Texts = new()
                    {
                        (filename.Replace(configuration.ProjectDirectoryName,""),null)
                    },
                TextProperties = new()
                {
                    Size = configuration.ListingTitleTextSize,
                    Bold = true,
                    JustificationType = WordJustificationType.Both,
                    ColumnCount = configuration.ColumnsCount,
                    //PageMagin = new()
                    //{
                    //    Top = 0,
                    //    Bottom = 0,
                    //    Right = 0,
                    //    Left = 0,
                    //}
                }
            });

            _abstractSaveToWord.CreateParagraph(new WordParagraph
            {
                Texts = texts,
                TextProperties = new()
                {
                    Size = configuration.ListingTextSize,
                    Bold = false,
                    JustificationType = WordJustificationType.Left,
                    ColumnCount = configuration.ColumnsCount
                }
            });

            _semaphore.Release();
        }

        private string GetFileText(string filePath)
        {
            BinaryReader instr = new BinaryReader(File.OpenRead(filePath));
            byte[] data = instr.ReadBytes((int)instr.BaseStream.Length);
            instr.Close();
            
            return GetEncoding(data).GetString(data);
        }

        private Encoding GetEncoding(byte[] data)
        {
            // определяем BOM (EF BB BF)
            if (data.Length > 2 && data[0] == 0xef && data[1] == 0xbb && data[2] == 0xbf)
            {
                if (data.Length != 3) return Encoding.UTF8;
                else return Encoding.Default;
            }

            int i = 0;
            while (i < data.Length - 1)
            {
                if (data[i] > 0x7f)
                { // не ANSI-символ
                    if ((data[i] >> 5) == 6)
                    {
                        if ((i > data.Length - 2) || ((data[i + 1] >> 6) != 2))
                            return Encoding.GetEncoding(1251);
                        i++;
                    }
                    else if ((data[i] >> 4) == 14)
                    {
                        if ((i > data.Length - 3) || ((data[i + 1] >> 6) != 2) || ((data[i + 2] >> 6) != 2))
                            return Encoding.GetEncoding(1251);
                        i += 2;
                    }
                    else if ((data[i] >> 3) == 30)
                    {
                        if ((i > data.Length - 4) || ((data[i + 1] >> 6) != 2) || ((data[i + 2] >> 6) != 2) || ((data[i + 3] >> 6) != 2))
                            return Encoding.GetEncoding(1251);
                        i += 3;
                    }
                    else
                    {
                        return Encoding.GetEncoding(1251);
                    }
                }
                i++;
            }

            return Encoding.UTF8;
        }
    }
}
