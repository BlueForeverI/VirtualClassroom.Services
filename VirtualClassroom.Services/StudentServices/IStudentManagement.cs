using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using VirtualClassroom.Services.POCO_Classes;

namespace VirtualClassroom.Services.StudentServices
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IStudentManagement" in both code and config file together.
    [ServiceContract]
    public interface IStudentManagement
    {
        [OperationContract]
        void RegisterStudent(Student student, string password);
    }
}
