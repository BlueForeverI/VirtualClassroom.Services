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
    public class Teacher
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string Username { get; set; }

        [DataMember]
        public string FirstName { get; set; }

        [DataMember]
        public string MiddleName { get; set; }

        [DataMember]
        public string LastName { get; set; }

        [DataMember]
        public string PasswordHash { get; set; }

        [DataMember]
        public List<Subject> Subjects { get; set; }

        
        //public Teacher(int id, string username, string firstName, string middleName,
        //    string lastName, string passwordHash, List<Subject> subjects)
        //{
        //    this.Id = id;
        //    this.Username = username;
        //    this.FirstName = firstName;
        //    this.MiddleName = middleName;
        //    this.LastName = lastName;
        //    this.PasswordHash = passwordHash;
        //    this.Subjects = subjects;
        //}

        //public static explicit operator Teacher(TeacherEntity entity)
        //{
        //    List<Subject> subjects = new List<Subject>();
        //    if (entity.Subjects != null)
        //    {
        //        foreach (var subjectEntity in entity.Subjects.ToList())
        //        {
        //            subjects.Add((Subject)subjectEntity);
        //        }
        //    }

        //    Teacher teacher = new Teacher(
        //        entity.Id,
        //        entity.Username,
        //        entity.FirstName,
        //        entity.MiddleName,
        //        entity.LastName,
        //        entity.PasswordHash,
        //        subjects
        //    );

        //    return teacher;
        //}

        //public static explicit operator  TeacherEntity(Teacher teacher)
        //{
        //    EntityCollection<SubjectEntity> subjectEntities = new EntityCollection<SubjectEntity>();
        //    if (teacher.Subjects != null)
        //    {
        //        foreach (var subject in teacher.Subjects)
        //        {
        //            subjectEntities.Add((SubjectEntity)subject);
        //        }
        //    }

        //    TeacherEntity entity = new TeacherEntity();
        //    entity.Id = teacher.Id;
        //    entity.Username = teacher.Username;
        //    entity.FirstName = teacher.FirstName;
        //    entity.MiddleName = teacher.MiddleName;
        //    entity.LastName = teacher.LastName;
        //    entity.PasswordHash = teacher.PasswordHash;
        //    entity.Subjects = subjectEntities;

        //    return entity;
        //}
    }
}
