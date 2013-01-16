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
                    Content = entity.Content,
                    ContentFilename = entity.ContentFilename,
                    Date = entity.Date,
                    HomeworkContent = entity.HomeworkContent,
                    HomeworkDeadline = entity.HomeworkDeadline,
                    HomeworkFilename = entity.HomeworkFilename,
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
    }
}
