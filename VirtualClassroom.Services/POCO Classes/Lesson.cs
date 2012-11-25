using System;
using System.Collections.Generic;
using System.Data.Objects.DataClasses;
using System.Linq;
using System.Runtime.Serialization;
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
            Lesson lesson = new Lesson(
                entity.Id,
                entity.Name,
                entity.Date,
                entity.HomeworkDueDate,
                entity.HomeworkContent,
                (from h in entity.Homeworks select Homework.FromHomeworkEntity(h)).ToList(),
                Subject.FromSubjectEntity(entity.Subject)
            );

            return lesson;
        }

        internal static LessonEntity ToLessonEntity(Lesson lesson)
        {
            LessonEntity entity = new LessonEntity();
            entity.Id = lesson.Id;
            entity.Name = lesson.Name;
            entity.Date = lesson.Date;
            entity.HomeworkDueDate = lesson.HomeworkDueDate;
            entity.HomeworkContent = lesson.HomeworkContent;
            entity.Homeworks =
                (EntityCollection<HomeworkEntity>) (from h in lesson.Homeworks select Homework.ToHomeworkEntity(h));
            entity.Subject = Subject.ToSubjectEntity(lesson.Subject);

            return entity;
        }
    }
}
