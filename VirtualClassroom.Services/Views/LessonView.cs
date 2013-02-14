using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VirtualClassroom.Services.Views
{
    public class LessonView
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Subject { get; set; }
        public DateTime Date { get; set; }
        public DateTime? HomeworkDeadline { get; set; }
    }
}