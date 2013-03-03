using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VirtualClassroom.Services.Views
{
    /// <summary>
    /// Holds the required information to display about a student
    /// </summary>
    public class StudentView
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public string EGN { get; set; }
        public string Class { get; set; }
    }
}