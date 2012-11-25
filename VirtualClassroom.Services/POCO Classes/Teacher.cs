using System;
using System.Collections.Generic;
using System.Data.Objects.DataClasses;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace VirtualClassroom.Services.POCO_Classes
{
    [DataContract]
    public class Teacher
    {
        [DataMember]
        public int Id { get; private set; }

        [DataMember]
        public string Username { get; private set; }

        [DataMember]
        public string FirstName { get; private set; }

        [DataMember]
        public string MiddleName { get; private set; }

        [DataMember]
        public string LastName { get; private set; }

        [DataMember]
        public string PasswordHash { get; private set; }

        [DataMember]
        public List<Subject> Subjects { get; private set; }

        public Teacher(int id, string username, string firstName, string middleName,
            string lastName, string passwordHash, List<Subject> subjects)
        {
            this.Id = id;
            this.Username = username;
            this.FirstName = firstName;
            this.MiddleName = middleName;
            this.LastName = lastName;
            this.PasswordHash = passwordHash;
            this.Subjects = subjects;
        }

        internal static Teacher FromTeacherEntity(TeacherEntity entity)
        {
            Teacher teacher = new Teacher(
                entity.Id,
                entity.Username,
                entity.FirstName,
                entity.MiddleName,
                entity.LastName,
                entity.PasswordHash,
                (from s in entity.Subjects
                 select Subject.FromSubjectEntity(s)).ToList()
            );

            return teacher;
        }

        internal static TeacherEntity ToTeacherEntity(Teacher teacher)
        {
            TeacherEntity entity = new TeacherEntity();
            entity.Id = teacher.Id;
            entity.Username = teacher.Username;
            entity.FirstName = teacher.FirstName;
            entity.MiddleName = teacher.MiddleName;
            entity.LastName = teacher.LastName;
            entity.PasswordHash = teacher.PasswordHash;
            entity.Subjects =
                (EntityCollection<SubjectEntity>) (from s in teacher.Subjects select Subject.ToSubjectEntity(s));

            return entity;
        }
    }
}
