using System;
using AutoMapper;
using DSLNG.PEAR.Data.Entities;
using DSLNG.PEAR.Data.Entities.Der;
using DSLNG.PEAR.Data.Enums;
using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Services.Requests.Der;
using DSLNG.PEAR.Services.Requests.Measurement;
using DSLNG.PEAR.Services.Requests.PmsSummary;
using DSLNG.PEAR.Services.Requests.Select;
using DSLNG.PEAR.Services.Responses.Der;
using DSLNG.PEAR.Services.Responses.KpiAchievement;
using DSLNG.PEAR.Services.Responses.Level;
using DSLNG.PEAR.Services.Responses.Menu;
using DSLNG.PEAR.Services.Requests.Menu;
using DSLNG.PEAR.Services.Responses.PmsSummary;
using DSLNG.PEAR.Services.Responses.Select;
using DSLNG.PEAR.Services.Responses.User;
using DSLNG.PEAR.Services.Requests.User;
using DSLNG.PEAR.Common.Extensions;
using DSLNG.PEAR.Services.Responses.Group;
using DSLNG.PEAR.Services.Requests.Group;
using DSLNG.PEAR.Services.Responses.Kpi;
using DSLNG.PEAR.Services.Responses.Measurement;
using DSLNG.PEAR.Services.Responses.Conversion;
using DSLNG.PEAR.Services.Requests.Level;
using DSLNG.PEAR.Services.Responses.RoleGroup;
using DSLNG.PEAR.Services.Requests.RoleGroup;
using DSLNG.PEAR.Services.Responses.Type;
using DSLNG.PEAR.Services.Requests.Type;
using DSLNG.PEAR.Services.Responses.Pillar;
using DSLNG.PEAR.Services.Requests.Pillar;
using DSLNG.PEAR.Services.Requests.Kpi;
using DSLNG.PEAR.Services.Requests.Method;
using DSLNG.PEAR.Services.Responses.Method;
using DSLNG.PEAR.Services.Requests.Periode;
using DSLNG.PEAR.Services.Responses.Periode;
using DSLNG.PEAR.Services.Requests.Artifact;
using DSLNG.PEAR.Services.Responses.Artifact;
using DSLNG.PEAR.Services.Requests.KpiTarget;
using DSLNG.PEAR.Services.Requests.Conversion;
using DSLNG.PEAR.Services.Responses.KpiTarget;
using DSLNG.PEAR.Services.Requests.Template;
using DSLNG.PEAR.Services.Responses.Template;
using DSLNG.PEAR.Services.Requests.KpiAchievement;
using System.Linq;
using PeriodeType = DSLNG.PEAR.Data.Enums.PeriodeType;
using DSLNG.PEAR.Services.Responses.Config;
using DSLNG.PEAR.Services.Responses.Highlight;
using DSLNG.PEAR.Services.Requests.Highlight;
using DSLNG.PEAR.Services.Responses.Vessel;
using DSLNG.PEAR.Services.Requests.Vessel;
using DSLNG.PEAR.Services.Requests.Buyer;
using DSLNG.PEAR.Services.Responses.Buyer;
using DSLNG.PEAR.Services.Requests.VesselSchedule;
using DSLNG.PEAR.Services.Responses.VesselSchedule;
using DSLNG.PEAR.Services.Requests.NLS;
using DSLNG.PEAR.Services.Responses.NLS;
using DSLNG.PEAR.Services.Requests.CalculatorConstant;
using DSLNG.PEAR.Services.Responses.CalculatorConstant;
using DSLNG.PEAR.Services.Requests.ConstantUsage;
using DSLNG.PEAR.Services.Responses.ConstantUsage;
using DSLNG.PEAR.Services.Responses.Weather;
using DSLNG.PEAR.Services.Requests.Weather;
using DSLNG.PEAR.Services.Responses.HighlightOrder;
using DSLNG.PEAR.Data.Entities.EconomicModel;
using DSLNG.PEAR.Services.Responses.AssumptionCategory;
using DSLNG.PEAR.Services.Requests.AssumptionCategory;
using DSLNG.PEAR.Services.Responses.OutputCategory;
using DSLNG.PEAR.Services.Requests.OutputCategory;
using DSLNG.PEAR.Services.Responses.OperationGroup;
using DSLNG.PEAR.Services.Requests.OperationGroup;
using DSLNG.PEAR.Services.Responses.AssumptionConfig;
using DSLNG.PEAR.Services.Requests.AssumptionConfig;
using DSLNG.PEAR.Services.Responses.Scenario;
using DSLNG.PEAR.Services.Requests.Scenario;
using DSLNG.PEAR.Services.Responses.AssumptionData;
using DSLNG.PEAR.Services.Requests.AssumptionData;
using DSLNG.PEAR.Services.Responses.Operation;
using DSLNG.PEAR.Services.Requests.Operation;
using DSLNG.PEAR.Services.Responses.OperationalData;
using DSLNG.PEAR.Services.Requests.OperationalData;
using DSLNG.PEAR.Services.Responses.EconomicSummary;
using DSLNG.PEAR.Services.Requests.EconomicSummary;
using DSLNG.PEAR.Services.Responses.EconomicConfig;
using DSLNG.PEAR.Services.Requests.EconomicConfig;
using DSLNG.PEAR.Services.Responses.HighlightGroup;
using DSLNG.PEAR.Services.Requests.HighlightGroup;
using System.Collections.Generic;
using OGetKpisResponse = DSLNG.PEAR.Services.Responses.OutputConfig.GetKpisResponse;
using GetKpisResponse = DSLNG.PEAR.Services.Responses.Kpi.GetKpisResponse;
using DSLNG.PEAR.Services.Responses.OutputConfig;
using DSLNG.PEAR.Services.Requests.OutputConfig;
using DSLNG.PEAR.Services.Requests.PlanningBlueprint;
using DSLNG.PEAR.Data.Entities.Blueprint;
using DSLNG.PEAR.Services.Responses.PlanningBlueprint;
using DSLNG.PEAR.Services.Responses.BusinessPosture;
using DSLNG.PEAR.Services.Requests.BusinessPosture;
using DSLNG.PEAR.Services.Responses.EnvironmentScanning;
using DSLNG.PEAR.Services.Requests.EnvironmentScanning;
using DSLNG.PEAR.Services.Responses.MidtermFormulation;
using DSLNG.PEAR.Services.Requests.MidtermFormulation;
using DSLNG.PEAR.Services.Responses.MidtermPlanning;
using DSLNG.PEAR.Services.Requests.MidtermPlanning;
using DSLNG.PEAR.Data.Entities.Pop;
using DSLNG.PEAR.Services.Responses.PopDashboard;
using DSLNG.PEAR.Services.Requests.PopDashboard;
using DSLNG.PEAR.Services.Requests.PopInformation;
using DSLNG.PEAR.Services.Responses.PopInformation;
using PopInformationType = DSLNG.PEAR.Data.Enums.PopInformationType;
using DSLNG.PEAR.Services.Requests.Signature;
using DSLNG.PEAR.Services.Responses.Wave;
using DSLNG.PEAR.Services.Requests.Wave;
using DSLNG.PEAR.Data.Entities.Mir;
using DSLNG.PEAR.Services.Responses.MirConfiguration;
using DSLNG.PEAR.Services.Requests.MirConfiguration;
using DSLNG.PEAR.Services.Requests.MirDataTable;
using DSLNG.PEAR.Services.Responses.ProcessBlueprint;
using DSLNG.PEAR.Services.Requests.ProcessBlueprint;
using DSLNG.PEAR.Services.Requests.FileManagerRolePrivilege;
using DSLNG.PEAR.Data.Entities.Files;
using DSLNG.PEAR.Services.Responses.Files;
using DSLNG.PEAR.Services.Requests.Files;
using DSLNG.PEAR.Services.Responses.Privilege;
using DSLNG.PEAR.Services.Requests.Privilege;
using DSLNG.PEAR.Services.Responses.DerTransaction;
using DSLNG.PEAR.Data.Entities.InputOriginalData;
using DSLNG.PEAR.Services.Requests.InputData;
using DSLNG.PEAR.Services.Responses.InputData;
using DSLNG.PEAR.Services.Requests.KpiTransformation;
using DSLNG.PEAR.Data.Entities.KpiTransformationEngine;
using DSLNG.PEAR.Services.Responses.KpiInformation;
using DSLNG.PEAR.Services.Requests.KpiTransformationSchedule;
using DSLNG.PEAR.Services.Responses.KpiTransformationSchedule;
using DSLNG.PEAR.Services.Responses.KpiTransformationLog;
using DSLNG.PEAR.Services.Requests.KpiTransformationLog;
using DSLNG.PEAR.Services.Responses.AuditTrail;
using DSLNG.PEAR.Services.Requests.DerTransaction;
using DSLNG.PEAR.Services.Requests.AuditTrail;
using DSLNG.PEAR.Services.Requests;

namespace DSLNG.PEAR.Services.AutoMapper
{
    public class ServicesMappingProfile : Profile
    {
        protected override void Configure()
        {
            ConfigurePmsSummary();
            ConfigureKpi();
            ConfigureKpiTarget();
            ConfigurePmsConfigDetails();
            ConfigureKpiAchievements();
            ConfigureSelects();
            ConfigureKeyOperation();
            ConfigureEconomicSummary();
            ConfigureDer();
            ConfigureProcessBlueprint();
            ConfigureFileRepository();
            ConfigureInputData();
            ConfigureMixed();
            ConfigureAudit();
            base.Configure();
        }

        private void ConfigureAudit()
        {
            Mapper.CreateMap<Data.Entities.AuditTrail, AuditTrailResponse>();
            Mapper.CreateMap<Data.Entities.AuditTrail, AuditTrailsResponse.AuditTrail>()
                .ForMember(k => k.UserName, o => o.MapFrom(z => z.User.Username));
            Mapper.CreateMap<BaseRequest, BaseAction>();
            Mapper.CreateMap<DerDeleteRequest, BaseAction>();
            Mapper.CreateMap<CreateDerInputFileRequest, BaseAction>();
            Mapper.CreateMap<DeleteKpiAchievementRequest, BaseAction>();
            Mapper.CreateMap<DeleteRolePrivilegeRequest, BaseAction>();
            Mapper.CreateMap<SaveRolePrivilegeRequest, BaseAction>();
            Mapper.CreateMap<DeleteKpiRequest, BaseAction>();
            Mapper.CreateMap<CreateKpiResponse, BaseAction>();
            Mapper.CreateMap<UpdateKpiRequest, BaseAction>();
            Mapper.CreateMap<SaveOrUpdateInputDataRequest, BaseAction>();
            Mapper.CreateMap<DeleteInputDataRequest, BaseAction>();
            Mapper.CreateMap<SaveHighlightRequest, BaseAction>();
            // DER Services
            Mapper.CreateMap<CreateOrUpdateDerLayoutRequest, BaseAction>();
            Mapper.CreateMap<DeleteDerlayoutRequest, BaseAction>();
            Mapper.CreateMap<DeleteDerLayoutItemRequest, BaseAction>();
            Mapper.CreateMap<DeleteFilenameRequest, BaseAction>();
            Mapper.CreateMap<CreateOrUpdateDerRequest, BaseAction>();

            Mapper.CreateMap<UserLogin, LoginUserResponse.Login>();
            Mapper.CreateMap<UserLogin, AuditUserLoginResponse>();
            Mapper.CreateMap<AuditUser, AuditUserLoginResponse.AuditUser>();
            Mapper.CreateMap<User, AuditUserLoginResponse.User>();
            Mapper.CreateMap<AuditUserLoginResponse.User, User>();
            Mapper.CreateMap<AuditUsersResponse.AuditUser, AuditUser>();
            Mapper.CreateMap<AuditUser, AuditUsersResponse.AuditUser>();

            Mapper.CreateMap<User, AuditUserLoginsResponse.UserLogin>();
            Mapper.CreateMap<UserLogin, AuditUserLoginsResponse.UserLogin>()
                .ForMember(x=>x.Username,o=>o.MapFrom(z=>z.User.Username));
            Mapper.CreateMap<AuditUser, AuditUserLoginsResponse.UserLogin.AuditUser>();
            Mapper.CreateMap<CreateAuditUserRequest, AuditUser>();
            Mapper.CreateMap<DeleteTransformationRequest, BaseAction>();
            Mapper.CreateMap<SaveKpiTransformationRequest, BaseAction>();
            Mapper.CreateMap<DeleteKPITransformationScheduleRequest, BaseAction>();
            Mapper.CreateMap<SaveKpiTransformationScheduleRequest, BaseAction>();
            Mapper.CreateMap<SaveKpiTransformationScheduleResponse, BaseAction>();
            Mapper.CreateMap<CreateKpiRequest, BaseAction>();
            
        }

