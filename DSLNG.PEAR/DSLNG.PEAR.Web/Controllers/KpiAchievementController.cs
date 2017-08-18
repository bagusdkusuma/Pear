using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DSLNG.PEAR.Common.Extensions;
using DSLNG.PEAR.Data.Enums;
using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Services.Requests.KpiAchievement;
using DSLNG.PEAR.Web.ViewModels.KpiAchievement;
using DevExpress.Web;
using System.Web.UI;
using DevExpress.Web.Mvc;
using System.Text;
using DevExpress.Spreadsheet;
using System.Drawing;
using System.IO;
using System.Data;
using DSLNG.PEAR.Services.Responses.KpiAchievement;

namespace DSLNG.PEAR.Web.Controllers
{
    public class KpiAchievementController : BaseController
    {
        private readonly IKpiAchievementService _kpiAchievementService;
        private readonly IDropdownService _dropdownService;
        private readonly IKpiService _kpiService;
        public KpiAchievementController(IKpiAchievementService kpiAchievementService, IDropdownService dropdownService, IKpiService kpiService)
        {
            _kpiAchievementService = kpiAchievementService;
            _dropdownService = dropdownService;
            _kpiService = kpiService;
        }

        public ActionResult Index()
        {
            var response = new AllKpiAchievementsResponse();
            bool isAdmin = this.UserProfile().IsSuperAdmin;
            var privilege = this.UserProfile().RolePrivilegeName;
            ViewBag.IsSuperAdmin = isAdmin;
            if (this.UserProfile().IsSuperAdmin)
            {
                response = _kpiAchievementService.GetAllKpiAchievements();
            }
            else
            {
                response = _kpiAchievementService.GetKpiAchievementsByRole(new GetKpiAchievementsConfigurationRequest { RoleGroupId = this.UserProfile().RoleId });
            }

            if (response.IsSuccess)
            {
                var viewModel = response.MapTo<IndexKpiAchievementViewModel>();
                return View(viewModel);
            }

            return base.ErrorPage(response.Message);
        }

        public ActionResult Update(int id, string periodeType)
        {
            int pmsSummaryId = id;
            PeriodeType pType = string.IsNullOrEmpty(periodeType)
                            ? PeriodeType.Yearly
                            : (PeriodeType)Enum.Parse(typeof(PeriodeType), periodeType);
            var request = new GetKpiAchievementsRequest { PeriodeType = pType, PmsSummaryId = pmsSummaryId };
            var response = _kpiAchievementService.GetKpiAchievements(request);
            if (response.IsSuccess)
            {
                var viewModel = response.MapTo<UpdateKpiAchievementsViewModel>();
                viewModel.PmsSummaryId = pmsSummaryId;
                viewModel.PeriodeType = pType.ToString();
                viewModel.PeriodeTypes = _dropdownService.GetPeriodeTypesForKpiTargetAndAchievement().MapTo<SelectListItem>();
                return View("Update", viewModel);
            }
            return base.ErrorPage(response.Message);
        }

        [HttpPost]
        public ActionResult Update(UpdateKpiAchievementsViewModel viewModel)
        {
            var request = viewModel.MapTo<UpdateKpiAchievementsRequest>();
            var response = _kpiAchievementService.UpdateKpiAchievements(request);
            TempData["IsSuccess"] = response.IsSuccess;
            TempData["Message"] = response.Message;
            return RedirectToAction("Update", new { id = viewModel.PmsSummaryId, periodeType = response.PeriodeType.ToString() });
        }

        public ActionResult UpdatePartial(int id, string periodeType)
        {
            int pmsSummaryId = id;
            PeriodeType pType = (PeriodeType)Enum.Parse(typeof(PeriodeType), periodeType);

            var request = new GetKpiAchievementsRequest { PeriodeType = pType, PmsSummaryId = pmsSummaryId };
            var response = _kpiAchievementService.GetKpiAchievements(request);
            string view = pType == PeriodeType.Yearly ? "_yearly" : "_monthly";
            if (response.IsSuccess)
            {
                var viewModel = response.MapTo<UpdateKpiAchievementsViewModel>();
                viewModel.PeriodeType = pType.ToString();
                viewModel.PmsSummaryId = pmsSummaryId;
                return PartialView(view, viewModel);
            }

            return Content(response.Message);
        }

