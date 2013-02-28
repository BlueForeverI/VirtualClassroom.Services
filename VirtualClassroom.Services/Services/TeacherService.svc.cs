﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using VirtualClassroom.Services.Models;
using VirtualClassroom.Services.Views;

namespace VirtualClassroom.Services.Services
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession, ConcurrencyMode = ConcurrencyMode.Single)]
    public class TeacherService : ITeacherService
    {
        private VirtualClassroomEntities entityContext = new VirtualClassroomEntities();
        private bool isLogged = false;

        private void CheckAuthentication()
        {
            if (isLogged == false)
            {
                throw new FaultException("Not logged in!");
            }
        }

        public Teacher LoginTeacher(string usernameCrypt, string passwordCrypt, string secret)
        {
            if (string.IsNullOrWhiteSpace(usernameCrypt) || string.IsNullOrEmpty(usernameCrypt)
                || string.IsNullOrWhiteSpace(passwordCrypt) || string.IsNullOrEmpty(passwordCrypt)
                || string.IsNullOrWhiteSpace(secret) || string.IsNullOrEmpty(secret))
            {
                return null;
            }

            string username = Crypto.DecryptStringAES(usernameCrypt, secret);
            string password = Crypto.DecryptStringAES(passwordCrypt, secret);

            if (entityContext.Teachers.Count(s => s.Username == username) == 0)
            {
                return null;
            }

            Teacher entity = entityContext.Teachers.Where(s => s.Username == username).First();
            if (BCrypt.Net.BCrypt.Verify(password, entity.PasswordHash))
            {
                isLogged = true;
                return entity;
            }

            return null;
        }

        public void AddLesson(Lesson lesson)
        {
            CheckAuthentication();

            lesson.Date = DateTime.Now;
            entityContext.Lessons.Add(lesson);
            entityContext.SaveChanges();
        }

        public void RemoveLessons(List<Lesson> lessons)
        {
            CheckAuthentication();

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

        public List<HomeworkView> GetHomeworkViewsByTeacher(int teacherId)
        {
            CheckAuthentication();

            var homeworksWithMarks = (from m in entityContext.Marks select m.HomeworkId).ToList();
            var entities =  (
                from s in entityContext.Subjects
                join l in entityContext.Lessons
                    on s.Id equals l.SubjectId
                join h in entityContext.Homeworks
                    on l.Id equals h.LessonId
                join st in entityContext.Students
                    on h.StudentId equals st.Id
                where s.TeacherId == teacherId
                select new HomeworkView()
                {
                    Id = h.Id,
                    Lesson = l.Name,
                    StudentFullName = st.FirstName + " " + st.MiddleName + " " + st.LastName,
                    Subject = s.Name,
                    HasMark = homeworksWithMarks.Contains(h.Id)
                }).ToList();
            

            return entities;
        }

        public List<LessonView> GetLessonViewsByTeacher(int teacherId)
        {
            CheckAuthentication();

            return (
                       from s in entityContext.Subjects
                       join l in entityContext.Lessons
                           on s.Id equals l.SubjectId
                       where s.TeacherId == teacherId
                       select new LessonView()
                                  {
                                      Id = l.Id,
                                      Date = l.Date,
                                      HomeworkDeadline = l.HomeworkDeadline,
                                      Name = l.Name,
                                      Subject = s.Name
                                  }).ToList();
        }

        public List<Subject> GetSubjectsByTeacher(int teacherId)
        {
            CheckAuthentication();

            return (from s in entityContext.Subjects
                    where s.TeacherId == teacherId
                    select s).ToList();
        }

        public List<MarkView> GetMarkViewsByTeacher(int teacherId)
        {
            CheckAuthentication();

            var markViews = (from m in entityContext.Marks
                             join h in entityContext.Homeworks on m.HomeworkId equals h.Id
                             join st in entityContext.Students on h.StudentId equals st.Id
                             join l in entityContext.Lessons on h.LessonId equals l.Id
                             join sub in entityContext.Subjects on l.SubjectId equals sub.Id
                             join c in entityContext.Classes on st.ClassId equals c.Id
                             join t in entityContext.Teachers on sub.TeacherId equals teacherId
                             select  new 
                             {
                                Id = m.Id,
                                Student = st.FirstName + " " + st.MiddleName + " " + st.LastName,
                                ClassNumber = c.Number,
                                ClassLetter = c.Letter,
                                Subject = sub.Name,
                                Lesson = l.Name,
                                Date = m.Date,
                                Value = m.Value
                             })
                .AsEnumerable().Distinct()
                .Select(m => new MarkView()
                {
                    Class = string.Format("{0} '{1}'", m.ClassNumber, m.ClassLetter),
                    Id = m.Id,
                    Date = m.Date,
                    Lesson = m.Lesson,
                    Student = m.Student,
                    Subject = m.Subject,
                    Value = m.Value
                }).ToList();

            return markViews;
        }

        public void AddMark(Mark mark)
        {
            CheckAuthentication();

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
            CheckAuthentication();

            Lesson lesson = (from l in entityContext.Lessons
                             where l.Id == lessonId
                             select l).First();

            return new File(lesson.ContentFilename, lesson.Content);
        }

        public File DownloadLessonHomework(int lessonId)
        {
            CheckAuthentication();

            Lesson lesson = (from l in entityContext.Lessons
                             where l.Id == lessonId
                             select l).First();

            return new File(lesson.HomeworkFilename, lesson.HomeworkContent);
        }

        public File DownloadSubmittedHomework(int homeworkId)
        {
            CheckAuthentication();

            var entity = (from h in entityContext.Homeworks
                          where h.Id == homeworkId
                          select h).First();

            return new File(entity.Filename, entity.Content);
        }
    }
}
