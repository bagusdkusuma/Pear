using DSLNG.PEAR.Data.Enums;
using DSLNG.PEAR.Common.Extensions;
using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Services.Requests.KpiAchievement;
using DSLNG.PEAR.Services.Requests.Operation;
using DSLNG.PEAR.Services.Responses;
using DSLNG.PEAR.Web.ViewModels.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using DSLNG.PEAR.Services.Requests.KpiTarget;
using System.Text;
using DSLNG.PEAR.Web.ViewModels.File;
using DevExpress.Web.Mvc;
using DSLNG.PEAR.Web.Extensions;
using System.IO;
using DevExpress.Spreadsheet;
using System.Drawing;
using DSLNG.PEAR.Services.Requests.OperationalData;
using DSLNG.PEAR.Services.Responses.Operation;
using DevExpress.Pdf;

namespace DSLNG.PEAR.Web.Controllers
{
    public class FileController : BaseController
    {
        public const string SESSION_KEY = "CurrentFile";
        private readonly IKpiAchievementService _kpiAchievementService;
        private readonly IDropdownService _dropdownService;
        private readonly IKpiTargetService _kpiTargetService;
        private readonly IOperationDataService _operationDataService;
        private readonly IOperationConfigService _operationConfigService;
        private readonly IKpiService _kpiService;

