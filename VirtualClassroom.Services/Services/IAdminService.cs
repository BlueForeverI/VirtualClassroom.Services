using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
//using VirtualClassroom.Services.POCO_Classes;
using System.ServiceModel.Web;
using System.Text;
using System.Collections.ObjectModel;

namespace VirtualClassroom.Services.Services
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IAdminService" in both code and config file together.
    [ServiceContract]
    public interface IAdminService
    {
        [OperationContract]
        void AddClass(Class c);

        [OperationContract]
        void RemoveClass(Class c);

        [OperationContract]
        List<Class> GetClasses();

        [OperationContract]
        void RegisterStudent(Student student, string password);

        [OperationContract]
        void RemoveStudent(Student student);

        [OperationContract]
        List<Student> GetStudents();

        [OperationContract]
        void AddSubject(Subject subject);

        [OperationContract]
        void RemoveSubject(Subject subject);

        [OperationContract]
        List<Subject> GetSubjects();

        [OperationContract]
        void RegisterTeacher(Teacher teacher, string password);

        [OperationContract]
        void RemoveTeacher(Teacher teacher);

        [OperationContract]
        List<Teacher> GetTeachers();

        [OperationContract]
        void AddClassesToSubject(Subject subject, List<Class> classes);

        [OperationContract]
        List<Subject> GetSubjectsByClass(int classId);
        
        [OperationContract]
        Student LoginStudent(string username, string password);

        [OperationContract]
        Teacher LoginTeacher(string username, string password);
    }
}
