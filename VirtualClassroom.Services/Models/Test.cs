using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VirtualClassroom.Services.Models
{
    /// <summary>
    /// Represents a test for a subject
    /// </summary>
    public partial class Test
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int SubjectId { get; set; }
        public DateTime Date { get; set; }
        public int MaxScore { get; set; }

        public virtual ICollection<Question> Questions { get; set; }
    }
}