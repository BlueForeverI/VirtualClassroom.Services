using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using VirtualClassroom.Services.Models;
using VirtualClassroom.Services.Views;

namespace VirtualClassroom.Services.Services
{
    /// <summary>
    /// Implementation of the IAdminService
    /// </summary>
    
    //enable sessions
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession, ConcurrencyMode = ConcurrencyMode.Single)]
    public class AdminService : IAdminService
    {
        private VirtualClassroomEntities entityContext = new VirtualClassroomEntities();
        private bool isLogged = false;          //stores login state

        /// <summary>
        /// Checks if the client is authenticated
        /// </summary>
        private void CheckAuthentication()
        {
            if(isLogged == false)
            {
                throw new FaultException("Не сте влезли в системата");
            }
        }

        #region Class Management

        /// <summary>
        /// Adds a class to the database
        /// </summary>
        /// <param name="c">The class to add</param>
        public bool AddClass(Class c)
        {
            CheckAuthentication();

            if (!entityContext.Classes.Any(cl => cl.Number == c.Number && cl.Letter == c.Letter))
            {
                entityContext.Classes.Add(c);
                entityContext.SaveChanges();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Add a list of classes (used for importing)
        /// </summary>
        /// <param name="classes">Classes to add</param>
        public void AddClasses(List<Class> classes)
        {
            CheckAuthentication();

            foreach (var c in classes)
            {
                if (!entityContext.Classes.Any(cl => cl.Number == c.Number && cl.Letter == c.Letter))
                {
                    entityContext.Classes.Add(c);
                } 
            }

            entityContext.SaveChanges();
        }

        /// <summary>
        /// Removes a list of classes from the database
        /// </summary>
        /// <param name="classes"></param>
        public void RemoveClasses(List<Class> classes)
        {
            CheckAuthentication();

            int[] ids = (from c in classes select c.Id).ToArray();

            var entities = (from c in entityContext.Classes
                            where ids.Contains(c.Id)
                            select c).ToList();

            foreach (var entity in entities)
            {
                entityContext.Classes.Remove(entity);
            }

            entityContext.SaveChanges();
        }

        /// <summary>
        /// Gets all classes from the database
        /// </summary>
        /// <returns>The classes</returns>
        public List<Class> GetClasses()
        {
            CheckAuthentication();

            CheckAuthentication();
            return entityContext.Classes.ToList();
        }


        /// <summary>
        /// Adds a list of classes to a subject
        /// </summary>
        /// <param name="subject">The subject</param>
        /// <param name="classes">The classes to add</param>
        public void AddClassesToSubject(Subject subject, List<Class> classes)
        {
            CheckAuthentication();

            var subjectEntity = entityContext.Subjects.Include("Classes")
                .Where(s => s.Id == subject.Id).FirstOrDefault();

            foreach (var c in classes)
            {
                if (!subjectEntity.Classes.Any(cl => cl.Id == c.Id))
                {
                    Class entity = entityContext.Classes.Where(cl => cl.Id == c.Id).FirstOrDefault();
                    subjectEntity.Classes.Add(entity);
                }
            }

            entityContext.SaveChanges();
        }

        #endregion

        #region Student Management

        /// <summary>
        /// Registers a student in the system
        /// </summary>
        /// <param name="student">Student information</param>
        /// <param name="passwordCrypt">Encrypted password</param>
        /// <param name="secret">The key to decrypt with</param>
        /// /// <returns>True if the student is registered successfully, false otherwise</returns>
        public bool RegisterStudent(Student student, string passwordCrypt, string secret)
        {
            CheckAuthentication();

            string username = Crypto.DecryptStringAES(student.Username, secret);
            string password = Crypto.DecryptStringAES(passwordCrypt, secret);
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

            student.Username = username;
            student.PasswordHash = password;

            if (IsStudentValid(student))
            {
                student.PasswordHash = passwordHash;
                entityContext.Students.Add(student);
                entityContext.SaveChanges();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Add a list of students (used for importing)
        /// </summary>
        /// <param name="students">Students to add</param>
        /// <param name="secret">Key to decrypt information</param>
        public void RegisterStudents(List<Student> students, string secret)
        {
            CheckAuthentication();

            // the RegisterStudent method is not used here
            // because it checks for authentication and call SaveContext() 
            // for every item
            foreach (var s in students)
            {
                string username = Crypto.DecryptStringAES(s.Username, secret);
                string password = Crypto.DecryptStringAES(s.PasswordHash, secret);
                string passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

                Student student = new Student()
                                      {
                                          ClassId = s.ClassId,
                                          EGN = s.EGN,
                                          FirstName = s.FirstName,
                                          MiddleName = s.MiddleName,
                                          LastName = s.LastName,
                                          Username = username,
                                          PasswordHash = password
                                      };

                if(IsStudentValid(student))
                {
                    student.PasswordHash = passwordHash;
                    entityContext.Students.Add(student);
                }
            }

            entityContext.SaveChanges();
        }

        /// <summary>
        /// Edits student information
        /// </summary>
        /// <param name="studentId">The student to edit</param>
        /// <param name="student">New information about the student</param>
        /// <param name="secret">Key to decrypt username and password</param>
        public void EditStudent(int studentId, Student student, string secret)
        {
            string username = Crypto.DecryptStringAES(student.Username, secret);
            string password = Crypto.DecryptStringAES(student.PasswordHash, secret);

            var entity = entityContext.Students.Where(s => s.Id == studentId).FirstOrDefault();
            entity.ClassId = student.ClassId;
            entity.EGN = student.EGN;
            entity.FirstName = student.FirstName;
            entity.MiddleName = student.MiddleName;
            entity.LastName = student.LastName;
            entity.Username = username;
            entity.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);

            entityContext.SaveChanges();
        }

        /// <summary>
        /// Gets a student by specified id
        /// </summary>
        /// <param name="studentId">The id to search</param>
        /// <returns>The found student</returns>
        public Student GetStudentById(int studentId)
        {
            return entityContext.Students.Where(s => s.Id == studentId).FirstOrDefault();
        }

        /// <summary>
        /// Removes a list of students from the database
        /// </summary>
        /// <param name="students">The students to remove</param>
        public void RemoveStudents(List<Student> students)
        {
            CheckAuthentication();

            int[] ids = (from s in students select s.Id).ToArray();

            var entities = (from s in entityContext.Students
                            where ids.Contains(s.Id)
                            select s).ToList();

            foreach (var entity in entities)
            {
                entityContext.Students.Remove(entity);
            }

            entityContext.SaveChanges();
        }

        /// <summary>
        /// Gets all students from the system
        /// </summary>
        /// <returns>The students encapsulated in a StudentView list</returns>
        public List<StudentView> GetStudentViews()
        {
            CheckAuthentication();

            return (
                       from s in entityContext.Students
                       join c in entityContext.Classes
                           on s.ClassId equals c.Id
                       select new
                       {
                           Id = s.Id,
                           ClassNumber = c.Number,
                           ClassLetter = c.Letter,
                           EGN = s.EGN,
                           FullName = s.FirstName + " " + s.MiddleName + " " + s.LastName,
                           Username = s.Username
                       })
                                  .AsEnumerable()
                                  .Select(x => new StudentView()
                                  {
                                      Class = string.Format("{0} '{1}'", x.ClassNumber, x.ClassLetter),
                                      EGN = x.EGN,
                                      FullName = x.FullName,
                                      Id = x.Id,
                                      Username = x.Username
                                  }).ToList();
        }

        /// <summary>
        /// Validates student information
        /// </summary>
        /// <param name="student">Student information</param>
        /// <returns>Whether the student information is valid</returns>
        private bool IsStudentValid(Student student)
        {
            const int MAX_NAME_LENGTH = 32;
            const int MIN_PASS_LENGTH = 4;
            const int MAX_PASS_LENGTH = 32;

            if (string.IsNullOrEmpty(student.Username)
                || string.IsNullOrWhiteSpace(student.Username))
            {
                return false;
            }

            if (!Regex.IsMatch(student.Username, "^[a-zA-Z]+[a-zA-Z0-9_\\.]*$"))
            {
                return false;
            }

            if (student.Username.Length > MAX_NAME_LENGTH)
            {
                return false;
            }

            if (string.IsNullOrEmpty(student.FirstName)
                || string.IsNullOrWhiteSpace(student.FirstName))
            {
                return false;
            }

            if (!Regex.IsMatch(student.FirstName, "\\A[а-яА-Я]+(-)?[а-яА-Я]+\\Z"))
            {
                return false;
            }

            if (student.FirstName.Length > MAX_NAME_LENGTH)
            {
                return false;
            }

            if (string.IsNullOrEmpty(student.MiddleName)
                || string.IsNullOrWhiteSpace(student.MiddleName))
            {
                return false;
            }

            if (!Regex.IsMatch(student.MiddleName, "\\A[а-яА-Я]+(-)?[а-яА-Я]+\\Z"))
            {
                return false;
            }

            if (student.MiddleName.Length > MAX_NAME_LENGTH)
            {
                return false;
            }

            if (string.IsNullOrEmpty(student.LastName)
                || string.IsNullOrWhiteSpace(student.LastName))
            {
                return false;
            }

            if (!Regex.IsMatch(student.LastName, "\\A[а-яА-Я]+(-)?[а-яА-Я]+\\Z"))
            {
                return false;
            }

            if (student.LastName.Length > MAX_NAME_LENGTH)
            {
                return false;
            }

            if (EgnValidator.IsEgnValid(student.EGN))
            {
                return false;
            }

            if (string.IsNullOrEmpty(student.PasswordHash)
                || string.IsNullOrWhiteSpace(student.PasswordHash))
            {
                return false;
            }

            if (student.PasswordHash.Length < MIN_PASS_LENGTH
                || student.PasswordHash.Length > MAX_PASS_LENGTH)
            {
                return false;
            }

            if (entityContext.Students.Any(s => s.Username == student.Username
                || s.EGN == student.EGN))
            {
                return false;
            }

            if(entityContext.Students.Any(s => s.EGN == student.EGN))
            {
                return false;
            }

            return true;
        }

        #endregion

        #region Subject Management

        /// <summary>
        /// Adds a subject to the database
        /// </summary>
        /// <param name="subject">The subject to add</param>
        public void AddSubject(Subject subject)
        {
            CheckAuthentication();

            entityContext.Subjects.Add(subject);
            entityContext.SaveChanges();
        }

        /// <summary>
        /// Removes a list of subjects
        /// </summary>
        /// <param name="subjects">The subjects to remove</param>
        public void RemoveSubjects(List<Subject> subjects)
        {
            CheckAuthentication();

            int[] ids = (from s in subjects select s.Id).ToArray();

            var entities = (from s in entityContext.Subjects
                            where ids.Contains(s.Id)
                            select s).ToList();

            foreach (var entity in entities)
            {
                entityContext.Subjects.Remove(entity);
            }

            entityContext.SaveChanges();
        }

        /// <summary>
        /// Gets all subjects from the database
        /// </summary>
        /// <returns>The subjects, encapsulated in a SubjectView list</returns>
        public List<SubjectView> GetSubjectViews()
        {
            CheckAuthentication();

            return (
                       from s in entityContext.Subjects
                       join t in entityContext.Teachers
                           on s.TeacherId equals t.Id
                       select new SubjectView()
                       {
                           Id = s.Id,
                           Name = s.Name,
                           TeacherFullName = t.FirstName + " " + t.MiddleName + " " + t.LastName
                       }).ToList();
        }

        /// <summary>
        /// Adds a list of subjects to a class
        /// </summary>
        /// <param name="c">The class</param>
        /// <param name="subjects">The subjects to add</param>
        public void AddSubjectsToClass(Class c, List<Subject> subjects)
        {
            CheckAuthentication();

            var classEntity = entityContext.Classes.Include("Subjects")
                .Where(cl => cl.Id == c.Id).FirstOrDefault();

            foreach (var subject in subjects)
            {
                if (!classEntity.Subjects.Any(s => s.Id == subject.Id))
                {
                    Subject entity = entityContext.Subjects.Where(s => s.Id == subject.Id).FirstOrDefault();
                    classEntity.Subjects.Add(entity);
                }
            }

            entityContext.SaveChanges();
        }

        /// <summary>
        /// Gets all subjects by a given class
        /// </summary>
        /// <param name="classId">The class to search</param>
        /// <returns>All subjects by that class</returns>
        public List<Subject> GetSubjectsByClass(int classId)
        {
            CheckAuthentication();

            var sub = (from cl in entityContext.Classes.Include("Subjects")
                       where cl.Id == classId
                       select cl.Subjects).First().ToList();

            return sub;
        }

        #endregion

        #region TeacherManagement

        /// <summary>
        /// Registers a teacher in the system
        /// </summary>
        /// <param name="teacher">Teacher information</param>
        /// <param name="passwordCrypt">Encrypted password</param>
        /// <param name="secret">The skey to decrypt with</param>
        /// <returns>True if the teacher is registered successfully, false otherwise</returns>
        public bool RegisterTeacher(Teacher teacher, string passwordCrypt, string secret)
        {
            CheckAuthentication();

            string username = Crypto.DecryptStringAES(teacher.Username, secret);
            string password = Crypto.DecryptStringAES(passwordCrypt, secret);
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

            teacher.Username = username;
            teacher.PasswordHash = password;

            if (IsTeacherValid(teacher))
            {
                teacher.PasswordHash = passwordHash;
                entityContext.Teachers.Add(teacher);
                entityContext.SaveChanges();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Add a list of teachers (used for importing
        /// </summary>
        /// <param name="teachers">The teachers to add</param>
        /// <param name="secret">Key to decrypt with</param>
        public void RegisterTeachers(List<Teacher> teachers, string secret)
        {
            CheckAuthentication();

            // the RegisterTeachers method is not used here
            // because it checks for authentication and call SaveContext() 
            // for every item
            foreach (var t in teachers)
            {
                string username = Crypto.DecryptStringAES(t.Username, secret);
                string password = Crypto.DecryptStringAES(t.PasswordHash, secret);
                string passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

                Teacher teacher = new Teacher()
                                      {
                                          FirstName = t.FirstName,
                                          MiddleName = t.MiddleName,
                                          LastName = t.LastName,
                                          PasswordHash = password,
                                          Username = username
                                      };

                if(IsTeacherValid(teacher))
                {
                    teacher.PasswordHash = passwordHash;
                    entityContext.Teachers.Add(teacher);
                }
            }

            entityContext.SaveChanges();
        }

        /// <summary>
        /// Removes a list of teacher from the system
        /// </summary>
        /// <param name="teachers">The teachers to remove</param>
        public void RemoveTeachers(List<Teacher> teachers)
        {
            CheckAuthentication();

            int[] ids = (from t in teachers select t.Id).ToArray();

            var entities = (from t in entityContext.Teachers
                            where ids.Contains(t.Id)
                            select t).ToList();

            foreach (var entity in entities)
            {
                entityContext.Teachers.Remove(entity);
            }

            entityContext.SaveChanges();
        }

        /// <summary>
        /// Gets all teachers from the system
        /// </summary>
        /// <returns></returns>
        public List<Teacher> GetTeachers()
        {
            CheckAuthentication();

            return entityContext.Teachers.ToList();
        }

        /// <summary>
        /// Validates teacher information
        /// </summary>
        /// <param name="teacher">Teacher information</param>
        /// <returns>Whether the information is valid</returns>
        private bool IsTeacherValid(Teacher teacher)
        {
            const int MAX_NAME_LENGTH = 32;
            const int MIN_PASS_LENGTH = 4;
            const int MAX_PASS_LENGTH = 32;

            if (string.IsNullOrEmpty(teacher.Username)
                || string.IsNullOrWhiteSpace(teacher.Username))
            {
                return false;
            }

            if (!Regex.IsMatch(teacher.Username, "^[a-zA-Z]+[a-zA-Z0-9_\\.]*$"))
            {
                return false;
            }

            if (teacher.Username.Length > MAX_NAME_LENGTH)
            {
                return false;
            }

            if (string.IsNullOrEmpty(teacher.FirstName)
                || string.IsNullOrWhiteSpace(teacher.FirstName))
            {
                return false;
            }

            if (!Regex.IsMatch(teacher.FirstName, "\\A[а-яА-Я]+(-)?[а-яА-Я]+\\Z"))
            {
                return false;
            }

            if (teacher.FirstName.Length > MAX_NAME_LENGTH)
            {
                return false;
            }

            if (string.IsNullOrEmpty(teacher.MiddleName)
                || string.IsNullOrWhiteSpace(teacher.MiddleName))
            {
                return false;
            }

            if (!Regex.IsMatch(teacher.MiddleName, "\\A[а-яА-Я]+(-)?[а-яА-Я]+\\Z"))
            {
                return false;
            }

            if (teacher.MiddleName.Length > MAX_NAME_LENGTH)
            {
                return false;
            }

            if (string.IsNullOrEmpty(teacher.LastName)
                || string.IsNullOrWhiteSpace(teacher.LastName))
            {
                return false;
            }

            if (!Regex.IsMatch(teacher.LastName, "\\A[а-яА-Я]+(-)?[а-яА-Я]+\\Z"))
            {
                return false;
            }

            if (teacher.LastName.Length > MAX_NAME_LENGTH)
            {
                return false;
            }

            if (string.IsNullOrEmpty(teacher.PasswordHash)
                || string.IsNullOrWhiteSpace(teacher.PasswordHash))
            {
                return false;
            }

            if (teacher.PasswordHash.Length < MIN_PASS_LENGTH
                || teacher.PasswordHash.Length > MAX_PASS_LENGTH)
            {
                return false;
            }

            if (!entityContext.Teachers.Any(t => t.Username == teacher.Username))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Edits teacher information
        /// </summary>
        /// <param name="teacherId">The teacher to edit</param>
        /// <param name="teacher">Teacher information</param>
        /// <param name="secret">Key to decrypt username and password</param>
        public void EditTeacher(int teacherId, Teacher teacher, string secret)
        {
            string username = Crypto.DecryptStringAES(teacher.Username, secret);
            string password = Crypto.DecryptStringAES(teacher.PasswordHash, secret);

            var entity = entityContext.Teachers.Where(t => t.Id == teacherId).FirstOrDefault();
            entity.Username = username;
            entity.FirstName = teacher.FirstName;
            entity.MiddleName = teacher.MiddleName;
            entity.LastName = teacher.LastName;
            entity.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);

            entityContext.SaveChanges();
        }

        /// <summary>
        /// Gets a teacher by given id
        /// </summary>
        /// <param name="teacherId">The teacher id to search</param>
        /// <returns>The found teacher</returns>
        public Teacher GetTeacherById(int teacherId)
        {
            return entityContext.Teachers.Where(t => t.Id == teacherId).FirstOrDefault();
        }

        #endregion

        #region Admin Management

        /// <summary>
        /// Registers an admin in the system
        /// </summary>
        /// <param name="admin">Admin information</param>
        /// <param name="passwordCrypt">Encrypted password</param>
        /// <param name="secret">The key to decrypt with</param>
        public bool RegisterAdmin(Admin admin, string passwordCrypt, string secret)
        {
            CheckAuthentication();

            string password = Crypto.DecryptStringAES(passwordCrypt, secret);
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
            admin.PasswordHash = passwordHash;

            if(IsAdminValid(admin))
            {
                entityContext.Admins.Add(admin);
                entityContext.SaveChanges();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Logs and admin into the system
        /// </summary>
        /// <param name="usernameCrypt">Encrypted username</param>
        /// <param name="passwordCrypt">Encrypted password</param>
        /// <param name="secret">The key to decrypt with</param>
        /// <returns>Admin information (if successfull login)</returns>
        public Admin LoginAdmin(string usernameCrypt, string passwordCrypt, string secret)
        {
            //empty information
            if(string.IsNullOrWhiteSpace(usernameCrypt) || string.IsNullOrEmpty(usernameCrypt)
                || string.IsNullOrWhiteSpace(passwordCrypt) || string.IsNullOrEmpty(passwordCrypt)
                || string.IsNullOrWhiteSpace(secret) || string.IsNullOrEmpty(secret))
            {
                return null;
            }
        

            //decrypt login details
            string username = Crypto.DecryptStringAES(usernameCrypt, secret);
            string password = Crypto.DecryptStringAES(passwordCrypt, secret);

            if(entityContext.Admins.Count(a => a.Username == username) == 0)
            {
                return null;
            }

            Admin entity = entityContext.Admins.Where(a => a.Username == username).First();
            if (BCrypt.Net.BCrypt.Verify(password, entity.PasswordHash))
            {
                //password is valid
                isLogged = true;
                return entity;
            }

            return null;
        }

        /// <summary>
        /// Validates admin information
        /// </summary>
        /// <param name="admin">Admin information</param>
        /// <returns>Whether the information is valid</returns>
        private bool IsAdminValid(Admin admin)
        {
            const int MAX_ADMIN_USERNAME_LENGTH = 32;

            if (string.IsNullOrEmpty(admin.Username)
                || string.IsNullOrWhiteSpace(admin.Username))
            {
                return false;
            }

            if(admin.Username.Length > MAX_ADMIN_USERNAME_LENGTH)
            {
                return false;
            }

            if (!Regex.IsMatch(admin.Username, "^[a-zA-Z]+[a-zA-Z0-9_\\.]*$"))
            {
                return false;
            }

            if(entityContext.Admins.Any(a => a.Username == admin.Username))
            {
                return false;
            }

            return true;
        }

        #endregion
    }
}
