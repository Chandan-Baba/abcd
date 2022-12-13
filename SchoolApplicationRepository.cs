using Mptaas.Core;
using MPTAAS.Utils;
using MPTAAS.ViewModel.ViewModels.GuestFaculty;
using MPTAAS.ViewModel.ViewModels.SchoolApplication;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace MPTAAS.Repository.Implementation.Repository.SchoolApplication
{
    public class SchoolApplicationRepository
    {
        data sqlhelper = new data();

        // School Application Masters & Preference Block/School Detail
        #region School Application Masters & Preference Block/School Detail
        public DataTable GetState()
        {
            DataTable dt = new DataTable();
            try
            {
                dt = sqlhelper.ExecuteDataTable("select * from M_State where StateCode=23");
            }
            catch (Exception ex)
            {
                LogBase.LogController.WriteError(ex);
            }
            return dt;
        }
        public DataTable GetDistrict(int? StateId)
        {
            DataTable dt = new DataTable();
            SqlParameter[] parameter = { new SqlParameter("@StateId", StateId) };
            try
            {
                dt = sqlhelper.ExecuteDataTable("[SchoolPayment].[usp_GetDistrict]", parameter);
            }
            catch (Exception ex)
            {
                LogBase.LogController.WriteError(ex);
            }
            return dt;
        }
        public DataTable GetApplicationSLA()
        {
            DataTable dt = new DataTable();
            try
            {
                dt = sqlhelper.ExecuteDataTable("[SchoolApplication].[usp_GetApplicationSLA]");
            }
            catch (Exception ex)
            {
                LogBase.LogController.WriteError(ex);
            }
            return dt;
        }
        public List<SelectListItem> GetEntityByAppliedFor(int id, string lang)
        {
            List<SelectListItem> lst = new List<SelectListItem>();
            DataTable dt = new DataTable();
            SqlParameter[] parameter = { new SqlParameter("@TypeId", id), new SqlParameter("@Lang", lang), };
            try
            {
                dt = sqlhelper.ExecuteDataTable("[SchoolApplication].[usp_GetEntityByTypeId]", parameter);
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                        if (row["EntityId"].ToString() == "2")
                        {
                            lst.Add(new SelectListItem { Value = Convert.ToString(row["EntityId"]), Text = Convert.ToString(row["EntityName"]) });
                        }
                }
            }
            catch (Exception ex)
            {
                LogBase.LogController.WriteError(ex);
            }
            return lst.ToList();
        }
        public List<SelectListItem> GetEntityByTypeId(int id, string lang)
        {
            List<SelectListItem> lst = new List<SelectListItem>();
            DataTable dt = new DataTable();
            SqlParameter[] parameter = { new SqlParameter("@TypeId", id), new SqlParameter("@Lang", lang), };
            try
            {
                dt = sqlhelper.ExecuteDataTable("[SchoolApplication].[usp_GetEntityByTypeId]", parameter);
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        lst.Add(new SelectListItem { Value = Convert.ToString(row["EntityId"]), Text = Convert.ToString(row["EntityName"]) });
                    }
                }
            }
            catch (Exception ex)
            {
                LogBase.LogController.WriteError(ex);
            }
            return lst.ToList();
        }
        public List<SelectListItem> GetPreferenceSchoolDistrict(int appliedPostId)
        {
            List<SelectListItem> lst = new List<SelectListItem>();
            DataTable dt = new DataTable();
            try
            {
                SqlParameter[] parameter = {
                    new SqlParameter("@AppliedPostId", appliedPostId)
                };
                dt = sqlhelper.ExecuteDataTable("[SchoolApplication].[usp_GetPreferenceSchoolDistrict]", parameter);
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                        lst.Add(new SelectListItem { Value = Convert.ToString(row["DistrictCode"]), Text = Convert.ToString(row["DistrictName"]) });
                }
            }
            catch (Exception ex)
            {
                LogBase.LogController.WriteError(ex);
            }
            return lst.ToList();
        }
        public List<SelectListItem> GetPreferenceSchoolTypes(int appliedPostId, int districtid)
        {
            List<SelectListItem> lst = new List<SelectListItem>();
            DataTable dt = new DataTable();
            SqlParameter[] parameter = {
                new SqlParameter("@AppliedPostId", appliedPostId),
                new SqlParameter("@DistrictCode", districtid)
            };
            try
            {
                dt = sqlhelper.ExecuteDataTable("[SchoolApplication].[usp_GetPreferenceSchoolTypes]", parameter);
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                        lst.Add(new SelectListItem { Value = Convert.ToString(row["SchoolType"]), Text = Convert.ToString(row["SchoolTypeName"]) });
                }
            }
            catch (Exception ex)
            {
                LogBase.LogController.WriteError(ex);
            }
            return lst.ToList();
        }
        public List<SelectListItem> GetPreferenceSchoolList(int appliedPostId, int districtid, string schoolType)
        {
            List<SelectListItem> lst = new List<SelectListItem>();
            DataTable dt = new DataTable();
            SqlParameter[] parameter = {
                new SqlParameter("@AppliedPostId", appliedPostId),
                new SqlParameter("@DistrictCode", districtid),
                new SqlParameter("@SchoolType", schoolType)
            };
            try
            {
                dt = sqlhelper.ExecuteDataTable("[SchoolApplication].[usp_GetPreferenceSchoolList]", parameter);
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                        lst.Add(new SelectListItem { Value = Convert.ToString(row["SchoolMasterId"]), Text = Convert.ToString(row["SchoolName"]) });
                }
            }
            catch (Exception ex)
            {
                LogBase.LogController.WriteError(ex);
            }
            return lst.ToList();
        }
        #endregion

        // Get Principal Or Teacher Oringnal and Current Post
        #region Gey Principal Or Teacher Oringnal and Current Post
        public DataSet GetOrignalCurrentPostDetail(int appliedPostId)
        {
            DataSet ds = new DataSet();
            SqlParameter[] parameter = { new SqlParameter("@AppliedPostId", appliedPostId) };
            try
            {
                ds = sqlhelper.ExecuteDataSet("[SchoolApplication].[usp_GetOrignalCurrentPostDetail]", parameter);
            }
            catch (Exception ex)
            {
                LogBase.LogController.WriteError(ex);
            }
            return ds;
        }

        #endregion

        // Insert/Submit School Application Detail
        #region Insert/Submit School Application Detail
        public DataSet InsertRegistrationDetail(SchoolApplicationViewModel objModel)
        {
            int i = 0;
            DataSet ds = new DataSet();
            DateTime? dobDate = null;
            DateTime? appointmentDate = null;
            DateTime? RetirementDate = null;
            DateTime FromDate = new DateTime();
            DateTime ToDate = new DateTime();

            DataTable dtDeputationDetail = new DataTable();
            DataTable dtSchoolExamResult = new DataTable();
            DataTable dtDPCProjectDetail = new DataTable();
            DataTable dtSchoolPreference = new DataTable();
            DataTable dtLanguage = new DataTable();
            DataTable dtComputerProficiency = new DataTable();

            if (!string.IsNullOrEmpty(objModel.DOB))
                dobDate = DateTime.ParseExact(objModel.DOB, "d/M/yyyy", CultureInfo.InvariantCulture);
            if (!string.IsNullOrEmpty(objModel.DateOfAppointment))
                appointmentDate = DateTime.ParseExact(objModel.DateOfAppointment, "d/M/yyyy", CultureInfo.InvariantCulture);
            if (!string.IsNullOrEmpty(objModel.Date_OfRetirement))
                RetirementDate = DateTime.ParseExact(objModel.Date_OfRetirement, "d/M/yyyy", CultureInfo.InvariantCulture);

            //dtDeputationDetail
            if (objModel.DeputationDetailList != null)
            {
                dtDeputationDetail.Clear();
                dtDeputationDetail.Columns.Add("DeputationDistrictCode");
                dtDeputationDetail.Columns.Add("DeputationInChargeCode");
                dtDeputationDetail.Columns.Add("DeputationInChargeNameOther");
                dtDeputationDetail.Columns.Add("DeputationFromDate");
                dtDeputationDetail.Columns.Add("DeputationToDate");
                foreach (var item in objModel.DeputationDetailList)
                {
                    DataRow _class = dtDeputationDetail.NewRow();
                    _class["DeputationDistrictCode"] = item.DeputationDistrictCode;
                    _class["DeputationInChargeCode"] = item.DeputationInChargeCode;
                    _class["DeputationInChargeNameOther"] = item.DeputationInChargeNameOther;
                    FromDate = DateTime.ParseExact(item.DeputationFromDate, "d/M/yyyy", CultureInfo.InvariantCulture);
                    _class["DeputationFromDate"] = FromDate;
                    ToDate = DateTime.ParseExact(item.DeputationToDate, "d/M/yyyy", CultureInfo.InvariantCulture);
                    _class["DeputationToDate"] = ToDate;
                    dtDeputationDetail.Rows.Add(_class);
                }
            }

            //dtSchoolExamResult
            if (objModel.SchoolExamResultList != null)
            {
                dtSchoolExamResult.Clear();
                dtSchoolExamResult.Columns.Add("ResultYear");
                dtSchoolExamResult.Columns.Add("ResultClass");
                dtSchoolExamResult.Columns.Add("DISESchoolName");
                dtSchoolExamResult.Columns.Add("PassingPercentage");
                dtSchoolExamResult.Columns.Add("FirstDivision");
                dtSchoolExamResult.Columns.Add("SecondDivision");
                foreach (var item in objModel.SchoolExamResultList)
                {
                    DataRow _class = dtSchoolExamResult.NewRow();
                    _class["ResultYear"] = item.ResultYear;
                    _class["ResultClass"] = item.ResultClass;
                    _class["DISESchoolName"] = item.DISESchoolName;
                    _class["PassingPercentage"] = item.PassingPercentage;
                    _class["FirstDivision"] = item.FirstDivision;
                    _class["SecondDivision"] = item.SecondDivision;
                    dtSchoolExamResult.Rows.Add(_class);
                }
            }

            //dtDPCProjectDetail
            if (objModel.DPCProjectDetailList != null)
            {
                dtDPCProjectDetail.Clear();
                dtDPCProjectDetail.Columns.Add("DPCProjectYear");
                dtDPCProjectDetail.Columns.Add("DPCProjectDistrict");
                dtDPCProjectDetail.Columns.Add("DPCProjectName");
                dtDPCProjectDetail.Columns.Add("DPCTotalAllocatedAmount");
                dtDPCProjectDetail.Columns.Add("DPCPerOfExpenditure");
                foreach (var item in objModel.DPCProjectDetailList)
                {
                    DataRow _class = dtDPCProjectDetail.NewRow();
                    _class["DPCProjectYear"] = item.DPCProjectYear;
                    _class["DPCProjectDistrict"] = item.DPCProjectDistrict;
                    _class["DPCProjectName"] = item.DPCProjectName;
                    _class["DPCTotalAllocatedAmount"] = item.DPCTotalAllocatedAmount;
                    _class["DPCPerOfExpenditure"] = item.DPCPerOfExpenditure;
                    dtDPCProjectDetail.Rows.Add(_class);
                }
            }

            //dtSchoolPreference
            if (objModel.SchoolPreferenceList != null)
            {
                dtSchoolPreference.Clear();
                dtSchoolPreference.Columns.Add("SchoolMasterCode");
                foreach (var item in objModel.SchoolPreferenceList)
                {
                    DataRow _class = dtSchoolPreference.NewRow();
                    _class["SchoolMasterCode"] = item.PSchoolMasterCode;
                    dtSchoolPreference.Rows.Add(_class);
                }
            }

            SqlParameter[] parameter = {
                    new SqlParameter("@tblDeputationDetail",dtDeputationDetail),
                    new SqlParameter("@tblSchoolExamResult",dtSchoolExamResult),
                    new SqlParameter("@tblDPCProjectDetail", dtDPCProjectDetail),
                    new SqlParameter("@tblSchoolPreference", dtSchoolPreference),
                    new SqlParameter("@ApplicationYear", objModel.ApplicationYearName),
                    new SqlParameter("@EmployeeCode", objModel.EmployeeCode),
                    new SqlParameter("@ProfileId", objModel.ProfileID),
                    new SqlParameter("@FullName", objModel.FullName),
                    new SqlParameter("@Gender", objModel.Gender),
                    new SqlParameter("@DOB", dobDate),
                    new SqlParameter("@Age", objModel.Age),
                    new SqlParameter("@MobileNo", objModel.MobileNo),
                    new SqlParameter("@EmailId", objModel.EmailId),
                    new SqlParameter("@AppliedPostId", objModel.AppliedPostId),
                    new SqlParameter("@AppliedSubjectId", objModel.AppliedSubjectId),
                    new SqlParameter("@AppliedSubjectOther", objModel.AppliedSubjectOther),
                    new SqlParameter("@OPostId", objModel.OPostId),
                    new SqlParameter("@OPostOther", objModel.OPostNameOther),
                    new SqlParameter("@CPostId", objModel.CPostId),
                    new SqlParameter("@CPostOther", objModel.CPostNameOther),
                    new SqlParameter("@DateOfAppointment", appointmentDate),
                    new SqlParameter("@CPostingDivisionCode", objModel.CPostingDivisionCode),
                    new SqlParameter("@CPostingDistrictCode", objModel.CPostingDistrictCode),
                    new SqlParameter("@CPostingUrbanRural", objModel.CPostingUrbanRural),
                    new SqlParameter("@CPostingBlockLocalBodyCode", objModel.CPostingBlockLocalBodyCode),
                    new SqlParameter("@CSchoolOfficeName", objModel.CSchoolOfficeName),
                    new SqlParameter("@CSchoolOfficeAddress", objModel.CSchoolOfficeAddress),
                    new SqlParameter("@SelectionProcedureId", objModel.SelectionProcedureId),
                    new SqlParameter("@SelectionProcedureOther", objModel.@SelectionProcedureOther),
                    new SqlParameter("@EducationQualification", objModel.QualificationName),
                    new SqlParameter("@AwardsDetail", objModel.AwardsDetail),
                    new SqlParameter("@NoOfYearExperience", objModel.NoOfYearExperience),
                    new SqlParameter("@PAddressLine1", objModel.PAddressLine1),
                    new SqlParameter("@PDivisionCode", objModel.PDivisionCode),
                    new SqlParameter("@PDistrictCode", objModel.PDistrictCode),
                    new SqlParameter("@PUrbanRural", objModel.PUrbanRural),
                    new SqlParameter("@PBlockLocalBodyCode", objModel.PBlockLocalBodyCode),
                    new SqlParameter("@PPinCode", objModel.PPinCode),
                    new SqlParameter("@PhotoName", objModel.PhotoName),
                    new SqlParameter("@PhotoSize", objModel.PhotoSize),
                    new SqlParameter("@PhotoPath", objModel.PhotoPath),
                    new SqlParameter("@Language", objModel.LanguageName),
                    new SqlParameter("@IsRead", objModel.IsRead),
                    new SqlParameter("@IsWrite", objModel.IsWrite),
                    new SqlParameter("@IsSpeak", objModel.IsSpeak),
                    new SqlParameter("@Computer", objModel.ComputerProficiencyName),
                    new SqlParameter("@IsExcel", objModel.IsExcel),
                    new SqlParameter("@IsWord", objModel.IsWord),
                    new SqlParameter("@IsPowerPoint", objModel.IsPowerPoint),
                    new SqlParameter("@IsDeclaration", objModel.IsDeclaration),
                    new SqlParameter("@IP_Address", objModel.IPAddress),

                    //add by yogesh 2.11.2022
                    new SqlParameter("@Name_InHindi", objModel.Name_InHindi),
                    new SqlParameter("@Adress_InHindi", objModel.Adress_InHindi),
                    new SqlParameter("@Hometown", objModel.HomeTownName),
                    new SqlParameter("@Date_OfRetirement", RetirementDate),


        };
            try
            {
                ds = sqlhelper.ExecuteDataSet("[SchoolApplication].[usp_InsertSchoolApplication]", parameter);
            }
            catch (Exception ex)
            {
                LogBase.LogController.WriteError(ex);
            }
            return ds;
        }
        #endregion

        //School Application Acknowledgement ID and Detail
        #region School Application Acknowledgement
        // Get School Application Acknowledgement ID
        public string GetApplicationAcknowledgementId(string ApplicationNumber, string profileDOB)
        {
            string criptRegistrationId = "";
            DateTime? dobDate = null;
            if (!string.IsNullOrEmpty(profileDOB))
                dobDate = DateTime.ParseExact(profileDOB, "d/M/yyyy", CultureInfo.InvariantCulture);


            SqlParameter[] parameters = {
                new SqlParameter("@ApplicationNo", ApplicationNumber),
                new SqlParameter("@DOB", dobDate),
                            };
            DataSet dt = sqlhelper.ExecuteDataSet("[SchoolApplication].[usp_GetApplicationAcknowledgmentId]", parameters);
            try
            {
                if (dt.Tables[0].Rows.Count > 0)
                {
                    criptRegistrationId = Convert.ToString(Utils.Utility.Crypt(Convert.ToString(dt.Tables[0].Rows[0]["RegistrationId"])));
                }
            }
            catch (Exception ex)
            {
                LogBase.LogController.WriteError(ex);
            }
            return criptRegistrationId;
        }
        // Get School Application Acknowledgement Detail
        public SchoolApplicationViewModel GetApplicationAcknowledgement(int pId)
        {
            SchoolApplicationViewModel bc = new SchoolApplicationViewModel();
            bc.AckDeputationDetailList = new List<DeputationDetailViewModel>();
            bc.AckSchoolExamResultList = new List<SchoolExamResultViewModel>();
            bc.AckDPCProjectDetailList = new List<DPCProjectDetailViewModel>();
            bc.AckSchoolPreferenceList = new List<SchoolPreferenceViewModel>();

            SqlParameter[] parameters = { new SqlParameter("@RegistrationId", pId), };
            //DataSet dt = sqlhelper.ExecuteDataSet("[SchoolStudent].[usp_GetPreExamAcknowledgment]", parameters);
            DataSet dt = sqlhelper.ExecuteDataSet("[SchoolApplication].[usp_GetApplicationAcknowledgment]", parameters);
            try
            {
                if (dt.Tables[0].Rows.Count > 0)
                {
                    //Basic
                    //bc.CurrentStatusId = dt.Tables[0].Rows[0]["CurrentStatusId"] == DBNull.Value ? (int?)null : Convert.ToInt32(dt.Tables[0].Rows[0]["CurrentStatusId"]);
                    //bc.CurrentStatus = Convert.ToString(dt.Tables[0].Rows[0]["CurrentStatus"]);
                    //bc.RegistrationId = Convert.ToInt32(dt.Tables[0].Rows[0]["RegistrationId"]);
                    bc.ApplicationYear = Convert.ToString(dt.Tables[0].Rows[0]["ApplicationYear"]);
                    bc.ApplicationNo = Convert.ToString(dt.Tables[0].Rows[0]["ApplicationNo"]);
                    bc.EmployeeCode = Convert.ToString(dt.Tables[0].Rows[0]["EmployeeCode"]);
                    bc.ProfileID = Convert.ToString(dt.Tables[0].Rows[0]["ProfileID"]);
                    bc.ApplicationDate = dt.Tables[0].Rows[0]["ApplicationDate"] == DBNull.Value ? (string)"" : Convert.ToDateTime(dt.Tables[0].Rows[0]["ApplicationDate"]).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                    bc.FullName = Convert.ToString(dt.Tables[0].Rows[0]["FullName"]);
                    bc.Gender = Convert.ToString(dt.Tables[0].Rows[0]["Gender"]);
                    bc.DOB = dt.Tables[0].Rows[0]["DOB"] == DBNull.Value ? (string)"" : Convert.ToDateTime(dt.Tables[0].Rows[0]["DOB"]).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                    bc.Age = Convert.ToString(dt.Tables[0].Rows[0]["Age"]);
                    bc.MobileNo = Convert.ToString(dt.Tables[0].Rows[0]["MobileNo"]);
                    bc.EmailId = Convert.ToString(dt.Tables[0].Rows[0]["EmailId"]);

                    bc.AppliedPostId = Convert.ToInt32(dt.Tables[0].Rows[0]["AppliedPostId"]);
                    bc.AppliedPostName = Convert.ToString(dt.Tables[0].Rows[0]["AppliedPostName"]);
                    if (bc.AppliedPostId == 2)
                    {
                        bc.AppliedSubjectId = Convert.ToInt32(dt.Tables[0].Rows[0]["AppliedSubjectId"]);
                        bc.AppliedSubjectName = Convert.ToString(dt.Tables[0].Rows[0]["AppliedSubjectName"]);
                        bc.AppliedSubjectOther = Convert.ToString(dt.Tables[0].Rows[0]["AppliedSubjectOther"]);
                    }
                    bc.OPostId = Convert.ToInt32(dt.Tables[0].Rows[0]["OPostId"]);
                    bc.OPostName = Convert.ToString(dt.Tables[0].Rows[0]["OPostName"]);
                    bc.OPostNameOther = Convert.ToString(dt.Tables[0].Rows[0]["OPostNameOther"]);
                    bc.CPostId = Convert.ToInt32(dt.Tables[0].Rows[0]["CPostId"]);
                    bc.CPostName = Convert.ToString(dt.Tables[0].Rows[0]["CPostName"]);
                    bc.CPostNameOther = Convert.ToString(dt.Tables[0].Rows[0]["CPostNameOther"]);
                    bc.DateOfAppointment = dt.Tables[0].Rows[0]["DateOfAppointment"] == DBNull.Value ? (string)"" : Convert.ToDateTime(dt.Tables[0].Rows[0]["DateOfAppointment"]).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                    bc.CPostingDivisionName = Convert.ToString(dt.Tables[0].Rows[0]["CPostingDivisionName"]);
                    bc.CPostingDistrictName = Convert.ToString(dt.Tables[0].Rows[0]["CPostingDistrictName"]);
                    bc.CPostingUrbanRural = Convert.ToString(dt.Tables[0].Rows[0]["CPostingUrbanRural"]);
                    bc.CPostingBlockLocalBodyName = Convert.ToString(dt.Tables[0].Rows[0]["CPostingBlockLocalBodyName"]);
                    bc.CSchoolOfficeName = Convert.ToString(dt.Tables[0].Rows[0]["CSchoolOfficeName"]);
                    bc.CSchoolOfficeAddress = Convert.ToString(dt.Tables[0].Rows[0]["CSchoolOfficeAddress"]);
                    bc.SelectionProcedureName = Convert.ToString(dt.Tables[0].Rows[0]["SelectionProcedureName"]);
                    bc.SelectionProcedureOther = Convert.ToString(dt.Tables[0].Rows[0]["SelectionProcedureOther"]);
                    bc.QualificationName = Convert.ToString(dt.Tables[0].Rows[0]["EducationQualification"]);
                    bc.AwardsDetail = Convert.ToString(dt.Tables[0].Rows[0]["AwardsDetail"]);
                    //bc.IsExperienceOfWork = Convert.ToBoolean(dt.Tables[0].Rows[0]["IsExperienceOfWork"]);
                    bc.NoOfYearExperience = Convert.ToInt32(dt.Tables[0].Rows[0]["NoOfYearExperience"]);
                    bc.PAddressLine1 = Convert.ToString(dt.Tables[0].Rows[0]["PAddressLine1"]);
                    bc.PDivisionName = Convert.ToString(dt.Tables[0].Rows[0]["PDivisionName"]);
                    bc.PDistrictName = Convert.ToString(dt.Tables[0].Rows[0]["PDistrictName"]);
                    bc.PUrbanRural = Convert.ToString(dt.Tables[0].Rows[0]["PUrbanRural"]);
                    bc.PBlockLocalBodyName = Convert.ToString(dt.Tables[0].Rows[0]["PBlockLocalBodyName"]);
                    //bc.PVillageWardName = Convert.ToString(dt.Tables[0].Rows[0]["PVillageWardName"]);
                    bc.PPinCode = Convert.ToString(dt.Tables[0].Rows[0]["PPinCode"]);
                    bc.PhotoName = Convert.ToString(dt.Tables[0].Rows[0]["PhotoName"]);
                    bc.PhotoPath = Convert.ToString(dt.Tables[0].Rows[0]["PhotoPath"]);
                    bc.LanguageName = Convert.ToString(dt.Tables[0].Rows[0]["LanguageName"]);
                    bc.IsRead = dt.Tables[0].Rows[0]["IsRead"] == DBNull.Value ? false : Convert.ToBoolean(dt.Tables[0].Rows[0]["IsRead"]);
                    bc.IsWrite = dt.Tables[0].Rows[0]["IsWrite"] == DBNull.Value ? false : Convert.ToBoolean(dt.Tables[0].Rows[0]["IsWrite"]);
                    bc.IsSpeak = dt.Tables[0].Rows[0]["IsSpeak"] == DBNull.Value ? false : Convert.ToBoolean(dt.Tables[0].Rows[0]["IsSpeak"]);
                    //bc.IsRead = dt.Tables[0].Rows[0]["IsRead"] == DBNull.Value ? false : Convert.ToBoolean(dt.Tables[0].Rows[0]["IsRead"]);
                    //bc.IsWrite = dt.Tables[0].Rows[0]["IsWrite"] == DBNull.Value ? false : Convert.ToBoolean(dt.Tables[0].Rows[0]["IsWrite"]);
                    //bc.IsSpeak = dt.Tables[0].Rows[0]["IsSpeak"] == DBNull.Value ? false : Convert.ToBoolean(dt.Tables[0].Rows[0]["IsSpeak"]);
                    bc.ComputerProficiencyName = Convert.ToString(dt.Tables[0].Rows[0]["ComputerProficiencyName"]);
                    bc.IsExcel = dt.Tables[0].Rows[0]["IsExcel"] == DBNull.Value ? false : Convert.ToBoolean(dt.Tables[0].Rows[0]["IsExcel"]);
                    bc.IsWord = dt.Tables[0].Rows[0]["IsWord"] == DBNull.Value ? false : Convert.ToBoolean(dt.Tables[0].Rows[0]["IsWord"]);
                    bc.IsPowerPoint = dt.Tables[0].Rows[0]["IsPowerPoint"] == DBNull.Value ? false : Convert.ToBoolean(dt.Tables[0].Rows[0]["IsPowerPoint"]);

                    // add by yogesh 2-11-2022
                    bc.Name_InHindi = Convert.ToString(dt.Tables[0].Rows[0]["Name_InHindi"]);
                    bc.Adress_InHindi = Convert.ToString(dt.Tables[0].Rows[0]["Adress_InHindi"]);
                    bc.Hometown = Convert.ToString(dt.Tables[0].Rows[0]["Hometown"]);
                    bc.Date_OfRetirement = dt.Tables[0].Rows[0]["Date_OfRetirement"] == DBNull.Value ? (string)"" : Convert.ToDateTime(dt.Tables[0].Rows[0]["Date_OfRetirement"]).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);


                    //DeputationDetailList
                    if (dt.Tables[1].Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Tables[1].Rows.Count; i++)
                        {
                            DeputationDetailViewModel SC = new DeputationDetailViewModel();
                            SC.SNo = Convert.ToString(i + 1);
                            SC.DeputationDistrictName = Convert.ToString(dt.Tables[1].Rows[i]["DeputationDistrictName"]);
                            SC.DeputationInChargeName = Convert.ToString(dt.Tables[1].Rows[i]["DeputationInChargeName"]);
                            SC.DeputationInChargeNameOther = Convert.ToString(dt.Tables[1].Rows[i]["DeputationInChargeNameOther"]);
                            SC.DeputationFromDate = dt.Tables[1].Rows[0]["DeputationFromDate"] == DBNull.Value ? (string)"" : Convert.ToDateTime(dt.Tables[1].Rows[i]["DeputationFromDate"]).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                            SC.DeputationToDate = dt.Tables[1].Rows[0]["DeputationToDate"] == DBNull.Value ? (string)"" : Convert.ToDateTime(dt.Tables[1].Rows[i]["DeputationToDate"]).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                            bc.AckDeputationDetailList.Add(SC);
                        }
                    }

                    //SchoolExamResultList
                    if (dt.Tables[2].Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Tables[2].Rows.Count; i++)
                        {
                            SchoolExamResultViewModel SC = new SchoolExamResultViewModel();
                            SC.SNo = Convert.ToString(i + 1);
                            SC.ResultYear = Convert.ToString(dt.Tables[2].Rows[i]["ResultYear"]);
                            SC.ResultClass = Convert.ToString(dt.Tables[2].Rows[i]["ResultClass"]);
                            SC.DISESchoolName = Convert.ToString(dt.Tables[2].Rows[i]["DISESchoolName"]);
                            SC.PassingPercentage = Convert.ToString(dt.Tables[2].Rows[i]["PassingPercentage"]);
                            SC.FirstDivision = Convert.ToString(dt.Tables[2].Rows[i]["FirstDivision"]);
                            SC.SecondDivision = Convert.ToString(dt.Tables[2].Rows[i]["SecondDivision"]);
                            bc.AckSchoolExamResultList.Add(SC);
                        }
                    }

                    // DPCProjectDetailList
                    if (dt.Tables[3].Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Tables[3].Rows.Count; i++)
                        {
                            DPCProjectDetailViewModel SC = new DPCProjectDetailViewModel();
                            SC.SNo = Convert.ToString(i + 1);
                            SC.DPCProjectYear = Convert.ToString(dt.Tables[3].Rows[i]["DPCProjectYear"]);
                            SC.DPCProjectDistrict = Convert.ToString(dt.Tables[3].Rows[i]["DPCProjectDistrictName"]);
                            SC.DPCProjectName = Convert.ToString(dt.Tables[3].Rows[i]["DPCProjectName"]);
                            SC.DPCTotalAllocatedAmount = Convert.ToString(dt.Tables[3].Rows[i]["DPCTotalAllocatedAmount"]);
                            SC.DPCPerOfExpenditure = Convert.ToString(dt.Tables[3].Rows[i]["DPCPerOfExpenditure"]);
                            bc.AckDPCProjectDetailList.Add(SC);
                        }
                    }

                    //SchoolPreference
                    if (dt.Tables[4].Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Tables[4].Rows.Count; i++)
                        {
                            SchoolPreferenceViewModel SC = new SchoolPreferenceViewModel();
                            SC.SNo = Convert.ToString(dt.Tables[4].Rows[i]["Preference"]);
                            SC.PSchoolDistrictName = Convert.ToString(dt.Tables[4].Rows[i]["PSchoolDistrictName"]);
                            SC.PSchoolTypeName = Convert.ToString(dt.Tables[4].Rows[i]["PSchoolTypeName"]);
                            SC.PSchoolSchoolName = Convert.ToString(dt.Tables[4].Rows[i]["PSchoolSchoolName"]);
                            SC.PSchoolMasterCode = Convert.ToString(dt.Tables[4].Rows[i]["SchoolMasterCode"]);
                            bc.AckSchoolPreferenceList.Add(SC);
                        }
                    }

                    if (!string.IsNullOrEmpty(Convert.ToString(dt.Tables[0].Rows[0]["PhotoPath"])))
                    {
                        if (bc.PhotoName == "Aadhar")
                        {
                            string verfilePath = Convert.ToString(dt.Tables[0].Rows[0]["PhotoPath"]);
                            verfilePath = verfilePath.Replace("\\", "/");
                            verfilePath = verfilePath.Replace(@"/", "\\");
                            string filePath = ConfigurationManager.AppSettings["AadharSSPhotoPath"].ToString() + verfilePath.Replace("~", "");
                            if (System.IO.File.Exists(filePath))
                                bc.PhotoPath = "https://www.tribal.mp.gov.in/MPTAAS/" + Convert.ToString(dt.Tables[0].Rows[0]["PhotoPath"]); //Convert.ToString(dt.Tables[0].Rows[0]["adh_photo_path"]).Replace("~", "");
                            else
                                bc.PhotoPath = "";
                        }
                        else
                        {
                            string filePath = ConfigurationManager.AppSettings["ProfileSSPhotoPath"].ToString() + Convert.ToString(dt.Tables[0].Rows[0]["PhotoPath"]);
                            if (System.IO.File.Exists(filePath))
                                bc.PhotoPath = ConfigurationManager.AppSettings["ProfileSSVirtualPath"].ToString() + Convert.ToString(dt.Tables[0].Rows[0]["PhotoPath"]); //Convert.ToString(dt.Tables[0].Rows[0]["adh_photo_path"]).Replace("~", "");
                            else
                                bc.PhotoPath = "";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogBase.LogController.WriteError(ex);
            }
            return bc;
        }
        #endregion

        //// Principal / Teachers Verification Process
        #region  Principal / Teachers Verification Process

        public DataTable GetSchoolApplicationStatusDetail(string applicationNo)
        {
            List<SchoolApplicationStatusDetail> SList = new List<SchoolApplicationStatusDetail>();
            DataTable dt = new DataTable();
            SqlParameter[] parameter = {
                new SqlParameter("@ApplicationNo",applicationNo),
            };
            try
            {
                dt = sqlhelper.ExecuteDataTable("[SchoolApplication].[usp_GetSchoolApplicationStatusDetail]", parameter);
                //if (ds.Tables[0].Rows.Count > 0)
                //{
                //    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                //    {
                //        SchoolApplicationStatusDetail bc = new SchoolApplicationStatusDetail();
                //        bc.SNo = i + 1;
                //        bc.Id = ds.Tables[0].Rows[i]["Id"] == DBNull.Value ? (int?)null : Convert.ToInt32(ds.Tables[0].Rows[i]["Id"]);
                //        bc.SMId = ds.Tables[0].Rows[i]["SMId"] == DBNull.Value ? (int?)null : Convert.ToInt32(ds.Tables[0].Rows[i]["SMId"]);
                //        bc.InfraStatusTypeId = Convert.ToInt32(ds.Tables[0].Rows[i]["InfraStatusTypeId"]);
                //        bc.InfraStatusId = Convert.ToInt32(ds.Tables[0].Rows[i]["InfraStatusId"]);
                //        bc.InfraStatusName = Convert.ToString(ds.Tables[0].Rows[i]["InfraStatusName"]);
                //        bc.IsAvailable = ds.Tables[0].Rows[i]["IsAvailable"] == DBNull.Value ? (bool?)null : Convert.ToBoolean(ds.Tables[0].Rows[i]["IsAvailable"]);
                //        bc.InfraType = ds.Tables[0].Rows[i]["InfraType"] == DBNull.Value ? (int?)null : Convert.ToInt32(ds.Tables[0].Rows[i]["InfraType"]);
                //        //bc.InfraType = new List<string>(Convert.ToString(ds.Tables[0].Rows[i]["InfraType"]).Split(new string[] { "," }, StringSplitOptions.None)).ToArray();
                //        bc.InfraAreaLengthDistance = ds.Tables[0].Rows[i]["InfraAreaLength"] == DBNull.Value ? (int?)null : Convert.ToInt32(ds.Tables[0].Rows[i]["InfraAreaLength"]);
                //        bc.InfraPhysicalCondition = ds.Tables[0].Rows[i]["InfraPhysicalCondition"] == DBNull.Value ? (int?)null : Convert.ToInt32(ds.Tables[0].Rows[i]["InfraPhysicalCondition"]);
                //        bc.InfraAdditionalRequirment = ds.Tables[0].Rows[i]["InfraAdditionalRequirment"] == DBNull.Value ? null : Convert.ToString(ds.Tables[0].Rows[i]["InfraAdditionalRequirment"]);
                //        SList.Add(bc);
                //    }
                //}
            }
            catch (Exception ex)
            {
                LogBase.LogController.WriteError(ex);
            }
            return dt;
        }
        public DataTable GetApplicationList(string applicationYear, int? districtId, int? appliedPostType, int? currentStatus)
        {
            SchoolApplicationViewModel bc = new SchoolApplicationViewModel();
            bc.AckDeputationDetailList = new List<DeputationDetailViewModel>();
            bc.AckSchoolExamResultList = new List<SchoolExamResultViewModel>();
            bc.AckDPCProjectDetailList = new List<DPCProjectDetailViewModel>();
            bc.AckSchoolPreferenceList = new List<SchoolPreferenceViewModel>();

            SqlParameter[] parameters = {
                new SqlParameter("@ApplicationYear", applicationYear),
                new SqlParameter("@DistrictCode", districtId),
                new SqlParameter("@AppliedPostId", appliedPostType),
                new SqlParameter("@CurrentStatusId", currentStatus),
            };
            DataTable dt = sqlhelper.ExecuteDataTable("[SchoolApplication].[usp_GetApplicationList]", parameters);
            return dt;
        }

        public int InsertApplicationVerifyStatus(SchoolApplicationViewModel objModel)
        {
            int i = 0;
            SqlParameter[] parameter = {
                new SqlParameter("@RegistrationId", objModel.RegistrationId),
                new SqlParameter("@ActionStatusId", objModel.CurrentStatusId),
                new SqlParameter("@ActionRemark", objModel.ActionRemark),
                new SqlParameter("@Doc1FilePath", objModel.Doc1FilePath),
                new SqlParameter("@Doc1FileName", objModel.Doc1FileName),
                new SqlParameter("@Doc1FileType", objModel.Doc1FileType),
                new SqlParameter("@Doc1FileSize", objModel.Doc1FileSize),
                new SqlParameter("@Doc2FilePath", objModel.Doc2FilePath),
                new SqlParameter("@Doc2FileName", objModel.Doc2FileName),
                new SqlParameter("@Doc2FileType", objModel.Doc2FileType),
                new SqlParameter("@Doc2FileSize", objModel.Doc2FileSize),
                new SqlParameter("@UserType", objModel.UserType),
                new SqlParameter("@UserId", objModel.UserId),
                new SqlParameter("@IPAddress", objModel.IPAddress),
            };
            try
            {
                //DataTable ds = sqlhelper.ExecuteDataTable("[SchoolPayment].[usp_InsertPoolAccountRecharge]", parameter);
                DataTable ds = sqlhelper.ExecuteDataTable("[SchoolApplication].[usp_InsertApplicationVerifyStatus]", parameter);
                i = Convert.ToInt32(ds.Rows[0]["StatusCode"]);
            }
            catch (Exception ex)
            {
                LogBase.LogController.WriteError(ex);
            }
            return i;
        }

        #endregion

        // Edit Application Form 
        #region Edit Application Form

        public DataTable GetApplicationNumber(string ApplicationNumber)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlParameter[] parameter = {
                    new SqlParameter("@ApplicationNo", ApplicationNumber)
                };

                dt = sqlhelper.ExecuteDataTable("[SchoolApplication].[School_PreferenceByApplicationNo]", parameter);
            }
            catch (Exception ex)
            {
                LogBase.LogController.WriteError(ex);
            }
            return dt;
        }

        public DataSet EditSchoolPreference(SchoolApplicationViewModel objModel)
        {
            DataSet ds = new DataSet();
            try
            {
                DataTable dtSchoolPreference = new DataTable();
                //dtSchoolPreference
                if (objModel.SchoolPreferenceList != null)
                {
                    dtSchoolPreference.Clear();
                    dtSchoolPreference.Columns.Add("SchoolMasterCode");
                    foreach (var item in objModel.SchoolPreferenceList)
                    {
                        DataRow _class = dtSchoolPreference.NewRow();
                        _class["SchoolMasterCode"] = item.PSchoolMasterCode;
                        dtSchoolPreference.Rows.Add(_class);
                    }
                }

                //SqlParameter[] parameter1 = {
                //    new SqlParameter("@ApplicationNo", objModel.ApplicationNo)
                //};

                //sqlhelper.ExecuteDataSet("[SchoolApplication].[Delete_SchoolPreference]", parameter1);

                SqlParameter[] parameter = {
                    new SqlParameter("@tblSchoolPreference", dtSchoolPreference),
                    new SqlParameter("@ApplicationYear", objModel.ApplicationYear),
                    new SqlParameter("@ApplicationNo", objModel.ApplicationNo)
                };

                ds = sqlhelper.ExecuteDataSet("[SchoolApplication].[Edit_SchoolPreference]", parameter);
            }
            catch (Exception ex)
            {
                LogBase.LogController.WriteError(ex);
            }
            return ds;
        }

        #endregion


        #region Insert Guest Faculty Master 
        public DataSet InsertGuestFacultyRegistrationDetail(RegistrationViewModel objModel)
        {
            DataSet ds = new DataSet();
            DateTime? dobDate = null;
            //if (!string.IsNullOrEmpty((objModel.DOB).ToString()))
            //    //dobDate = DateTime.ParseExact(objModel.DOB.ToString(), "d/M/yyyy", CultureInfo.InvariantCulture);



            SqlParameter[] parameter = {
                    new SqlParameter("@FirstName", objModel.FirstName),
                    new SqlParameter("@MiddleName", objModel.MiddleName),
                    new SqlParameter("@Lastname", objModel.Lastname),
                    new SqlParameter("@AddharNumber", objModel.AddharNumber),
                    new SqlParameter("@Gender", objModel.Gender),
                    new SqlParameter("@MobileNumber", objModel.MobileNumber),
                    new SqlParameter("@EmailID", objModel.EmailID),
                    new SqlParameter("@Caste ",  objModel.Caste),
                    new SqlParameter("@DOB",  objModel.DOB),
                    new SqlParameter("@Address", objModel.Address),
                    new SqlParameter("@Division", objModel.Division),
                    new SqlParameter("@City", objModel.City),
                    new SqlParameter("@Block", objModel.Block),
                    new SqlParameter("@Urban", objModel.Urban),
                    new SqlParameter("@DistrictID", objModel.DistrictID),
                    new SqlParameter("@PinCode", objModel.PinCode),
                    new SqlParameter("@ApplicantType", objModel.ApplicantType),
                    new SqlParameter("@CreatedAt",DateTime.Now),
                    new SqlParameter("@Password","123456"),
                    new SqlParameter("@ProfileImage",objModel.ProfileImage)
        };
            try
            {
                ds = sqlhelper.ExecuteDataSet("SchoolApplication.InsertGuestFacultyMaster", parameter);
            }
            catch (Exception ex)
            {
                LogBase.LogController.WriteError(ex);
            }
            return ds;
        }
        public List<SelectListItem> GetApplyForDetail()
        {
            List<SelectListItem> lst = new List<SelectListItem>();
            DataTable dt = new DataTable();
            try
            {
                //SqlParameter[] parameter = {
                //    new SqlParameter("@AppliedPostId", appliedPostId)
                //};
                dt = sqlhelper.ExecuteDataTable("[SchoolApplication].[Guest_ApplyFor]");
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                        lst.Add(new SelectListItem { Value = Convert.ToString(row["ID"]), Text = Convert.ToString(row["Name"]) });
                }
            }
            catch (Exception ex)
            {
                LogBase.LogController.WriteError(ex);
            }
            return lst.ToList();
        }
        public List<SelectListItem> GetSchoolTypeDetail()
        {
            List<SelectListItem> lst = new List<SelectListItem>();
            DataTable dt = new DataTable();
            try
            {
                //SqlParameter[] parameter = {
                //    new SqlParameter("@AppliedPostId", appliedPostId)
                //};
                dt = sqlhelper.ExecuteDataTable("[SchoolApplication].[Guest_SchoolType]");
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                        lst.Add(new SelectListItem { Value = Convert.ToString(row["ID"]), Text = Convert.ToString(row["Name"]) });
                }
            }
            catch (Exception ex)
            {
                LogBase.LogController.WriteError(ex);
            }
            return lst.ToList();
        }
        public DataTable GetVacancy()
        {
            List<SelectListItem> lst = new List<SelectListItem>();
            DataTable dt = new DataTable();
            try
            {
                dt = sqlhelper.ExecuteDataTable("[SchoolApplication].[Guest_GetVacancy]");
            }
            catch (Exception ex)
            {
                LogBase.LogController.WriteError(ex);
            }
            return dt;
        }
        public DataSet InsertGuestFacultyApplyDetail(GuestFacultyApplyViewModel objModel)
        {
            int i = 0;
            DataSet ds = new DataSet();
            DateTime? dobDate = null;
            DateTime? appointmentDate = null;
            DateTime? RetirementDate = null;
            //DateTime FromDate = new DateTime();
            //DateTime ToDate = new DateTime();

            DataTable dtQualification = new DataTable();
            DataTable dtVacancy = new DataTable();

            DataTable dtExperience = new DataTable();
            //dtQualification
            if (objModel.lstGuestFacultyQualification != null)
            {
                dtQualification.Clear();                
                dtQualification.Columns.Add("CourseType");
                dtQualification.Columns.Add("SubjectId");
                dtQualification.Columns.Add("SubjectName");
                dtQualification.Columns.Add("CoursePercentage");
                dtQualification.Columns.Add("CourseFileName");
                dtQualification.Columns.Add("CourseFilePath");
                dtQualification.Columns.Add("CreatedAt");

                foreach (var item in objModel.lstGuestFacultyQualification)
                {
                    DataRow _class = dtQualification.NewRow();
                    
                    _class["CourseType"] = item.CourseType;
                    _class["SubjectId"] = 0;
                    _class["SubjectName"] = "";
                    _class["CoursePercentage"] = item.Percentage;
                    _class["CourseFileName"] = item.CourseFileName;
                    _class["CourseFilePath"] = item.CourseFilePath;
                    _class["CreatedAt"] = DateTime.Now;

                    dtQualification.Rows.Add(_class);
                }
            }

            //dtExperience
            if (objModel.ExperienceFileName != null && objModel.ExperienceFileName != "")
            {
                dtExperience.Clear();
                dtExperience.Columns.Add("SchoolBoardTypeID");
                dtExperience.Columns.Add("SchoolBoardType");
                dtExperience.Columns.Add("ExperienceYear");
                dtExperience.Columns.Add("ExperienceFileName");
                dtExperience.Columns.Add("ExperienceFilePath");
                dtExperience.Columns.Add("CreatedAt");

                DataRow _class = dtExperience.NewRow();
                _class["SchoolBoardTypeID"] = 0;
                _class["SchoolBoardType"] = "";
                _class["ExperienceYear"] = objModel.YearsofExperience;
                _class["ExperienceFileName"] = objModel.ExperienceFileName;
                _class["ExperienceFilePath"] = objModel.ExperienceFilePath;
                _class["CreatedAt"] = DateTime.Now;
                dtExperience.Rows.Add(_class);
                //foreach (var item in objModel.lstGuestFacultyExperience)
                //{
                //    DataRow _class = dtExperience.NewRow();
                //    _class["SchoolBoardTypeID"] = item.SchoolBoardTypeID;
                //    _class["SchoolBoardType"] = item.SchoolBoardType;
                //    _class["ExperienceYear"] = item.ExperienceYear;
                //    _class["ExperienceFileName"] = item.ExperienceFileName;
                //    _class["ExperienceFileName"] = item.ExperienceFileName;
                //    _class["ExperienceFilePath"] = item.ExperienceFilePath;
                //    dtExperience.Rows.Add(_class);
                //}
            }

            //dtVacancy
            if (objModel.lstGuestFacultyVacancies != null)
            {
                dtVacancy.Clear();
                dtVacancy.Columns.Add("SubjectID");
                dtVacancy.Columns.Add("SubjectName");
                dtVacancy.Columns.Add("DistrictCode");
                dtVacancy.Columns.Add("District");
                dtVacancy.Columns.Add("SchoolTypeID");
                dtVacancy.Columns.Add("SchoolType");
                dtVacancy.Columns.Add("SchoolID");
                dtVacancy.Columns.Add("SchoolName");
                //dtVacancy.Columns.Add("Vacancies");
                dtVacancy.Columns.Add("Prefrence");
                dtVacancy.Columns.Add("CreatedAt");
                dtVacancy.Columns.Add("VacancieID");

                foreach (var item in objModel.lstGuestFacultyVacancies)
                {
                    
                    DataRow _class = dtVacancy.NewRow();
                    _class["SubjectID"] = 0;
                    _class["SubjectName"] = "";
                    _class["DistrictCode"] = 1;
                    _class["District"] = "abc";
                    _class["SchoolTypeID"] = 0;
                    _class["SchoolType"] = item.SchoolType;
                    _class["SchoolID"] = 0;
                    _class["SchoolName"] = item.SchoolName;
                    //_class["Vacancies"] = item.Vacancies;
                    _class["Prefrence"] = item.SNo;
                    _class["CreatedAt"] = DateTime.Now;
                    _class["VacancieID"] = item.VacancieID;
                    dtVacancy.Rows.Add(_class);
                }
            }

            SqlParameter[] parameter = {
                    new SqlParameter("@tblQualification",dtQualification),
                    new SqlParameter("@tblExperience",dtExperience),
                    new SqlParameter("@tblVacancy", dtVacancy),
                    new SqlParameter("@UserID", objModel.UserID),
                    new SqlParameter("@SchoolType", objModel.SchoolType),
                    new SqlParameter("@ApplyFor", objModel.ApplyFor),
                    new SqlParameter("@IsDeclaration", objModel.IsDeclaration),
                    new SqlParameter("@CreatedAt",DateTime.Now),
                    new SqlParameter("@IP", objModel.IP),
        };
            try
            {
                ds = sqlhelper.ExecuteDataSet("[SchoolApplication].[Guest_InsertApplyMaster]", parameter);
            }
            catch (Exception ex)
            {
                LogBase.LogController.WriteError(ex);
            }
            return ds;
        }
        public GuestFacultyApplyViewModel GetGuestFacultyAcknowledgement(int pId)
        {
            GuestFacultyApplyViewModel bc = new GuestFacultyApplyViewModel();
            bc.lstGuestFacultyBEdQualified = new List<GuestFacultyBEdQualified>();
            bc.lstGuestFacultyExperience = new List<GuestFacultyExperience>();
            bc.lstGuestFacultyQualification = new List<GuestFacultyQualification>();
            bc.lstGuestFacultySchoolPrefrence = new List<GuestFacultySchoolPrefrence>();
            bc.lstGuestFacultyTestQualified = new List<GuestFacultyTestQualified>();

            SqlParameter[] parameters = { new SqlParameter("@ApplyId", pId), };
            //DataSet dt = sqlhelper.ExecuteDataSet("[SchoolStudent].[usp_GetPreExamAcknowledgment]", parameters);
            DataSet dt = sqlhelper.ExecuteDataSet("[SchoolApplication].[Guest_GetPrintAcknowledgment]", parameters);
            try
            {
                if (dt.Tables[0].Rows.Count > 0)
                {
                    //Basic

                    //bc.ApplicationYear = Convert.ToString(dt.Tables[0].Rows[0]["ApplicationYear"]);
                    //bc.ApplicationNo = Convert.ToString(dt.Tables[0].Rows[0]["ApplicationNo"]);
                    //bc.EmployeeCode = Convert.ToString(dt.Tables[0].Rows[0]["EmployeeCode"]);
                    bc.UserID = Convert.ToInt32(dt.Tables[0].Rows[0]["UserID"]);
                    bc.ApplyID = Convert.ToInt32(dt.Tables[0].Rows[0]["ApplyID"]);
                    //bc.ApplicationDate = dt.Tables[0].Rows[0]["ApplicationDate"] == DBNull.Value ? (string)"" : Convert.ToDateTime(dt.Tables[0].Rows[0]["ApplicationDate"]).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                    bc.FullName = Convert.ToString(dt.Tables[0].Rows[0]["FullName"]);
                    bc.ApplyFor = Convert.ToInt32(dt.Tables[0].Rows[0]["ApplyFor"]);
                    bc.ApplyForName = Convert.ToString(dt.Tables[0].Rows[0]["ApplyForName"]);
                    bc.SchoolType = Convert.ToInt32(dt.Tables[0].Rows[0]["SchoolType"]);
                    bc.SchoolTypeName = Convert.ToString(dt.Tables[0].Rows[0]["SchoolTypeName"]);                 

                    //QualificationList
                    if (dt.Tables[1].Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Tables[1].Rows.Count; i++)
                        {
                           
                            GuestFacultyQualification SC = new GuestFacultyQualification();
                            SC.SNo = Convert.ToInt32(i + 1);
                            SC.CourseType = Convert.ToString(dt.Tables[1].Rows[i]["CourseType"]);
                            SC.Percentage = Convert.ToDecimal(dt.Tables[1].Rows[i]["CoursePercentage"]);
                            SC.CourseFileName = Convert.ToString(dt.Tables[1].Rows[i]["CourseFileName"]);
                            SC.CourseFilePath = Convert.ToString(dt.Tables[1].Rows[i]["CourseFilePath"]);
                            SC.SubjectName = Convert.ToString(dt.Tables[1].Rows[i]["SubjectName"]);
                            SC.SubjectId = Convert.ToInt32(dt.Tables[1].Rows[i]["SubjectId"]);
                            SC.QulifID = Convert.ToInt32(dt.Tables[1].Rows[i]["QulifID"]);
                            //SC.DeputationToDate = dt.Tables[1].Rows[0]["DeputationToDate"] == DBNull.Value ? (string)"" : Convert.ToDateTime(dt.Tables[1].Rows[i]["DeputationToDate"]).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                            bc.lstGuestFacultyQualification.Add(SC);
                        }
                    }
                   


                    //Experience
                    if (dt.Tables[2].Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Tables[2].Rows.Count; i++)
                        {
                            GuestFacultyExperience SC = new GuestFacultyExperience();
                            SC.SNo = Convert.ToInt32(i + 1);
                            SC.SchoolBoardType = Convert.ToString(dt.Tables[2].Rows[i]["SchoolBoardType"]);
                            SC.SchoolBoardTypeID = Convert.ToInt32(dt.Tables[2].Rows[i]["SchoolBoardTypeID"]);
                            SC.ExperienceYear = Convert.ToInt32(dt.Tables[2].Rows[i]["ExperienceYear"]);
                            SC.ExperienceFileName = Convert.ToString(dt.Tables[2].Rows[i]["ExperienceFileName"]);
                            SC.ExperienceFilePath = Convert.ToString(dt.Tables[2].Rows[i]["ExperienceFilePath"]);
                            SC.ExpID = Convert.ToInt32(dt.Tables[2].Rows[i]["ExpID"]);                          
                            bc.lstGuestFacultyExperience.Add(SC);
                        }
                    }                

                    //SchoolPrefrence
                    if (dt.Tables[3].Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Tables[3].Rows.Count; i++)
                        {
                            GuestFacultySchoolPrefrence SC = new GuestFacultySchoolPrefrence();
                            SC.SNo = Convert.ToInt32(i + 1);
                            SC.Prefrence = Convert.ToInt32(dt.Tables[4].Rows[i]["Prefrence"]);
                            SC.SubjectId = Convert.ToInt32(dt.Tables[4].Rows[i]["SubjectID"]);
                            SC.SubjectName = Convert.ToString(dt.Tables[3].Rows[i]["SubjectName"]);
                            SC.ApplicationNo = Convert.ToString(dt.Tables[3].Rows[i]["ApplicationNo"]);                            
                            SC.District = Convert.ToString(dt.Tables[3].Rows[i]["District"]);
                            SC.DistrictCode = Convert.ToInt32(dt.Tables[3].Rows[i]["DistrictCode"]);
                            SC.SchoolID = Convert.ToInt32(dt.Tables[3].Rows[i]["SchoolID"]);
                            SC.SchoolName = Convert.ToString(dt.Tables[3].Rows[i]["SchoolName"]);
                            SC.SchoolType = Convert.ToString(dt.Tables[3].Rows[i]["SchoolType"]);
                            SC.SchoolTypeID = Convert.ToInt32(dt.Tables[3].Rows[i]["SchoolTypeID"]);
                            SC.VacancieID = Convert.ToInt32(dt.Tables[3].Rows[i]["VacancieID"]);
                            SC.Vacancies = Convert.ToInt32(dt.Tables[3].Rows[i]["Vacancies"]);
                            bc.lstGuestFacultySchoolPrefrence.Add(SC);
                        }
                    }               

                    //BEdQualified
                    if (dt.Tables[4].Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Tables[4].Rows.Count; i++)
                        {
                            GuestFacultyBEdQualified SC = new GuestFacultyBEdQualified();
                            SC.SNo = Convert.ToInt32(i + 1);
                            SC.BEdQualifiedID = Convert.ToInt32(dt.Tables[4].Rows[i]["BEdQualifiedID"]);
                            SC.BEdQualifiedName = Convert.ToString(dt.Tables[4].Rows[i]["BEdQualifiedName"]);
                            SC.BEd_Equivalent = Convert.ToString(dt.Tables[4].Rows[i]["BEd_Equivalent"]);
                            SC.Percentage = Convert.ToDecimal(dt.Tables[4].Rows[i]["Percentage"]);
                            SC.QualifiedFileName = Convert.ToString(dt.Tables[4].Rows[i]["QualifiedFileName"]);
                            SC.QualifiedFilePath = Convert.ToString(dt.Tables[4].Rows[i]["QualifiedFilePath"]);                            
                        }
                    }                  
                    //TestQualified
                    if (dt.Tables[5].Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Tables[4].Rows.Count; i++)
                        {
                            GuestFacultyTestQualified SC = new GuestFacultyTestQualified();
                            SC.SNo = Convert.ToInt32(i + 1);
                            SC.TestFileName = Convert.ToString(dt.Tables[4].Rows[i]["TestFileName"]);
                            SC.TestFilePath = Convert.ToString(dt.Tables[4].Rows[i]["TestFilePath"]);
                            SC.TestID = Convert.ToInt32(dt.Tables[4].Rows[i]["TestID"]);
                            SC.TestName = Convert.ToString(dt.Tables[4].Rows[i]["TestName"]);
                            SC.Result = Convert.ToString(dt.Tables[4].Rows[i]["Result"]);
                        }
                    }

                    //if (!string.IsNullOrEmpty(Convert.ToString(dt.Tables[0].Rows[0]["PhotoPath"])))
                    //{
                    //    if (bc.PhotoName == "Aadhar")
                    //    {
                    //        string verfilePath = Convert.ToString(dt.Tables[0].Rows[0]["PhotoPath"]);
                    //        verfilePath = verfilePath.Replace("\\", "/");
                    //        verfilePath = verfilePath.Replace(@"/", "\\");
                    //        string filePath = ConfigurationManager.AppSettings["AadharSSPhotoPath"].ToString() + verfilePath.Replace("~", "");
                    //        if (System.IO.File.Exists(filePath))
                    //            bc.PhotoPath = "https://www.tribal.mp.gov.in/MPTAAS/" + Convert.ToString(dt.Tables[0].Rows[0]["PhotoPath"]); //Convert.ToString(dt.Tables[0].Rows[0]["adh_photo_path"]).Replace("~", "");
                    //        else
                    //            bc.PhotoPath = "";
                    //    }
                    //    else
                    //    {
                    //        string filePath = ConfigurationManager.AppSettings["ProfileSSPhotoPath"].ToString() + Convert.ToString(dt.Tables[0].Rows[0]["PhotoPath"]);
                    //        if (System.IO.File.Exists(filePath))
                    //            bc.PhotoPath = ConfigurationManager.AppSettings["ProfileSSVirtualPath"].ToString() + Convert.ToString(dt.Tables[0].Rows[0]["PhotoPath"]); //Convert.ToString(dt.Tables[0].Rows[0]["adh_photo_path"]).Replace("~", "");
                    //        else
                    //            bc.PhotoPath = "";
                    //    }
                    //}
                }
            }
            catch (Exception ex)
            {
                LogBase.LogController.WriteError(ex);
            }
            return bc;
        }

        #endregion
    }
}
