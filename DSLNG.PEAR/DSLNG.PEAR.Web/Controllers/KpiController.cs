using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ClosedXML.Excel;
using DSLNG.PEAR.Common.Contants;
using DSLNG.PEAR.Services;
using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Services.Responses.Kpi;
using System.Web.Mvc;
using DSLNG.PEAR.Web.Grid;
using DevExpress.Web.Mvc;
using DSLNG.PEAR.Services.Requests.Kpi;
using DSLNG.PEAR.Web.ViewModels.Kpi;
using DSLNG.PEAR.Common.Extensions;
using DSLNG.PEAR.Web.Attributes;

namespace DSLNG.PEAR.Web.Controllers
{
    [Authorize]
    public class KpiController : BaseController
    {
        private readonly IKpiService _kpiService;
        private readonly ILevelService _levelService;
        private readonly IRoleGroupService _roleGroupService;
        private readonly IPillarService _pillarService;
        private readonly IDropdownService _dropdownService;

        public KpiController(IKpiService service,
            ILevelService levelService,
            IRoleGroupService roleGroupService,
            IPillarService pillarService,
            IDropdownService dropdownService)
        {
            _kpiService = service;
            _levelService = levelService;
            _roleGroupService = roleGroupService;
            _pillarService = pillarService;
            _dropdownService = dropdownService;
        }

        [AuthorizeUser(AccessLevel="AllowView")]
        public ActionResult Index()
        {

            return View();
        }

        public ActionResult IndexPartial()
        {
            var viewModel = GridViewExtension.GetViewModel("gridKpiIndex");
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
            var viewModel = new GridViewModel();
            viewModel.KeyFieldName = "Id";
            viewModel.Columns.Add("Code");
            viewModel.Columns.Add("Name");
            viewModel.Columns.Add("PillarName");
            viewModel.Columns.Add("Order");
            viewModel.Columns.Add("IsEconomic");
            viewModel.Columns.Add("Remark");
            viewModel.Columns.Add("IsActive");
            viewModel.Pager.PageSize = 10;
            return viewModel;
        }

        public ActionResult PagingAction(GridViewPagerState pager)
        {
            var viewModel = GridViewExtension.GetViewModel("gridKpiIndex");
            viewModel.ApplyPagingState(pager);
            return BindingCore(viewModel);
        }

        public void GetDataRowCount(GridViewCustomBindingGetDataRowCountArgs e)
        {

            e.DataRowCount = _kpiService.GetKpis(new GetKpisRequest()).Kpis.Count;
        }

        public void GetData(GridViewCustomBindingGetDataArgs e)
        {
            e.Data = _kpiService.GetKpis(new GetKpisRequest
            {
                Skip = e.StartDataRowIndex,
                Take = e.DataRowCount
            }).Kpis;
        }

        public CreateKpiViewModel CreateViewModel(CreateKpiViewModel viewModel)
        {
            viewModel.LevelList = _dropdownService.GetLevels().MapTo<SelectListItem>();
            viewModel.PillarList = _dropdownService.GetPillars().MapTo<SelectListItem>();
            viewModel.RoleGroupList = _dropdownService.GetRoleGroups().MapTo<SelectListItem>();
            viewModel.TypeList = _dropdownService.GetTypes().MapTo<SelectListItem>();
            viewModel.GroupList = _dropdownService.GetGroups().MapTo<SelectListItem>();
            viewModel.YtdFormulaList = _dropdownService.GetYtdFormulas().MapTo<SelectListItem>();
            viewModel.PeriodeList = _dropdownService.GetPeriodeTypes().MapTo<SelectListItem>();
            viewModel.MethodList = _dropdownService.GetMethods().MapTo<SelectListItem>();
            viewModel.MeasurementList = _dropdownService.GetMeasurement().MapTo<SelectListItem>();
            viewModel.KpiList = _dropdownService.GetKpis().MapTo<SelectListItem>();
            var ytd = Enum.GetValues(typeof(DSLNG.PEAR.Data.Enums.YtdFormula)).Cast<DSLNG.PEAR.Data.Enums.YtdFormula>();
            var periode = Enum.GetValues(typeof(DSLNG.PEAR.Data.Enums.PeriodeType)).Cast<DSLNG.PEAR.Data.Enums.PeriodeType>();
            viewModel.YtdFormulaList = ytd.Select(x => new SelectListItem { Text = x.ToString(), Value = x.ToString() }).ToList();
            viewModel.PeriodeList = periode.Select(x => new SelectListItem { Text = x.ToString(), Value = x.ToString() }).ToList();
            viewModel.IsActive = true;
            return viewModel;
        }

