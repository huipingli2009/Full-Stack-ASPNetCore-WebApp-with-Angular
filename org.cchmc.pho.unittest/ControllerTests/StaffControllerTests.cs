using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using org.cchmc.pho.api.Controllers;
using org.cchmc.pho.api.Mappings;
using org.cchmc.pho.api.ViewModels;
using org.cchmc.pho.core.DataModels;
using org.cchmc.pho.core.Interfaces;

namespace org.cchmc.pho.unittest.ControllerTests
{
    class StaffTests
    {
        private StaffController _StaffController;
        private Mock<ILogger<StaffController>> _mockLogger;
        private Mock<IStaff> _mockStaffDal;
        private IMapper _mapper;

        [TestInitialize]
        public void Initialize()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddMaps(Assembly.GetExecutingAssembly());
                cfg.AddMaps(Assembly.GetAssembly(typeof(StaffMappings)));
            });
            _mapper = config.CreateMapper();
            _mockStaffDal = new Mock<IStaff>();
            _mockLogger = new Mock<ILogger<StaffController>>();
        }


        [TestMethod]
        public async Task ListStaff_Mapping_Success()
        {
            // setup            
            var myStaff = new List<Staff>
            {
                new Staff()
                {
                    Id = 20101,
                    FirstName = "Carwood",
                    LastName = "Lipton",
                    Email = "cli@gmail.com",
                    Phone = "513-123-4567",
                    PositionId = 37,
                    CredentialId = 76,
                    Registry = false,
                    Responsibilities = "Practice Manager"
                },
                new Staff()
                {
                    Id = 20101,
                    FirstName = "Richard",
                    LastName = "Winters",
                    Email = "wintersr@yahoo.com",
                    Phone = "513-555-4567",
                    PositionId = 12,
                    CredentialId = 23,
                    Registry = true,
                    Responsibilities = "Auditor"
                }
        };

            _mockStaffDal.Setup(s => s.ListStaff(3,"","","")).Returns(Task.FromResult(myStaff)).Verifiable();
            _StaffController = new StaffController(_mockLogger.Object, _mapper, _mockStaffDal.Object);

            // execute
            var result = await _StaffController.ListStaff("", "", "") as ObjectResult;
            var resultList = result.Value as List<StaffViewModel>;

            // assert
            Assert.AreEqual(myStaff[0].Id, resultList[0].Id);
            Assert.AreEqual(myStaff[0].FirstName, resultList[0].FirstName);
            Assert.AreEqual(myStaff[0].LastName, resultList[0].LastName);
            Assert.AreEqual(myStaff[0].Email, resultList[0].Email);
            Assert.AreEqual(myStaff[0].Phone, resultList[0].Phone);
            Assert.AreEqual(myStaff[0].PositionId, resultList[0].PositionId);
            Assert.AreEqual(myStaff[0].CredentialId, resultList[0].CredentialId);
            Assert.AreEqual(myStaff[0].Registry, resultList[0].Registry);
            Assert.AreEqual(myStaff[0].Responsibilities, resultList[0].Responsibilities);
            Assert.AreEqual(myStaff[1].Id, resultList[1].Id);
            Assert.AreEqual(myStaff[1].FirstName, resultList[1].FirstName);
            Assert.AreEqual(myStaff[1].LastName, resultList[1].LastName);
            Assert.AreEqual(myStaff[1].Email, resultList[1].Email);
            Assert.AreEqual(myStaff[1].Phone, resultList[1].Phone);
            Assert.AreEqual(myStaff[1].PositionId, resultList[1].PositionId);
            Assert.AreEqual(myStaff[1].CredentialId, resultList[1].CredentialId);
            Assert.AreEqual(myStaff[1].Registry, resultList[1].Registry);
            Assert.AreEqual(myStaff[1].Responsibilities, resultList[1].Responsibilities);
        }

        [TestMethod]
        public async Task GetStaff_DataLayerThrowsException_ReturnsError()
        {
            // setup
            var userId = 3;
            _mockStaffDal.Setup(p => p.ListStaff(userId, "","","")).Throws(new Exception()).Verifiable();
            _StaffController = new StaffController(_mockLogger.Object, _mapper, _mockStaffDal.Object);

            // execute
            var result = await _StaffController.ListStaff("", "", "") as ObjectResult;

            // assert
            Assert.AreEqual(500, result.StatusCode);
        }


        [TestMethod]
        public async Task ListPositions_Mapping_Success()
        {
            // setup            
            var myPositions = new List<Position>
            {
                new Position()
                {
                    Id = 1,
                    Name = "Manager"
                },
                new Position()
                {

                    Id = 2,
                    Name = "Coffee Getter"
                }
            };

            _mockStaffDal.Setup(s => s.ListPositions()).Returns(Task.FromResult(myPositions)).Verifiable();
            _StaffController = new StaffController(_mockLogger.Object, _mapper, _mockStaffDal.Object);

            // execute
            var result = await _StaffController.ListPositions() as ObjectResult;
            var resultList = result.Value as List<Position>;

            // assert
            Assert.AreEqual(myPositions[0].Id, resultList[0].Id);
            Assert.AreEqual(myPositions[0].Name, resultList[0].Name);
            Assert.AreEqual(myPositions[1].Id, resultList[1].Id);
            Assert.AreEqual(myPositions[1].Name, resultList[1].Name);
        }

        [TestMethod]
        public async Task ListPositions_DataLayerThrowsException_ReturnsError()
        {
            // setup
            _mockStaffDal.Setup(p => p.ListPositions()).Throws(new Exception()).Verifiable();
            _StaffController = new StaffController(_mockLogger.Object, _mapper, _mockStaffDal.Object);

            // execute
            var result = await _StaffController.ListPositions() as ObjectResult;

            // assert
            Assert.AreEqual(500, result.StatusCode);
        }


        [TestMethod]
        public async Task ListCredentials_Mapping_Success()
        {
            // setup            
            var myCredentials = new List<Credential>
            {
                new Credential()
                {
                    Id = 1,
                    Name = "MD",
                    Description = "Medical Doctor"
                },
                new Credential()
                {

                    Id = 2,
                    Name = "RN",
                    Description = "Registered Nurse"
                }
            };

            _mockStaffDal.Setup(s => s.ListCredentials()).Returns(Task.FromResult(myCredentials)).Verifiable();
            _StaffController = new StaffController(_mockLogger.Object, _mapper, _mockStaffDal.Object);

            // execute
            var result = await _StaffController.ListCredentials() as ObjectResult;
            var resultList = result.Value as List<Credential>;

            // assert
            Assert.AreEqual(myCredentials[0].Id, resultList[0].Id);
            Assert.AreEqual(myCredentials[0].Name, resultList[0].Name);
            Assert.AreEqual(myCredentials[0].Description, resultList[0].Description);
            Assert.AreEqual(myCredentials[1].Id, resultList[1].Id);
            Assert.AreEqual(myCredentials[1].Name, resultList[1].Name);
            Assert.AreEqual(myCredentials[1].Description, resultList[1].Description);
        }

        [TestMethod]
        public async Task ListCredentials_DataLayerThrowsException_ReturnsError()
        {
            // setup
            _mockStaffDal.Setup(p => p.ListCredentials()).Throws(new Exception()).Verifiable();
            _StaffController = new StaffController(_mockLogger.Object, _mapper, _mockStaffDal.Object);

            // execute
            var result = await _StaffController.ListCredentials() as ObjectResult;

            // assert
            Assert.AreEqual(500, result.StatusCode);
        }


        [TestMethod]
        public async Task ListResponsibilities_Mapping_Success()
        {
            // setup            
            var myResponsibilities = new List<Responsibility>
            {
                new Responsibility()
                {
                    Id = 1,
                    Name = "F",
                    Type = "Fiscal"
                },
                new Responsibility()
                {

                    Id = 2,
                    Name = "S",
                    Type = "Social"
                }
            };

            _mockStaffDal.Setup(s => s.ListResponsibilities()).Returns(Task.FromResult(myResponsibilities)).Verifiable();
            _StaffController = new StaffController(_mockLogger.Object, _mapper, _mockStaffDal.Object);

            // execute
            var result = await _StaffController.ListResponsibilities() as ObjectResult;
            var resultList = result.Value as List<Responsibility>;

            // assert
            Assert.AreEqual(myResponsibilities[0].Id, resultList[0].Id);
            Assert.AreEqual(myResponsibilities[0].Name, resultList[0].Name);
            Assert.AreEqual(myResponsibilities[0].Type, resultList[0].Type);
            Assert.AreEqual(myResponsibilities[1].Id, resultList[1].Id);
            Assert.AreEqual(myResponsibilities[1].Name, resultList[1].Name);
            Assert.AreEqual(myResponsibilities[1].Type, resultList[1].Type);
        }

        [TestMethod]
        public async Task ListResponsibilities_DataLayerThrowsException_ReturnsError()
        {
            // setup
            _mockStaffDal.Setup(p => p.ListResponsibilities()).Throws(new Exception()).Verifiable();
            _StaffController = new StaffController(_mockLogger.Object, _mapper, _mockStaffDal.Object);

            // execute
            var result = await _StaffController.ListResponsibilities() as ObjectResult;

            // assert
            Assert.AreEqual(500, result.StatusCode);
        }
    }
}
