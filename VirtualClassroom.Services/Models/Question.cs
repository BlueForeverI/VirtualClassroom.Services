using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VirtualClassroom.Services.Models
{
    /// <summary>
    /// Represents a question from a test
    /// </summary>
    public partial class Question
    {
        public Question()
        {
            this.Answers = new HashSet<Answer>();
        }

        public int Id { get; set; }
        public string Text { get; set; }
        public int TestId { get; set; }
        public Answer SelectedAnswer { get; set; }

        public virtual ICollection<Answer> Answers { get; set; }
    }
}