using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace VirtualClassroom.Services.Models
{
    /// <summary>
    /// Code-first database model
    /// </summary>
    public partial class VirtualClassroomEntities : DbContext
    {
        public VirtualClassroomEntities()
            : base("name=VirtualClassroomEntities")
        {
            //disable lazy loading and proxy creation to avoid
            //circular reference
            this.Configuration.ProxyCreationEnabled = false;
            this.Configuration.LazyLoadingEnabled = false;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {

        }

        public DbSet<Class> Classes { get; set; }
        public DbSet<Homework> Homeworks { get; set; }
        public DbSet<Lesson> Lessons { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Mark> Marks { get; set; }
    }
}