        [AuthorizeUser(AccessLevel = "AllowCreate")]
        public ActionResult Create()
        {
            var viewModel = new CreateKpiViewModel();
            viewModel = CreateViewModel(viewModel);
            viewModel.Icons = Directory.EnumerateFiles(Server.MapPath(PathConstant.KpiPath)).ToList();
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Create(CreateKpiViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                viewModel.Code = string.Format("{0}{1}{2}{3}", viewModel.CodeFromPillar, viewModel.CodeFromLevel, viewModel.Code, viewModel.CodeFromRoleGroup);
                viewModel.YtdFormula = (DSLNG.PEAR.Web.ViewModels.Kpi.YtdFormula)Enum.Parse(typeof(DSLNG.PEAR.Data.Enums.YtdFormula), viewModel.YtdFormulaValue);
                viewModel.Periode = (DSLNG.PEAR.Web.ViewModels.Kpi.PeriodeType)Enum.Parse(typeof(DSLNG.PEAR.Data.Enums.PeriodeType), viewModel.PeriodeValue);
                var request = viewModel.MapTo<CreateKpiRequest>();
                request.ActionName = "Create";
                request.ControllerName = "KPI";
                request.UserId = this.UserProfile().UserId;
                var response = _kpiService.Create(request);
                TempData["IsSuccess"] = response.IsSuccess;
                TempData["Message"] = response.Message;
                if (response.IsSuccess)
                {
                    return RedirectToAction("Index");
                }
            }
            viewModel = CreateViewModel(viewModel);
            return View("Create", viewModel);
        }

