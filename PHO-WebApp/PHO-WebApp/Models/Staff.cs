using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace PHO_WebApp.Models
{
    public class Staff
    {
        public int Id { get; set; }
        public int StaffId { get; set; }

        [Required(ErrorMessage = "First Name can't be blank")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name can't be blank")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email can't be blank")]
        public string EmailAddress { get; set; }

        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }

        public string RegistryAccess { get; set; }

        public string LeadPhysician { get; set; }
        public string QI_Team { get; set; }
        public string PracticeManager { get; set; }
        public string InterventionContact { get; set; }

        public int StateId { get; set; }


        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Not a valid phone number")]
        [Required(ErrorMessage = "Phone number can't be blank")]
        public string Phone { get; set; }

        //Position will replace role for staff. Hence position id here 
        [Required(ErrorMessage = "Position can't be blank")]
        public string PositionId { get; set; }

        //staff credential
        [Required(ErrorMessage = "Credential can't be blank")]
        public int CredId { get; set; }

        public string CredName { get; set; }

        //practice location, not required       
        public int PracticelocationId { get; set; }

        public bool ActiveFlag { get; set; }
        public bool DeletedFlag { get; set; }

        public int StaffTypeId { get; set; }
        public string StaffPosition { get; set; }

        public string CreatedbyId { get; set; }

        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        public DateTime? CreatedOnDate { get; set; }

        public string ModifiedbyId { get; set; }

        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        public DateTime? ModifiedDate { get; set; }

        public StaffTypeEnum StaffTypeEnum { get; set; }
        //public string StaffId { get; set; }
        //public string StaffId { get; set; }
        //public string StaffId { get; set; }
        //public int StaffId { get; set; }

        public Staff()
        {
            
        }
        public Staff(int staffId, int staffTypeId, string firstName, string lastName)
        {
            this.StaffId = staffId;
            this.StaffTypeId = staffTypeId;
            this.FirstName = firstName;
            this.LastName = lastName;
        }

        public string LookupDisplayText
        {
            get
            {
                string returnValue = string.Empty;

                if (!string.IsNullOrEmpty(this.LastName))
                {
                    returnValue += this.LastName;
                    if (!string.IsNullOrEmpty(this.FirstName))
                    {
                        returnValue += ", ";
                        returnValue += this.FirstName;

                    }


                    if (!string.IsNullOrEmpty(this.StaffPosition))
                    {
                        returnValue += " ";
                        returnValue += this.StaffPosition;
                    }

                    if (this.StaffId > 0)
                    {
                        returnValue += " ID: ";
                        returnValue += this.StaffId.ToString();
                    }
                }

                return returnValue;

            }
        }
    }   
}