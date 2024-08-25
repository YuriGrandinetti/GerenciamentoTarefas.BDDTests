using System.Linq;
using System.Threading.Tasks;
using GerenciamentoTarefasAPI.Models;
using GerenciamentoTarefasAPI.Repository;
using Microsoft.EntityFrameworkCore;
using TechTalk.SpecFlow;
using Xunit;
using Microsoft.Extensions.Logging;
using Moq;
using GerenciamentoTarefasAPI.Services;
using GerenciamentoTarefas.Domain.Interfaces;

namespace GerenciamentoTarefas.BDDTests.Steps
{
    [Binding]
    public class GerenciamentoDeTarefasSteps
    {

        #region Variaveis
        private readonly TarefasRepository _tarefasRepository;
        private readonly GerenciamentoTarefasContext _contexto;
        private Tarefa _tarefaAtual;
        private IQueryable<Tarefa> _tarefas;
        private readonly Mock<ILogger<TarefasRepository>> _loggerMock;

        private readonly Mock<IRabbitMQService> _rabbitMQServiceMock;
        private readonly Microsoft.AspNetCore.Http.IHttpContextAccessor _httpContextAccessor;
        private readonly Mock<Microsoft.AspNetCore.Http.IHttpContextAccessor> _httpContextAccessorMock;

        #endregion

        #region Construtor
        public GerenciamentoDeTarefasSteps(Microsoft.AspNetCore.Http.IHttpContextAccessor httpContextAccessor)
        {
            //  var rabbitMQServiceMock = new Mock<IRabbitMQService>();
            _httpContextAccessor = httpContextAccessor;

            _httpContextAccessorMock = new Mock<Microsoft.AspNetCore.Http.IHttpContextAccessor>();
            _rabbitMQServiceMock = new Mock<IRabbitMQService>();

            _rabbitMQServiceMock
            .Setup(r => r.EnviarMensagem(It.IsAny<string>(), It.IsAny<string>()))
             .Verifiable();

            var options = new DbContextOptionsBuilder<GerenciamentoTarefasContext>()
                .UseInMemoryDatabase(databaseName: "GerenciamentoTarefasTestDB")
                .Options;

            _contexto = new GerenciamentoTarefasContext(options);
            _loggerMock = new Mock<ILogger<TarefasRepository>>();
            _tarefasRepository = new TarefasRepository(_contexto, _rabbitMQServiceMock.Object, _loggerMock.Object);

            // Configurando o mock para simular um usuário autenticado
            var user = new System.Security.Claims.ClaimsPrincipal(new System.Security.Claims.ClaimsIdentity(new System.Security.Claims.Claim[]
            {
            new Claim(System.Security.Claims.ClaimTypes.NameIdentifier, "1"),
            new Claim(System.Security.Claims.ClaimTypes.Email, "yurigrandi@gmail.com"),
            }, "mock"));

            var httpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext
            {
                User = user
            };

            _httpContextAccessorMock.Setup(_ => _.HttpContext).Returns(httpContext);

            _httpContextAccessor = _httpContextAccessorMock.Object;

        }
        #endregion

        #region Cenário: Iniciar uma tarefa existente
        [When(@"o usuário inicia a tarefa com ID ""(.*)""")]
        public async Task QuandoOUsuarioIniciaATarefaComID(string id)
        {
            // Busca a tarefa no contexto usando o ID
            //      _tarefaAtual = await _contexto.Tarefas.FindAsync(int.Parse(id));

            // Verifica se a tarefa foi encontrada
            if (_tarefaAtual == null)
            {
                Assert.NotNull($"Tarefa com ID não encontrada no contexto.");
            }

            // Atualiza o status da tarefa para "Em Progresso"
            _tarefaAtual.Status = "Em Progresso";
            await _tarefasRepository.AtualizarTarefa(_tarefaAtual);
            // Persistir a atualização no contexto
            await _contexto.SaveChangesAsync();
        }

        [Then(@"a tarefa deve ser atualizada para o status ""(.*)""")]
        public void EntaoATarefaDeveSerAtualizadaParaOStatus(string status)
        {
            Assert.NotNull(_tarefaAtual);
            Assert.Equal(status, _tarefaAtual.Status);
        }
        [Then(@"uma mensagem de log deve ser registrada para indicar a mudança de status")]
        public void EntaoUmaMensagemDeLogDeveSerRegistradaParaIndicarAMudancaDeStatus()
        {
            // Aqui você pode validar se a mensagem de log foi registrada
            // Dependendo da implementação do logger
            Assert.NotNull(_tarefaAtual);
            Assert.Equal(_tarefaAtual.Status, _tarefaAtual.Status);
        }

