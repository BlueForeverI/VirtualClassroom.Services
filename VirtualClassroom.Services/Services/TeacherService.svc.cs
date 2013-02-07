using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace VirtualClassroom.Services.Services
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "TeacherService" in code, svc and config file together.
    public class TeacherService : ITeacherService
    {
        private VirtualClassroomEntities entityContext = new VirtualClassroomEntities();

        public Teacher LoginTeacher(string username, string password)
        {
            if (entityContext.Teachers.Count(s => s.Username == username) == 0)
            {
                return null;
            }

            Teacher entity = entityContext.Teachers.Where(s => s.Username == username).First();
            if (BCrypt.Net.BCrypt.Verify(password, entity.PasswordHash))
            {
                return entity;
            }

            return null;
        }

        public void AddLesson(Lesson lesson)
        {
            //lesson.Date = DateTime.Now;
            entityContext.Lessons.Add(lesson);
            entityContext.SaveChanges();
        }

        public void RemoveLessons(List<Lesson> lessons)
        {
            int[] ids = (from l in lessons select l.Id).ToArray();

            var entities = (from l in entityContext.Lessons
                            where ids.Contains(l.Id)
                            select l).ToList();

            foreach (var entity in entities)
            {
                entityContext.Lessons.Remove(entity);
            }

            entityContext.SaveChanges();
        }

        public List<Homework> GetHomeworksByTeacher(int teacherId, bool unrated = true)
        {
            List<Homework> homeworks = new List<Homework>();
            var homeworksWithMarks = (from m in entityContext.Marks select m.HomeworkId).ToList();

            var entities = new List<Homework>();

            if(unrated == true)
            {
                entities = (from s in entityContext.Subjects.Include("Lessons")
                            from l in s.Lessons
                            from h in l.Homeworks
                            where s.TeacherId == teacherId && !homeworksWithMarks.Contains(h.Id)
                            select h).ToList();
            }
            else
            {
                entities = (from s in entityContext.Subjects.Include("Lessons")
                            from l in s.Lessons
                            from h in l.Homeworks
                            where s.TeacherId == teacherId
                            select h).ToList();
            }

            foreach (var homework in entities)
            {
                homeworks.Add(new Homework()
                {
                    Id = homework.Id,
                    Date = homework.Date,
                    LessonId = homework.LessonId,
                    StudentId = homework.StudentId,
                });
            }

            return homeworks;
        }

        public List<Lesson> GetLessonsByTeacher(int teacherId)
        {
            List<Lesson> lessons = new List<Lesson>();

            var entities = (from s in entityContext.Subjects.Include("Lessons")
                            from l in s.Lessons
                            where s.TeacherId == teacherId
                            select l).ToList();

            foreach (var entity in entities)
            {
                lessons.Add(new Lesson()
                {
                    Id = entity.Id,
                    Date = entity.Date,
                    HomeworkDeadline = entity.HomeworkDeadline,
                    Name = entity.Name,
                    SubjectId = entity.SubjectId
                });
            }

            return lessons;
        }

        public List<Subject> GetSubjectsByTeacher(int teacherId)
        {
            return (from s in entityContext.Subjects
                    where s.TeacherId == teacherId
                    select s).ToList();
        }

        public List<Student> GetStudentsByTeacher(int teacherId)
        {
            var entities = (from s in entityContext.Subjects.Include("Classes")
                            from c in s.Classes
                            from st in c.Students
                            where s.TeacherId == teacherId
                            select st).ToList();

            List<Student> students = new List<Student>();
            foreach (var entity in entities)
            {
                students.Add(new Student()
                                 {
                                     Id = entity.Id,
                                     ClassId = entity.ClassId,
                                     EGN = entity.EGN,
                                     FirstName = entity.FirstName,
                                     MiddleName = entity.MiddleName,
                                     LastName = entity.LastName,
                                     Username = entity.Username
                                 });
            }

            return students;
        }

        public void AddMark(Mark mark)
        {
            mark.Date = DateTime.Now;
            mark.SubjectName = (from sub in entityContext.Subjects.Include("Lessons")
                                from l in sub.Lessons
                                from h in l.Homeworks
                                where h.Id == mark.HomeworkId
                                select sub.Name).First();

            mark.LessonName = (from l in entityContext.Lessons.Include("Homworks")
                               where l.Homeworks.Any(h => h.Id == mark.HomeworkId)
                               select l.Name).First();

            entityContext.Marks.Add(mark);
            entityContext.SaveChanges();
        }

        public File DownloadLessonContent(int lessonId)
        {
            Lesson lesson = (from l in entityContext.Lessons
                             where l.Id == lessonId
                             select l).First();

            return new File(lesson.ContentFilename, lesson.Content);
        }

        public File DownloadLessonHomework(int lessonId)
        {
            Lesson lesson = (from l in entityContext.Lessons
                             where l.Id == lessonId
                             select l).First();

            return new File(lesson.HomeworkFilename, lesson.HomeworkContent);
        }

        public File DownloadSubmittedHomework(int homeworkId)
        {
            var entity = (from h in entityContext.Homeworks
                          where h.Id == homeworkId
                          select h).First();

            return new File(entity.Filename, entity.Content);
        }
    }
}