        private void ConfigureMixed()
        {
            Mapper.CreateMap<Data.Entities.User, GetUsersResponse.User>();
            Mapper.CreateMap<GetUsersResponse.User, Data.Entities.User>();
            //.ForMember(x => x.RoleName, o => o.MapFrom(m => m.Role.Name));
            Mapper.CreateMap<CreateUserRequest, User>();
            Mapper.CreateMap<UpdateUserRequest, User>();
            Mapper.CreateMap<GetUserRequest, User>();
            Mapper.CreateMap<Data.Entities.RoleGroup, GetUserResponse.RoleGroup>();
            Mapper.CreateMap<Data.Entities.RolePrivilege, GetUserResponse.RolePrivilege>();
            Mapper.CreateMap<Data.Entities.RolePrivilege, GetRoleGroupResponse.RolePrivilege>();
            Mapper.CreateMap<Data.Entities.RolePrivilege, GetPrivilegesResponse.RolePrivilege>();
            Mapper.CreateMap<Data.Entities.RoleGroup, GetUsersResponse.RoleGroup>();
            Mapper.CreateMap<Data.Entities.User, GetUserResponse>();
            Mapper.CreateMap<Data.Entities.User, LoginUserResponse>();
            Mapper.CreateMap<Data.Entities.RolePrivilege, LoginUserResponse.RolePrivilege>();
            Mapper.CreateMap<Data.Entities.RoleGroup, LoginUserResponse.RoleGroup>();
            Mapper.CreateMap<Data.Entities.ResetPassword, ResetPasswordResponse>();
            Mapper.CreateMap<ResetPasswordResponse, ResetPassword>();
            //Mapper.CreateMap<ResetPasswordResponse.User, Data.Entities.User>();
            Mapper.CreateMap<GetUserResponse, ResetPasswordResponse.User>();
            Mapper.CreateMap<MenuRolePrivilege, GetMenuPrivilegeResponse>();
            Mapper.CreateMap<Data.Entities.RolePrivilege, GetMenuPrivilegeResponse.Privilege>();
            Mapper.CreateMap<Data.Entities.Menu, GetMenuPrivilegeResponse.MenuPrivilege>();

            Mapper.CreateMap<MenuRolePrivilege, GetMenuRolePrivilegeResponse.MenuRolePrivilege>();
            Mapper.CreateMap<Data.Entities.RolePrivilege, GetMenuRolePrivilegeResponse.MenuRolePrivilege.Privilege>();
            Mapper.CreateMap<Data.Entities.RoleGroup, GetMenuRolePrivilegeResponse.MenuRolePrivilege.Privilege.RoleGroup>();
            Mapper.CreateMap<Data.Entities.Menu, GetMenuRolePrivilegeResponse.MenuRolePrivilege.MenuPrivilege>();
            //Mapper.CreateMap<
            /*
             * Privilege
             */
            Mapper.CreateMap<Data.Entities.RolePrivilege, GetPrivilegeResponse>();
            Mapper.CreateMap<Data.Entities.RoleGroup, GetPrivilegeResponse.RoleGroup>();
            Mapper.CreateMap<SaveRolePrivilegeRequest, Data.Entities.RolePrivilege>();
            Mapper.CreateMap<SaveRolePrivilegeRequest.MenuRolePrivilege, Data.Entities.MenuRolePrivilege>();
            /*Level*/

            Mapper.CreateMap<Data.Entities.Level, GetLevelsResponse.Level>();
            Mapper.CreateMap<Data.Entities.Level, GetLevelResponse>();
            Mapper.CreateMap<CreateLevelRequest, Data.Entities.Level>();
            Mapper.CreateMap<UpdateLevelRequest, Data.Entities.Level>();
            Mapper.CreateMap<Data.Entities.Level, UpdateLevelResponse>();

            Mapper.CreateMap<Data.Entities.Menu, GetSiteMenusResponse.Menu>();
            Mapper.CreateMap<Data.Entities.Menu, GetMenusResponse.Menu>();
            Mapper.CreateMap<CreateMenuRequest, Data.Entities.Menu>();
            Mapper.CreateMap<Data.Entities.Menu, GetMenuResponse>();
            Mapper.CreateMap<GetMenuResponse, UpdateMenuRequest>();
            Mapper.CreateMap<Data.Entities.Menu, DSLNG.PEAR.Services.Responses.Menu.Menu>();

            Mapper.CreateMap<UpdateMenuRequest, Data.Entities.Menu>();
            Mapper.CreateMap<Data.Entities.Level, Responses.Menu.Level>();
            Mapper.CreateMap<Data.Entities.RoleGroup, Responses.Menu.RoleGroup>();

            Mapper.CreateMap<Data.Entities.Menu, GetSiteMenuActiveResponse>();
            Mapper.CreateMap<GetSiteMenuActiveResponse, Data.Entities.Menu>();

            Mapper.CreateMap<Data.Entities.Group, GetGroupResponse>();
            Mapper.CreateMap<Data.Entities.Group, GetGroupsResponse.Group>();
            Mapper.CreateMap<CreateGroupRequest, Data.Entities.Group>();
            Mapper.CreateMap<UpdateGroupRequest, Data.Entities.Group>();


            Mapper.CreateMap<Data.Entities.Measurement, GetMeasurementsResponse>();
            Mapper.CreateMap<CreateMeasurementRequest, Data.Entities.Measurement>();
            Mapper.CreateMap<UpdateMeasurementRequest, Data.Entities.Measurement>();
            Mapper.CreateMap<Data.Entities.Measurement, UpdateMeasurementResponse>();
            Mapper.CreateMap<Data.Entities.Measurement, GetMeasurementResponse>();
            Mapper.CreateMap<GetMeasurementRequest, Data.Entities.Measurement>();
            Mapper.CreateMap<Data.Entities.Measurement, GetMeasurementsResponse.Measurement>();

            Mapper.CreateMap<Kpi, GetKpiToSeriesResponse.Kpi>()
                  .ForMember(x => x.Name, y => y.MapFrom(z => z.Name + "(" + z.Measurement.Name + ")"));
            Mapper.CreateMap<Kpi, GetKpisResponse.Kpi>()
                .ForMember(k => k.PillarName, o => o.MapFrom(k => k.Pillar.Name));
            Mapper.CreateMap<CreateKpiRequest, Kpi>()
                .ForMember(k => k.Period, o => o.MapFrom(k => k.Periode));
            Mapper.CreateMap<CreateKpiRequest, Data.Entities.Type>();
            Mapper.CreateMap<DSLNG.PEAR.Services.Requests.Kpi.KpiRelationModel, DSLNG.PEAR.Data.Entities.KpiRelationModel>();
            //Mapper.CreateMap<DSLNG.PEAR.Services.Requests.Kpi.Level, Data.Entities.Level>();
            Mapper.CreateMap<DSLNG.PEAR.Services.Requests.Kpi.RoleGroup, Data.Entities.RoleGroup>();
            //Mapper.CreateMap<DSLNG.PEAR.Services.Requests.Kpi.Type, Data.Entities.Type>();
            //Mapper.CreateMap<DSLNG.PEAR.Services.Requests.Kpi.Group, Data.Entities.Group>();
            //Mapper.CreateMap<DSLNG.PEAR.Services.Requests.Kpi.Measurement, Data.Entities.Measurement>();
            Mapper.CreateMap<Data.Entities.Level, DSLNG.PEAR.Services.Requests.Kpi.Level>();
            //Mapper.CreateMap<Data.Entities.RoleGroup, DSLNG.PEAR.Services.Requests.Kpi.RoleGroup>();
            Mapper.CreateMap<Data.Entities.Group, DSLNG.PEAR.Services.Requests.Kpi.Group>();
            Mapper.CreateMap<Data.Entities.Measurement, DSLNG.PEAR.Services.Requests.Kpi.Measurement>();
            Mapper.CreateMap<Data.Entities.Level, GetKpisResponse.Level>();
            Mapper.CreateMap<Data.Entities.RoleGroup, GetKpisResponse.RoleGroup>();
            Mapper.CreateMap<Data.Entities.Type, GetKpisResponse.Type>();
            Mapper.CreateMap<Data.Entities.Pillar, GetKpisResponse.Pillar>();

            Mapper.CreateMap<DSLNG.PEAR.Data.Entities.KpiRelationModel, DSLNG.PEAR.Services.Responses.Kpi.KpiRelationModel>()
                .ForMember(k => k.KpiId, o => o.MapFrom(k => k.Kpi.Id));

            Mapper.CreateMap<Data.Entities.Measurement, GetMeasurementsResponse>();
            Mapper.CreateMap<Data.Entities.Method, GetMethodResponse>();

            Mapper.CreateMap<Data.Entities.Conversion, GetConversionResponse>();

            Mapper.CreateMap<Data.Entities.RoleGroup, GetRoleGroupsResponse.RoleGroup>()
                .ForMember(x => x.LevelName, o => o.MapFrom(k => k.Level.Name));
            Mapper.CreateMap<Data.Entities.Level, Responses.RoleGroup.Level>();
            //Mapper.CreateMap<Data.Entities.RoleGroup, GetRoleGroupsResponse>();
            Mapper.CreateMap<Data.Entities.RoleGroup, GetRoleGroupResponse>();
            Mapper.CreateMap<CreateRoleGroupRequest, Data.Entities.RoleGroup>();
            Mapper.CreateMap<CreateRoleGroupRequest, Data.Entities.BaseAction>();
            Mapper.CreateMap<UpdateRoleGroupRequest, Data.Entities.RoleGroup>();
            Mapper.CreateMap<Data.Entities.RoleGroup, UpdateRoleGroupResponse>();

            Mapper.CreateMap<Data.Entities.Type, GetTypeResponse>();
            Mapper.CreateMap<Data.Entities.Type, GetTypesResponse>();
            Mapper.CreateMap<Data.Entities.Type, GetTypesResponse.Type>();
            Mapper.CreateMap<CreateTypeRequest, Data.Entities.Type>();
            Mapper.CreateMap<UpdateTypeRequest, Data.Entities.Type>();
            Mapper.CreateMap<Data.Entities.Type, UpdateTypeResponse>();
            Mapper.CreateMap<KpiAchievement, GetPmsDetailsResponse.KpiAchievment>()
                .ForMember(k => k.Period, o => o.MapFrom(k => k.Periode.ToString("MMM")))
                .ForMember(k => k.Type, o => o.MapFrom(k => k.PeriodeType.ToString()));
            Mapper.CreateMap<UpdateKpiAchievementItemRequest, KpiAchievement>()
                .ForMember(x => x.Value, y => y.MapFrom(z => z.RealValue));
            Mapper.CreateMap<UpdateKpiAchievementItemRequest, BaseAction>();
            Mapper.CreateMap<Data.Entities.Pillar, GetPillarsResponse>();
            Mapper.CreateMap<Data.Entities.Pillar, GetPillarResponse>();
            Mapper.CreateMap<Data.Entities.Pillar, GetPillarsResponse.Pillar>();

            Mapper.CreateMap<CreatePillarRequest, Data.Entities.Pillar>();
            Mapper.CreateMap<UpdatePillarRequest, Data.Entities.Pillar>();

            Mapper.CreateMap<Data.Entities.Method, GetMethodResponse>();
            Mapper.CreateMap<CreateMethodRequest, Data.Entities.Method>();
            Mapper.CreateMap<Data.Entities.Method, GetMethodsResponse.Method>();
            Mapper.CreateMap<UpdateMethodRequest, Data.Entities.Method>();

            Mapper.CreateMap<Data.Entities.Method, GetMethodsResponse.Method>();
            Mapper.CreateMap<Data.Entities.Group, GetGroupResponse>();

            Mapper.CreateMap<Data.Entities.Method, Responses.Kpi.Method>();
            Mapper.CreateMap<Data.Entities.Method, GetMethodResponse>();
            Mapper.CreateMap<Data.Entities.Level, Data.Entities.Kpi>();
            Mapper.CreateMap<Data.Entities.Periode, GetPeriodesResponse.Periode>();
            Mapper.CreateMap<Data.Entities.Periode, GetPeriodeResponse>();
            Mapper.CreateMap<CreatePeriodeRequest, Data.Entities.Periode>();
            Mapper.CreateMap<UpdatePeriodeRequest, Data.Entities.Periode>();

            Mapper.CreateMap<CreateArtifactRequest, Artifact>()
                .ForMember(x => x.Series, o => o.Ignore())
                .ForMember(x => x.Plots, o => o.Ignore())
                .ForMember(x => x.Rows, o => o.Ignore())
                .ForMember(x => x.Tank, o => o.Ignore())
                .ForMember(x => x.Charts, o => o.Ignore());
            Mapper.CreateMap<CreateArtifactRequest.SeriesRequest, ArtifactSerie>()
                .ForMember(x => x.Stacks, o => o.Ignore());
            Mapper.CreateMap<CreateArtifactRequest.PlotRequest, ArtifactPlot>();
            Mapper.CreateMap<CreateArtifactRequest.StackRequest, ArtifactStack>();
            Mapper.CreateMap<CreateArtifactRequest.RowRequest, ArtifactRow>();
            Mapper.CreateMap<CreateArtifactRequest.ChartRequest, ArtifactChart>()
                .ForMember(x => x.Series, o => o.Ignore());
            Mapper.CreateMap<CreateArtifactRequest, BaseAction>();
            Mapper.CreateMap<UpdateArtifactRequest, Artifact>()
               .ForMember(x => x.Series, o => o.Ignore())
               .ForMember(x => x.Plots, o => o.Ignore())
               .ForMember(x => x.Charts, o => o.Ignore());
            Mapper.CreateMap<UpdateArtifactRequest.SeriesRequest, ArtifactSerie>()
                .ForMember(x => x.Stacks, o => o.Ignore());
            Mapper.CreateMap<UpdateArtifactRequest.PlotRequest, ArtifactPlot>();
            Mapper.CreateMap<UpdateArtifactRequest.StackRequest, ArtifactStack>();
            Mapper.CreateMap<UpdateArtifactRequest.TankRequest, ArtifactTank>();
            Mapper.CreateMap<UpdateArtifactRequest.RowRequest, ArtifactRow>();
            Mapper.CreateMap<UpdateArtifactRequest.ChartRequest, ArtifactChart>()
                .ForMember(x => x.Series, o => o.Ignore());
            Mapper.CreateMap<UpdateArtifactRequest, BaseAction>();

            Mapper.CreateMap<Artifact, GetArtifactsResponse.Artifact>()
                .ForMember(x => x.Used, o => o.MapFrom(x => x.LayoutColumns.Count > 0));
            Mapper.CreateMap<Artifact, GetArtifactResponse>()
                .ForMember(x => x.PlotBands, o => o.MapFrom(s => s.Plots.MapTo<GetArtifactResponse.PlotResponse>()))
                .ForMember(x => x.Series, o => o.MapFrom(s => s.Series.MapTo<GetArtifactResponse.SeriesResponse>()))
                .ForMember(x => x.Measurement, o => o.MapFrom(s => s.Measurement.Name))
                .ForMember(x => x.MeasurementId, o => o.MapFrom(s => s.Measurement.Id));
            Mapper.CreateMap<ArtifactPlot, GetArtifactResponse.PlotResponse>();
            Mapper.CreateMap<ArtifactSerie, GetArtifactResponse.SeriesResponse>()
                  .ForMember(x => x.Stacks, o => o.MapFrom(s => s.Stacks.MapTo<GetArtifactResponse.StackResponse>()))
                  .ForMember(x => x.KpiId, o => o.MapFrom(s => s.Kpi.Id))
                  .ForMember(x => x.KpiName, o => o.MapFrom(s => s.Kpi.Name + " (" + s.Kpi.Measurement.Name + ")"));
            Mapper.CreateMap<ArtifactStack, GetArtifactResponse.StackResponse>()
                .ForMember(x => x.KpiId, o => o.MapFrom(s => s.Kpi.Id))
                .ForMember(x => x.KpiName, o => o.MapFrom(s => s.Kpi.Name + " (" + s.Kpi.Measurement.Name + ")"));
            Mapper.CreateMap<ArtifactRow, GetArtifactResponse.RowResponse>()
                  .ForMember(x => x.KpiId, o => o.MapFrom(s => s.Kpi.Id))
                  .ForMember(x => x.KpiName, o => o.MapFrom(s => s.Kpi.Name + " (" + s.Kpi.Measurement.Name + ")"));
            Mapper.CreateMap<ArtifactTank, GetArtifactResponse.TankResponse>()
               .ForMember(x => x.VolumeInventoryId, o => o.MapFrom(s => s.VolumeInventory.Id))
               .ForMember(x => x.VolumeInventory, o => o.MapFrom(s => s.VolumeInventory.Name + " (" + s.VolumeInventory.Measurement.Name + ")"))
               .ForMember(x => x.DaysToTankTopId, o => o.MapFrom(s => s.DaysToTankTop.Id))
               .ForMember(x => x.DaysToTankTop, o => o.MapFrom(s => s.DaysToTankTop.Name + " (" + s.DaysToTankTop.Measurement.Name + ")"));
            Mapper.CreateMap<ArtifactChart, GetArtifactResponse.ChartResponse>()
                .ForMember(x => x.MeasurementId, o => o.MapFrom(s => s.Measurement.Id));
            Mapper.CreateMap<CreateConversionRequest, Data.Entities.Conversion>();
            Mapper.CreateMap<Data.Entities.Conversion, GetConversionsResponse.Conversion>()
                .ForMember(f => f.FromName, o => o.MapFrom(k => k.From.Name))
                .ForMember(f => f.ToName, o => o.MapFrom(k => k.To.Name));
            Mapper.CreateMap<Data.Entities.Conversion, GetConversionResponse>();
            Mapper.CreateMap<Data.Entities.Measurement, Responses.Conversion.Measurement>();
            Mapper.CreateMap<UpdateConversionRequest, Data.Entities.Conversion>();
            Mapper.CreateMap<KpiTarget, GetKpiTargetsResponse.KpiTarget>()
               .ForMember(k => k.KpiName, o => o.MapFrom(k => k.Kpi.Name))
               .ForMember(k => k.KpiId, o => o.MapFrom(k => k.Kpi.Id))
               .ForMember(k => k.MeasurementName, o => o.MapFrom(k => k.Kpi.Measurement != null ? k.Kpi.Measurement.Name : string.Empty ))
               .ForMember(k => k.PeriodeType, o => o.MapFrom(k => k.PeriodeType.ToString()));

            Mapper.CreateMap<Artifact, GetArtifactsToSelectResponse.ArtifactResponse>()
                .ForMember(d => d.Name, o => o.MapFrom(s => s.GraphicName));
            Mapper.CreateMap<CreateTemplateRequest, DashboardTemplate>()
                .ForMember(d => d.LayoutRows, o => o.Ignore());
            Mapper.CreateMap<GetTabularDataRequest, GetTabularDataResponse>()
                .ForMember(d => d.Rows, o => o.Ignore());
            Mapper.CreateMap<GetTankDataRequest.TankRequest, GetTankDataResponse>()
                .ForMember(d => d.DaysToTankTop, o => o.Ignore())
                .ForMember(d => d.VolumeInventory, o => o.Ignore());
            //Mapper.CreateMap<CreateTemplateRequest.RowRequest, LayoutRow>();
            //Mapper.CreateMap<CreateTemplateRequest.ColumnRequest, LayoutColumn>();
            Mapper.CreateMap<DashboardTemplate, GetTemplatesResponse.TemplateResponse>();
            Mapper.CreateMap<DashboardTemplate, GetTemplateResponse>();
            Mapper.CreateMap<LayoutRow, GetTemplateResponse.RowResponse>();
            Mapper.CreateMap<LayoutColumn, GetTemplateResponse.ColumnResponse>()
                .ForMember(d => d.ArtifactId, o => o.MapFrom(s => s.Artifact.Id))
                .ForMember(d => d.ArtifactName, o => o.MapFrom(s => s.Artifact.GraphicName))
                .ForMember(d => d.HighlightTypeId, o => o.MapFrom(s => s.HighlightType.Id));
            Mapper.CreateMap<GetMultiaxisChartDataRequest, GetCartesianChartDataRequest>();
            Mapper.CreateMap<GetMultiaxisChartDataRequest.ChartRequest, GetCartesianChartDataRequest>();
            Mapper.CreateMap<GetMultiaxisChartDataRequest.ChartRequest.SeriesRequest, GetCartesianChartDataRequest.SeriesRequest>();
            Mapper.CreateMap<GetMultiaxisChartDataRequest.ChartRequest.StackRequest, GetCartesianChartDataRequest.StackRequest>();
            Mapper.CreateMap<GetCartesianChartDataResponse, GetMultiaxisChartDataResponse.ChartResponse>();
            Mapper.CreateMap<GetCartesianChartDataResponse.SeriesResponse, GetMultiaxisChartDataResponse.ChartResponse.SeriesViewModel>()
                .ForMember(x => x.marker, y => y.MapFrom(z => new GetMultiaxisChartDataResponse.ChartResponse.SeriesViewModel.MarkerViewModel { fillColor = z.MarkerColor, lineColor = z.MarkerColor }))
                .ForMember(x => x.dashStyle, y => y.MapFrom(z => z.LineType)); ;
            Mapper.CreateMap<GetArtifactResponse, GetMultiaxisChartDataRequest>();
            Mapper.CreateMap<GetArtifactResponse.ChartResponse, GetMultiaxisChartDataRequest.ChartRequest>();
            Mapper.CreateMap<GetArtifactResponse.SeriesResponse, GetMultiaxisChartDataRequest.ChartRequest.SeriesRequest>();
            Mapper.CreateMap<GetArtifactResponse.StackResponse, GetMultiaxisChartDataRequest.ChartRequest.StackRequest>();

            Mapper.CreateMap<GetComboChartDataRequest, GetCartesianChartDataRequest>();
            Mapper.CreateMap<GetComboChartDataRequest.ChartRequest, GetCartesianChartDataRequest>();
            Mapper.CreateMap<GetComboChartDataRequest.ChartRequest.SeriesRequest, GetCartesianChartDataRequest.SeriesRequest>();
            Mapper.CreateMap<GetComboChartDataRequest.ChartRequest.StackRequest, GetCartesianChartDataRequest.StackRequest>();
            Mapper.CreateMap<GetCartesianChartDataResponse, GetComboChartDataResponse.ChartResponse>();
            Mapper.CreateMap<GetCartesianChartDataResponse.SeriesResponse, GetComboChartDataResponse.ChartResponse.SeriesViewModel>()
                .ForMember(x => x.marker, y => y.MapFrom(z => new GetComboChartDataResponse.ChartResponse.SeriesViewModel.MarkerViewModel { fillColor = z.MarkerColor, lineColor = z.MarkerColor }))
                .ForMember(x => x.dashStyle, y => y.MapFrom(z => z.LineType));
            Mapper.CreateMap<GetArtifactResponse, GetComboChartDataRequest>();
            Mapper.CreateMap<GetArtifactResponse.ChartResponse, GetComboChartDataRequest.ChartRequest>();
            Mapper.CreateMap<GetArtifactResponse.SeriesResponse, GetComboChartDataRequest.ChartRequest.SeriesRequest>();
            Mapper.CreateMap<GetArtifactResponse.StackResponse, GetComboChartDataRequest.ChartRequest.StackRequest>();

            Mapper.CreateMap<Kpi, GetConfigurationResponse.Kpi>();
            Mapper.CreateMap<KpiAchievement, GetConfigurationResponse.KpiAchievement>();
            Mapper.CreateMap<KpiTarget, GetConfigurationResponse.KpiTarget>();
            Mapper.CreateMap<Economic, GetConfigurationResponse.Economic>();
            Mapper.CreateMap<Highlight, GetHighlightsResponse.HighlightResponse>()
                .ForMember(x => x.Type, o => o.MapFrom(s => s.HighlightType.Text));
            Mapper.CreateMap<SaveHighlightRequest, Highlight>();

            Mapper.CreateMap<SaveVesselRequest, Vessel>();
            Mapper.CreateMap<Vessel, GetVesselsResponse.VesselResponse>()
                .ForMember(x => x.Measurement, o => o.MapFrom(s => s.Measurement.Name))
                .ForMember(x => x.MeasurementId, o => o.MapFrom(s => s.Measurement.Id));
            Mapper.CreateMap<Vessel, GetVesselResponse>()
                .ForMember(x => x.Measurement, o => o.MapFrom(s => s.Measurement.Name))
                .ForMember(x => x.MeasurementId, o => o.MapFrom(s => s.Measurement.Id));

            Mapper.CreateMap<SaveBuyerRequest, Buyer>();
            Mapper.CreateMap<Buyer, GetBuyersResponse.BuyerResponse>();
            Mapper.CreateMap<Buyer, GetBuyerResponse>();
            Mapper.CreateMap<Buyer, GetBuyerForGridResponse.BuyerForGrid>();

            Mapper.CreateMap<SaveVesselScheduleRequest, VesselSchedule>();
            Mapper.CreateMap<VesselSchedule, GetVesselSchedulesResponse.VesselScheduleResponse>()
                .ForMember(x => x.Vessel, o => o.MapFrom(s => s.Vessel.Name))
                .ForMember(x => x.Buyer, o => o.MapFrom(s => s.Buyer.Name));
            Mapper.CreateMap<VesselSchedule, GetVesselScheduleResponse>()
                .ForMember(x => x.VesselId, o => o.MapFrom(s => s.Vessel.Id))
                .ForMember(x => x.BuyerId, o => o.MapFrom(s => s.Buyer.Id))
                .ForMember(x => x.VesselName, o => o.MapFrom(s => s.Vessel.Name))
                .ForMember(x => x.BuyerName, o => o.MapFrom(s => s.Buyer.Name));

            Mapper.CreateMap<SaveNLSRequest, NextLoadingSchedule>();
            Mapper.CreateMap<NextLoadingSchedule, GetNLSListResponse.NLSResponse>()
                .ForMember(x => x.Vessel, o => o.MapFrom(s => s.VesselSchedule.Vessel.Name))
                .ForMember(x => x.ETA, o => o.MapFrom(s => s.VesselSchedule.ETA))
                .ForMember(x => x.ETD, o => o.MapFrom(s => s.VesselSchedule.ETD));
            Mapper.CreateMap<NextLoadingSchedule, GetNLSResponse>()
                .ForMember(x => x.VesselScheduleId, o => o.MapFrom(s => s.VesselSchedule.Id))
                 .ForMember(x => x.VesselName, o => o.MapFrom(s => s.VesselSchedule.Vessel.Name));

            Mapper.CreateMap<SaveCalculatorConstantRequest, CalculatorConstant>();
            Mapper.CreateMap<CalculatorConstant, GetCalculatorConstantsResponse.CalculatorConstantResponse>();
            Mapper.CreateMap<CalculatorConstant, GetCalculatorConstantResponse>();
            Mapper.CreateMap<CalculatorConstant, GetCalculatorConstantsForGridRespone.CalculatorConstantsForGrid>();

            Mapper.CreateMap<SaveConstantUsageRequest, ConstantUsage>();
            Mapper.CreateMap<ConstantUsage, GetConstantUsagesResponse.ConstantUsageResponse>()
                .ForMember(x => x.StringConstants, o => o.MapFrom(s => s.Constants.Select(x => x.Name)));
            Mapper.CreateMap<CalculatorConstant, GetConstantUsagesResponse.ConstantResponse>();
            Mapper.CreateMap<ConstantUsage, GetConstantUsageResponse>();
            Mapper.CreateMap<CalculatorConstant, GetConstantUsageResponse.CalculatorConstantResponse>();

            Mapper.CreateMap<Highlight, GetHighlightResponse>()
                .ForMember(x => x.TypeId, o => o.MapFrom(s => s.HighlightType.Id));
            Mapper.CreateMap<Weather, GetWeathersResponse.WeatherResponse>()
                .ForMember(x => x.Value, o => o.MapFrom(s => s.Value.Text));
            Mapper.CreateMap<SaveWeatherRequest, Weather>();
            Mapper.CreateMap<SaveWeatherRequest, BaseAction>();
            Mapper.CreateMap<Weather, GetWeatherResponse>()
                .ForMember(x => x.Value, o => o.MapFrom(s => s.Value.Value))
                .ForMember(x => x.Text, o => o.MapFrom(s => s.Value.Text));
            Mapper.CreateMap<SelectOption, GetHighlightOrdersResponse.HighlightOrderResponse>()
                .ForMember(x => x.GroupId, o => o.MapFrom(x => x.Group != null ? x.Group.Id : 0))
                .ForMember(x => x.RoleGroupIds, o => o.MapFrom(s => s.RoleGroups.Select(x => x.Id).ToArray()));
            Mapper.CreateMap<KeyAssumptionCategory, GetAssumptionCategoriesResponse.AssumptionCategory>()
                .ForMember(x => x.Assumptions, o =>
                    o.MapFrom(s => s.KeyAssumptions != null ?
                        s.KeyAssumptions.Where(x => x.IsActive == true).MapTo<GetAssumptionCategoriesResponse.Assumption>()
                        : new List<GetAssumptionCategoriesResponse.Assumption>()));
            Mapper.CreateMap<KeyAssumptionConfig, GetAssumptionCategoriesResponse.Assumption>()
                .ForMember(x => x.Measurement, o => o.MapFrom(s => s.Measurement.Name));
            Mapper.CreateMap<SaveAssumptionCategoryRequest, KeyAssumptionCategory>();
            Mapper.CreateMap<KeyAssumptionCategory, GetAssumptionCategoryResponse>();

            Mapper.CreateMap<KeyOutputCategory, GetOutputCategoriesResponse.OutputCategory>();
            Mapper.CreateMap<SaveOutputCategoryRequest, KeyOutputCategory>();
            Mapper.CreateMap<KeyOutputCategory, GetOutputCategoryResponse>();


            Mapper.CreateMap<KeyOperationGroup, GetOperationGroupsResponse.OperationGroup>();
            Mapper.CreateMap<SaveOperationGroupRequest, KeyOperationGroup>();
            Mapper.CreateMap<KeyOperationGroup, GetOperationGroupResponse>();

            Mapper.CreateMap<KeyAssumptionConfig, GetAssumptionConfigsResponse.AssumptionConfig>()
                .ForMember(x => x.Category, o => o.MapFrom(s => s.Category.Name))
                .ForMember(x => x.Measurement, o => o.MapFrom(s => s.Measurement.Name));
            Mapper.CreateMap<KeyAssumptionCategory, GetAssumptionConfigCategoryResponse.AssumptionConfigCategoryResponse>();
            Mapper.CreateMap<Data.Entities.Measurement, GetAssumptionConfigCategoryResponse.MeasurementSelectList>();
            Mapper.CreateMap<KeyAssumptionConfig, GetAssumptionConfigResponse>()
                .ForMember(x => x.IdCategory, o => o.MapFrom(s => s.Category.Id))
                .ForMember(x => x.IdMeasurement, o => o.MapFrom(s => s.Measurement.Id));
            Mapper.CreateMap<SaveAssumptionConfigRequest, KeyAssumptionConfig>()
                .ForMember(x => x.Category, o => o.Ignore())
                .ForMember(x => x.Measurement, o => o.Ignore());

            Mapper.CreateMap<Scenario, GetScenariosResponse.Scenario>();
            Mapper.CreateMap<SaveScenarioRequest, Scenario>();
            Mapper.CreateMap<Scenario, GetScenarioResponse>();


            Mapper.CreateMap<KeyAssumptionData, GetAssumptionDatasResponse.AssumptionData>()
                .ForMember(x => x.Scenario, o => o.MapFrom(s => s.Scenario.Name))
                .ForMember(x => x.IdScenario, o => o.MapFrom(s => s.Scenario.Id))
                .ForMember(x => x.IdConfig, o => o.MapFrom(s => s.KeyAssumptionConfig.Id))
                .ForMember(x => x.Config, o => o.MapFrom(s => s.KeyAssumptionConfig.Name));
            Mapper.CreateMap<KeyAssumptionConfig, GetAssumptionDataConfigResponse.AssumptionDataConfig>()
                .ForMember(x => x.Measurement, o => o.MapFrom(s => s.Measurement.Name));
            Mapper.CreateMap<Scenario, GetAssumptionDataConfigResponse.Scenario>();
            Mapper.CreateMap<SaveAssumptionDataRequest, KeyAssumptionData>()
                .ForMember(x => x.Scenario, o => o.Ignore())
                .ForMember(x => x.KeyAssumptionConfig, o => o.Ignore())
                .ForMember(x => x.ActualValue, o => o.Condition(s => !s.IsSourceValueNull))
                .ForMember(x => x.ForecastValue, o => o.Condition(s => !s.IsSourceValueNull));
            Mapper.CreateMap<KeyAssumptionData, GetAssumptionDataResponse>()
                .ForMember(x => x.IdScenario, o => o.MapFrom(s => s.Scenario.Id))
                .ForMember(x => x.IdConfig, o => o.MapFrom(s => s.KeyAssumptionConfig.Id));

            Mapper.CreateMap<KeyOperationConfig, GetOperationsResponse.Operation>()
                //.ForMember(x => x.KeyOperationGroup, o => o.MapFrom(s => s.KeyOperationGroup.Name))
                .ForMember(x => x.KeyOperationGroupId, o => o.MapFrom(s => s.KeyOperationGroup.Id))
                .ForMember(x => x.Kpi, o => o.MapFrom(s => s.Kpi.Name + " (" + s.Kpi.Measurement.Name + ") "))
                .ForMember(x => x.KpiId, o => o.MapFrom(s => s.Kpi.Id));
            Mapper.CreateMap<KeyOperationGroup, OperationGroupsResponse.OperationGroup>();
            Mapper.CreateMap<Kpi, OperationGroupsResponse.Kpi>();
            Mapper.CreateMap<SaveOperationRequest, KeyOperationConfig>()
                .ForMember(x => x.KeyOperationGroup, o => o.Ignore())
                .ForMember(x => x.Kpi, o => o.Ignore());
            Mapper.CreateMap<KeyOperationConfig, GetOperationResponse>()
                .ForMember(x => x.KeyOperationGroupId, o => o.MapFrom(s => s.KeyOperationGroup.Id))
                .ForMember(x => x.KpiId, o => o.MapFrom(s => s.Kpi.Id));

            Mapper.CreateMap<KeyOperationData, GetOperationalDatasResponse.OperationalData>()
                .ForMember(x => x.KeyOperation, o => o.MapFrom(s => s.KeyOperationConfig.Kpi.Name))
                .ForMember(x => x.Kpi, o => o.MapFrom(s => s.Kpi.Name))
                .ForMember(x => x.Scenario, o => o.MapFrom(s => s.Scenario.Name));
            Mapper.CreateMap<KeyOperationConfig, GetOperationalSelectListResponse.Operation>();
            Mapper.CreateMap<Kpi, GetOperationalSelectListResponse.KPI>();
            Mapper.CreateMap<SaveOperationalDataRequest, KeyOperationData>()
                //.ForMember(x => x.KeyOperation, o => o.Ignore())
                .ForMember(x => x.Kpi, o => o.Ignore());
            Mapper.CreateMap<KeyOperationData, GetOperationalDataResponse>()
                //.ForMember(x => x.IdKeyOperation, o => o.MapFrom(s => s.KeyOperation.Id))
                .ForMember(x => x.IdKPI, o => o.MapFrom(s => s.Kpi.Id));


            Mapper.CreateMap<HighlightGroup, GetHighlightGroupsResponse.HighlightGroupResponse>()
                .ForMember(x => x.HighlightTypes, o => o.MapFrom(s => s.Options.OrderBy(x => x.Order).Where(x => x.IsActive == true).ToList()));
            Mapper.CreateMap<HighlightGroup, GetHighlightGroupResponse>();
            Mapper.CreateMap<SaveHighlightGroupRequest, HighlightGroup>();
            Mapper.CreateMap<SelectOption, GetHighlightGroupsResponse.HighlightTypeResponse>()
                .ForMember(x => x.RoleGroupIds, o => o.MapFrom(s => s.RoleGroups.Select(y => y.Id).ToArray()));
            Mapper.CreateMap<Highlight, GetDynamicHighlightsResponse.HighlightResponse>()
                .ForMember(x => x.TypeId, o => o.MapFrom(s => s.HighlightType.Id));

            Mapper.CreateMap<Kpi, OGetKpisResponse.Kpi>()
                .ForMember(x => x.Name, o => o.MapFrom(s => s.Name + " (" + s.Measurement.Name + ")"));
            Mapper.CreateMap<KeyAssumptionConfig, GetKeyAssumptionsResponse.KeyAssumption>();
            Mapper.CreateMap<SaveOutputConfigRequest, KeyOutputConfiguration>()
                .ForMember(x => x.KeyAssumptionIds, o => o.MapFrom(s => string.Join(",", s.KeyAssumptionIds)))
                .ForMember(x => x.KpiIds, o => o.MapFrom(s => string.Join(",", s.KpiIds)));
            Mapper.CreateMap<KeyOutputConfiguration, GetOutputConfigResponse>()
                .ForMember(x => x.MeasurementId, o => o.MapFrom(s => s.Measurement == null ? 0 : s.Measurement.Id))
                .ForMember(x => x.CategoryId, o => o.MapFrom(s => s.Category.Id))
                .ForMember(x => x.KpiIds, o => o.MapFrom(s => s.KpiIds.Split(',').Select(m => int.Parse(m)).ToList()))
                .ForMember(x => x.KeyAssumptionIds, o => o.MapFrom(s => s.KeyAssumptionIds.Split(',').Select(m => int.Parse(m)).ToList()));
            Mapper.CreateMap<Kpi, GetOutputConfigResponse.Kpi>();
            Mapper.CreateMap<KeyAssumptionConfig, GetOutputConfigResponse.KeyAssumptionConfig>();

            Mapper.CreateMap<KeyOutputConfiguration, GetOutputConfigsResponse.OutputConfig>()
                .ForMember(x => x.Category, o => o.MapFrom(s => s.Category.Name))
                .ForMember(x => x.Measurement, o => o.MapFrom(s => s.Measurement.Name));

            //Mapper.CreateMap<KeyOutputCategory, GetActiveOutputCategoriesResponse.OutputCategoryResponse>();
            //Mapper.CreateMap<KeyOutputConfiguration, GetActiveOutputCategoriesResponse.KeyOutputResponse>()
            //    .ForMember(x => x.
            Mapper.CreateMap<KeyOutputCategory, CalculateOutputResponse.OutputCategoryResponse>()
                .ForMember(x => x.KeyOutputs, o => o.Ignore());
            Mapper.CreateMap<KeyOutputConfiguration, CalculateOutputResponse.KeyOutputResponse>()
                .ForMember(x => x.Measurement, o => o.MapFrom(s => s.Measurement.Name));
            Mapper.CreateMap<KeyOperationData, GetOperationIdResponse.OperationData>()
                .ForMember(x => x.Kpi, o => o.MapFrom(s => s.Kpi.Id))
                .ForMember(x => x.KeyOperationConfig, o => o.MapFrom(s => s.KeyOperationConfig.Id))
                .ForMember(x => x.Scenario, o => o.MapFrom(s => s.Scenario.Id));

            Mapper.CreateMap<StaticHighlightPrivilege, GetStaticHighlightOrdersResponse.HighlightOrderResponse>()
                .ForMember(x => x.RoleGroupIds, o => o.MapFrom(s => s.RoleGroups.Select(x => x.Id).ToArray()));
            Mapper.CreateMap<SavePlanningBlueprintRequest, PlanningBlueprint>();
            Mapper.CreateMap<PlanningBlueprint, GetPlanningBlueprintsResponse.PlanningBlueprint>();
            Mapper.CreateMap<PlanningBlueprint, GetPlanningBlueprintResponse>()
                .ForMember(x => x.KeyOutputs, o => o.MapFrom(s => s.KeyOutput));
            Mapper.CreateMap<KeyOutputConfiguration, GetPlanningBlueprintResponse.KeyOutputResponse>();
            Mapper.CreateMap<EnvironmentsScanning, GetPlanningBlueprintsResponse.EnvironmentsScanning>();
            Mapper.CreateMap<BusinessPostureIdentification, GetPlanningBlueprintsResponse.BusinessPostureIdentification>();
            Mapper.CreateMap<MidtermPhaseFormulation, GetPlanningBlueprintsResponse.MidtermPhaseFormulation>();
            Mapper.CreateMap<MidtermStrategyPlanning, GetPlanningBlueprintsResponse.MidtermStragetyPlanning>();
            Mapper.CreateMap<Kpi, GetKpiDetailResponse>()
                  .ForMember(x => x.Code, y => y.MapFrom(z => z.Code))
                  .ForMember(x => x.Group, y => y.MapFrom(z => z.Group.Name))
                  .ForMember(x => x.Measurement, y => y.MapFrom(z => z.Measurement.Name))
                  .ForMember(x => x.Method, y => y.MapFrom(z => z.Method.Name))
                  .ForMember(x => x.Periode, y => y.MapFrom(z => z.Period.ToString()))
                  .ForMember(x => x.RoleGroup, y => y.MapFrom(z => z.RoleGroup.Name))
                  .ForMember(x => x.Type, y => y.MapFrom(z => z.Type.Name))
                  .ForMember(x => x.Pillar, y => y.MapFrom(z => z.Pillar.Name))
                  .ForMember(x => x.IsActive, y => y.MapFrom(z => z.IsActive.ToString()))
                  .ForMember(x => x.YtdFormula, y => y.MapFrom(z => z.YtdFormula.ToString()))
                  .ForMember(x => x.Level, y => y.MapFrom(z => z.Level.Name.ToString()))
                ;
            Mapper.CreateMap<BusinessPostureIdentification, GetBusinessPostureResponse>()
                .ForMember(x => x.PlanningBlueprintId, o => o.MapFrom(s => s.PlanningBlueprint.Id));
            Mapper.CreateMap<ESCategory, GetESCategoriesResponse.ESCategory>();
            Mapper.CreateMap<DesiredState, GetBusinessPostureResponse.DesiredState>();
            Mapper.CreateMap<Posture, GetBusinessPostureResponse.Posture>();
            Mapper.CreateMap<PostureChallenge, GetBusinessPostureResponse.PostureChallenge>()
                .ForMember(x => x.HasRelation, o => o.MapFrom(y => y.DesiredStates.Count > 0))
                .ForMember(x => x.Ids, o => o.MapFrom(y => y.DesiredStates.Select(z => z.Id)));
            Mapper.CreateMap<PostureConstraint, GetBusinessPostureResponse.PostureConstraint>()
                .ForMember(x => x.HasRelation, o => o.MapFrom(y => y.DesiredStates.Count > 0))
                .ForMember(x => x.Ids, o => o.MapFrom(y => y.DesiredStates.Select(z => z.Id)));
            Mapper.CreateMap<EnvironmentsScanning, GetBusinessPostureResponse.EnvironmentScanning>();
            Mapper.CreateMap<UltimateObjectivePoint, GetBusinessPostureResponse.EnvironmentScanning.UltimateObjective>();
            Mapper.CreateMap<Constraint, GetBusinessPostureResponse.EnvironmentScanning.Constraint>()
                .ForMember(x => x.Category, o => o.MapFrom(s => s.ESCategory.Name));
            Mapper.CreateMap<Challenge, GetBusinessPostureResponse.EnvironmentScanning.Challenge>()
                .ForMember(x => x.Category, o => o.MapFrom(s => s.ESCategory.Name));



            Mapper.CreateMap<SaveDesiredStateRequest, DesiredState>();
            Mapper.CreateMap<DesiredState, SaveDesiredStateResponse>();
            Mapper.CreateMap<SavePostureChallengeRequest, PostureChallenge>();
            Mapper.CreateMap<SavePostureConstraintRequest, PostureConstraint>();

            Mapper.CreateMap<EnvironmentsScanning, GetEnvironmentsScanningResponse>()
                .ForMember(x => x.BusinessPostureId, o => o.MapFrom(s => s.PlanningBlueprint.BusinessPostureIdentification.Id))
                .ForMember(x => x.IsApproved, o => o.MapFrom(s => s.PlanningBlueprint.BusinessPostureIdentification.IsApproved))
                .ForMember(x => x.IsBeingReviewed, o => o.MapFrom(s => s.PlanningBlueprint.BusinessPostureIdentification.IsBeingReviewed))
                .ForMember(x => x.IsRejected, o => o.MapFrom(s => s.PlanningBlueprint.BusinessPostureIdentification.IsRejected));
            Mapper.CreateMap<UltimateObjectivePoint, GetEnvironmentsScanningResponse.UltimateObjective>();
            Mapper.CreateMap<EnvironmentalScanning, GetEnvironmentsScanningResponse.Environmental>();
            Mapper.CreateMap<SaveEnvironmentScanningRequest, UltimateObjectivePoint>();
            Mapper.CreateMap<SaveEnvironmentalScanningRequest, EnvironmentalScanning>()
                .ForMember(x => x.Desc, y => y.MapFrom(z => z.Description));
            Mapper.CreateMap<Constraint, GetEnvironmentsScanningResponse.Constraint>()
                .ForMember(x => x.ThreatIds, y => y.MapFrom(z => z.Relations.Where(u => u.ThreatHost != null).Select(m => m.Id).ToArray()))
                .ForMember(x => x.OpportunityIds, y => y.MapFrom(z => z.Relations.Where(u => u.OpportunityHost != null).Select(m => m.Id).ToArray()))
                .ForMember(x => x.WeaknessIds, y => y.MapFrom(z => z.Relations.Where(u => u.WeaknessHost != null).Select(m => m.Id).ToArray()))
                .ForMember(x => x.StrengthIds, y => y.MapFrom(z => z.Relations.Where(u => u.StrengthHost != null).Select(m => m.Id).ToArray()))
                .ForMember(x => x.RelationIds, y => y.MapFrom(z => z.Relations.Select(m => m.Id).ToArray()))
                .ForMember(x => x.Category, o => o.MapFrom(y => y.ESCategory.Name))
                .ForMember(x => x.CategoryId, o => o.MapFrom(y => y.ESCategory.Id));
            Mapper.CreateMap<Challenge, GetEnvironmentsScanningResponse.Challenge>()
                .ForMember(x => x.ThreatIds, y => y.MapFrom(z => z.Relations.Where(u => u.ThreatHost != null).Select(m => m.Id).ToArray()))
                .ForMember(x => x.OpportunityIds, y => y.MapFrom(z => z.Relations.Where(u => u.OpportunityHost != null).Select(m => m.Id).ToArray()))
                .ForMember(x => x.WeaknessIds, y => y.MapFrom(z => z.Relations.Where(u => u.WeaknessHost != null).Select(m => m.Id).ToArray()))
                .ForMember(x => x.StrengthIds, y => y.MapFrom(z => z.Relations.Where(u => u.StrengthHost != null).Select(m => m.Id).ToArray()))
                .ForMember(x => x.RelationIds, y => y.MapFrom(z => z.Relations.Select(m => m.Id).ToArray()))
                .ForMember(x => x.Category, o => o.MapFrom(y => y.ESCategory.Name))
                .ForMember(x => x.CategoryId, o => o.MapFrom(y => y.ESCategory.Id));
            Mapper.CreateMap<SaveConstraintRequest, Constraint>();
            Mapper.CreateMap<Constraint, SaveConstraintResponse>()
                .ForMember(x => x.CategoryId, o => o.MapFrom(y => y.ESCategory.Id));
            Mapper.CreateMap<SaveChallengeRequest, Challenge>();
            Mapper.CreateMap<Challenge, SaveChallengeResponse>()
                .ForMember(x => x.CategoryId, o => o.MapFrom(y => y.ESCategory.Id));
            Mapper.CreateMap<SaveESCategoryRequest, ESCategory>();
            Mapper.CreateMap<ESCategory, GetESCategoryResponse>();

            Mapper.CreateMap<UltimateObjectivePoint, GetVoyagePlanResponse.UltimateObjectivePoint>();
            Mapper.CreateMap<Challenge, GetVoyagePlanResponse.Challenge>();
            Mapper.CreateMap<Constraint, GetVoyagePlanResponse.Constraint>();
            Mapper.CreateMap<Posture, GetVoyagePlanResponse.Posture>();
            Mapper.CreateMap<DesiredState, GetVoyagePlanResponse.DesiredState>();
            Mapper.CreateMap<PostureChallenge, GetVoyagePlanResponse.PostureChallenge>();
            Mapper.CreateMap<PostureConstraint, GetVoyagePlanResponse.PostureConstraint>();
            Mapper.CreateMap<CalculateOutputResponse.KeyOutputResponse, GetVoyagePlanResponse.KeyOutputResponse>();
            Mapper.CreateMap<MidtermPhaseFormulationStage, GetVoyagePlanResponse.MidtermFormulationStage>();
            Mapper.CreateMap<MidtermPhaseDescription, GetVoyagePlanResponse.MidtermPhaseDescription>();
            Mapper.CreateMap<MidtermPhaseKeyDriver, GetVoyagePlanResponse.MidtermPhaseKeyDriver>();
            Mapper.CreateMap<Constraint, GetConstraintResponse>()
                .ForMember(x => x.ThreatIds, y => y.MapFrom(z => z.Relations.Where(o => o.ThreatHost != null)))
                .ForMember(x => x.Opportunitys, y => y.MapFrom(z => z.Relations.Where(o => o.OpportunityHost != null)))
                .ForMember(x => x.WeaknessIds, y => y.MapFrom(z => z.Relations.Where(o => o.WeaknessHost != null)))
                .ForMember(x => x.StrengthIds, y => y.MapFrom(z => z.Relations.Where(o => o.StrengthHost != null)));
            Mapper.CreateMap<EnvironmentalScanning, GetConstraintResponse.Environmental>();
            Mapper.CreateMap<Challenge, GetChallengeResponse>()
                .ForMember(x => x.ThreatIds, y => y.MapFrom(z => z.Relations.Where(o => o.ThreatHost != null)))
                .ForMember(x => x.Opportunitys, y => y.MapFrom(z => z.Relations.Where(o => o.OpportunityHost != null)))
                .ForMember(x => x.WeaknessIds, y => y.MapFrom(z => z.Relations.Where(o => o.WeaknessHost != null)))
                .ForMember(x => x.StrengthIds, y => y.MapFrom(z => z.Relations.Where(o => o.StrengthHost != null)));
            Mapper.CreateMap<EnvironmentalScanning, GetChallengeResponse.Environmental>();

            Mapper.CreateMap<PostureChallenge, GetPostureChallengeResponse>()
                .ForMember(x => x.DesiredStates, y => y.MapFrom(z => z.DesiredStates.Where(o => o.Posture != null)));
            Mapper.CreateMap<DesiredState, GetPostureChallengeResponse.DesiredState>();

            Mapper.CreateMap<PostureConstraint, GetPostureConstraintResponse>()
                .ForMember(x => x.DesiredStates, y => y.MapFrom(z => z.DesiredStates.Where(o => o.Posture != null)));
            Mapper.CreateMap<DesiredState, GetPostureConstraintResponse.DesiredState>();

            Mapper.CreateMap<Posture, GetMidtermFormulationResponse.Posture>();
            Mapper.CreateMap<DesiredState, GetMidtermFormulationResponse.DesiredState>();
            Mapper.CreateMap<MidtermPhaseFormulationStage, GetMidtermFormulationResponse.MidtermFormulationStage>();
            Mapper.CreateMap<MidtermPhaseDescription, GetMidtermFormulationResponse.MidtermPhaseDescription>();
            Mapper.CreateMap<MidtermPhaseKeyDriver, GetMidtermFormulationResponse.MidtermPhaseKeyDriver>();

            Mapper.CreateMap<AddStageRequest, MidtermPhaseFormulationStage>();
            Mapper.CreateMap<AddDefinitionRequest, MidtermPhaseDescription>();
            Mapper.CreateMap<AddDefinitionRequest, MidtermPhaseKeyDriver>();

            Mapper.CreateMap<MidtermStrategicPlanning, GetMidtermPlanningsResponse.MidtermPlanning>()
                .ForMember(d => d.Kpis, o => o.MapFrom(s => s.Kpis.Select(x => x.Kpi).ToList().MapTo<GetMidtermPlanningsResponse.Kpi>()));
            Mapper.CreateMap<MidtermStrategicPlanningObjective, GetMidtermPlanningsResponse.MidtermPlanningObjective>();
            Mapper.CreateMap<Kpi, GetMidtermPlanningsResponse.Kpi>()
                .ForMember(d => d.Measurement, o => o.MapFrom(s => s.Measurement.Name));
            //Mapper.CreateMap<KpiAchievement, GetMidtermPlanningsResponse.KpiData>()
            //    .ForMember(d => d.KpiId, o => o.MapFrom(s => s.Kpi.Id))
            //    .ForMember(d => d.Year, o => o.MapFrom(s => s.Periode.Year));

            Mapper.CreateMap<AddObjectiveRequest, MidtermStrategicPlanningObjective>();
            Mapper.CreateMap<AddMidtermPlanningRequest, MidtermStrategicPlanning>();

            Mapper.CreateMap<KeyOutputCategory, GetActiveOutputCategoriesResponse.OutputCategoryResponse>()
                .ForMember(x => x.KeyOutputs, o => o.MapFrom(s =>
                    s.KeyOutputs.Where(x => x.IsActive).MapTo<GetActiveOutputCategoriesResponse.KeyOutputResponse>()));
            Mapper.CreateMap<KeyOutputConfiguration, GetActiveOutputCategoriesResponse.KeyOutputResponse>()
                .ForMember(x => x.KeyAssumptions, o => o.Ignore())
                .ForMember(x => x.Kpis, o => o.Ignore())
                .ForMember(x => x.Measurement, o => o.Ignore());


            Mapper.CreateMap<PopDashboard, GetPopDashboardsResponse.PopDashboard>();
            Mapper.CreateMap<PopDashboardAttachment, GetPopDashboardsResponse.Attachment>();
            Mapper.CreateMap<SavePopDashboardRequest, PopDashboard>();
            Mapper.CreateMap<PopDashboard, GetPopDashboardResponse>();
            Mapper.CreateMap<PopDashboardAttachment, GetPopDashboardResponse.Attachment>();
            Mapper.CreateMap<PopInformation, GetPopDashboardResponse.PopInformation>();
            Mapper.CreateMap<Signature, GetPopDashboardResponse.Signature>()
                .ForMember(x => x.User, o => o.MapFrom(y => y.User.Username))
                .ForMember(x => x.UserId, o => o.MapFrom(y => y.User.Id))
                .ForMember(x => x.SignatureImage, o => o.MapFrom(y => y.User.SignatureImage));
            Mapper.CreateMap<SavePopInformationRequest, PopInformation>();
            Mapper.CreateMap<SaveSignatureRequest, DSLNG.PEAR.Data.Entities.Pop.Signature>()
                .ForMember(x => x.Type, o => o.MapFrom(y => y.TypeSignature));


            Mapper.CreateMap<ApproveSignatureRequest, Signature>();
            Mapper.CreateMap<Wave, GetWavesResponse.WaveResponse>()
                .ForMember(x => x.Value, y => y.MapFrom(z => z.Value.Text));
            Mapper.CreateMap<SaveWaveRequest, Wave>();
            Mapper.CreateMap<SaveWaveRequest, BaseAction>();


            Mapper.CreateMap<MirConfiguration, GetsMirConfigurationsResponse.MirConfiguration>();
            Mapper.CreateMap<SaveMirConfigurationRequest, MirConfiguration>();
            Mapper.CreateMap<MirConfiguration, GetMirConfigurationsResponse>();
            Mapper.CreateMap<MirDataTable, GetMirConfigurationsResponse.MirDataTable>()
                .ForMember(x => x.KpiIds, o => o.MapFrom(s => s.Kpis.Select(y => y.Id).ToArray()));
            Mapper.CreateMap<Kpi, GetMirConfigurationsResponse.MirDataTable.Kpi>();
            Mapper.CreateMap<SaveMirDataTableRequest, MirDataTable>();

            Mapper.CreateMap<DerLayoutItem, GetDerLayoutItemsResponse.DerLayoutItem>();
            Mapper.CreateMap<DerHighlight, GetDerLayoutItemsResponse.DerHighlight>()
                .ForMember(x => x.Value, o => o.MapFrom(s => s.SelectOption.Value))
                .ForMember(x => x.Text, o => o.MapFrom(s => s.SelectOption.Text))
                .ForMember(x => x.HighlightTypeId, o => o.MapFrom(s => s.SelectOption.Id));
            Mapper.CreateMap<DerKpiInformation, GetDerLayoutItemsResponse.KpiInformation>()
                .ForMember(x => x.KpiId, o => o.MapFrom(s => s.Kpi.Id));
            Mapper.CreateMap<KpiTransformation, GetKpiTransformationsResponse.KpiTransformationResponse>();
            Mapper.CreateMap<DSLNG.PEAR.Data.Entities.RoleGroup, GetKpiTransformationsResponse.RoleGroupResponse>();
            Mapper.CreateMap<KpiTransformation, GetKpiTransformationResponse>();
            Mapper.CreateMap<DSLNG.PEAR.Data.Entities.RoleGroup, GetKpiTransformationResponse.RoleGroupResponse>();
            Mapper.CreateMap<Kpi, GetKpiTransformationResponse.KpiResponse>()
                .ForMember(x => x.Name, o => o.MapFrom(s => s.Name + " (" + s.Measurement.Name + ")"));
            Mapper.CreateMap<SaveKpiTransformationScheduleRequest, KpiTransformationSchedule>();
            Mapper.CreateMap<KpiTransformationSchedule, SaveKpiTransformationScheduleResponse>()
                .ForMember(x => x.KpiTransformationId, o => o.MapFrom(s => s.KpiTransformation.Id))
                .ForMember(x => x.PeriodeType, o => o.MapFrom(s => s.KpiTransformation.PeriodeType));
            Mapper.CreateMap<Kpi, SaveKpiTransformationScheduleResponse.KpiResponse>()
                .ForMember(x => x.MethodId, o => o.MapFrom(s => s.Method.Id));
            Mapper.CreateMap<KpiTransformationSchedule, GetKpiTransformationSchedulesResponse.KpiTransformationScheduleResponse>()
                .ForMember(x => x.PeriodeType, o => o.MapFrom(s => s.KpiTransformation.PeriodeType));
            Mapper.CreateMap<KpiTransformationLog, GetKpiTransformationLogsResponse.KpiTransformationLogResponse>()
                .ForMember(x => x.KpiName, o => o.MapFrom(s => s.Kpi.Name))
                .ForMember(x => x.KpiMeasurement, o => o.MapFrom(s => s.Kpi.Measurement.Name))
                .ForMember(x => x.PeriodeType, o => o.MapFrom(s => s.Schedule.KpiTransformation.PeriodeType));
            Mapper.CreateMap<SaveKpiTransformationLogRequest, KpiTransformationLog>();
            Mapper.CreateMap<VesselSchedule, SaveVesselScheduleResponse>()
                .ForMember(x => x.VesselId, o => o.MapFrom(s => s.Vessel.Id))
                .ForMember(x => x.VesselCapacity, o => o.MapFrom(s => s.Vessel.Capacity))
                .ForMember(x => x.VesselType, o => o.MapFrom(s => s.Vessel.Type))
                .ForMember(x => x.VesselName, o => o.MapFrom(s => s.Vessel.Name))
                .ForMember(x => x.VesselMeasuremant, o => o.MapFrom(s => s.Vessel.Measurement.Name))
                .ForMember(x => x.BuyerId, o => o.MapFrom(s => s.Buyer.Id))
                .ForMember(x => x.BuyerName, o => o.MapFrom(s => s.Buyer.Name))
                .ForMember(x => x.ETA, o => o.MapFrom(s => s.ETA.HasValue ? s.ETA.Value.ToString("dd-MM-yyyy") : "TBD"))
                .ForMember(x => x.ETD, o => o.MapFrom(s => s.ETD.HasValue ? s.ETD.Value.ToString("dd-MM-yyyy") : "TBD"));
            base.Configure();
        }

