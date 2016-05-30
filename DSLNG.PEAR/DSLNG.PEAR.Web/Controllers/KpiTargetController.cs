using DSLNG.PEAR.Data.Enums;
using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Services.Responses.KpiTarget;
using DSLNG.PEAR.Web.ViewModels.KpiTarget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DSLNG.PEAR.Common.Extensions;
using DSLNG.PEAR.Services.Requests.KpiTarget;
using DevExpress.Web.Mvc;
using DSLNG.PEAR.Web.Extensions;
using System.Text;
using System.IO;
using DevExpress.Spreadsheet;
using System.Drawing;
using DSLNG.PEAR.Web.Attributes;

namespace DSLNG.PEAR.Web.Controllers
{
    [Authorize]
    public class KpiTargetController : BaseController
    {
        private readonly IKpiTargetService _kpiTargetService;
        private readonly IDropdownService _dropdownService;

        public KpiTargetController(IKpiTargetService kpiTargetService, IDropdownService dropdownService)
        {
            _kpiTargetService = kpiTargetService;
            _dropdownService = dropdownService;
        }

        [AuthorizeUser(AccessLevel="AllowUpdate")]
        public ActionResult Update(int id, string periodeType)
        {
            int pmsSummaryId = id;
            PeriodeType pType = string.IsNullOrEmpty(periodeType)
                            ? PeriodeType.Yearly
                            : (PeriodeType)Enum.Parse(typeof(PeriodeType), periodeType);
            var request = new GetKpiTargetRequest { PeriodeType = pType, PmsSummaryId = pmsSummaryId };
            var response = _kpiTargetService.GetKpiTarget(request);
            if (response.IsSuccess)
            {
                var viewModel = response.MapTo<UpdateKpiTargetViewModel>();
                viewModel.PmsSummaryId = pmsSummaryId;
                viewModel.PeriodeType = pType.ToString();
                viewModel.PeriodeTypes = _dropdownService.GetPeriodeTypesForKpiTargetAndAchievement().MapTo<SelectListItem>();
                return View("Update", viewModel);
            }
            return base.ErrorPage(response.Message);
        }

        [HttpPost]
        public ActionResult Update(UpdateKpiTargetViewModel viewModel)
        {
            var request = viewModel.MapTo<UpdateKpiTargetRequest>();
            var response = _kpiTargetService.UpdateKpiTarget(request);
            TempData["IsSuccess"] = response.IsSuccess;
            TempData["Message"] = response.Message;
            return RedirectToAction("Update", new { id = viewModel.PmsSummaryId, periodeType = response.PeriodeType.ToString() });
        }

