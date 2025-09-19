using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApiJobs.Data.Entities
{
    [Table("EmailAttachments")]
    public class EmailAttachmentEntity
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey(nameof(Email))]
        public int FkEmail { get; set; }
        public EmailEntity? Email { get; set; }

        public string FileName { get; set; }
        public byte[] FileBytes { get; set; }
        public string FileContentType { get; set; }

        public EmailAttachmentEntity()
        {
            FileName = string.Empty;
            FileBytes = [];
            FileContentType = string.Empty;
        }
    }
}
