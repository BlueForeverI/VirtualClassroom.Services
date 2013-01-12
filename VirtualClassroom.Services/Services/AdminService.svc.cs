using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace VirtualClassroom.Services.Services
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "AdminService" in code, svc and config file together.
    public class AdminService : IAdminService
    {
        VirtualClassroomEntities entityContext = new VirtualClassroomEntities();

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

        public List<Student> GetStudents()
        {
            return entityContext.Students.ToList();
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
            Subject subjectEntity = new Subject() { Id = subject.Id };
            //entitityContext.AttachTo("Subjects", subjectEntity);
            entityContext.Subjects.Attach(subjectEntity);
            foreach (var c in classes)
            {
                Class entity = new Class() { Id = c.Id };
                //entitityContext.AttachTo("Classes", entity);
                entityContext.Classes.Attach(entity);
                subjectEntity.Classes.Add(entity);
            }

            entityContext.SaveChanges();
        }

        public void AddSubjectsToClass(Class c, List<Subject> subjects)
        {
            Class classEntity = new Class(){Id = c.Id};
            entityContext.Classes.Attach(classEntity);

            foreach (var subject in subjects)
            {
                Subject entity = new Subject(){Id = subject.Id};
                entityContext.Subjects.Attach(entity);
                classEntity.Subjects.Add(entity);
            }

            entityContext.SaveChanges();
        }

        public List<Subject> GetSubjects()
        {
            return entityContext.Subjects.ToList();
        }

        //to refactor
        private static bool IsTeacherValid(Teacher teacher)
        {
            return true;
        }

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
    }
}
