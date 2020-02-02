using Croco.Core.Abstractions.Data.Repository;
using Croco.Core.Logic.Workers.Files;
using CrocoChat.Model.Entities;

namespace CrocoChat.Logic.Implementations
{
    public class ApplicationFileManager : DbFileManager<DbFile, ApplicationDbFileHistory>
    {
        public ApplicationFileManager(IRepositoryFactory repositoryFactory) : base(repositoryFactory)
        {
        }
    }
}