using Croco.Core.Data.Implementations.DbAudit.Models;
using Croco.Core.EventSourcing.Implementations.StatusLog.Models;
using Croco.Core.Model.Entities;
using Croco.Core.Model.Entities.Store;
using CrocoChat.Model.Entities.Ecc.Chats;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Zoo.Core;
using Zoo.Core.Abstractions;

namespace CrocoChat.Model.Contexts
{
    public class ChatDbContext : ApplicationDbContext, IChatDbContext, IStoreContext
    {
        public const string ServerConnection = "ServerConnection";

        public const string LocalConnection = "DefaultConnection";

#if DEBUG
        public static string ConnectionString => LocalConnection;
#else
        public static string ConnectionString => ServerConnection;
#endif

        #region Конструкторы
        public ChatDbContext(DbContextOptions<ChatDbContext> options) : base(options)
        {
        }
        #endregion

        public static ChatDbContext Create(IConfiguration configuration)
        {
            return Create(configuration.GetConnectionString(ConnectionString));
        }

        public static ChatDbContext Create(string connectionString)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ChatDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new ChatDbContext(optionsBuilder.Options);
        }


        #region IStore
        public DbSet<RobotTask> RobotTasks { get; set; }

        public DbSet<LoggedUserInterfaceAction> LoggedUserInterfaceActions { get; set; }

        public DbSet<LoggedApplicationAction> LoggedApplicationActions { get; set; }

        public DbSet<AuditLog> AuditLogs { get; set; }

        public DbSet<IntegrationMessageLog> IntegrationMessageLogs { get; set; }

        public DbSet<IntegrationMessageStatusLog> IntegrationMessageStatusLogs { get; set; }

        public DbSet<WebAppRequestContextLog> WebAppRequestContextLogs { get; set; }
        #endregion

        #region Сообщения и чаты
        public DbSet<ApplicationChat> Chats { get; set; }

        public DbSet<ApplicationChatMessage> ChatMessages { get; set; }

        public DbSet<ApplicationChatUserRelation> ChatUserRelations { get; set; }

        public DbSet<ApplicationChatMessageAttachment> ChatMessageAttachments { get; set; }
        #endregion
    }
}