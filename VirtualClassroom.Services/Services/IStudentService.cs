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
    /// <summary>
    /// Interface contract for the student service
    /// </summary>
    
    //using reliable sessions
    [ServiceContract(SessionMode = SessionMode.Required)]
    public interface IStudentService
    {
        [OperationContract]
        Student LoginStudent(string usernameCrypt, string passwordCrypt, string secret);

        [OperationContract]
        List<LessonView> GetLessonViewsByStudent(int studentId);

        [OperationContract]
        File DownloadLessonContent(int lessonId);

        [OperationContract]
        File DownloadSentHomework(int studentId, int lessonId);

        [OperationContract]
        File DownloadLessonHomework(int lessonId);

        [OperationContract]
        bool AddHomework(Homework homework);

        [OperationContract]
        List<Homework> GetHomeworksByStudent(int studentId);

        [OperationContract]
        List<Mark> GetMarksByStudent(int studentId);

        [OperationContract]
        int EvaluateTest(Test test, int studentId);

        [OperationContract]
        List<TestView> GetTestViewsByStudent(int studentId);

        [OperationContract]
        Test GetTest(int testId);
    }
}
