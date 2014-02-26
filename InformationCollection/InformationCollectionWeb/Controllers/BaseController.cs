using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TimiSoft.InformationCollection;
using TimiSoft.InformationCollection.Models;
using TimiSoft.InformationCollectionWeb.Filters;

namespace TimiSoft.InformationCollectionWeb.Controllers
{
    [InitializeSimpleMembership]
    public class BaseController : Controller
    {
        protected static log4net.ILog log = log4net.LogManager.GetLogger(typeof(AccountController));

        protected override void OnException(ExceptionContext filterContext)
        {
            log.Error(filterContext.Exception);
            base.OnException(filterContext);
        }
    }
}