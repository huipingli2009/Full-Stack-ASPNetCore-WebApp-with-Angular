//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PHO_WebApp
{
    using System;
    using System.Collections.Generic;
    
    public partial class Patient
    {
        public int Id { get; set; }
        public int PersonTypeId { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public Nullable<int> Sex_Id { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public Nullable<System.DateTime> PersonDOB { get; set; }
        public string Condition { get; set; }
        public string EmailAddress { get; set; }
        public string SSN { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public Nullable<int> State_Id { get; set; }
        public string Zip { get; set; }
        public string Phone1 { get; set; }
        public string Phone2 { get; set; }
        public string CCHMCMrn { get; set; }
        public Nullable<int> InsurerId { get; set; }
        public Nullable<int> PCPId { get; set; }
        public string PCP_FirstName { get; set; }
        public string PCP_LastName { get; set; }
        public string PMCAScore { get; set; }
        public string ProviderPMCAScore { get; set; }
        public string ProviderPMCANotes { get; set; }
        public string PMCA_ProvFirst { get; set; }
        public string PMCA_ProvLast { get; set; }
        public Nullable<bool> ActiveFlag { get; set; }
        public Nullable<bool> DeletedFlag { get; set; }
        public Nullable<int> CreatedById { get; set; }
        public Nullable<System.DateTime> CreatedOnDate { get; set; }
        public Nullable<int> ModifiedById { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<int> DeletedById { get; set; }
        public Nullable<System.DateTime> DeletedDate { get; set; }
    }
}
