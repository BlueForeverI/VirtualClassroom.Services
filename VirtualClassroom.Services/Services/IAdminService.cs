using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Collections.ObjectModel;
using VirtualClassroom.Services.Models;
using VirtualClassroom.Services.Views;

namespace VirtualClassroom.Services.Services
{
    /// <summary>
    /// Interface contract for the admin service
    /// </summary>
    
    //using reliable sessions
    [ServiceContract(SessionMode = SessionMode.Required)]
    public interface IAdminService
    {
        #region Class Management

        [OperationContract]
        void AddClass(Class c);

        [OperationContract]
        void RemoveClasses(List<Class> classes);

        [OperationContract]
        List<Class> GetClasses();

        [OperationContract]
        void AddClassesToSubject(Subject subject, List<Class> classes);

        #endregion

        #region Student Management

        [OperationContract]
        void RegisterStudent(Student student, string passwordCrypt, string secret);

        [OperationContract]
        void RemoveStudents(List<Student> students);

        [OperationContract]
        List<StudentView> GetStudentViews();

        #endregion

        #region Subject Management
        
        [OperationContract]
        void AddSubject(Subject subject);

        [OperationContract]
        void RemoveSubjects(List<Subject> subjects); 

        [OperationContract]
        List<SubjectView> GetSubjectViews();

        [OperationContract]
        void AddSubjectsToClass(Class c, List<Subject> subjects);

        [OperationContract]
        List<Subject> GetSubjectsByClass(int classId);

        #endregion

        #region Teacher Management
        
        [OperationContract]
        void RegisterTeacher(Teacher teacher, string passwordCrypt, string secret);

        [OperationContract]
        void RemoveTeachers(List<Teacher> teachers);

        [OperationContract]
        List<Teacher> GetTeachers();

        #endregion

        #region Admin Management

        [OperationContract]
        void RegisterAdmin(Admin admin, string passwordCrypt, string secret);

        [OperationContract]
        Admin LoginAdmin(string usernameCrypt, string passwordCrypt, string secret);

        #endregion
    }
}