        public ActionResult UpdatePartial(int id, string periodeType)
        {
            int pmsSummaryId = id;
            PeriodeType pType = (PeriodeType)Enum.Parse(typeof(PeriodeType), periodeType);

            var request = new GetKpiTargetRequest { PeriodeType = pType, PmsSummaryId = pmsSummaryId };
            var response = _kpiTargetService.GetKpiTarget(request);
            string view = pType == PeriodeType.Yearly ? "_yearly" : "_monthly";
            if (response.IsSuccess)
            {
                var viewModel = response.MapTo<UpdateKpiTargetViewModel>();
                viewModel.PeriodeType = pType.ToString();
                viewModel.PmsSummaryId = pmsSummaryId;
                return PartialView(view, viewModel);
            }

            return Content(response.Message);
        }
        [AuthorizeUser(AccessLevel = "AllowUpdate")]
        public ActionResult Configuration(ConfigurationParamViewModel paramViewModel)
        {
            int roleGroupId = paramViewModel.Id;
            PeriodeType pType = string.IsNullOrEmpty(paramViewModel.PeriodeType)
                                    ? PeriodeType.Yearly
                                    : (PeriodeType)Enum.Parse(typeof(PeriodeType), paramViewModel.PeriodeType);

            var request = new GetKpiTargetsConfigurationRequest();
            request.PeriodeType = pType.ToString();
            request.RoleGroupId = roleGroupId;
            request.Year = paramViewModel.Year;
            request.Month = paramViewModel.Month;
            var response = _kpiTargetService.GetKpiTargetsConfiguration(request);
            if (response.IsSuccess)
            {
                var viewModel = response.MapTo<ConfigurationKpiTargetsViewModel>();
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
        private string _ExportToExcel(ConfigurationKpiTargetsViewModel viewModel)
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
            var path = System.Web.HttpContext.Current.Request.MapPath(TemplateDirectory + "/KpiTarget/");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string resultFilePath = System.Web.HttpContext.Current.Request.MapPath(string.Format("{0}/KpiTarget/{1}", TemplateDirectory, fileName));
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

                foreach (var target in kpi.KpiTargets)
                {
                    worksheet.Cells[HeaderRow.Index, j].Value = target.Periode;
                    worksheet.Cells[HeaderRow.Index, j].NumberFormat = dateFormat;
                    worksheet.Cells[HeaderRow.Index, j].AutoFitColumns();

                    worksheet.Cells[i, j].Value = target.Value;
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
            return string.Format("{0}KpiTarget/{1}", TemplateDirectory,fileName);
        }

        public ActionResult ConfigurationPartial(ConfigurationParamViewModel paramViewModel)
        {
            int roleGroupId = paramViewModel.Id;
            PeriodeType pType = string.IsNullOrEmpty(paramViewModel.PeriodeType)
                                    ? PeriodeType.Yearly
                                    : (PeriodeType)Enum.Parse(typeof(PeriodeType), paramViewModel.PeriodeType);

            var request = new GetKpiTargetsConfigurationRequest();
            request.PeriodeType = pType.ToString();
            request.RoleGroupId = roleGroupId;
            request.Year = paramViewModel.Year;
            request.Month = paramViewModel.Month;
            var response = _kpiTargetService.GetKpiTargetsConfiguration(request);
            if (response.IsSuccess)
            {
                var viewModel = response.MapTo<ConfigurationKpiTargetsViewModel>();
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
        [AuthorizeUser(AccessLevel = "AllowView")]
        public ActionResult Index()
        {
            var isAdmin = this.UserProfile().IsSuperAdmin;
            ViewBag.IsSuperAdmin = isAdmin;
            var response = new AllKpiTargetsResponse();

            if (isAdmin)
            {
                response = _kpiTargetService.GetAllKpiTargets();
            }
            else
            {
                response = _kpiTargetService.GetAllKpiTargetByRole(new GetKpiTargetsConfigurationRequest { RoleGroupId = this.UserProfile().RoleId });
            }
            if (response.IsSuccess)
            {
                var viewModel = response.MapTo<IndexKpiTargetViewModel>();
                return View(viewModel);
            }

            return base.ErrorPage(response.Message);
        }

        /*public ActionResult IndexPartial()
        {
            var viewModel = GridViewExtension.GetViewModel("gridKpiTargetIndex");
            if (viewModel == null)
                viewModel = CreateGridViewModel();
            return BindingCore(viewModel);
        }

        PartialViewResult BindingCore(GridViewModel gridViewModel)
        {
            gridViewModel.ProcessCustomBinding(
                GetDataRowCount,
                GetData
            );
            return PartialView("_GridViewPartial", gridViewModel);
        }

        static GridViewModel CreateGridViewModel()
        {
            var viewModel = new GridViewModel() { KeyFieldName = "Id" };
            viewModel.Columns.Add("KpiName");
            viewModel.Columns.Add("PeriodeType");
            viewModel.Columns.Add("Value");
            viewModel.Columns.Add("IsActive");
            viewModel.Pager.PageSize = 10;
            return viewModel;
        }

        public ActionResult PagingAction(GridViewPagerState pager)
        {
            var viewModel = GridViewExtension.GetViewModel("gridKpiTargetIndex");
            viewModel.ApplyPagingState(pager);
            return BindingCore(viewModel);
        }

        public void GetDataRowCount(GridViewCustomBindingGetDataRowCountArgs e)
        {

            e.DataRowCount = _kpiTargetService.GetKpiTargets(new GetKpiTargetsRequest { Take = 0, Skip = 0 }).KpiTargets.Count;
        }

        public void GetData(GridViewCustomBindingGetDataArgs e)
        {
            e.Data = _kpiTargetService.GetKpiTargets(new GetKpiTargetsRequest
            {
                Skip = e.StartDataRowIndex,
                Take = e.DataRowCount
            }).KpiTargets;
        }*/

        public ActionResult Create()
        {
            var viewModel = new CreateKpiTargetViewModel();
            viewModel = SetViewModel(viewModel);
            return View(viewModel);
        }

        public CreateKpiTargetViewModel SetViewModel(CreateKpiTargetViewModel viewModel)
        {
            var pmsConfigs = _kpiTargetService.GetPmsConfigs(new GetPmsConfigsRequest { Id = 1 }).PmsConfigs;
            if (pmsConfigs.Count > 0)
            {
                foreach (var pmsConfig in pmsConfigs)
                {
                    var pillarSelectListItem = new List<SelectListItem>();
                    pillarSelectListItem.Add(new SelectListItem { Text = pmsConfig.Pillar.Name, Value = pmsConfig.Pillar.Id.ToString() });
                    var pmsConfigDetails = pmsConfig.PmsConfigDetailsList;
                    if (pmsConfigDetails.Count > 0)
                    {
                        var kpiTargetList = new List<KpiTarget>();
                        foreach (var pmsConfigDetail in pmsConfigDetails)
                        {
                            var kpiSelectListItem = new List<SelectListItem>();
                            kpiSelectListItem.Add(new SelectListItem { Text = pmsConfigDetail.Kpi.Name, Value = pmsConfigDetail.Kpi.Id.ToString() });
                            var kpi = pmsConfigDetail.Kpi.MapTo<Kpi>();
                            kpiTargetList.Add(new KpiTarget
                            {
                                Kpi = kpi,
                                KpiList = kpiSelectListItem,
                                Periode = new DateTime(pmsConfig.PmsSummary.Year, 1, 1),
                                KpiId = pmsConfigDetail.Kpi.Id
                                //IsActive = pmsConfig.IsActive 
                            });
                        }
                        viewModel.PillarKpiTarget.Add(new PillarTarget
                        {
                            PillarList = pillarSelectListItem,
                            KpiTargetList = kpiTargetList
                        });
                    }
                }
            }
            return viewModel;
        }

        [HttpPost]
        public ActionResult Create(CreateKpiTargetViewModel viewModel)
        {
            if (viewModel.PillarKpiTarget.Count > 0)
            {
                var request = new CreateKpiTargetsRequest();
                request.KpiTargets = new List<CreateKpiTargetsRequest.KpiTarget>();
                foreach (var item in viewModel.PillarKpiTarget)
                {
                    if (item.KpiTargetList.Count > 0)
                    {
                        foreach (var kpi in item.KpiTargetList)
                        {
                            request.KpiTargets.Add(new CreateKpiTargetsRequest.KpiTarget
                            {
                                IsActive = true,
                                KpiId = kpi.KpiId,
                                Periode = kpi.Periode,
                                PeriodeType = (DSLNG.PEAR.Data.Enums.PeriodeType)kpi.PeriodeType,
                                Remark = kpi.Remark,
                                Value = kpi.Value
                            });
                        }
                    }
                }

                var response = _kpiTargetService.Creates(request);
                TempData["IsSuccess"] = response.IsSuccess;
                TempData["Message"] = response.Message;
                if (response.IsSuccess)
                {
                    return RedirectToAction("Index");
                }
            }
            viewModel = SetViewModel(viewModel);
            return View(viewModel);
        }


        [HttpPost]
        public JsonResult KpiTargetItem(KpiTargetItem kpiTarget)
        {
            /*if (kpiTarget.Id > 0)
            {
                var request = kpiTarget.MapTo<UpdateKpiTargetItemRequest>();
                var response = _kpiTargetService.UpdateKpiTargetItem(request);
                return Json(new { Id = response.Id, Message = response.Message, isSuccess = response.IsSuccess });
            }
            else
            {
                var request = kpiTarget.MapTo<CreateKpiTargetRequest>();
                var response = _kpiTargetService.Create(request);
                return Json(new { Id = response.Id, Message = response.Message, isSuccess = response.IsSuccess });
            }*/

            var request = kpiTarget.MapTo<UpdateKpiTargetItemRequest>();
            request.UserId = this.UserProfile().UserId;
            var response = _kpiTargetService.UpdateKpiTargetItem(request);
            return Json(new { Id = response.Id, Message = response.Message, isSuccess = response.IsSuccess });
        }

        public ActionResult UploadControlCallbackAction()
        {
            string[] extension = { ".xls", ".xlsx", ".csv", };

            ExcelUploadHelper.setPath(TemplateDirectory + "Target/", UploadDirectory + "Target/");
            ExcelUploadHelper.setValidationSettings(extension, 20971520);

            UploadControlExtension.GetUploadedFiles("uc", ExcelUploadHelper.ValidationSettings, ExcelUploadHelper.FileUploadComplete);
            //UploadControlExtension.GetUploadedFiles("uc", UploadControlHelper.ValidationSettings, UploadControlHelper.FileUploadComplete);
            return null;
        }
    }
}