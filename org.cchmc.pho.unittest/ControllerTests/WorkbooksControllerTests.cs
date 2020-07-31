using System;
using System.Security.Claims;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Threading.Tasks;
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
using org.cchmc.pho.identity.Interfaces;
using org.cchmc.pho.identity.Models;

namespace org.cchmc.pho.unittest.ControllerTests
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class WorkbooksControllerTests
    {
        private WorkbooksController _workbooksController;
        private Mock<ILogger<WorkbooksController>> _mockLogger;
        private Mock<IUserService> _mockUserService;
        private Mock<IOptions<CustomOptions>> _mockOptions;
        private Mock<IWorkbooks> _mockWorkbooksDal;
        private IMapper _mapper;

        //Security moq objects
        private const string _userName = "bblackmore";
        private const string _password = "P@ssw0rd!";
        private const int _userId = 3;
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
                cfg.AddMaps(Assembly.GetAssembly(typeof(WorkbooksMappings)));
            });
            _mapper = config.CreateMapper();
            _mockUserService = new Mock<IUserService>();
            _mockWorkbooksDal = new Mock<IWorkbooks>();
            _mockLogger = new Mock<ILogger<WorkbooksController>>();
            _mockOptions = new Mock<IOptions<CustomOptions>>();
            //todo populate values later.
            _mockOptions.Setup(op => op.Value).Returns(new CustomOptions());

        }

        [TestMethod]
        public async Task ListPatients_Mapping_Success()
        {
            // setup  
            var formResponseId = 109;

            var myWorkbooksPatients = new List<core.DataModels.WorkbooksDepressionPatient>
            {
                new core.DataModels.WorkbooksDepressionPatient()
                {
                     FormResponseId = 109 ,
                     PatientId = 78835,
                     DOB = DateTime.Parse("2010-01-27"),
                     Phone = "5132536333",
                     Provider = "Bond",                     
                     DateOfService = DateTime.Parse("2020-03-03"),
                     PHQ9_Score = "10",
                     ActionFollowUp = true
                },
               new core.DataModels.WorkbooksDepressionPatient()
                {
                     FormResponseId = 109 ,
                     PatientId = 88835,
                     DOB = DateTime.Parse("2018-10-02"),
                     Phone = "5132536333",
                     Provider = "Smith",
                     DateOfService = DateTime.Parse("2020-01-10"),
                     PHQ9_Score = "12",
                     ActionFollowUp = true
                },
            };
            _mockUserService.Setup(p => p.GetUserIdFromClaims(It.IsAny<IEnumerable<Claim>>())).Returns(_userId).Verifiable();
            _mockWorkbooksDal.Setup(s => s.ListPatients(_userId,formResponseId)).Returns(Task.FromResult(myWorkbooksPatients)).Verifiable();
            _workbooksController = new WorkbooksController(_mockLogger.Object, _mockUserService.Object, _mapper, _mockWorkbooksDal.Object);

            // execute
            var result = await _workbooksController.ListPatients(109) as ObjectResult;
            var resultList = result.Value as List<WorkbooksDepressionPatientViewModel>;

            // assert
            Assert.AreEqual(myWorkbooksPatients[0].FormResponseId, resultList[0].FormResponseId);
            Assert.AreEqual(myWorkbooksPatients[0].PatientId, resultList[0].PatientId);
            Assert.AreEqual(myWorkbooksPatients[0].DOB, resultList[0].DOB);
            Assert.AreEqual(myWorkbooksPatients[0].Phone, resultList[0].Phone);
            Assert.AreEqual(myWorkbooksPatients[0].Provider, resultList[0].Provider);
            Assert.AreEqual(myWorkbooksPatients[0].DateOfService, resultList[0].DateOfService);
            Assert.AreEqual(myWorkbooksPatients[0].PHQ9_Score, resultList[0].PHQ9_Score);
            Assert.AreEqual(myWorkbooksPatients[0].ActionFollowUp, resultList[0].ActionFollowUp);          
        }

        [TestMethod]
        public async Task ListPatients_DataLayerThrowsException_ReturnsError()
        {
            // setup  
            var formResponseId = 109;

            _mockUserService.Setup(p => p.GetUserIdFromClaims(It.IsAny<IEnumerable<Claim>>())).Returns(_userId).Verifiable();
            _mockWorkbooksDal.Setup(s => s.ListPatients(_userId,formResponseId)).Throws(new Exception()).Verifiable();
            _workbooksController = new WorkbooksController(_mockLogger.Object, _mockUserService.Object, _mapper, _mockWorkbooksDal.Object);

            //execute
            var result = await _workbooksController.ListPatients(formResponseId) as ObjectResult;

            //assert
            Assert.AreEqual(500, result.StatusCode);
        }

        [TestMethod]
        public async Task GetPracticeWorkbooks_Success()
        {
            // setup  
            var formResponseId = 109;

            var myWorkbooksPractice = new WorkbooksPractice()
            {
                FormResponseId = 109,
                Header = "Depression WORKBOOK FOR 01/12/2015",
                Line1 = "The Depression Quality Initiative will be an ongoing quality improvement project that will be a step-wise approach.",
                Line2 = "For kids within age between 12 to 17 years old.",
                JobAidURL = "https://cchmc.sharepoint.com/",
                Line3 = "Patients had a PHQ-9 score of 10 or above"
            };

            _mockUserService.Setup(p => p.GetUserIdFromClaims(It.IsAny<IEnumerable<Claim>>())).Returns(_userId).Verifiable();
            _mockWorkbooksDal.Setup(s => s.GetPracticeWorkbooks(_userId, formResponseId)).Returns(Task.FromResult(myWorkbooksPractice)).Verifiable();
            _workbooksController = new WorkbooksController(_mockLogger.Object, _mockUserService.Object, _mapper, _mockWorkbooksDal.Object);

            // execute
            var result = await _workbooksController.GetPracticeWorkbooks(formResponseId) as ObjectResult;
            var resultList = result.Value as WorkbooksPracticeViewModel;

            // assert
            Assert.AreEqual(myWorkbooksPractice.FormResponseId, resultList.FormResponseId);
            Assert.AreEqual(myWorkbooksPractice.Header, resultList.Header);
            Assert.AreEqual(myWorkbooksPractice.Line1, resultList.Line1);
            Assert.AreEqual(myWorkbooksPractice.Line2, resultList.Line2);
            Assert.AreEqual(myWorkbooksPractice.JobAidURL, resultList.JobAidURL);
            Assert.AreEqual(myWorkbooksPractice.Line3, resultList.Line3);
        }

        [TestMethod]
        public async Task GetPracticeWorkbooksProviders_Success()
        {
            // setup  
            var formResponseId = 109;

            var workbooksproviders = new List<WorkbooksProvider>
            { 
                new WorkbooksProvider()
                {
                    FormResponseID = 109 ,
                    StaffID =  100 ,
                    Provider = "Smith",
                    PHQS = 14 ,
                    TOTAL =  20
                },

                new WorkbooksProvider()
                {
                    FormResponseID = 100 ,
                    StaffID =  88 ,
                    Provider = "Trump",
                    PHQS = 10 ,
                    TOTAL = 15
                },

                 new WorkbooksProvider()
                {
                    FormResponseID = 109,
                    StaffID =  365 ,
                    Provider = "Heaton",
                    PHQS = 8 ,
                    TOTAL = 12
                }
            };
            _mockUserService.Setup(p => p.GetUserIdFromClaims(It.IsAny<IEnumerable<Claim>>())).Returns(_userId).Verifiable();
            _mockWorkbooksDal.Setup(s => s.GetPracticeWorkbooksProviders(_userId, formResponseId)).Returns(Task.FromResult(workbooksproviders)).Verifiable();
            _workbooksController = new WorkbooksController(_mockLogger.Object, _mockUserService.Object, _mapper, _mockWorkbooksDal.Object);

            // execute
            var result = await _workbooksController.GetPracticeWorkbooksProviders(formResponseId) as ObjectResult;
            var resultList = result.Value as List<WorkbooksProviderViewModel>;

            // assert
            Assert.AreEqual(workbooksproviders[0].FormResponseID, resultList[0].FormResponseID);
            Assert.AreEqual(workbooksproviders[0].StaffID, resultList[0].StaffID);
            Assert.AreEqual(workbooksproviders[0].Provider, resultList[0].Provider);
            Assert.AreEqual(workbooksproviders[0].PHQS, resultList[0].PHQS);
            Assert.AreEqual(workbooksproviders[0].TOTAL, resultList[0].TOTAL);

            Assert.AreEqual(workbooksproviders[1].FormResponseID, resultList[1].FormResponseID);
            Assert.AreEqual(workbooksproviders[1].StaffID, resultList[1].StaffID);
            Assert.AreEqual(workbooksproviders[1].Provider, resultList[1].Provider);
            Assert.AreEqual(workbooksproviders[1].PHQS, resultList[1].PHQS);
            Assert.AreEqual(workbooksproviders[1].TOTAL, resultList[1].TOTAL);

            Assert.AreEqual(workbooksproviders[2].FormResponseID, resultList[2].FormResponseID);
            Assert.AreEqual(workbooksproviders[2].StaffID, resultList[2].StaffID);
            Assert.AreEqual(workbooksproviders[2].Provider, resultList[2].Provider);
            Assert.AreEqual(workbooksproviders[2].PHQS, resultList[2].PHQS);
            Assert.AreEqual(workbooksproviders[2].TOTAL, resultList[2].TOTAL);
        }       

        [TestMethod]
        public async Task AddPatientToWorkbooks_Success()
        {
            //set up        
            var patientId = 10809;
            bool expected = true;

            WorkbooksDepressionPatientViewModel selectedPatient = new WorkbooksDepressionPatientViewModel()
            {
                FormResponseId = 109,
                PatientId = 10809,
                DOB = DateTime.Parse("2018-03-27"),
                Phone = "5132536333",
                Provider = "Theiss",
                ProviderId = 302,
                Patient ="Test",
                DateOfService = DateTime.Parse("2020-03-10"),
                PHQ9_Score = "10",
                ActionFollowUp = true
            };

            _mockUserService.Setup(p => p.GetUserIdFromClaims(It.IsAny<IEnumerable<Claim>>())).Returns(_userId).Verifiable();
            _mockWorkbooksDal.Setup(s => s.AddPatientToWorkbooks(_userId, selectedPatient.FormResponseId, selectedPatient.PatientId, selectedPatient.ProviderId, selectedPatient.DateOfService, int.Parse(selectedPatient.PHQ9_Score), selectedPatient.ActionFollowUp))
                             .Returns(Task.FromResult(expected))
                             .Verifiable();
            _workbooksController = new WorkbooksController(_mockLogger.Object, _mockUserService.Object, _mapper, _mockWorkbooksDal.Object);

            // execute
            var result = await _workbooksController.AddPatientToWorkbooks(patientId, selectedPatient) as ObjectResult;
            var resultvalue = result.Value;

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expected, resultvalue);            
        }
    }
}
