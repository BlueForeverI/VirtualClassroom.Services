using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace VirtualClassroom.Services.POCO_Classes
{
    [DataContract]
    public class Mark
    {
        [DataMember]
        public int Id { get; private set; }

        [DataMember]
        public int Value { get; private set; }

        [DataMember]
        public Homework Homework { get; private set; }

        public Mark(int id, int value, Homework homework)
        {
            this.Id = id;
            this.Value = value;
            this.Homework = homework;
        }

        internal static Mark FromMarkEntity(MarkEntity entity)
        {
            Mark mark = new Mark(
                entity.Id,
                entity.Value,
                Homework.FromHomeworkEntity(entity.Homework)
            );

            return mark;
        }

        internal static MarkEntity ToMarkEntity(Mark mark)
        {
            MarkEntity entity = new MarkEntity();
            entity.Id = mark.Id;
            entity.Value = mark.Value;
            entity.Homework = Homework.ToHomeworkEntity(mark.Homework);

            return entity;
        }
    }
}
