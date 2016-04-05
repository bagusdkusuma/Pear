﻿using DSLNG.PEAR.Data.Enums;
using DSLNG.PEAR.Common.Extensions;
using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Services.Requests.KpiAchievement;
using DSLNG.PEAR.Services.Responses;
using DSLNG.PEAR.Web.ViewModels.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using DSLNG.PEAR.Services.Requests.KpiTarget;
using System.Text;
using DevExpress.Web.Mvc;
using DSLNG.PEAR.Web.Extensions;
using System.IO;
using DevExpress.Spreadsheet;
using System.Drawing;

namespace DSLNG.PEAR.Web.Controllers
{
    public class FileController : BaseController
    {
        private readonly IKpiAchievementService _kpiAchievementService;
        private readonly IDropdownService _dropdownService;
        private readonly IKpiTargetService _kpiTargetService;

        public FileController(IKpiAchievementService kpiAchievementService, IKpiTargetService kpiTargetService, IDropdownService dropdownService)
        {
            _kpiAchievementService = kpiAchievementService;
            _kpiTargetService = kpiTargetService;
            _dropdownService = dropdownService;
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

            var viewModel = new ConfigurationViewModel() { /*switch (config)*/ /*{*/ /*    case ConfigType.KpiAchievement:*/ /*        var request = new GetKpiAchievementsConfigurationRequest();*/ /*        var achievement = _kpiAchievementService.GetKpiAchievementsConfiguration(request);*/ /*        model = achievement.MapTo<ConfigurationViewModel>();*/ /*        break;*/ /*    case ConfigType.KpiTarget:*/ /*        var targetRequest = new GetKpiTargetsConfigurationRequest();*/ /*        var target = _kpiTargetService.GetKpiTargetsConfiguration(targetRequest);*/ /*        model = target.MapTo<ConfigurationViewModel>();*/ /*        break;*/ /*    case ConfigType.Economic:*/ /*        //var request = new GetKpiAchievementsConfigurationRequest();*/ /*        //var achievement = _kpiAchievementService.GetKpiAchievementsConfiguration(request);*/ /*        //model = achievement.MapTo<ConfigurationViewModel>();*/ /*        break;*/ /*}*/PeriodeType = "Yearly", Year = DateTime.Now.Year, Month = DateTime.Now.Month, ConfigType = config.ToString(), Years = _dropdownService.GetYears().MapTo<SelectListItem>(), Months = _dropdownService.GetMonths().MapTo<SelectListItem>(), PeriodeTypes = _dropdownService.GetPeriodeTypes().MapTo<SelectListItem>() };
            return PartialView("_Download", viewModel);
        }

