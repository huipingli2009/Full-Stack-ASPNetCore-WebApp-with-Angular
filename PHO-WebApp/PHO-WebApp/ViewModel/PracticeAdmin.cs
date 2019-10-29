using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PHO_WebApp.Models;
using PHO_WebApp.DataAccessLayer;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;

namespace PHO_WebApp.ViewModel
{ 

    public class PracticeAdmin
    {
        private SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["con"].ConnectionString);
        static int temp;
        //constructor
        public PracticeAdmin()
        {
            Init();
            PracStaff = new List<Staff>();
            Entity = new Staff();
        }

        public List<Staff> PracStaff { get; set; }
        public UserDetails UserLogin { get; set; }

        public Staff Entity { get; set; }  //for adding staff purpose
        public bool IsValid { get; set; }  //for checking staff data added
        public string Mode { get; set; }

        //properties to control visibility
        public bool IsStaffDetailsAreaVisible { get; set; }
        public bool IsStaffListAreaVisible { get; set; }

        public string EventCommand { get; set; }
        public string EventArgument { get; set; }

        //public List<Staff> GetStaffs()
        //{
        //    List<Staff> list = new List<Staff>();
        //    StaffDAL std = new StaffDAL();
        //    Staff stf = new Staff();

        //    list = std.getPracticeStaffs();

        //    stf = list.FindAll(s=>s.Id==)
        //    return list;
        //}

        private void Init()
        {
            EventCommand = "StaffList";
            EventArgument = string.Empty;
            ListMode();
        }

        private void Add()
        {
            IsValid = true;

            //initialize
            Entity = new Staff();
            Entity.FirstName = string.Empty;
            Entity.LastName = string.Empty;
            Entity.EmailAddress = string.Empty;
            Entity.Phone = string.Empty;
            Entity.PositionId = "";
            Entity.PracticelocationId = 0;
            Entity.AddressLine1 = "";
            Entity.AddressLine2 = "";
            Entity.City = "";
            Entity.State = "";
            Entity.Zip = "";
            Entity.StaffTypeId = 0;
            Entity.CredId = 0;

            //Put ViewModel mode to AddMode
            AddMode();
        }
        private void SaveStaff()
        {
            PracticeAdmin vmPA = new PracticeAdmin();

            //added here for testing purpose
            IsValid = true;

            if (IsValid)
            {
                if (Mode == "AddStaff")
                {
                    vmPA.Insert(Entity);
                    //Insert(vmPA);
                }
                else   //update staff
                {
                    //vmPA.SaveStaff(Entity);
                    vmPA.Update(Entity);                       
                }
            }
            else

            {
                if (Mode == "AddStaff")
                {
                    AddMode();
                }
                else   //update staff
                {
                    EditMode();
                }
            }
        }

        private void EditStaff()
        {
            PracticeAdmin pa = new PracticeAdmin();

            Entity = pa.Get(Convert.ToInt32(EventArgument));

            EditMode();
        }

        private bool SaveStaff(Staff stf)
        {
            bool ret = false;

            //ret = Validate(stf);
           
            if(ret)
            {
                //update
            }

            EditMode();

            return true;
        }

        public void DeleteStaff()
        {
            PracticeAdmin pa = new PracticeAdmin();
            Staff stf = new Staff();

            stf.Id = Convert.ToInt32(EventArgument);

            pa.DeleteStaff(stf);

            GetStaffs();

            ListMode();
        }

        public Staff Get(int stId)
        {
            StaffDAL std = new StaffDAL();
            Staff stf = new Staff();

            temp = stId;

            //stf.Id = stId;
           // PracStaff = std.getPracticeStaffs();
            stf = std.getPracticeStaff(stId);   


            return stf;
        }

        //private bool Insert(Staff st)
        //{
        //    bool ret = true; //preset to true and may come back reset to false when working with validation

        //    //if pass validation, code to add data to database
        //    //ret = Validate(st);
        //    if (ret)
        //    {
        //        //ToDo: add data to database

        //    }

        //    //end if

        //    return ret;
        //}

        private void ListMode()
        {
            IsValid = true;
            IsStaffListAreaVisible = true;
            IsStaffDetailsAreaVisible = false;
            //IsStaffDetailsAreaVisible = true;

            Mode = "StaffList";
        }