        private void ConfigureFileRepository()
        {
            Mapper.CreateMap<FileRepository, GetFileRepositoryResponse>();
            Mapper.CreateMap<FileRepository, GetFilesRepositoryResponse.FileRepository>();
            Mapper.CreateMap<SaveFileRepositoryRequest, FileRepository>();
        }

        private void ConfigureProcessBlueprint()
        {
            Mapper.CreateMap<ProcessBlueprint, GetProcessBlueprintResponse>()
                .ForMember(x => x.CreatedBy, y => y.MapFrom(z => z.CreatedBy.Id));
            Mapper.CreateMap<ProcessBlueprint, GetProcessBlueprintsResponse.ProcessBlueprint>()
                .ForMember(x => x.CreatedBy, y => y.MapFrom(z => z.CreatedBy.Id));
            Mapper.CreateMap<SaveProcessBlueprintRequest, ProcessBlueprint>();
            Mapper.CreateMap<FileManagerRolePrivilege, GetProcessBlueprintPrivilegesResponse.FileManagerRolePrivilege>()
                .ForMember(x => x.FileId, y => y.MapFrom(z => z.ProcessBlueprint.Id))
                .ForMember(x => x.RoleGroupId, y => y.MapFrom(z => z.RoleGroup.Id))
                .ForMember(x => x.RoleGroupName, y => y.MapFrom(z => z.RoleGroup.Name));
            Mapper.CreateMap<ProcessBlueprint, GetProcessBlueprintPrivilegesResponse.FileManagerRolePrivilege.BlueprintFile>();
            Mapper.CreateMap<UpdateFilePrivilegeRequest, FileManagerRolePrivilege>();
            Mapper.CreateMap<FilePrivilegeRequest, FileManagerRolePrivilege>();

            Mapper.CreateMap<DerInputFile, GetDerInputFilesResponse.DerInputFile>()
                .ForMember(x => x.CreatedBy, y => y.MapFrom(z => z.CreatedBy.Username));
        }