        [AuthorizeUser(AccessLevel = "AllowUpdate")]
        public ActionResult Update(int id)
        {
            var response = _kpiService.GetKpi(new GetKpiRequest { Id = id });
            var viewModel = response.MapTo<UpdateKpiViewModel>();
            viewModel.LevelList = _dropdownService.GetLevels().MapTo<SelectListItem>();
            viewModel.PillarList = _dropdownService.GetPillars().MapTo<SelectListItem>();
            viewModel.RoleGroupList = _dropdownService.GetRoleGroups().MapTo<SelectListItem>();
            viewModel.TypeList = _dropdownService.GetTypes().MapTo<SelectListItem>();
            viewModel.GroupList = _dropdownService.GetGroups().MapTo<SelectListItem>();
            viewModel.YtdFormulaList = _dropdownService.GetYtdFormulas().MapTo<SelectListItem>();
            viewModel.PeriodeList = _dropdownService.GetPeriodeTypes().MapTo<SelectListItem>();
            viewModel.MethodList = _dropdownService.GetMethods().MapTo<SelectListItem>();
            viewModel.MeasurementList = _dropdownService.GetMeasurement().MapTo<SelectListItem>();
            viewModel.KpiList = _dropdownService.GetKpis().MapTo<SelectListItem>();
            viewModel.YtdFormulaList = _dropdownService.GetYtdFormulas().MapTo<SelectListItem>();
            viewModel.PeriodeList = _dropdownService.GetPeriodeTypes().MapTo<SelectListItem>();
            if (!string.IsNullOrEmpty(viewModel.Code))
            {
                var numbers = String.Join("", viewModel.Code.Where(char.IsDigit));
                //var code = 3;
                //var takeCode = viewModel.Code.Length - 3;
                if (viewModel.PillarId.HasValue)
                {
                    viewModel.CodeFromPillar = GetPillarCode(viewModel.PillarId.Value);
                    //code += 2;
                    //takeCode -= viewModel.CodeFromPillar.Length;
                }
                viewModel.CodeFromLevel = GetLevelCode(viewModel.LevelId);
                if (viewModel.RoleGroupId.HasValue)
                {
                    viewModel.CodeFromRoleGroup = !string.IsNullOrEmpty(GetRoleGroupCode(viewModel.RoleGroupId.Value)) ? GetRoleGroupCode(viewModel.RoleGroupId.Value) : "";
                    //takeCode -= viewModel.CodeFromRoleGroup.Length;
                }
                //viewModel.Code = response.Code.Substring(code, takeCode);
                viewModel.Code = numbers;
            }

            if (viewModel.RelationModels.Count == 0)
            {
                viewModel.RelationModels.Add(new ViewModels.Kpi.KpiRelationModel { KpiId = 0, Method = "" });
            }
            viewModel.YtdFormulaValue = viewModel.YtdFormula.ToString();
            viewModel.Icon = response.Icon;
            viewModel.Icons = Directory.EnumerateFiles(Server.MapPath(PathConstant.KpiPath)).ToList();
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Update(UpdateKpiViewModel viewModel)
        {
            viewModel.YtdFormula = (ViewModels.Kpi.YtdFormula)Enum.Parse(typeof(Data.Enums.YtdFormula), viewModel.YtdFormulaValue);
            viewModel.Periode = (ViewModels.Kpi.PeriodeType)Enum.Parse(typeof(Data.Enums.PeriodeType), viewModel.PeriodeValue);
            viewModel.Code = string.Format("{0}{1}{2}{3}", viewModel.CodeFromPillar, viewModel.CodeFromLevel, viewModel.Code, viewModel.CodeFromRoleGroup);
            //if(viewModel.YtdFormula != ViewModels.Kpi.YtdFormula.Custom)
            //{
            //    viewModel.CustomFormula = null;
            //}
            if (viewModel.MethodId == 3)
            {
                viewModel.CustomFormula = null;
            }
            var request = viewModel.MapTo<UpdateKpiRequest>();
            request.ActionName = "Update";
            request.ControllerName = "KPI";
            request.UserId = this.UserProfile().UserId;
            if (!ModelState.IsValid)
            {
                return View("Update", viewModel);
            }

            var response = _kpiService.Update(request);
            TempData["IsSuccess"] = response.IsSuccess;
            TempData["Message"] = response.Message;
            if (response.IsSuccess)
            {
                return RedirectToAction("Index");
            }
            viewModel.LevelList = _dropdownService.GetLevels().MapTo<SelectListItem>();
            viewModel.PillarList = _dropdownService.GetPillars().MapTo<SelectListItem>();
            viewModel.RoleGroupList = _dropdownService.GetRoleGroups().MapTo<SelectListItem>();
            viewModel.TypeList = _dropdownService.GetTypes().MapTo<SelectListItem>();
            viewModel.GroupList = _dropdownService.GetGroups().MapTo<SelectListItem>();
            viewModel.YtdFormulaList = _dropdownService.GetYtdFormulas().MapTo<SelectListItem>();
            viewModel.PeriodeList = _dropdownService.GetPeriodeTypes().MapTo<SelectListItem>();
            viewModel.MethodList = _dropdownService.GetMethods().MapTo<SelectListItem>();
            viewModel.MeasurementList = _dropdownService.GetMeasurement().MapTo<SelectListItem>();
            viewModel.KpiList = _dropdownService.GetKpis().MapTo<SelectListItem>();
            viewModel.YtdFormulaList = _dropdownService.GetYtdFormulas().MapTo<SelectListItem>();
            viewModel.PeriodeList = _dropdownService.GetPeriodeTypes().MapTo<SelectListItem>();
            if (viewModel.RelationModels.Count == 0)
            {
                viewModel.RelationModels.Add(new ViewModels.Kpi.KpiRelationModel { KpiId = 0, Method = "" });
            }
            return View("Update", viewModel);
        }

        [HttpPost]
        [AuthorizeUser(AccessLevel = "AllowUpload")]
        public ActionResult Upload(HttpPostedFileBase iconFile, string returnUrl)
        {

            var validImageTypes = new string[]
                {
                    "image/gif",
                    "image/jpeg",
                    "image/pjpeg",
                    "image/png"
                };

            if (iconFile != null)
            {
                if (!validImageTypes.Contains(iconFile.ContentType))
                {
                    TempData["IsSuccess"] = false;
                    TempData["Message"] = string.Format(@"Please choose either a GIF, JPG or PNG image");
                }
                else
                {
                    using (System.Drawing.Image image = System.Drawing.Image.FromStream(iconFile.InputStream, true, true))
                    {
                        if (image.Width <= ImageConstant.Width && image.Height <= ImageConstant.Height)
                        {
                            if (!Directory.Exists(Server.MapPath(PathConstant.KpiPath)))
                            {
                                Directory.CreateDirectory(Server.MapPath(PathConstant.KpiPath));
                            }
                            var imagePath = Path.Combine(Server.MapPath(PathConstant.KpiPath), iconFile.FileName);
                            iconFile.SaveAs(imagePath);
                            TempData["IsSuccess"] = true;
                            TempData["Message"] = "Icon has been uploaded successfully";
                        }
                        else
                        {
                            TempData["IsSuccess"] = false;
                            TempData["Message"] =
                                string.Format(@"The dimensions of image should not be more than {0}x{1} px",
                                              ImageConstant.Width, ImageConstant.Height);
                        }
                    }
                }
            }

            return Redirect(returnUrl);

        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            var response = _kpiService.Delete(new DeleteKpiRequest {
                Id = id,
                ControllerName ="KPI Configuration",
                ActionName ="Delete",
                UserId = this.UserProfile().UserId
            });
            TempData["IsSuccess"] = response.IsSuccess;
            TempData["Message"] = response.Message;
            return RedirectToAction("Index");
        }

        public string GetLevelCode(int id)
        {
            var level = _levelService.GetLevel(new Services.Requests.Level.GetLevelRequest { Id = id }).Code;
            return level;
        }

        public string GetPillarCode(int id)
        {
            var pillar = _pillarService.GetPillar(new Services.Requests.Pillar.GetPillarRequest { Id = id }).Code;
            return pillar;
        }

        public string GetRoleGroupCode(int id)
        {
            var roleGroup = _roleGroupService.GetRoleGroup(new Services.Requests.RoleGroup.GetRoleGroupRequest { Id = id }).Code;
            return roleGroup;
        }

        public ActionResult DeleteIcon(string name, string redirectAction)
        {
            string fullPath = Request.MapPath(PathConstant.KpiPath + "/" + name);
            if (System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath);
            }

            return RedirectToAction(redirectAction);
        }

