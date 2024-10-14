using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebAPI.Models
{
    [Table("LOG_ENTRIES")]
    public class LogEntry
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("LOG_ENTRY_ID")]
        public long LogEntryID{ get; set; }

        [Required]
        [Column("TIMESTAMP")]
        public DateTime Timestamp{ get; set; }

        [Required]
        [Column("LEVEL")]
        public LogLevel Level { get; set; }

        [Required]
        [Column("MESSAGE")]
        [StringLength(500)]
        public string Message { get; set; }
    }
}
