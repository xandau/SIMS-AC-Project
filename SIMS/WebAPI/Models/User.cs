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
        public string UserName { get; set; }

        [Required]
        [StringLength(50)]
        [Column("FIRSTNAME")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        [Column("LASTNAME")]
        public string LastName { get; set; }

        [Required]
        [StringLength(300)]
        [Column("PASSWORD")]
        public string Password { get; set; }

        [Required]
        [StringLength(50)]
        [Column("EMAIL")]
        public string Email { get; set; }

        [Required]
        [Column("ROLE")]
        public ERoles Role { get; set; } = ERoles.USER;

        [Required]
        [Column("BLOCKED")]
        public bool Blocked { get; set; } = false;

        public List<Ticket>? Tickets { get; set; }

    }
}