        public ActionResult Grid(GridParams gridParams)
        {
            var kpis = _kpiService.GetKpis(new GetKpisRequest
                {
                    Skip = gridParams.DisplayStart,
                    Take = gridParams.DisplayLength,
                    SortingDictionary = gridParams.SortingDictionary,
                    Search = gridParams.Search
                });
            IList<GetKpisResponse.Kpi> kpisResponse = kpis.Kpis;
            var data = new
            {
                sEcho = gridParams.Echo + 1,
                iTotalDisplayRecords = kpis.TotalRecords,
                iTotalRecords = kpis.Kpis.Count,
                aaData = kpisResponse
            };
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [AuthorizeUser(AccessLevel = "AllowDownload")]
        public ActionResult Download()
        {
            var sheetName = "Kpi";
            var data = _kpiService.DownloadKpis().Select(x => new
                {
                    x.Id,
                    x.Code,
                    x.Name,
                    PillarId = x.Pillar != null ? x.Pillar.Id.ToString() : string.Empty,
                    PillarName = x.Pillar != null ? x.Pillar.Name : string.Empty,
                    TypeName = x.Type != null ? x.Type.Name : string.Empty,
                    x.IsEconomic,
                    x.Order,
                    x.YtdFormula,
                    x.ConversionId,
                    x.FormatInput,
                    x.Period,
                    x.Remark,
                    x.Value,
                    x.Icon,
                    x.Color,
                    x.IsActive,
                    CreatedDate = x.CreatedDate.ToString(DateFormat.DateForGrid),
                    UpdatedDate = x.UpdatedDate.ToString(DateFormat.DateForGrid),
                    CreatedBy_Id = x.CreatedBy != null ? x.CreatedBy.Id.ToString() : string.Empty,
                    Group_Id = x.Group != null ? x.Group.Id.ToString() : string.Empty,
                    Level_Id = x.Level != null ? x.Level.Id.ToString() : string.Empty,
                    Measurement_Id = x.Measurement != null ?  x.Measurement.Id.ToString() : string.Empty,
                    Measurement = x.Measurement != null ? x.Measurement.Name : string.Empty,
                    Method_Id = x.Method != null ? x.Method.Id.ToString() : string.Empty,
                    RoleGroup_Id = x.RoleGroup != null ? x.RoleGroup.Id.ToString() : string.Empty,
                    Type_Id = x.Type != null ? x.Type.Id.ToString() : string.Empty,
                    UpdatedBy_Id = x.UpdatedBy != null ? x.UpdatedBy.Id.ToString() : string.Empty
                });
            XLWorkbook wb = new XLWorkbook();
            var ws = wb.Worksheets.Add(sheetName);
            ws.Cell(2, 1).InsertTable(data);
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", String.Format(@"attachment;filename={0}.xlsx", sheetName.Replace(" ", "_")));

            using (MemoryStream memoryStream = new MemoryStream())
            {
                wb.SaveAs(memoryStream);
                memoryStream.WriteTo(Response.OutputStream);
                memoryStream.Close();
            }

            Response.End();
            return View("Index");
        }

        public ActionResult Detail(int id)
        {
            var response = _kpiService.GetKpiDetail(new GetKpiRequest { Id = id });
            var viewModel = response.MapTo<DetailKpiViewModel>();
            return View(viewModel);
        }
    }
}