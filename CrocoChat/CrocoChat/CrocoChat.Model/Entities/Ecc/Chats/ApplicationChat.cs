using CrocoChat.Model.Entities.Clt;
using Microsoft.EntityFrameworkCore;
using Zoo.Ecc.Entities.Chats;

namespace CrocoChat.Model.Entities.Ecc.Chats
{
    public class ApplicationChat : Chat<ApplicationChatMessage, ApplicationChatUserRelation>
    {
    }

    public class ApplicationChatMessage : ChatMessage<ApplicationChat, Client, ApplicationChatMessageAttachment>
    {
    }

    public class ApplicationChatUserRelation : ChatUserRelation<ApplicationChat, Client>
    {
    }

    public class ApplicationChatMessageAttachment : ChatMessageAttachment<ApplicationChatMessage, DbFile>
    {
    }

    public interface IChatDbContext
    {
        DbSet<ApplicationChat> Chats { get; set; }

        DbSet<ApplicationChatMessage> ChatMessages { get; set; }

        DbSet<ApplicationChatUserRelation> ChatUserRelations { get; set; }

        DbSet<ApplicationChatMessageAttachment> ChatMessageAttachments { get; set; }
    }
}