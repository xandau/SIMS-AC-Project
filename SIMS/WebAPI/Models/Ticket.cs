using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using System.Xml.Schema;
using WebAPI.Enums;

namespace WebAPI.Models
{
    [Table("TICKETS")]
    public class Ticket : AItem
    {
        [Required]
        [StringLength(50)]
        [Column("TITLE")]
        public required string Title { get; set; }

        [Required]
        [StringLength(500)]
        [Column("DESCRIPTION")]
        public required string Description { get; set; }

        [Required]
        [Column("STATE")]
        public required ETicketState State { get; set; } = ETicketState.OPEN;

        [Required]
        [Column("CREATION_TIME")]
        public required DateTime CreationTime { get; set; }

        [Required]
        [Column("Severity")]
        [Range(1, 10)]
        public required byte Severity { get; set; }

        [Required]
        [Column("CVE")]
        public required string CVE { get; set; }

        [Required]
        [Column("TIMESTAMP")]
        public required DateTime Timestamp { get; set; }


        [Column("CREATOR_ID")]
        public long CreatorID { get; set; }

        public virtual User? Creator { get; set; }

        [Column("ASSIGNEDPERSON_ID")]
        public long? AssignedPersonID { get; set; }
        public virtual User? AssignedPerson { get; set; }
    }
}
