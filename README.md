# Gerenciamento de Tarefas - Testes BDD

Este repositório contém testes automatizados para o sistema de Gerenciamento de Tarefas, utilizando Behavior-Driven Development (BDD) com SpecFlow e Xunit.

## Estrutura do Projeto

- **Features/**: Contém os arquivos `.feature` escritos em Gherkin, que descrevem os cenários de teste.
  - `GerenciamentoDeTarefas.feature`: Descreve os cenários relacionados ao gerenciamento de tarefas, como criação e conclusão de tarefas.
  
- **Steps/**: Contém as implementações dos steps BDD.
  - `GerenciamentoDeTarefasSteps.cs`: Implementa os passos (steps) descritos nos arquivos `.feature`.
  
- **Hooks/**: Contém os hooks do SpecFlow para inicializar e limpar o ambiente de teste.
  - `Hooks.cs`: Configura o ambiente antes e depois da execução dos cenários de teste.

- **Support/**: Arquivos de suporte para os testes.
  - `Claim.cs`: Classe usada para modelar reivindicações (claims) de segurança ou identidade do usuário.

- **Configurações**:
  - `specflow.json`: Configurações do SpecFlow, como cultura e linguagem.

## Requisitos

- .NET 6.0 ou superior
- Visual Studio 2022 ou superior
- Pacotes NuGet:
  - SpecFlow
  - Xunit
  - Moq (opcional, para mocks)

## Como Executar os Testes

1. **Clonar o Repositório**:
   ```bash
   git clone https://github.com/seuusuario/seurepositorio.git
   cd seurepositorio