        public ActionResult Configuration(ConfigurationParamViewModel paramViewModel)
        {
            int roleGroupId = paramViewModel.Id;
            PeriodeType pType = string.IsNullOrEmpty(paramViewModel.PeriodeType)
                                    ? PeriodeType.Yearly
                                    : (PeriodeType)Enum.Parse(typeof(PeriodeType), paramViewModel.PeriodeType);

            var request = new GetKpiAchievementsConfigurationRequest();
            request.PeriodeType = pType.ToString();
            request.RoleGroupId = roleGroupId;
            request.Year = paramViewModel.Year;
            request.Month = paramViewModel.Month;
            var response = _kpiAchievementService.GetKpiAchievementsConfiguration(request);
            if (response.IsSuccess)
            {
                var viewModel = response.MapTo<ConfigurationKpiAchievementsViewModel>();
                viewModel.Year = request.Year;
                viewModel.Month = request.Month;
                viewModel.Years = _dropdownService.GetYears().MapTo<SelectListItem>();
                viewModel.Months = _dropdownService.GetMonths().MapTo<SelectListItem>();
                viewModel.PeriodeType = pType.ToString();
                //viewModel.FileName = this._ExportToExcel(viewModel);
                return View(viewModel);
            }

            return base.ErrorPage(response.Message);

        }

        public ActionResult ConfigurationPartial(ConfigurationParamViewModel paramViewModel)
        {
            int roleGroupId = paramViewModel.Id;
            PeriodeType pType = string.IsNullOrEmpty(paramViewModel.PeriodeType)
                                    ? PeriodeType.Yearly
                                    : (PeriodeType)Enum.Parse(typeof(PeriodeType), paramViewModel.PeriodeType);

            var request = new GetKpiAchievementsConfigurationRequest();
            request.PeriodeType = pType.ToString();
            request.RoleGroupId = roleGroupId;
            request.Year = paramViewModel.Year;
            request.Month = paramViewModel.Month;
            var response = _kpiAchievementService.GetKpiAchievementsConfiguration(request);
            if (response.IsSuccess)
            {
                var viewModel = response.MapTo<ConfigurationKpiAchievementsViewModel>();
                viewModel.Year = request.Year;
                viewModel.Month = request.Month;
                viewModel.Years = _dropdownService.GetYears().MapTo<SelectListItem>();
                viewModel.Months = _dropdownService.GetMonths().MapTo<SelectListItem>();
                viewModel.PeriodeType = pType.ToString();
                //viewModel.FileName = this._ExportToExcel(viewModel);
                return PartialView("Configuration/_" + pType.ToString(), viewModel);
            }

            return base.ErrorPage(response.Message);
        }

        public FileResult DownloadTemplate(string filename)
        {
            var file = Server.MapPath(filename);
            //string[] filePaths = Directory.GetFiles(file);
            if (!System.IO.File.Exists(file))
            {
                return null;
            }
            string namafile = Path.GetFileName(file);
            byte[] fileBytes = System.IO.File.ReadAllBytes(file);
            var response = new FileContentResult(fileBytes, "application/octet-stream");
            response.FileDownloadName = namafile;
            return response;
        }

        public class ReadExcelFileModel
        {
            public bool isSuccess { get; set; }
            public string Message { get; set; }
            public int Success { get; set; }
            public int Skipped { get; set; }
            public int Rejected { get; set; }

        }

        public JsonResult ProceedFile(string filename)
        {
            var response = this._ReadExcelFile(UploadDirectory + filename);
            return Json(new { isSuccess = response.isSuccess, Message = response.Message, Succeed = response.Success, skipped = response.Skipped, rejected = response.Rejected });
        }