        private void ConfigureEconomicSummary()
        {
            Mapper.CreateMap<EconomicSummary, GetEconomicSummaryResponse>();
            Mapper.CreateMap<Scenario, GetEconomicSummaryResponse.Scenario>();
            Mapper.CreateMap<EconomicSummary, GetEconomicSummariesResponse.EconomicSummary>()
                .ForMember(x => x.Scenarios, y => y.MapFrom(z => string.Join(", ", z.Scenarios.Select(x => x.Name))));
            Mapper.CreateMap<SaveEconomicSummaryRequest, EconomicSummary>()
                .ForMember(x => x.Scenarios, y => y.Ignore());
            Mapper.CreateMap<Scenario, GetEconomicConfigSelectListResponse.Scenario>();
            Mapper.CreateMap<EconomicSummary, GetEconomicConfigSelectListResponse.EconomicSummary>();
            Mapper.CreateMap<SaveEconomicSummaryRequest.Scenario, Scenario>();

        }

        private void ConfigureSelects()
        {
            Mapper.CreateMap<Select, GetSelectsResponse.Select>()
                  .ForMember(x => x.Options, y => y.MapFrom(z => string.Join(", ", z.Options.Select(opt => opt.Text))))
                  .ForMember(x => x.Parent, o => o.MapFrom(s => s.Parent.Name))
                  .ForMember(x => x.ParentOption, o => o.MapFrom(s => s.ParentOption.Text));
            Mapper.CreateMap<CreateSelectRequest, Select>();
            Mapper.CreateMap<CreateSelectRequest.SelectOption, SelectOption>();
            Mapper.CreateMap<Select, GetSelectResponse>();
            Mapper.CreateMap<SelectOption, GetSelectResponse.SelectOptionResponse>();
            Mapper.CreateMap<UpdateSelectRequest, Select>();
            //.ForMember(x => x.Options, x => x.Ignore());
            Mapper.CreateMap<UpdateSelectRequest.SelectOption, SelectOption>();
        }

