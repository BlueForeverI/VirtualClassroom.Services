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
    public partial class Homework
    {
        public Homework()
        {
            this.Marks = new HashSet<Mark>();
        }
    
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int LessonId { get; set; }
        public byte[] Content { get; set; }
        public System.DateTime Date { get; set; }
    
        public virtual ICollection<Mark> Marks { get; set; }
    }
    
}
