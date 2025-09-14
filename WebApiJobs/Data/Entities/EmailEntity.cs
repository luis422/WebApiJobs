using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApiJobs.Data.Entities
{
    public enum EEmailStatus
    {
        Pending = 0,
        Sent = 1,
        Fail = 2
    }

    [Table("Emails")]
    [Index(nameof(Status), Name = "i_emails_status")]
    public class EmailEntity
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Receiver { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
        public EEmailStatus Status { get; set; }
        public DateTime? SentAt { get; set; }
        public DateTime CreatedAt { get; set; }

        public EmailEntity()
        {
            Receiver = "";
            Subject = "";
            Content = "";
            Status = EEmailStatus.Pending;
            CreatedAt = DateTime.UtcNow;
        }
    }
}
