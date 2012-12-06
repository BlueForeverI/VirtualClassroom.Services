using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using VirtualClassroom.Services.POCO_Classes;
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
        void RegisterStudent(Student student, string password);

        [OperationContract]
        List<Class> GetClasses();

        [OperationContract]
        void AddSubject(Subject subject);

        [OperationContract]
        void RegisterTeacher(Teacher teacher, string password);

        [OperationContract]
        List<Teacher> GetTeachers();

        [OperationContract]
        void AddClassesToSubject(Subject subject, List<Class> classes);

        [OperationContract]
        List<Subject> GetSubjects();
    }
}