        private void AddMode()
        {
            IsStaffListAreaVisible = false;
            IsStaffDetailsAreaVisible = true;

            Mode = "AddStaff";
        }

        private void EditMode()
        {
            IsStaffListAreaVisible = false;
            IsStaffDetailsAreaVisible = true;

            Mode = "EditStaff";
        }


        private void CancelMode()
        {
            IsStaffListAreaVisible = true;
            IsStaffDetailsAreaVisible = false;
        }

        public void HandleRequest()
        {
            switch (EventCommand.ToLower())
            {
                case "stafflist":
                    ListMode();
                    GetStaffs();
                    break;

                case "addstaff":
                    Add();
                    //AddStaffs();
                    break;

                case "editstaff":
                    //System.Diagnostics.Debugger.Break();
                    IsValid = true;
                    EditStaff();
                    break;

                case "savestaff":
                    SaveStaff();
                    //if(IsValid)
                    //{
                        GetStaffs();
                    //}                   
                    break;
                case "deletestaff":
                    DeleteStaff();
                    break;
                case "cancel":
                    ListMode();
                    GetStaffs();
                    break;

                default:
                    break;
            }
        }
        private void GetStaffs()
        {
            StaffDAL std = new StaffDAL();

            //hardcoding this for a jif. Need to pull practiceId from either local property or controller layer
            PracStaff = std.getPracticeStaffs(7); 
        }

        public void Insert(Staff model)
        {
            SqlCommand com = new SqlCommand("spAddNewStaff", con);
            com.CommandType = CommandType.StoredProcedure;

            com.Parameters.Add("@PracticeId", SqlDbType.Int);
            com.Parameters.Add("@StaffTypeId", SqlDbType.Int);
            com.Parameters.Add("@FirstName", SqlDbType.NVarChar);
            com.Parameters.Add("@LastName", SqlDbType.NVarChar);
            com.Parameters.Add("@EmailAddress", SqlDbType.NVarChar);
            com.Parameters.Add("@Phone", SqlDbType.NVarChar);
            com.Parameters.Add("@NPI", SqlDbType.Int);
            com.Parameters.Add("@CreatedById", SqlDbType.Int);
            //com.Parameters.Add("@CreatedONDate", SqlDbType.DateTime);

            //replace this part with coding when we have Create Staff part done
            //Create Staff is the prior step before creating user login per system design
            com.Parameters["@PracticeId"].Value = int.Parse(HttpContext.Current.Session["PracticeId"].ToString()); ;   //practice staff type id = 4           

            //hard code for practice staff. Will be replaced with Staff Type look ups
            com.Parameters["@StaffTypeId"].Value = 5;

            //if (!String.IsNullOrWhiteSpace(model.Entity.FirstName))
            //{
            //    com.Parameters["@FirstName"].Value = model.Entity.FirstName;
            //}
            if (!String.IsNullOrWhiteSpace(model.FirstName))
            {
                com.Parameters["@FirstName"].Value = model.FirstName;
            }
            else
            {
                com.Parameters["@FirstName"].Value = DBNull.Value;
            }

            if (!String.IsNullOrWhiteSpace(model.LastName))
            {
                com.Parameters["@LastName"].Value = model.LastName;
            }
            else
            {
                com.Parameters["@LastName"].Value = DBNull.Value;
            }

            if (!String.IsNullOrWhiteSpace(model.EmailAddress))
            {
                com.Parameters["@EmailAddress"].Value = model.EmailAddress;
            }
            else
            {
                com.Parameters["@EmailAddress"].Value = DBNull.Value;
            }

            if (!String.IsNullOrWhiteSpace(model.Phone))
            {
                com.Parameters["@Phone"].Value = model.Phone;
            }
            else
            {
                com.Parameters["@Phone"].Value = DBNull.Value;
            }

            //com.Parameters["@NPI"].Value = model.NPI;
            if (!String.IsNullOrWhiteSpace(model.NPI.ToString()))
            {
                com.Parameters["@NPI"].Value = model.NPI;
            }
            else
            {
                com.Parameters["@NPI"].Value = DBNull.Value;
            }

            //com.Parameters["@CreatedById"].Value = model.NPI;

            if (!String.IsNullOrWhiteSpace(HttpContext.Current.Session["UserId"].ToString()))
            {
                com.Parameters["@CreatedById"].Value = int.Parse(HttpContext.Current.Session["UserId"].ToString());
            }
            else
            {
                com.Parameters["@CreatedById"].Value = DBNull.Value;
            }

            //com.Parameters["@CreatedONDate"].Value = model.CreatedOnDate;
           
            con.Open();
            com.ExecuteNonQuery();
            con.Close();
        }

