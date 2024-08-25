using BoDi;
using Microsoft.AspNetCore.Http;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace GerenciamentoTarefas.BDDTests
{
    [Binding]
    public class Hooks
    {
        private readonly IObjectContainer _objectContainer;

        public Hooks(IObjectContainer objectContainer)
        {
            _objectContainer = objectContainer;
        }

        [BeforeScenario]
        public void RegisterDependencies()
        {
            var httpContextAccessorMock = new Mock<IHttpContextAccessor>();

            // Configurar o mock como mencionado anteriormente
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
            new Claim(ClaimTypes.NameIdentifier, "1"),
            new Claim(ClaimTypes.Email, "usuario@exemplo.com"),
            }, "mock"));

            var httpContext = new DefaultHttpContext
            {
                User = user
            };

            httpContextAccessorMock.Setup(_ => _.HttpContext).Returns(httpContext);

            _objectContainer.RegisterInstanceAs<IHttpContextAccessor>(httpContextAccessorMock.Object);
        }
    }

}
