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
    public class Student
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
        public string EGN { get; set; }

        [DataMember]
        public string PasswordHash { get; private set; }

        [DataMember]
        public Class Class { get; private set; }

        [DataMember]
        public List<Homework> Homeworks { get; private set; }

        public Student(int id, string username, string firstName, string middleName,
            string lastName, string egn, string passwordHash, Class c, List<Homework> homeworks)
        {
            this.Id = id;
            this.Username = username;
            this.FirstName = firstName;
            this.MiddleName = middleName;
            this.LastName = lastName;
            this.EGN = egn;
            this.PasswordHash = passwordHash;
            this.Class = c;
            this.Homeworks = homeworks;
        }

        public static explicit operator Student(StudentEntity entity)
        {
            List<Homework> homeworks = new List<Homework>();
            if (entity.Homeworks != null)
            {
                foreach(var homeworkEntity in entity.Homeworks.ToList())
                {
                    homeworks.Add((Homework)homeworkEntity);
                }
            }

            Student student = new Student(
                entity.Id,
                entity.Username,
                entity.FirstName,
                entity.MiddleName,
                entity.LastName,
                entity.EGN,
                entity.PasswordHash,
                (Class)entity.Class,
                homeworks
            );

            return student;
        }

        public static explicit operator StudentEntity(Student student)
        {
            EntityCollection<HomeworkEntity> homeworkEntities = new EntityCollection<HomeworkEntity>();
            if (student.Homeworks != null)
            {
                foreach(var homework in student.Homeworks)
                {
                    homeworkEntities.Add((HomeworkEntity)homework);
                }
            }

            StudentEntity entity = new StudentEntity();
            entity.Id = student.Id;
            entity.Username = student.Username;
            entity.FirstName = student.FirstName;
            entity.MiddleName = student.MiddleName;
            entity.LastName = student.LastName;
            entity.EGN = student.EGN;
            entity.PasswordHash = student.PasswordHash;
            entity.Class = (ClassEntity)student.Class;
            entity.Homeworks = homeworkEntities;

            return entity;
        }
    }
}
