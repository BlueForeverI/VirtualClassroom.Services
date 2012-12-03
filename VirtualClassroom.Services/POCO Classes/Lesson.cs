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
        public int Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public DateTime Date { get; set; }

        [DataMember]
        public DateTime? HomeworkDueDate { get; set; }

        [DataMember]
        public byte[] HomeworkContent { get; set; }

        [DataMember]
        public List<Homework> Homeworks { get; set; }

        [DataMember]
        public int SubjectId { get; private set; }

        public Lesson(int id, string name, DateTime date, DateTime? homeworkDueDate,
            byte[] homeworkContent, List<Homework> homeworks, int subjectId)
        {
            this.Id = id;
            this.Name = name;
            this.Date = date;
            this.HomeworkDueDate = homeworkDueDate;
            this.HomeworkContent = homeworkContent;
            this.Homeworks = homeworks;
            this.SubjectId = subjectId;
        }

        public static explicit operator Lesson(LessonEntity entity)
        {
            List<Homework> homeworks = new List<Homework>();
            if (entity.Homeworks != null)
            {
                foreach (var homeworkEntity in entity.Homeworks.ToList())
                {
                    homeworks.Add((Homework)homeworkEntity);                    
                }
            }

            Lesson lesson = new Lesson(
                entity.Id,
                entity.Name,
                entity.Date,
                entity.HomeworkDueDate,
                entity.HomeworkContent,
                homeworks,
                entity.SubjectId
            );

            return lesson;
        }

        public static explicit operator LessonEntity(Lesson lesson)
        {
            EntityCollection<HomeworkEntity> homeworkEntities = new EntityCollection<HomeworkEntity>();
            if (lesson.Homeworks != null)
            {
                foreach(var homework in lesson.Homeworks)
                {
                    homeworkEntities.Add((HomeworkEntity)homework);
                }
            }

            LessonEntity entity = new LessonEntity();
            entity.Id = lesson.Id;
            entity.Name = lesson.Name;
            entity.Date = lesson.Date;
            entity.HomeworkDueDate = lesson.HomeworkDueDate;
            entity.HomeworkContent = lesson.HomeworkContent;
            entity.Homeworks = homeworkEntities;
            entity.SubjectId =lesson.SubjectId;

            return entity;
        }
    }
}
