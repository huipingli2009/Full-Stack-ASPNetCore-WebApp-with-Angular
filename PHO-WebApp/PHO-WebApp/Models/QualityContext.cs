using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace PHO_WebApp.Models
{
    public class QualityContext : DbContext
    {
        // You can add custom code to this file. Changes will not be overwritten.
        // 
        // If you want Entity Framework to drop and regenerate your database
        // automatically whenever you change your model schema, please use data migrations.
        // For more information refer to the documentation:
        // http://msdn.microsoft.com/en-us/data/jj591621.aspx
    
        public QualityContext() : base("name=QualityContext")
        {
        }

        public System.Data.Entity.DbSet<PHO_WebApp.Models.QualityImprovement> QualityImprovements { get; set; }

        public System.Data.Entity.DbSet<PHO_WebApp.Models.Staff> Staffs { get; set; }

        public System.Data.Entity.DbSet<PHO_WebApp.Models.Patient> Patients { get; set; }

        public System.Data.Entity.DbSet<PHO_WebApp.Models.Practice> Practices { get; set; }
    }
}
