﻿namespace Demo.Web.Windsor
{
    using MvcExtensions;
    using MvcExtensions.Windsor;

    public class MvcApplication : WindsorMvcApplication
    {
        public MvcApplication()
        {
            Bootstrapper.BootstrapperTasks
                        .Include<RegisterModelMetadata>()
                        .Include<RegisterControllers>()
                        .Include<ConfigureFilters>()
                        .Include<ConfigureModelBinders>()
                        .Include<RegisterRoutes>()
                        .Include<RegisterActionInvokers>();
        }
    }
}