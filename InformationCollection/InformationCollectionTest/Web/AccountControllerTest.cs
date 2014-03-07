using System;
using System.Web;
using System.Web.Routing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TimiSoft.InformationCollectionWeb;

namespace InformationCollectionTest.Web
{
    [TestClass]
    public class AccountControllerTest
    {
        [TestMethod]
        public void LoginTest()
        {
            //var mockContext = MockRepository..GenerateMock<HttpContextBase>();
            //var mockContext = MockRepository.GenerateMock<HttpContext>(); 
            //var request = new Mock<HttpRequestBase>(MockBehavior.Strict);
            //request.SetupGet(x => x.ApplicationPath).Returns("/");
            //request.SetupGet(x => x.Url).Returns(new Uri("http://localhost/a", UriKind.Absolute));
            //request.SetupGet(x => x.ServerVariables).Returns(new System.Collections.Specialized.NameValueCollection());

            //var response = new Mock<HttpResponseBase>(MockBehavior.Strict);
            //response.Setup(x => x.ApplyAppPathModifier(Moq.It.IsAny<String>())).Returns((String url) => url);
            //// response.SetupGet(x => x.Cookies).Returns(new HttpCookieCollection()); // This also failed to work

            //var context = new Mock<HttpContextBase>(MockBehavior.Strict);
            //context.SetupGet(x => x.Request).Returns(request.Object);
            //context.SetupGet(x => x.Response).Returns(response.Object);
            //context.SetupGet(x => x.Response.Cookies).Returns(new HttpCookieCollection()); // still can't call the Clear() method

            ////
            //// Mock the controller context (using the objects mocked above)
            ////
            //var moqCtx = new Mock<ControllerContext>(context.Object, new RouteData(), ctl);
            //moqCtx.SetupGet(p => p.HttpContext.User.Identity.Name).Returns(username);
            //moqCtx.SetupGet(p => p.HttpContext.User.Identity.IsAuthenticated).Returns(true);
            //if (!string.IsNullOrEmpty(role.ToString()))
            //    moqCtx.Setup(p => p.HttpContext.User.IsInRole(role.ToString())).Returns(true);

            ////
            //// Pass the mocked ControllerContext and create UrlHelper for the controller and return
            ////
            //ctl.ControllerContext = moqCtx.Object;
            //ctl.Url = new UrlHelper(new RequestContext(context.Object, new RouteData()), routes);
            //return 1;
        }
    }
}
