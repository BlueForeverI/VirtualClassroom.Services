using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
//using VirtualClassroom.Services.POCO_Classes;
using System.ServiceModel.Web;
using System.Text;
using System.Collections.ObjectModel;
using VirtualClassroom.Services.Models;
using VirtualClassroom.Services.Views;

namespace VirtualClassroom.Services.Services
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IAdminService" in both code and config file together.
    [ServiceContract]
    public interface IAdminService
    {
        [OperationContract]
        void AddClass(Class c);

        [OperationContract]
        void RemoveClasses(List<Class> classes);

        [OperationContract]
        List<Class> GetClasses();

        [OperationContract]
        void RegisterStudent(Student student, string password);

        [OperationContract]
        void RemoveStudents(List<Student> students);

        [OperationContract]
        List<StudentView> GetStudentViews();

        [OperationContract]
        void AddSubject(Subject subject);

        [OperationContract]
        void RemoveSubjects(List<Subject> subjects); 

        [OperationContract]
        List<SubjectView> GetSubjectViews();

        [OperationContract]
        void RegisterTeacher(Teacher teacher, string password);

        [OperationContract]
        void RemoveTeachers(List<Teacher> teachers);

        [OperationContract]
        List<Teacher> GetTeachers();

        [OperationContract]
        void AddClassesToSubject(Subject subject, List<Class> classes);

        [OperationContract]
        void AddSubjectsToClass(Class c, List<Subject> subjects);

        [OperationContract]
        List<Subject> GetSubjectsByClass(int classId);

        [OperationContract]
        void RegisterAdmin(Admin admin, string password);

        [OperationContract]
        Admin LoginAdmin(string username, string password);
    }
}
