﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;


namespace PHO_WebApp.Models
{
    public class QualityImprovement
    {
        public int Id { get; set; }
        public int PracticeId { get; set; }
        public string PracticeName { get; set; }

        [Required(ErrorMessage = "Name is required!")]
        public string Name { get; set; }
        
        public string Desc { get; set;}
        public int InitiativeId { get; set;}        
        public string Initiative { get; set; }
        public string InitiativeDesc { get; set; }
        public string InitiativeOwner { get; set; }
        public string InitiativeStatus { get; set; }
        public int CohortId { get; set; }
        public string CohortName { get; set; }
        public string CohortDesc { get; set; }
        public string CohortDataSources { get; set; }
        public string CohortLookback { get; set; }
        public string CohortStatus { get; set; }
        public int MeasureId { get; set; }
        public string MeasureName { get; set; }
        public string MeasureDesc { get; set; }
        public string MeasureStatus { get; set; }
        public string MeasureFrequency { get; set; }
        public string MeasureNumeratorDesc { get; set; }
        public string MeasureDenominatorDesc { get; set; }
        public string N { get; set; }
        public int Factor { get; set; }
        public string ChartDesc { get; set; }
        public string ChartType { get; set; }
        public DateTime? LastMeasureDate { get; set; }
        public int? MeasueNumerator { get; set; }
        public int? MeasueDenominator { get; set; }
        public decimal? MeasueValue { get; set; }
        public int MeasureN { get; set; }
    }
}