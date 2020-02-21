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
    public class MetricControllerTests
    {
        private MetricController _MetricController;
        private Mock<ILogger<MetricController>> _mockLogger;
        private Mock<IMetric> _mockMetricDal;
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
            _mockMetricDal = new Mock<IMetric>();
            _mockLogger = new Mock<ILogger<MetricController>>();

        }

        [TestMethod]
        public async Task ListDashboardMetrics_Mapping_Success()
        {
            // setup
            var userId = 5;
            var myMetrics = new List<Metric>()
            {
                new Metric()
                {
                    PracticeId = 1,
                    DashboardLabel = "LabelTextFor1",
                    MeasureDesc = "DescriptionText1",
                    MeasureType = "QI",
                    PracticeTotal = 14,
                    NetworkTotal = 234
                },
                new Metric()
                {
                    PracticeId = 1,
                    DashboardLabel = "LabelTextFor2",
                    MeasureDesc = "DescriptionText2",
                    MeasureType = "QI",
                    PracticeTotal = 17,
                    NetworkTotal = 132
                }
            };
            _mockMetricDal.Setup(p => p.ListDashboardMetrics(userId)).Returns(Task.FromResult(myMetrics)).Verifiable();
            _MetricController = new MetricController(_mockLogger.Object, _mapper, _mockMetricDal.Object);

            // execute
            var result = await _MetricController.ListDashboardMetrics(userId.ToString()) as ObjectResult;
            var resultList = result.Value as List<MetricViewModel>;

            // assert
            Assert.AreEqual(2, resultList.Count);
            Assert.AreEqual(myMetrics[0].PracticeId, resultList[0].PracticeId);
            Assert.AreEqual(myMetrics[0].DashboardLabel, resultList[0].DashboardLabel);
            Assert.AreEqual(myMetrics[0].MeasureDesc, resultList[0].MeasureDesc);
            Assert.AreEqual(myMetrics[0].MeasureType, resultList[0].MeasureType);
            Assert.AreEqual(myMetrics[0].PracticeTotal, resultList[0].PracticeTotal);
            Assert.AreEqual(myMetrics[0].NetworkTotal, resultList[0].NetworkTotal);
            Assert.AreEqual(myMetrics[1].PracticeId, resultList[1].PracticeId);
            Assert.AreEqual(myMetrics[1].DashboardLabel, resultList[1].DashboardLabel);
            Assert.AreEqual(myMetrics[1].MeasureDesc, resultList[1].MeasureDesc);
            Assert.AreEqual(myMetrics[1].MeasureType, resultList[1].MeasureType);
            Assert.AreEqual(myMetrics[1].PracticeTotal, resultList[1].PracticeTotal);
            Assert.AreEqual(myMetrics[1].NetworkTotal, resultList[1].NetworkTotal);

        }

        [TestMethod]
        public async Task ListDashboardMetrics_UserIdDoesNotValidate_Throws400()
        {
            // setup
            var userId = "asdf";
            _MetricController = new MetricController(_mockLogger.Object, _mapper, _mockMetricDal.Object);

            // execute
            var result = await _MetricController.ListDashboardMetrics(userId) as ObjectResult;

            // assert
            Assert.AreEqual(400, result.StatusCode);
            Assert.AreEqual("user is not a valid integer", result.Value);
            _mockMetricDal.Verify(p => p.ListDashboardMetrics(It.IsAny<int>()), Times.Never);
        }

        [TestMethod]
        public async Task ListDashboardMetrics_DataLayerThrowsException_ReturnsError()
        {
            // setup
            var userId = 5;
            _mockMetricDal.Setup(p => p.ListDashboardMetrics(userId)).Throws(new Exception()).Verifiable();
            _MetricController = new MetricController(_mockLogger.Object, _mapper, _mockMetricDal.Object);

            // execute
            var result = await _MetricController.ListDashboardMetrics(userId.ToString()) as ObjectResult;

            // assert
            Assert.AreEqual(500, result.StatusCode);
        }
               
        [TestMethod]
        public async Task ListEDChart_Mapping_Success()
        {
            // setup
            var userId = 5;
            var myMetrics = new List<EDChart>()
            {
                new EDChart()
                {
                    PracticeId = 1,
                    AdmitDate = new DateTime(2020, 12, 1, 12, 12, 00, 00),
                    ChartLabel = "ChartTitleEtc1",
                    ChartTitle = "TitleOfSomeKind1",
                    EDVisits = 8
                },
                new EDChart()
                {
                    PracticeId = 1,
                    AdmitDate = new DateTime(2020, 12, 1, 12, 12, 00, 00),
                    ChartLabel = "ChartTitleEtc2",
                    ChartTitle = "TitleOfSomeKind2",
                    EDVisits = 12
                }
            };
            _mockMetricDal.Setup(p => p.ListEDChart(userId)).Returns(Task.FromResult(myMetrics)).Verifiable();
            _MetricController = new MetricController(_mockLogger.Object, _mapper, _mockMetricDal.Object);

            // execute
            var result = await _MetricController.ListEDChart(userId.ToString()) as ObjectResult;
            var resultList = result.Value as List<EDChartViewModel>;

            // assert
            Assert.AreEqual(2, resultList.Count);
            Assert.AreEqual(myMetrics[0].PracticeId, resultList[0].PracticeId);
            Assert.AreEqual(myMetrics[0].AdmitDate, resultList[0].AdmitDate);
            Assert.AreEqual(myMetrics[0].ChartLabel, resultList[0].ChartLabel);
            Assert.AreEqual(myMetrics[0].ChartTitle, resultList[0].ChartTitle);
            Assert.AreEqual(myMetrics[0].EDVisits, resultList[0].EDVisits);
            Assert.AreEqual(myMetrics[1].PracticeId, resultList[1].PracticeId);
            Assert.AreEqual(myMetrics[1].AdmitDate, resultList[1].AdmitDate);
            Assert.AreEqual(myMetrics[1].ChartLabel, resultList[1].ChartLabel);
            Assert.AreEqual(myMetrics[1].ChartTitle, resultList[1].ChartTitle);
            Assert.AreEqual(myMetrics[1].EDVisits, resultList[1].EDVisits);

        }

        [TestMethod]
        public async Task ListEDChart_UserIdDoesNotValidate_Throws400()
        {
            // setup
            var userId = "asdf";
            _MetricController = new MetricController(_mockLogger.Object, _mapper, _mockMetricDal.Object);

            // execute
            var result = await _MetricController.ListEDChart(userId) as ObjectResult;

            // assert
            Assert.AreEqual(400, result.StatusCode);
            Assert.AreEqual("user is not a valid integer", result.Value);
            _mockMetricDal.Verify(p => p.ListEDChart(It.IsAny<int>()), Times.Never);
        }

        [TestMethod]
        public async Task ListEDChart_DataLayerThrowsException_ReturnsError()
        {
            // setup
            var userId = 5;
            _mockMetricDal.Setup(p => p.ListEDChart(userId)).Throws(new Exception()).Verifiable();
            _MetricController = new MetricController(_mockLogger.Object, _mapper, _mockMetricDal.Object);

            // execute
            var result = await _MetricController.ListEDChart(userId.ToString()) as ObjectResult;

            // assert
            Assert.AreEqual(500, result.StatusCode);
        }



        [TestMethod]
        public async Task ListEDDetails_Mapping_Success()
        {
            // setup
            var userId = 5;
            var admitDate = "12/1/2020 12:00:00 AM";
            DateTime admitDateTime = Convert.ToDateTime(admitDate);
            var myMetrics = new List<EDDetail>()
            {
                new EDDetail()
                {
                    PatientMRN = "123_456_789",
                    PatientEncounterID = "4512398",
                    Facility = "Anderson",
                    PatientName = "John Doe",
                    PatientDOB = new DateTime(2010, 11, 20, 12, 12, 00, 00),
                    PCP = "Dr Nick Riviera",
                    HospitalAdmission = new DateTime(2020, 11, 19, 12, 12, 00, 00),
                    HospitalDischarge = new DateTime(2020, 12, 1, 12, 12, 00, 00),
                    VisitType = "Followup",
                    PrimaryDX = "A234",
                    PrimaryDX_10Code = "10",
                    DX2 = "B789",
                    DX2_10Code = "11"
                },
                new EDDetail()
                {
                    PatientMRN = "987_654_321",
                    PatientEncounterID = "7612986",
                    Facility = "Fairfield",
                    PatientName = "Susan Smith",
                    PatientDOB = new DateTime(2012, 3, 15, 12, 12, 00, 00),
                    PCP = "Dr Richard Kimble",
                    HospitalAdmission = new DateTime(2020, 10, 19, 12, 12, 00, 00),
                    HospitalDischarge = null,
                    VisitType = "Followup",
                    PrimaryDX = "D234",
                    PrimaryDX_10Code = "10",
                    DX2 = "C789",
                    DX2_10Code = "11"
                }
            };
            _mockMetricDal.Setup(p => p.ListEDDetails(userId, admitDateTime)).Returns(Task.FromResult(myMetrics)).Verifiable();
            _MetricController = new MetricController(_mockLogger.Object, _mapper, _mockMetricDal.Object);

            // execute
            var result = await _MetricController.ListEDDetails(userId.ToString(), admitDate.ToString()) as ObjectResult;
            var resultList = result.Value as List<EDDetail>;

            // assert
            Assert.AreEqual(2, resultList.Count);
            Assert.AreEqual(myMetrics[0].PatientMRN, resultList[0].PatientMRN);
            Assert.AreEqual(myMetrics[0].PatientEncounterID, resultList[0].PatientEncounterID);
            Assert.AreEqual(myMetrics[0].Facility, resultList[0].Facility);
            Assert.AreEqual(myMetrics[0].PatientName, resultList[0].PatientName);
            Assert.AreEqual(myMetrics[0].PatientDOB, resultList[0].PatientDOB);
            Assert.AreEqual(myMetrics[0].PCP, resultList[0].PCP);
            Assert.AreEqual(myMetrics[0].HospitalAdmission, resultList[0].HospitalAdmission);
            Assert.AreEqual(myMetrics[0].HospitalDischarge, resultList[0].HospitalDischarge);
            Assert.AreEqual(myMetrics[0].VisitType, resultList[0].VisitType);
            Assert.AreEqual(myMetrics[0].PrimaryDX, resultList[0].PrimaryDX);
            Assert.AreEqual(myMetrics[0].PrimaryDX_10Code, resultList[0].PrimaryDX_10Code);
            Assert.AreEqual(myMetrics[0].DX2, resultList[0].DX2);
            Assert.AreEqual(myMetrics[0].DX2_10Code, resultList[0].DX2_10Code);
            Assert.AreEqual(myMetrics[1].PatientMRN, resultList[1].PatientMRN);
            Assert.AreEqual(myMetrics[1].PatientEncounterID, resultList[1].PatientEncounterID);
            Assert.AreEqual(myMetrics[1].Facility, resultList[1].Facility);
            Assert.AreEqual(myMetrics[1].PatientName, resultList[1].PatientName);
            Assert.AreEqual(myMetrics[1].PatientDOB, resultList[1].PatientDOB);
            Assert.AreEqual(myMetrics[1].PCP, resultList[1].PCP);
            Assert.AreEqual(myMetrics[1].HospitalAdmission, resultList[1].HospitalAdmission);
            Assert.AreEqual(myMetrics[1].HospitalDischarge, resultList[1].HospitalDischarge);
            Assert.AreEqual(myMetrics[1].VisitType, resultList[1].VisitType);
            Assert.AreEqual(myMetrics[1].PrimaryDX, resultList[1].PrimaryDX);
            Assert.AreEqual(myMetrics[1].PrimaryDX_10Code, resultList[1].PrimaryDX_10Code);
            Assert.AreEqual(myMetrics[1].DX2, resultList[1].DX2);
            Assert.AreEqual(myMetrics[1].DX2_10Code, resultList[1].DX2_10Code);
        }

        [TestMethod]
        public async Task ListEDDetails_UserIdDoesNotValidate_Throws400()
        {
            // setup
            var userId = "asdf";
            var admitDate = "12/1/2020 12:00:00 AM";
            DateTime admitDateTime = Convert.ToDateTime(admitDate);
            _MetricController = new MetricController(_mockLogger.Object, _mapper, _mockMetricDal.Object);

            // execute
            var result = await _MetricController.ListEDDetails(userId, admitDate) as ObjectResult;

            // assert
            Assert.AreEqual(400, result.StatusCode);
            Assert.AreEqual("user is not a valid integer", result.Value);
            _mockMetricDal.Verify(p => p.ListEDDetails(It.IsAny<int>(), It.IsAny<DateTime>()), Times.Never);
        }

        [TestMethod]
        public async Task ListEDDetails_AdmitDateDoesNotValidate_Throws400()
        {
            // setup
            var userId = "5";
            var admitDate = "asdf";
            _MetricController = new MetricController(_mockLogger.Object, _mapper, _mockMetricDal.Object);

            // execute
            var result = await _MetricController.ListEDDetails(userId, admitDate) as ObjectResult;

            // assert
            Assert.AreEqual(400, result.StatusCode);
            Assert.AreEqual("admitdate is not a valid datetime", result.Value);
            _mockMetricDal.Verify(p => p.ListEDDetails(It.IsAny<int>(), It.IsAny<DateTime>()), Times.Never);
        }

        [TestMethod]
        public async Task ListEDDetails_DataLayerThrowsException_ReturnsError()
        {
            // setup
            var userId = 5; 
            var admitDate = "12/1/2020 12:00:00 AM";
            DateTime admitDateTime = Convert.ToDateTime(admitDate);
            _mockMetricDal.Setup(p => p.ListEDDetails(userId, admitDateTime)).Throws(new Exception()).Verifiable();
            _MetricController = new MetricController(_mockLogger.Object, _mapper, _mockMetricDal.Object);

            // execute
            var result = await _MetricController.ListEDDetails(userId.ToString(), admitDate) as ObjectResult;

            // assert
            Assert.AreEqual(500, result.StatusCode);
        }
    }
}
