using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using org.cchmc.pho.api.Controllers;
using org.cchmc.pho.api.Mappings;
using org.cchmc.pho.api.ViewModels;
using org.cchmc.pho.core.DataAccessLayer;
using org.cchmc.pho.core.DataModels;
using org.cchmc.pho.core.Interfaces;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Threading.Tasks;

namespace org.cchmc.pho.unittest.DataLayerTests
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class AlertDALTests
    {
        private AlertDAL _alertDal;
        private string _connectionString = "";

        [TestInitialize]
        public void Initialize()
        {
            Cleanup();

            // TODO: Any other setup required
        }

        [TestCleanup]
        public void Cleanup()
        {

        }

        [TestMethod]
        public async Task ListActiveAlerts_ReturnsAlerts()
        {
            /* TODO: This method should:
             * - insert data in the database
             * - call ListActiveAlerts with the correct user based on data
             * - assert that the correct number of alerts in the database were returned and that the data was properly mapped
             */
        }

        [TestMethod]
        public async Task ListActiveAlerts_ReturnsAlertsOnlyForUser()
        {
            /* TODO: This method should:
             * - insert data in the database for at least 2 users
             * - call ListActiveAlerts with a user based on data
             * - assert that the correct alerts in the database were returned and that the data was properly mapped
             */
        }

        [TestMethod]
        public async Task ListActiveAlerts_ReturnsActiveAlertsOnly()
        {
            /* TODO: This method should:
             * - insert data in the database for a user, some acknowledged and some not
             * - call ListActiveAlerts with a user based on data
             * - assert that the correct alerts in the database were returned and that the data was properly mapped
             */
        }

        [TestMethod]
        public async Task ListActiveAlerts_ProperlyHandlesADOException()
        {
            /* TODO: This method should:
             * - insert data in the database for a user
             * - call DAL with a bad connection string to force an ADO exception
             * - assert that the correct error handling behavior happened
             */
        }
    }
}
