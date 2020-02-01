using Croco.Core.Model.Entities.Application;
using System.Collections.Generic;

namespace CrocoChat.Model.Entities
{
    /// <summary>
    /// Класс описывающий файл который находится в базе данных
    /// </summary>
    public class DbFile : DbFileIntId
    {
        public virtual ICollection<ApplicationDbFileHistory> History { get; set; }
    }

    public class ApplicationDbFileHistory : DbFileHistory<DbFile>
    {
    }
}