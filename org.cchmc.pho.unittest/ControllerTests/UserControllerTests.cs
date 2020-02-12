using AutoMapper;
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
        private Mock<UserManager<User>> _mockUserManager;
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

        [TestInitialize]
        public void Initialize()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddMaps(Assembly.GetExecutingAssembly());
                cfg.AddMaps(Assembly.GetAssembly(typeof(AlertMappings)));
            });
            _mapper = config.CreateMapper();
            //_mockUserManager = new Mock<IUserStore<User>>();
            _mockUserManager = MockUserManager();
            _mockLogger = new Mock<ILogger<UserController>>();
            _mockOptions = new Mock<IOptions<CustomOptions>>();
            //todo populate values later.
            _mockOptions.Setup(op => op.Value).Returns(new CustomOptions());
        }

        [TestMethod]
        public async Task GetUser_Success()
        {
            // setup
            var user = new User()
            {
                FirstName = "some",
                Email = "someone@example.com",
                LastName = "one",
                UserName = "someone"
            };
            var roles = new List<string> { "role1", "role2" };
            _mockUserManager.Setup(p => p.FindByNameAsync(user.UserName)).Returns(Task.FromResult(user)).Verifiable();
            _mockUserManager.Setup(p => p.GetRolesAsync(It.Is<User>(x => x.UserName == user.UserName))).Returns(Task.FromResult((IList<string>)roles)).Verifiable();
            _userController = new UserController(_mockLogger.Object, _mapper, _mockUserManager.Object);

            // execute
            var result = await _userController.GetUser(user.UserName) as ObjectResult;

            // assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Value);
            var resultValue = result.Value as UserViewModel;
            Assert.AreEqual(2, resultValue.Roles.Count);
            Assert.IsTrue(resultValue.Roles.Contains(roles[0]));
            Assert.IsTrue(resultValue.Roles.Contains(roles[1]));
            Assert.AreEqual(user.FirstName, resultValue.FirstName);
            Assert.AreEqual(user.Email, resultValue.Email);
            Assert.AreEqual(user.LastName, resultValue.LastName);
            Assert.AreEqual(user.UserName, resultValue.UserName);
            _mockUserManager.Verify();
        }
    }
}
