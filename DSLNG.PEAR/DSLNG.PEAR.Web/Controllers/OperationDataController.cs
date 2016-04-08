using System.Drawing;
using System.Text;
using AutoMapper;
using DSLNG.PEAR.Data.Enums;
using DSLNG.PEAR.Web.ViewModels.OperationData;
using DevExpress.Web.Mvc;
using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Services.Requests.OperationalData;
using DSLNG.PEAR.Web.ViewModels.OperationalData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DSLNG.PEAR.Common.Extensions;
using DSLNG.PEAR.Web.Grid;
using DSLNG.PEAR.Common.Contants;
using DevExpress.Spreadsheet;
using System.IO;
using DSLNG.PEAR.Web.ViewModels.Config;
using DSLNG.PEAR.Web.Extensions;
using DSLNG.PEAR.Services.Responses;
using DevExpress.Web;
using System.Web.UI;
using DSLNG.PEAR.Services.Requests.Operation;

namespace DSLNG.PEAR.Web.Controllers
{
    public class OperationDataController : BaseController
    {
        private readonly IOperationDataService _operationDataService;
        private readonly IDropdownService _dropdownService;
        private readonly IOperationConfigService _operationConfigService;

        public OperationDataController(IOperationDataService operationDataService,
            IDropdownService dropdownService,
            IOperationConfigService operationConfigService)
        {
            _operationDataService = operationDataService;
            _dropdownService = dropdownService;
            _operationConfigService = operationConfigService;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create()
        {
            var viewModel = new OperationalDataViewModel();
            var selectList = _operationDataService.GetOperationalSelectList();
            viewModel.KeyOperations = selectList.Operations.Select
                (x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList();

            viewModel.KPIS = selectList.KPIS.Select
                (x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList();

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Create(OperationalDataViewModel viewModel)
        {
            var request = viewModel.MapTo<SaveOperationalDataRequest>();
            var response = _operationDataService.SaveOperationalData(request);
            TempData["IsSuccess"] = response.IsSuccess;
            TempData["Message"] = response.Message;
            if (response.IsSuccess)
            {
                return RedirectToAction("Index");
            }
            return View("Create", viewModel);
        }


        public ActionResult Edit(int id)
        {
            var viewModel = _operationDataService.GetOperationalData(new GetOperationalDataRequest { Id = id }).MapTo<OperationalDataViewModel>();
            var selectList = _operationDataService.GetOperationalSelectList();
            viewModel.KeyOperations = selectList.Operations.Select
                (x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList();

            viewModel.KPIS = selectList.KPIS.Select
                (x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList();

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Edit(OperationalDataViewModel viewModel)
        {
            var request = viewModel.MapTo<SaveOperationalDataRequest>();
            var response = _operationDataService.SaveOperationalData(request);
            TempData["IsSuccess"] = response.IsSuccess;
            TempData["Message"] = response.Message;
            if (response.IsSuccess)
            {
                return RedirectToAction("Index");
            }
            return View("Edit", viewModel);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            var response = _operationDataService.DeleteOperationalData(new DeleteOperationalDataRequest { Id = id });
            TempData["IsSuccess"] = response.IsSuccess;
            TempData["Message"] = response.Message;
            if (response.IsSuccess)
            {
                return RedirectToAction("Index");
            }
            return View();
        }

        public ActionResult Grid(GridParams gridParams)
        {
            var operational = _operationDataService.GetOperationalDatas(new GetOperationalDatasRequest
                {
                    Skip = gridParams.DisplayStart,
                    Take = gridParams.DisplayLength,
                    Search = gridParams.Search,
                    SortingDictionary = gridParams.SortingDictionary
                });
            var data = new
            {
                sEcho = gridParams.Echo + 1,
                iTotalDisplayRecords = operational.TotalRecords,
                iTotalRecords = operational.OperationalDatas.Count,
                aaData = operational.OperationalDatas.Select(x => new
                {
                    x.Id,
                    x.KeyOperation,
                    x.Kpi,
                    Periode = x.Periode.ToString(DateFormat.DateForGrid),
                    x.PeriodeType,
                    x.Remark,
                    x.Scenario,
                    x.Value
                })
            };

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Detail(int id)
        {
            var response =
                _operationDataService.GetOperationalDataDetail(new GetOperationalDataDetailRequest() { Id = id });
            var viewModel = response.MapTo<OperationDataDetailViewModel>();
            viewModel.ScenarioId = id;
            return View(viewModel);
        }

        //actually it can also be processed by check if it is ajax request but you know.. deadline happens
        public ActionResult ConfigurationPartial(OperationDataParamConfigurationViewModel paramViewModel)
        {
            var viewModel = ConfigurationViewModel(paramViewModel, null);
            return PartialView("Configuration/_" + viewModel.PeriodeType, viewModel);
        }

        public ActionResult Configuration(OperationDataParamConfigurationViewModel paramViewModel)
        {
            var viewModel = ConfigurationViewModel(paramViewModel, null);
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Update(UpdateOperationDataViewModel viewModel)
        {
            var request = viewModel.MapTo<UpdateOperationDataRequest>();
            var response = _operationDataService.Update(request);
            return Json(new { Message = response.Message, isSuccess = response.IsSuccess, id = response.Id });
        }

        public ActionResult DetailPartial(OperationDataParamConfigurationViewModel paramViewModel)
        {
            return View("_DetailPartial", ConfigurationViewModel(paramViewModel, true));
        }

        public ActionResult DetailPartialPeriodeType(OperationDataParamConfigurationViewModel paramViewModel)
        {
            var viewModel = ConfigurationViewModel(paramViewModel, true);
            return PartialView("DetailPartial/_" + viewModel.PeriodeType, viewModel);
        }

        public ActionResult Upload(string configType, int scenarioId)
        {
            OperationDataDetailViewModel model = new OperationDataDetailViewModel();
            model.ConfigType = configType;
            model.ScenarioId = scenarioId;
            return PartialView("_UploadOperationData", model);
        }

        public ActionResult UploadControlCallbackAction(string configType, int scenarioId)
        {

            string[] extension = { ".xls", ".xlsx", ".csv", };
            var sourcePath = string.Format("{0}{1}/", TemplateDirectory, configType);
            var targetPath = string.Format("{0}{1}/", UploadDirectory, configType);
            ExcelUploadHelper.setPath(sourcePath, targetPath);
            ExcelUploadHelper.setValidationSettings(extension, 20971520);
            UploadControlExtension.GetUploadedFiles("uc", ExcelUploadHelper.ValidationSettings, ExcelUploadHelper.FileUploadComplete);
            return null;
        }

        public JsonResult ProceedFile(string filename, int scenarioId, string configType)
        {
            var file = string.Format("{0}{1}/{2}", UploadDirectory, configType, filename);
            var response = this._ReadExcelFile(file, scenarioId, configType);
            return Json(new { isSuccess = response.IsSuccess, Message = response.Message });
        }

        public ActionResult Download(int scenarioId)
        {
            var list = new List<SelectListItem>();
            list.Add(new SelectListItem { Text = "Yearly", Value = "Yearly" });
            list.Add(new SelectListItem { Text = "Monthly", Value = "Monthly" });
            var model = new ConfigurationViewModel()
                {
                    PeriodeType = "Yearly",
                    Year = DateTime.Now.Year,
                    Month = DateTime.Now.Month,
                    Years = _dropdownService.GetYears().MapTo<SelectListItem>(),
                    Months = _dropdownService.GetMonths().MapTo<SelectListItem>(),
                    PeriodeTypes = list
                };

            ViewBag.ScenarioId = scenarioId;
            return PartialView("_Download", model);
        }

        public ActionResult DownloadTemplate(OperationDataParamConfigurationViewModel viewModel)
        {
            var data = ConfigurationViewModel(viewModel, false);

            return ConvertToExcelFile(viewModel, data);
        }

        public ActionResult DownloadTemplateForAllGroup(OperationDataParamConfigurationViewModel paramViewModel)
        {
            PeriodeType pType = string.IsNullOrEmpty(paramViewModel.PeriodeType)
                                   ? PeriodeType.Yearly
                                   : (PeriodeType)Enum.Parse(typeof(PeriodeType), paramViewModel.PeriodeType);

            var request = paramViewModel.MapTo<GetOperationDataConfigurationRequest>();
            request.PeriodeType = pType;
            request.IsPartial = false;
            var response = _operationDataService.GetOperationDataConfigurationForAllGroup(request);
            var viewModel = response.MapTo<OperationDataConfigurationViewModel>();
            viewModel.Years = _dropdownService.GetYearsForOperationData().MapTo<SelectListItem>();
            viewModel.PeriodeType = pType.ToString();
            viewModel.Year = request.Year;
            viewModel.ConfigType = ConfigType.OperationData.ToString();
            return ConvertToExcelFile(paramViewModel, viewModel);
        }

        private ActionResult ConvertToExcelFile(OperationDataParamConfigurationViewModel viewModel,
                                                OperationDataConfigurationViewModel data)
        {
            var resultPath = Server.MapPath(string.Format("{0}{1}/", TemplateDirectory, ConfigType.OperationData));
            if (!Directory.Exists(resultPath))
            {
                Directory.CreateDirectory(resultPath);
            }

            string workSheetName = new StringBuilder(viewModel.PeriodeType).ToString();
            string dateFormat = string.Empty;
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
                    workSheetName = string.Format("{0}_{1}-{2}", workSheetName, viewModel.Year,
                                                  viewModel.Month.ToString().PadLeft(2, '0'));
                    break;
            }

           string fileName = string.Format(@"{0}.xlsx", DateTime.Now.ToString("yyyymmddMMss"));
            /*string fileName = new StringBuilder(guid).Append(".xlsx").ToString();*/

            IWorkbook workbook = new Workbook();
            Worksheet worksheet = workbook.Worksheets[0];

            worksheet.Name = workSheetName;
            workbook.Worksheets.ActiveWorksheet = worksheet;
            RowCollection rows = workbook.Worksheets[0].Rows;
            ColumnCollection columns = workbook.Worksheets[0].Columns;

            Row headerRow = rows[0];
            headerRow.FillColor = Color.DarkGray;
            headerRow.Alignment.Horizontal = SpreadsheetHorizontalAlignment.Center;
            headerRow.Alignment.Vertical = SpreadsheetVerticalAlignment.Center;
            Column kpiIdColumn = columns[0];
            Column kpiNameColumn = columns[1];
            kpiIdColumn.Visible = false;

            headerRow.Worksheet.Cells[headerRow.Index, kpiIdColumn.Index].Value = "KPI ID";
            headerRow.Worksheet.Cells[headerRow.Index, kpiNameColumn.Index].Value = "KPI Name";

            int i = 1;
            foreach (var kpi in data.Kpis)
            {
                int j = 2;
                worksheet.Cells[i, kpiIdColumn.Index].Value = kpi.Id;
                worksheet.Cells[i, kpiNameColumn.Index].Value = string.Format(@"{0} ({1})", kpi.Name, kpi.MeasurementName);

                foreach (var operationData in kpi.OperationData.OrderBy(x => x.Periode))
                {
                    worksheet.Cells[headerRow.Index, j].Value = operationData.Periode;
                    worksheet.Cells[headerRow.Index, j].NumberFormat = dateFormat;
                    worksheet.Cells[headerRow.Index, j].AutoFitColumns();

                    worksheet.Cells[i, j].Value = operationData.Value;
                    worksheet.Cells[i, j].NumberFormat = "#,0.#0";
                    worksheet.Columns[j].AutoFitColumns();
                    j++;
                }

                Column totalValueColumn = worksheet.Columns[j];
                if (i == headerRow.Index + 1)
                {
                    worksheet.Cells[headerRow.Index, totalValueColumn.Index].Value = "Average";
                    worksheet.Cells[headerRow.Index, totalValueColumn.Index + 1].Value = "SUM";
                    Range r1 = worksheet.Range.FromLTRB(kpiNameColumn.Index + 1, i, j - 1, i);
                    worksheet.Cells[i, j].Formula = string.Format("=AVERAGE({0})", r1.GetReferenceA1());
                    worksheet.Cells[i, j + 1].Formula = string.Format("=SUM({0})", r1.GetReferenceA1());
                }
                else
                {
                    // add formula
                    Range r2 = worksheet.Range.FromLTRB(kpiNameColumn.Index + 1, i, j - 1, i);
                    worksheet.Cells[i, j].Formula = string.Format("=AVERAGE({0})", r2.GetReferenceA1());
                    worksheet.Cells[i, j + 1].Formula = string.Format("=SUM({0})", r2.GetReferenceA1());
                }

                i++;
            }

            kpiNameColumn.AutoFitColumns();
            worksheet.FreezePanes(headerRow.Index, kpiNameColumn.Index);

            string resultFilePath = string.Format("{0},{1}", resultPath, fileName);

            using (FileStream stream = new FileStream(resultFilePath, FileMode.Create, FileAccess.ReadWrite))
            {
                workbook.SaveDocument(stream, DevExpress.Spreadsheet.DocumentFormat.Xlsx);
                stream.Close();
            }

            string namafile = Path.GetFileName(resultFilePath);
            byte[] fileBytes = System.IO.File.ReadAllBytes(resultFilePath);
            var response = new FileContentResult(fileBytes, "application/octet-stream") { FileDownloadName = fileName };
            return response;
        }

        private BaseResponse _ReadExcelFile(string filename, int scenarioId, string configType)
        {
            var response = new BaseResponse();
            string periodType = string.Empty;
            PeriodeType pType = PeriodeType.Yearly;
            int tahun = DateTime.Now.Year, bulan = DateTime.Now.Month;
            List<OperationDataConfigurationViewModel.Item> list_data = new List<OperationDataConfigurationViewModel.Item>();
            if (filename != Path.GetFullPath(filename))
            {
                filename = Server.MapPath(filename);
            }
            /*
             * cek file exist and return immediatelly if not exist
             */
            if (!System.IO.File.Exists(filename))
            {
                response.IsSuccess = false;
                response.Message = "File Not Found";
                return response;
            }
            Workbook workbook = new Workbook();
            using (FileStream stream = new FileStream(filename, FileMode.Open))
            {
                workbook.LoadDocument(stream, DevExpress.Spreadsheet.DocumentFormat.OpenXml);
                #region foreach
                foreach (var worksheet in workbook.Worksheets)
                {
                    string[] name = worksheet.Name.Split('_');
                    if (name[0] == "Daily" || name[0] == "Monthly" || name[0] == "Yearly")
                    {
                        periodType = name[0];
                        pType = string.IsNullOrEmpty(periodType)
                            ? PeriodeType.Yearly
                            : (PeriodeType)Enum.Parse(typeof(PeriodeType), periodType);
                        string period = name[name.Count() - 1];
                        string[] periodes = null;
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

                        workbook.Worksheets.ActiveWorksheet = worksheet;
                        //get row

                        Range range = worksheet.GetUsedRange();
                        int rows = range.RowCount;
                        int column = range.ColumnCount - 2;
                        int Kpi_Id = 0;
                        DateTime periodData = new DateTime();
                        double? nilai = null;
                        List<int> list_Kpi = new List<int>();

                        for (int i = 1; i < rows; i++)
                        {
                            for (int j = 0; j < column; j++)
                            {
                                if (j == 0)
                                {
                                    if (worksheet.Cells[i, j].Value.Type == CellValueType.Numeric)
                                    {
                                        int Kpis_Id = int.Parse(worksheet.Cells[i, j].Value.ToString());
                                        list_Kpi.Add(Kpis_Id);
                                    }
                                }
                            }
                        }
                        var operationsId = _operationConfigService.GetOperationIn(new GetOperationsInRequest { KpiIds = list_Kpi });

                        //get rows
                        for (int i = 1; i < rows; i++)
                        {
                            for (int j = 0; j < column; j++)
                            {
                                bool fromExistedToNull = false;
                                if (j == 0)
                                {
                                    if (worksheet.Cells[i, j].Value.Type == CellValueType.Numeric)
                                    {
                                        Kpi_Id = int.Parse(worksheet.Cells[i, j].Value.ToString());

                                    }

                                }
                                else if (j > 1)
                                {
                                    var operationId = 0;
                                    var operation = operationsId.KeyOperations.FirstOrDefault(x => x.KpiId == Kpi_Id);
                                    if (operation != null)
                                    {
                                        operationId = operation.Id;
                                    }

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



                                    if (nilai != null || fromExistedToNull)
                                    {
                                        // try to cacth and update
                                        var data = new OperationDataConfigurationViewModel.Item() { Value = nilai, KpiId = Kpi_Id, Periode = periodData, PeriodeType = pType, ScenarioId = scenarioId, OperationId = operationId };
                                        list_data.Add(data);
                                        //switch (configType)
                                        //{
                                        //    case "KpiTarget":
                                        //        response = this._UpdateKpiTarget(data);
                                        //        break;
                                        //    case "KpiAchievement":
                                        //        response = this._UpdateKpiAchievement(data);
                                        //        break;
                                        //    case "Economic":
                                        //        response = this._UpdateEconomic(data);
                                        //        break;
                                        //    default:
                                        //        response.IsSuccess = false;
                                        //        response.Message = "No Table Selected";
                                        //        break;
                                        //}
                                    }


                                }
                            }
                        }
                    }
                    else
                    {
                        response.IsSuccess = false;
                        response.Message = "File Not Valid";
                        break;
                    }
                    switch (configType)
                    {
                        //case "KpiTarget":
                        //    response = this._UpdateKpiTarget(list_data);
                        //    break;
                        //case "KpiAchievement":
                        //    response = this._UpdateKpiAchievement(list_data, pType.ToString(), tahun, bulan);
                        //    break;
                        case "Economic":
                            response = this._UpdateEconomic(list_data);
                            break;
                        default:
                            response.IsSuccess = false;
                            response.Message = "No Table Selected";
                            break;
                    }
                }
                #endregion
            }

            //here to read excel fileController
            return response;
        }

        private BaseResponse _UpdateEconomic(List<OperationDataConfigurationViewModel.Item> datas)
        {
            var response = new BaseResponse();
            if (datas != null)
            {
                var batch = new BatchUpdateOperationDataRequest();
                foreach (var data in datas)
                {
                    //var prepare = new UpdateOperationDataRequest() { Id = data.Id, KpiId = data.KpiId, Periode = data.Periode, Value = data.Value, PeriodeType = data.PeriodeType, Remark = data.Remark, KeyOperationConfigId = data.OperationId, ScenarioId = data.ScenarioId };// data.MapTo<UpdateKpiAchievementItemRequest>();
                    //batch.BatchUpdateOperationDataItemRequest.Add(prepare);
                }
                response = _operationDataService.BatchUpdateOperationDatas(batch);
            }
            return response;
        }

        private OperationDataConfigurationViewModel ConfigurationViewModel(OperationDataParamConfigurationViewModel paramViewModel, bool? isIncludeGroup)
        {
            PeriodeType pType = string.IsNullOrEmpty(paramViewModel.PeriodeType)
                                    ? PeriodeType.Yearly
                                    : (PeriodeType)Enum.Parse(typeof(PeriodeType), paramViewModel.PeriodeType);

            var request = paramViewModel.MapTo<GetOperationDataConfigurationRequest>();
            request.PeriodeType = pType;
            request.IsPartial = isIncludeGroup.HasValue && isIncludeGroup.Value;
            var response = _operationDataService.GetOperationDataConfiguration(request);
            var viewModel = response.MapTo<OperationDataConfigurationViewModel>();
            viewModel.Years = _dropdownService.GetYearsForOperationData().MapTo<SelectListItem>();
            viewModel.PeriodeType = pType.ToString();
            viewModel.Year = request.Year;
            viewModel.ConfigType = ConfigType.OperationData.ToString();
            return viewModel;
        }
    }


}