        private void ConfigurePmsSummary()
        {
            //common 
            Mapper.CreateMap<Common.PmsSummary.ScoreIndicator, ScoreIndicator>();
            Mapper.CreateMap<ScoreIndicator, Common.PmsSummary.ScoreIndicator>();

            //pms summary
            Mapper.CreateMap<PmsSummary, GetPmsSummaryResponse>();
            Mapper.CreateMap<UpdatePmsSummaryRequest, PmsSummary>();

            Mapper.CreateMap<PmsSummary, GetPmsSummaryListResponse.PmsSummary>();
            Mapper.CreateMap<Kpi, GetPmsSummaryConfigurationResponse.Kpi>()
                  .ForMember(x => x.Measurement, y => y.MapFrom(z => z.Measurement.Name));
            Mapper.CreateMap<Data.Entities.Pillar, GetPmsSummaryConfigurationResponse.Pillar>();
            Mapper.CreateMap<PmsConfig, GetPmsSummaryConfigurationResponse.PmsConfig>()
                .ForMember(x => x.PillarId, y => y.MapFrom(z => z.Pillar.Id))
                .ForMember(x => x.PillarName, y => y.MapFrom(z => z.Pillar.Name))
                .ForMember(x => x.PmsConfigDetailsList, y => y.MapFrom(z => z.PmsConfigDetailsList));
            Mapper.CreateMap<PmsConfigDetails, GetPmsSummaryConfigurationResponse.PmsConfigDetails>();



            Mapper.CreateMap<CreateKpiTargetsRequest.KpiTarget, KpiTarget>();
            Mapper.CreateMap<CreateKpiTargetRequest, KpiTarget>();
            Mapper.CreateMap<UpdateKpiTargetItemRequest, KpiTarget>()
                .ForMember(x => x.Value, y => y.MapFrom(z => z.RealValue));
            Mapper.CreateMap<Kpi, GetKpisByPillarIdResponse.Kpi>();
            Mapper.CreateMap<CreatePmsConfigRequest, PmsConfig>()
                .ForMember(x => x.ScoringType, y => y.MapFrom(z => Enum.Parse(typeof(ScoringType), z.ScoringType)));

            Mapper.CreateMap<CreatePmsSummaryRequest, PmsSummary>();

            ConfigurePmsConfig();
        }

