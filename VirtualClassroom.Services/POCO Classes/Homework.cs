using System;
using System.Collections.Generic;
using System.Data.Objects.DataClasses;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace VirtualClassroom.Services.POCO_Classes
{
    [DataContract]
    public class Homework
    {
        [DataMember]
        public int Id { get; private set; }

        [DataMember]
        public byte[] Content { get; private set; }

        [DataMember]
        public DateTime Date { get; private set; }

        [DataMember]
        public Student Student { get; private set; }

        [DataMember]
        public Lesson Lesson { get; private set; }

        [DataMember]
        public Mark Mark { get; private set; }

        public Homework(int id, byte[] content, DateTime date, Student student,
            Lesson lesson, Mark mark)
        {
            this.Id = id;
            this.Content = content;
            this.Date = date;
            this.Student = student;
            this.Lesson = lesson;
            this.Mark = mark;
        }

        internal static Homework FromHomeworkEntity(HomeworkEntity entity)
        {
            Homework homework = new Homework(
                entity.Id,
                entity.Content,
                entity.Date,
                Student.FromStudentEntity(entity.Student),
                Lesson.FromLessonEntity(entity.Lesson),
                Mark.FromMarkEntity(entity.Marks.First())
            );

            return homework;
        }

        internal static HomeworkEntity ToHomeworkEntity(Homework homework)
        {
            HomeworkEntity entity = new HomeworkEntity();
            entity.Id = homework.Id;
            entity.Content = homework.Content;
            entity.Date = homework.Date;
            entity.Student = Student.ToStudentEntity(homework.Student);
            entity.Lesson = Lesson.ToLessonEntity(homework.Lesson);
            entity.Marks = new EntityCollection<MarkEntity>();
            entity.Marks.Add(Mark.ToMarkEntity(homework.Mark));

            return entity;
        }
    }
}
