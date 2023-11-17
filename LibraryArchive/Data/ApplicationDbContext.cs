using LibraryArchive.HelpingTools;
using LibraryArchive.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryArchive.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            this.Users = this.Set<User>();           
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure BookAuthor as a junction table with no primary key
            modelBuilder.Entity<BookAuthor>()
                .HasKey(ba => new { ba.BookId, ba.AuthorId });

            // Define the many-to-many relationship between Book and Author
            modelBuilder.Entity<Book>()
                .HasMany(b => b.Authors)
                .WithMany(a => a.Books)
                .UsingEntity<BookAuthor>(
                    j => j
                        .HasOne(ba => ba.Author)
                        .WithMany()
                        .HasForeignKey(ba => ba.AuthorId),
                    j => j
                        .HasOne(ba => ba.Book)
                        .WithMany()
                        .HasForeignKey(ba => ba.BookId)
                );

            modelBuilder.Entity<BookGenre>()
                .HasKey(ba => new { ba.BookId, ba.GenreId });

            // Define the many-to-many relationship between Book and Genre
            modelBuilder.Entity<Book>()
                .HasMany(b => b.Genres)
                .WithMany(a => a.Books)
                .UsingEntity<BookGenre>(
                    j => j
                        .HasOne(ba => ba.Genre)
                        .WithMany()
                        .HasForeignKey(ba => ba.GenreId),
                    j => j
                        .HasOne(ba => ba.Book)
                        .WithMany()
                        .HasForeignKey(ba => ba.BookId)
                );

            modelBuilder.Entity<Borrowing>()
                 .HasOne(b => b.User)
                 .WithMany(u => u.Borrowings)
                 .HasForeignKey(b => b.UserId)
                 .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Borrowing>()
                .HasOne(b => b.Book)
                .WithMany()
                .HasForeignKey(b => b.BookId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Role>().HasData(
               new Role
               {
                   Id = 1,
                   RoleName = "Librarian"
               });

            modelBuilder.Entity<Role>().HasData(
               new Role
               {
                   Id = 2,
                   RoleName = "Reader"
               });

            modelBuilder.Entity<Librarian>().HasData(
               new Librarian
               {
                   Id = 1,
                   FirstName = "Виктория",
                   LastName = "Тотева",
                   BirthDate = new DateTime(2003, 4, 25),
                   PhoneNumber = "0885904536",
                   Username = "vttoteva",
                   Email = "viktoriya.toteva@abv.bg",
                   RoleId = 1,
                   Password = Hasher.Hash("123")
               }
             );           

            base.OnModelCreating(modelBuilder);          
        }

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Librarian> Librarians { get; set; }
        public virtual DbSet<Book> Books { get; set; }
        public virtual DbSet<Author> Authors { get; set; }
        public virtual DbSet<Borrowing> Borrowing { get; set; }
        public virtual DbSet<Genre> Genres { get; set; }
        public virtual DbSet<Publisher> Publishers { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<BookAuthor> BookAuthors { get; set; }
        public virtual DbSet<BookGenre> BookGenres { get; set; }
    }
}
