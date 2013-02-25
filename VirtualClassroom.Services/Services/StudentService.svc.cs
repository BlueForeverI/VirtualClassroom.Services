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
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "StudentService" in code, svc and config file together.
    public class StudentService : IStudentService
    {
        VirtualClassroomEntities entityContext = new VirtualClassroomEntities();

        public Student LoginStudent(string username, string password)
        {
            if (entityContext.Students.Count(s => s.Username == username) == 0)
            {
                return null;
            }

            Student entity = entityContext.Students.Where(s => s.Username == username).First();
            if (BCrypt.Net.BCrypt.Verify(password, entity.PasswordHash))
            {
                return entity;
            }

            return null;
        }

        public List<LessonView> GetLessonViewsByStudent(int studentId)
        {
            int classId = (from c in entityContext.Classes.Include("Students")
                           where c.Students.Any(s => s.Id == studentId)
                           select c.Id).First();

            var addedHomeworks = this.GetHomeworksByStudent(studentId).Select(h => h.LessonId);

            return (
                       from c in entityContext.Classes.Include("Subjects")
                       from s in c.Subjects
                       join l in entityContext.Lessons
                           on s.Id equals l.SubjectId
                       where c.Id == classId
                       select new LessonView()
                                  {
                                      Id = l.Id,
                                      Date = l.Date,
                                      HomeworkDeadline = l.HomeworkDeadline,
                                      HasHomework = (l.HomeworkDeadline == null) ? false : true,
                                      SentHomework = (addedHomeworks.Contains(l.Id)) ? true : false,
                                      Name = l.Name,
                                      Subject = s.Name
                                  }).ToList();
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

        public void AddHomework(Homework homework)
        {
            homework.Date = DateTime.Now;
            entityContext.Homeworks.Add(homework);
            entityContext.SaveChanges();
        }

        public List<Homework> GetHomeworksByStudent(int studentId)
        {
            int classId = (from c in entityContext.Classes.Include("Students")
                           where c.Students.Any(s => s.Id == studentId)
                           select c.Id).First();

            return (from c in entityContext.Classes.Include("Subjects")
                            from s in c.Subjects
                            from l in s.Lessons
                            from h in l.Homeworks
                            where c.Id == classId
                            select h).ToList();
        }

        public List<Mark> GetMarksByStudent(int studentId)
        {
            return (from h in entityContext.Homeworks.Include("Marks")
                            from m in h.Marks
                            where h.StudentId == studentId
                            select m).ToList();
        }
    }
}
