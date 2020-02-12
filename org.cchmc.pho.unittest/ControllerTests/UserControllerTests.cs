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

        [TestInitialize]
        public void Initialize()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddMaps(Assembly.GetExecutingAssembly());
                cfg.AddMaps(Assembly.GetAssembly(typeof(AlertMappings)));
            });
            _mapper = config.CreateMapper();
            _mockUserManager = new Mock<UserManager<User>>();
            _mockLogger = new Mock<ILogger<UserController>>();
            _mockOptions = new Mock<IOptions<CustomOptions>>();
            //todo populate values later.
            _mockOptions.Setup(op => op.Value).Returns(new CustomOptions());
        }
    }
}
