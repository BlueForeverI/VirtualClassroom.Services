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
        VirtualClassroomEntities entitityContext = new VirtualClassroomEntities();

        public void AddClass(Class c)
        {
            entitityContext.Classes.Add(c);
            entitityContext.SaveChanges();
        }

        public void RemoveClass(Class c)
        {
            Class classEntity = (from cl in entitityContext.Classes
                                 where cl.Id == c.Id
                                 select cl).First();

            entitityContext.Classes.Remove(classEntity);
            entitityContext.SaveChanges();
        }

        public void RegisterStudent(Student student, string password)
        {
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
            student.PasswordHash = passwordHash;

            if (IsStudentValid(student))
            {
                entitityContext.Students.Add(student);
                entitityContext.SaveChanges();
            }
        }

        public void RemoveStudent(Student student)
        {
            Student studentEntity = (from s in entitityContext.Students
                                     where s.Id == student.Id
                                     select s).First();

            entitityContext.Students.Remove(studentEntity);
            entitityContext.SaveChanges();
        }

        //to refactor
        private static bool IsStudentValid(Student student)
        {
            return true;
        }

        public List<Class> GetClasses()
        {
            return entitityContext.Classes.ToList();
        }

        public void AddSubject(Subject subject)
        {
            entitityContext.Subjects.Add(subject);
            entitityContext.SaveChanges();
        }

        public void RemoveSubject(Subject subject)
        {
            Subject subjectEntity = (from s in entitityContext.Subjects
                                     where s.Id == subject.Id
                                     select s).First();

            entitityContext.Subjects.Remove(subjectEntity);
            entitityContext.SaveChanges();
        }

        public void RegisterTeacher(Teacher teacher, string password)
        {
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
            teacher.PasswordHash = passwordHash;

            if (IsTeacherValid(teacher))
            {
                entitityContext.Teachers.Add(teacher);
                entitityContext.SaveChanges();
            }
        }

        public void RemoveTeacher(Teacher teacher)
        {
            Teacher teacherEntity = (from t in entitityContext.Teachers
                                     where t.Id == teacher.Id
                                     select t).First();

            entitityContext.Teachers.Remove(teacherEntity);
            entitityContext.SaveChanges();
        }

        public List<Teacher> GetTeachers()
        {
            return entitityContext.Teachers.ToList();
        }

        public void AddClassesToSubject(Subject subject, List<Class> classes)
        {
            Subject subjectEntity = new Subject() { Id = subject.Id };
            //entitityContext.AttachTo("Subjects", subjectEntity);
            entitityContext.Subjects.Attach(subjectEntity);
            foreach (var c in classes)
            {
                Class entity = new Class() { Id = c.Id };
                //entitityContext.AttachTo("Classes", entity);
                entitityContext.Classes.Attach(entity);
                subjectEntity.Classes.Add(entity);
            }

            entitityContext.SaveChanges();
        }

        public List<Subject> GetSubjects()
        {
            return entitityContext.Subjects.ToList();
        }

        //to refactor
        private static bool IsTeacherValid(Teacher teacher)
        {
            return true;
        }

        public Student LoginStudent(string username, string password)
        {
            if (entitityContext.Students.Count(s => s.Username == username) == 0)
            {
                return null;
            }

            Student entity = entitityContext.Students.Where(s => s.Username == username).First();
            if (BCrypt.Net.BCrypt.Verify(password, entity.PasswordHash))
            {
                return entity;
            }

            return null;
        }

        public Teacher LoginTeacher(string username, string password)
        {
            if (entitityContext.Teachers.Count(s => s.Username == username) == 0)
            {
                return null;
            }

            Teacher entity = entitityContext.Teachers.Where(s => s.Username == username).First();
            if (BCrypt.Net.BCrypt.Verify(password, entity.PasswordHash))
            {
                return entity;
            }

            return null;
        }


        public List<Subject> GetSubjectsByClass(int classId)
        {
            Class c = (from cl in entitityContext.Classes.Include("Subjects") 
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
