using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace VirtualClassroom.Services.Services
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IStudentService" in both code and config file together.
    [ServiceContract]
    public interface IStudentService
    {
        [OperationContract]
        Student LoginStudent(string username, string password);

        [OperationContract]
        List<Lesson> GetLessonsByStudent(int studentId);

        [OperationContract]
        List<Subject> GetSubjectsByStudent(int studentId);

        [OperationContract]
        File DownloadLessonContent(int lessonId);

        [OperationContract]
        File DownloadLessonHomework(int lessonId);

        [OperationContract]
        void AddHomework(Homework homework);

        [OperationContract]
        List<Homework> GetHomeworksByStudent(int studentId);

        [OperationContract]
        List<Mark> GetMarksByStudent(int studentId);
    }
}
