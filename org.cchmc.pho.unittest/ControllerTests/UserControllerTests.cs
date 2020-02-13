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
using System.Threading.Tasks;

namespace org.cchmc.pho.unittest.ControllerTests
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class UserControllerTests
    {
        private UserController _userController;
        private Mock<ILogger<UserController>> _mockLogger;
        private Mock<IUserService> _mockUserService;
        private Mock<IOptions<CustomOptions>> _mockOptions;
        private IMapper _mapper;

        /*
         * Found this method on SO: https://stackoverflow.com/questions/49165810/how-to-mock-usermanager-in-net-core-testing
         * You can't just new Mock<UserManager<User>> like everything else. Also in this SO question is a link to the
         * Identity source code on GitHub showing how they mock it. Included here for full context:
         * https://github.com/aspnet/AspNetCore/blob/master/src/Identity/test/Shared/MockHelpers.cs
         */
        protected static Mock<UserManager<User>> MockUserManager()
        {
            var store = new Mock<IUserStore<User>>();
            var mgr = new Mock<UserManager<User>>(store.Object, null, null, null, null, null, null, null, null);
            mgr.Object.UserValidators.Add(new UserValidator<User>());
            mgr.Object.PasswordValidators.Add(new PasswordValidator<User>());
            return mgr;
        }

        protected static Mock<SignInManager<User>> MockSignInManager(Mock<UserManager<User>> mockUserManager)
        {
            return new Mock<SignInManager<User>>(mockUserManager.Object,
                                                    new Mock<IHttpContextAccessor>().Object,
                                                    new Mock<IUserClaimsPrincipalFactory<User>>().Object,
                                                    new Mock<IOptions<IdentityOptions>>().Object,
                                                    new Mock<ILogger<SignInManager<User>>>().Object,
                                                    new Mock<IAuthenticationSchemeProvider>().Object);
        }

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
            _mockLogger = new Mock<ILogger<UserController>>();
            _mockOptions = new Mock<IOptions<CustomOptions>>();
            //todo populate values later.
            _mockOptions.Setup(op => op.Value).Returns(new CustomOptions());

            _userController = new UserController(_mockLogger.Object, _mapper, _mockUserService.Object, _mockOptions.Object);
        }

        [TestMethod]
        public async Task Authenticate_Success()
        {
            // setup
            var userName = "myuserName";
            var password = "mypassword";
            var user = new User()
            {
                Email = "someone@example.com",
                FirstName = "Some",
                LastName = "One",
                UserName = userName,
                Token = "fhgdfsa"
            };
            _mockUserService.Setup(p => p.Authenticate(userName, password)).Returns(Task.FromResult(user)).Verifiable();

            // execute
            var result = await _userController.Authenticate(new AuthenticationRequest() { Username = userName, Password = password }) as ObjectResult;

            // assert
            Assert.IsNotNull(result);
            var authResult = result.Value as AuthenticationResult;
            Assert.AreEqual("Authorized", authResult.Status);
            Assert.AreEqual(user.Token, authResult.Token);
        }

        [TestMethod]
        public async Task Authenticate_Failure()
        {
            // setup
            var userName = "myuserName";
            var password = "mypassword";
            _mockUserService.Setup(p => p.Authenticate(userName, password)).Returns(Task.FromResult((User)null)).Verifiable();

            // execute
            var result = await _userController.Authenticate(new AuthenticationRequest() { Username = userName, Password = password }) as ObjectResult;

            // assert
            Assert.IsNotNull(result);
            var authResult = result.Value as AuthenticationResult;
            Assert.AreEqual("User not found or password did not match", authResult.Status);
            Assert.IsTrue(String.IsNullOrEmpty(authResult.Token));
        }
    }
}
