using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Subtitles.Models
{
    public class Phrase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public int SequenceNumber { get; set; }

        public string Content { get; set; }

        public int MovieId { get; set; }
        public Movie Movie { get; set; }

        public List<PhraseTranslation> PhraseTranslations { get; set; }
    }
}
