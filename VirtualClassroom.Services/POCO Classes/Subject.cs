using System;
using System.Collections.Generic;
using System.Data.Objects.DataClasses;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Web;

namespace VirtualClassroom.Services.POCO_Classes
{
    [DataContract]
    public class Subject
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public List<Lesson> Lessons { get; set; }

        [DataMember]
        public List<Class> Classes { get; set; }

        [DataMember]
        public int TeacherId { get; set; }

        public Subject(int id, string name, List<Lesson> lessons, List<Class> classes,
            int teacherId)
        {
            this.Id = id;
            this.Name = name;
            this.Lessons = lessons;
            this.Classes = classes;
            this.TeacherId = teacherId;
        }

        public static explicit operator Subject(SubjectEntity entity)
        {
            List<Lesson> lessons = new List<Lesson>();
            if (entity.Lessons != null)
            {
                foreach(var lessonEntity in entity.Lessons.ToList())
                {
                    lessons.Add((Lesson)lessonEntity);
                }
            }

            List<Class> classes = new List<Class>();
            if (entity.Classes != null)
            {
                foreach(var classEntity in entity.Classes.ToList())
                {
                    classes.Add((Class)classEntity);
                }
            }

            Subject subject = new Subject(
                entity.Id,
                entity.Name,
                lessons,
                classes,
                entity.TeacherId
            );

            return subject;
        }

        public static explicit operator SubjectEntity(Subject subject)
        {
            EntityCollection<LessonEntity> lessonEntities = new EntityCollection<LessonEntity>();
            if (subject.Lessons != null)
            {
                foreach (var lesson in subject.Lessons)
                {
                    lessonEntities.Add((LessonEntity)lesson);
                }
            }

            EntityCollection<ClassEntity> classEntities = new EntityCollection<ClassEntity>();
            if (subject.Classes != null)
            {
                foreach (var cl in subject.Classes)
                {
                    classEntities.Add((ClassEntity)cl);
                }
            }

            SubjectEntity entity = new SubjectEntity();
            entity.Id = subject.Id;
            entity.Name = subject.Name;
            entity.Lessons = lessonEntities;
            entity.Classes = classEntities;
            entity.TeacherId = subject.TeacherId;

            return entity;
        }
    }
}