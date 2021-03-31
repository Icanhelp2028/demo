using System.ComponentModel.DataAnnotations.Schema;

namespace IBLL.Models.ChatDb
{
    [Table("ChatGroupUser")]
    public class ChatGroupUserMdl
    {
        public int GroupId { get; set; }

        public int UserId { get; set; }

        public int OwnId { get; set; }
    }
}