using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace PikaStatus.Models
{
    public class MessageEntity
    {
        [Required]
        [NotNull]
        public int Id { get; set; }
        
        [Required]
        [NotNull]
        [DisplayName("Date created")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        [Required]
        [DisplayName("Date last updated")]
        public DateTime UpdatedAt { get; set; }

        [Required] 
        [NotNull] 
        [StringLength(1000, ErrorMessage = "Message should not be longer than {1} and has at least {2}", MinimumLength = 10)]
        [DisplayName("Message content")]
        public string Message { get; set; } = "";
        
        [Required]
        [DisplayName("Message type")]
        public MessageType MessageType { get; set; } = MessageType.None;
        
        [Required]
        [DisplayName("Message related issues")]
        public IList<IssueEntity> RelatedIssues { get; set; } = new List<IssueEntity>();
    }
}