        private ReadExcelFileModel _ReadExcelFile(string filename)
        {
            var response = new ReadExcelFileModel();
            try
            {
                int inserted = 0;
                int skipped = 0;
                int rejected = 0;
                var listPrev = new List<UpdateKpiAchievementsViewModel.KpiAchievementItem>();

                var file = Server.MapPath(filename);
                var userId = UserProfile().UserId;
                Workbook workbook = new Workbook();
                using (FileStream stream = new FileStream(file, FileMode.Open))
                {
                    workbook.LoadDocument(stream, DevExpress.Spreadsheet.DocumentFormat.OpenXml);
                    foreach (var worksheet in workbook.Worksheets)
                    {
                        string[] name = worksheet.Name.Split('_');
                        //if (name.Count() > 0 && name[0] != "Sheet1" && (name[0] == PeriodeType.Daily.ToString() || name[0] == PeriodeType.Hourly.ToString() || name[0] == PeriodeType.Monthly.ToString() || name[0] == PeriodeType.Weekly.ToString() || name[0] == PeriodeType.Yearly.ToString()))
                        if (name[0] == "Daily" || name[0] == "Monthly" || name[0] == "Yearly")
                        {

                            string periodType = name[0];
                            PeriodeType pType = string.IsNullOrEmpty(periodType)
                                                    ? PeriodeType.Yearly
                                                    : (PeriodeType)Enum.Parse(typeof(PeriodeType), periodType);
                            string period = name[name.Count() - 1];
                            string[] periodes = null;
                            int tahun, bulan;
                            //validate and switch value by periodType
                            if (periodType != period && !string.IsNullOrEmpty(period))
                            {
                                switch (periodType)
                                {
                                    case "Daily":
                                        periodes = period.Split('-');
                                        tahun = int.Parse(periodes[0]);
                                        bulan = int.Parse(periodes[periodes.Count() - 1]);
                                        break;
                                    case "Monthly":
                                        tahun = int.Parse(period);
                                        break;
                                    case "Yearly":
                                    default:
                                        break;
                                }
                            }

                            //coba baca value
                            workbook.Worksheets.ActiveWorksheet = worksheet;
                            //get row

                            Range range = worksheet.GetUsedRange();
                            int rows = range.RowCount;
                            int column = range.ColumnCount - 2;
                            int kpiId = 0;
                            DateTime periodData = new DateTime();
                            double? nilai = null;
                            for (int i = 1; i < rows; i++)
                            {
                                //get rows
                                for (int j = 0; j < column; j++)
                                {
                                    bool fromExistedToNull = false;
                                    var prepareDataContainer = new UpdateKpiAchievementsViewModel.KpiAchievementItem();
                                    //get rows header and period
                                    if (j == 0)
                                    {
                                        if (worksheet.Cells[i, j].Value.Type == CellValueType.Numeric)
                                        {
                                            kpiId = int.Parse(worksheet.Cells[i, j].Value.ToString());
                                        }
                                    }
                                    else if (j > 1)
                                    {
                                        if (worksheet.Cells[0, j].Value.Type == CellValueType.DateTime)
                                        {
                                            periodData = DateTime.Parse(worksheet.Cells[0, j].Value.ToString());
                                        }

                                        if (worksheet.Cells[i, j].Value.Type == CellValueType.Numeric)
                                        {
                                            nilai = double.Parse(worksheet.Cells[i, j].Value.ToString());
                                        }
                                        else if (worksheet.Cells[i, j].Value.Type == CellValueType.Text)
                                        {
                                            fromExistedToNull = true;
                                            nilai = null;
                                        }
                                        else
                                        {
                                            nilai = null;
                                        }

                                        bool isValidKpi = false;
                                        if (!this.UserProfile().IsSuperAdmin)
                                        {
                                            skipped++;
                                            isValidKpi =
                                                _kpiService.IsValidKpi(new Services.Requests.Kpi.GetKpiByRole
                                                    {
                                                        RoleId = this.UserProfile().RoleId
                                                    });
                                        }
                                        else
                                        {
                                            isValidKpi = true;
                                        }

                                        if (isValidKpi && (nilai != null || fromExistedToNull))
                                        {
                                            prepareDataContainer.Value = nilai.ToString();
                                            prepareDataContainer.KpiId = kpiId;
                                            prepareDataContainer.Periode = periodData;
                                            prepareDataContainer.PeriodeType = pType;
                                            var oldKpiAchievement =
                                                _kpiAchievementService.GetKpiAchievementByValue(
                                                    new GetKpiAchievementRequestByValue
                                                        {
                                                            KpiId = kpiId,
                                                            Periode = periodData,
                                                            PeriodeType = periodType
                                                        });
                                            if (oldKpiAchievement.IsSuccess)
                                            {
                                                prepareDataContainer.Id = oldKpiAchievement.Id;
                                            }
                                            else
                                            {
                                                var fixedError = false;
                                                if (oldKpiAchievement.ExceptionType != null &&
                                                    oldKpiAchievement.ExceptionType ==
                                                    typeof(InvalidOperationException))
                                                {
                                                    //var delete = _kpiAchievementService.DeleteKpiAchievement(kpiId, periodData, pType);
                                                    var delete = _kpiAchievementService.DeleteKpiAchievement(new DeleteKpiAchievementRequest {
                                                        kpiId = kpiId,
                                                        periode = periodData,
                                                        periodeType = pType,
                                                        ControllerName = "KPI Achievement",
                                                        ActionName = "Read Excel File",
                                                        UserId = UserProfile().UserId
                                                    });
                                                    fixedError = delete.IsSuccess;
                                                }

                                                if (!fixedError)
                                                {
                                                    throw new Exception(string.Format(@"KPI with id {0} and periode {1} and periode type {2} can't be inserted", kpiId, periodData.ToShortDateString(), pType.ToString()));
                                                }
                                            }
                                            var request = prepareDataContainer.MapTo<UpdateKpiAchievementItemRequest>();
                                            request.UserId = userId;
                                            request.UpdateFrom = "KPIAchievementForm";
                                            var insert = _kpiAchievementService.UpdateKpiAchievementItem(request);
                                            if (insert.IsSuccess)
                                            {
                                                inserted++;
                                            }
                                            else
                                            {
                                                rejected++;
                                            }
                                        }
                                    }
                                }
                            }

                            response.isSuccess = true;
                            response.Message = "Success :" + inserted + "\r\n";
                            response.Message += "Skipped :" + skipped + "\r\n";
                            response.Message += "Rejected :" + rejected + "\r\n";
                        }
                        else
                        {
                            response.isSuccess = false;
                            response.Message = "File Not Valid";
                            break;
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                response.Message = exception.Message;
                response.isSuccess = false;
            }

            return response;
        }

        private string _ExportToExcel(ConfigurationKpiAchievementsViewModel viewModel)
        {
            string dateFormat = "dd-mmm-yy";
            string workSheetName = new StringBuilder(viewModel.PeriodeType).ToString();
            switch (viewModel.PeriodeType)
            {
                case "Yearly":
                    dateFormat = "yyyy";
                    break;
                case "Monthly":
                    dateFormat = "mmm-yy";
                    workSheetName = string.Format("{0}_{1}", workSheetName, viewModel.Year);
                    break;
                default:
                    dateFormat = "dd-mmm-yy";
                    workSheetName = string.Format("{0}_{1}-{2}", workSheetName, viewModel.Year, viewModel.Month.ToString().PadLeft(2, '0'));
                    break;
            }
            string fileName = new StringBuilder(workSheetName).Append(".xls").ToString();
            var path = System.Web.HttpContext.Current.Request.MapPath(TemplateDirectory + "/KpiAchievement/");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string resultFilePath = System.Web.HttpContext.Current.Request.MapPath(string.Format("{0}/KpiAchievement/{1}", TemplateDirectory, fileName));
            Workbook workbook = new Workbook();
            Worksheet worksheet = workbook.Worksheets[0];

            worksheet.Name = workSheetName;
            workbook.Worksheets.ActiveWorksheet = worksheet;

            RowCollection rows = workbook.Worksheets[0].Rows;
            ColumnCollection columns = workbook.Worksheets[0].Columns;

            Row HeaderRow = rows[0];
            HeaderRow.FillColor = Color.DarkGray;
            HeaderRow.Alignment.Horizontal = SpreadsheetHorizontalAlignment.Center;
            HeaderRow.Alignment.Vertical = SpreadsheetVerticalAlignment.Center;
            Column KpiIdColumn = columns[0];
            Column KpiNameColumn = columns[1];
            KpiIdColumn.Visible = false;

            HeaderRow.Worksheet.Cells[HeaderRow.Index, KpiIdColumn.Index].Value = "KPI ID";
            HeaderRow.Worksheet.Cells[HeaderRow.Index, KpiNameColumn.Index].Value = "KPI Name";
            int i = 1; //i for row
            foreach (var kpi in viewModel.Kpis)
            {
                worksheet.Cells[i, KpiIdColumn.Index].Value = kpi.Id;
                worksheet.Cells[i, KpiNameColumn.Index].Value = string.Format("{0} ({1})", kpi.Name, kpi.Measurement);
                int j = 2; // for column

                foreach (var achievement in kpi.KpiAchievements)
                {
                    worksheet.Cells[HeaderRow.Index, j].Value = achievement.Periode;
                    worksheet.Cells[HeaderRow.Index, j].NumberFormat = dateFormat;
                    worksheet.Cells[HeaderRow.Index, j].AutoFitColumns();

                    worksheet.Cells[i, j].Value = achievement.Value;
                    worksheet.Cells[i, j].NumberFormat = "#,0.#0";
                    worksheet.Columns[j].AutoFitColumns();
                    j++;
                }
                Column TotalValueColumn = worksheet.Columns[j];
                if (i == HeaderRow.Index + 1)
                {
                    worksheet.Cells[HeaderRow.Index, TotalValueColumn.Index].Value = "Average";
                    worksheet.Cells[HeaderRow.Index, TotalValueColumn.Index + 1].Value = "SUM";
                    Range r1 = worksheet.Range.FromLTRB(KpiNameColumn.Index + 1, i, j - 1, i);
                    worksheet.Cells[i, j].Formula = string.Format("=AVERAGE({0})", r1.GetReferenceA1());
                    worksheet.Cells[i, j + 1].Formula = string.Format("=SUM({0})", r1.GetReferenceA1());
                }
                else
                {
                    // add formula
                    Range r2 = worksheet.Range.FromLTRB(KpiNameColumn.Index + 1, i, j - 1, i);
                    worksheet.Cells[i, j].Formula = string.Format("=AVERAGE({0})", r2.GetReferenceA1());
                    worksheet.Cells[i, j + 1].Formula = string.Format("=SUM({0})", r2.GetReferenceA1());
                }
                i++;
            }

            KpiNameColumn.AutoFitColumns();
            worksheet.FreezePanes(HeaderRow.Index, KpiNameColumn.Index);

            using (FileStream stream = new FileStream(resultFilePath, FileMode.Create, FileAccess.ReadWrite))
            {
                workbook.SaveDocument(stream, DevExpress.Spreadsheet.DocumentFormat.Xlsx);
                stream.Close();
            }

            //workbook.SaveDocument(resultFilePath, DocumentFormat.OpenXml);
            //todo create file from viewModel
            return string.Format("{0}KpiAchievement/{1}", TemplateDirectory, fileName);
        }


        [HttpPost]
        public JsonResult KpiAchievementItem(UpdateKpiAchievementsViewModel.KpiAchievementItem kpiAchievement)
        {
            var request = kpiAchievement.MapTo<UpdateKpiAchievementItemRequest>();
            request.UserId = this.UserProfile().UserId;
            request.UpdateFrom = "KPIAchievementForm";
            var response = _kpiAchievementService.UpdateKpiAchievementItem(request);
            //var response = _kpiAchievementService.UpdateOriginalData(request);
            return Json(new { Id = response.Id, Message = response.Message, isSuccess = response.IsSuccess });
        }

        public ActionResult UploadControlCallbackAction()
        {
            UploadControlExtension.GetUploadedFiles("uc", UploadControlHelper.ValidationSettings, UploadControlHelper.FileUploadComplete);
            return null;
        }

        public class UploadControlHelper
        {
            public static readonly UploadControlValidationSettings ValidationSettings = new UploadControlValidationSettings
            {
                AllowedFileExtensions = new string[] { ".xls", ".xlsx", ".csv", },
                MaxFileSize = 20971520,
            };

            public static void FileUploadComplete(object sender, FileUploadCompleteEventArgs e)
            {
                if (e.UploadedFile.IsValid)
                {
                    var path = System.Web.HttpContext.Current.Request.MapPath(UploadDirectory);
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    string resultFilePath = System.Web.HttpContext.Current.Request.MapPath(UploadDirectory + e.UploadedFile.FileName);
                    e.UploadedFile.SaveAs(resultFilePath, true);//Code Central Mode - Uncomment This Line
                    IUrlResolutionService urlResolver = sender as IUrlResolutionService;
                    if (urlResolver != null)
                    {
                        e.CallbackData = Path.GetFileName(resultFilePath);//urlResolver.ResolveClientUrl(resultFilePath);
                    }
                }
            }
        }
    }

}