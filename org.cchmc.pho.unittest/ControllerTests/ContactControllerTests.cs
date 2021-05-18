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
    public class ContactControllerTests
    {
        private ContactsController _contactController;
        private Mock<ILogger<ContactsController>> _mockLogger;
        private Mock<IUserService> _mockUserService;
        private Mock<IContact> _mockContactDAL;
        private IMapper _mapper;

        //Security moq objects
        private const string _userName = "bblackmore";
        private const string _password = "P@ssw0rd!";
        private const int _userId = 3;

        //parameters setup 
        private const bool qpl = true;
        private const string specialty = null;
        private const string membership = null;
        private const string board = null;
        private const string namesearch = null;

        private List<Role> _role = new List<Role>()
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
                cfg.AddMaps(Assembly.GetAssembly(typeof(ContactMappings)));
            });
            _mapper = config.CreateMapper();
            _mockUserService = new Mock<IUserService>();
            _mockContactDAL = new Mock<IContact>();
            _mockLogger = new Mock<ILogger<ContactsController>>();
        }

        [TestMethod]
        public async Task GetContacts_Mapping_Success()
        {
            //setup - parameters setup have been moved up as const to avoid hard coding

            List<Contact> myContacts = new List<Contact>()
            {
                new Contact()
                {
                     PracticeId = 1,
                     PracticeName = "My Practice1",
                     PracticeType = "PHO Practice",
                     EMR = "Athena",
                     Phone = "(513)888-9999",
                     Fax = "513-888-9090",
                     WebsiteURL = "https://www.mypractice1.com"
                },
                 new Contact()
                {
                     PracticeId = 2,
                     PracticeName = "My Practice2",
                     PracticeType = "PHO Practice",
                     EMR = "Athena",
                     Phone = "(513)234-5678",
                     Fax = "513-432-5566",
                     WebsiteURL = "https://www.mypractice2.com"
                }
            };

            _mockUserService.Setup(p => p.GetUserIdFromClaims(It.IsAny<IEnumerable<Claim>>())).Returns(_userId).Verifiable();
            _mockContactDAL.Setup(p => p.GetContacts(_userId, qpl, specialty, membership, board, namesearch)).Returns(Task.FromResult(myContacts)).Verifiable();
            _contactController = new ContactsController(_mockLogger.Object, _mapper, _mockUserService.Object, _mockContactDAL.Object);

            // execute
            var results = await _contactController.GetContacts(qpl, specialty, membership, board, namesearch) as ObjectResult;
            var resultList = results.Value as List<ContactViewModel>;

            // assert
            Assert.AreEqual(2, resultList.Count);
            Assert.AreEqual(myContacts[0].PracticeId, resultList[0].PracticeId);
            Assert.AreEqual(myContacts[0].PracticeName, resultList[0].PracticeName);
            Assert.AreEqual(myContacts[0].PracticeType, resultList[0].PracticeType);
            Assert.AreEqual(myContacts[0].EMR, resultList[0].EMR);
            Assert.AreEqual(myContacts[0].Phone, resultList[0].Phone);
            Assert.AreEqual(myContacts[0].Fax, resultList[0].Fax);
            Assert.AreEqual(myContacts[0].WebsiteURL, resultList[0].WebsiteURL);

            Assert.AreEqual(myContacts[1].PracticeId, resultList[1].PracticeId);
            Assert.AreEqual(myContacts[1].PracticeName, resultList[1].PracticeName);
            Assert.AreEqual(myContacts[1].PracticeType, resultList[1].PracticeType);
            Assert.AreEqual(myContacts[1].EMR, resultList[1].EMR);
            Assert.AreEqual(myContacts[1].Phone, resultList[1].Phone);
            Assert.AreEqual(myContacts[1].Fax, resultList[1].Fax);
            Assert.AreEqual(myContacts[1].WebsiteURL, resultList[1].WebsiteURL);
        }

        [TestMethod]
        public async Task GetContacts_DataLayerThrowsException_ReturnsError()
        {
            // setup
            _mockUserService.Setup(p => p.GetUserIdFromClaims(It.IsAny<IEnumerable<Claim>>())).Returns(_userId).Verifiable();
            _mockContactDAL.Setup(p => p.GetContacts(_userId, qpl, specialty, membership, board, namesearch)).Throws(new Exception()).Verifiable();
            _contactController = new ContactsController(_mockLogger.Object, _mapper, _mockUserService.Object, _mockContactDAL.Object);

            // execute
            var result = (ObjectResult)await _contactController.GetContacts(qpl, specialty, membership, board, namesearch);
            // assert
            Assert.AreEqual(500, result.StatusCode);
        }
    }
}
