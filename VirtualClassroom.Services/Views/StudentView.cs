using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VirtualClassroom.Services.Views
{
    public class StudentView
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public string EGN { get; set; }
        public string Class { get; set; }
    }
}