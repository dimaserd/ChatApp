using Croco.Core.Abstractions.Files;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace CrocoChat.Logic.Extensions
{
    /// <summary>
    /// Расширения файлов
    /// </summary>
    public static class FileExtensions
    {
        /// <summary>
        /// Файл
        /// </summary>
        internal class TempFileData : IFileData
        {
            /// <summary>
            /// Название файла
            /// </summary>
            public string FileName { get; set; }

            /// <summary>
            /// Данные в файле
            /// </summary>
            public byte[] Data { get; set; }
        }

        /// <summary>
        /// Переложить файл из платформы AspNetCore для работы с библиотекой NetCroco.Core
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static IFileData ToFileData(this IFormFile file)
        {
            using var ms = new MemoryStream();
            file.CopyTo(ms);
            var fileBytes = ms.ToArray();

            return new TempFileData
            {
                FileName = file.FileName,
                Data = fileBytes
            };

        }
    }
}