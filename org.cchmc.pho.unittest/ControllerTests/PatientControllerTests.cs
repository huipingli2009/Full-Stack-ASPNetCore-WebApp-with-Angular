using System;
using System.Security.Claims;
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
using org.cchmc.pho.identity.Interfaces;
using org.cchmc.pho.identity.Models;

namespace org.cchmc.pho.unittest.ControllerTests
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class PatientControllerTests
    {
        private PatientsController _PatientController;
        private Mock<ILogger<PatientsController>> _mockLogger;
        private Mock<IUserService> _mockUserService;
        private Mock<IPatient> _mockPatientDal;
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
                cfg.AddMaps(Assembly.GetAssembly(typeof(PatientMappings)));
            });
            _mapper = config.CreateMapper();
            _mockUserService = new Mock<IUserService>();
            _mockPatientDal = new Mock<IPatient>();
            _mockLogger = new Mock<ILogger<PatientsController>>();
        }


        [TestMethod]
        public async Task GetPatientDetails_Mapping_Success()
        {
            // setup
            var patientId = 5000;
            bool potentialPatient = false;

            List<PatientCondition> conditions = new List<PatientCondition>();
            conditions.Add(new PatientCondition(1, "Asthma"));
            conditions.Add(new PatientCondition(2, "Depression"));

            var myPatientDetails = new PatientDetails()
            {
                Id = 20101,
                PatientMRNId = "01302109",
                PatId = "Z667259",
                PracticeId = 37,
                FirstName = "Ryan",
                MiddleName = "",
                LastName = "Abbott",
                PatientDOB = Convert.ToDateTime("2004-08-25T00:00:00"),
                PCPId = 602,
                PCPFirstName = "Nancy",
                PCPLastName = "Young",
                InsuranceId = 2038,
                InsuranceName = "UMR",
                AddressLine1 = "5613 Glenbrook Ct",
                AddressLine2 = "",
                City = "Mason",
                State = "OH",
                Zip = "45040",
                Conditions = conditions,
                PMCAScore = 0,
                ProviderPMCAScore = 2,
                ProviderNotes = "Ryan needs more care. Just updated",
                ActiveStatus = true,
                PotentiallyActiveStatus = true,
                GenderId = 1,
                Gender = "M",
                Email = "",
                Phone1 = "5137700902",
                Phone2 = "",
                PracticeVisits = 28,
                CCHMCEncounters = 15,
                HealthBridgeEncounters = 0,
                UniqueDXs = 0,
                UniqueCPTCodes = 23,
                LastPracticeVisit = Convert.ToDateTime("2018-09-11T00:00:00"),
                LastCCHMCAdmit = Convert.ToDateTime("2018-09-12T00:00:00"),
                LastHealthBridgeAdmit = Convert.ToDateTime("2016-01-28T21:04:00"),
                LastDiagnosis = "",
                NextCCHMCAppointment = Convert.ToDateTime("2016-02-03T19:00:00")
            };


            _mockUserService.Setup(p => p.GetUserIdFromClaims(It.IsAny<IEnumerable<Claim>>())).Returns(_userId).Verifiable();
            _mockPatientDal.Setup(p => p.GetPatientDetails(_userId, patientId, false)).Returns(Task.FromResult(myPatientDetails)).Verifiable();
            _PatientController = new PatientsController(_mockLogger.Object, _mockUserService.Object, _mapper, _mockPatientDal.Object);

            // execute
            var result = await _PatientController.GetPatientDetails(patientId.ToString(), potentialPatient) as ObjectResult;
            var resultList = result.Value as PatientDetailsViewModel;

            // assert
            Assert.AreEqual(myPatientDetails.Id, resultList.Id);
            Assert.AreEqual(myPatientDetails.PatientMRNId, resultList.PatientMRNId);
            Assert.AreEqual(myPatientDetails.PatId, resultList.ClarityPatientId);
            Assert.AreEqual(myPatientDetails.PracticeId, resultList.PracticeId);
            Assert.AreEqual(myPatientDetails.FirstName, resultList.FirstName);
            Assert.AreEqual(myPatientDetails.MiddleName, resultList.MiddleName);
            Assert.AreEqual(myPatientDetails.LastName, resultList.LastName);
            Assert.AreEqual(myPatientDetails.PatientDOB, resultList.PatientDOB);
            Assert.AreEqual(myPatientDetails.PCPId, resultList.PCPId);
            Assert.AreEqual(myPatientDetails.PCPFirstName, resultList.PCPFirstName);
            Assert.AreEqual(myPatientDetails.PCPLastName, resultList.PCPLastName);
            Assert.AreEqual(myPatientDetails.InsuranceId, resultList.InsuranceId);
            Assert.AreEqual(myPatientDetails.InsuranceName, resultList.InsuranceName);
            Assert.AreEqual(myPatientDetails.AddressLine1, resultList.AddressLine1);
            Assert.AreEqual(myPatientDetails.AddressLine2, resultList.AddressLine2);
            Assert.AreEqual(myPatientDetails.City, resultList.City);
            Assert.AreEqual(myPatientDetails.State, resultList.State);
            Assert.AreEqual(myPatientDetails.Zip, resultList.Zip);
            Assert.AreEqual(myPatientDetails.PMCAScore, resultList.PMCAScore);
            Assert.AreEqual(myPatientDetails.ProviderPMCAScore, resultList.ProviderPMCAScore);
            Assert.AreEqual(myPatientDetails.ProviderNotes, resultList.ProviderNotes);

            Assert.IsNotNull(myPatientDetails.ActiveStatus);
            Assert.IsNotNull(resultList.ActiveStatus);
            Assert.AreEqual(myPatientDetails.ActiveStatus, resultList.ActiveStatus);

            CompareConditionMapping(myPatientDetails.Conditions, resultList.Conditions);

            Assert.AreEqual(myPatientDetails.PotentiallyActiveStatus, resultList.PendingStatusConfirmation);
            Assert.AreEqual(myPatientDetails.GenderId, resultList.GenderId);
            Assert.AreEqual(myPatientDetails.Gender, resultList.Gender);
            Assert.AreEqual(myPatientDetails.Email, resultList.Email);
            Assert.AreEqual(myPatientDetails.Phone1, resultList.Phone1);
            Assert.AreEqual(myPatientDetails.Phone2, resultList.Phone2);
            Assert.AreEqual(myPatientDetails.PracticeVisits, resultList.PracticeVisits);
            Assert.AreEqual(myPatientDetails.HealthBridgeEncounters, resultList.HealthBridgeEncounters);
            Assert.AreEqual(myPatientDetails.UniqueDXs, resultList.UniqueDXs);
            Assert.AreEqual(myPatientDetails.UniqueCPTCodes, resultList.UniqueCPTCodes);
            Assert.AreEqual(myPatientDetails.LastPracticeVisit, resultList.LastPracticeVisit);
            Assert.AreEqual(myPatientDetails.LastCCHMCAdmit, resultList.LastCCHMCAdmit);
            Assert.AreEqual(myPatientDetails.LastHealthBridgeAdmit, resultList.LastHealthBridgeAdmit);
            Assert.AreEqual(myPatientDetails.LastDiagnosis, resultList.LastDiagnosis);
            Assert.AreEqual(myPatientDetails.NextCCHMCAppointment, resultList.NextCCHMCAppointment);
        }

        [TestMethod]
        public async Task GetPatientDetails_UserIdDoesNotValidate_Throws400()
        {
            // setup
            var patientId = "asdf";
            bool potentialPatient = false;
            _PatientController = new PatientsController(_mockLogger.Object, _mockUserService.Object, _mapper, _mockPatientDal.Object);

            // execute
            var result = await _PatientController.GetPatientDetails(patientId, potentialPatient) as ObjectResult;

            // assert
            Assert.AreEqual(400, result.StatusCode);
            Assert.AreEqual("patient is not a valid integer", result.Value);
            _mockPatientDal.Verify(p => p.GetPatientDetails(It.IsAny<int>(), It.IsAny<int>(), false), Times.Never);
        }

      
        [TestMethod]
        public async Task GetPatientDetails_DataLayerThrowsException_ReturnsError()
        {
            // setup
            var patientId = 5;
            bool potentialPatient = false;

            _mockUserService.Setup(p => p.GetUserIdFromClaims(It.IsAny<IEnumerable<Claim>>())).Returns(_userId).Verifiable();
            _mockPatientDal.Setup(p => p.GetPatientDetails(_userId, patientId, potentialPatient)).Throws(new Exception()).Verifiable();
            _PatientController = new PatientsController(_mockLogger.Object, _mockUserService.Object, _mapper, _mockPatientDal.Object);

            // execute
            var result = await _PatientController.GetPatientDetails(patientId.ToString(), false) as ObjectResult;

            // assert
            Assert.AreEqual(500, result.StatusCode);
        }

        private void CompareConditionMapping(List<PatientCondition> data, List<PatientConditionViewModel> view)
        {
            Assert.IsNotNull(data);
            Assert.IsNotNull(view);
            Assert.AreEqual(data.Count, view.Count);

            for (int i = 0; i < data.Count; i++)
            {
                Assert.AreEqual(data[i].ID, view[i].ID);
                Assert.AreEqual(data[i].Name, view[i].Name);
            }

        }


        [TestMethod]
        public async Task UpdatePatientDetails_Success()
        {
            // setup
            var userId = 3;

            List<PatientCondition> conditions = new List<PatientCondition>();
            conditions.Add(new PatientCondition(1, "Asthma"));
            conditions.Add(new PatientCondition(2, "Depression"));

            var myPatientDetails = new PatientDetails()
            {
                Id = 20101,
                PatientMRNId = "01302109",
                PatId = "Z667259",
                PracticeId = 37,
                FirstName = "Ryan",
                MiddleName = "",
                LastName = "Abbott",
                PatientDOB = Convert.ToDateTime("2004-08-25T00:00:00"),
                PCPId = 602,
                PCPFirstName = "Nancy",
                PCPLastName = "Young",
                InsuranceId = 2038,
                InsuranceName = "UMR",
                AddressLine1 = "5613 Glenbrook Ct",
                AddressLine2 = "",
                City = "Mason",
                State = "OH",
                Zip = "45040",
                Conditions = conditions,
                PMCAScore = 0,
                ProviderPMCAScore = 2,
                ProviderNotes = "Ryan needs more care. Just updated",
                ActiveStatus = true,
                PotentiallyActiveStatus = true,
                GenderId = 1,
                Gender = "M",
                Email = "",
                Phone1 = "5137700902",
                Phone2 = "",
                PracticeVisits = 28,
                CCHMCEncounters = 15,
                HealthBridgeEncounters = 0,
                UniqueDXs = 0,
                UniqueCPTCodes = 23,
                LastPracticeVisit = Convert.ToDateTime("2018-09-11T00:00:00"),
                LastCCHMCAdmit = Convert.ToDateTime("2018-09-12T00:00:00"),
                LastHealthBridgeAdmit = Convert.ToDateTime("2016-01-28T21:04:00"),
                LastDiagnosis = "",
                NextCCHMCAppointment = Convert.ToDateTime("2016-02-03T19:00:00")
            };

            _mockPatientDal.Setup(p => p.UpdatePatientDetails(userId, myPatientDetails))
                .Returns(Task.FromResult(myPatientDetails)).Verifiable();
            _mockPatientDal.Setup(p => p.IsPatientInSamePractice(It.IsAny<Int32>(), It.IsAny<Int32>())).Returns(true);
            _PatientController = new PatientsController(_mockLogger.Object, _mockUserService.Object, _mapper, _mockPatientDal.Object);

            // execute
            var result = await _PatientController.UpdatePatientDetails(_mapper.Map<PatientDetailsViewModel>(myPatientDetails), "20101");

            // assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Assert.AreEqual(200, okResult.StatusCode);
        }

        [TestMethod]
        public async Task UpdatePatientDetails_PatientDetailIsNull_Throws400()
        {
            // setup
            var patientId = "1";
            //var potentialPatient = false;

            _PatientController = new PatientsController(_mockLogger.Object, _mockUserService.Object, _mapper, _mockPatientDal.Object);

            // execute
            var result = await _PatientController.UpdatePatientDetails(null, patientId) as ObjectResult;

            // assert
            Assert.AreEqual(400, result.StatusCode);
            Assert.AreEqual("patient is null", result.Value);
            _mockPatientDal.Verify(p => p.GetPatientDetails(It.IsAny<int>(), It.IsAny<int>(), false), Times.Never);
        }

        [TestMethod]
        public async Task UpdatePatientDetails_PatientDetailDoesNotMatch_Throws400()
        {
            // setup
            var patientId = "1";
            _PatientController = new PatientsController(_mockLogger.Object, _mockUserService.Object, _mapper, _mockPatientDal.Object);

            // execute
            var result = await _PatientController.UpdatePatientDetails(new PatientDetailsViewModel(), patientId) as ObjectResult;

            // assert
            Assert.AreEqual(400, result.StatusCode);
            Assert.AreEqual("patient id does not match", result.Value);
            _mockPatientDal.Verify(p => p.GetPatientDetails(It.IsAny<int>(), It.IsAny<int>(), false), Times.Never);
        }

        [TestMethod]
        public async Task UpdatePatientDetails_PatientAndUserNotSamePractice_Throws400()
        {
            // setup
            var patientId = "20101";
            
            var myPatientDetails = new PatientDetailsViewModel()
            {
                Id = 20101,
                PatientMRNId = "01302109",
                ClarityPatientId = "Z667259",
                PracticeId = 37,
                FirstName = "Ryan",
                MiddleName = "",
                LastName = "Abbott",
                PatientDOB = Convert.ToDateTime("2004-08-25T00:00:00"),
                PCPId = 602,
                PCPFirstName = "Nancy",
                PCPLastName = "Young",
                InsuranceId = 2038,
                InsuranceName = "UMR",
                AddressLine1 = "5613 Glenbrook Ct",
                AddressLine2 = "",
                City = "Mason",
                State = "OH",
                Zip = "45040",
                Conditions = new List<PatientConditionViewModel>(),
                PMCAScore = 0,
                ProviderPMCAScore = 2,
                ProviderNotes = "Ryan needs more care. Just updated",
                ActiveStatus = true,
                PendingStatusConfirmation = true,
                GenderId = 1,
                Gender = "M",
                Email = "",
                Phone1 = "5137700902",
                Phone2 = "",
                PracticeVisits = 28,
                HealthBridgeEncounters = 0,
                UniqueDXs = 0,
                UniqueCPTCodes = 23,
                LastPracticeVisit = Convert.ToDateTime("2018-09-11T00:00:00"),
                LastCCHMCAdmit = Convert.ToDateTime("2018-09-12T00:00:00"),
                LastHealthBridgeAdmit = Convert.ToDateTime("2016-01-28T21:04:00"),
                LastDiagnosis = "",
                CCHMCAppointment = Convert.ToDateTime("2016-02-03T19:00:00")
            };

            _PatientController = new PatientsController(_mockLogger.Object, _mockUserService.Object, _mapper, _mockPatientDal.Object);
            _mockPatientDal.Setup(p => p.IsPatientInSamePractice(It.IsAny<Int32>(), It.IsAny<Int32>())).Returns(false);

            // execute
            var result = await _PatientController.UpdatePatientDetails(myPatientDetails, patientId) as ObjectResult;

            // assert
            Assert.AreEqual(400, result.StatusCode);
            Assert.AreEqual("patient practice does not match user", result.Value);
            _mockPatientDal.Verify(p => p.GetPatientDetails(It.IsAny<int>(), It.IsAny<int>(), false), Times.Never);
        }

        [TestMethod]
        public async Task UpdatePatientDetails_DataLayerThrowsException_ReturnsError()
        {
            // setup

            List<PatientConditionViewModel> conditions = new List<PatientConditionViewModel>();
            PatientConditionViewModel condition1 = new PatientConditionViewModel();
            condition1.ID = 1;
            condition1.Name = "Asthma";
            conditions.Add(condition1);


            var myPatientDetails = new PatientDetailsViewModel()
            {
                Id = 20101,
                PatientMRNId = "01302109",
                ClarityPatientId = "Z667259",
                PracticeId = 37,
                FirstName = "Ryan",
                MiddleName = "",
                LastName = "Abbott",
                PatientDOB = Convert.ToDateTime("2004-08-25T00:00:00"),
                PCPId = 602,
                PCPFirstName = "Nancy",
                PCPLastName = "Young",
                InsuranceId = 2038,
                InsuranceName = "UMR",
                AddressLine1 = "5613 Glenbrook Ct",
                AddressLine2 = "",
                City = "Mason",
                State = "OH",
                Zip = "45040",
                Conditions = conditions,
                PMCAScore = 0,
                ProviderPMCAScore = 2,
                ProviderNotes = "Ryan needs more care. Just updated",
                ActiveStatus = true,
                PendingStatusConfirmation = true,
                GenderId = 1,
                Gender = "M",
                Email = "",
                Phone1 = "5137700902",
                Phone2 = "",
                PracticeVisits = 28,
                HealthBridgeEncounters = 0,
                UniqueDXs = 0,
                UniqueCPTCodes = 23,
                LastPracticeVisit = Convert.ToDateTime("2018-09-11T00:00:00"),
                LastCCHMCAdmit = Convert.ToDateTime("2018-09-12T00:00:00"),
                LastHealthBridgeAdmit = Convert.ToDateTime("2016-01-28T21:04:00"),
                LastDiagnosis = "",
                CCHMCAppointment = Convert.ToDateTime("2016-02-03T19:00:00")
            };

            _mockUserService.Setup(p => p.GetUserIdFromClaims(It.IsAny<IEnumerable<Claim>>())).Returns(_userId).Verifiable();
            _mockPatientDal.Setup(p => p.UpdatePatientDetails(_userId, It.IsAny<PatientDetails>())).Throws(new Exception());
            _mockPatientDal.Setup(p => p.IsPatientInSamePractice(It.IsAny<Int32>(), It.IsAny<Int32>())).Returns(true);
            _PatientController = new PatientsController(_mockLogger.Object, _mockUserService.Object, _mapper, _mockPatientDal.Object);


            // execute
            var result = await _PatientController.UpdatePatientDetails(myPatientDetails, "20101") as ObjectResult;


            // assert
            Assert.AreEqual(500, result.StatusCode);
        }

    }
}
