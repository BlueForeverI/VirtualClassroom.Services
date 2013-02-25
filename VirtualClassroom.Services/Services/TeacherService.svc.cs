using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using VirtualClassroom.Services.Models;
using VirtualClassroom.Services.Views;

namespace VirtualClassroom.Services.Services
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "TeacherService" in code, svc and config file together.
    public class TeacherService : ITeacherService
    {
        private VirtualClassroomEntities entityContext = new VirtualClassroomEntities();

        public Teacher LoginTeacher(string username, string password)
        {
            if (entityContext.Teachers.Count(s => s.Username == username) == 0)
            {
                return null;
            }

            Teacher entity = entityContext.Teachers.Where(s => s.Username == username).First();
            if (BCrypt.Net.BCrypt.Verify(password, entity.PasswordHash))
            {
                return entity;
            }

            return null;
        }

        public void AddLesson(Lesson lesson)
        {
            //lesson.Date = DateTime.Now;
            entityContext.Lessons.Add(lesson);
            entityContext.SaveChanges();
        }

        public void RemoveLessons(List<Lesson> lessons)
        {
            int[] ids = (from l in lessons select l.Id).ToArray();

            var entities = (from l in entityContext.Lessons
                            where ids.Contains(l.Id)
                            select l).ToList();

            foreach (var entity in entities)
            {
                entityContext.Lessons.Remove(entity);
            }

            entityContext.SaveChanges();
        }

        public List<HomeworkView> GetHomeworkViewsByTeacher(int teacherId)
        {
            var homeworksWithMarks = (from m in entityContext.Marks select m.HomeworkId).ToList();
            var entities =  (
                from s in entityContext.Subjects
                join l in entityContext.Lessons
                    on s.Id equals l.SubjectId
                join h in entityContext.Homeworks
                    on l.Id equals h.LessonId
                join st in entityContext.Students
                    on h.StudentId equals st.Id
                where s.TeacherId == teacherId
                select new HomeworkView()
                {
                    Id = h.Id,
                    Lesson = l.Name,
                    StudentFullName = st.FirstName + " " + st.MiddleName + " " + st.LastName,
                    Subject = s.Name,
                    HasMark = homeworksWithMarks.Contains(h.Id)
                }).ToList();
            

            return entities;
        }

        public List<LessonView> GetLessonViewsByTeacher(int teacherId)
        {
            return (
                       from s in entityContext.Subjects
                       join l in entityContext.Lessons
                           on s.Id equals l.SubjectId
                       where s.TeacherId == teacherId
                       select new LessonView()
                                  {
                                      Id = l.Id,
                                      Date = l.Date,
                                      HomeworkDeadline = l.HomeworkDeadline,
                                      Name = l.Name,
                                      Subject = s.Name
                                  }).ToList();
        }

        public List<Subject> GetSubjectsByTeacher(int teacherId)
        {
            return (from s in entityContext.Subjects
                    where s.TeacherId == teacherId
                    select s).ToList();
        }

        public List<MarkView> GetMarkViewsByTeacher(int teacherId)
        {
            var markViews = (from m in entityContext.Marks
                             join h in entityContext.Homeworks on m.HomeworkId equals h.Id
                             join st in entityContext.Students on h.StudentId equals st.Id
                             join l in entityContext.Lessons on h.LessonId equals l.Id
                             join sub in entityContext.Subjects on l.SubjectId equals sub.Id
                             join c in entityContext.Classes on st.ClassId equals c.Id
                             join t in entityContext.Teachers on sub.TeacherId equals teacherId
                             select  new 
                             {
                                Id = m.Id,
                                Student = st.FirstName + " " + st.MiddleName + " " + st.LastName,
                                ClassNumber = c.Number,
                                ClassLetter = c.Letter,
                                Subject = sub.Name,
                                Lesson = l.Name,
                                Date = m.Date,
                                Value = m.Value
                             })
                .AsEnumerable().Distinct()
                .Select(m => new MarkView()
                {
                    Class = string.Format("{0} '{1}'", m.ClassNumber, m.ClassLetter),
                    Id = m.Id,
                    Date = m.Date,
                    Lesson = m.Lesson,
                    Student = m.Student,
                    Subject = m.Subject,
                    Value = m.Value
                }).ToList();

            return markViews;
        }

        public void AddMark(Mark mark)
        {
            mark.Date = DateTime.Now;
            mark.SubjectName = (from sub in entityContext.Subjects.Include("Lessons")
                                from l in sub.Lessons
                                from h in l.Homeworks
                                where h.Id == mark.HomeworkId
                                select sub.Name).First();

            mark.LessonName = (from l in entityContext.Lessons.Include("Homworks")
                               where l.Homeworks.Any(h => h.Id == mark.HomeworkId)
                               select l.Name).First();

            entityContext.Marks.Add(mark);
            entityContext.SaveChanges();
        }

        public File DownloadLessonContent(int lessonId)
        {
            Lesson lesson = (from l in entityContext.Lessons
                             where l.Id == lessonId
                             select l).First();

            return new File(lesson.ContentFilename, lesson.Content);
        }

        public File DownloadLessonHomework(int lessonId)
        {
            Lesson lesson = (from l in entityContext.Lessons
                             where l.Id == lessonId
                             select l).First();

            return new File(lesson.HomeworkFilename, lesson.HomeworkContent);
        }

        public File DownloadSubmittedHomework(int homeworkId)
        {
            var entity = (from h in entityContext.Homeworks
                          where h.Id == homeworkId
                          select h).First();

            return new File(entity.Filename, entity.Content);
        }
    }
}
