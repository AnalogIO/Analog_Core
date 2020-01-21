using System.Collections.Generic;
using System.Security.Claims;
using CoffeeCard.Models;
using CoffeeCard.Models.DataTransferObjects.User;

namespace CoffeeCard.Services
{
    public interface IAccountService
    {
        User GetAccountByClaims(IEnumerable<Claim> claims);
        User GetAccountByEmail(string email);
        User GetUserById(int id);
        User RegisterAccount(RegisterDTO registerDto);
        string Login(string username, string password, string version);
        bool VerifyRegistration(string token);
        User UpdateAccount(IEnumerable<Claim> claims, UpdateUserDTO userDto);
        void UpdateExperience(int userId, int exp);
        void ForgotPassword(string email);
        bool RecoverUser(string token);
    }
}