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
            Subject subject = new Subject(
                entity.Id,
                entity.Name,
                (from l in entity.Lessons select Lesson.FromLessonEntity(l)).ToList(),
                (from c in entity.Classes select Class.FromClassEntity(c)).ToList(),
                Teacher.FromTeacherEntity(entity.Teacher)
            );

            return subject;
        }

        internal static SubjectEntity ToSubjectEntity(Subject subject)
        {
            SubjectEntity entity = new SubjectEntity();
            entity.Id = subject.Id;
            entity.Name = subject.Name;
            entity.Lessons =
                (EntityCollection<LessonEntity>) (from l in subject.Lessons select Lesson.ToLessonEntity(l));
            entity.Classes = (EntityCollection<ClassEntity>) (from c in subject.Classes select Class.ToClassEntity(c));
            entity.Teacher = Teacher.ToTeacherEntity(subject.Teacher);

            return entity;
        }
    }
}