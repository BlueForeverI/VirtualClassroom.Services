using System;
using System.Collections.Generic;
using System.Data.Objects.DataClasses;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace VirtualClassroom.Services.POCO_Classes
{
    [DataContract]
    public class Subject
    {
        [DataMember]
        public int Id { get; private set; }

        [DataMember]
        public string Name { get; private set; }

        [DataMember]
        public List<Lesson> Lessons { get; private set; }

        [DataMember]
        public List<Class> Classes { get; private set; }

        [DataMember]
        public Teacher Teacher { get; private set; }

        public Subject(int id, string name, List<Lesson> lessons, List<Class> classes,
            Teacher teacher)
        {
            this.Id = id;
            this.Name = name;
            this.Lessons = lessons;
            this.Classes = classes;
            this.Teacher = teacher;
        }

        internal static Subject FromSubjectEntity(SubjectEntity entity)
        {
            List<Lesson> lessons = new List<Lesson>();
            if(entity.Lessons != null)
            {
                lessons = (from l in entity.Lessons select Lesson.FromLessonEntity(l)).ToList();
            }

            List<Class> classes = new List<Class>();
            if(entity.Classes != null)
            {
                classes = (from c in entity.Classes select Class.FromClassEntity(c)).ToList();
            }

            Subject subject = new Subject(
                entity.Id,
                entity.Name,
                lessons,
                classes,
                Teacher.FromTeacherEntity(entity.Teacher)
            );

            return subject;
        }

        internal static SubjectEntity ToSubjectEntity(Subject subject)
        {
            EntityCollection<LessonEntity> lessonEntities = new EntityCollection<LessonEntity>();
            if(subject.Lessons != null)
            {
                lessonEntities = (EntityCollection<LessonEntity>)
                    (from l in subject.Lessons select Lesson.ToLessonEntity(l));
            }

            EntityCollection<ClassEntity> classEntities = new EntityCollection<ClassEntity>();
            if(subject.Classes != null)
            {
                classEntities = (EntityCollection<ClassEntity>)
                    (from c in subject.Classes select Class.ToClassEntity(c));
            }

            SubjectEntity entity = new SubjectEntity();
            entity.Id = subject.Id;
            entity.Name = subject.Name;
            entity.Lessons = lessonEntities;
            entity.Classes = classEntities;
            entity.Teacher = Teacher.ToTeacherEntity(subject.Teacher);

            return entity;
        }
    }
}