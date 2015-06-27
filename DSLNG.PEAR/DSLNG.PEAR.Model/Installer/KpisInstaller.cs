﻿
using DSLNG.PEAR.Data.Entities;
using DSLNG.PEAR.Data.Persistence;
using System.Linq;
using System.Data.Entity;

namespace DSLNG.PEAR.Data.Installer
{
    public class KpisInstaller
    {
        private readonly DataContext _context;
        public KpisInstaller(DataContext context)
        {
            _context = context;
        }
        public void Install()
        {
            var fatality = new Kpi
            {
                Id = 1,
                Name = "Fatality/Strap Disability",
                Measurement = _context.Measurements.Local.First(x => x.Id == 1),
                Pillar = _context.Pillars.Local.First(x => x.Id == 1),
                Order = 1
            };
            var securityIncident = new Kpi
            {
                Id = 2,
                Name = "QHSE Training Attend",
                Measurement = _context.Measurements.Local.First(x => x.Id == 1),
                Pillar = _context.Pillars.Local.First(x => x.Id == 1),
                Order = 2
            };
            var rif = new Kpi
            {
                Id = 3,
                Name = "RIF",
                Measurement = _context.Measurements.Local.First(x => x.Id == 1),
                Pillar = _context.Pillars.Local.First(x => x.Id == 1),
                Order = 3
            };

            var plantAvailability = new Kpi
                {
                    Id = 4,
                    Name = "Plant Availability",
                    Measurement = _context.Measurements.Local.First(x => x.Id == 1),
                    Pillar = _context.Pillars.Local.First(x => x.Id == 2),
                    Order = 3
                };
            _context.Kpis.Add(fatality);
            _context.Kpis.Add(securityIncident);
            _context.Kpis.Add(rif);
            _context.Kpis.Add(plantAvailability);
        }
    }
}