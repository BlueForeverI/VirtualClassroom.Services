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
        public int Id { get; private set; }

        [DataMember]
        public int Number { get; private set; }

        [DataMember]
        public char Letter { get; private set; }

        [DataMember]
        public List<Student> Students { get; private set; }

        [DataMember]
        public List<Subject> Subjects { get; private set; }

        public Class(int id, int number, char letter, List<Student> students,
            List<Subject> subjects)
        {
            this.Id = id;
            this.Number = number;
            this.Letter = letter;
            this.Students = students;
            this.Subjects = subjects;
        }

        internal static Class FromClassEntity(ClassEntity entity)
        {
            List<Student> students = new List<Student>();
            if(entity.Students != null)
            {
                students = (from s in entity.Students select Student.FromStudentEntity(s)).ToList();
            }

            List<Subject> subjects = new List<Subject>();
            if(entity.Subjects != null)
            {
                subjects = (from s in entity.Subjects select Subject.FromSubjectEntity(s)).ToList();
            }

            Class cl = new Class(
                entity.Id,
                entity.Number,
                entity.Letter.First(),
                students,
                subjects
            );

            return cl;
        }

        internal static ClassEntity ToClassEntity(Class cl)
        {
            EntityCollection<StudentEntity> studentEntities = new EntityCollection<StudentEntity>();
            if(cl.Students != null)
            {
                studentEntities = (EntityCollection<StudentEntity>)
                    (from s in cl.Students select Student.ToStudentEntity(s));
            }

            EntityCollection<SubjectEntity> subjectEntities = new EntityCollection<SubjectEntity>();
            if(cl.Subjects != null)
            {
                subjectEntities =  (EntityCollection<SubjectEntity>) 
                    (from s in cl.Subjects select Subject.ToSubjectEntity(s));
            }

            ClassEntity entity = new ClassEntity();
            entity.Id = cl.Id;
            entity.Number = cl.Number;
            entity.Letter = cl.Letter.ToString();
            entity.Students = studentEntities;
            entity.Subjects = subjectEntities;

            return entity;
        }

        [OperationContract]
        public static Class CreateInstance(int id, int number, char letter, List<Student> students,
            List<Subject> subjects)
        {
            return new Class(id, number, letter, students, subjects);
        }
    }
}
