using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Timpra.BusinessLogic.DTOs.Orders;
using Timpra.BusinessLogic.Exceptions;
using Timpra.BusinessLogic.Helpers.TokenAuthentication;
using Timpra.BusinessLogic.Mappers;
using Timpra.BusinessLogic.Services.Abstractions;
using Timpra.DataAccess.Entities;
using Timpra.DataAccess.Repository.Abstraction;

namespace Timpra.BusinessLogic.Services
{
    public class AuthenticateService : IAuthenticateService
    {
        private readonly IRepository<User> _userRepository;
        private readonly ITokenManager _tokenManager;

        public AuthenticateService(IRepository<User> userRepository, ITokenManager tokenManager)
        {
            _userRepository = userRepository;
            _tokenManager = tokenManager;
        }

        public async Task<User> Login(LoginDTO loginModel)
        {
            var user = _tokenManager.Authenticate(loginModel.Username, loginModel.Password);
            return await Task.FromResult(user);
        }

        public async Task<UserDTO> Register(UserDTO item, bool applyChanges = true)
        {
            if (item == null)
            {
                throw new NotFoundException("User was not found!");
            }

            if (await CheckUserNameExistAsync(item.UserName))
            {
                throw new AlreadyExistsException("Username already exists!");
            }

            if (await CheckEmailExistAsync(item.Email))
            {
                throw new AlreadyExistsException("E-mail already exists!");
            }

            if (IsValidEmail(item.Email) == false)
            {
                throw new InvalidEmailException("E-mail has an incorrect structure!");
            }

            if (item.Password != item.ConfirmedPassword)
            {
                throw new NotMatchingPasswordsException("Passwords do not match!");
            }

            var pass = CheckPasswordStrength(item.Password);
            if (!string.IsNullOrEmpty(pass))
            {
                throw new IncorrectPasswordException(pass);
            }

            item.Password = TokenManager.HashPassword(item.Password);
            item.ConfirmedPassword = TokenManager.HashPassword(item.ConfirmedPassword);

            await _userRepository.AddAsync(item.MapFromDto());

            return item;

        }

        public async Task<bool> CheckUserNameExistAsync(string userName)
        {
            var usersQuerayble = await _userRepository.GetAll();
            var usersList = await usersQuerayble.ToListAsync();
            return usersList.Any(x => x.UserName == userName);
        }


        public async Task<bool> CheckEmailExistAsync(string email)
        {
            var usersQuerayble = await _userRepository.GetAll();
            var usersList = await usersQuerayble.ToListAsync();
            return usersList.Any(x => x.Email == email);
        }

        public string CheckPasswordStrength(string password)
        {
            StringBuilder sb = new StringBuilder();
            if (password.Length < 8)
            {
                sb.Append("Minimum password length should be 8!" + Environment.NewLine);
            }
            if (!(Regex.IsMatch(password, "[a-z]") && Regex.IsMatch(password, "[A-Z]") && Regex.IsMatch(password, "[0-9]")))
            {
                sb.Append("Password should be AlphaNumeric!" + Environment.NewLine);
            }
            if (!Regex.IsMatch(password, "[<,@,#,,%,(,{,},!,?]"))
            {
                sb.Append("Password should contain special characters " + Environment.NewLine);
            }

            return sb.ToString();
        }

        public bool IsValidEmail(string email)
        {
            var trimmedEmail = email.Trim();

            if (trimmedEmail.EndsWith("."))
            {
                return false;
            }
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == trimmedEmail;
            }
            catch
            {
                return false;
            }
        }
    }
}