        private void ConfigureKpi()
        {
            Mapper.CreateMap<Data.Entities.Group, GetKpisResponse.Group>();
            Mapper.CreateMap<Data.Entities.Measurement, GetKpisResponse.Measurement>();
            Mapper.CreateMap<Data.Entities.Pillar, GetKpisResponse.Pillar>();
            Mapper.CreateMap<Data.Entities.Level, DSLNG.PEAR.Services.Responses.Kpi.Level>();
            Mapper.CreateMap<Data.Entities.RoleGroup, DSLNG.PEAR.Services.Responses.Kpi.RoleGroup>();
            Mapper.CreateMap<Data.Entities.Group, DSLNG.PEAR.Services.Responses.Kpi.Group>();
            Mapper.CreateMap<Data.Entities.Type, DSLNG.PEAR.Services.Responses.Kpi.Type>();
            Mapper.CreateMap<Data.Entities.Measurement, DSLNG.PEAR.Services.Responses.Kpi.Measurement>();
            Mapper.CreateMap<Kpi, GetKpiResponse>()
                .ForMember(k => k.Periode, o => o.MapFrom(k => k.Period));
            Mapper.CreateMap<UpdateKpiRequest, Kpi>()
               .ForMember(k => k.Level, o => o.Ignore())
               .ForMember(k => k.Group, o => o.Ignore())
               .ForMember(k => k.RoleGroup, o => o.Ignore())
               .ForMember(k => k.Measurement, o => o.Ignore())
               .ForMember(k => k.Type, o => o.Ignore())
               .ForMember(k => k.Period, o => o.MapFrom(k => k.Periode));
        }

