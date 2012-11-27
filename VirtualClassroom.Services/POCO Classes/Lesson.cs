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
    public class Lesson
    {
        [DataMember]
        public int Id { get; private set; }

        [DataMember]
        public string Name { get; private set; }

        [DataMember]
        public DateTime Date { get; private set; }

        [DataMember]
        public DateTime? HomeworkDueDate { get; private set; }

        [DataMember]
        public byte[] HomeworkContent { get; private set; }

        [DataMember]
        public List<Homework> Homeworks { get; private set; }

        [DataMember]
        public Subject Subject { get; private set; }

        public Lesson(int id, string name, DateTime date, DateTime? homeworkDueDate,
            byte[] homeworkContent, List<Homework> homeworks, Subject subject)
        {
            this.Id = id;
            this.Name = name;
            this.Date = date;
            this.HomeworkDueDate = homeworkDueDate;
            this.HomeworkContent = homeworkContent;
            this.Homeworks = homeworks;
            this.Subject = subject;
        }

        internal static Lesson FromLessonEntity(LessonEntity entity)
        {
            List<Homework> homeworks = new List<Homework>();
            if(entity.Homeworks != null)
            {
                homeworks = (from h in entity.Homeworks select Homework.FromHomeworkEntity(h)).ToList();
            }

            Lesson lesson = new Lesson(
                entity.Id,
                entity.Name,
                entity.Date,
                entity.HomeworkDueDate,
                entity.HomeworkContent,
                homeworks,
                Subject.FromSubjectEntity(entity.Subject)
            );

            return lesson;
        }

        internal static LessonEntity ToLessonEntity(Lesson lesson)
        {
            EntityCollection<HomeworkEntity> homeworkEntities = new EntityCollection<HomeworkEntity>();
            if(lesson.Homeworks != null)
            {
                homeworkEntities = (EntityCollection<HomeworkEntity>)
                    (from h in lesson.Homeworks select Homework.ToHomeworkEntity(h));
            }

            LessonEntity entity = new LessonEntity();
            entity.Id = lesson.Id;
            entity.Name = lesson.Name;
            entity.Date = lesson.Date;
            entity.HomeworkDueDate = lesson.HomeworkDueDate;
            entity.HomeworkContent = lesson.HomeworkContent;
            entity.Homeworks = homeworkEntities;
            entity.Subject = Subject.ToSubjectEntity(lesson.Subject);

            return entity;
        }

        [OperationContract]
        public static Lesson CreateInstance(int id, string name, DateTime date, DateTime? homeworkDueDate,
            byte[] homeworkContent, List<Homework> homeworks, Subject subject)
        {
            return new Lesson(id, name, date, homeworkDueDate, homeworkContent, homeworks, subject);
        }
    }
}
