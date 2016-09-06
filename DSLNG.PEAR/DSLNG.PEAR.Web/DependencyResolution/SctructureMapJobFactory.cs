using FluentScheduler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DSLNG.PEAR.Web.DependencyResolution
{
    public class StructureMapJobFactory : IJobFactory
    {
        public IJob GetJobInstance<T>() where T : IJob
        {
            return ObjectFactory.Container.GetInstance<T>();
        }
    }

}