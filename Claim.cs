
namespace GerenciamentoTarefas.BDDTests.Steps
{
    internal class Claim : System.Security.Claims.Claim
    {
        public Claim(string type, string value) : base(type, value)
        {
        }
    }
}