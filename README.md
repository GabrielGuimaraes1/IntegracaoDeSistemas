# README

🌎 [Português](#português) | 🇺🇸 [English](#english)

## Português

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

*Este repositório não representa um projeto completo — seu objetivo é apenas ilustrar de forma simples como estruturo 
integrações e organizo cada etapa do processo.*  

---   ---   ---   ---   ---   ---   ---   ---   ---   ---   ---   ---   ---   ---   ---   ---   ---   ---   ---   ---   ---   ---   ---   ---   ---   ---   ---   ---   ---   ---   ---   ---   ---   ---   ---   ---   ---   ---   ---   

## English

*This repository demonstrates, in a direct and objective way, how I structure the main stages of a system-to-system integration, 
illustrating these stages with code excerpts related to the implementation of the integration.*

---

## 1. Data Structure Modeling (Input Stage)

### Dispositivo.cs

Defines several classes used to represent the data returned by the external API.  
Here, JSON models (using Newtonsoft) are mapped, such as:

- **DeviceGroup** – device grouping  
- **Device** – basic properties of a specific device  
- **Item** – conditions, status, alarms  
- **DeviceData** – complete device model (with all attributes)

---

## 2. Integration Execution (Processing and Business Rules)

### IntegrarSistema.cs

Coordinates the entire integration flow.  
This code excerpt performs:

- reading of internal pending items  
- integration of members and vehicles  
- status updates  
- tracker-to-tracker transfers  
- record validation  
- execution of stored procedures  
- error handling and assembly of the final result  

---

## 3. Communication with the External API (Sending Updates)

### MetodoPUT_Dispositivo

Responsible for sending updates to the external system.

Includes:

- defining the device status (active/inactive)  
- building the URL using IMEI, token, and parameters  
- sending a POST/PUT request using RestSharp  
- validating the JSON response  

---

## 4. API Route Definitions (Central Mapping)

### Sistema_a_Integrar.resx

Contains all API routes used by the integration:

- activation/deactivation  
- device search  
- full device listing  
- listing of recently modified devices  
- device creation  
- auxiliary routes  

---

## 5. Smart Upsert (Insert + Automatic Correction)

### TrechoDispositivo.cs

A flexible piece of code that works for both creation and update:

- completes device information when it already exists internally  
- corrects outdated or inconsistent information  
- fetches data from the external API when necessary  
- converts and organizes data into the format expected by the API  

This is where alignment between systems happens, ensuring consistency even when the external API has limitations.

---

## Summary of Stages

| Stage | Responsible File |
|------|-------------------|
| Data structure modeling | `Dispositivo.cs` |
| Main integration processing | `IntegrarSistema.cs` |
| Sending updates to the provider | `MetodoPUT_Dispositivo` |
| Centralization of API routes | `Sistema_a_Integrar.resx` |
| Smart insert/update (upsert) | `TrechoDispositivo.cs` |

---

*This repository is not a complete project — its purpose is simply to illustrate how I structure system integrations and organize each stage of the process.*  

