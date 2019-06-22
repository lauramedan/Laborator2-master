using Laborator2.Models;
using Laborator2.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Laborator2.Services
{
    public interface IUsersService
    {
        UserGetModel Authenticate(string username, string password);
        IEnumerable<UserGetModel> GetAll();
        User GetUserById(int id);
        UserGetModel Register(RegisterPostModel registerInfo);
        User GetLoggedInUser(HttpContext httpContext);
        User Delete(int id, User loggedInUser, out int notFoundOrForbidden);
        User Upsert(int id, User user, User loggedInUser);
    }

    public class UsersService : IUsersService
    {

        private ExpensesDbContext context;

        private readonly AppSettings appSettings;

        public UsersService(ExpensesDbContext context, IOptions<AppSettings> appSettings)
        {
            this.appSettings = appSettings.Value;
            this.context = context;
        }

        public UserGetModel Authenticate(string username, string password)
        {
            var user = context.Users.SingleOrDefault(x => x.Username == username && x.Password == ComputeSha256Hash(password));

            // return null if user not found
            if (user == null)
            {
                return null;
            }

            // authentication successful so generate jwt token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Username.ToString()),
                    new Claim(ClaimTypes.Role, user.UserRole.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);

            var result = new UserGetModel
            {
                Id = user.Id,
                Email = user.Email,
                Username = user.Username,
                Token = tokenHandler.WriteToken(token),
                UserRole = user.UserRole,
                UserRoleStartDate = user.UserRoleStartDate
            };


            return result;

        }

        public IEnumerable<UserGetModel> GetAll()
        {
            // return users without passwords
            return context.Users.Select(user => new UserGetModel
            {
                Id = user.Id,
                Email = user.Email,
                Username = user.Username,
                Token = null,
                UserRole = user.UserRole,
                UserRoleStartDate = user.UserRoleStartDate

            });
        }

        public User GetUserById(int id)
        {
            return context.Users.AsNoTracking()
                .FirstOrDefault(u => u.Id == id);
        }

        private string ComputeSha256Hash(string rawData)
        {
            // Create a SHA256   
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array  
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        public UserGetModel Register(RegisterPostModel registerInfo)
        {
            User existing = context.Users.FirstOrDefault(u => u.Username == registerInfo.Username);
            if (existing != null)
            {
                return null;
            }
            context.Users.Add(new User
            {
                Email = registerInfo.Email,
                LastName = registerInfo.LastName,
                FirstName = registerInfo.FirstName,
                Password = ComputeSha256Hash(registerInfo.Password),
                Username = registerInfo.Username,
                UserRole = UserRole.Regular,
                UserRoleStartDate = DateTime.Now

            });
            context.SaveChanges();
            return Authenticate(registerInfo.Username, registerInfo.Password);

        }

        // noi

        public User GetLoggedInUser(HttpContext httpContext)
        {
            string username = httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value;
            return context.Users.FirstOrDefault(u => u.Username == username);
        }

        public User Delete(int id, User loggedInUser, out int notFoundOrForbidden)
        {
            User existing = context.Users.FirstOrDefault(u => u.Id == id); // verifies there's a user with the given ID
            if (existing == null) // if there isn't one...
            {
                notFoundOrForbidden = 1;
                return null; // there's nothing to delete
            }


            if (
                (loggedInUser.UserRole == UserRole.Admin) // if I am logged in as an Admin
                ||  // or
               (existing.UserRole == UserRole.UserManager && (loggedInUser.UserRoleStartDate.AddMonths(6) <= DateTime.Now)) // if the user I want to delete is a UserManager && I am logged in as a UserManager (at least 6 months old) 
                ||  // or
               (existing.UserRole != UserRole.Admin && existing.UserRole != UserRole.UserManager) // if the user I want to delete is not an Admin or a UserManager
               )
            {
                context.Users.Remove(existing); // Then DELETE
                context.SaveChanges();
                notFoundOrForbidden = 0;
                return existing;
            }

            notFoundOrForbidden = 2;
            return null;
        }

        public User Upsert(int id, User user, User loggedInUser)
        {
            var existing = context.Users.AsNoTracking().FirstOrDefault(c => c.Id == id);
            if (existing == null)
            {
                if (
                    (loggedInUser.UserRole == UserRole.Admin) // if the logged in user is an Admin
                    ||  //or
                    (user.UserRole == UserRole.UserManager && (loggedInUser.UserRoleStartDate.AddMonths(6) <= DateTime.Now)) // if the user to be added is a UserManager && the logged in is a UserManager (at least 6 months old) 
                    ||  //or
                    (user.UserRole != UserRole.Admin && user.UserRole != UserRole.UserManager) // if the user to be added is neither an Admin nor a UserManager
                )
                {
                    user.Password = ComputeSha256Hash(user.Password);
                    context.Users.Add(user);
                    context.SaveChanges();
                    return user;
                }
                return null;
            }

            if (
                (loggedInUser.UserRole == UserRole.Admin) // if the logged in user is an Admin
                ||  //or
               (existing.UserRole == UserRole.UserManager && (loggedInUser.UserRoleStartDate.AddMonths(6) <= DateTime.Now)) // if the user to be updated is a UserManager && the logged in user is a UserManager (at least 6 months old) 
                ||  //or
               (existing.UserRole != UserRole.Admin && existing.UserRole != UserRole.UserManager) // if the user to be updated is neither an Admin nor a UserManager
               )
            {
                if (loggedInUser.UserRole == UserRole.UserManager)
                { // A UserManager cannot update other users' role
                    //if (user.UserRole != existing.UserRole)
                    //{
                    //    return null;
                    //}
                    user.UserRole = existing.UserRole; // keep the existing/old role
                    user.UserRoleStartDate = existing.UserRoleStartDate; // keep the existing role start date
                }

                user.Id = id;

                user.Password = ComputeSha256Hash(user.Password);
                context.Users.Update(user); // Then UPDATE
                context.SaveChanges();
                return user;
            }

            return null;
        }
    }
}
