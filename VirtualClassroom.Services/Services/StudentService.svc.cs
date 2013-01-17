using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

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

        public List<Lesson> GetLessonsByStudent(int studentId)
        {
            int classId = (from c in entityContext.Classes.Include("Students")
                           where c.Students.Any(s => s.Id == studentId)
                           select c.Id).First();

            var entities = (from c in entityContext.Classes.Include("Subjects")
                            from s in c.Subjects
                            from l in s.Lessons
                            where c.Id == classId
                            select l).ToList();

            List<Lesson> lessons = new List<Lesson>();
            foreach (var entity in entities)
            {
                lessons.Add(new Lesson()
                {
                    Id = entity.Id,
                    Date = entity.Date,
                    HomeworkDeadline = entity.HomeworkDeadline,
                    Name = entity.Name,
                    SubjectId = entity.SubjectId
                });
            }

            return lessons;
        }

        public List<Subject> GetSubjectsByStudent(int studentId)
        {
            int classId = (from c in entityContext.Classes.Include("Students")
                           where c.Students.Any(s => s.Id == studentId)
                           select c.Id).First();

            var entities = (from c in entityContext.Classes.Include("Subjects")
                            from s in c.Subjects
                            where c.Id == classId
                            select s).ToList();

            List<Subject> subjects = new List<Subject>();
            foreach (var entity in entities)
            {
                subjects.Add(new Subject()
                {
                    Id = entity.Id, 
                    Name = entity.Name, 
                    TeacherId = entity.TeacherId
                });
            }

            return subjects;
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

            var entities = (from c in entityContext.Classes.Include("Subjects")
                            from s in c.Subjects
                            from l in s.Lessons
                            from h in l.Homeworks
                            where c.Id == classId
                            select h).ToList();

            List<Homework> homeworks = new List<Homework>();
            foreach (var entity in entities)
            {
                homeworks.Add(new Homework()
                                  {
                                      Id = entity.Id,
                                      Date = entity.Date,
                                      LessonId = entity.LessonId,
                                      Mark = entity.Mark,
                                  });
            }

            return homeworks;
        }
    }
}