        private void ConfigureKpiTarget()
        {
            Mapper.CreateMap<Data.Entities.Kpi, GetPmsConfigsResponse.Kpi>();
            Mapper.CreateMap<Data.Entities.Measurement, GetPmsConfigsResponse.Measurement>();
            Mapper.CreateMap<Data.Entities.PmsConfigDetails, GetPmsConfigsResponse.PmsConfigDetails>();
            Mapper.CreateMap<Data.Entities.Pillar, GetPmsConfigsResponse.Pillar>();
            Mapper.CreateMap<Data.Entities.PmsSummary, GetPmsConfigsResponse.PmsSummary>();
            Mapper.CreateMap<Data.Entities.PmsConfig, GetPmsConfigsResponse.PmsConfig>();

            Mapper.CreateMap<Kpi, AllKpiTargetsResponse.Kpi>()
                  .ForMember(x => x.Type, y => y.MapFrom(z => z.Type.Name))
                  .ForMember(x => x.Measurement, y => y.MapFrom(z => z.Measurement.Name));

            Mapper.CreateMap<Kpi, GetKpiTargetsConfigurationResponse.Kpi>()
                .ForMember(x => x.KpiTargets, y => y.Ignore())
                .ForMember(x => x.Measurement, y => y.MapFrom(z => z.Measurement.Name));

            Mapper.CreateMap<KpiTarget, GetKpiTargetsConfigurationResponse.KpiTarget>();
            Mapper.CreateMap<KpiTarget, GetKpiTargetItemResponse>();
            Mapper.CreateMap<Kpi, GetKpiTargetItemResponse.KpiResponse>();
            Mapper.CreateMap<SaveKpiTargetRequest, KpiTarget>()
                .ForMember(x => x.Value, y => y.MapFrom(z => z.RealValue));
            Mapper.CreateMap<SaveKpiTargetRequest, BaseAction>();
        }

        private void ConfigurePmsConfig()
        {
            Mapper.CreateMap<CreatePmsConfigRequest, PmsConfig>();
            Mapper.CreateMap<PmsConfig, GetPmsConfigResponse>()
                .ForMember(x => x.PillarName, y => y.MapFrom(z => z.Pillar.Name));
            Mapper.CreateMap<UpdatePmsConfigRequest, PmsConfig>();
        }

        private void ConfigurePmsConfigDetails()
        {
            Mapper.CreateMap<CreatePmsConfigDetailsRequest, PmsConfigDetails>()
                  .ForMember(x => x.ScoringType, y => y.MapFrom(z => Enum.Parse(typeof(ScoringType), z.ScoringType)));

            Mapper.CreateMap<PmsConfigDetails, GetPmsConfigDetailsResponse>()
                  .ForMember(x => x.KpiId, y => y.MapFrom(z => z.Kpi.Id))
                  .ForMember(x => x.PillarId, y => y.MapFrom(z => z.Kpi.Pillar.Id))
                  .ForMember(x => x.KpiName, y => y.MapFrom(z => z.Kpi.Name))
                  .ForMember(x => x.PillarName, y => y.MapFrom(z => z.Kpi.Pillar.Name));

            Mapper.CreateMap<UpdatePmsConfigDetailsRequest, PmsConfigDetails>()
                .ForMember(x => x.ScoringType, y => y.MapFrom(z => Enum.Parse(typeof(ScoringType), z.ScoringType)))
                .ForMember(x => x.TargetType, y => y.Ignore())
                .ForMember(x => x.Target, y => y.MapFrom(z => z.Target));
        }

        private void ConfigureKpiAchievements()
        {
            Mapper.CreateMap<Kpi, AllKpiAchievementsResponse.Kpi>()
                  .ForMember(x => x.Type, y => y.MapFrom(z => z.Type.Name))
                  .ForMember(x => x.Measurement, y => y.MapFrom(z => z.Measurement.Name));

            Mapper.CreateMap<Kpi, GetKpiAchievementsConfigurationResponse.Kpi>()
                .ForMember(x => x.KpiAchievements, y => y.Ignore())
                .ForMember(x => x.Measurement, y => y.MapFrom(z => z.Measurement.Name));

            Mapper.CreateMap<KpiAchievement, GetKpiAchievementsConfigurationResponse.KpiAchievement>();
            Mapper.CreateMap<KpiAchievement, GetKpiAchievementResponse>();
                
            Mapper.CreateMap<Kpi, GetKpiAchievementResponse.KpiResponse>()
                .ForMember(x => x.KpiMeasurement, y => y.MapFrom(z => z.Measurement != null ? z.Measurement.Name : string.Empty));

            Mapper.CreateMap<KpiAchievement, GetAchievementsResponse>();
            Mapper.CreateMap<Kpi, GetAchievementsResponse>();
        }

