using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Croco.Core.Model.Abstractions.Entity;
using Newtonsoft.Json;

namespace Zoo.Clt.Entities
{
    public class WebApplicationUser : ICrocoUser
    {
        public string Id { get; set; }
        public string Email { get; set; }

        /// <inheritdoc />
        /// <summary>
        /// Имя
        /// </summary>
        public string Name { get; set; }

        /// <inheritdoc />
        /// <summary>
        /// Фамилия
        /// </summary>
        public string Surname { get; set; }

        /// <inheritdoc />
        /// <summary>
        /// Отчество
        /// </summary>
        public string Patronymic { get; set; }


        #region Свойства для аудита
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }

        public DateTime? LastModifiedOn { get; set; }
        public string LastModifiedBy { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

        #endregion
    }

    public class WebApplicationUser<TAvatarFile> : WebApplicationUser, ICrocoUser<TAvatarFile> where TAvatarFile : class
    {
        /// <inheritdoc />
        /// <summary>
        /// Идентификатор файла с аватаром пользователя
        /// </summary>
        [ForeignKey(nameof(AvatarFile))]
        public int? AvatarFileId { get; set; }

        /// <inheritdoc />
        /// <summary>
        /// Аватар пользователя
        /// </summary>
        [JsonIgnore]
        public virtual TAvatarFile AvatarFile { get; set; }
    }
}