        public FileController(IKpiAchievementService kpiAchievementService, IKpiTargetService kpiTargetService, IDropdownService dropdownService, IOperationDataService operationDataService, IOperationConfigService operationConfigService, IKpiService kpiService)
        {
            _kpiAchievementService = kpiAchievementService;
            _kpiTargetService = kpiTargetService;
            _dropdownService = dropdownService;
            _operationDataService = operationDataService;
            _operationConfigService = operationConfigService;
            _kpiService = kpiService;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Download(string configType)
        {
            ConfigType config = string.IsNullOrEmpty(configType)
                                    ? ConfigType.KpiAchievement
                                    : (ConfigType)Enum.Parse(typeof(ConfigType), configType);

            var viewModel = new ConfigurationViewModel()
            {
                PeriodeType = "Yearly",
                Year = DateTime.Now.Year,
                Month = DateTime.Now.Month,
                ConfigType = config.ToString(),
                Years = _dropdownService.GetYears().MapTo<SelectListItem>(),
                Months = _dropdownService.GetMonths().MapTo<SelectListItem>(),
                PeriodeTypes = _dropdownService.GetPeriodeTypes().MapTo<SelectListItem>()
            };

            return PartialView("_Download", viewModel);
        }

        public FileResult DownloadTemplate(DownloadTemplateViewModel vModel)
        {
            ConfigType config = string.IsNullOrEmpty(vModel.ConfigType) ? ConfigType.KpiTarget
                                    : (ConfigType)Enum.Parse(typeof(ConfigType), vModel.ConfigType);

            #region Get Data
            PeriodeType pType = string.IsNullOrEmpty(vModel.PeriodeType) ? PeriodeType.Yearly
                            : (PeriodeType)Enum.Parse(typeof(PeriodeType), vModel.PeriodeType);

            var viewModel = new ConfigurationViewModel();
            switch (config)
            {
                case ConfigType.KpiTarget:
                    {
                        var request = new GetKpiTargetsConfigurationRequest()
                        {
                            PeriodeType = vModel.PeriodeType,
                            Year = vModel.Year,
                            Month = vModel.Month,
                            RoleGroupId = vModel.RoleGroupId
                        };
                        var target = _kpiTargetService.GetKpiTargetsConfiguration(request);
                        viewModel = target.MapTo<ConfigurationViewModel>();
                        break;
                    }

                case ConfigType.KpiAchievement:
                    {
                        var request = new GetKpiAchievementsConfigurationRequest()
                        {
                            PeriodeType = vModel.PeriodeType,
                            Year = vModel.Year,
                            Month = vModel.Month,
                            RoleGroupId = vModel.RoleGroupId
                        };
                        var achievement = _kpiAchievementService.GetKpiAchievementsConfiguration(request);
                        viewModel = achievement.MapTo<ConfigurationViewModel>();
                        break;
                    }

                case ConfigType.OperationData:
                    {
                        var request = vModel.MapTo<GetOperationDataConfigurationRequest>();
                        request.PeriodeType = pType;
                        request.IsPartial = false;
                        var operationData = _operationDataService.GetOperationDataConfiguration(request);
                        viewModel = operationData.MapTo<ConfigurationViewModel>();
                        //return new FileContentResult(null, "application/octet-stream") { FileDownloadName = "as" };
                        break;
                    }

                default:
                    break;
            }
            #endregion

            /*
             * Find and Create Directory
             */
            var resultPath = Server.MapPath(string.Format("{0}{1}/", TemplateDirectory, vModel.ConfigType));
            if (!Directory.Exists(resultPath))
            {
                Directory.CreateDirectory(resultPath);
            }


            #region parsing data to excel
            string dateFormat = string.Empty;
            string workSheetName = new StringBuilder(vModel.PeriodeType).ToString();
            switch (vModel.PeriodeType)
            {
                case "Yearly":
                    dateFormat = "yyyy";
                    break;
                case "Monthly":
                    dateFormat = "mmm-yy";
                    workSheetName = string.Format("{0}_{1}", workSheetName, vModel.Year);
                    break;
                default:
                    dateFormat = "dd-mmm-yy";
                    workSheetName = string.Format("{0}_{1}-{2}", workSheetName, vModel.Year, vModel.Month.ToString().PadLeft(2, '0'));
                    break;
            }

            string fileName = string.Format(@"{0}_{1}_{2}.xlsx", vModel.ConfigType, vModel.PeriodeType, DateTime.Now.ToString("yyyymmddMMss"));

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
            int i = 1; //i for row
            #region inserting from models
            foreach (var kpi in viewModel.Kpis)
            {
                worksheet.Cells[i, kpiIdColumn.Index].Value = kpi.Id;
                worksheet.Cells[i, kpiNameColumn.Index].Value = string.Format("{0} ({1})", kpi.Name, kpi.Measurement);
                int j = 2; // for column
                var items = new List<ConfigurationViewModel.Item>();
                switch (vModel.ConfigType)
                {
                    case "KpiTarget":
                        {
                            foreach (var target in kpi.KpiTargets)
                            {
                                var item = new ConfigurationViewModel.Item
                                {
                                    Id = target.Id,
                                    KpiId = kpi.Id,
                                    Periode = target.Periode,
                                    Remark = target.Remark,
                                    Value = target.Value.HasValue ? target.Value.ToString() : string.Empty,
                                    PeriodeType = pType
                                };
                                items.Add(item);
                            }
                            break;
                        }

                    case "KpiAchievement":
                        {
                            foreach (var achievement in kpi.KpiAchievements)
                            {
                                var item = new ConfigurationViewModel.Item()
                                {
                                    Id = achievement.Id,
                                    KpiId = kpi.Id,
                                    Periode = achievement.Periode,
                                    Remark = achievement.Remark,
                                    Value = achievement.Value.HasValue ? achievement.Value.ToString() : string.Empty,
                                    PeriodeType = pType
                                };
                                items.Add(item);
                            }
                            break;
                        }

                    case "OperationData":
                        {
                            //items = kpi.OperationData.MapTo<ConfigurationViewModel.Item>();
                            foreach (var operationData in kpi.OperationData)
                            {
                                var item = new ConfigurationViewModel.Item()
                                {
                                    Id = operationData.Id,
                                    KpiId = kpi.Id,
                                    Periode = operationData.Periode,
                                    Remark = operationData.Remark,
                                    Value = operationData.Value.HasValue ? operationData.Value.ToString() : string.Empty,
                                    PeriodeType = pType
                                };
                                items.Add(item);
                            }
                            break;
                        }
                }

                foreach (var item in items)
                {
                    worksheet.Cells[headerRow.Index, j].Value = item.Periode;
                    worksheet.Cells[headerRow.Index, j].NumberFormat = dateFormat;
                    worksheet.Cells[headerRow.Index, j].AutoFitColumns();

                    worksheet.Cells[i, j].Value = item.RealValue;
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
            #endregion
            kpiNameColumn.AutoFitColumns();
            worksheet.FreezePanes(headerRow.Index, kpiNameColumn.Index);

            string resultFilePath = string.Format("{0},{1}", resultPath, fileName);
            //System.Web.HttpContext.Current.Request.MapPath(resultPath + fileName);
            //System.Web.HttpContext.Current.Response.Clear();
            //System.Web.HttpContext.Current.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            //System.Web.HttpContext.Current.Response.AddHeader("content-disposition", String.Format(@"attachment;filename={0}", fileName));

            using (FileStream stream = new FileStream(resultFilePath, FileMode.Create, FileAccess.ReadWrite))
            {
                workbook.SaveDocument(stream, DevExpress.Spreadsheet.DocumentFormat.Xlsx);
                stream.Close();
            }
            //System.Web.HttpContext.Current.Response.End();
            //workbook.SaveDocument(resultFilePath, DocumentFormat.OpenXml);
            //workbook.Dispose();
            #endregion

            string namafile = Path.GetFileName(resultFilePath);
            byte[] fileBytes = System.IO.File.ReadAllBytes(resultFilePath);
            var response = new FileContentResult(fileBytes, "application/octet-stream") { FileDownloadName = fileName };
            return response;
        }

        public ActionResult Upload(string configType)
        {
            var viewModel = new ConfigurationViewModel();
            viewModel.ConfigType = configType;
            return PartialView("_Upload", viewModel);
        }

        public ActionResult UploadControlCallbackAction(string configType)
        {

            string[] extension = { ".xls", ".xlsx", ".csv", };
            var sourcePath = string.Format("{0}{1}/", TemplateDirectory, configType);
            var targetPath = string.Format("{0}{1}/", UploadDirectory, configType);
            ExcelUploadHelper.setPath(sourcePath, targetPath);
            ExcelUploadHelper.setValidationSettings(extension, 30000000);
            UploadControlExtension.GetUploadedFiles("uc", ExcelUploadHelper.ValidationSettings, ExcelUploadHelper.FileUploadComplete);
            return null;
            //switch (configType)
            //{
            //    case "KpiAchievement":
            //        return PartialView("UploadControlPartial", configType);
            //        break;
            //    default:
            //        break;
            //}
        }

        public JsonResult ProcessFile(ProcessFileViewModel viewModel)
        {
            var file = string.Format("{0}{1}/{2}", UploadDirectory, viewModel.ConfigType, viewModel.Filename);
            viewModel.Filename = file;
            var response = ReadExcelFile(viewModel);
            return Json(new { isSuccess = response.IsSuccess, Message = response.Message });
        }

        private BaseResponse ReadExcelFile(ProcessFileViewModel viewModel)
        {
            var response = new BaseResponse();

            try
            {
                int year = DateTime.Now.Year;
                int month = DateTime.Now.Month;
                var listData = new List<ConfigurationViewModel.Item>();

                if (viewModel.Filename != Path.GetFullPath(viewModel.Filename))
                {
                    viewModel.Filename = Server.MapPath(viewModel.Filename);
                }

                if (!System.IO.File.Exists(viewModel.Filename))
                {
                    response.IsSuccess = false;
                    response.Message = "File Not Found";
                    return response;
                }

                Workbook workbook = new Workbook();
                using (FileStream stream = new FileStream(viewModel.Filename, FileMode.Open))
                {
                    workbook.LoadDocument(stream, DevExpress.Spreadsheet.DocumentFormat.OpenXml);
                    #region foreach
                    foreach (var worksheet in workbook.Worksheets)
                    {
                        string[] name = worksheet.Name.Split('_');
                        PeriodeType pType;
                        if (name[0] == "Daily" || name[0] == "Monthly" || name[0] == "Yearly")
                        {
                            string periodeType = name[0];
                            pType = string.IsNullOrEmpty(periodeType) ? PeriodeType.Yearly : (PeriodeType)Enum.Parse(typeof(PeriodeType), periodeType);
                            string period = name[name.Count() - 1];
                            string[] periodes = null;

                            if (periodeType != period && !string.IsNullOrEmpty(period))
                            {
                                switch (periodeType)
                                {
                                    case "Yearly":
                                        break;
                                    case "Monthly":
                                        year = int.Parse(period);
                                        break;
                                    case "Daily":
                                        periodes = period.Split('-');
                                        year = int.Parse(periodes[0]);
                                        month = int.Parse(periodes[periodes.Count() - 1]);
                                        break;
                                }
                            }

                            workbook.Worksheets.ActiveWorksheet = worksheet;

                            Range range = worksheet.GetUsedRange();
                            int rows = range.RowCount;
                            int column = range.ColumnCount - 2;
                            int kpiId = 0;
                            var kpiListWithOperationConfigId = new List<int>();
                            var operationIds = new GetOperationsInResponse();

                            #region get kpi list with key operation config
                            if (viewModel.ConfigType.ToLowerInvariant().Equals("operationdata"))
                            {
                                for (int i = 1; i < rows; i++)
                                {
                                    for (int j = 0; j < column; j++)
                                    {
                                        if (j == 0)
                                        {
                                            if (worksheet.Cells[i, j].Value.Type == CellValueType.Numeric)
                                            {
                                                kpiId = int.Parse(worksheet.Cells[i, j].Value.ToString());
                                                kpiListWithOperationConfigId.Add(kpiId);
                                            }
                                        }
                                    }
                                }
                                operationIds = _operationConfigService.GetOperationIn(new GetOperationsInRequest { KpiIds = kpiListWithOperationConfigId });
                            }
                            #endregion

                            for (int i = 1; i < rows; i++)
                            {
                                bool isAuthorizedKPI = false;
                                for (int j = 0; j < column; j++)
                                {
                                    if (j == 0)
                                    {
                                        if (worksheet.Cells[i, j].Value.Type == CellValueType.Numeric)
                                        {
                                            kpiId = int.Parse(worksheet.Cells[i, j].Value.ToString());
                                            //this will validate authorized KPI based on Role
                                            isAuthorizedKPI = ValidateAuthorizeKPI(kpiId);
                                            if (!isAuthorizedKPI)
                                            {
                                                break;
                                            }
                                        }
                                    }
                                    else if (j > 1)
                                    {
                                        if (worksheet.Cells[0, j].Value.Type == CellValueType.DateTime)
                                        {
                                            DateTime periodData = DateTime.Parse(worksheet.Cells[0, j].Value.ToString());

                                            if (worksheet.Cells[i, j].Value.Type == CellValueType.Numeric ||
                                                 worksheet.Cells[i, j].Value.Type == CellValueType.Text)
                                            {
                                                string value = worksheet.Cells[i, j].Value.ToString();
                                                int operationId = 0;
                                                if (viewModel.ConfigType.ToLowerInvariant().Equals(ConfigType.OperationData.ToString().ToLowerInvariant()))
                                                {

                                                    var operation = operationIds.KeyOperations.FirstOrDefault(x => x.KpiId == kpiId);
                                                    if (operation != null)
                                                    {
                                                        operationId = operation.Id;
                                                    }
                                                }

                                                if (!string.IsNullOrEmpty(value))
                                                {
                                                    var data = new ConfigurationViewModel.Item
                                                    {
                                                        Value = value,
                                                        KpiId = kpiId,
                                                        Periode = periodData,
                                                        PeriodeType = pType,
                                                        OperationId = operationId,
                                                        ScenarioId = viewModel.ScenarioId
                                                    };

                                                    listData.Add(data);
                                                }
                                            }
                                        }
                                    }
                                }
                                if (!isAuthorizedKPI) continue;
                            }
                        }
                        else
                        {
                            response.IsSuccess = false;
                            response.Message = "File Not Valid";
                            break;
                        }
                        switch (viewModel.ConfigType)
                        {
                            case "KpiTarget":
                                response = UpdateKpiTarget(listData);
                                break;
                            case "KpiAchievement":
                                response = UpdateKpiAchievement(listData);
                                break;
                            case "OperationData":
                                response = UpdateOperationData(listData);
                                break;
                            default:
                                response.IsSuccess = false;
                                response.Message = string.Format(@"config type for {0} is not existed",
                                                                 viewModel.ConfigType);
                                break;
                        }
                    }
                    #endregion
                }
            }
            catch (Exception exception)
            {
                response.IsSuccess = false;
                response.Message = exception.Message;
            }

            return response;
        }

        private bool ValidateAuthorizeKPI(int kpiId)
        {
            var response = new DSLNG.PEAR.Services.Responses.Kpi.GetKpiDetailResponse();
            if (this.UserProfile().IsSuperAdmin) return true;
            response = _kpiService.GetKpiDetail(new Services.Requests.Kpi.GetKpiRequest { Id = kpiId });
            if (response.IsSuccess)
            {
                if (response.RoleGroup == this.UserProfile().RoleName)
                {
                    return true;
                }
                else
                    return false;
            }
            return false;
        }

        private BaseResponse UpdateOperationData(IEnumerable<ConfigurationViewModel.Item> datas)
        {
            var response = new BaseResponse { IsSuccess = false, Message = "Data Not Valid" };
            if (datas != null)
            {
                var batch = new BatchUpdateOperationDataRequest();
                foreach (var data in datas)
                {
                    var prepare = new UpdateOperationDataRequest()
                    {
                        Id = data.Id,
                        KpiId = data.KpiId,
                        Periode = data.Periode,
                        Value = data.Value,
                        PeriodeType = data.PeriodeType,
                        Remark = data.Remark,
                        KeyOperationConfigId = data.OperationId,
                        ScenarioId = data.ScenarioId
                    };// data.MapTo<UpdateKpiAchievementItemRequest>();
                    batch.BatchUpdateOperationDataItemRequest.Add(prepare);
                }
                response = _operationDataService.BatchUpdateOperationDatas(batch);
            }
            return response;
        }

        private BaseResponse UpdateKpiAchievement(IEnumerable<ConfigurationViewModel.Item> data)
        {
            var response = new BaseResponse { IsSuccess = false, Message = "Data Not Valid" };
            if (data != null)
            {
                var batch = new BatchUpdateKpiAchievementRequest();
                foreach (var datum in data)
                {
                    var prepare = new UpdateKpiAchievementItemRequest() { Id = datum.Id, KpiId = datum.KpiId, Periode = datum.Periode, Value = datum.Value, PeriodeType = datum.PeriodeType, Remark = datum.Remark, UpdateFrom= "KPIAchievementForm" };// data.MapTo<UpdateKpiAchievementItemRequest>();
                    batch.BatchUpdateKpiAchievementItemRequest.Add(prepare);
                }
                batch.ControllerName = "File";
                batch.ActionName = "ProcessFile";
                batch.UserId = UserProfile().UserId;
                response = _kpiAchievementService.BatchUpdateKpiAchievements(batch);
            }
            return response;
        }

        private BaseResponse UpdateKpiTarget(IEnumerable<ConfigurationViewModel.Item> data)
        {
            var response = new BaseResponse { IsSuccess = false, Message = "Data Not Valid" };
            if (data != null)
            {
                var batch = new BatchUpdateTargetRequest();
                foreach (var datum in data)
                {
                    var prepare = new SaveKpiTargetRequest() { Value = datum.Value, KpiId = datum.KpiId, Periode = datum.Periode, PeriodeType = datum.PeriodeType, Remark = datum.Remark };
                    batch.BatchUpdateKpiTargetItemRequest.Add(prepare);
                }
                response = _kpiTargetService.BatchUpdateKpiTargetss(batch);

            }
            return response;
        }

        #region PDF Viewer
        public ActionResult Preview()
        {
            return PartialView("_PdfViewerPartial");
        }
        public ActionResult PreviewByFilename(string filename)
        {
            if (System.IO.File.Exists(Server.MapPath(filename)))
            {
                Session[filename] = System.IO.File.ReadAllBytes(Server.MapPath(filename));
            }
            return PartialView("_PdfViewerPartial");
        }
        /// <summary>
        /// Read PDF file with two option, physical file should send full path filename in parameters, other wise will use session cookie using it's filename
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        protected MemoryStream GetStream(string filename)
        {
            MemoryStream stream = null;
            if (System.IO.File.Exists(Server.MapPath(filename)))
            {
                stream = new MemoryStream(System.IO.File.ReadAllBytes(Server.MapPath(filename)));
            }
            else if (Session[filename] != null)
            {
                stream = new MemoryStream((byte[])Session[filename]);
            }
            else
            {
                stream = new MemoryStream();
            }
            Session[SESSION_KEY] = ReadAllBytes(stream);
            return stream;
        }

        protected static byte[] ReadAllBytes(Stream stream)
        {
            byte[] buffer = new byte[16 * 1024];
            int readCount;
            using (MemoryStream ms = new MemoryStream())
            {
                while ((readCount = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, readCount);
                }
                return ms.ToArray();
            }
        }
        public ActionResult PdfViewer(string filename)
        {
            MemoryStream stream = null;//

            if (!string.IsNullOrEmpty(filename))
            {
                stream = GetStream(filename);
            }
            else if (Session[SESSION_KEY] != null)
            {
                stream = new MemoryStream((byte[])Session[SESSION_KEY]);
            }

            List<PdfPageViewModel> model = new List<PdfPageViewModel>();
            if (stream != null)
            {
                PdfDocumentProcessor documentProcessor = new PdfDocumentProcessor();
                documentProcessor.LoadDocument(stream);

                for (int pageNumber = 1; pageNumber < documentProcessor.Document.Pages.Count; pageNumber++)
                {
                    model.Add(new PdfPageViewModel(documentProcessor)
                    {
                        PageNumber = pageNumber
                    });
                }
            }
            
            return PartialView("_DocumentViewPartial", model);
        }

        public ActionResult ExecutiveSummary(string[] ExSumParams)
        {
            string filename = string.Empty;
            int pageNumber = 3;
            if (ExSumParams != null && ExSumParams.Count() > 0) {
                filename = ExSumParams[0];
                pageNumber = int.Parse(ExSumParams[1]);
            }

            MemoryStream stream = null;//

            if (!string.IsNullOrEmpty(filename))
            {
                stream = GetStream(filename);
            }
            else if (Session[SESSION_KEY] != null)
            {
                stream = new MemoryStream((byte[])Session[SESSION_KEY]);
            }

            List<PdfPageViewModel> model = new List<PdfPageViewModel>();
            if (stream != null)
            {
                PdfDocumentProcessor documentProcessor = new PdfDocumentProcessor();
                documentProcessor.LoadDocument(stream);

                model.Add(new PdfPageViewModel(documentProcessor)
                {
                    PageNumber = pageNumber
                });
            }

            return PartialView("_DocumentViewPartial", model);
        }
        #endregion
    }
}
