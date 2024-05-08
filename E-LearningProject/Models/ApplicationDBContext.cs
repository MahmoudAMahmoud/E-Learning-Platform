using Microsoft.EntityFrameworkCore;


namespace E_LearningProject.Models
{
    public class ApplicationDBContext : DbContext
    {

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=GHOST\\SQLEXPRESS;Database=GraduateProjectDB;Integrated Security=True;Encrypt=False;");
        }
        public DbSet<User> Users { get; set; }

        public DbSet<Course> Courses { get; set; }

        public DbSet<Category> Categories { get; set; }



    }
} 