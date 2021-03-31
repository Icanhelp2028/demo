using IBLL.Injections;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IBLL.Models.ChatDb
{
    [Table("ChatMessage")]
    public class ChatMessageMdl
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int OwnId { get; set; }
        public int GroupId { get; set; }
        public int FromUser { get; set; }
        public string Text { get; set; }
        public string Data { get; set; }
        public ChatNotifyType Notify { get; set; }
    }
}