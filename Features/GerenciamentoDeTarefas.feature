# language: pt
Funcionalidade: Gerenciamento de Tarefas
  Como um usuário do sistema
  Eu quero gerenciar minhas tarefas
  Para que eu possa manter o controle do meu trabalho

  Cenário: Iniciar uma tarefa existente
    Dado que existe uma tarefa com ID "1" 
    E status "Pendente"
    Quando o usuário inicia a tarefa com ID "1"
    Então a tarefa deve ser atualizada para o status "Em Progresso"
    E uma mensagem de log deve ser registrada para indicar a mudança de status

  Cenário: Concluir uma tarefa existente
    Dado que existe uma tarefa com ID "2" 
    E status "Em Progresso"
    Quando o usuário conclui a tarefa com ID "2"
    Então a tarefa deve ser atualizada para o status "Concluída"
    E uma notificação de tarefa concluída deve ser enviada

  Cenário: Cancelar uma tarefa existente
    Dado que existe uma tarefa com ID "3" 
    E status "Em Progresso"
    Quando o usuário cancela a tarefa com ID "3"
    Então a tarefa deve ser atualizada para o status "Cancelada"
 

  Cenário: Obter uma tarefa por ID
    Dado que existe uma tarefa com ID "4"
    Quando o usuário solicita a tarefa com ID "4"
    Então o sistema deve retornar os detalhes da tarefa com ID "4"
  

 

  Cenário: Criar uma nova tarefa
    Dado que um usuário válido deseja criar uma nova tarefa
    Quando o usuário cria uma tarefa com a descrição "Tarefa 1"
    Então o sistema deve adicionar a nova tarefa
    E uma notificação de criação de tarefa deve ser enviada

  Cenário: Alterar uma tarefa existente
    Dado que existe uma tarefa com ID "5"
    Quando o usuário altera a descrição para "Nova Descrição" e status para "Em Progresso"
    Então a tarefa deve ser atualizada no sistema

  Cenário: Remover uma tarefa existente
    Dado que existe uma tarefa com ID "5"
    Quando o usuário remove a tarefa com ID "5"
    Então a tarefa deve ser removida do sistema
  

  
