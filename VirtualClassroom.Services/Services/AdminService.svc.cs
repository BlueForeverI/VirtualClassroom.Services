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
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "AdminService" in code, svc and config file together.
    public class AdminService : IAdminService
    {
        private VirtualClassroomEntities entityContext = new VirtualClassroomEntities();

        public void AddClass(Class c)
        {
            entityContext.Classes.Add(c);
            entityContext.SaveChanges();
        }

        public void RemoveClasses(List<Class> classes)
        {
            int[] ids = (from c in classes select c.Id).ToArray();

            var entities = (from c in entityContext.Classes
                            where ids.Contains(c.Id)
                            select c).ToList();

            foreach (var entity in entities)
            {
                entityContext.Classes.Remove(entity);
            }

            entityContext.SaveChanges();
        }

        public void RegisterStudent(Student student, string password)
        {
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
            student.PasswordHash = passwordHash;

            if (IsStudentValid(student))
            {
                entityContext.Students.Add(student);
                entityContext.SaveChanges();
            }
        }

        public void RemoveStudents(List<Student> students)
        {
            int[] ids = (from s in students select s.Id).ToArray();

            var entities = (from s in entityContext.Students
                            where ids.Contains(s.Id)
                            select s).ToList();

            foreach (var entity in entities)
            {
                entityContext.Students.Remove(entity);
            }

            entityContext.SaveChanges();
        }

        public List<StudentView> GetStudentViews()
        {
            return (
                       from s in entityContext.Students
                       join c in entityContext.Classes
                           on s.ClassId equals c.Id
                       select new 
                                  {
                                      Id = s.Id,
                                      ClassNumber = c.Number,
                                      ClassLetter = c.Letter,
                                      EGN = s.EGN,
                                      FullName = s.FirstName + " " + s.MiddleName + " " + s.LastName,
                                      Username = s.Username
                                  })
                                  .AsEnumerable()
                                  .Select(x => new StudentView()
                                                   {
                                                       Class = string.Format("{0} '{1}'", x.ClassNumber, x.ClassLetter),
                                                       EGN = x.EGN,
                                                       FullName = x.FullName,
                                                       Id = x.Id,
                                                       Username = x.Username
                                                   }).ToList();
        }

        //to refactor
        private static bool IsStudentValid(Student student)
        {
            return true;
        }

        public List<Class> GetClasses()
        {
            return entityContext.Classes.ToList();
        }

        public void AddSubject(Subject subject)
        {
            entityContext.Subjects.Add(subject);
            entityContext.SaveChanges();
        }

        public void RemoveSubjects(List<Subject> subjects)
        {
            int[] ids = (from s in subjects select s.Id).ToArray();

            var entities = (from s in entityContext.Subjects
                            where ids.Contains(s.Id)
                            select s).ToList();

            foreach (var entity in entities)
            {
                entityContext.Subjects.Remove(entity);
            }

            entityContext.SaveChanges();
        }

        public List<SubjectView> GetSubjectViews()
        {
            return (
                       from s in entityContext.Subjects
                       join t in entityContext.Teachers
                           on s.TeacherId equals t.Id
                       select new SubjectView()
                                  {
                                      Id = s.Id,
                                      Name = s.Name,
                                      TeacherFullName = t.FirstName + " " + t.MiddleName + " " + t.LastName
                                  }).ToList();
        }

        public void RegisterTeacher(Teacher teacher, string password)
        {
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
            teacher.PasswordHash = passwordHash;

            if (IsTeacherValid(teacher))
            {
                entityContext.Teachers.Add(teacher);
                entityContext.SaveChanges();
            }
        }

        public void RemoveTeachers(List<Teacher> teachers)
        {
            int[] ids = (from t in teachers select t.Id).ToArray();

            var entities = (from t in entityContext.Teachers
                            where ids.Contains(t.Id)
                            select t).ToList();

            foreach (var entity in entities)
            {
                entityContext.Teachers.Remove(entity);
            }

            entityContext.SaveChanges();
        }

        public List<Teacher> GetTeachers()
        {
            return entityContext.Teachers.ToList();
        }

        public void AddClassesToSubject(Subject subject, List<Class> classes)
        {
            var subjectEntity = entityContext.Subjects.Include("Classes")
                .Where(s => s.Id == subject.Id).FirstOrDefault();

            foreach (var c in classes)
            {
                Class entity = new Class() { Id = c.Id };

                if (!subjectEntity.Classes.Any(cl => cl.Id == entity.Id))
                {
                    entityContext.Classes.Attach(entity);
                    subjectEntity.Classes.Add(entity);
                }
            }

            entityContext.SaveChanges();
        }

        public void AddSubjectsToClass(Class c, List<Subject> subjects)
        {
            var classEntity = entityContext.Classes.Include("Subjects")
                .Where(cl => cl.Id == c.Id).FirstOrDefault();

            foreach (var subject in subjects)
            {
                Subject entity = new Subject(){Id = subject.Id};

                if (!classEntity.Subjects.Any(s => s.Id == entity.Id))
                {
                    entityContext.Subjects.Attach(entity);
                    classEntity.Subjects.Add(entity);
                }
            }

            entityContext.SaveChanges();
        }

        //to refactor
        private static bool IsTeacherValid(Teacher teacher)
        {
            return true;
        }

        public List<Subject> GetSubjectsByClass(int classId)
        {
            Class c = (from cl in entityContext.Classes.Include("Subjects") 
                       where cl.Id == classId 
                       select cl).First();

            //avoid circular reference and infinite loop
            foreach (var subject in c.Subjects)
            {
                subject.Classes = null;
            }

            return c.Subjects.ToList();
        }

        public void RegisterAdmin(Admin admin, string password)
        {
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
            admin.PasswordHash = passwordHash;

            if(IsAdminValid(admin))
            {
                entityContext.Admins.Add(admin);
                entityContext.SaveChanges();
            }
        }

        public Admin LoginAdmin(string username, string password)
        {
            if(entityContext.Admins.Count(a => a.Username == username) == 0)
            {
                return null;
            }

            Admin entity = entityContext.Admins.Where(a => a.Username == username).First();
            if (BCrypt.Net.BCrypt.Verify(password, entity.PasswordHash))
            {
                return entity;
            }

            return null;
        }

        //to refactor
        private bool IsAdminValid(Admin admin)
        {
            return true;
        }
    }
}
