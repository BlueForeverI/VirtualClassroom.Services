using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VirtualClassroom.Services.Views
{
    public class MarkView
    {
        public int Id { get; set; }
        public string Student { get; set; }
        public string Class { get; set; }
        public string Subject { get; set; }
        public string Lesson { get; set; }
        public DateTime Date { get; set; }
        public float Value { get; set; }
    }
}