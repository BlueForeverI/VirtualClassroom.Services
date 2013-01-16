using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace VirtualClassroom.Services.Services
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "ITeacherService" in both code and config file together.
    [ServiceContract]
    public interface ITeacherService
    {
        [OperationContract]
        Teacher LoginTeacher(string username, string password);

        [OperationContract]
        void AddLesson(Lesson lesson);

        [OperationContract]
        void RemoveLessons(List<Lesson> lessons);

        [OperationContract]
        List<Homework> GetHomeworksByTeacher(int teacherId);

        [OperationContract]
        List<Lesson> GetLessonsByTeacher(int teacherId);

        [OperationContract]
        List<Subject> GetSubjectsByTeacher(int teacherId);

        [OperationContract]
        void AddMark(Homework homework, float? mark);

        [OperationContract]
        File DownloadLessonContent(int lessonId);

        [OperationContract]
        File DownloadLessonHomework(int lessonId);
    }
}
