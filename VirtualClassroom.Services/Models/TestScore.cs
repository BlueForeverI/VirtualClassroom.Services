using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VirtualClassroom.Services.Models
{
    /// <summary>
    /// Represents a score tha a student has made after
    /// solving a test
    /// </summary>
    public partial class TestScore
    {
        public int Id { get; set; }
        public int TestId { get; set; }
        public int StudentId { get; set; }
        public int Score { get; set; }
        public int MaxScore { get; set; }
    }
}