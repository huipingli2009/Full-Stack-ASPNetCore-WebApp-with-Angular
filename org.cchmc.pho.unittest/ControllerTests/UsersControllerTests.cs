using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using org.cchmc.pho.api.Controllers;
using org.cchmc.pho.api.Mappings;
using org.cchmc.pho.api.ViewModels;
using org.cchmc.pho.core.DataModels;
using org.cchmc.pho.core.Interfaces;
using org.cchmc.pho.core.Models;
using org.cchmc.pho.identity.Interfaces;
using org.cchmc.pho.identity.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;

namespace org.cchmc.pho.unittest.ControllerTests
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class UsersControllerTests
    {
        private UsersController _userController;
        private Mock<ILogger<UsersController>> _mockLogger;
        private Mock<IUserService> _mockUserService;
        private Mock<IOptions<CustomOptions>> _mockOptions;
        private IMapper _mapper;
        private Mock<IStaff> _mockStaff;

        private const string _userName = "cjenkinson";
        private const string _password = "P@ssw0rd!";
        private const int _userId = 7;
        private CustomOptions _customOptions = new CustomOptions()
        {
            MaximumAttemptsBeforeLockout = 3,
            MinimumPasswordLength = 8,
            RequireDigit = true,
            RequireLowercase = true,
            RequireNonAlphaNumeric = true,
            RequireUppercase = true
        };
        private User _user;
        private List<Role> _roles = new List<Role>()
        {
            new Role()
            {
                Id = 1,
                Name = "Practice Member"
            },
            new Role()
            {
                Id = 2,
                Name = "Practice Admin"
            },
            new Role()
            {
                Id = 3,
                Name = "PHO Member"
            },
            new Role()
            {
                Id = 4,
                Name = "PHO Admin"
            }
        };
        
        [TestInitialize]
        public void Initialize()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddMaps(Assembly.GetExecutingAssembly());
                cfg.AddMaps(Assembly.GetAssembly(typeof(AlertMappings)));
            });
            _mapper = config.CreateMapper();
            _mockUserService = new Mock<IUserService>();
            _mockStaff = new Mock<IStaff>();
            _mockLogger = new Mock<ILogger<UsersController>>();
            _mockOptions = new Mock<IOptions<CustomOptions>>();
            _mockOptions.Setup(op => op.Value).Returns(_customOptions);

            _userController = new UsersController(_mockLogger.Object, _mapper, _mockUserService.Object, _mockOptions.Object, _mockStaff.Object);
            _user = new User()
            {
                CreatedBy = "system",
                CreatedDate = DateTime.Parse("1/1/20"),
                Email = "email@example.com",
                FirstName = "First",
                Id = _userId,
                LastName = "Last",
                LastUpdatedBy = "system",
                LastUpdatedDate = DateTime.Parse("1/2/20"),
                Role = new Role() { Id = 1, Name = "Practice Member" },
                StaffId = 5,
                UserName = _userName,
                Token = Guid.NewGuid().ToString() // let's not worry about the token contents just yet
            };
        }

        #region Authenticate Tests
        [TestMethod]
        public async Task Authenticate_Success_ReturnsToken()
        {
            _mockUserService.Setup(p => p.Authenticate(_userName, _password)).Returns(Task.FromResult(_user)).Verifiable();

            var result = await _userController.Authenticate(new AuthenticationRequest() { Password = _password, Username = _userName }) as ObjectResult;

            // assert
            Assert.IsNotNull(result);
            var authResult = result.Value as AuthenticationResult;
            Assert.AreEqual("Authorized", authResult.Status);
            Assert.IsNotNull(authResult.User);
            Assert.AreEqual(_user.Token, authResult.User.Token);
            _mockUserService.VerifyAll(); _mockStaff.VerifyAll();
        }

        [TestMethod]
        public async Task Authenticate_NullResult_Returns401()
        {
            _mockUserService.Setup(p => p.Authenticate(_userName, _password)).Returns(Task.FromResult((User)null)).Verifiable();

            var result = await _userController.Authenticate(new AuthenticationRequest() { Password = _password, Username = _userName }) as ObjectResult;

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(401, result.StatusCode);
            var authResult = result.Value as AuthenticationResult;
            Assert.AreEqual("User not found or password did not match", authResult.Status);
            Assert.IsNull(authResult.User);
            _mockUserService.VerifyAll(); _mockStaff.VerifyAll();
        }

        [TestMethod]
        public async Task Authenticate_ThrowsException_Returns500()
        {
            _mockUserService.Setup(p => p.Authenticate(_userName, _password)).Throws(new Exception()).Verifiable();

            var result = await _userController.Authenticate(new AuthenticationRequest() { Password = _password, Username = _userName }) as ObjectResult;

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(500, result.StatusCode);
            _mockUserService.VerifyAll(); _mockStaff.VerifyAll();
        }
        #endregion

        #region UpdateUserPassword Tests
        [TestMethod]
        public async Task UpdateUserPassword_UpdateSelf_Success_ReturnsTrue()
        {
            string myUserName = _userName;
            string myUserRole = "Practice Member";
            string newPassword = "Th1sP@sses!";
            _mockUserService.Setup(p => p.GetUserNameFromClaims(It.IsAny<IEnumerable<Claim>>())).Returns(myUserName).Verifiable();
            _mockUserService.Setup(p => p.GetRoleNameFromClaims(It.IsAny<IEnumerable<Claim>>())).Returns(myUserRole).Verifiable();
            _mockUserService.Setup(p => p.GetUser(_userId)).Returns(Task.FromResult(_user)).Verifiable();
            _mockUserService.Setup(p => p.ResetUserPassword(_userId, newPassword, myUserName)).Returns(Task.FromResult(true)).Verifiable();

            var result = await _userController.UpdateUserPassword(_userId, new PasswordChangeViewModel() { NewPassword = newPassword }) as ObjectResult;

            Assert.IsNotNull(result);
            Assert.IsTrue((bool)result.Value);
            _mockUserService.VerifyAll(); _mockStaff.VerifyAll();
        }

        [TestMethod]
        public async Task UpdateUserPassword_UpdateOther_Success_ReturnsTrue()
        {
            string myUserName = "SomeOtherName";
            string myUserRole = "PHO Admin";
            string newPassword = "Th1sP@sses!";
            _mockUserService.Setup(p => p.GetUserNameFromClaims(It.IsAny<IEnumerable<Claim>>())).Returns(myUserName).Verifiable();
            _mockUserService.Setup(p => p.GetRoleNameFromClaims(It.IsAny<IEnumerable<Claim>>())).Returns(myUserRole).Verifiable();
            _mockUserService.Setup(p => p.GetUser(_userId)).Returns(Task.FromResult(_user)).Verifiable();
            _mockUserService.Setup(p => p.ResetUserPassword(_userId, newPassword, myUserName)).Returns(Task.FromResult(true)).Verifiable();

            var result = await _userController.UpdateUserPassword(_userId, new PasswordChangeViewModel() { NewPassword = newPassword }) as ObjectResult;

            Assert.IsNotNull(result);
            Assert.IsTrue((bool)result.Value);
            _mockUserService.VerifyAll(); _mockStaff.VerifyAll();
        }

        [TestMethod]
        public async Task UpdateUserPassword_UpdateSelf_Fails_ReturnsFalse()
        {
            string myUserName = _userName;
            string myUserRole = "Practice Member";
            string newPassword = "Th1sP@sses!";
            _mockUserService.Setup(p => p.GetUserNameFromClaims(It.IsAny<IEnumerable<Claim>>())).Returns(myUserName).Verifiable();
            _mockUserService.Setup(p => p.GetRoleNameFromClaims(It.IsAny<IEnumerable<Claim>>())).Returns(myUserRole).Verifiable();
            _mockUserService.Setup(p => p.GetUser(_userId)).Returns(Task.FromResult(_user)).Verifiable();
            _mockUserService.Setup(p => p.ResetUserPassword(_userId, newPassword, myUserName)).Returns(Task.FromResult(false)).Verifiable();

            var result = await _userController.UpdateUserPassword(_userId, new PasswordChangeViewModel() { NewPassword = newPassword }) as ObjectResult;

            Assert.IsNotNull(result);
            Assert.IsFalse((bool)result.Value);
            _mockUserService.VerifyAll(); _mockStaff.VerifyAll();
        }

        [TestMethod]
        public async Task UpdateUserPassword_ViewModelNull_Returns400()
        {
            var result = await _userController.UpdateUserPassword(_userId, null) as ObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(400, result.StatusCode);
            _mockUserService.Verify(p => p.ResetUserPassword(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _mockUserService.VerifyAll(); _mockStaff.VerifyAll();
        }

        [TestMethod]
        public async Task UpdateUserPassword_PasswordNull_Returns400()
        {
            var result = await _userController.UpdateUserPassword(_userId, new PasswordChangeViewModel() { NewPassword = null }) as ObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(400, result.StatusCode);
            _mockUserService.Verify(p => p.ResetUserPassword(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _mockUserService.VerifyAll(); _mockStaff.VerifyAll();
        }

        [TestMethod]
        public async Task UpdateUserPassword_PasswordEmpty_Returns400()
        {
            var result = await _userController.UpdateUserPassword(_userId, new PasswordChangeViewModel() { NewPassword = "" }) as ObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(400, result.StatusCode);
            _mockUserService.Verify(p => p.ResetUserPassword(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _mockUserService.VerifyAll(); _mockStaff.VerifyAll();
        }

        [TestMethod]
        public async Task UpdateUserPassword_UserDoesNotExist_Returns400()
        {
            string newPassword = "Th1sP@sses!";
            _mockUserService.Setup(p => p.GetUser(_userId)).Returns(Task.FromResult((User)null)).Verifiable();

            var result = await _userController.UpdateUserPassword(_userId, new PasswordChangeViewModel() { NewPassword = newPassword }) as ObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(400, result.StatusCode);
            _mockUserService.Verify(p => p.ResetUserPassword(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _mockUserService.VerifyAll(); _mockStaff.VerifyAll();
        }

        [TestMethod]
        public async Task UpdateUserPassword_CannotUpdateAnotherUser_Returns400()
        {
            string myUserName = "SomeOtherName";
            string myUserRole = "Practice Member";
            string newPassword = "Th1sP@sses!";
            _mockUserService.Setup(p => p.GetUserNameFromClaims(It.IsAny<IEnumerable<Claim>>())).Returns(myUserName).Verifiable();
            _mockUserService.Setup(p => p.GetRoleNameFromClaims(It.IsAny<IEnumerable<Claim>>())).Returns(myUserRole).Verifiable();
            _mockUserService.Setup(p => p.GetUser(_userId)).Returns(Task.FromResult(_user)).Verifiable();

            var result = await _userController.UpdateUserPassword(_userId, new PasswordChangeViewModel() { NewPassword = newPassword }) as ObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(400, result.StatusCode);
            _mockUserService.Verify(p => p.ResetUserPassword(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _mockUserService.VerifyAll(); _mockStaff.VerifyAll();
        }

        [TestMethod]
        public async Task UpdateUserPassword_CannotUpdateAnotherUserInADifferentPractice_Returns400()
        {
            string myUserName = "SomeOtherName";
            string myUserRole = "Practice Admin";
            string newPassword = "Th1sP@sses!";
            int myUserId = 7;
            _mockUserService.Setup(p => p.GetUserNameFromClaims(It.IsAny<IEnumerable<Claim>>())).Returns(myUserName).Verifiable();
            _mockUserService.Setup(p => p.GetRoleNameFromClaims(It.IsAny<IEnumerable<Claim>>())).Returns(myUserRole).Verifiable();
            _mockUserService.Setup(p => p.GetUserIdFromClaims(It.IsAny<IEnumerable<Claim>>())).Returns(myUserId).Verifiable();
            _mockStaff.Setup(p => p.IsStaffInSamePractice(myUserId, _user.StaffId)).Returns(false).Verifiable();
            _mockUserService.Setup(p => p.GetUser(_userId)).Returns(Task.FromResult(_user)).Verifiable();
            
            var result = await _userController.UpdateUserPassword(_userId, new PasswordChangeViewModel() { NewPassword = newPassword }) as ObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(400, result.StatusCode);
            _mockUserService.Verify(p => p.ResetUserPassword(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _mockUserService.VerifyAll(); _mockStaff.VerifyAll();
        }

        [TestMethod]
        public async Task UpdateUserPassword_CanUpdateAnotherUserInSamePractice_Success()
        {
            string myUserName = "SomeOtherName";
            string myUserRole = "Practice Admin";
            string newPassword = "Th1sP@sses!";
            int myUserId = 7;
            _mockUserService.Setup(p => p.GetUserNameFromClaims(It.IsAny<IEnumerable<Claim>>())).Returns(myUserName).Verifiable();
            _mockUserService.Setup(p => p.GetRoleNameFromClaims(It.IsAny<IEnumerable<Claim>>())).Returns(myUserRole).Verifiable();
            _mockUserService.Setup(p => p.GetUser(_userId)).Returns(Task.FromResult(_user)).Verifiable();
            _mockUserService.Setup(p => p.GetUserIdFromClaims(It.IsAny<IEnumerable<Claim>>())).Returns(myUserId).Verifiable();
            _mockStaff.Setup(p => p.IsStaffInSamePractice(myUserId, _user.StaffId)).Returns(true).Verifiable();
            _mockUserService.Setup(p => p.ResetUserPassword(_userId, newPassword, myUserName)).Returns(Task.FromResult(true)).Verifiable();

            var result = await _userController.UpdateUserPassword(_userId, new PasswordChangeViewModel() { NewPassword = newPassword }) as ObjectResult;

            Assert.IsNotNull(result);
            Assert.IsTrue((bool)result.Value);
            _mockUserService.VerifyAll(); _mockStaff.VerifyAll();
        }

        [TestMethod]
        public async Task UpdateUserPassword_PasswordTooShort_Returns400()
        {
            string myUserName = _userName;
            string myUserRole = "Practice Member";
            string newPassword = "Th1sP@s";
            _mockUserService.Setup(p => p.GetUserNameFromClaims(It.IsAny<IEnumerable<Claim>>())).Returns(myUserName).Verifiable();
            _mockUserService.Setup(p => p.GetRoleNameFromClaims(It.IsAny<IEnumerable<Claim>>())).Returns(myUserRole).Verifiable();
            _mockUserService.Setup(p => p.GetUser(_userId)).Returns(Task.FromResult(_user)).Verifiable();

            var result = await _userController.UpdateUserPassword(_userId, new PasswordChangeViewModel() { NewPassword = newPassword }) as ObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(400, result.StatusCode);
            Assert.IsTrue(result.Value.ToString().Contains($"Password must be at least {_customOptions.MinimumPasswordLength} characters"));
            _mockUserService.Verify(p => p.ResetUserPassword(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _mockUserService.VerifyAll(); _mockStaff.VerifyAll();
        }

        [TestMethod]
        public async Task UpdateUserPassword_PasswordMissingDigit_Returns400()
        {
            string myUserName = _userName;
            string myUserRole = "Practice Member";
            string newPassword = "ThisP@ss";
            _mockUserService.Setup(p => p.GetUserNameFromClaims(It.IsAny<IEnumerable<Claim>>())).Returns(myUserName).Verifiable();
            _mockUserService.Setup(p => p.GetRoleNameFromClaims(It.IsAny<IEnumerable<Claim>>())).Returns(myUserRole).Verifiable();
            _mockUserService.Setup(p => p.GetUser(_userId)).Returns(Task.FromResult(_user)).Verifiable();

            var result = await _userController.UpdateUserPassword(_userId, new PasswordChangeViewModel() { NewPassword = newPassword }) as ObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(400, result.StatusCode);
            Assert.IsTrue(result.Value.ToString().Contains("Password must contain a digit"));
            _mockUserService.Verify(p => p.ResetUserPassword(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _mockUserService.VerifyAll(); _mockStaff.VerifyAll();
        }

        [TestMethod]
        public async Task UpdateUserPassword_PasswordMissingLowercase_Returns400()
        {
            string myUserName = _userName;
            string myUserRole = "Practice Member";
            string newPassword = "TH1SP@SS";
            _mockUserService.Setup(p => p.GetUserNameFromClaims(It.IsAny<IEnumerable<Claim>>())).Returns(myUserName).Verifiable();
            _mockUserService.Setup(p => p.GetRoleNameFromClaims(It.IsAny<IEnumerable<Claim>>())).Returns(myUserRole).Verifiable();
            _mockUserService.Setup(p => p.GetUser(_userId)).Returns(Task.FromResult(_user)).Verifiable();

            var result = await _userController.UpdateUserPassword(_userId, new PasswordChangeViewModel() { NewPassword = newPassword }) as ObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(400, result.StatusCode);
            Assert.IsTrue(result.Value.ToString().Contains("Password must contain a lowercase character"));
            _mockUserService.Verify(p => p.ResetUserPassword(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _mockUserService.VerifyAll(); _mockStaff.VerifyAll();
        }

        [TestMethod]
        public async Task UpdateUserPassword_PasswordMissingUppercase_Returns400()
        {
            string myUserName = _userName;
            string myUserRole = "Practice Member";
            string newPassword = "th1sp@ss";
            _mockUserService.Setup(p => p.GetUserNameFromClaims(It.IsAny<IEnumerable<Claim>>())).Returns(myUserName).Verifiable();
            _mockUserService.Setup(p => p.GetRoleNameFromClaims(It.IsAny<IEnumerable<Claim>>())).Returns(myUserRole).Verifiable();
            _mockUserService.Setup(p => p.GetUser(_userId)).Returns(Task.FromResult(_user)).Verifiable();

            var result = await _userController.UpdateUserPassword(_userId, new PasswordChangeViewModel() { NewPassword = newPassword }) as ObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(400, result.StatusCode);
            Assert.IsTrue(result.Value.ToString().Contains("Password must contain an uppercase character"));
            _mockUserService.Verify(p => p.ResetUserPassword(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _mockUserService.VerifyAll(); _mockStaff.VerifyAll();
        }

        [TestMethod]
        public async Task UpdateUserPassword_PasswordMissingSpecial_Returns400()
        {
            string myUserName = _userName;
            string myUserRole = "Practice Member";
            string newPassword = "ThisNoPass1";
            _mockUserService.Setup(p => p.GetUserNameFromClaims(It.IsAny<IEnumerable<Claim>>())).Returns(myUserName).Verifiable();
            _mockUserService.Setup(p => p.GetRoleNameFromClaims(It.IsAny<IEnumerable<Claim>>())).Returns(myUserRole).Verifiable();
            _mockUserService.Setup(p => p.GetUser(_userId)).Returns(Task.FromResult(_user)).Verifiable();

            var result = await _userController.UpdateUserPassword(_userId, new PasswordChangeViewModel() { NewPassword = newPassword }) as ObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(400, result.StatusCode);
            Assert.IsTrue(result.Value.ToString().Contains("Password must contain a special character"));
            _mockUserService.Verify(p => p.ResetUserPassword(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _mockUserService.VerifyAll(); _mockStaff.VerifyAll();
        }

        [TestMethod]
        public async Task UpdateUserPassword_PasswordHasSpace_Returns400()
        {
            string myUserName = _userName;
            string myUserRole = "Practice Member";
            string newPassword = "th1sp@s ";
            _mockUserService.Setup(p => p.GetUserNameFromClaims(It.IsAny<IEnumerable<Claim>>())).Returns(myUserName).Verifiable();
            _mockUserService.Setup(p => p.GetRoleNameFromClaims(It.IsAny<IEnumerable<Claim>>())).Returns(myUserRole).Verifiable();
            _mockUserService.Setup(p => p.GetUser(_userId)).Returns(Task.FromResult(_user)).Verifiable();

            var result = await _userController.UpdateUserPassword(_userId, new PasswordChangeViewModel() { NewPassword = newPassword }) as ObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(400, result.StatusCode);
            Assert.IsTrue(result.Value.ToString().Contains("Passwords cannot contain spaces"));
            _mockUserService.Verify(p => p.ResetUserPassword(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _mockUserService.VerifyAll(); _mockStaff.VerifyAll();
        }

        [TestMethod]
        public async Task UpdateUserPassword_ThrowsException_Returns500()
        {
            string newPassword = "Th1sP@sses!";
            _mockUserService.Setup(p => p.GetUser(_userId)).Throws(new Exception()).Verifiable();

            var result = await _userController.UpdateUserPassword(_userId, new PasswordChangeViewModel() { NewPassword = newPassword }) as ObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(500, result.StatusCode);
            _mockUserService.Verify(p => p.ResetUserPassword(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _mockUserService.VerifyAll(); _mockStaff.VerifyAll();
        }
        #endregion

        #region UpdateUser Tests
        [TestMethod]
        public async Task UpdateUser_UpdateSelf_Success()
        {
            UserViewModel newUser = new UserViewModel()
            {
                Email = "something@example.com",
                Id = _user.Id,
                Role = new RoleViewModel() { Id = _user.Role.Id, Name = _user.Role.Name }
            };
            string userMakingChange = _userName;
            string roleMakingChange = _user.Role.Name;
            _mockUserService.Setup(p => p.GetUser(_userId)).Returns(Task.FromResult(_user)).Verifiable();
            _mockUserService.Setup(p => p.ListRoles()).Returns(Task.FromResult(_roles)).Verifiable();
            _mockUserService.Setup(p => p.GetUserNameFromClaims(It.IsAny<IEnumerable<Claim>>())).Returns(userMakingChange).Verifiable();
            _mockUserService.Setup(p => p.GetRoleNameFromClaims(It.IsAny<IEnumerable<Claim>>())).Returns(roleMakingChange).Verifiable();
            _mockUserService.Setup(p => p.UpdateUser(It.IsAny<User>(), userMakingChange))
                .Returns<User, string>((x,y) => {
                    x.LastUpdatedBy = y;
                    x.LastUpdatedDate = DateTime.Now.Date;
                    return Task.FromResult(x);
                }).Verifiable();

            var result = await _userController.UpdateUser(_userId, newUser) as ObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            var resultUser = result.Value as UserViewModel;
            Assert.AreEqual(userMakingChange, resultUser.LastUpdatedBy);
            Assert.AreEqual(newUser.Email, resultUser.Email);
            _mockUserService.VerifyAll(); _mockStaff.VerifyAll();
        }

        [TestMethod]
        public async Task UpdateUser_UpdateOtherUser_Success()
        {
            UserViewModel newUser = new UserViewModel()
            {
                Email = "something@example.com",
                Id = _user.Id,
                Role = new RoleViewModel() { Id = _user.Role.Id, Name = _user.Role.Name }
            };
            string userMakingChange = "PHO Admin";
            string roleMakingChange = "PHO Admin";
            _mockUserService.Setup(p => p.GetUser(_userId)).Returns(Task.FromResult(_user)).Verifiable();
            _mockUserService.Setup(p => p.ListRoles()).Returns(Task.FromResult(_roles)).Verifiable();
            _mockUserService.Setup(p => p.GetUserNameFromClaims(It.IsAny<IEnumerable<Claim>>())).Returns(userMakingChange).Verifiable();
            _mockUserService.Setup(p => p.GetRoleNameFromClaims(It.IsAny<IEnumerable<Claim>>())).Returns(roleMakingChange).Verifiable();
            _mockUserService.Setup(p => p.UpdateUser(It.IsAny<User>(), userMakingChange))
                .Returns<User, string>((x, y) => {
                    x.LastUpdatedBy = y;
                    x.LastUpdatedDate = DateTime.Now.Date;
                    return Task.FromResult(x);
                }).Verifiable();

            var result = await _userController.UpdateUser(_userId, newUser) as ObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            var resultUser = result.Value as UserViewModel;
            Assert.AreEqual(userMakingChange, resultUser.LastUpdatedBy);
            Assert.AreEqual(newUser.Email, resultUser.Email);
            _mockUserService.VerifyAll(); _mockStaff.VerifyAll();
        }

        [TestMethod]
        public async Task UpdateUser_UserObjectNull_Returns400()
        {
            var result = await _userController.UpdateUser(_userId, null) as ObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(400, result.StatusCode);
            _mockUserService.Verify(p => p.UpdateUser(It.IsAny<User>(), It.IsAny<string>()), Times.Never);
            _mockUserService.VerifyAll(); _mockStaff.VerifyAll();
        }

        [TestMethod]
        public async Task UpdateUser_UserIdMismatch_Returns400()
        {
            UserViewModel newUser = new UserViewModel()
            {
                Email = "something@example.com",
                Id = _userId - 1,
                Role = new RoleViewModel() { Id = _user.Role.Id, Name = _user.Role.Name }
            };

            var result = await _userController.UpdateUser(_userId, newUser) as ObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(400, result.StatusCode);
            _mockUserService.Verify(p => p.UpdateUser(It.IsAny<User>(), It.IsAny<string>()), Times.Never);
            _mockUserService.VerifyAll(); _mockStaff.VerifyAll();
        }

        [TestMethod]
        public async Task UpdateUser_UserDoesNotExist_Returns400()
        {
            UserViewModel newUser = new UserViewModel()
            {
                Email = "something@example.com",
                Id = _userId,
                Role = new RoleViewModel() { Id = _user.Role.Id, Name = _user.Role.Name }
            };
            _mockUserService.Setup(p => p.GetUserNameFromClaims(It.IsAny<IEnumerable<Claim>>())).Returns("ASDF").Verifiable();
            _mockUserService.Setup(p => p.GetUser(_userId)).Returns(Task.FromResult((User)null)).Verifiable();

            var result = await _userController.UpdateUser(_userId, newUser) as ObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(400, result.StatusCode);
            _mockUserService.Verify(p => p.UpdateUser(It.IsAny<User>(), It.IsAny<string>()), Times.Never);
            _mockUserService.VerifyAll(); _mockStaff.VerifyAll();
        }

        [TestMethod]
        public async Task UpdateUser_SpecifiedNewRoleDoesNotExist_Returns400()
        {
            UserViewModel newUser = new UserViewModel()
            {
                Email = "something@example.com",
                Id = _user.Id,
                Role = new RoleViewModel() { Id = 8, Name = _user.Role.Name }
            };
            _mockUserService.Setup(p => p.GetUser(_userId)).Returns(Task.FromResult(_user)).Verifiable();
            _mockUserService.Setup(p => p.ListRoles()).Returns(Task.FromResult(_roles)).Verifiable();

            var result = await _userController.UpdateUser(_userId, newUser) as ObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(400, result.StatusCode);
            _mockUserService.Verify(p => p.UpdateUser(It.IsAny<User>(), It.IsAny<string>()), Times.Never);
            _mockUserService.VerifyAll(); _mockStaff.VerifyAll();
        }

        [TestMethod]
        public async Task UpdateUser_PracticeMemberCantUpdateAnotherUser_Returns400()
        {
            UserViewModel newUser = new UserViewModel()
            {
                Email = "something@example.com",
                Id = _user.Id,
                Role = new RoleViewModel() { Id = _user.Role.Id, Name = _user.Role.Name }
            };
            string userMakingChange = "SomeoneElse";
            string roleMakingChange = "Practice Member";
            _mockUserService.Setup(p => p.GetUser(_userId)).Returns(Task.FromResult(_user)).Verifiable();
            _mockUserService.Setup(p => p.ListRoles()).Returns(Task.FromResult(_roles)).Verifiable();
            _mockUserService.Setup(p => p.GetUserNameFromClaims(It.IsAny<IEnumerable<Claim>>())).Returns(userMakingChange).Verifiable();
            _mockUserService.Setup(p => p.GetRoleNameFromClaims(It.IsAny<IEnumerable<Claim>>())).Returns(roleMakingChange).Verifiable();
            
            var result = await _userController.UpdateUser(_userId, newUser) as ObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(400, result.StatusCode);
            _mockUserService.Verify(p => p.UpdateUser(It.IsAny<User>(), It.IsAny<string>()), Times.Never);
            _mockUserService.VerifyAll(); _mockStaff.VerifyAll();
        }

        [TestMethod]
        public async Task UpdateUser_PHOMemberCantUpdateAnotherUser_Returns400()
        {
            UserViewModel newUser = new UserViewModel()
            {
                Email = "something@example.com",
                Id = _user.Id,
                Role = new RoleViewModel() { Id = _user.Role.Id, Name = _user.Role.Name }
            };
            string userMakingChange = "SomeoneElse";
            string roleMakingChange = "PHO Member";
            _mockUserService.Setup(p => p.GetUser(_userId)).Returns(Task.FromResult(_user)).Verifiable();
            _mockUserService.Setup(p => p.ListRoles()).Returns(Task.FromResult(_roles)).Verifiable();
            _mockUserService.Setup(p => p.GetUserNameFromClaims(It.IsAny<IEnumerable<Claim>>())).Returns(userMakingChange).Verifiable();
            _mockUserService.Setup(p => p.GetRoleNameFromClaims(It.IsAny<IEnumerable<Claim>>())).Returns(roleMakingChange).Verifiable();

            var result = await _userController.UpdateUser(_userId, newUser) as ObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(400, result.StatusCode);
            _mockUserService.Verify(p => p.UpdateUser(It.IsAny<User>(), It.IsAny<string>()), Times.Never);
            _mockUserService.VerifyAll(); _mockStaff.VerifyAll();
        }

        [TestMethod]
        public async Task UpdateUser_PracticeMemberCantChangeTheirOwnRole_Returns400()
        {
            UserViewModel newUser = new UserViewModel()
            {
                Email = "something@example.com",
                Id = _user.Id,
                Role = new RoleViewModel() { Id = 2, Name = "Practice Admin" }
            };
            string userMakingChange = _user.UserName;
            string roleMakingChange = "Practice Member";
            _mockUserService.Setup(p => p.GetUser(_userId)).Returns(Task.FromResult(_user)).Verifiable();
            _mockUserService.Setup(p => p.ListRoles()).Returns(Task.FromResult(_roles)).Verifiable();
            _mockUserService.Setup(p => p.GetUserNameFromClaims(It.IsAny<IEnumerable<Claim>>())).Returns(userMakingChange).Verifiable();
            _mockUserService.Setup(p => p.GetRoleNameFromClaims(It.IsAny<IEnumerable<Claim>>())).Returns(roleMakingChange).Verifiable();

            var result = await _userController.UpdateUser(_userId, newUser) as ObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(400, result.StatusCode);
            _mockUserService.Verify(p => p.UpdateUser(It.IsAny<User>(), It.IsAny<string>()), Times.Never);
            _mockUserService.VerifyAll(); _mockStaff.VerifyAll();
        }

        [TestMethod]
        public async Task UpdateUser_PHOMemberCantChangeTheirOwnRole_Returns400()
        {
            _user.Role = new Role() { Id = 3, Name = "PHO Member" };
            UserViewModel newUser = new UserViewModel()
            {
                Email = "something@example.com",
                Id = _user.Id,
                Role = new RoleViewModel() { Id = 1, Name = "Practice Member" }
            };
            string userMakingChange = _user.UserName;
            string roleMakingChange = "PHO Member";
            _mockUserService.Setup(p => p.GetUser(_userId)).Returns(Task.FromResult(_user)).Verifiable();
            _mockUserService.Setup(p => p.ListRoles()).Returns(Task.FromResult(_roles)).Verifiable();
            _mockUserService.Setup(p => p.GetUserNameFromClaims(It.IsAny<IEnumerable<Claim>>())).Returns(userMakingChange).Verifiable();
            _mockUserService.Setup(p => p.GetRoleNameFromClaims(It.IsAny<IEnumerable<Claim>>())).Returns(roleMakingChange).Verifiable();

            var result = await _userController.UpdateUser(_userId, newUser) as ObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(400, result.StatusCode);
            _mockUserService.Verify(p => p.UpdateUser(It.IsAny<User>(), It.IsAny<string>()), Times.Never);
            _mockUserService.VerifyAll(); _mockStaff.VerifyAll();
        }

        [TestMethod]
        public async Task UpdateUser_PracticeAdminsCantSelectAPhoRole_Returns400()
        {
            UserViewModel newUser = new UserViewModel()
            {
                Email = "something@example.com",
                Id = _user.Id,
                Role = new RoleViewModel() { Id = 3, Name = "PHO Member" }
            };
            string userMakingChange = _user.UserName;
            string roleMakingChange = "Practice Admin";
            _mockUserService.Setup(p => p.GetUser(_userId)).Returns(Task.FromResult(_user)).Verifiable();
            _mockUserService.Setup(p => p.ListRoles()).Returns(Task.FromResult(_roles)).Verifiable();
            _mockUserService.Setup(p => p.GetUserNameFromClaims(It.IsAny<IEnumerable<Claim>>())).Returns(userMakingChange).Verifiable();
            _mockUserService.Setup(p => p.GetRoleNameFromClaims(It.IsAny<IEnumerable<Claim>>())).Returns(roleMakingChange).Verifiable();

            var result = await _userController.UpdateUser(_userId, newUser) as ObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(400, result.StatusCode);
            _mockUserService.Verify(p => p.UpdateUser(It.IsAny<User>(), It.IsAny<string>()), Times.Never);
            _mockUserService.VerifyAll(); _mockStaff.VerifyAll();
        }

        [TestMethod]
        public async Task UpdateUser_PracticeAdminCantChangeUserOutsideTheirPractice_Returns400()
        {
            UserViewModel newUser = new UserViewModel()
            {
                Email = "something@example.com",
                Id = _user.Id,
                Role = new RoleViewModel() { Id = 1, Name = "Practice Member" }
            };
            string userMakingChange = "someothername";
            string roleMakingChange = "Practice Admin";
            int userIdMakingChange = 8;
            _mockUserService.Setup(p => p.GetUser(_userId)).Returns(Task.FromResult(_user)).Verifiable();
            _mockUserService.Setup(p => p.ListRoles()).Returns(Task.FromResult(_roles)).Verifiable();
            _mockUserService.Setup(p => p.GetUserIdFromClaims(It.IsAny<IEnumerable<Claim>>())).Returns(userIdMakingChange).Verifiable();
            _mockUserService.Setup(p => p.GetUserNameFromClaims(It.IsAny<IEnumerable<Claim>>())).Returns(userMakingChange).Verifiable();
            _mockUserService.Setup(p => p.GetRoleNameFromClaims(It.IsAny<IEnumerable<Claim>>())).Returns(roleMakingChange).Verifiable();
            _mockStaff.Setup(p => p.IsStaffInSamePractice(userIdMakingChange, _user.StaffId)).Returns(false).Verifiable();

            var result = await _userController.UpdateUser(_userId, newUser) as ObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(400, result.StatusCode);
            _mockUserService.Verify(p => p.UpdateUser(It.IsAny<User>(), It.IsAny<string>()), Times.Never);
            _mockUserService.VerifyAll(); _mockStaff.VerifyAll();
        }

        [TestMethod]
        public async Task UpdateUser_PracticeAdminCanChangeUserInsideTheirPractice_Success()
        {
            UserViewModel newUser = new UserViewModel()
            {
                Email = "something@example.com",
                Id = _user.Id,
                Role = new RoleViewModel() { Id = 1, Name = "Practice Member" }
            };
            string userMakingChange = "someothername";
            string roleMakingChange = "Practice Admin";
            int userIdMakingChange = 8;
            _mockUserService.Setup(p => p.GetUser(_userId)).Returns(Task.FromResult(_user)).Verifiable();
            _mockUserService.Setup(p => p.ListRoles()).Returns(Task.FromResult(_roles)).Verifiable();
            _mockUserService.Setup(p => p.GetUserIdFromClaims(It.IsAny<IEnumerable<Claim>>())).Returns(userIdMakingChange).Verifiable();
            _mockUserService.Setup(p => p.GetUserNameFromClaims(It.IsAny<IEnumerable<Claim>>())).Returns(userMakingChange).Verifiable();
            _mockUserService.Setup(p => p.GetRoleNameFromClaims(It.IsAny<IEnumerable<Claim>>())).Returns(roleMakingChange).Verifiable();
            _mockStaff.Setup(p => p.IsStaffInSamePractice(userIdMakingChange, _user.StaffId)).Returns(true).Verifiable();
            _mockUserService.Setup(p => p.UpdateUser(It.IsAny<User>(), userMakingChange))
                .Returns<User, string>((x, y) => {
                    x.LastUpdatedBy = y;
                    x.LastUpdatedDate = DateTime.Now.Date;
                    return Task.FromResult(x);
                }).Verifiable();

            var result = await _userController.UpdateUser(_userId, newUser) as ObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            var resultUser = result.Value as UserViewModel;
            Assert.AreEqual(userMakingChange, resultUser.LastUpdatedBy);
            Assert.AreEqual(newUser.Email, resultUser.Email);
            _mockUserService.VerifyAll(); _mockStaff.VerifyAll();
        }

        [TestMethod]
        public async Task UpdateUser_ThrowsException_Returns500()
        {
            UserViewModel newUser = new UserViewModel()
            {
                Email = "something@example.com",
                Id = _user.Id,
                Role = new RoleViewModel() { Id = _user.Role.Id, Name = _user.Role.Name }
            };
            string userMakingChange = _userName;
            string roleMakingChange = _user.Role.Name;
            _mockUserService.Setup(p => p.GetUser(_userId)).Returns(Task.FromResult(_user)).Verifiable();
            _mockUserService.Setup(p => p.ListRoles()).Returns(Task.FromResult(_roles)).Verifiable();
            _mockUserService.Setup(p => p.GetUserNameFromClaims(It.IsAny<IEnumerable<Claim>>())).Returns(userMakingChange).Verifiable();
            _mockUserService.Setup(p => p.GetRoleNameFromClaims(It.IsAny<IEnumerable<Claim>>())).Returns(roleMakingChange).Verifiable();
            _mockUserService.Setup(p => p.UpdateUser(It.IsAny<User>(), userMakingChange))
                .Throws(new Exception()).Verifiable();

            var result = await _userController.UpdateUser(_userId, newUser) as ObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(500, result.StatusCode);
            _mockUserService.VerifyAll(); _mockStaff.VerifyAll();
        }
        #endregion

        #region InsertUser Tests
        [TestMethod]
        public async Task InsertUser_Success()
        {
            UserViewModel newUser = new UserViewModel()
            {
                Email = "something@example.com",
                Role = new RoleViewModel() { Id = 1, Name = _user.Role.Name },
                CreatedBy = "someone",
                CreatedDate = DateTime.Now.Date,
                FirstName = "first",
                LastName = "last",
                LastUpdatedBy = "smeone",
                LastUpdatedDate = DateTime.Now.Date,
                UserName = "someusername",
                StaffId = 7,
                NewPassword = "P@ssw0rd"
            };
            string userMakingChange = "sdffhasdf";
            string roleMakingChange = "PHO Admin";
            User tempUser = new User();
            _mockUserService.Setup(p => p.GetUser(newUser.UserName)).Returns(Task.FromResult((User)null)).Verifiable();
            _mockUserService.Setup(p => p.GetUserByStaffId(newUser.StaffId)).Returns(Task.FromResult((User)null)).Verifiable();
            _mockUserService.Setup(p => p.ListRoles()).Returns(Task.FromResult(_roles)).Verifiable();
            _mockUserService.Setup(p => p.GetUserNameFromClaims(It.IsAny<IEnumerable<Claim>>())).Returns(userMakingChange).Verifiable();
            _mockUserService.Setup(p => p.GetRoleNameFromClaims(It.IsAny<IEnumerable<Claim>>())).Returns(roleMakingChange).Verifiable();
            _mockStaff.Setup(p => p.GetStaffDetailsById(newUser.StaffId)).Returns(Task.FromResult(new StaffDetail())).Verifiable();
            int userIdMakingChange = 34;
            _mockUserService.Setup(p => p.GetUserIdFromClaims(It.IsAny<IEnumerable<Claim>>())).Returns(userIdMakingChange).Verifiable();
            _mockStaff.Setup(p => p.IsStaffInSamePractice(userIdMakingChange, newUser.StaffId)).Returns(true).Verifiable();
            _mockUserService.Setup(p => p.InsertUser(It.IsAny<User>(), newUser.NewPassword, userMakingChange))
                .Returns<User, string, string>((x, z, y) =>
                {
                    x.Id = 3;
                    x.IsPending = true;
                    x.StaffId = 0;
                    tempUser = x;
                    return Task.FromResult(x);
                }).Verifiable();
            _mockUserService.Setup(p => p.AssignStaffIdToUser(3, newUser.StaffId, userMakingChange))
                .Returns<int, int, string>((x, y, z) =>
                {
                    tempUser.StaffId = y;
                    tempUser.IsPending = false;
                    return Task.FromResult(tempUser);
                }).Verifiable();

            var result = await _userController.InsertUser(newUser) as ObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            Assert.IsTrue(result.Value is UserViewModel);
            var resultValue = result.Value as UserViewModel;
            Assert.IsNull(resultValue.NewPassword);
            _mockUserService.VerifyAll(); _mockStaff.VerifyAll();
        }

        [TestMethod]
        public async Task InsertUser_UserModelNull_400()
        {
            var result = await _userController.InsertUser(null) as ObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(400, result.StatusCode);
            _mockUserService.Verify(p => p.InsertUser(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _mockUserService.Verify(p => p.AssignStaffIdToUser(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()), Times.Never);
            _mockUserService.VerifyAll(); _mockStaff.VerifyAll();
        }

        [TestMethod]
        public async Task InsertUser_NewPasswordNull_400()
        {
            UserViewModel newUser = new UserViewModel()
            {
                Email = "something@example.com",
                Role = new RoleViewModel() { Id = 1, Name = _user.Role.Name },
                CreatedBy = "someone",
                CreatedDate = DateTime.Now.Date,
                FirstName = "first",
                LastName = "last",
                LastUpdatedBy = "smeone",
                LastUpdatedDate = DateTime.Now.Date,
                UserName = "someusername",
                StaffId = 7,
                NewPassword = null
            };
            
            var result = await _userController.InsertUser(newUser) as ObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(400, result.StatusCode);
            _mockUserService.Verify(p => p.InsertUser(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _mockUserService.Verify(p => p.AssignStaffIdToUser(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()), Times.Never);
            _mockUserService.VerifyAll(); _mockStaff.VerifyAll();
        }

        [TestMethod]
        public async Task InsertUser_NewPasswordEmpty_400()
        {
            UserViewModel newUser = new UserViewModel()
            {
                Email = "something@example.com",
                Role = new RoleViewModel() { Id = 1, Name = _user.Role.Name },
                CreatedBy = "someone",
                CreatedDate = DateTime.Now.Date,
                FirstName = "first",
                LastName = "last",
                LastUpdatedBy = "smeone",
                LastUpdatedDate = DateTime.Now.Date,
                UserName = "someusername",
                StaffId = 7,
                NewPassword = " "
            };

            var result = await _userController.InsertUser(newUser) as ObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(400, result.StatusCode);
            _mockUserService.Verify(p => p.InsertUser(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _mockUserService.Verify(p => p.AssignStaffIdToUser(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()), Times.Never);
            _mockUserService.VerifyAll(); _mockStaff.VerifyAll();
        }

        [TestMethod]
        public async Task InsertUser_PasswordValidationFailure_400()
        {
            UserViewModel newUser = new UserViewModel()
            {
                Email = "something@example.com",
                Role = new RoleViewModel() { Id = 1, Name = _user.Role.Name },
                CreatedBy = "someone",
                CreatedDate = DateTime.Now.Date,
                FirstName = "first",
                LastName = "last",
                LastUpdatedBy = "smeone",
                LastUpdatedDate = DateTime.Now.Date,
                UserName = "someusername",
                StaffId = 7,
                NewPassword = "yikes"
            };
            string userMakingChange = "sdffhasdf";
            string roleMakingChange = "PHO Admin";
            User tempUser = new User();
            _mockUserService.Setup(p => p.GetUser(newUser.UserName)).Returns(Task.FromResult((User)null)).Verifiable();
            _mockUserService.Setup(p => p.GetUserByStaffId(newUser.StaffId)).Returns(Task.FromResult((User)null)).Verifiable();
            _mockUserService.Setup(p => p.ListRoles()).Returns(Task.FromResult(_roles)).Verifiable();
            _mockUserService.Setup(p => p.GetUserNameFromClaims(It.IsAny<IEnumerable<Claim>>())).Returns(userMakingChange).Verifiable();
            _mockUserService.Setup(p => p.GetRoleNameFromClaims(It.IsAny<IEnumerable<Claim>>())).Returns(roleMakingChange).Verifiable();

            var result = await _userController.InsertUser(newUser) as ObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(400, result.StatusCode);
            _mockUserService.Verify(p => p.InsertUser(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _mockUserService.Verify(p => p.AssignStaffIdToUser(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()), Times.Never);
            _mockUserService.VerifyAll(); _mockStaff.VerifyAll();
        }

        [TestMethod]
        public async Task InsertUser_StaffDoesNotExist_400()
        {
            UserViewModel newUser = new UserViewModel()
            {
                Email = "something@example.com",
                Role = new RoleViewModel() { Id = 1, Name = _user.Role.Name },
                CreatedBy = "someone",
                CreatedDate = DateTime.Now.Date,
                FirstName = "first",
                LastName = "last",
                LastUpdatedBy = "smeone",
                LastUpdatedDate = DateTime.Now.Date,
                UserName = "someusername",
                StaffId = 7,
                NewPassword = "P@ssw0rd"
            };
            string userMakingChange = "sdffhasdf";
            string roleMakingChange = "PHO Admin";
            User tempUser = new User();
            _mockUserService.Setup(p => p.GetUser(newUser.UserName)).Returns(Task.FromResult((User)null)).Verifiable();
            _mockUserService.Setup(p => p.GetUserByStaffId(newUser.StaffId)).Returns(Task.FromResult((User)null)).Verifiable();
            _mockUserService.Setup(p => p.ListRoles()).Returns(Task.FromResult(_roles)).Verifiable();
            _mockUserService.Setup(p => p.GetUserNameFromClaims(It.IsAny<IEnumerable<Claim>>())).Returns(userMakingChange).Verifiable();
            _mockUserService.Setup(p => p.GetRoleNameFromClaims(It.IsAny<IEnumerable<Claim>>())).Returns(roleMakingChange).Verifiable();
            _mockStaff.Setup(p => p.GetStaffDetailsById(newUser.StaffId)).Returns(Task.FromResult((StaffDetail)null)).Verifiable();

            var result = await _userController.InsertUser(newUser) as ObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(400, result.StatusCode);
            _mockUserService.Verify(p => p.InsertUser(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _mockUserService.Verify(p => p.AssignStaffIdToUser(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()), Times.Never);
            _mockUserService.VerifyAll(); _mockStaff.VerifyAll();
        }

        [TestMethod]
        public async Task InsertUser_StaffIsNotInMyPractice_400()
        {
            UserViewModel newUser = new UserViewModel()
            {
                Email = "something@example.com",
                Role = new RoleViewModel() { Id = 1, Name = _user.Role.Name },
                CreatedBy = "someone",
                CreatedDate = DateTime.Now.Date,
                FirstName = "first",
                LastName = "last",
                LastUpdatedBy = "smeone",
                LastUpdatedDate = DateTime.Now.Date,
                UserName = "someusername",
                StaffId = 7,
                NewPassword = "P@ssw0rd"
            };
            string userMakingChange = "sdffhasdf";
            string roleMakingChange = "PHO Admin";
            User tempUser = new User();
            _mockUserService.Setup(p => p.GetUser(newUser.UserName)).Returns(Task.FromResult((User)null)).Verifiable();
            _mockUserService.Setup(p => p.GetUserByStaffId(newUser.StaffId)).Returns(Task.FromResult((User)null)).Verifiable();
            _mockUserService.Setup(p => p.ListRoles()).Returns(Task.FromResult(_roles)).Verifiable();
            _mockUserService.Setup(p => p.GetUserNameFromClaims(It.IsAny<IEnumerable<Claim>>())).Returns(userMakingChange).Verifiable();
            _mockUserService.Setup(p => p.GetRoleNameFromClaims(It.IsAny<IEnumerable<Claim>>())).Returns(roleMakingChange).Verifiable();
            _mockStaff.Setup(p => p.GetStaffDetailsById(newUser.StaffId)).Returns(Task.FromResult(new StaffDetail())).Verifiable();
            int userIdMakingChange = 34;
            _mockUserService.Setup(p => p.GetUserIdFromClaims(It.IsAny<IEnumerable<Claim>>())).Returns(userIdMakingChange).Verifiable();
            _mockStaff.Setup(p => p.IsStaffInSamePractice(userIdMakingChange, newUser.StaffId)).Returns(false).Verifiable();
            var result = await _userController.InsertUser(newUser) as ObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(400, result.StatusCode);
            _mockUserService.Verify(p => p.InsertUser(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _mockUserService.Verify(p => p.AssignStaffIdToUser(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()), Times.Never);
            _mockUserService.VerifyAll(); _mockStaff.VerifyAll();
        }

        [TestMethod]
        public async Task InsertUser_UserAlreadyExistsForStaffId_400()
        {
            UserViewModel newUser = new UserViewModel()
            {
                Email = "something@example.com",
                Role = new RoleViewModel() { Id = 1, Name = _user.Role.Name },
                CreatedBy = "someone",
                CreatedDate = DateTime.Now.Date,
                FirstName = "first",
                LastName = "last",
                LastUpdatedBy = "smeone",
                LastUpdatedDate = DateTime.Now.Date,
                UserName = "someusername",
                StaffId = 7,
                NewPassword = "P@ssw0rd"
            };
            User tempUser = new User();
            _mockUserService.Setup(p => p.GetUser(newUser.UserName)).Returns(Task.FromResult((User)null)).Verifiable();
            _mockUserService.Setup(p => p.GetUserByStaffId(newUser.StaffId)).Returns(Task.FromResult(new User())).Verifiable();
            var result = await _userController.InsertUser(newUser) as ObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(400, result.StatusCode);
            _mockUserService.Verify(p => p.InsertUser(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _mockUserService.Verify(p => p.AssignStaffIdToUser(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()), Times.Never);
            _mockUserService.VerifyAll(); _mockStaff.VerifyAll();
        }

        [TestMethod]
        public async Task InsertUser_UserAlreadyExistsByUserName_Returns400()
        {
            UserViewModel newUser = new UserViewModel()
            {
                Email = "something@example.com",
                Role = new RoleViewModel() { Id = 1, Name = _user.Role.Name },
                CreatedBy = "someone",
                CreatedDate = DateTime.Now.Date,
                FirstName = "first",
                LastName = "last",
                LastUpdatedBy = "smeone",
                LastUpdatedDate = DateTime.Now.Date,
                UserName = "someusername",
                StaffId = 7,
                NewPassword = "P@ssw0rd"
            };
            _mockUserService.Setup(p => p.GetUser(newUser.UserName)).Returns(Task.FromResult(_user)).Verifiable();

            var result = await _userController.InsertUser(newUser) as ObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(400, result.StatusCode);
            _mockUserService.Verify(p => p.InsertUser(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _mockUserService.VerifyAll(); _mockStaff.VerifyAll();
        }

        [TestMethod]
        public async Task InsertUser_PracticeAdminCantUsePHORole_Returns400()
        {
            UserViewModel newUser = new UserViewModel()
            {
                Email = "something@example.com",
                Role = new RoleViewModel() { Id = 3, Name = "PHO Member" },
                CreatedBy = "someone",
                CreatedDate = DateTime.Now.Date,
                FirstName = "first",
                LastName = "last",
                LastUpdatedBy = "smeone",
                LastUpdatedDate = DateTime.Now.Date,
                UserName = "someusername",
                StaffId = 7,
                NewPassword = "P@ssw0rd"
            };
            _mockUserService.Setup(p => p.GetUser(newUser.UserName)).Returns(Task.FromResult((User)null)).Verifiable();
            _mockUserService.Setup(p => p.GetUserByStaffId(newUser.StaffId)).Returns(Task.FromResult((User)null)).Verifiable();
            string userMakingChange = "sdffhasdf";
            string roleMakingChange = "Practice Admin";
            _mockUserService.Setup(p => p.ListRoles()).Returns(Task.FromResult(_roles)).Verifiable();
            _mockUserService.Setup(p => p.GetUser(newUser.UserName)).Returns(Task.FromResult((User)null)).Verifiable();
            _mockUserService.Setup(p => p.GetUserNameFromClaims(It.IsAny<IEnumerable<Claim>>())).Returns(userMakingChange).Verifiable();
            _mockUserService.Setup(p => p.GetRoleNameFromClaims(It.IsAny<IEnumerable<Claim>>())).Returns(roleMakingChange).Verifiable();

            var result = await _userController.InsertUser(newUser) as ObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(400, result.StatusCode);
            _mockUserService.Verify(p => p.InsertUser(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _mockUserService.VerifyAll(); _mockStaff.VerifyAll();
        }

        [TestMethod]
        public async Task InsertUser_RoleNotFound_Returns400()
        {
            UserViewModel newUser = new UserViewModel()
            {
                Email = "something@example.com",
                Role = new RoleViewModel() { Id = 1, Name = _user.Role.Name },
                CreatedBy = "someone",
                CreatedDate = DateTime.Now.Date,
                FirstName = "first",
                LastName = "last",
                LastUpdatedBy = "smeone",
                LastUpdatedDate = DateTime.Now.Date,
                UserName = "someusername",
                StaffId = 7,
                NewPassword = "P@ssw0rd"
            };
            string userMakingChange = "sdffhasdf";
            _mockUserService.Setup(p => p.GetUser(newUser.UserName)).Returns(Task.FromResult((User)null)).Verifiable();
            _mockUserService.Setup(p => p.GetUserByStaffId(newUser.StaffId)).Returns(Task.FromResult((User)null)).Verifiable();
            _mockUserService.Setup(p => p.ListRoles()).Returns(Task.FromResult(new List<Role>())).Verifiable();

            var result = await _userController.InsertUser(newUser) as ObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(400, result.StatusCode);
            _mockUserService.Verify(p => p.InsertUser(It.IsAny<User>(), It.IsAny<string>(), userMakingChange), Times.Never);
            _mockUserService.VerifyAll(); _mockStaff.VerifyAll();
        }

        [TestMethod]
        public async Task InsertUser_ThrowsException_Returns500()
        {
            UserViewModel newUser = new UserViewModel()
            {
                Email = "something@example.com",
                Role = new RoleViewModel() { Id = 1, Name = _user.Role.Name },
                CreatedBy = "someone",
                CreatedDate = DateTime.Now.Date,
                FirstName = "first",
                LastName = "last",
                LastUpdatedBy = "smeone",
                LastUpdatedDate = DateTime.Now.Date,
                UserName = "someusername",
                StaffId = 7,
                NewPassword = "P@ssw0rd"
            };
            string userMakingChange = "sdffhasdf";
            string roleMakingChange = "PHO Admin";
            _mockUserService.Setup(p => p.GetUser(newUser.UserName)).Returns(Task.FromResult((User)null)).Verifiable();
            _mockUserService.Setup(p => p.GetUserByStaffId(newUser.StaffId)).Returns(Task.FromResult((User)null)).Verifiable();
            _mockUserService.Setup(p => p.ListRoles()).Returns(Task.FromResult(_roles)).Verifiable();
            _mockUserService.Setup(p => p.GetUserNameFromClaims(It.IsAny<IEnumerable<Claim>>())).Returns(userMakingChange).Verifiable();
            _mockUserService.Setup(p => p.GetRoleNameFromClaims(It.IsAny<IEnumerable<Claim>>())).Returns(roleMakingChange).Verifiable();
            _mockStaff.Setup(p => p.GetStaffDetailsById(newUser.StaffId)).Returns(Task.FromResult(new StaffDetail())).Verifiable();
            int userIdMakingChange = 34;
            _mockUserService.Setup(p => p.GetUserIdFromClaims(It.IsAny<IEnumerable<Claim>>())).Returns(userIdMakingChange).Verifiable();
            _mockStaff.Setup(p => p.IsStaffInSamePractice(userIdMakingChange, newUser.StaffId)).Returns(true).Verifiable();
            _mockUserService.Setup(p => p.InsertUser(It.IsAny<User>(), It.IsAny<string>(), userMakingChange))
                .Throws(new Exception()).Verifiable();

            var result = await _userController.InsertUser(newUser) as ObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(500, result.StatusCode);
            _mockUserService.VerifyAll(); _mockStaff.VerifyAll();
        }
        #endregion

        #region RemoveLockoutFromUser Tests
        [TestMethod]
        public async Task RemoveLockoutFromUser_Success()
        {
            string userMakingChange = "asdf";
            _mockUserService.Setup(p => p.GetUser(_userId)).Returns(Task.FromResult(_user)).Verifiable();
            _mockUserService.Setup(p => p.GetUserNameFromClaims(It.IsAny<IEnumerable<Claim>>())).Returns(userMakingChange).Verifiable();
            _mockUserService.Setup(p => p.RemoveLockoutFromUser(_userId, userMakingChange)).Returns(Task.FromResult(_user)).Verifiable();

            var result = await _userController.RemoveLockoutFromUser(_userId) as ObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            Assert.IsTrue(result.Value is UserViewModel);
            _mockUserService.VerifyAll(); _mockStaff.VerifyAll();
        }

        [TestMethod]
        public async Task RemoveLockoutFromUser_UserDoesNotExist_Returns400()
        {
            _mockUserService.Setup(p => p.GetUser(_userId)).Returns(Task.FromResult((User)null)).Verifiable();

            var result = await _userController.RemoveLockoutFromUser(_userId) as ObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(400, result.StatusCode);
            _mockUserService.Verify(p => p.RemoveLockoutFromUser(It.IsAny<int>(), It.IsAny<string>()), Times.Never);
            _mockUserService.VerifyAll(); _mockStaff.VerifyAll();
        }

        [TestMethod]
        public async Task RemoveLockoutFromUser_ThrowsException_Returns500()
        {
            _mockUserService.Setup(p => p.GetUser(_userId)).Throws(new Exception()).Verifiable();

            var result = await _userController.RemoveLockoutFromUser(_userId) as ObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(500, result.StatusCode);
            _mockUserService.Verify(p => p.RemoveLockoutFromUser(It.IsAny<int>(), It.IsAny<string>()), Times.Never);
            _mockUserService.VerifyAll(); _mockStaff.VerifyAll();
        }
        #endregion

        #region ToggleDeleteOnUser Tests
        [TestMethod]
        public async Task ToggleDeleteOnUser_Success()
        {
            string userMakingChange = "asdf";
            _mockUserService.Setup(p => p.GetUser(_userId)).Returns(Task.FromResult(_user)).Verifiable();
            _mockUserService.Setup(p => p.GetUserNameFromClaims(It.IsAny<IEnumerable<Claim>>())).Returns(userMakingChange).Verifiable();
            _mockUserService.Setup(p => p.ToggleDeleteOnUser(_userId, It.IsAny<bool>(), userMakingChange)).Returns(Task.FromResult(_user)).Verifiable();

            var result = await _userController.ToggleDeleteOnUser(_userId, true) as ObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            Assert.IsTrue(result.Value is UserViewModel);
            _mockUserService.VerifyAll(); _mockStaff.VerifyAll();
        }

        [TestMethod]
        public async Task ToggleDeleteOnUser_UserDoesNotExist_Returns400()
        {
            _mockUserService.Setup(p => p.GetUser(_userId)).Returns(Task.FromResult((User)null)).Verifiable();

            var result = await _userController.ToggleDeleteOnUser(_userId, true) as ObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(400, result.StatusCode);
            _mockUserService.Verify(p => p.ToggleDeleteOnUser(It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Never);
            _mockUserService.VerifyAll(); _mockStaff.VerifyAll();

        }

        [TestMethod]
        public async Task ToggleDeleteOnUser_ThrowsException_Returns500()
        {
            _mockUserService.Setup(p => p.GetUser(_userId)).Throws(new Exception()).Verifiable();

            var result = await _userController.ToggleDeleteOnUser(_userId, true) as ObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(500, result.StatusCode);
            _mockUserService.Verify(p => p.ToggleDeleteOnUser(It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Never);
            _mockUserService.VerifyAll(); _mockStaff.VerifyAll();
        }
        #endregion

        #region ListRoles Tests
        [TestMethod]
        public async Task ListRoles_Success()
        {
            _mockUserService.Setup(p => p.ListRoles()).Returns(Task.FromResult(_roles)).Verifiable();

            var result = await _userController.ListRoles() as ObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            Assert.IsTrue(result.Value is List<RoleViewModel>);
            Assert.AreEqual(4, ((List<RoleViewModel>)result.Value).Count);
            _mockUserService.VerifyAll(); _mockStaff.VerifyAll();
        }

        [TestMethod]
        public async Task ListRoles_ThrowsException_Returns500()
        {
            _mockUserService.Setup(p => p.ListRoles()).Throws(new Exception()).Verifiable();

            var result = await _userController.ListRoles() as ObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(500, result.StatusCode);
            _mockUserService.VerifyAll(); _mockStaff.VerifyAll();
        }
        #endregion

        #region Refresh Tests
        [TestMethod]
        public async Task Refresh_Success()
        {
            var refreshModel = new AuthenticationRefreshRequest()
            {
                RefreshToken = "ASDF",
                Token = "DSAAESRTFDJ"
            };

            _mockUserService.Setup(p => p.Refresh(refreshModel.Token, refreshModel.RefreshToken))
                .Returns(Task.FromResult(_user)).Verifiable();

            var result = await _userController.Refresh(refreshModel) as ObjectResult;

            // assert
            Assert.IsNotNull(result);
            var authResult = result.Value as AuthenticationResult;
            Assert.AreEqual("Authorized", authResult.Status);
            Assert.IsNotNull(authResult.User);
            _mockUserService.VerifyAll(); _mockStaff.VerifyAll();
        }

        [TestMethod]
        public async Task Refresh_ModelNull_401()
        {
            var result = await _userController.Refresh(null) as ObjectResult;

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(401, result.StatusCode);
            _mockUserService.Verify(p => p.Refresh(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _mockUserService.VerifyAll(); _mockStaff.VerifyAll();
        }

        [TestMethod]
        public async Task Refresh_RefreshTokenNull_401()
        {
            var refreshModel = new AuthenticationRefreshRequest()
            {
                RefreshToken = null,
                Token = "DSAAESRTFDJ"
            };

            var result = await _userController.Refresh(refreshModel) as ObjectResult;

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(401, result.StatusCode);
            _mockUserService.Verify(p => p.Refresh(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _mockUserService.VerifyAll(); _mockStaff.VerifyAll();
        }

        [TestMethod]
        public async Task Refresh_RefreshTokenEmpty_401()
        {
            var refreshModel = new AuthenticationRefreshRequest()
            {
                RefreshToken = " ",
                Token = "DSAAESRTFDJ"
            };

            var result = await _userController.Refresh(refreshModel) as ObjectResult;

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(401, result.StatusCode);
            _mockUserService.Verify(p => p.Refresh(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _mockUserService.VerifyAll(); _mockStaff.VerifyAll();
        }

        [TestMethod]
        public async Task Refresh_RefreshUnsuccessful_401()
        {
            var refreshModel = new AuthenticationRefreshRequest()
            {
                RefreshToken = "ASDF",
                Token = "DSAAESRTFDJ"
            };

            _mockUserService.Setup(p => p.Refresh(refreshModel.Token, refreshModel.RefreshToken))
                .Returns(Task.FromResult((User)null)).Verifiable();

            var result = await _userController.Refresh(refreshModel) as ObjectResult;

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(401, result.StatusCode);
            _mockUserService.VerifyAll(); _mockStaff.VerifyAll();
        }

        [TestMethod]
        public async Task Refresh_CatchesException_500()
        {
            var refreshModel = new AuthenticationRefreshRequest()
            {
                RefreshToken = "ASDF",
                Token = "DSAAESRTFDJ"
            };

            _mockUserService.Setup(p => p.Refresh(refreshModel.Token, refreshModel.RefreshToken))
                .Throws(new Exception()).Verifiable();

            var result = await _userController.Refresh(refreshModel) as ObjectResult;

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(500, result.StatusCode);
            _mockUserService.VerifyAll(); _mockStaff.VerifyAll();
        }
        #endregion

        #region GetUser Tests
        [TestMethod]
        public async Task GetUser_PracticeMember_RetrieveSelf_Success()
        {
            string currentUserName = _user.UserName;
            string currentRole = "Practice Member";
            _mockUserService.Setup(p => p.GetUserByStaffId(_user.StaffId)).Returns(Task.FromResult(_user)).Verifiable();
            _mockUserService.Setup(p => p.GetUserNameFromClaims(It.IsAny<IEnumerable<Claim>>())).Returns(currentUserName).Verifiable();
            _mockUserService.Setup(p => p.GetRoleNameFromClaims(It.IsAny<IEnumerable<Claim>>())).Returns(currentRole).Verifiable();

            var result = await _userController.GetUser(_user.StaffId) as ObjectResult;

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            Assert.IsTrue(result.Value is UserViewModel);
            var resultValue = result.Value as UserViewModel;

            _mockUserService.VerifyAll(); _mockStaff.VerifyAll();
        }

        [TestMethod]
        public async Task GetUser_PracticeAdmin_RetrieveSelf_Success()
        {
            string currentUserName = _user.UserName;
            string currentRole = "Practice Admin";
            _mockUserService.Setup(p => p.GetUserByStaffId(_user.StaffId)).Returns(Task.FromResult(_user)).Verifiable();
            _mockUserService.Setup(p => p.GetUserNameFromClaims(It.IsAny<IEnumerable<Claim>>())).Returns(currentUserName).Verifiable();
            _mockUserService.Setup(p => p.GetRoleNameFromClaims(It.IsAny<IEnumerable<Claim>>())).Returns(currentRole).Verifiable();

            var result = await _userController.GetUser(_user.StaffId) as ObjectResult;

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            Assert.IsTrue(result.Value is UserViewModel);
            var resultValue = result.Value as UserViewModel;

            _mockUserService.VerifyAll(); _mockStaff.VerifyAll();
        }

        [TestMethod]
        public async Task GetUser_PracticeAdmin_RetrieveOther_Success()
        {
            string currentUserName = "ASDF";
            string currentRole = "Practice Admin";
            int currentUserId = 999;
            _mockUserService.Setup(p => p.GetUserByStaffId(_user.StaffId)).Returns(Task.FromResult(_user)).Verifiable();
            _mockUserService.Setup(p => p.GetUserNameFromClaims(It.IsAny<IEnumerable<Claim>>())).Returns(currentUserName).Verifiable();
            _mockUserService.Setup(p => p.GetRoleNameFromClaims(It.IsAny<IEnumerable<Claim>>())).Returns(currentRole).Verifiable();
            _mockUserService.Setup(p => p.GetUserIdFromClaims(It.IsAny<IEnumerable<Claim>>())).Returns(currentUserId).Verifiable();
            _mockStaff.Setup(p => p.IsStaffInSamePractice(currentUserId, _user.StaffId)).Returns(true).Verifiable();
            
            var result = await _userController.GetUser(_user.StaffId) as ObjectResult;

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            Assert.IsTrue(result.Value is UserViewModel);
            var resultValue = result.Value as UserViewModel;

            _mockUserService.VerifyAll(); _mockStaff.VerifyAll();
        }

        [TestMethod]
        public async Task GetUser_PHOMember_RetrieveSelf_Success()
        {
            string currentUserName = _user.UserName;
            string currentRole = "PHO Member";
            _mockUserService.Setup(p => p.GetUserByStaffId(_user.StaffId)).Returns(Task.FromResult(_user)).Verifiable();
            _mockUserService.Setup(p => p.GetUserNameFromClaims(It.IsAny<IEnumerable<Claim>>())).Returns(currentUserName).Verifiable();
            _mockUserService.Setup(p => p.GetRoleNameFromClaims(It.IsAny<IEnumerable<Claim>>())).Returns(currentRole).Verifiable();

            var result = await _userController.GetUser(_user.StaffId) as ObjectResult;

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            Assert.IsTrue(result.Value is UserViewModel);
            var resultValue = result.Value as UserViewModel;

            _mockUserService.VerifyAll(); _mockStaff.VerifyAll();
        }

        [TestMethod]
        public async Task GetUser_PHOAdmin_RetrieveSelf_Success()
        {
            string currentUserName = _user.UserName;
            string currentRole = "PHO Admin";
            _mockUserService.Setup(p => p.GetUserByStaffId(_user.StaffId)).Returns(Task.FromResult(_user)).Verifiable();
            _mockUserService.Setup(p => p.GetUserNameFromClaims(It.IsAny<IEnumerable<Claim>>())).Returns(currentUserName).Verifiable();
            _mockUserService.Setup(p => p.GetRoleNameFromClaims(It.IsAny<IEnumerable<Claim>>())).Returns(currentRole).Verifiable();

            var result = await _userController.GetUser(_user.StaffId) as ObjectResult;

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            Assert.IsTrue(result.Value is UserViewModel);
            var resultValue = result.Value as UserViewModel;

            _mockUserService.VerifyAll(); _mockStaff.VerifyAll();
        }

        [TestMethod]
        public async Task GetUser_PHOAdmin_RetrieveOther_Success()
        {
            string currentUserName = "ASDF";
            string currentRole = "PHO Admin";
            _mockUserService.Setup(p => p.GetUserByStaffId(_user.StaffId)).Returns(Task.FromResult(_user)).Verifiable();
            _mockUserService.Setup(p => p.GetUserNameFromClaims(It.IsAny<IEnumerable<Claim>>())).Returns(currentUserName).Verifiable();
            _mockUserService.Setup(p => p.GetRoleNameFromClaims(It.IsAny<IEnumerable<Claim>>())).Returns(currentRole).Verifiable();
            
            var result = await _userController.GetUser(_user.StaffId) as ObjectResult;

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            Assert.IsTrue(result.Value is UserViewModel);
            var resultValue = result.Value as UserViewModel;

            _mockUserService.VerifyAll(); _mockStaff.VerifyAll();
        }

        [TestMethod]
        public async Task GetUser_UserDoesNotExist_404()
        {
            _mockUserService.Setup(p => p.GetUserByStaffId(_user.StaffId)).Returns(Task.FromResult((User)null)).Verifiable();
            
            var result = await _userController.GetUser(_user.StaffId) as ObjectResult;

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(404, result.StatusCode);
            _mockUserService.VerifyAll(); _mockStaff.VerifyAll();
        }

        [TestMethod]
        public async Task GetUser_PracticeMemberCannotRetrieveOtherUsers_400()
        {
            string currentUserName = "ASDF";
            string currentRole = "Practice Member";
            _mockUserService.Setup(p => p.GetUserByStaffId(_user.StaffId)).Returns(Task.FromResult(_user)).Verifiable();
            _mockUserService.Setup(p => p.GetUserNameFromClaims(It.IsAny<IEnumerable<Claim>>())).Returns(currentUserName).Verifiable();
            _mockUserService.Setup(p => p.GetRoleNameFromClaims(It.IsAny<IEnumerable<Claim>>())).Returns(currentRole).Verifiable();
            
            var result = await _userController.GetUser(_user.StaffId) as ObjectResult;

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(400, result.StatusCode);
            _mockUserService.VerifyAll(); _mockStaff.VerifyAll();
        }

        [TestMethod]
        public async Task GetUser_PracticeAdminsCannotRetrieveUserInAnotherPractice_400()
        {
            string currentUserName = "ASDF";
            string currentRole = "Practice Admin";
            int currentUserId = 999;
            _mockUserService.Setup(p => p.GetUserByStaffId(_user.StaffId)).Returns(Task.FromResult(_user)).Verifiable();
            _mockUserService.Setup(p => p.GetUserNameFromClaims(It.IsAny<IEnumerable<Claim>>())).Returns(currentUserName).Verifiable();
            _mockUserService.Setup(p => p.GetRoleNameFromClaims(It.IsAny<IEnumerable<Claim>>())).Returns(currentRole).Verifiable();
            _mockUserService.Setup(p => p.GetUserIdFromClaims(It.IsAny<IEnumerable<Claim>>())).Returns(currentUserId).Verifiable();
            _mockStaff.Setup(p => p.IsStaffInSamePractice(currentUserId, _user.StaffId)).Returns(false).Verifiable();

            var result = await _userController.GetUser(_user.StaffId) as ObjectResult;

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(400, result.StatusCode);
            _mockUserService.VerifyAll(); _mockStaff.VerifyAll();
        }

        [TestMethod]
        public async Task GetUser_PHOMembersCannotRetrieveOtherUsers_400()
        {
            string currentUserName = "ASDF";
            string currentRole = "PHO Member";
            _mockUserService.Setup(p => p.GetUserByStaffId(_user.StaffId)).Returns(Task.FromResult(_user)).Verifiable();
            _mockUserService.Setup(p => p.GetUserNameFromClaims(It.IsAny<IEnumerable<Claim>>())).Returns(currentUserName).Verifiable();
            _mockUserService.Setup(p => p.GetRoleNameFromClaims(It.IsAny<IEnumerable<Claim>>())).Returns(currentRole).Verifiable();
            
            var result = await _userController.GetUser(_user.StaffId) as ObjectResult;

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(400, result.StatusCode);
            _mockUserService.VerifyAll(); _mockStaff.VerifyAll();
        }

        [TestMethod]
        public async Task GetUser_CatchesException_500()
        {
            _mockUserService.Setup(p => p.GetUserByStaffId(_user.StaffId)).Throws(new Exception()).Verifiable();

            var result = await _userController.GetUser(_user.StaffId) as ObjectResult;

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(500, result.StatusCode);
            _mockUserService.VerifyAll(); _mockStaff.VerifyAll();
        }
        #endregion
    }
}
