using System;
using System.Collections.Generic;
using System.Data.Objects.DataClasses;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace VirtualClassroom.Services.POCO_Classes
{
    [DataContract]
    public class Class
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public int Number { get; set; }

        [DataMember]
        public char Letter { get; set; }

        [DataMember]
        public List<Student> Students { get; set; }

        [DataMember]
        public List<Subject> Subjects { get; set; }

        //public Class(int id, int number, char letter, List<Student> students,
        //    List<Subject> subjects)
        //{
        //    this.Id = id;
        //    this.Number = number;
        //    this.Letter = letter;
        //    this.Students = students;
        //    this.Subjects = subjects;
        //}

        //public static explicit operator Class(ClassEntity entity)
        //{
        //    List<Student> students = new List<Student>();
        //    foreach (var studentEntity in entity.Students.ToList())
        //    {
        //        students.Add((Student)studentEntity);
        //    }
            
        //    List<Subject> subjects = new List<Subject>();
        //    foreach (var subjectEntity in entity.Subjects.ToList())
        //    {
        //        subjects.Add((Subject)subjectEntity);
        //    }
             

        //    Class cl = new Class(
        //        entity.Id,
        //        entity.Number,
        //        entity.Letter.First(),
        //        students,
        //        subjects
        //    );

        //    return cl;
        //}

        //public static explicit operator ClassEntity(Class cl)
        //{
        //    EntityCollection<StudentEntity> studentEntities = new EntityCollection<StudentEntity>();
        //    if (cl.Students != null)
        //    {
        //        foreach (var student in cl.Students)
        //        {
        //            studentEntities.Add((StudentEntity)student);
        //        }
        //    }

        //    EntityCollection<SubjectEntity> subjectEntities = new EntityCollection<SubjectEntity>();
        //    if (cl.Subjects != null)
        //    {
        //        foreach(var subject in cl.Subjects)
        //        {
        //            subjectEntities.Add((SubjectEntity)subject);
        //        }
        //    }

        //    ClassEntity entity = new ClassEntity();
        //    entity.Id = cl.Id;
        //    entity.Number = cl.Number;
        //    entity.Letter = cl.Letter.ToString();
        //    entity.Students = studentEntities;
        //    entity.Subjects = subjectEntities;

        //    return entity;
        //}
    }
}
