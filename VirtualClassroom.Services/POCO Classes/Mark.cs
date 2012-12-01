using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace VirtualClassroom.Services.POCO_Classes
{
    [DataContract]
    public class Mark
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public int Value { get; set; }

        [DataMember]
        public Homework Homework { get; set; }

        public Mark(int id, int value, Homework homework)
        {
            this.Id = id;
            this.Value = value;
            this.Homework = homework;
        }

        public static explicit operator Mark(MarkEntity entity)
        {
            Mark mark = new Mark(
                entity.Id,
                entity.Value,
                (Homework)entity.Homework
            );

            return mark;
        }

        public static explicit operator MarkEntity(Mark mark)
        {
            MarkEntity entity = new MarkEntity();
            entity.Id = mark.Id;
            entity.Value = mark.Value;
            entity.Homework = (HomeworkEntity)mark.Homework;

            return entity;
        }
    }
}
