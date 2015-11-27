using System;
using DSLNG.PEAR.Data.Entities;
using DSLNG.PEAR.Data.Enums;
using DSLNG.PEAR.Data.Persistence;
using System.Data.Entity.Migrations;
using System.Data.Entity;
using Type = DSLNG.PEAR.Data.Entities.Type;

namespace DSLNG.PEAR.Data.Installer
{
    public class DataInitializer : DropCreateDatabaseIfModelChanges<DataContext> 
    //public class DataInitializer : DropCreateDatabaseAlways<DataContext>
    {
        protected override void Seed(DataContext context)
        {
            base.Seed(context);

            //var typeInstaller = new TypeInstaller(context);
            //typeInstaller.Install();

            //var levelsInstaller = new LevelsInstaller(context);
            //levelsInstaller.Install();

            //var roleGroupsInstaller = new RoleGroupsInstaller(context);
            //roleGroupsInstaller.Install();

            //var usersInstaller = new UsersInstaller(context);
            //usersInstaller.Install();

            //var measurementsIntaller = new MeasurementsInstaller(context);
            //measurementsIntaller.Install();

            //var periodeInstaller = new PeriodeInstaller(context);
            //periodeInstaller.Install();

            //var methodInstaller = new MethodInstaller(context);
            //methodInstaller.Install();

            //var groupsInstaller = new GroupsInstaller(context);
            //groupsInstaller.Installer();

            //var pillarsInstaller = new PillarsInstaller(context);
            //pillarsInstaller.Installer();

            //var kpisInstaller = new KpisInstaller(context);
            //kpisInstaller.Install();

            //var kpiTargetsInstaller = new KpiTargetsInstaller(context);
            //kpiTargetsInstaller.Install();

            //var kpiAchievementsInstaller = new KpiAchievementsInstaller(context);
            //kpiAchievementsInstaller.Install();

            //var pmsSummariesInstaller = new PmsSummariesInstaller(context);
            //pmsSummariesInstaller.Install();

            //var pmsConfigsInstaller = new PmsConfigsInstaller(context);
            //pmsConfigsInstaller.Install();

            //var pmsConfigDetailsInstaller = new PmsConfigDetailsInstaller(context);
            //pmsConfigDetailsInstaller.Install();

            //var kpiWithRelationModel = new KpiWithRelationModelInstaller(context);
            //kpiWithRelationModel.Install();

            //var menuInstaller = new MenuInstaller(context);
            //menuInstaller.Install();

            //context.SaveChanges();
        }

        /*private void AddUser(DataContext context)
        {
            var user1 = new User();
            user1.Id = 1;
            user1.Username = "torres";
            user1.Email = "fernando@torres.com";
            user1.IsActive = true;
            user1.Role = new RoleGroup { Id = 1 };
            context.Users.AddOrUpdate(user1);
        }*/

        /*private void AddPilar(DataContext context)
        {
            var pilar1 = new Pilar();
            pilar1.Id = 1;
            pilar1.Code = "sft";
            pilar1.Name = "Safety";
            pilar1.Color = "#122381";
            pilar1.Order = 1;
            pilar1.Icon = "apalah";
            pilar1.Remark = "Ini Safety";
            pilar1.IsActive = true;

            var pilar2 = new Pilar();
            pilar1.Id = 2;
            pilar2.Code = "PaC";
            pilar2.Name = "Productivity and Efficienty";
            pilar2.Color = "#122381";
            pilar2.Order = 1;
            pilar2.Icon = "apalah";
            pilar2.Remark = "Ini Pac";
            pilar2.IsActive = true;

            context.Pilars.AddOrUpdate(pilar1);
            context.Pilars.AddOrUpdate(pilar2);
        }*/

        /*private void AddGroup(DataContext context)
        {
            var group = new Group();
            group.Id = 1;
            group.IsActive = true;
            group.Name = "Fatality";
            group.Order = 1;
            group.Remark = "test";
            context.Groups.AddOrUpdate(group);
        }

        private void AddMeasurement(DataContext context)
        {
            var measurement = new Measurement();
            measurement.Id = 1;
            measurement.IsActive = true;
            measurement.Name = "Measurement 1";
            measurement.Remark = "test";
            context.Measurements.AddOrUpdate(measurement);
        }*/

        //private void AddConversion(DataContext context)
        //{
        //    var conversion = new Conversion();
        //    conversion.Id = 1;
        //    conversion.From = new Measurement { Id = 1 };
        //    conversion.To = new Measurement { Id = 1 };
        //    conversion.IsReverse = false;
        //    conversion.Name = "Conversion One";
        //    conversion.Value = 200;
        //    context.Conversions.AddOrUpdate(conversion);
        //}

        //private void AddMethod(DataContext context)
        //{
        //    var method = new Method();
        //    method.Id = 1;
        //    method.Name = "Manual Input";
        //    method.IsActive = true;
        //    method.Remark = "hs";
        //    context.Methods.AddOrUpdate(method);
        //}

        //private void AddPeriode(DataContext context)
        //{
        //    var periode = new Periode();
        //    periode.Id = 1;
        //    periode.Name = PeriodeType.Monthly;
        //    periode.Remark = "Test";
        //    context.Periodes.AddOrUpdate(periode);
        //}

        /*private void AddKpi(DataContext context)
        {
            var kpi = new Kpi();
            kpi.Id = 1;
            kpi.Pilar = new Pilar { Id = 1 };
            kpi.Name = "Fatality/Strap Disability";
            kpi.Level = new Level { Id = 2 };
            kpi.RoleGroup = new RoleGroup { Id = 1 };
            kpi.Type = new Type { Id = 1 };
            kpi.Group = new Group { Id = 1 };
            kpi.Code = "ft";
            kpi.IsEconomic = false;
            kpi.Order = 1;
            kpi.YtdFormula = YtdFormula.Sum;
            kpi.Measurement = new Measurement { Id = 1 };
            kpi.Method = new Method { Id = 1 };
            kpi.Conversion = new Conversion { Id = 1 };
            kpi.ConversionId = 1;
            kpi.FormatInput = FormatInput.Sum;
            kpi.Periode = new Periode { Id = 1 };
            kpi.Value = DateTime.Now;
            context.Kpis.AddOrUpdate(kpi);
        }*/

        /*private void AddKpiTargets(DataContext context)
        {
            var kpiTargetFirstMonth = new KpiTarget()
            {
                Id = 1,
                CreatedBy = new User { Id = 1 },
                CreatedDate = DateTime.Now,
                IsActive = true,
                Periode = new DateTime(2015, 1, 1),
                Remark = "test",
                Value = 10,
                Kpi = new Kpi { Id = 1 }
            };

            var kpiTargetSecondMonth = new KpiTarget()
            {
                Id = 1,
                CreatedBy = new User { Id = 1 },
                CreatedDate = DateTime.Now,
                IsActive = true,
                Periode = new DateTime(2015, 2, 1),
                Remark = "test",
                Value = 20,
                Kpi = new Kpi { Id = 1 }
            };

            context.KpiTargets.AddOrUpdate(kpiTargetFirstMonth);
            context.KpiTargets.AddOrUpdate(kpiTargetSecondMonth);
        }*/

        /* private void AddKpiAchievements(DataContext context)
         {
             var kpiAchievementFirstMonth = new KpiAchievement()
             {
                 Id = 1,
                 CreatedBy = new User { Id = 1 },
                 CreatedDate = DateTime.Now,
                 IsActive = true,
                 Periode = new DateTime(2015, 1, 1),
                 Remark = "test",
                 Value = 5,
                 Kpi = new Kpi { Id = 1 }
             };

             var kpiAchievementSecondMonth = new KpiAchievement()
             {
                 Id = 1,
                 CreatedBy = new User { Id = 1 },
                 CreatedDate = DateTime.Now,
                 IsActive = true,
                 Periode = new DateTime(2015, 2, 1),
                 Remark = "test",
                 Value = 20,
                 Kpi = new Kpi { Id = 1 }
             };

             context.KpiAchievements.AddOrUpdate(kpiAchievementFirstMonth);
             context.KpiAchievements.AddOrUpdate(kpiAchievementSecondMonth);
         }*/

        /*private void AddPmsData(DataContext context)
        {
            var pmsConfig = new PmsConfig();
            pmsConfig.Id = 1;
            pmsConfig.IsActive = true;
            pmsConfig.Pilar = new Pilar { Id = 1 };
            pmsConfig.ScoreIndicator = new ScoreIndicator { IsActive = true, Color = "#126712", MaxValue = 20, MinValue = 0, RefId = 1 };
            pmsConfig.ScoringType = ScoringType.Positive;
            pmsConfig.Weight = 80;
            context.PmsConfigs.AddOrUpdate(pmsConfig);

            var pmsConfigDetails = new PmsConfigDetails();
            pmsConfigDetails.Id = 1;
            pmsConfigDetails.AsGraphic = true;
            pmsConfigDetails.CreatedBy = new User { Id = 9 };
            pmsConfigDetails.UpdatedBy = new User { Id = 9 };
            pmsConfigDetails.IsActive = true;
            pmsConfigDetails.Kpi = new Kpi { Id = 1 };
            pmsConfigDetails.ScoringType = ScoringType.Positive;
            pmsConfigDetails.ScoreIndicators.Add(new ScoreIndicator { Color = "#897879", IsActive = true, MaxValue = 20, MinValue = 0, RefId = 1 });
            pmsConfigDetails.ScoreIndicators.Add(new ScoreIndicator { Color = "#897879", IsActive = true, MaxValue = 60, MinValue = 21, RefId = 1 });
            pmsConfigDetails.Weight = 100;
            context.PmsConfigDetails.AddOrUpdate(pmsConfigDetails);
        }*/

        /* private void AddPmsSummary(DataContext context)
         {
             var pmsSummary = new PmsSummary();
             pmsSummary.CreatedBy = new User { Id = 9 };

             pmsSummary.Id = 1;
             pmsSummary.IsActive = true;
             pmsSummary.Periode = new Periode { Id = 1 };
             pmsSummary.ScoreIndicators.Add(new ScoreIndicator()
                 {
                     IsActive = true,
                     Color = "#213243",
                     MaxValue = 100,
                     MinValue = 20
                 });
             pmsSummary.Title = "pms summary";
             context.PmsSummaries.AddOrUpdate(pmsSummary);
         }*/
    }
}