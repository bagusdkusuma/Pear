using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using DSLNG.PEAR.Data.Entities;
using Type = DSLNG.PEAR.Data.Entities.Type;

namespace DSLNG.PEAR.Data.Persistence
{
    public interface IDataContext
    {
        IDbSet<Activity> Activities { get; set; }
        IDbSet<Artifact> Artifacts { get; set; }
        IDbSet<ArtifactSerie> ArtifactSeries { get; set; }
        IDbSet<ArtifactStack> ArtifactStacks { get; set; }
        IDbSet<ArtifactPlot> ArtifactPlots { get; set; }
        IDbSet<ArtifactRow> ArtifactRows { get; set; }
        IDbSet<ArtifactChart> ArtifactCharts { get; set; }
        IDbSet<Conversion> Conversions { get; set; }
        IDbSet<DashboardTemplate> DashboardTemplates { get; set; }
        IDbSet<Group> Groups { get; set; }
        IDbSet<Kpi> Kpis { get; set; }
        IDbSet<KpiAchievement> KpiAchievements { get; set; }
        IDbSet<KpiTarget> KpiTargets { get; set; }
        IDbSet<LayoutColumn> LayoutColumns { get; set; }
        IDbSet<LayoutRow> LayoutRows { get; set; }
        IDbSet<Level> Levels { get; set; }
        IDbSet<Measurement> Measurements { get; set; }
        IDbSet<Menu> Menus { get; set; }
        IDbSet<Method> Methods { get; set; }
        IDbSet<Periode> Periodes { get; set; }
        IDbSet<Pillar> Pillars { get; set; }
        IDbSet<PmsConfig> PmsConfigs { get; set; }
        IDbSet<PmsConfigDetails> PmsConfigDetails { get; set; }
        IDbSet<PmsSummary> PmsSummaries { get; set; }
        IDbSet<RoleGroup> RoleGroups { get; set; }
        IDbSet<ScoreIndicator> ScoreIndicators { get; set; }
        IDbSet<Type> Types { get; set; }
        IDbSet<User> Users { get; set; }
        IDbSet<KpiRelationModel> KpiRelationModels { get; set; }
        IDbSet<ArtifactTank> ArtifactTanks { get; set; }
        IDbSet<Highlight> Highlights { get; set; }
        Database Database { get; }
        int SaveChanges();
        DbEntityEntry Entry(object entity);
    }
}
