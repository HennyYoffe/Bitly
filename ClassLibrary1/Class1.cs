using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;

namespace ClassLibrary1
{
    
      public class User
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Email { get; set; }
            public string PasswordHash { get; set; }
            public List<Bitly> Bitlies { get; set; }          
        }
        public class Bitly
        {
            public int Id { get; set; }
            public string Url { get; set; } 
            public string Hash { get; set; }

            public int Views { get; set; }

            public int? UserId { get; set; }
            public User User { get; set; }
        }
        public class BitlyManager
        {
            private string _connectionString;

            public BitlyManager(string connectionString)
            {
                _connectionString = connectionString;
            }

            #region User
            public User GetUserById(int id)
            {
                using (var ctx = new BitlyContext(_connectionString))
                {
                    return ctx.Users.FirstOrDefault(u => u.Id == id);
                }
            }
            public User GetByEmail(string email)
            {
                using (var ctx = new BitlyContext(_connectionString))
                {
                    return ctx.Users.FirstOrDefault(u => u.Email == email);
                }
            }
            public User Login(string email, string password)
            {
                var user = GetByEmail(email);
                if (user == null)
                {
                    return null;
                }

                bool isCorrectPassword = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
                if (isCorrectPassword)
                {
                    return user;
                }

                return null;
            }

            public void AddUser(User user, string password)
            {
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
                using (var ctx = new BitlyContext(_connectionString))
                {
                    ctx.Users.Add(user);
                    ctx.SaveChanges();
                }
            }

            #endregion
            #region Bitly
            public int AddBitly(Bitly b)
            {            

                using (var ctx = new BitlyContext(_connectionString))
                {
                    ctx.Bitlies.Add(b);
                    ctx.SaveChanges();
                    return b.Id;
                }
            }
            public bool CheckIfContainedHash(string hash)
            {
                using (var ctx = new BitlyContext(_connectionString))
                {
                   return ctx.Bitlies.Any(b => b.Hash == hash);
                }
            }
            public Bitly GetBitlyForHash(string hash)
            {
                using (var ctx = new BitlyContext(_connectionString))
                {
                    return ctx.Bitlies.Include(b=> b.User).FirstOrDefault(b => b.Hash == hash);
                }
            }
            public List<Bitly> GetBitlyForUser(int UserId)
            {
                using (var ctx = new BitlyContext(_connectionString))
                {
                    return ctx.Bitlies.Include(b => b.User).Where(b => b.UserId == UserId).ToList();
                }
            }
        public void AddView(int bitlyid, int views)
        {
            using (var ctx = new BitlyContext(_connectionString))
            {
                ctx.Database.ExecuteSqlCommand("UPDATE Bitly SET Views = @view WHERE Id = @id",
                                       new SqlParameter("@view", views +1),
             new SqlParameter("@id", bitlyid));
            }
        }
            #endregion
        }
        public class BitlyContext : DbContext
        {
            private string _connectionString;

            public BitlyContext(string connectionString)
            {
                _connectionString = connectionString;
            }

            public DbSet<User> Users { get; set; }
           public DbSet<Bitly> Bitlies { get; set; }
            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                optionsBuilder.UseSqlServer(_connectionString);
            }

        }
        public class BitlyContextFactory : IDesignTimeDbContextFactory<BitlyContext>
        {
            public BitlyContext CreateDbContext(string[] args)
            {
                var config = new ConfigurationBuilder()
                    .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), $"..{Path.DirectorySeparatorChar}HW65_Bitly_Route_May20"))
                    .AddJsonFile("appsettings.json")
                    .AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true).Build();

                return new BitlyContext(config.GetConnectionString("ConStr"));
            }
        }
    }


//#region Errand
//public int AddErrand(Errand e)
//{
//    using (var ctx = new ErrandContext(_connectionString))
//    {
//        ctx.Errands.Add(e);
//        ctx.SaveChanges();
//        return e.Id;
//    }
//}
//public List<Errand> GetErrands()
//{
//    using (var ctx = new ErrandContext(_connectionString))
//    {
//        return ctx.Errands.Include(e => e.User).ToList();
//    }
//}
//public Errand GetErrand(int id)
//{
//    using (var ctx = new ErrandContext(_connectionString))
//    {
//        return ctx.Errands.Include(e => e.User).FirstOrDefault(e => e.Id == id);
//    }
//}
//public void TakeTask(int errandId, int userid)
//{
//    using (var context = new ErrandContext(_connectionString))
//    {
//        context.Database.ExecuteSqlCommand("UPDATE Errand SET Status = @status UserId= @userid WHERE Id = @id",
//                              new SqlParameter("@status", Status.Taken),
//                                new SqlParameter("@userid", userid),
//    new SqlParameter("@id", errandId));
//    }
//}
//public void CompleteTask(int errandId)
//{
//    using (var context = new ErrandContext(_connectionString))
//    {
//        context.Database.ExecuteSqlCommand("UPDATE Errand SET Status = @status WHERE Id = @id",
//                              new SqlParameter("@status", Status.Completed),
//    new SqlParameter("@id", errandId));
//    }
//}



//#endregion