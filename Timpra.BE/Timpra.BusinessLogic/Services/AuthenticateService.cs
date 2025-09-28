using System;
using System.Linq;
using System.Security.Cryptography;
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
        private static readonly int SaltSize = 16;
        private static readonly int HashSize = 20;
        private static readonly int Iterations = 10000;

        public AuthenticateService(IRepository<User> userRepository, ITokenManager tokenManager)
        {
            _userRepository = userRepository;
            _tokenManager = tokenManager;
        }

        public async Task<TokenDTO> Login(LoginDTO loginModel)
        {
            var usersQuerayble = await _userRepository.GetAll();
            var usersList = await usersQuerayble.Include(t => t.Role).ToListAsync();

            var user = usersList.FirstOrDefault(x => x.UserName == loginModel.Username);

            if (user == null)
            {
                throw new NotFoundException("User was not found!");
            }


            if (!VerifyPassword(loginModel.Password, user.Password))
            {
                throw new IncorrectPasswordException("Password is incorrect!");
            }

            user.Token = _tokenManager.NewToken(user);
            var newAccessToken = user.Token;
            var newRefreshToken = _tokenManager.NewToken();
            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(1);

            await _userRepository.UpdateAsync(user, user.Id);

            return new TokenDTO
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
            };
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

            item.Password = HashPassword(item.Password);
            item.ConfirmedPassword = HashPassword(item.ConfirmedPassword);

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

        public static bool VerifyPassword(string password, string base64Hash)
        {
            var hashBytes = Convert.FromBase64String(base64Hash);

            var salt = new byte[SaltSize];
            Array.Copy(hashBytes, 0, salt, 0, SaltSize);

            var key = new Rfc2898DeriveBytes(password, salt, Iterations);
            byte[] hash = key.GetBytes(HashSize);

            for (var i = 0; i < HashSize; i++)
            {
                if (hashBytes[i + SaltSize] != hash[i])
                {
                    return false;
                }
            }

            return true;

        }

        public static string HashPassword(string password)
        {
            byte[] salt;
            var rng = RandomNumberGenerator.Create();
            rng.GetBytes(salt = new byte[SaltSize]);
            var key = new Rfc2898DeriveBytes(password, salt, Iterations);
            var hash = key.GetBytes(HashSize);

            var hashBytes = new byte[SaltSize + HashSize];
            Array.Copy(salt, 0, hashBytes, 0, SaltSize);
            Array.Copy(hash, 0, hashBytes, SaltSize, HashSize);

            var base64Hash = Convert.ToBase64String(hashBytes);
            return base64Hash;
        }

        public async Task<TokenDTO> Refresh(TokenDTO tokenApiDto)
        {
            if (string.IsNullOrEmpty(tokenApiDto.AccessToken) || string.IsNullOrEmpty(tokenApiDto.RefreshToken))
            {
                throw new NotFoundException("Tokens are missing or empty.");
            }

            if (tokenApiDto is null)
            {
                throw new InvalidRequestException("Invalid Client Request");
            }

            var usersQuerayble = await _userRepository.GetAll();
            var usersList = await usersQuerayble.ToListAsync();

            string AccessToken = tokenApiDto.AccessToken;
            string RefreshToken = tokenApiDto.RefreshToken;
            var principal = _tokenManager.VerifyToken(AccessToken);
            var username = principal.Identity.Name;
            var user = usersList.FirstOrDefault(u => u.UserName == username);
            if (user is null || user.RefreshToken != RefreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
            {
                throw new InvalidRequestException("Invalid Request");
            }
            var newAccessToken = _tokenManager.NewToken(user);
            var newRefreshToken = _tokenManager.NewToken();
            user.RefreshToken = newRefreshToken;
            await _userRepository.UpdateAsync(user, user.Id);
            return new TokenDTO()
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
            };
        }

    }
}