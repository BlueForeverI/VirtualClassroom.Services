using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VirtualClassroom.Services.Views
{
    /// <summary>
    /// Holds the required information to display about a subject
    /// </summary>
    public class SubjectView
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string TeacherFullName { get; set; }
    }
}