        public FileResult DownloadTemplate(string configType, string periodeType, int year, int month)
        {
            ConfigType config = string.IsNullOrEmpty(configType)
                                    ? ConfigType.KpiTarget
                                    : (ConfigType)Enum.Parse(typeof(ConfigType), configType);

            #region Get Data
            PeriodeType pType = string.IsNullOrEmpty(periodeType)
                            ? PeriodeType.Yearly
                            : (PeriodeType)Enum.Parse(typeof(PeriodeType), periodeType);

            var viewModel = new ConfigurationViewModel();
            switch (config)
            {
                case ConfigType.KpiTarget:
                    //todo get KpiTarget Data
                    var targetRequest = new GetKpiTargetsConfigurationRequest() { PeriodeType = periodeType, Year = year, Month = month };
                    var target = _kpiTargetService.GetKpiTargetsConfiguration(targetRequest);
                    viewModel = target.MapTo<ConfigurationViewModel>();
                    break;
                case ConfigType.KpiAchievement:
                    //todo get KpiAchievement Data
                    var request = new GetKpiAchievementsConfigurationRequest() { PeriodeType = periodeType, Year = year, Month = month };
                    var achievement = _kpiAchievementService.GetKpiAchievementsConfiguration(request);
                    viewModel = achievement.MapTo<ConfigurationViewModel>();
                    break;
                case ConfigType.Economic:
                    break;
                default:
                    break;
            }
            #endregion

            /*
             * Find and Create Directory
             */
            var resultPath = Server.MapPath(string.Format("{0}{1}/", TemplateDirectory, configType));
            if (!System.IO.Directory.Exists(resultPath))
            {
                System.IO.Directory.CreateDirectory(resultPath);
            }


            #region parsing data to excel
            string dateFormat = string.Empty;
            string workSheetName = new StringBuilder(periodeType).ToString();
            switch (periodeType)
            {
                case "Yearly":
                    dateFormat = "yyyy";
                    break;
                case "Monthly":
                    dateFormat = "mmm-yy";
                    workSheetName = string.Format("{0}_{1}", workSheetName, year);
                    break;
                default:
                    dateFormat = "dd-mmm-yy";
                    workSheetName = string.Format("{0}_{1}-{2}", workSheetName, year, month.ToString().PadLeft(2, '0'));
                    break;
            }

            string fileName = string.Format(@"{0}.xlsx", DateTime.Now.ToString("yyyymmddMMss"));

            //using (FileStream stream = new FileStream(fileName,FileMode.Create,FileAccess.ReadWrite)
            //{

            //}
            IWorkbook workbook = new Workbook();
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
            #region inserting from models
            foreach (var kpi in viewModel.Kpis)
            {
                worksheet.Cells[i, KpiIdColumn.Index].Value = kpi.Id;
                worksheet.Cells[i, KpiNameColumn.Index].Value = string.Format("{0} ({1})", kpi.Name, kpi.Measurement);
                int j = 2; // for column
                var items = new List<ConfigurationViewModel.Item>();
                switch (configType)
                {
                    case "KpiTarget":
                        foreach (var target in kpi.KpiTargets)
                        {
                            var item = new ConfigurationViewModel.Item();
                            item.Id = target.Id;
                            item.KpiId = kpi.Id;
                            item.Periode = target.Periode;
                            item.Remark = target.Remark;
                            item.Value = target.Value;
                            item.PeriodeType = pType;
                            items.Add(item);
                        }
                        break;
                    case "KpiAchievement":
                        foreach (var achieve in kpi.KpiAchievements)
                        {
                            var item = new ConfigurationViewModel.Item() { Id = achieve.Id, KpiId = achieve.Id, Periode = achieve.Periode, Remark = achieve.Remark, Value = achieve.Value, PeriodeType = pType };
                            items.Add(item);
                        }
                        break;
                    case "Economic":
                        items = kpi.Economics.MapTo<ConfigurationViewModel.Item>();
                        break;
                    default:
                        break;
                }
                foreach (var achievement in items)
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
            #endregion
            KpiNameColumn.AutoFitColumns();
            worksheet.FreezePanes(HeaderRow.Index, KpiNameColumn.Index);

            string resultFilePath = string.Format("{0},{1}", resultPath, fileName);// System.Web.HttpContext.Current.Request.MapPath(resultPath + fileName);
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
            ExcelUploadHelper.setValidationSettings(extension, 20971520);
            UploadControlExtension.GetUploadedFiles("uc", ExcelUploadHelper.ValidationSettings, ExcelUploadHelper.FileUploadComplete);
            return null;
        }

        public JsonResult ProcessFile(string configType, string filename)
        {
            var file = string.Format("{0}{1}/{2}", UploadDirectory, configType, filename);
            var response = ReadExcelFile(configType, file);
            return Json(new { isSuccess = response.IsSuccess, Message = response.Message });
        }

        private BaseResponse ReadExcelFile(string configType, string filename)
        {
            var response = new BaseResponse();
            string periodType = string.Empty;
            int tahun = DateTime.Now.Year, bulan = DateTime.Now.Month;
            var listData = new List<ConfigurationViewModel.Item>();
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
                    PeriodeType pType;
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
                        int kpiId = 0;
                        DateTime periodData;

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
                                        kpiId = int.Parse(worksheet.Cells[i, j].Value.ToString());
                                    }
                                }
                                else if (j > 1)
                                {
                                    if (worksheet.Cells[0, j].Value.Type == CellValueType.DateTime)
                                    {
                                        periodData = DateTime.Parse(worksheet.Cells[0, j].Value.ToString());
                                        double? nilai;
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
                                            var data = new ConfigurationViewModel.Item() { Value = nilai, KpiId = kpiId, Periode = periodData, PeriodeType = pType };
                                            listData.Add(data);
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
                    }
                    else
                    {
                        response.IsSuccess = false;
                        response.Message = "File Not Valid";
                        break;
                    }
                    switch (configType)
                    {
                        case "KpiTarget":
                            response = this.UpdateKpiTarget(listData);
                            break;
                        case "KpiAchievement":
                            response = this.UpdateKpiAchievement(listData, pType.ToString(), tahun, bulan);
                            break;
                        case "Economic":
                            response = this.UpdateEconomic(listData);
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

        private BaseResponse UpdateEconomic(List<ConfigurationViewModel.Item> datas)
        {
            var response = new BaseResponse { IsSuccess = false, Message = "Data Not Valid" };

            return response;
        }

        private bool CompareData(List<ConfigurationViewModel.Item> newList, List<ConfigurationViewModel.Item> oldList, out List<ConfigurationViewModel.Item> deleted, out List<ConfigurationViewModel.Item> inserted)
        {
            deleted = newList.Except(oldList).ToList();
            inserted = oldList.Except(newList).ToList();
            return false;
        }

        private BaseResponse UpdateKpiAchievement(IEnumerable<ConfigurationViewModel.Item> data, string periodeType, int year, int month)
        {
            var response = new BaseResponse();
            if (data != null)
            {
                var batch = new BatchUpdateKpiAchievementRequest();
                foreach (var datum in data)
                {
                    var prepare = new UpdateKpiAchievementItemRequest() { Id = datum.Id, KpiId = datum.KpiId, Periode = datum.Periode, Value = datum.Value.HasValue ? datum.Value.ToString() : string.Empty, PeriodeType = datum.PeriodeType, Remark = datum.Remark };// data.MapTo<UpdateKpiAchievementItemRequest>();
                    batch.BatchUpdateKpiAchievementItemRequest.Add(prepare);
                }
                response = _kpiAchievementService.BatchUpdateKpiAchievements(batch);
            }
            return response;
        }

        private BaseResponse UpdateKpiTarget(IEnumerable<ConfigurationViewModel.Item> data)
        {
            var response = new BaseResponse();
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

    }
}
