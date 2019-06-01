using Laborator2.Models;
using Laborator2.Services;
using Laborator2.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Tests
{
    public class UsersServiceTests
    {
        private IOptions<AppSettings> config;
        [SetUp]
        public void Setup()
        {
            config = Options.Create(new AppSettings
            {
                Secret = "qwruiolmkjdertykhgi"
            });
        }

        [Test]
        public void ValidRegisterShouldCreateNewUser()
        {
            var options = new DbContextOptionsBuilder<ExpensesDbContext>()
                .UseInMemoryDatabase(databaseName: nameof(ValidRegisterShouldCreateNewUser))
                .Options;

            using (var context = new ExpensesDbContext(options))
            {
                var userService = new UsersService(context, config);

                var added = new Laborator2.ViewModels.RegisterPostModel
                {
                    Email = "x@y.z",
                    FirstName = "abc",
                    LastName = "def",
                    Password = "1234567",
                    Username = "abc"

                };

                var result = userService.Register(added);

                Assert.IsNotNull(result);
                Assert.AreEqual(added.Username, result.Username);
            }

        }

        [Test]
        public void InvalidRegisterShouldNotCerateNewUserWithTheSameUsername()
        {
            var options = new DbContextOptionsBuilder<ExpensesDbContext>()
                .UseInMemoryDatabase(databaseName: nameof(InvalidRegisterShouldNotCerateNewUserWithTheSameUsername))
                .Options;

            using (var context = new ExpensesDbContext(options))
            {
                var userService = new UsersService(context, config);

                var added1 = new Laborator2.ViewModels.RegisterPostModel
                {
                    Email = "aaa@bbb.ccc",
                    FirstName = "aaa",
                    LastName = "bbb",
                    Password = "123456789",
                    Username = "xxx"

                };
                var added2 = new Laborator2.ViewModels.RegisterPostModel
                {
                    Email = "xxx@yyy.zzz",
                    FirstName = "xxx",
                    LastName = "yyy",
                    Password = "12345678910",
                    Username = "xxx"

                };

                userService.Register(added1);
                var result = userService.Register(added2);

                Assert.AreEqual(null, result);
            }

        }

        [Test]
        public void ValidAuthenticationShouldAuthenticateValidUser()
        {
            var options = new DbContextOptionsBuilder<ExpensesDbContext>()
                .UseInMemoryDatabase(databaseName: nameof(ValidAuthenticationShouldAuthenticateValidUser))
                .Options;

            using (var context = new ExpensesDbContext(options))
            {
                var userService = new UsersService(context, config);

                var addedUser = new Laborator2.ViewModels.RegisterPostModel
                {
                    Email = "d@e.f",
                    FirstName = "ddd",
                    LastName = "eee",
                    Password = "111222333",
                    Username = "ddd"

                };

                var addResult = userService.Register(addedUser);

                Assert.IsNotNull(addResult);
                Assert.AreEqual(addedUser.Username, addResult.Username);

                var authenticate = new Laborator2.ViewModels.UserGetModel
                {
                    Email = "d@e.f",
                    Username = "ddd"
                };

                var result = userService.Authenticate(addedUser.Username, addedUser.Password);

                Assert.IsNotNull(result);

                Assert.AreEqual(authenticate.Username, result.Username);
            }

        }

        [Test]
        public void InvalidAuthenticationShouldNotAuthenticateUserWithInvalidPassword()
        {
            var options = new DbContextOptionsBuilder<ExpensesDbContext>()
                .UseInMemoryDatabase(databaseName: nameof(InvalidAuthenticationShouldNotAuthenticateUserWithInvalidPassword))
                .Options;

            using (var context = new ExpensesDbContext(options))
            {
                var userService = new UsersService(context, config);

                var addedUser = new Laborator2.ViewModels.RegisterPostModel
                {
                    Email = "g@h.i",
                    FirstName = "ggg",
                    LastName = "hhh",
                    Password = "12345678",
                    Username = "ggg"

                };

                var addResult = userService.Register(addedUser);

                Assert.IsNotNull(addResult);
                Assert.AreEqual(addedUser.Username, addResult.Username);

                var authenticate = new Laborator2.ViewModels.UserGetModel
                {
                    Email = "g@h.i",
                    Username = "ggg"
                };

                var result = userService.Authenticate(addedUser.Username, "11111111");

                Assert.AreEqual(null, result);
            }

        }

        [Test]
        public void ValidGetAllShouldReturnAllUsers()
        {
            var options = new DbContextOptionsBuilder<ExpensesDbContext>()
                .UseInMemoryDatabase(databaseName: nameof(ValidGetAllShouldReturnAllUsers))
                .Options;

            using (var context = new ExpensesDbContext(options))
            {
                var userService = new UsersService(context, config);

                var addedUser1 = new Laborator2.ViewModels.RegisterPostModel
                {
                    Email = "a@b.c",
                    FirstName = "aaa",
                    LastName = "bbb",
                    Password = "0987654321",
                    Username = "user1"

                };
                var addedUser2 = new Laborator2.ViewModels.RegisterPostModel
                {
                    Email = "b@c.d",
                    FirstName = "bbb",
                    LastName = "ccc",
                    Password = "34567890",
                    Username = "user2"

                };

                var addedUser3 = new Laborator2.ViewModels.RegisterPostModel
                {
                    Email = "c@d.e",
                    FirstName = "ccc",
                    LastName = "ddd",
                    Password = "567890",
                    Username = "user3"

                };

                UserGetModel user1 = userService.Register(addedUser1);
                UserGetModel user2 = userService.Register(addedUser2);
                UserGetModel user3 = userService.Register(addedUser3);
                List<UserGetModel> actual = new List<UserGetModel>();
                user1.Token = null;
                user2.Token = null;
                user3.Token = null;
                actual.Add(user1);
                actual.Add(user2);
                actual.Add(user3);

                IEnumerable<UserGetModel> result = userService.GetAll();
                IEnumerable<UserGetModel> expected = actual.AsEnumerable();

                Assert.IsTrue(expected.SequenceEqual(actual));

            }

        }

    }
}