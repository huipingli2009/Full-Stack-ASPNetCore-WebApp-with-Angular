using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PHO_WebApp.Models
{
    public class DataDictionary
    {
        public int skDataDictionary { get; set; }
        public string DatabaseName { get; set; }
        public string SchemaName { get; set; }
        public string ObjectName { get; set; }
        public string ObjectType { get; set; }
        public string ColumnName { get; set; }
        public string SQLColumnDesc { get; set; }
        public string IsNullable { get; set; }
        public string DataType { get; set; }
        public int MaxLength { get; set; }
        public string PHIFlag { get; set; }
        public string BusinessDefinition { get; set; }

    }
}