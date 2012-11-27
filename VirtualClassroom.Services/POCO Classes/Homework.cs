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
            Mark mark = null;
            if(entity.Marks != null)
            {
                if(entity.Marks.Count > 0)
                {
                    mark = Mark.FromMarkEntity(entity.Marks.First());
                }
            }

            Homework homework = new Homework(
                entity.Id,
                entity.Content,
                entity.Date,
                Student.FromStudentEntity(entity.Student),
                Lesson.FromLessonEntity(entity.Lesson),
                mark
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
            if (homework.Mark != null)
            {
                entity.Marks.Add(Mark.ToMarkEntity(homework.Mark));
            }

            return entity;
        }

        [OperationContract]
        public static Homework CreateInstance(int id, byte[] content, DateTime date, Student student,
            Lesson lesson, Mark mark)
        {
            return new Homework(id, content, date, student, lesson, mark);
        }
    }
}
