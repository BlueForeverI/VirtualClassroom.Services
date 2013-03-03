using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace VirtualClassroom.Services.Models
{
    /// <summary>
    /// A simple File structure to hold information about lesson and homework contents
    /// </summary>
    [DataContract]
    public class File
    {
        [DataMember]
        public string Filename { get; private set; }

        [DataMember]
        public byte[] Content { get; private set; }

        public File(string fileName, byte[] content)
        {
            this.Filename = fileName;
            this.Content = content;
        }
    }
}