        private void ConfigureKeyOperation()
        {
            Mapper.CreateMap<Kpi, GetOperationDataConfigurationResponse.Kpi>()
                  .ForMember(x => x.MeasurementName, y => y.MapFrom(z => z.Measurement.Name));
            Mapper.CreateMap<KeyOperationData, GetOperationDataConfigurationResponse.OperationData>()
                  .ForMember(x => x.ScenarioId, y => y.MapFrom(z => z.Scenario.Id))
                  .ForMember(x => x.KeyOperationConfigId, y => y.MapFrom(z => z.KeyOperationConfig.Id));
            //Mapper.CreateMap<UpdateOperationDataRequest, KeyOperationData>()
            //      .ForMember(x => x.PeriodeType, y => y.MapFrom(z => (PeriodeType)Enum.Parse(typeof (PeriodeType), z.PeriodeType)));
            Mapper.CreateMap<UpdateOperationDataRequest, KeyOperationData>()
                  .ForMember(x => x.PeriodeType, y => y.MapFrom(z => z.PeriodeType))
                  .ForMember(x => x.Kpi, y => y.Ignore())
                  .ForMember(x => x.Scenario, y => y.Ignore())
                  .ForMember(x => x.KeyOperationConfig, y => y.Ignore())
                  .ForMember(x => x.Value, y => y.MapFrom(z => z.RealValue));
        }

        private void ConfigureDer()
        {
            Mapper.CreateMap<Der, GetDersResponse.Der>()
                .ForMember(x => x.GenerateBy, o => o.MapFrom(s => s.GenerateBy.FullName))
                .ForMember(x => x.RevisionBy, o => o.MapFrom(s => s.RevisionBy.FullName));
            Mapper.CreateMap<Der, GetDerResponse>();
            Mapper.CreateMap<DerLayout, GetActiveDerResponse>();
            Mapper.CreateMap<DerLayoutItem, GetActiveDerResponse.DerItem>();
            Mapper.CreateMap<DerItem, GetDerItemResponse>();
            Mapper.CreateMap<GetDerItemRequest, GetDerItemResponse>();
            Mapper.CreateMap<DerLayoutItem, GetDerLayoutitemResponse>()
                  .ForMember(x => x.DerLayoutId, y => y.MapFrom(z => z.DerLayout.Id))
                  .ForMember(x => x.SignedBy, y => y.MapFrom(z => z.SignedBy.Id));

            Mapper.CreateMap<DerLayout, GetDerLayoutResponse>();
            Mapper.CreateMap<DerLayoutItem, GetDerLayoutResponse.DerLayoutItem>();

            Mapper.CreateMap<DerLayoutItem, GetDerLayoutitemResponse>();
            Mapper.CreateMap<DerArtifact, GetDerLayoutitemResponse.DerArtifact>()
            .ForMember(x => x.MeasurementId, y => y.MapFrom(z => z.Measurement.Id))
            .ForMember(x => x.MeasurementName, y => y.MapFrom(z => z.Measurement.Name));
            Mapper.CreateMap<DerArtifactSerie, GetDerLayoutitemResponse.DerArtifactSerie>()
                .ForMember(x => x.KpiId, y => y.MapFrom(z => z.Kpi.Id))
                .ForMember(x => x.KpiName, y => y.MapFrom(z => z.Kpi.Name + " (" + z.Kpi.Measurement.Name + ")"));
            Mapper.CreateMap<DerArtifactChart, GetDerLayoutitemResponse.DerArtifactChart>()
                  .ForMember(x => x.MeasurementId, y => y.MapFrom(z => z.Measurement.Id));
            Mapper.CreateMap<DerArtifactPlot, GetDerLayoutitemResponse.DerArtifactPlot>();
            Mapper.CreateMap<Kpi, GetDerLayoutitemResponse.KpiResponse>()
                .ForMember(x => x.MeasurementName, o => o.MapFrom(s => s.Measurement.Name));
            Mapper.CreateMap<SaveLayoutItemRequest, DerLayoutItem>()
                .ForMember(x => x.Artifact, y => y.Ignore())
                .ForMember(x => x.SignedBy, o => o.Ignore());
            Mapper.CreateMap<SaveLayoutItemRequest, DerArtifact>()
                .ForMember(x => x.GraphicType, y => y.MapFrom(z => z.Type))
                .ForMember(x => x.HeaderTitle, y => y.MapFrom(z => z.Artifact.HeaderTitle));
            Mapper.CreateMap<SaveLayoutItemRequest.LayoutItemArtifact, DerArtifact>();

            Mapper.CreateMap<SaveLayoutItemRequest.LayoutItemArtifactChart, DerArtifactChart>()
                .ForMember(x => x.Series, y => y.Ignore());
            Mapper.CreateMap<SaveLayoutItemRequest.LayoutItemArtifactSerie, DerArtifactSerie>();
            Mapper.CreateMap<SaveLayoutItemRequest.LayoutItemPlotBand, DerArtifactPlot>();

            //tank
            Mapper.CreateMap<SaveLayoutItemRequest.LayoutItemArtifactTank, DerArtifactTank>();
            Mapper.CreateMap<DerArtifactTank, GetDerLayoutitemResponse.DerArtifactTank>()
                .ForMember(x => x.VolumeInventoryId, o => o.MapFrom(s => s.VolumeInventory.Id))
               .ForMember(x => x.VolumeInventory, o => o.MapFrom(s => s.VolumeInventory.Name + " (" + s.VolumeInventory.Measurement.Name + ")"))
               .ForMember(x => x.DaysToTankTopId, o => o.MapFrom(s => s.DaysToTankTop.Id))
               .ForMember(x => x.DaysToTankTop, o => o.MapFrom(s => s.DaysToTankTop.Name + " (" + s.DaysToTankTop.Measurement.Name + ")")); ;

            //speedometer
            //Mapper.CreateMap<SaveLayoutItemRequest, DerA>();
            //Mapper.CreateMap<DerArtifactTank, GetDerLayoutitemResponse.DerArtifactTank>()
            //    .ForMember(x => x.VolumeInventoryId, o => o.MapFrom(s => s.VolumeInventory.Id))
            //   .ForMember(x => x.VolumeInventory, o => o.MapFrom(s => s.VolumeInventory.Name + " (" + s.VolumeInventory.Measurement.Name + ")"))
            //   .ForMember(x => x.DaysToTankTopId, o => o.MapFrom(s => s.DaysToTankTop.Id))
            //   .ForMember(x => x.DaysToTankTop, o => o.MapFrom(s => s.DaysToTankTop.Name + " (" + s.DaysToTankTop.Measurement.Name + ")")); ;


            //highlight
            Mapper.CreateMap<DerHighlight, GetDerLayoutitemResponse.DerHighlight>()
                  .ForMember(x => x.SelectOptionId, y => y.MapFrom(z => z.SelectOption.Id));

            Mapper.CreateMap<DerKpiInformation, GetDerLayoutitemResponse.KpiInformationResponse>();
            Mapper.CreateMap<Kpi, GetDerLayoutitemResponse.KpiInformationResponse.KpiResponse>()
                .ForMember(x => x.MeasurementName, y => y.MapFrom(z => z.Measurement.Name));
            Mapper.CreateMap<SelectOption, GetDerLayoutitemResponse.KpiInformationResponse.SelectOptionResponse>();

            Mapper.CreateMap<SaveLayoutItemRequest.DerKpiInformationRequest, DerKpiInformation>()
                .ForMember(x => x.KpiMeasurement, y => y.MapFrom(z => z.KpiMeasurement))
                .ForMember(x => x.KpiLabel, y => y.MapFrom(z => z.KpiLabel))
                .ForMember(x => x.Position, y => y.MapFrom(z => z.Position))
                .ForMember(x => x.Kpi, y => y.Ignore());



            //DER Original Data
            Mapper.CreateMap<DerOriginalData, GetOriginalDataResponse.OriginalDataResponse>()
                  .ForMember(x => x.LayoutItemId, y => y.MapFrom(z => z.LayoutItem.Id));
            Mapper.CreateMap<SaveOriginalDataRequest, DerOriginalData>();
            Mapper.CreateMap<SaveOriginalDataRequest.OriginalDataRequest, DerOriginalData>()
                  .ForMember(x => x.LayoutItem, y => y.Ignore())
                  .ForMember(x => x.Id, y => y.Ignore());
            Mapper.CreateMap<GetKpiTargetItemResponse, GetKpiValueResponse>();
            Mapper.CreateMap<GetKpiTargetItemResponse.KpiResponse, GetKpiValueResponse.KpiResponse>();

            Mapper.CreateMap<GetKpiAchievementResponse, GetKpiValueResponse>();
            Mapper.CreateMap<GetKpiAchievementResponse.KpiResponse, GetKpiValueResponse.KpiResponse>();

            Mapper.CreateMap<Wave, GetWaveResponse>()
                .ForMember(x => x.Value, o => o.MapFrom(s => s.Value.Value))
                .ForMember(x => x.Text, o => o.MapFrom(s => s.Value.Text));
        }

        private void ConfigureInputData()
        {
            Mapper.CreateMap<SaveOrUpdateInputDataRequest, InputData>()
                .ForMember(x => x.PeriodeType, y => y.MapFrom(z => Enum.Parse(typeof(PeriodeType), z.PeriodeType)))
                .ForMember(x => x.GroupInputDatas, y => y.Ignore());
            Mapper.CreateMap<InputData, GetInputDataResponse>()
                .ForMember(x => x.PeriodeType, y => y.MapFrom(z => z.PeriodeType.ToString()))
                .ForMember(x => x.AccountabilityId, y => y.MapFrom(z => z.Accountability.Id));

            Mapper.CreateMap<GroupInputData, GetInputDataResponse.GroupInputData>();
            Mapper.CreateMap<InputDataKpiAndOrder, GetInputDataResponse.InputDataKpiAndOrder>()
                .ForMember(x => x.KpiId, y => y.MapFrom(z => z.Kpi.Id))
                .ForMember(x => x.KpiName, y => y.MapFrom(z => z.Kpi.Name))
                .ForMember(x => x.KpiMeasurement, y => y.MapFrom(z => z.Kpi.Measurement.Name))
                .ForMember(x => x.Order, y => y.MapFrom(z => z.Order));

            Mapper.CreateMap<InputData, GetInputDatasResponse.InputData>()
                .ForMember(x => x.Accountability, y => y.MapFrom(z => z.Accountability.Name))
                .ForMember(x => x.PeriodeType, y => y.MapFrom(z => z.PeriodeType.ToString()));

            //Mapper.CreateMap<SaveOrUpdateInputDataRequest.GroupInputData, GroupInputData>();
            //Mapper.CreateMap<SaveOrUpdateInputDataRequest.InputDataKpiAndOrder, InputDataKpiAndOrder>();

            //Mapper.CreateMap<SaveOrUpdateInputDataRequest.GroupInput, GroupInputData>();
            //Mapper.CreateMap<SaveOrUpdateInputDataRequest.Kpi, Kpi>()
            //    .ForMember(x => x.Id, y => y.MapFrom(z => z.Id))
            //    .ForMember(x => x.Order, y => y.Ignore())
            //    .ForMember(x => x.Type, y => y.Ignore());
            //Mapper.CreateMap<SaveOrUpdateInputDataRequest.GroupInput, InputDataKpiAndOrder>()
            //    .ForMember(x => x.Order, y => y.MapFrom(z => z.Kpis.));
            //Mapper.CreateMap<SaveOrUpdateInputDataRequest.Kpi, Kpi>()
            //    .for;
            //Mapper.CreateMap<SaveOrUpdateInputDataRequest.Kpi, Kpi>()
            //    .ForMember(x => x.Type, y => y.Ignore());


        }
    }
}

