//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace VirtualClassroom.Services
{
    public partial class Subject
    {
        public Subject()
        {
            this.Lessons = new HashSet<Lesson>();
            this.Classes = new HashSet<Class>();
        }
    
        public int Id { get; set; }
        public string Name { get; set; }
        public int TeacherId { get; set; }
    
        public virtual ICollection<Lesson> Lessons { get; set; }
        public virtual ICollection<Class> Classes { get; set; }
    }
    
}