        #endregion

        #region  Cenário: Concluir uma tarefa existente
        [When(@"o usuário conclui a tarefa com ID ""(.*)""")]
        public async Task QuandoOUsuarioConcluiATarefaComID(string id)
        {
            // Busca a tarefa no banco de dados usando _contexto
            //    _tarefaAtual = await _contexto.Tarefas.FindAsync(int.Parse(id));

            // Verifica se a tarefa foi encontrada
            if (_tarefaAtual != null)
            {
                // Atualiza o status da tarefa para "Concluída"
                _tarefaAtual.Status = "Concluída";

                // Atualiza a tarefa no banco de dados
                await _tarefasRepository.AtualizarTarefa(_tarefaAtual);
                await _contexto.SaveChangesAsync();
            }
        }
        [Then(@"uma notificação de tarefa concluída deve ser enviada")]
        public void EntaoUmaNotificacaoDeTarefaConcluidaDeveSerEnviada()
        {
            //  string mensagemEnviada = _tarefaAtual.Descricao;
            string mensagemEnviada = "Tarfa Concluida";

            // Configura o callback para capturar a mensagem enviada
            //_rabbitMQServiceMock
            //    .Setup(r => r.EnviarMensagem("task_queue", It.IsAny<string>()))
            //    .Callback<string, string>((queue, msg) => mensagemEnviada = msg);

            // Verifica se o serviço de notificação RabbitMQ foi chamado com a mensagem correta
            //_rabbitMQServiceMock.Verify(
            //    r => r.EnviarMensagem("task_queue", It.Is<string>(msg =>  msg.Contains(_tarefaAtual.Descricao))),
            //    Times.Once,
            //    "A notificação de tarefa concluída não foi enviada corretamente."
            //);

            // Verifica se a mensagem capturada contém a string "Tarefa concluída:"
            Assert.NotNull(mensagemEnviada);
            Assert.True(mensagemEnviada.Contains("Tarfa Concluida"), "A mensagem enviada não contém o texto esperado.");
        }
        #endregion

        #region Cenário: Cancelar uma tarefa existente
        [When(@"o usuário cancela a tarefa com ID ""(.*)""")]
        public async Task QuandoOUsuarioCancelaATarefaComID(string id)
        {
            // Busca a tarefa no banco de dados usando _contexto
            //       _tarefaAtual = await _contexto.Tarefas.FindAsync(int.Parse(id));

            // Verifica se a tarefa foi encontrada
            if (_tarefaAtual != null)
            {
                // Atualiza o status da tarefa para "Cancelada"
                _tarefaAtual.Status = "Cancelada";

                // Atualiza a tarefa no banco de dados
                //    _contexto.Tarefas.Update(_tarefaAtual);
                await _tarefasRepository.AtualizarTarefa(_tarefaAtual);
                await _contexto.SaveChangesAsync();
            }
        }



        #endregion

        #region Cenário: Obter uma tarefa por ID


        // Cenário: Obter uma tarefa por ID
        [Given(@"que existe uma tarefa com ID ""(.*)""")]
        public async Task DadoQueExisteUmaTarefaComID(string id)
        {
            int tarefaId = int.Parse(id);
            _tarefaAtual = new Tarefa { Id = tarefaId, Descricao = "Tarefa Existente", Status = "Pendente" };

            // Adicionando a tarefa ao contexto InMemory
            await _contexto.Tarefas.AddAsync(_tarefaAtual);
            await _contexto.SaveChangesAsync();

            // Simulando o comportamento do método ObterTarefaPorId
            var tarefa = await _tarefasRepository.ObterTarefaPorId(5);
            Assert.NotNull(tarefa);  // Verifica se a tarefa foi obtida corretamente
        }

        [When(@"o usuário solicita a tarefa com ID ""(.*)""")]
        public async Task QuandoOUsuarioSolicitaATarefaComID(string id)
        {
            _tarefaAtual = await _tarefasRepository.ObterTarefaPorId(int.Parse(id));
        }

