using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VirtualClassroom.Services.Views
{
    public class HomeworkView
    {
        public int Id { get; set; }
        public string Subject { get; set; }
        public string Lesson { get; set; }
        public string StudentFullName { get; set; }
    }
}