using AutoMapper;
using System.Security.Claims;
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
using org.cchmc.pho.identity.Interfaces;
using org.cchmc.pho.identity.Models;

namespace org.cchmc.pho.unittest.ControllerTests
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class ContentsControllerTests
    {
        private ContentsController _ContentController;
        private Mock<ILogger<ContentsController>> _mockLogger;
        private Mock<IUserService> _mockUserService;
        private Mock<IContent> _mockContentDal;
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
                cfg.AddMaps(Assembly.GetAssembly(typeof(MetricMappings)));
            });
            _mapper = config.CreateMapper();
            _mockUserService = new Mock<IUserService>();
            _mockContentDal = new Mock<IContent>();
            _mockLogger = new Mock<ILogger<ContentsController>>();

        }

        [TestMethod]
        public async Task ListActiveSpotlights_Mapping_Success()
        {
            // setup
            var mySpotlights = new List<SpotLight>()
            {
                new SpotLight()
                {
                    Header = "Wonderful things!",
                    Body = "DescriptionText1",
                    Hyperlink = "http://yadayada.com",
                    ImageHyperlink = "http://yadayada.com",
                    LocationId = 1
                },
                new SpotLight()
                {
                    Header = "Good news!",
                    Body = "DescriptionText2",
                    Hyperlink = "http://yadayada2.com",
                    ImageHyperlink = "http://yadayada2.com",
                    LocationId = 1
                }
            };
            _mockContentDal.Setup(p => p.ListActiveSpotLights()).Returns(Task.FromResult(mySpotlights)).Verifiable();
            _ContentController = new ContentsController(_mockLogger.Object, _mockUserService.Object, _mapper, _mockContentDal.Object);

            // execute
            var result = await _ContentController.ListActiveSpotLights() as ObjectResult;
            var resultList = result.Value as List<SpotLightViewModel>;

            // assert
            Assert.AreEqual(2, resultList.Count);
            Assert.AreEqual(mySpotlights[0].Header, resultList[0].Header);
            Assert.AreEqual(mySpotlights[0].Body, resultList[0].Body);
            Assert.AreEqual(mySpotlights[0].Hyperlink, resultList[0].Hyperlink);
            Assert.AreEqual(mySpotlights[0].ImageHyperlink, resultList[0].ImageHyperlink);
            Assert.AreEqual(mySpotlights[0].LocationId, resultList[0].LocationId);
            Assert.AreEqual(mySpotlights[1].Header, resultList[1].Header);
            Assert.AreEqual(mySpotlights[1].Body, resultList[1].Body);
            Assert.AreEqual(mySpotlights[1].Hyperlink, resultList[1].Hyperlink);
            Assert.AreEqual(mySpotlights[1].ImageHyperlink, resultList[1].ImageHyperlink);
            Assert.AreEqual(mySpotlights[1].LocationId, resultList[1].LocationId);

        }

        [TestMethod]
        public async Task ListActiveSpotlights_DataLayerThrowsException_ReturnsError()
        {
            // setup
            _mockContentDal.Setup(p => p.ListActiveSpotLights()).Throws(new Exception()).Verifiable();
            _ContentController = new ContentsController(_mockLogger.Object, _mockUserService.Object, _mapper, _mockContentDal.Object);

            // execute
            var result = await _ContentController.ListActiveSpotLights() as ObjectResult;

            // assert
            Assert.AreEqual(500, result.StatusCode);
        }


        [TestMethod]
        public async Task ListActiveQuicklinks_Mapping_Success()
        {
            // setup
            var myQuicklinks = new List<Quicklink>()
            {
                new Quicklink()
                {
                    PlacementOrder = 1,
                    Body = "DescriptionText1",
                    Hyperlink = "http://yadayada.com",
                    LocationId = 2
                },
                new Quicklink()
                {
                    PlacementOrder = 2,
                    Body = "DescriptionText2",
                    Hyperlink = "http://yadayada2.com",
                    LocationId = 2
                }
            };
            _mockContentDal.Setup(p => p.ListActiveQuicklinks()).Returns(Task.FromResult(myQuicklinks)).Verifiable();
            _ContentController = new ContentsController(_mockLogger.Object, _mockUserService.Object, _mapper, _mockContentDal.Object);

            // execute
            var result = await _ContentController.ListActiveQuicklinks() as ObjectResult;
            var resultList = result.Value as List<QuicklinkViewModel>;

            // assert
            Assert.AreEqual(2, resultList.Count);
            Assert.AreEqual(myQuicklinks[0].PlacementOrder, resultList[0].PlacementOrder);
            Assert.AreEqual(myQuicklinks[0].Body, resultList[0].Body);
            Assert.AreEqual(myQuicklinks[0].Hyperlink, resultList[0].Hyperlink);
            Assert.AreEqual(myQuicklinks[0].LocationId, resultList[0].LocationId);
            Assert.AreEqual(myQuicklinks[1].PlacementOrder, resultList[1].PlacementOrder);
            Assert.AreEqual(myQuicklinks[1].Body, resultList[1].Body);
            Assert.AreEqual(myQuicklinks[1].Hyperlink, resultList[1].Hyperlink);
            Assert.AreEqual(myQuicklinks[1].LocationId, resultList[1].LocationId);

        }

        [TestMethod]
        public async Task ListActiveQuicklinks_DataLayerThrowsException_ReturnsError()
        {
            // setup
            _mockContentDal.Setup(p => p.ListActiveQuicklinks()).Throws(new Exception()).Verifiable();
            _ContentController = new ContentsController(_mockLogger.Object, _mockUserService.Object, _mapper, _mockContentDal.Object);

            // execute
            var result = await _ContentController.ListActiveQuicklinks() as ObjectResult;

            // assert
            Assert.AreEqual(500, result.StatusCode);
        }
    }
}
