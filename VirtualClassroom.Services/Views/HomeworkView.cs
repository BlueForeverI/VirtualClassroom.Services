using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VirtualClassroom.Services.Views
{
    /// <summary>
    /// Holds the required information to display about a homework
    /// </summary>
    public class HomeworkView
    {
        public int Id { get; set; }
        public string Subject { get; set; }
        public string Lesson { get; set; }
        public string StudentFullName { get; set; }
        public DateTime Date { get; set; }
        public bool HasMark { get; set; }
    }
}