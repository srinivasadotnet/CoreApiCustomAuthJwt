using AutoMapper.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NetCoreAPI.Entities;
using NetCoreAPI.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreAPI.Services
{
    public interface IUserService
    {
        User Authenticate(string username, string password);
        IEnumerable<User> GetAll();
        User GetById(int id);
        User Create(User user, string password);
        void Update(User user, string password = null);
        void Delete(int id);
    }
    public class UserService : IUserService
    {
        private AppSettings AppSettings { get; }

        public UserService(IOptions<AppSettings> appSettings)
        {
            AppSettings = appSettings.Value;
        }

        public User Authenticate(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password)) return null;

            var user = DBUtility.GetUserByUserName(AppSettings.ConnectionString, username);

            // Check if username exists
            if (user == null) return null;

            // check if password is correct
            if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt)) return null;

            // authentication successful
            return user;
        }

        public User Create(User newUser, string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new AppException("Password is required");

            var user = DBUtility.GetUserByUserName(AppSettings.ConnectionString, newUser.UserName);

            if (user != null)
                throw new AppException("UserName \"" + newUser.UserName + "\" is already taken");

            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(password, out passwordHash, out passwordSalt);

            newUser.PasswordHash = passwordHash;
            newUser.PasswordSalt = passwordSalt;

            DBUtility.SaveUser(AppSettings.ConnectionString, newUser);

            return newUser;
        }

        public void Delete(int id)
        {
            //var user = _context.Users.Find(id);
            //if(user != null)
            //{
            //    _context.Users.Remove(user);
            //    _context.SaveChanges();
            //}
        }

        public IEnumerable<User> GetAll()
        {
            return new List<User>();
        }

        public User GetById(int id)
        {
            return DBUtility.GetUserById(AppSettings.ConnectionString, id);
        }

        public void Update(User userParam, string password = null)
        {
            var user = DBUtility.GetUserById(AppSettings.ConnectionString, userParam.Id);

            if (user == null)
                throw new AppException("User not found");

            if(userParam.UserName != user.UserName)
            {
                var updatedUser = DBUtility.GetUserByUserName(AppSettings.ConnectionString, userParam.UserName);
                // username has changed so check if the new username is already taken
                if (updatedUser != null)
                    throw new AppException("Username " + userParam.UserName + " is already taken");
            }

            user.FirstName = userParam.FirstName;
            user.LastName = userParam.LastName;
            user.UserName = userParam.UserName;

            // update password if it was entered
            if (!string.IsNullOrWhiteSpace(password)){
                byte[] passwordHash, passwordSalt;
                CreatePasswordHash(password, out passwordHash, out passwordSalt);

                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
            }

            DBUtility.SaveUser(AppSettings.ConnectionString, user);
        }

        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            if (password == null) throw new ArgumentException("password");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace", "password");

            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private static bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            if (password == null) throw new ArgumentException("password");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace", "password");
            if (storedHash.Length != 64) throw new ArgumentException("Invalid lenght of password hash (64 bytes expected)", "passwordHash");
            if (storedSalt.Length != 128) throw new ArgumentException("Invalid lenght of password salt (128 bytes expected)", "passwordSalt");

            using(var hmac = new System.Security.Cryptography.HMACSHA512(storedSalt))
            {
                var comptueHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for(int i = 0; i< comptueHash.Length; i++)
                {
                    if (comptueHash[i] != storedHash[i]) return false;
                }
            }
            return true;
        }
    }
}
