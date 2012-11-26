using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using VirtualClassroom.Services.POCO_Classes;
using BCrypt.Net;

namespace VirtualClassroom.Services.StudentServices
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "StudentManagement" in code, svc and config file together.
    public class StudentManagement : IStudentManagement
    {
        VirtualClassroomEntities entities = new VirtualClassroomEntities();

        public void RegisterStudent(Student student, string password)
        {
            if(IsValidStudent(student))
            {
                StudentEntity studentEntity = Student.ToStudentEntity(student);
                studentEntity.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
                entities.StudentEntities.AddObject(studentEntity);
                entities.SaveChanges();

            }
        }

        //to implement
        private bool IsValidStudent(Student student)
        {
            return true;
        }
    }
}
