using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebAPI.Enums;

namespace WebAPI.Models
{
    [Table("TICKETS")]
    public class Ticket
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("TICKET_ID")]
        public int TicketId { get; set; }

        [Required]
        [StringLength(50)]
        [Column("TITLE")]
        public string Title { get; set; }

        [Required]
        [StringLength(500)]
        [Column("DESCRIPTION")]
        public string Description { get; set; }

        [Required]
        [Column("STATE")]
        public ETicketState State { get; set; } = ETicketState.OPEN;

        [Required]
        [Column("CREATION_TIME")]
        public DateTime CreationTime { get; set; }

        [Required]
        [Column("CREATOR_ID")]
        public long CreatorID { get; set; }
        public User Creator { get; set; }

        
    }
}