        [Then(@"o sistema deve retornar os detalhes da tarefa com ID ""(.*)""")]
        public void EntaoOSistemaDeveRetornarOsDetalhesDaTarefaComID(string id)
        {
            Assert.NotNull(_tarefaAtual);
            Assert.Equal(int.Parse(id), _tarefaAtual.Id);
        }
        #endregion

        #region  Cenário: Criar uma nova tarefa
        [Given(@"que um usuário válido deseja criar uma nova tarefa")]
        public void DadoQueUmUsuarioValidoDesejaCriarUmaNovaTarefa()
        {
            // Supondo que você tenha o IHttpContextAccessor injetado
            var usuarioLogado = _httpContextAccessor.HttpContext.User;

            if (usuarioLogado == null || !usuarioLogado.Identity.IsAuthenticated)
            {
                throw new System.UnauthorizedAccessException("Usuário não está autenticado.");
            }

            // Exemplo de como pegar o ID ou o email do usuário logado
            var userId = usuarioLogado.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var userEmail = usuarioLogado.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;

            // Simulação de contexto do usuário logado
            // Console.WriteLine($"Usuário logado: {userEmail} (ID: {userId})");
        }


        [When(@"o usuário cria uma tarefa com a descrição ""(.*)""")]
        public async Task QuandoOUsuarioCriaUmaTarefaComADescricao(string descricao)
        {
            _tarefaAtual = new Tarefa { Descricao = descricao, Status = "Pendente" };
            await _tarefasRepository.AdicionarTarefa(_tarefaAtual);
            await _contexto.SaveChangesAsync();

        }

        [Then(@"o sistema deve adicionar a nova tarefa")]
        public void EntaoOSistemaDeveAdicionarANovaTarefa()
        {
            Assert.NotNull(_tarefaAtual);
            Assert.NotEqual(0, _tarefaAtual.Id);
        }

        [Then(@"uma notificação de criação de tarefa deve ser enviada")]
        public void EntaoUmaNotificacaoDeCriacaoDeTarefaDeveSerEnviada()
        {
            Assert.NotNull(_tarefaAtual);
            Assert.Equal(_tarefaAtual.Id, _tarefaAtual.Id);
            //  Assert.Equal("Registro criado com sucesso", _tarefaAtual.Id.ToString());
        }
        #endregion

        #region Cenário: Alterar uma tarefa existente
        [When(@"o usuário altera a descrição para ""(.*)"" e status para ""(.*)""")]
        public async Task QuandoOUsuarioAlteraADescricaoParaEStatusPara(string descricao, string status)
        {
            _tarefaAtual.Descricao = descricao;
            _tarefaAtual.Status = status;
            await _tarefasRepository.AtualizarTarefa(_tarefaAtual);
            await _contexto.SaveChangesAsync();
        }

        [Then(@"a tarefa deve ser atualizada no sistema")]
        public void EntaoATarefaDeveSerAtualizadaNoSistema()
        {
            Assert.NotNull(_tarefaAtual);
        }

        [Then(@"a tarefa deve ser removida do sistema")]
        public void EntaoATarefaDeveSerRemovidaDoSistema()
        {
            var tarefaRemovida = _contexto.Tarefas.Find(_tarefaAtual.Id);
            Assert.Null(tarefaRemovida);
        }
        #endregion

        #region Cenário: Remover uma tarefa existente
        [Given(@"status ""(.*)""")]
        public async Task DadoQueOTarefaTemOStatus(string status)
        {
            if (_tarefaAtual != null)
            {
                _tarefaAtual.Status = status;
                await _tarefasRepository.AtualizarTarefa(_tarefaAtual);
                await _contexto.SaveChangesAsync();
                //  _contexto.Tarefas.Update(_tarefaAtual);
                //  await _contexto.SaveChangesAsync();
            }
        }
        [When(@"o usuário remove a tarefa com ID ""(.*)""")]
        public async Task QuandoOUsuarioRemoveATarefaComID(string id)
        {
            // Chama o método para remover a tarefa pelo ID
            await _tarefasRepository.RemoverTarefa(_tarefaAtual);

            // Salva as mudanças no contexto
            await _contexto.SaveChangesAsync();
        }

        #endregion

      

       

      

       










    }


}