        public void Update(Staff model)
        {
            SqlCommand com = new SqlCommand("spUpdateNewStaff", con);
            com.CommandType = CommandType.StoredProcedure;

            //com.Parameters.Add("@PracticeId", SqlDbType.Int);

            com.Parameters.Add("@Id", SqlDbType.Int);
            com.Parameters.Add("@StaffTypeId", SqlDbType.Int);
            com.Parameters.Add("@FirstName", SqlDbType.NVarChar);
            com.Parameters.Add("@LastName", SqlDbType.NVarChar);
            com.Parameters.Add("@EmailAddress", SqlDbType.NVarChar);
            com.Parameters.Add("@Phone", SqlDbType.NVarChar);
            com.Parameters.Add("@NPI", SqlDbType.Int);
            com.Parameters.Add("@ModifiedById", SqlDbType.Int);
            //com.Parameters.Add("@CreatedONDate", SqlDbType.DateTime);


            //replace this part with coding when we have Create Staff part done
            //Create Staff is the prior step before creating user login per system design
            //com.Parameters["@PracticeId"].Value = 7;   //practice staff type id = 4           

            com.Parameters["@Id"].Value = temp; //model.Id; //hard coding for now
            //hard code for practice staff. Will be replaced with Staff Type look ups
            com.Parameters["@StaffTypeId"].Value = 5;

            //if (!String.IsNullOrWhiteSpace(model.Entity.FirstName))
            //{
            //    com.Parameters["@FirstName"].Value = model.Entity.FirstName;
            //}
            if (!String.IsNullOrWhiteSpace(model.FirstName))
            {
                com.Parameters["@FirstName"].Value = model.FirstName;
            }
            else
            {
                com.Parameters["@FirstName"].Value = DBNull.Value;
            }

            if (!String.IsNullOrWhiteSpace(model.LastName))
            {
                com.Parameters["@LastName"].Value = model.LastName;
            }
            else
            {
                com.Parameters["@LastName"].Value = DBNull.Value;
            }

            if (!String.IsNullOrWhiteSpace(model.EmailAddress))
            {
                com.Parameters["@EmailAddress"].Value = model.EmailAddress;
            }
            else
            {
                com.Parameters["@EmailAddress"].Value = DBNull.Value;
            }

            if (!String.IsNullOrWhiteSpace(model.Phone))
            {
                com.Parameters["@Phone"].Value = model.Phone;
            }
            else
            {
                com.Parameters["@Phone"].Value = DBNull.Value;
            }

            com.Parameters["@NPI"].Value = model.NPI;

            //com.Parameters["@CreatedById"].Value = model.NPI;

            if (!String.IsNullOrWhiteSpace(HttpContext.Current.Session["UserId"].ToString()))
            {
                com.Parameters["@ModifiedById"].Value = int.Parse(HttpContext.Current.Session["UserId"].ToString());
            }
            else
            {
                com.Parameters["@ModifiedById"].Value = DBNull.Value;
            }

            //com.Parameters["@CreatedONDate"].Value = model.CreatedOnDate;

            con.Open();
            com.ExecuteNonQuery();
            con.Close();
        }
        public bool DeleteStaff(Staff stf)
        {
            SqlCommand com = new SqlCommand("spDeleteStaff", con);
            com.CommandType = CommandType.StoredProcedure;

            com.Parameters.Add("@Id", SqlDbType.Int);
            com.Parameters.Add("@DeletedById", SqlDbType.Int);

            com.Parameters["@Id"].Value = stf.Id;

            if (!String.IsNullOrWhiteSpace(HttpContext.Current.Session["UserId"].ToString()))
            {
                com.Parameters["@DeletedById"].Value = int.Parse(HttpContext.Current.Session["UserId"].ToString());
            }
            else
            {
                com.Parameters["@DeletedById"].Value = DBNull.Value;
            }

            con.Open();
            com.ExecuteNonQuery();
            con.Close();

            return true;
        }
    }
}