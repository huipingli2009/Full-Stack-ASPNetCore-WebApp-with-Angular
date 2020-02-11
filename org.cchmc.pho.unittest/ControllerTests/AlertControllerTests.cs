using AutoMapper;
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
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Threading.Tasks;

namespace org.cchmc.pho.unittest.controllertests
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class AlertControllerTests
    {
        private AlertController _alertController;
        private Mock<ILogger<AlertController>> _mockLogger;
        private Mock<IAlert> _mockAlertDal;
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
            _mockAlertDal = new Mock<IAlert>();
            _mockLogger = new Mock<ILogger<AlertController>>();
            _mockOptions = new Mock<IOptions<CustomOptions>>();
            //todo populate values later.
            _mockOptions.Setup(op => op.Value).Returns(new CustomOptions());
            
        }

        [TestMethod]
        public async Task ListActiveAlert_Mapping_Success()
        {
            // setup
            var userId = 5;
            var myAlerts = new List<Alert>()
            {
                new Alert()
                {
                    AlertId = 1,
                    AlertScheduleId = 7,
                    Definition = "MyDefinition",
                    LinkText = "SomeLinks",
                    Message = "A Message",
                    Url = "http://www.example.com"
                },
                new Alert()
                {
                    AlertId = 3,
                    AlertScheduleId = 2,
                    Definition = "MyDefinition2",
                    LinkText = "SomeLinksasdf",
                    Message = "Aasdf Message",
                    Url = "http://www.example2.com"
                }
            };
            _mockAlertDal.Setup(p => p.ListActiveAlerts(userId)).Returns(Task.FromResult(myAlerts)).Verifiable();
            _alertController = new AlertController(_mockLogger.Object, _mapper, _mockAlertDal.Object, _mockOptions.Object);

            // execute
            var result = await _alertController.ListActiveAlerts(userId.ToString()) as ObjectResult;
            var resultList = result.Value as List<AlertViewModel>;

            // assert
            Assert.AreEqual(2, resultList.Count);
            Assert.AreEqual(myAlerts[0].AlertId, resultList[0].AlertId);
            Assert.AreEqual(myAlerts[0].AlertScheduleId, resultList[0].AlertScheduleId);
            Assert.AreEqual(myAlerts[0].Definition, resultList[0].Definition);
            Assert.AreEqual(myAlerts[0].LinkText, resultList[0].LinkText);
            Assert.AreEqual(myAlerts[0].Message, resultList[0].Message);
            Assert.AreEqual(myAlerts[0].Url, resultList[0].Url);
            Assert.AreEqual(myAlerts[1].AlertId, resultList[1].AlertId);
            Assert.AreEqual(myAlerts[1].AlertScheduleId, resultList[1].AlertScheduleId);
            Assert.AreEqual(myAlerts[1].Definition, resultList[1].Definition);
            Assert.AreEqual(myAlerts[1].LinkText, resultList[1].LinkText);
            Assert.AreEqual(myAlerts[1].Message, resultList[1].Message);
            Assert.AreEqual(myAlerts[1].Url, resultList[1].Url);

        }

        [TestMethod]
        public async Task ListActiveAlert_UserIdDoesNotValidate_Throws400()
        {
            // setup
            var userId = "asdf";
            _alertController = new AlertController(_mockLogger.Object, _mapper, _mockAlertDal.Object, _mockOptions.Object);

            // execute
            var result = await _alertController.ListActiveAlerts(userId) as ObjectResult;

            // assert
            Assert.AreEqual(400, result.StatusCode);
            Assert.AreEqual("user is not a valid integer", result.Value);
            _mockAlertDal.Verify(p => p.ListActiveAlerts(It.IsAny<int>()), Times.Never);
        }

        [TestMethod]
        public async Task ListActiveAlert_DataLayerThrowsException_ReturnsError()
        {
            // setup
            var userId = 5;
            _mockAlertDal.Setup(p => p.ListActiveAlerts(userId)).Throws(new Exception()).Verifiable();
            _alertController = new AlertController(_mockLogger.Object, _mapper, _mockAlertDal.Object, _mockOptions.Object);

            // execute
            var result = await _alertController.ListActiveAlerts(userId.ToString()) as ObjectResult;

            // assert
            Assert.AreEqual(500, result.StatusCode);
        }

        [TestMethod]
        public async Task MarkAlertAction_Success()
        {
            // setup
            var userId = 5;
            var alertSchedule = 7;
            var alertActionId = 9;
            var alertDateTime = DateTime.Parse("1/19/20");
            
            _mockAlertDal.Setup(p => p.MarkAlertAction(userId, alertSchedule, alertActionId, alertDateTime))
                .Returns(Task.CompletedTask).Verifiable();
            _alertController = new AlertController(_mockLogger.Object, _mapper, _mockAlertDal.Object, _mockOptions.Object);

            // execute
            var result = await _alertController.MarkAlertAction(userId.ToString(), alertSchedule.ToString(), new AlertActionViewModel()
            {
                ActionDateTime = alertDateTime,
                AlertActionId = alertActionId
            });

            // assert
            Assert.IsInstanceOfType(result, typeof(OkResult));
            var okResult = result as OkResult;
            Assert.AreEqual(200, okResult.StatusCode);
        }

        [TestMethod]
        public async Task MarkAlertAction_UserIdDoesNotValidate_Throws400()
        {
            // setup
            var userId = "asdf";
            var alertSchedule = 7;
            var alertActionId = 9;
            var alertDateTime = DateTime.Parse("1/19/20");

            _alertController = new AlertController(_mockLogger.Object, _mapper, _mockAlertDal.Object, _mockOptions.Object);

            // execute
            var result = await _alertController.MarkAlertAction(userId.ToString(), alertSchedule.ToString(), new AlertActionViewModel()
            {
                ActionDateTime = alertDateTime,
                AlertActionId = alertActionId
            }) as ObjectResult;

            // assert
            Assert.AreEqual(400, result.StatusCode);
            Assert.AreEqual("user is not a valid integer", result.Value);
            _mockAlertDal.Verify(p => p.MarkAlertAction(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTime>()), Times.Never);
        }

        [TestMethod]
        public async Task MarkAlertAction_AlertScheduleDoesNotValidate_Throws400()
        {
            // setup
            var userId = 5;
            var alertSchedule = "asdf";
            var alertActionId = 9;
            var alertDateTime = DateTime.Parse("1/19/20");

            _alertController = new AlertController(_mockLogger.Object, _mapper, _mockAlertDal.Object, _mockOptions.Object);

            // execute
            var result = await _alertController.MarkAlertAction(userId.ToString(), alertSchedule.ToString(), new AlertActionViewModel()
            {
                ActionDateTime = alertDateTime,
                AlertActionId = alertActionId
            }) as ObjectResult;

            // assert
            Assert.AreEqual(400, result.StatusCode);
            Assert.AreEqual("alertSchedule is not a valid integer", result.Value);
            _mockAlertDal.Verify(p => p.MarkAlertAction(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTime>()), Times.Never);
        }

        [TestMethod]
        public async Task MarkAlertAction_DataLayerThrowsException_ReturnsError()
        {
            // setup
            var userId = 5;
            var alertSchedule = 7;
            var alertActionId = 9;
            var alertDateTime = DateTime.Parse("1/19/20");

            _mockAlertDal.Setup(p => p.MarkAlertAction(userId, alertSchedule, alertActionId, alertDateTime))
                .Throws(new Exception()).Verifiable();
            _alertController = new AlertController(_mockLogger.Object, _mapper, _mockAlertDal.Object, _mockOptions.Object);

            // execute
            var result = await _alertController.MarkAlertAction(userId.ToString(), alertSchedule.ToString(), new AlertActionViewModel()
            {
                ActionDateTime = alertDateTime,
                AlertActionId = alertActionId
            }) as ObjectResult;

            // assert
            Assert.AreEqual(500, result.StatusCode);
        }
    }
}
