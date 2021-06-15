using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Subtitles.Models
{
    public class PhraseTranslation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public string Content { get; set; }

        public int VotesCount { get; set; }

        public long PhraseId { get; set; }

        [JsonIgnore]
        public Phrase Phrase { get; set; }
    }
}
