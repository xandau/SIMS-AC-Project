using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using WebAPI.Enums;
using WebAPI.Models;

[Table("USERS")]
public class User
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("UserID")]
    public long UserID { get; set; }

    [Column("User_UUID", TypeName = "VARCHAR(300)")]
    public Guid? UserUUID { get; set; }

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

    [Column("PASSWORD_HASH")]
    public byte[]? PasswordHash { get; set; }

    [Column("PASSWORD_SALT")]
    public byte[]? PasswordSalt { get; set; }

    [Required]
    [StringLength(50)]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    [Column("EMAIL")]
    public string Email { get; set; }

    [Column("ROLE")]
    public ERoles? Role { get; set; }

    [Column("BLOCKED")]
    public bool? Blocked { get; set; }

    public List<Ticket>? Tickets { get; set; }

    [NotMapped]
    public string Password { get; set; }

    //method for hashing passwords
    public void SetPassword(string password)
    {
        PasswordSalt = new byte[16];
        RandomNumberGenerator.Fill(PasswordSalt); // Generate a random salt

        // Hash the password with salt
        this.PasswordHash = HashPassword(password, PasswordSalt);
    }

    // Verify password
    public bool VerifyPassword(string password)
    {
        var hash = HashPassword(password, PasswordSalt);
        return PasswordHash.SequenceEqual(hash);
    }

    // Hash password method using PBKDF2
    private byte[] HashPassword(string password, byte[] salt)
    {
        return KeyDerivation.Pbkdf2(
            password: password,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 10000,
            numBytesRequested: 32
        );
    }
}
