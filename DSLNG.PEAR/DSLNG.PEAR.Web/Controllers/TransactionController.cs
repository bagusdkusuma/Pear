using DSLNG.PEAR.Services.Interfaces;
using FluentScheduler;
using StructureMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;

namespace DSLNG.PEAR.Web.Controllers
{
    public class TransactionController : BaseController
    {
        IUserService _userService;

        public TransactionController(IUserService userService)
        {
            _userService = userService;
        }

        public ActionResult Index()
        {
            var registry = new Registry();
            registry.Schedule<MyJob>().ToRunNow().AndEvery(10).Seconds();
            JobManager.Initialize(registry);
            return View();
        }
    }

    

    public class MyJob : IJob, System.Web.Hosting.IRegisteredObject
    {
        IUserService _userService;
        private readonly object _lock = new object();

        private bool _shuttingDown;

        public MyJob(IUserService userService)
        {
            _userService = userService;
            HostingEnvironment.RegisterObject(this);
        }

        public void Execute()
        {
            lock (_lock)
            {
                if (_shuttingDown)
                    return;

                var user = _userService.GetUserByEmail(new Services.Requests.User.GetUserRequest { Email = "admin@dslng.com" });
                _userService.Create(new Services.Requests.User.CreateUserRequest
                {
                    Email = new Random().Next(200).ToString() + "@gmail.com",
                    IsActive = true,
                    Password = "asas",
                    RoleId = 1
                });
            }

           

        }

        public void Stop(bool immediate)
        {
            lock (_lock)
            {
                _shuttingDown = true;
            }

            HostingEnvironment.UnregisterObject(this);
        }
    }
}