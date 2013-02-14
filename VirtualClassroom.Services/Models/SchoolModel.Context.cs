using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace VirtualClassroom.Services.Models
{
    public partial class VirtualClassroomEntities : DbContext
    {
        public VirtualClassroomEntities()
            : base("name=VirtualClassroomEntities")
        {
            this.Configuration.ProxyCreationEnabled = false;
            this.Configuration.LazyLoadingEnabled = false;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //throw new UnintentionalCodeFirstException();
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