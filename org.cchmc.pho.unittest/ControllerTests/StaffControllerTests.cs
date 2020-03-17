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

namespace org.cchmc.pho.unittest.controllertests
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class StaffControllerTests
    {
        private StaffController _staffController;
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
                    IsRegistry = false,
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
                    IsRegistry = true,
                    Responsibilities = "Auditor"
                }
        };

            _mockStaffDal.Setup(s => s.ListStaff(3,"","","")).Returns(Task.FromResult(myStaff)).Verifiable();
            _staffController = new StaffController(_mockLogger.Object, _mapper, _mockStaffDal.Object);

            // execute
            var result = await _staffController.ListStaff("", "", "") as ObjectResult;
            var resultList = result.Value as List<StaffViewModel>;

            // assert
            Assert.AreEqual(myStaff[0].Id, resultList[0].Id);
            Assert.AreEqual(myStaff[0].FirstName, resultList[0].FirstName);
            Assert.AreEqual(myStaff[0].LastName, resultList[0].LastName);
            Assert.AreEqual(myStaff[0].Email, resultList[0].Email);
            Assert.AreEqual(myStaff[0].Phone, resultList[0].Phone);
            Assert.AreEqual(myStaff[0].PositionId, resultList[0].PositionId);
            Assert.AreEqual(myStaff[0].CredentialId, resultList[0].CredentialId);
            Assert.AreEqual(myStaff[0].IsRegistry, resultList[0].IsRegistry);
            Assert.AreEqual(myStaff[0].Responsibilities, resultList[0].Responsibilities);
            Assert.AreEqual(myStaff[1].Id, resultList[1].Id);
            Assert.AreEqual(myStaff[1].FirstName, resultList[1].FirstName);
            Assert.AreEqual(myStaff[1].LastName, resultList[1].LastName);
            Assert.AreEqual(myStaff[1].Email, resultList[1].Email);
            Assert.AreEqual(myStaff[1].Phone, resultList[1].Phone);
            Assert.AreEqual(myStaff[1].PositionId, resultList[1].PositionId);
            Assert.AreEqual(myStaff[1].CredentialId, resultList[1].CredentialId);
            Assert.AreEqual(myStaff[1].IsRegistry, resultList[1].IsRegistry);
            Assert.AreEqual(myStaff[1].Responsibilities, resultList[1].Responsibilities);
        }

        [TestMethod]
        public async Task ListStaff_DataLayerThrowsException_ReturnsError()
        {
            // setup
            var userId = 3;
            _mockStaffDal.Setup(p => p.ListStaff(userId, "","","")).Throws(new Exception()).Verifiable();
            _staffController = new StaffController(_mockLogger.Object, _mapper, _mockStaffDal.Object);

            // execute
            var result = await _staffController.ListStaff("", "", "") as ObjectResult;

            // assert
            Assert.AreEqual(500, result.StatusCode);
        }


        [TestMethod]
        public async Task GetStaffDetails_Mapping_Success()
        {
            // setup            
            var myStaff = new StaffDetail()
            {
                Id = 20101,
                FirstName = "Carwood",
                LastName = "Lipton",
                Email = "cli@gmail.com",
                Phone = "513-123-4567",
                StartDate = Convert.ToDateTime("12-30-2011 12:00:00 AM"),
                PositionId = 37,
                CredentialId = 76,
                IsLeadPhysician = false,
                IsQITeam = true,
                IsPracticeManager = false,
                IsInterventionContact = true,
                IsQPLLeader = false,
                IsPHOBoard = false,
                IsOVPCABoard = false,
                IsRVPIBoard = false,
            };

            _mockStaffDal.Setup(s => s.GetStaffDetails(3, 20101)).Returns(Task.FromResult(myStaff)).Verifiable();
            _staffController = new StaffController(_mockLogger.Object, _mapper, _mockStaffDal.Object);

            // execute
            var result = await _staffController.GetStaffDetails("20101") as ObjectResult;
            var resultList = result.Value as StaffDetailViewModel;

            // assert
            Assert.AreEqual(myStaff.Id, resultList.Id);
            Assert.AreEqual(myStaff.FirstName, resultList.FirstName);
            Assert.AreEqual(myStaff.LastName, resultList.LastName);
            Assert.AreEqual(myStaff.Email, resultList.Email);
            Assert.AreEqual(myStaff.Phone, resultList.Phone);
            Assert.AreEqual(myStaff.PositionId, resultList.PositionId);
            Assert.AreEqual(myStaff.CredentialId, resultList.CredentialId);
            Assert.AreEqual(myStaff.IsLeadPhysician, resultList.IsLeadPhysician);
            Assert.AreEqual(myStaff.IsQITeam, resultList.IsQITeam);
            Assert.AreEqual(myStaff.IsPracticeManager, resultList.IsPracticeManager);
            Assert.AreEqual(myStaff.IsInterventionContact, resultList.IsInterventionContact);
            Assert.AreEqual(myStaff.IsQPLLeader, resultList.IsQPLLeader);
            Assert.AreEqual(myStaff.IsPHOBoard, resultList.IsPHOBoard);
            Assert.AreEqual(myStaff.IsOVPCABoard, resultList.IsOVPCABoard);
            Assert.AreEqual(myStaff.IsRVPIBoard, resultList.IsRVPIBoard);
        }

        [TestMethod]
        public async Task GetStaffDetails_DataLayerThrowsException_ReturnsError()
        {
            // setup
            var userId = 3;
            _mockStaffDal.Setup(s => s.GetStaffDetails(userId, 20101)).Throws(new Exception()).Verifiable();
            _staffController = new StaffController(_mockLogger.Object, _mapper, _mockStaffDal.Object);

            // execute
            var result = await _staffController.GetStaffDetails("20101") as ObjectResult;

            // assert
            Assert.AreEqual(500, result.StatusCode);
        }

        [TestMethod]
        public async Task UpdateStaffDetails_Success()
        {
            // setup
            var userId = 3;
            var myStaff = new StaffDetail()
            {
                Id = 20101,
                FirstName = "Carwood",
                LastName = "Lipton",
                Email = "cli@gmail.com",
                Phone = "513-123-4567",
                StartDate = Convert.ToDateTime("12-30-2011 12:00:00 AM"),
                PositionId = 37,
                CredentialId = 76,
                IsLeadPhysician = false,
                IsQITeam = true,
                IsPracticeManager = false,
                IsInterventionContact = true,
                IsQPLLeader = false,
                IsPHOBoard = false,
                IsOVPCABoard = false,
                IsRVPIBoard = false,
            };

            _mockStaffDal.Setup(p => p.UpdateStaffDetails(userId, myStaff))
                .Returns(Task.FromResult(myStaff)).Verifiable();
            _mockStaffDal.Setup(p => p.IsStaffInSamePractice(It.IsAny<Int32>(), It.IsAny<Int32>())).Returns(true);
            _staffController = new StaffController(_mockLogger.Object, _mapper, _mockStaffDal.Object);

            // execute
            var result = await _staffController.UpdateStaffDetails(_mapper.Map<StaffDetailViewModel>(myStaff), "20101");

            // assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Assert.AreEqual(200, okResult.StatusCode);
        }

        [TestMethod]
        public async Task UpdateStaffDetails_StaffDetailIsNull_Throws400()
        {
            // setup
            var staffId = "1";
            _staffController = new StaffController(_mockLogger.Object, _mapper, _mockStaffDal.Object);

            // execute
            var result = await _staffController.UpdateStaffDetails(null, staffId) as ObjectResult;

            // assert
            Assert.AreEqual(400, result.StatusCode);
            Assert.AreEqual("staff is null", result.Value);
            _mockStaffDal.Verify(p => p.GetStaffDetails(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        }

        [TestMethod]
        public async Task UpdateStaffDetails_StaffDetailDoesNotMatch_Throws400()
        {
            // setup
            var staffId = "1";
            _staffController = new StaffController(_mockLogger.Object, _mapper, _mockStaffDal.Object);

            // execute
            var result = await _staffController.UpdateStaffDetails(new StaffDetailViewModel(), staffId) as ObjectResult;

            // assert
            Assert.AreEqual(400, result.StatusCode);
            Assert.AreEqual("staff id does not match", result.Value);
            _mockStaffDal.Verify(p => p.GetStaffDetails(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        }

        [TestMethod]
        public async Task UpdateStaffDetails_PatientAndUserNotSamePractice_Throws400()
        {
            // setup
            var staffId = "20101";

            var myStaff = new StaffDetailViewModel()
            {
                Id = 20101,
                FirstName = "Carwood",
                LastName = "Lipton",
                Email = "cli@gmail.com",
                Phone = "513-123-4567",
                StartDate = Convert.ToDateTime("12-30-2011 12:00:00 AM"),
                PositionId = 37,
                CredentialId = 76,
                IsLeadPhysician = false,
                IsQITeam = true,
                IsPracticeManager = false,
                IsInterventionContact = true,
                IsQPLLeader = false,
                IsPHOBoard = false,
                IsOVPCABoard = false,
                IsRVPIBoard = false,
            };

            _staffController = new StaffController(_mockLogger.Object, _mapper, _mockStaffDal.Object);
            _mockStaffDal.Setup(p => p.IsStaffInSamePractice(It.IsAny<Int32>(), It.IsAny<Int32>())).Returns(false);

            // execute
            var result = await _staffController.UpdateStaffDetails(myStaff, staffId) as ObjectResult;

            // assert
            Assert.AreEqual(400, result.StatusCode);
            Assert.AreEqual("staff practice does not match user", result.Value);
            _mockStaffDal.Verify(p => p.GetStaffDetails(It.IsAny<int>(), It.IsAny<Int32>()), Times.Never);
        }

        [TestMethod]
        public async Task UpdateStaffDetails_DataLayerThrowsException_ReturnsError()
        {
            // setup
            var userId = 3; //todo update to match the hardcoded
            var myStaff = new StaffDetail()
            {
                Id = 20101,
                FirstName = "Carwood",
                LastName = "Lipton",
                Email = "cli@gmail.com",
                Phone = "513-123-4567",
                StartDate = Convert.ToDateTime("12-30-2011 12:00:00 AM"),
                PositionId = 37,
                CredentialId = 76,
                IsLeadPhysician = false,
                IsQITeam = true,
                IsPracticeManager = false,
                IsInterventionContact = true,
                IsQPLLeader = false,
                IsPHOBoard = false,
                IsOVPCABoard = false,
                IsRVPIBoard = false,
            };

            _mockStaffDal.Setup(p => p.UpdateStaffDetails(userId, It.IsAny<StaffDetail>())).Throws(new Exception());
            _mockStaffDal.Setup(p => p.IsStaffInSamePractice(It.IsAny<Int32>(), It.IsAny<Int32>())).Returns(true);
            _staffController = new StaffController(_mockLogger.Object, _mapper, _mockStaffDal.Object);
            

            // execute
            var result = await _staffController.UpdateStaffDetails(new StaffDetailViewModel() 
            {
                Id = 20101,
                FirstName = "Carwood",
                LastName = "Lipton",
                Email = "cli@gmail.com",
                Phone = "513-123-4567",
                StartDate = Convert.ToDateTime("12-30-2011 12:00:00 AM"),
                PositionId = 37,
                CredentialId = 76,
                IsLeadPhysician = false,
                IsQITeam = true,
                IsPracticeManager = false,
                IsInterventionContact = true,
                IsQPLLeader = false,
                IsPHOBoard = false,
                IsOVPCABoard = false,
                IsRVPIBoard = false,
            }
            , "20101"
            ) as ObjectResult;


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
            _staffController = new StaffController(_mockLogger.Object, _mapper, _mockStaffDal.Object);

            // execute
            var result = await _staffController.ListPositions() as ObjectResult;
            var resultList = result.Value as List<PositionViewModel>;

            // assert
            Assert.AreEqual(2, resultList.Count);
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
            _staffController = new StaffController(_mockLogger.Object, _mapper, _mockStaffDal.Object);

            // execute
            var result = await _staffController.ListPositions() as ObjectResult;

            // assert

            Assert.AreEqual(500, result.StatusCode);
        }


        [TestMethod]
        public async Task ListCredentials_Mapping_Success()
        {
            // setup            
            var myCredentials = new List<Credential>()
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
            _staffController = new StaffController(_mockLogger.Object, _mapper, _mockStaffDal.Object);

            // execute
            var result = await _staffController.ListCredentials() as ObjectResult;
            var resultList = result.Value as List<CredentialViewModel>;

            // assert
            Assert.AreEqual(2, resultList.Count);
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
            _staffController = new StaffController(_mockLogger.Object, _mapper, _mockStaffDal.Object);

            // execute
            var result = await _staffController.ListCredentials() as ObjectResult;

            // assert
            Assert.AreEqual(500, result.StatusCode);
        }


        [TestMethod]
        public async Task ListResponsibilities_Mapping_Success()
        {
            // setup            
            var myResponsibilities = new List<Responsibility>()
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
            _staffController = new StaffController(_mockLogger.Object, _mapper, _mockStaffDal.Object);

            // execute
            var result = await _staffController.ListResponsibilities() as ObjectResult;
            var resultList = result.Value as List<ResponsibilityViewModel>;

            // assert
            Assert.AreEqual(2, resultList.Count);
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
            _staffController = new StaffController(_mockLogger.Object, _mapper, _mockStaffDal.Object);

            // execute
            var result = await _staffController.ListResponsibilities() as ObjectResult;

            // assert
            Assert.AreEqual(500, result.StatusCode);
        }
    }
}
