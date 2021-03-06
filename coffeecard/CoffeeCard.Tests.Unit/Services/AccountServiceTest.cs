using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CoffeeCard.Common.Configuration;
using CoffeeCard.WebApi.Helpers;
using CoffeeCard.WebApi.Models;
using CoffeeCard.WebApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace CoffeeCard.Tests.Unit.Services
{
    public class AccountServiceTest
    {
        [Fact(DisplayName = "RecoverUser given malformed token returns false")]
        public async Task RecoverUserGivenMalformedTokenReturnsFalse()
        {
            // Arrange
            var builder = new DbContextOptionsBuilder<CoffeeCardContext>()
                .UseInMemoryDatabase(nameof(RecoverUserGivenMalformedTokenReturnsFalse));

            var databaseSettings = new DatabaseSettings
            {
                SchemaName = "test"
            };
            var environmentSettings = new EnvironmentSettings();
            var loginLimiterSettings = new LoginLimiterSettings()
            {
                IsEnabled = true, MaximumLoginAttemptsWithinTimeOut = 5, TimeOutPeriodInMinutes = 5
            };

            var expectedResult = false;
            bool result;

            // Act
            await using (var context = new CoffeeCardContext(builder.Options, databaseSettings))
            {
                var accountService = new AccountService(context, environmentSettings, new Mock<ITokenService>().Object,
                    new Mock<IEmailService>().Object, new Mock<IHashService>().Object,
                    new Mock<IHttpContextAccessor>().Object, new Mock<ILoginLimiter>().Object, loginLimiterSettings);
                result = await accountService.RecoverUser("bogus", "3433");
            }

            // Assert
            Assert.Equal(expectedResult, result);
        }

        [Fact(DisplayName = "RecoverUser given valid token returns true")]
        public async Task RecoverUserGivenValidTokenReturnsTrue()
        {
            // Arrange
            var builder = new DbContextOptionsBuilder<CoffeeCardContext>()
                .UseInMemoryDatabase(nameof(RecoverUserGivenValidTokenReturnsTrue));

            var databaseSettings = new DatabaseSettings
            {
                SchemaName = "test"
            };
            var environmentSettings = new EnvironmentSettings();
            var loginLimiterSettings = new LoginLimiterSettings()
            {
                IsEnabled = true, MaximumLoginAttemptsWithinTimeOut = 5, TimeOutPeriodInMinutes = 5
            };

            var expectedResult = true;
            bool result;

            var claim = new Claim(ClaimTypes.Email, "test@email.dk");
            var claims = new List<Claim> {claim};
            var validToken = new JwtSecurityToken("analog", "all", claims);

            var tokenService = new Mock<ITokenService>();
            tokenService.Setup(t => t.ReadToken("valid")).Returns(validToken);
            tokenService.Setup(t => t.ValidateToken("valid")).ReturnsAsync(true);

            // Act
            await using (var context = new CoffeeCardContext(builder.Options, databaseSettings))
            {
                var token = new Token("valid");
                var userTokens = new List<Token> {token};

                var user = new User {Tokens = userTokens, Email = "test@email.dk", Programme = new Programme()};
                context.Add(user);
                context.SaveChanges();

                var accountService = new AccountService(context, environmentSettings, tokenService.Object,
                    new Mock<IEmailService>().Object, new Mock<IHashService>().Object,
                    new Mock<IHttpContextAccessor>().Object, new Mock<ILoginLimiter>().Object, loginLimiterSettings);

                result = await accountService.RecoverUser("valid", "3433");
            }

            // Assert
            Assert.Equal(expectedResult, result);
        }

        [Fact(DisplayName = "RecoverUser given valid token updates password and resets users tokens")]
        public async Task RecoverUserGivenValidTokenUpdatesPasswordAndResetsUsersTokens()
        {
            // Arrange
            var builder = new DbContextOptionsBuilder<CoffeeCardContext>()
                .UseInMemoryDatabase(nameof(RecoverUserGivenValidTokenUpdatesPasswordAndResetsUsersTokens));

            var databaseSettings = new DatabaseSettings
            {
                SchemaName = "test"
            };
            var environmentSettings = new EnvironmentSettings();
            var loginLimiterSettings = new LoginLimiterSettings()
            {
                IsEnabled = true, MaximumLoginAttemptsWithinTimeOut = 5, TimeOutPeriodInMinutes = 5
            };

            var claim = new Claim(ClaimTypes.Email, "test@email.dk");
            var claims = new List<Claim> {claim};
            var validToken = new JwtSecurityToken("analog", "all", claims);

            var tokenService = new Mock<ITokenService>();
            tokenService.Setup(t => t.ReadToken("valid")).Returns(validToken);
            tokenService.Setup(t => t.ValidateToken("valid")).ReturnsAsync(true);

            var userPass = "not set";
            string newUserPass;
            ICollection<Token> newUserTokens;

            // Act
            await using (var context = new CoffeeCardContext(builder.Options, databaseSettings))
            {
                var token = new Token("valid");
                var userTokens = new List<Token> {token};

                var user = new User
                    {Tokens = userTokens, Email = "test@email.dk", Programme = new Programme(), Password = userPass};
                context.Add(user);
                context.SaveChanges();

                var accountService = new AccountService(context, environmentSettings, tokenService.Object,
                    new Mock<IEmailService>().Object, new Mock<IHashService>().Object,
                    new Mock<IHttpContextAccessor>().Object, new Mock<ILoginLimiter>().Object, loginLimiterSettings);

                await accountService.RecoverUser("valid", "3433");

                var updatedUser = context.Users.FirstOrDefault(u => u.Email == user.Email);
                newUserPass = updatedUser.Password;
                newUserTokens = updatedUser.Tokens;
            }

            var expectedTokenCount = 0;

            // Assert
            Assert.NotEqual(newUserPass, userPass);
            Assert.Equal(newUserTokens.Count, expectedTokenCount);
        }

        [Fact(DisplayName = "Login given valid credentials returns token")]
        public async Task LoginGivenValidCredentialsReturnsToken()
        {
            // Arrange
            var builder = new DbContextOptionsBuilder<CoffeeCardContext>()
                .UseInMemoryDatabase(nameof(LoginGivenValidCredentialsReturnsToken));

            var databaseSettings = new DatabaseSettings()
            {
                SchemaName = "test"
            };
            var environmentSettings = new EnvironmentSettings()
                {DeploymentUrl = "test", EnvironmentType = EnvironmentType.Test, MinAppVersion = "2.1.0"};
            var loginLimiterSettings = new LoginLimiterSettings()
            {
                IsEnabled = true, MaximumLoginAttemptsWithinTimeOut = 5, TimeOutPeriodInMinutes = 5
            };

            var userTokens = new List<Token>();
            var user = new User
            {
                Id = 1, Name = "test", Tokens = userTokens, Email = "test@email.dk", Programme = new Programme(),
                Password = "test", IsVerified = true
            };

            var hasher = new Mock<IHashService>();
            hasher.Setup(h => h.Hash(user.Password)).Returns(user.Password);

            var expectedToken = "valid";
            string actualToken;

            var tokenService = new Mock<ITokenService>();
            tokenService.Setup(t => t.GenerateToken(It.IsAny<IEnumerable<Claim>>())).Returns(expectedToken);

            // Act
            await using (var context = new CoffeeCardContext(builder.Options, databaseSettings))
            {
                context.Add(user);
                context.SaveChanges();

                var accountService = new AccountService(context, environmentSettings, tokenService.Object,
                    new Mock<IEmailService>().Object, hasher.Object,
                    new Mock<IHttpContextAccessor>().Object, new LoginLimiter(loginLimiterSettings), loginLimiterSettings);

                actualToken = accountService.Login(user.Email, user.Password, "2.1.0");
            }

            // Assert
            Assert.Equal(expectedToken, actualToken);
        }

        [Fact(DisplayName = "LoginLimiter is called if limiter is enabled")]
        public async Task LoginRejectsAfterFiveFailedLogins()
        {
            // Arrange
            var builder = new DbContextOptionsBuilder<CoffeeCardContext>()
                .UseInMemoryDatabase(nameof(LoginRejectsAfterFiveFailedLogins));

            var databaseSettings = new DatabaseSettings()
            {
                SchemaName = "test"
            };
            var environmentSettings = new EnvironmentSettings()
                {DeploymentUrl = "test", EnvironmentType = EnvironmentType.Test, MinAppVersion = "2.1.0"};
            var loginLimiterSettings = new LoginLimiterSettings()
            {
                IsEnabled = true, MaximumLoginAttemptsWithinTimeOut = 5, TimeOutPeriodInMinutes = 5
            };

            var userTokens = new List<Token>();
            var user = new User
            {
                Id = 1, Name = "test", Tokens = userTokens, Email = "test@email.dk", Programme = new Programme(),
                Password = "test", IsVerified = true
            };

            var wrongPass = "wrongPassword";

            var httpContextAccessor = new Mock<IHttpContextAccessor>();
            httpContextAccessor.Setup(h => h.HttpContext).Returns(new DefaultHttpContext().HttpContext);
            
            var loginLimiter = new Mock<ILoginLimiter>();
            loginLimiter.Setup(l => l.LoginAllowed(user)).Returns(true);

            // Act
            await using (var context = new CoffeeCardContext(builder.Options, databaseSettings))
            {
                context.Add(user);
                context.SaveChanges();

                var accountService = new AccountService(context, environmentSettings, new Mock<ITokenService>().Object,
                    new Mock<IEmailService>().Object, new HashService(), httpContextAccessor.Object,
                    loginLimiter.Object, loginLimiterSettings);

                //Attempts to login
                Assert.Throws<ApiException>(() => accountService.Login(user.Email, wrongPass, "2.1.0"));
                
                // Assert
                loginLimiter.Verify(l => l.LoginAllowed(user), Times.Once);

            }
        }
        
        [Fact(DisplayName = "LoginLimiter not called if limiter is disabled")]
        public async Task LoginLimiterNotCalledWhenDisabled()
        {
            // Arrange
            var builder = new DbContextOptionsBuilder<CoffeeCardContext>()
                .UseInMemoryDatabase(nameof(LoginLimiterNotCalledWhenDisabled));

            var databaseSettings = new DatabaseSettings()
            {
                SchemaName = "test"
            };
            var environmentSettings = new EnvironmentSettings()
                {DeploymentUrl = "test", EnvironmentType = EnvironmentType.Test, MinAppVersion = "2.1.0"};
            var loginLimiterSettings = new LoginLimiterSettings()
            {
                IsEnabled = false, MaximumLoginAttemptsWithinTimeOut = 5, TimeOutPeriodInMinutes = 5
            };

            var userTokens = new List<Token>();
            var user = new User
            {
                Id = 1, Name = "test", Tokens = userTokens, Email = "test@email.dk", Programme = new Programme(),
                Password = "test", IsVerified = true
            };

            var wrongPass = "wrongPassword";

            var httpContextAccessor = new Mock<IHttpContextAccessor>();
            httpContextAccessor.Setup(h => h.HttpContext).Returns(new DefaultHttpContext().HttpContext);
            
            var loginLimiter = new Mock<ILoginLimiter>();
            loginLimiter.Setup(l => l.LoginAllowed(user)).Returns(true);

            // Act
            await using (var context = new CoffeeCardContext(builder.Options, databaseSettings))
            {
                context.Add(user);
                context.SaveChanges();

                var accountService = new AccountService(context, environmentSettings, new Mock<ITokenService>().Object,
                    new Mock<IEmailService>().Object, new HashService(), httpContextAccessor.Object,
                    loginLimiter.Object, loginLimiterSettings);

                //Attempts to login
                Assert.Throws<ApiException>(() => accountService.Login(user.Email, wrongPass, "2.1.0"));
                
                // Assert
                loginLimiter.Verify(l => l.LoginAllowed(user), Times.Never);
            }
        }
        
        [Fact(DisplayName = "Login throws exception when limit is reached and limiter is enabled")]
        public async Task LoginThrowsExceptionWhenLimitIsReached()
        {
            // Arrange
            var builder = new DbContextOptionsBuilder<CoffeeCardContext>()
                .UseInMemoryDatabase(nameof(LoginThrowsExceptionWhenLimitIsReached));

            var databaseSettings = new DatabaseSettings()
            {
                SchemaName = "test"
            };
            var environmentSettings = new EnvironmentSettings()
                {DeploymentUrl = "test", EnvironmentType = EnvironmentType.Test, MinAppVersion = "2.1.0"};
            var loginLimiterSettings = new LoginLimiterSettings()
            {
                IsEnabled = true, MaximumLoginAttemptsWithinTimeOut = 1, TimeOutPeriodInMinutes = 1
            };

            var userTokens = new List<Token>();
            var user = new User
            {
                Id = 1, Name = "test", Tokens = userTokens, Email = "test@email.dk", Programme = new Programme(),
                Password = "test", IsVerified = true
            };

            var wrongPass = "wrongPassword";

            var httpContextAccessor = new Mock<IHttpContextAccessor>();
            httpContextAccessor.Setup(h => h.HttpContext).Returns(new DefaultHttpContext().HttpContext);

            // Act
            await using (var context = new CoffeeCardContext(builder.Options, databaseSettings))
            {
                context.Add(user);
                context.SaveChanges();

                var accountService = new AccountService(context, environmentSettings, new Mock<ITokenService>().Object,
                    new Mock<IEmailService>().Object, new HashService(), httpContextAccessor.Object,
                    new LoginLimiter(loginLimiterSettings), loginLimiterSettings);

                //Attempts to login with the wrong credentials 
                Assert.Throws<ApiException>(() => accountService.Login(user.Email, wrongPass, "2.1.0"));

                //Attempts to login a sixth time with the same credentials and captures the exception 
                var tooManyLoginsException =
                    (ApiException) Record.Exception(() => accountService.Login(user.Email, wrongPass, "2.1.0"));
                // Assert
                var expectedException =
                    new ApiException(
                        $"Amount of failed login attempts exceeds the allowed amount, please wait for {loginLimiterSettings.TimeOutPeriodInMinutes} minutes before trying again",
                        429);
                Assert.Equal(tooManyLoginsException.StatusCode, expectedException.StatusCode);
                Assert.Equal(tooManyLoginsException.Message, expectedException.Message);
            }
        }
    }
}