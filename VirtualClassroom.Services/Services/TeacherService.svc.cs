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
    /// <summary>
    /// Implementation of the ITeacherService
    /// </summary>
    
    //enable sessions
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession, ConcurrencyMode = ConcurrencyMode.Single)]
    public class TeacherService : ITeacherService
    {
        private VirtualClassroomEntities entityContext = new VirtualClassroomEntities();
        private bool isLogged = false;          //stores login state

        /// <summary>
        /// Checks if the client is authenticated
        /// </summary>
        private void CheckAuthentication()
        {
            if (isLogged == false)
            {
                throw new FaultException("Не сте влезли в системата");
            }
        }

        /// <summary>
        /// Logs a teacher into the system
        /// </summary>
        /// <param name="usernameCrypt">Encrypted username</param>
        /// <param name="passwordCrypt">Encrypted password</param>
        /// <param name="secret">The key to decrypt with</param>
        /// <returns>Teacher information (if successfull login)</returns>
        public Teacher LoginTeacher(string usernameCrypt, string passwordCrypt, string secret)
        {
            if (string.IsNullOrWhiteSpace(usernameCrypt) || string.IsNullOrEmpty(usernameCrypt)
                || string.IsNullOrWhiteSpace(passwordCrypt) || string.IsNullOrEmpty(passwordCrypt)
                || string.IsNullOrWhiteSpace(secret) || string.IsNullOrEmpty(secret))
            {
                return null;
            }

            //decrypt login details
            string username = Crypto.DecryptStringAES(usernameCrypt, secret);
            string password = Crypto.DecryptStringAES(passwordCrypt, secret);

            if (entityContext.Teachers.Count(s => s.Username == username) == 0)
            {
                return null;
            }

            Teacher entity = entityContext.Teachers.Where(s => s.Username == username).First();
            if (BCrypt.Net.BCrypt.Verify(password, entity.PasswordHash))
            {
                //password is valid
                isLogged = true;
                return entity;
            }

            return null;
        }

        /// <summary>
        /// Adds a lesson to the database
        /// </summary>
        /// <param name="lesson">The lesson to add</param>
        public void AddLesson(Lesson lesson)
        {
            CheckAuthentication();

            lesson.Date = DateTime.Now;
            entityContext.Lessons.Add(lesson);
            entityContext.SaveChanges();
        }

        /// <summary>
        /// Removes a list of lesson from the database
        /// </summary>
        /// <param name="lessons">The lessons to remove</param>
        public void RemoveLessons(List<Lesson> lessons)
        {
            CheckAuthentication();

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

        /// <summary>
        /// Gets all homeworks added to the teacher's lessons
        /// </summary>
        /// <param name="teacherId">The teacher's id</param>
        /// <returns>The homeworks</returns>
        public List<HomeworkView> GetHomeworkViewsByTeacher(int teacherId)
        {
            CheckAuthentication();

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
                    HasMark = homeworksWithMarks.Contains(h.Id),
                    Date = h.Date
                }).ToList();
            

            return entities;
        }

        /// <summary>
        /// Gets all lessons submitted by the teacher
        /// </summary>
        /// <param name="teacherId">The teacher's id</param>
        /// <returns>The lessons</returns>
        public List<LessonView> GetLessonViewsByTeacher(int teacherId)
        {
            CheckAuthentication();

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

        /// <summary>
        /// Gets the subject that the teacher teaches
        /// </summary>
        /// <param name="teacherId">the teacher's id</param>
        /// <returns>The subjects</returns>
        public List<Subject> GetSubjectsByTeacher(int teacherId)
        {
            CheckAuthentication();

            return (from s in entityContext.Subjects
                    where s.TeacherId == teacherId
                    select s).ToList();
        }

        /// <summary>
        /// Get all marks that the teacher has given
        /// </summary>
        /// <param name="teacherId">The teacher's id</param>
        /// <returns>The marks</returns>
        public List<MarkView> GetMarkViewsByTeacher(int teacherId)
        {
            CheckAuthentication();

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

        /// <summary>
        /// Adds a mark to the database
        /// </summary>
        /// <param name="mark">Mark information</param>
        public void AddMark(Mark mark)
        {
            CheckAuthentication();

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

        /// <summary>
        /// Downloads a lesson' content
        /// </summary>
        /// <param name="lessonId">Lesson id</param>
        /// <returns>Lesson's raw content and file name, encapsulated in a File structure</returns>
        public File DownloadLessonContent(int lessonId)
        {
            CheckAuthentication();

            Lesson lesson = (from l in entityContext.Lessons
                             where l.Id == lessonId
                             select l).First();

            return new File(lesson.ContentFilename, lesson.Content);
        }

        /// <summary>
        /// Downloads a lesson's homework
        /// </summary>
        /// <param name="lessonId">Lesson's id</param>
        /// <returns>Lesson's raw homework content and filename, encapsulated in a File structure</returns>
        public File DownloadLessonHomework(int lessonId)
        {
            CheckAuthentication();

            Lesson lesson = (from l in entityContext.Lessons
                             where l.Id == lessonId
                             select l).First();

            return new File(lesson.HomeworkFilename, lesson.HomeworkContent);
        }

        /// <summary>
        /// Downloads a homework, submitted by a student
        /// </summary>
        /// <param name="homeworkId">Homework's id</param>
        /// <returns>Homework's raw content and file name, encapsulated in a File structure</returns>
        public File DownloadSubmittedHomework(int homeworkId)
        {
            CheckAuthentication();

            var entity = (from h in entityContext.Homeworks
                          where h.Id == homeworkId
                          select h).First();

            return new File(entity.Filename, entity.Content);
        }

        /// <summary>
        /// Adds a test with questions
        /// </summary>
        /// <param name="test">The test to add</param>
        public void AddTest(Test test)
        {
            CheckAuthentication();

            int maxScore = 0;
            foreach (var question in test.Questions)
            {
                maxScore += question.Answers.Count(a => a.IsCorrect);
            }

            test.MaxScore = maxScore;
            test.Date = DateTime.Now;
            entityContext.Tests.Add(test);
            entityContext.SaveChanges();
        }

        /// <summary>
        /// Gets all the tests that the teacher has submitted
        /// </summary>
        /// <param name="teacherId">The teacher to search</param>
        /// <returns>All tests by this teacher</returns>
        public List<TestView> GetTestsByTeacher(int teacherId)
        {
            CheckAuthentication();

            var subjects = this.GetSubjectsByTeacher(teacherId);

            var tests = (from s in subjects
                         join t in entityContext.Tests on s.Id equals t.SubjectId
                         select new TestView()
                                    {
                                        Date = t.Date,
                                        Id = t.Id,
                                        Subject = s.Name,
                                        Title = t.Title
                                    }).ToList();

            return tests;
        }

        /// <summary>
        /// Gets a test by specified ID
        /// </summary>
        /// <param name="id">ID of the test</param>
        /// <returns>The test with all its questions</returns>
        public Test GetTest(int id)
        {
            CheckAuthentication();

            var test = entityContext.Tests.Where(t => t.Id == id).FirstOrDefault();

            test.Questions = (from q in entityContext.Questions.Include("Answers")
                              where q.TestId == id
                              select q).ToList();

            return test;
        }

        /// <summary>
        /// Removes a list of tests from the system
        /// </summary>
        /// <param name="tests">The tests to remove</param>
        public void RemoveTests(List<Test> tests)
        {
            int[] ids = tests.Select(t => t.Id).ToArray();

            var entities = (from t in entityContext.Tests
                            where ids.Contains(t.Id)
                            select t).ToList();

            foreach (var entity in entities)
            {
                entityContext.Tests.Remove(entity);
            }

            entityContext.SaveChanges();
        }
    }
}
