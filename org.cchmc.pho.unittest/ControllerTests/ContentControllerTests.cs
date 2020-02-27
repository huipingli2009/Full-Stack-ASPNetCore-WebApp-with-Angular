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
    public class ContentControllerTests
    {
        private ContentController _ContentController;
        private Mock<ILogger<ContentController>> _mockLogger;
        private Mock<IContent> _mockContentDal;
        private IMapper _mapper;

        [TestInitialize]
        public void Initialize()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddMaps(Assembly.GetExecutingAssembly());
                cfg.AddMaps(Assembly.GetAssembly(typeof(MetricMappings)));
            });
            _mapper = config.CreateMapper();
            _mockContentDal = new Mock<IContent>();
            _mockLogger = new Mock<ILogger<ContentController>>();

        }

        [TestMethod]
        public async Task ListActiveSpotlights_Mapping_Success()
        {
            // setup
            var mySpotlights = new List<SpotLight>()
            {
                //new SpotLight()
                //{
                //    PracticeId = 1,
                //    DashboardLabel = "LabelTextFor1",
                //    MeasureDesc = "DescriptionText1",
                //    MeasureType = "QI",
                //    PracticeTotal = 14,
                //    NetworkTotal = 234
                //},
                //new SpotLight()
                //{
                //    PracticeId = 1,
                //    DashboardLabel = "LabelTextFor2",
                //    MeasureDesc = "DescriptionText2",
                //    MeasureType = "QI",
                //    PracticeTotal = 17,
                //    NetworkTotal = 132
                //}
            };
            _mockContentDal.Setup(p => p.ListActiveSpotLights()).Returns(Task.FromResult(mySpotlights)).Verifiable();
            _ContentController = new ContentController(_mockLogger.Object, _mapper, _mockContentDal.Object);

            // execute
            var result = await _ContentController.ListActiveSpotLights() as ObjectResult;
            var resultList = result.Value as List<SpotLightViewModel>;

            // assert
            //Assert.AreEqual(2, resultList.Count);
            //Assert.AreEqual(mySpotlights[0].PracticeId, resultList[0].PracticeId);
            //Assert.AreEqual(mySpotlights[0].DashboardLabel, resultList[0].DashboardLabel);
            //Assert.AreEqual(mySpotlights[0].MeasureDesc, resultList[0].MeasureDesc);
            //Assert.AreEqual(mySpotlights[0].MeasureType, resultList[0].MeasureType);
            //Assert.AreEqual(mySpotlights[0].PracticeTotal, resultList[0].PracticeTotal);
            //Assert.AreEqual(mySpotlights[0].NetworkTotal, resultList[0].NetworkTotal);
            //Assert.AreEqual(mySpotlights[1].PracticeId, resultList[1].PracticeId);
            //Assert.AreEqual(mySpotlights[1].DashboardLabel, resultList[1].DashboardLabel);
            //Assert.AreEqual(mySpotlights[1].MeasureDesc, resultList[1].MeasureDesc);
            //Assert.AreEqual(mySpotlights[1].MeasureType, resultList[1].MeasureType);
            //Assert.AreEqual(mySpotlights[1].PracticeTotal, resultList[1].PracticeTotal);
            //Assert.AreEqual(mySpotlights[1].NetworkTotal, resultList[1].NetworkTotal);

        }

        [TestMethod]
        public async Task ListActiveSpotlights_DataLayerThrowsException_ReturnsError()
        {
            // setup
            _mockContentDal.Setup(p => p.ListActiveSpotLights()).Throws(new Exception()).Verifiable();
            _ContentController = new ContentController(_mockLogger.Object, _mapper, _mockContentDal.Object);

            // execute
            var result = await _ContentController.ListActiveSpotLights() as ObjectResult;

            // assert
            Assert.AreEqual(500, result.StatusCode);
        }
    }
}
