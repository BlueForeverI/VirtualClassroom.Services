using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using VirtualClassroom.Services.POCO_Classes;

namespace VirtualClassroom.Services.Services
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "AdminService" in code, svc and config file together.
    public class AdminService : IAdminService
    {
        VirtualClassroomEntities entitityContext = new VirtualClassroomEntities();

        public void AddClass(Class c)
        {
            ClassEntity classEntity = Class.ToClassEntity(c);
            entitityContext.ClassEntities.AddObject(classEntity);
            entitityContext.SaveChanges();
        }

        public void RegisterStudent(Student student)
        {
            StudentEntity studentEntity = Student.ToStudentEntity(student);
            entitityContext.StudentEntities.AddObject(studentEntity);
            entitityContext.SaveChanges();
        }

        public List<Class> GetClasses()
        {
            List<Class> classes = new List<Class>();
            foreach (var entity in entitityContext.ClassEntities.ToList())
            {
                classes.Add((Class)entity);
            }

            return classes;
        }
    }
}
