using CrocoChat.Model.Entities.Ecc.Chats;
using Microsoft.EntityFrameworkCore;

namespace CrocoChat.Model.Contexts
{
    public class ChatDbContext : ApplicationDbContext, IChatDbContext
    {
        public ChatDbContext(DbContextOptions<ChatDbContext> options) : base(options)
        {
        }

        public DbSet<ApplicationChat> Chats { get; set; }

        public DbSet<ApplicationChatMessage> ChatMessages { get; set; }

        public DbSet<ApplicationChatUserRelation> ChatUserRelations { get; set; }

        public DbSet<ApplicationChatMessageAttachment> ChatMessageAttachments { get; set; }
    }
}