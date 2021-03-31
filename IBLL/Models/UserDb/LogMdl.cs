using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IBLL.Models.UserDb
{
    [Table("Logs")]
    public class LogMdl
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int SourceId { get; set; }
        public string SourceName { get; set; }
        public int LogLevel { get; set; }
        public string Body { get; set; }
    }
}