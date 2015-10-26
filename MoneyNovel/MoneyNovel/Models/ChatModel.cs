using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;
using System.Web.Security;

namespace MoneyNovel.Models
{

    public class ChatContext : DbContext
    {
        public ChatContext()
            : base("DefaultConnection")
        {
        }

        public DbSet<ChatMessage> ChatMessages { get; set; }
        public DbSet<UserToChatMessage> UserToChatMessages { get; set; }
    }

    //public class UserToChatMessageContext : DbContext
    //{
    //    public UserToChatMessageContext()
    //        : base("DefaultConnection")
    //    {
    //    }

    //}

    [Table("ChatMessage")]
    public class ChatMessage{
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int ChatMessageID { get; set; }
        public string FBID { get; set; }
        public string UserName { get; set; }
        public string Message { get; set; }
        public DateTime TimeSent { get; set; }
    }

    [Table("WhoSawChatMessage")]
    public class UserToChatMessage
    {
        [Key, Column(Order = 0)]
        public int ChatMessageID { get; set; }
        [Key, Column(Order = 1)]
        public string FBID { get; set; }
    }

    public class PusherUsers
    {
        public List<PusherUser> users { get; set; }
    }

    public class PusherUser
    {
        public string id { get; set; }
    }

}