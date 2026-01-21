using Microsoft.EntityFrameworkCore;
using LibraryManagement.Models;

namespace LibraryManagement.Data
{
    public class LibraryDbContext : DbContext
    {
        public LibraryDbContext(DbContextOptions<LibraryDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<BookIssue> BookIssues { get; set; }
        public DbSet<Fine> Fines { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships
            modelBuilder.Entity<BookIssue>()
                .HasOne(bi => bi.Book)
                .WithMany(b => b.BookIssues)
                .HasForeignKey(bi => bi.BookId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BookIssue>()
                .HasOne(bi => bi.User)
                .WithMany(u => u.BookIssues)
                .HasForeignKey(bi => bi.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Fine>()
                .HasOne(f => f.BookIssue)
                .WithOne(bi => bi.Fine)
                .HasForeignKey<Fine>(f => f.IssueId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Fine>()
                .HasOne(f => f.User)
                .WithMany(u => u.Fines)
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Seed initial data
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Seed Admin user (password: admin123)
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    UserId = 1,
                    Name = "Admin User",
                    Email = "admin@library.com",
                    Password = BCrypt.Net.BCrypt.HashPassword("admin123"),
                    Role = UserRole.Admin,
                    Phone = "1234567890",
                    Address = "Library HQ",
                    CreatedDate = DateTime.Now,
                    IsActive = true
                }
            );

            // Seed sample books
            modelBuilder.Entity<Book>().HasData(
                new Book
                {
                    BookId = 1,
                    ISBN = "9780132350884",
                    Title = "Clean Code",
                    Author = "Robert C. Martin",
                    Publisher = "Prentice Hall",
                    PublishedYear = 2008,
                    Category = "Programming",
                    TotalCopies = 5,
                    AvailableCopies = 5,
                    Description = "A Handbook of Agile Software Craftsmanship",
                    AddedDate = DateTime.Now,
                    IsActive = true
                },
                new Book
                {
                    BookId = 2,
                    ISBN = "9780134685991",
                    Title = "Effective Java",
                    Author = "Joshua Bloch",
                    Publisher = "Addison-Wesley",
                    PublishedYear = 2018,
                    Category = "Programming",
                    TotalCopies = 3,
                    AvailableCopies = 3,
                    Description = "Best practices for the Java platform",
                    AddedDate = DateTime.Now,
                    IsActive = true
                },
                new Book
                {
                    BookId = 3,
                    ISBN = "9780201633610",
                    Title = "Design Patterns",
                    Author = "Gang of Four",
                    Publisher = "Addison-Wesley",
                    PublishedYear = 1994,
                    Category = "Software Engineering",
                    TotalCopies = 4,
                    AvailableCopies = 4,
                    Description = "Elements of Reusable Object-Oriented Software",
                    AddedDate = DateTime.Now,
                    IsActive = true
                }
            );
        }
    }
}
