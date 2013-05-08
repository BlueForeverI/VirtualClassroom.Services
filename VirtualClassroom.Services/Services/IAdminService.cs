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
        bool AddClass(Class c);

        [OperationContract]
        void AddClasses(List<Class> classes);

        [OperationContract]
        void RemoveClasses(List<Class> classes);

        [OperationContract]
        List<Class> GetClasses();

        [OperationContract]
        void AddClassesToSubject(Subject subject, List<Class> classes);

        #endregion

        #region Student Management

        [OperationContract]
        bool RegisterStudent(Student student, string passwordCrypt, string secret);

        [OperationContract]
        void RegisterStudents(List<Student> students, string secret);

        [OperationContract]
        void RemoveStudents(List<Student> students);

        [OperationContract]
        List<StudentView> GetStudentViews();

        [OperationContract]
        void EditStudent(int studentId, Student student, string secret);

        [OperationContract]
        Student GetStudentById(int studentId);

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
        bool RegisterTeacher(Teacher teacher, string passwordCrypt, string secret);

        [OperationContract]
        void RegisterTeachers(List<Teacher> teachers, string secret);

        [OperationContract]
        void RemoveTeachers(List<Teacher> teachers);

        [OperationContract]
        List<Teacher> GetTeachers();

        [OperationContract]
        void EditTeacher(int teacherId, Teacher teacher, string secret);

        [OperationContract]
        Teacher GetTeacherById(int teacherId);

        #endregion

        #region Admin Management

        [OperationContract]
        bool RegisterAdmin(Admin admin, string passwordCrypt, string secret);

        [OperationContract]
        Admin LoginAdmin(string usernameCrypt, string passwordCrypt, string secret);

        #endregion
    }
}
