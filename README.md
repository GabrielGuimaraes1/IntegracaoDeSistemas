*Este repositório demonstra, de forma direta e objetiva, como estruturo as principais etapas de uma integração entre sistemas,
exemplificando tais etapas com trechos de códigos relacionados a implementação da integração.*

---

## 1. Modelagem das Estruturas (Entrada da Informação)

### Dispositivo.cs

Define algumas classes usadas para representar os dados retornados pela API externa.  
Aqui são mapeados, via JSON (Newtonsoft), modelos como:

- **DeviceGroup** – agrupamento de dispositivos  
- **Device** – propriedades básicas de determinado dispositivo 
- **Item** – condições, status, alarmes  
- **DeviceData** – modelo completo do dispositivo (com todos os atributos)

---

## 2. Execução da Integração (Processamento e Regras de Negócio)

### IntegrarSistema.cs

Coordena todo o fluxo da integração.  
Este trecho de código faz:

- leitura das pendências internas  
- integração de associados e veículos  
- atualização de status  
- transferências entre rastreadores  
- validação dos registros  
- chamadas a stored procedures  
- tratamento de erros e montagem do retorno final  

---

## 3. Comunicação com a API Externa (Envio de Alterações)

### MetodoPUT_Dispositivo

Responsável por enviar alterações ao sistema externo. 

Inclui:

- definição do status (ativo/inativo)  
- construção da URL com IMEI, token e parâmetros  
- envio da requisição POST/PUT via RestSharp  
- validação do retorno JSON  

---

## 4. Definição das Rotas da API (Mapa Central)

### Sistema_a_Integrar.resx

Contém todas as rotas usadas nas requisições URL da integração:

- ativação/desativação  
- busca por dispositivo  
- listagem completa  
- listagem de modificados  
- criação de dispositivos  
- rotas auxiliares  

---

## 5. Upsert Inteligente (Inserção + Correção Automática)

### TrechoDispositivo.cs

Trecho de código flexível que funciona tanto como criação quanto alteração:

- completa as informações do dispositivo quando já existe no interno
- corrige informações desatualizadas ou diferentes
- busca os dados na API externa quando necessário  
- converte e organiza para o formato aceito pela API  

É onde acontece o alinhamento entre os sistemas, garantindo consistência mesmo quando a API tem limitações.

---

## Resumo Geral das Etapas

| Etapa | Arquivo Responsável |
|------|----------------------|
| Modelagem dos dados recebidos | `Dispositivo.cs` |
| Processamento principal da integração | `IntegrarSistema.cs` |
| Envio de atualizações ao fornecedor | `MetodoPUT_Dispositivo` |
| Centralização das rotas da API | `Sistema_a_Integrar.resx` |
| Inserção/atualização inteligente (upsert) | `TrechoDispositivo.cs` |

---

*Este repositório não representa um projeto completo — seu objetivo é apenas ilustrar de forma simples como estruturo integrações e organizo cada etapa do processo.*  
