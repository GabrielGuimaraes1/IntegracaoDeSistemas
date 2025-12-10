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

-------------------------------------------------------------------------------------------------------------------------

- MetodoPUT_Dispositivo -
--> exemplo de mecanismo de método de alteração de dados em integração.
  
Implementa a lógica responsável por alterar o status de um dispositivo de rastreamento em um sistema externo, 
utilizando uma chamada HTTP via RestSharp com arquivo JSON.

A rotina executa:
- Determinação do estado do dispositivo (ativo / inativo) com base na situação do veículo.
- Construção dinâmica da URL da API do fornecedor, incluindo parâmetros como IMEI, token de autenticação e status desejado.
- Envio de uma requisição POST para o endpoint remoto (/api/admin/device/{imei}/status).
- Interpretação da resposta, validando sucesso a partir do conteúdo JSON retornado.
- Retorno da resposta, indicando se a operação foi concluída com êxito.

O método executa a alternância entre ativação/desativação do dispositivo de rastreamento no sistema do fornecedor via API.

------------------------------------------------------------------------------------------------------------------------

- Sistema_a_Integrar.resx -
  
O arquivo define todas as rotas da API Sistema_a_Integrar que são utilizadas pela integração para consultar, criar e alterar
dispositivos de rastreamento. Ele centraliza as URLs necessárias para:
ativar/desativar dispositivos;
buscar um dispositivo específico;
listar todos os dispositivos;
obter apenas dispositivos alterados recentemente;
recuperar informações adicionais;
criar novos dispositivos.

Em essência, o arquivo funciona como um mapeamento central de endpoints, padronizando e organizando todas as URLs 
usadas pela integração com o sistema SmartLocaliza.

