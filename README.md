- IntegrarSistema.cs -

IntegrarSistema.cs implementa a rotina responsável por integrar os dados do fornecedor "Sistema_a_Integrar"
ao nosso sistema ERP interno. A classe centraliza todo o fluxo de execução da integração desde a obtenção 
das pendências até o processamento detalhado de cada registro. Ela inicia carregando o contexto de dados e 
verificando a existência de integrações pendentes e, a partir disso, executa operações como:

- integração de associados;
- integração de veículos;
- atualização de status;
- transferência de veículos entre rastreadores;
- montagem de retorno formatado para exibição no sistema.

O código contém métodos privados especializados para manipular listas retornadas por stored procedures, 
validar registros, aplicar regras de negócio e enviar as atualizações ao banco. A estrutura também trata 
erros de execução e garante que somente integrações válidas sejam processadas.





