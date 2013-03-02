using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using VirtualClassroom.Services.Models;
using VirtualClassroom.Services.Views;
using System.Security.Cryptography;

namespace VirtualClassroom.Services.Services
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession, ConcurrencyMode = ConcurrencyMode.Single)]
    public class StudentService : IStudentService
    {
        VirtualClassroomEntities entityContext = new VirtualClassroomEntities();
        private bool isLogged = false;

        private void CheckAuthentication()
        {
            if (isLogged == false)
            {
                throw new FaultException("Не сте влезли в системата");
            }
        }

        public Student LoginStudent(string usernameCrypt, string passwordCrypt, string secret)
        {
            if (string.IsNullOrWhiteSpace(usernameCrypt) || string.IsNullOrEmpty(usernameCrypt)
                || string.IsNullOrWhiteSpace(passwordCrypt) || string.IsNullOrEmpty(passwordCrypt)
                || string.IsNullOrWhiteSpace(secret) || string.IsNullOrEmpty(secret))
            {
                return null;
            }

            string username = Crypto.DecryptStringAES(usernameCrypt, secret);
            string password = Crypto.DecryptStringAES(passwordCrypt, secret);

            if (entityContext.Students.Count(s => s.Username == username) == 0)
            {
                return null;
            }

            Student entity = entityContext.Students.Where(s => s.Username == username).First();
            if (BCrypt.Net.BCrypt.Verify(password, entity.PasswordHash))
            {
                isLogged = true;
                return entity;
            }

            return null;
        }

        public List<LessonView> GetLessonViewsByStudent(int studentId)
        {
            CheckAuthentication();

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
            CheckAuthentication();

            Lesson lesson = (from l in entityContext.Lessons
                             where l.Id == lessonId
                             select l).First();

            return new File(lesson.ContentFilename, lesson.Content);
        }

        public File DownloadLessonHomework(int lessonId)
        {
            CheckAuthentication();

            Lesson lesson = (from l in entityContext.Lessons
                             where l.Id == lessonId
                             select l).First();

            return new File(lesson.HomeworkFilename, lesson.HomeworkContent);
        }

        public void AddHomework(Homework homework)
        {
            CheckAuthentication();

            homework.Date = DateTime.Now;
            entityContext.Homeworks.Add(homework);
            entityContext.SaveChanges();
        }

        public List<Homework> GetHomeworksByStudent(int studentId)
        {
            CheckAuthentication();

            int classId = (from c in entityContext.Classes.Include("Students")
                           where c.Students.Any(s => s.Id == studentId)
                           select c.Id).First();

            return (from c in entityContext.Classes.Include("Subjects")
                            from s in c.Subjects
                            from l in s.Lessons
                            from h in l.Homeworks
                            where c.Id == classId && h.StudentId == studentId
                            select h).ToList();
        }

        public List<Mark> GetMarksByStudent(int studentId)
        {
            CheckAuthentication();

            return (from h in entityContext.Homeworks.Include("Marks")
                            from m in h.Marks
                            where h.StudentId == studentId
                            select m).ToList();
        }
    }
}
