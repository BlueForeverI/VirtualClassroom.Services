using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VirtualClassroom.Services.Views
{
    /// <summary>
    /// Holds the required information to display about a lesson
    /// </summary>
    public class LessonView
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Subject { get; set; }
        public DateTime Date { get; set; }
        public DateTime? HomeworkDeadline { get; set; }
        public bool HasHomework { get; set; }
        public bool SentHomework { get; set; }
    }
}