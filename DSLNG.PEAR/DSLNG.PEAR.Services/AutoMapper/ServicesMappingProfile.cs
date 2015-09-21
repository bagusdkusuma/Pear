using System;
using AutoMapper;
using DSLNG.PEAR.Data.Entities;
using DSLNG.PEAR.Data.Enums;
using DSLNG.PEAR.Services.Requests.Measurement;
using DSLNG.PEAR.Services.Requests.PmsSummary;
using DSLNG.PEAR.Services.Requests.Select;
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

            Mapper.CreateMap<User, GetUsersResponse.User>();
            //.ForMember(x => x.RoleName, o => o.MapFrom(m => m.Role.Name));
            Mapper.CreateMap<CreateUserRequest, User>();
            Mapper.CreateMap<UpdateUserRequest, User>();
            Mapper.CreateMap<GetUserRequest, User>();
            Mapper.CreateMap<Data.Entities.RoleGroup, GetUserResponse.RoleGroup>();
            Mapper.CreateMap<Data.Entities.RoleGroup, GetUsersResponse.RoleGroup>();
            Mapper.CreateMap<Data.Entities.User, GetUserResponse>();
            Mapper.CreateMap<Data.Entities.User, LoginUserResponse>();
            Mapper.CreateMap<Data.Entities.RoleGroup, LoginUserResponse.RoleGroup>();
            //Mapper.CreateMap<

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
            Mapper.CreateMap<UpdateKpiAchievementItemRequest, KpiAchievement>();
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

            Mapper.CreateMap<Artifact, GetArtifactsResponse.Artifact>();
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
                .ForMember(d => d.ArtifactName, o => o.MapFrom(s => s.Artifact.GraphicName));
            Mapper.CreateMap<GetMultiaxisChartDataRequest, GetCartesianChartDataRequest>();
            Mapper.CreateMap<GetMultiaxisChartDataRequest.ChartRequest, GetCartesianChartDataRequest>();
            Mapper.CreateMap<GetMultiaxisChartDataRequest.ChartRequest.SeriesRequest, GetCartesianChartDataRequest.SeriesRequest>();
            Mapper.CreateMap<GetMultiaxisChartDataRequest.ChartRequest.StackRequest, GetCartesianChartDataRequest.StackRequest>();
            Mapper.CreateMap<GetCartesianChartDataResponse, GetMultiaxisChartDataResponse.ChartResponse>();
            Mapper.CreateMap<GetCartesianChartDataResponse.SeriesResponse, GetMultiaxisChartDataResponse.ChartResponse.SeriesViewModel>();
            Mapper.CreateMap<GetArtifactResponse, GetMultiaxisChartDataRequest>();
            Mapper.CreateMap<GetArtifactResponse.ChartResponse, GetMultiaxisChartDataRequest.ChartRequest>();
            Mapper.CreateMap<GetArtifactResponse.SeriesResponse, GetMultiaxisChartDataRequest.ChartRequest.SeriesRequest>();
            Mapper.CreateMap<GetArtifactResponse.StackResponse, GetMultiaxisChartDataRequest.ChartRequest.StackRequest>();

            Mapper.CreateMap<GetComboChartDataRequest, GetCartesianChartDataRequest>();
            Mapper.CreateMap<GetComboChartDataRequest.ChartRequest, GetCartesianChartDataRequest>();
            Mapper.CreateMap<GetComboChartDataRequest.ChartRequest.SeriesRequest, GetCartesianChartDataRequest.SeriesRequest>();
            Mapper.CreateMap<GetComboChartDataRequest.ChartRequest.StackRequest, GetCartesianChartDataRequest.StackRequest>();
            Mapper.CreateMap<GetCartesianChartDataResponse, GetComboChartDataResponse.ChartResponse>();
            Mapper.CreateMap<GetCartesianChartDataResponse.SeriesResponse, GetComboChartDataResponse.ChartResponse.SeriesViewModel>();
            Mapper.CreateMap<GetArtifactResponse, GetComboChartDataRequest>();
            Mapper.CreateMap<GetArtifactResponse.ChartResponse, GetComboChartDataRequest.ChartRequest>();
            Mapper.CreateMap<GetArtifactResponse.SeriesResponse, GetComboChartDataRequest.ChartRequest.SeriesRequest>();
            Mapper.CreateMap<GetArtifactResponse.StackResponse, GetComboChartDataRequest.ChartRequest.StackRequest>();

            Mapper.CreateMap<Kpi, GetConfigurationResponse.Kpi>();
            Mapper.CreateMap<KpiAchievement, GetConfigurationResponse.KpiAchievement>();
            Mapper.CreateMap<KpiTarget, GetConfigurationResponse.KpiTarget>();
            Mapper.CreateMap<Economic, GetConfigurationResponse.Economic>();
            Mapper.CreateMap<Highlight, GetHighlightsResponse.HighlightResponse>();
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

            Mapper.CreateMap<SaveVesselScheduleRequest, VesselSchedule>();
            Mapper.CreateMap<VesselSchedule, GetVesselSchedulesResponse.VesselScheduleResponse>()
                .ForMember(x => x.Vessel, o => o.MapFrom(s => s.Vessel.Name))
                .ForMember(x => x.Buyer, o => o.MapFrom(s => s.Buyer.Name))
                .ForMember(x => x.Name, o => o.MapFrom(s => s.Vessel.Name));
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

            base.Configure();
        }

        private void ConfigureSelects()
        {
            Mapper.CreateMap<Select, GetSelectsResponse.Select>()
                  .ForMember(x => x.Options, y => y.MapFrom(z => string.Join(", ", z.Options.Select(opt => opt.Text))));
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
            Mapper.CreateMap<UpdateKpiTargetItemRequest, KpiTarget>();
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
            Mapper.CreateMap<Kpi, GetKpiTargetItemResponse.Kpi>();
            Mapper.CreateMap<SaveKpiTargetRequest, KpiTarget>();
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
                .ForMember(x => x.ScoringType, y => y.MapFrom(z => Enum.Parse(typeof(ScoringType), z.ScoringType)));
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
            Mapper.CreateMap<Kpi, GetKpiAchievementResponse.Kpi>();

            Mapper.CreateMap<KpiAchievement, GetAchievementsResponse>();
            Mapper.CreateMap<Kpi, GetAchievementsResponse>();
        }
    }
}

