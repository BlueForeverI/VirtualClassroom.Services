using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using VirtualClassroom.Services.Converters;
using VirtualClassroom.Services.POCO_Classes;
using VirtualClassroom.Services.Resolvers;
using AutoMapper;

namespace VirtualClassroom.Services.Services
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "AdminService" in code, svc and config file together.
    public class AdminService : IAdminService
    {
        VirtualClassroomEntities entitityContext = new VirtualClassroomEntities();

        static AdminService()
        {
            Mapper.CreateMap<ClassEntity, Class>();
            Mapper.CreateMap<StudentEntity, Student>();
            Mapper.CreateMap<MarkEntity, Mark>();
            Mapper.CreateMap<HomeworkEntity, Homework>();
            Mapper.CreateMap<SubjectEntity, Subject>().ForMember(x => x.Classes, y => y.Condition(c => c.Classes.Count > 0))
                .ForMember(x => x.Classes, y => y.Ignore());
            Mapper.CreateMap<LessonEntity, Lesson>();
            Mapper.CreateMap<TeacherEntity, Teacher>();

            Mapper.CreateMap<Class, ClassEntity>().ForMember(x => x.EntityKey, y => y.Ignore());
            Mapper.CreateMap<Student, StudentEntity>().ForMember(x => x.EntityKey, y => y.Ignore())
                .ForMember(x => x.Class, y => y.Ignore()).ForMember(x => x.ClassReference, y => y.Ignore());

            Mapper.CreateMap<Mark, MarkEntity>().ForMember(x => x.EntityKey, y => y.Ignore())
                .ForMember(x => x.Homework, y => y.Ignore()).ForMember(x => x.HomeworkReference, y => y.Ignore());

            Mapper.CreateMap<Homework, HomeworkEntity>().ForMember(x => x.EntityKey, y => y.Ignore())
                .ForMember(x => x.Lesson, y => y.Ignore()).ForMember(x => x.LessonReference, y => y.Ignore())
                .ForMember(x => x.Student, y => y.Ignore()).ForMember(x => x.StudentReference, y => y.Ignore());

            Mapper.CreateMap<Subject, SubjectEntity>().ForMember(x => x.EntityKey, y => y.Ignore())
                .ForMember(x => x.Teacher, y => y.Ignore()).ForMember(x => x.TeacherReference, y => y.Ignore());

            Mapper.CreateMap<Lesson, LessonEntity>().ForMember(x => x.EntityKey, y => y.Ignore())
                .ForMember(x => x.Subject, y => y.Ignore()).ForMember(x => x.SubjectReference, y => y.Ignore());

            Mapper.CreateMap<Teacher, TeacherEntity>().ForMember(x => x.EntityKey, y => y.Ignore());

            Mapper.AssertConfigurationIsValid();
        }
        

        public void AddClass(Class c)
        {
            ClassEntity classEntity = Mapper.Map<Class, ClassEntity>(c);
            entitityContext.ClassEntities.AddObject(classEntity);
            entitityContext.SaveChanges();
        }

        public void RegisterStudent(Student student, string password)
        {
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
            student.PasswordHash = passwordHash;

            if (IsStudentValid(student))
            {
                StudentEntity studentEntity = Mapper.Map<Student, StudentEntity>(student);
                entitityContext.StudentEntities.AddObject(studentEntity);
                entitityContext.SaveChanges();
            }
        }

        //to refactor
        private static bool IsStudentValid(Student student)
        {
            return true;
        }

        public List<Class> GetClasses()
        {
            List<Class> classes = new List<Class>();

            List<ClassEntity> classEntities = entitityContext.ClassEntities.ToList();
            classes = Mapper.Map<List<ClassEntity>, List<Class>>(classEntities).ToList();

            return classes;
        }

        public void AddSubject(Subject subject)
        {
            SubjectEntity subjectEntity = new SubjectEntity();//(SubjectEntity) subject;
            subjectEntity.Classes.Clear();

            entitityContext.SubjectEntities.AddObject(subjectEntity);
            entitityContext.SaveChanges();
            subjectEntity.Classes.Add(new ClassEntity(){Letter = "A", Number = 11});
            //AddClassesToSubject(subject, subject.Classes);
            //subjectEntity.Classes.Attach(entitityContext.ClassEntities);
            entitityContext.SaveChanges();
        }

        public void RegisterTeacher(Teacher teacher, string password)
        {
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
            teacher.PasswordHash = passwordHash;

            if(IsTeacherValid(teacher))
            {
                TeacherEntity teacherEntity = Mapper.Map<Teacher, TeacherEntity>(teacher);
                entitityContext.TeacherEntities.AddObject(teacherEntity);
                entitityContext.SaveChanges();
            }
        }

        public List<Teacher> GetTeachers()
        {
            List<TeacherEntity> teacherEntities = entitityContext.TeacherEntities.ToList();
            List<Teacher> teachers = Mapper.Map<List<TeacherEntity>, List<Teacher>>(teacherEntities).ToList();

            return teachers;
        }

        public void AddClassesToSubject(Subject subject, List<Class> classes)
        {
            SubjectEntity subjectEntity = new SubjectEntity();//(SubjectEntity) subject;
            foreach (var c in classes)
            {
                //subjectEntity.Classes.Add((ClassEntity)c);
            }

            entitityContext.SaveChanges();
        }

        public List<Subject> GetSubjects()
        {
            List<SubjectEntity> entities = entitityContext.SubjectEntities.ToList();
            List<Subject> subjects = Mapper.Map<List<SubjectEntity>, List<Subject>>(entities);

            return subjects;
        }

        //to refactor
        private static bool IsTeacherValid(Teacher teacher)
        {
            return true;
        }
    }
}
