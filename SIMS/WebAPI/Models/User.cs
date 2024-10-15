using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using WebAPI.Enums;

namespace WebAPI.Models
{
    [Table("USERS")]
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("UserID")]
        public long UserID { get; set; }

        [Required]
        [Column("User_UUID", TypeName = "VARCHAR(300)")]
        public Guid UserUUID { get; set; }

        [Required]
        [StringLength(50)]
        [Column("USERNAME")]
        public required string UserName { get; set; }

        [Required]
        [StringLength(50)]
        [Column("FIRSTNAME")]
        public required string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        [Column("LASTNAME")]
        public required string LastName { get; set; }

        [Required]
        [StringLength(300)]
        [Column("PASSWORD")]
        public required string Password { get; set; }

        [Required]
        [StringLength(50)]
        [Column("EMAIL")]
        public required string Email { get; set; }

        [Required]
        [Column("ROLE")]
        public required ERoles Role { get; set; }

        [Required]
        [Column("BLOCKED")]
        public required bool Blocked { get; set; }

        public List<Ticket>? Tickets { get; set; }

    }
}
