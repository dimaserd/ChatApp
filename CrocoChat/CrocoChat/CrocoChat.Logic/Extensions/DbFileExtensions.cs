using Croco.Core.Logic.Models.Files;
using CrocoChat.Logic.Implementations;
using CrocoChat.Model.Entities;

namespace CrocoChat.Logic.Extensions
{
    public static class DbFileExtensions
    {
        public static bool IsImage(this DbFileIntIdModelNoData model)
        {
            return MyCrocoWebApplication.IsImage(model.FileName);
        }

        public static bool IsImage(this DbFile file)
        {
            return MyCrocoWebApplication.IsImage(file.FileName);
        }
    }
}