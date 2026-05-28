Aqui está o texto prontinho e formatado em Markdown para você salvar diretamente no seu arquivo `README.md`. Ele está alinhado exatamente com o seu projeto, refletindo o uso do .NET 8, MySQL e JWT.

---

# 🏦 Meu Banco Digital

Este repositório contém o código de uma aplicação bancária completa e funcional, desenvolvida com foco em segurança, performance e código limpo. O projeto simula a experiência de um banco digital moderno através de uma interface interativa integrada a uma API robusta.

---

## 1. Descrição do Projeto

O **Meu Banco Digital** é um sistema bancário baseado em uma arquitetura desacoplada. O sistema funciona como uma **Single Page Application (SPA)** no frontend, que consome uma **API REST** segura no backend. O objetivo do projeto é gerenciar de forma consistente contas bancárias, autenticação de clientes, registros de histórico de transações em tempo real e um sistema de poupança integrada através de um "Cofrinho".

---

## 2. Tecnologias Utilizadas

* **Linguagem:** C# (Backend) e JavaScript / HTML5 / CSS3 (Frontend)
* **Framework:** ASP.NET Core (.NET 8) e Entity Framework Core
* **Banco de Dados:** MySQL
* **Segurança:** Autenticação e autorização baseadas em **Tokens JWT (JSON Web Tokens)**
* **Documentação de API:** Swagger / OpenAPI

---

## 3. Instruções de Execução

Para rodar o projeto localmente no seu ambiente de desenvolvimento, siga os passos abaixo:

1. **Configurar o Banco de Dados:** Certifique-se de que o serviço do MySQL está ativo na sua máquina. Abra o arquivo `appsettings.json` na raiz da API e ajuste as credenciais do campo `DefaultConnection` com a sua senha local do MySQL.
2. **Instalar a Ferramenta do EF Core:** Caso ainda não possua a CLI do Entity Framework instalada globalmente, execute o seguinte comando no seu terminal:
```bash
dotnet tool install --global dotnet-ef

```


3. **Criar a Estrutura do Banco:** Na pasta raiz do projeto backend, execute as migrações para desenhar o banco de dados e as tabelas automaticamente no seu MySQL:
```bash
dotnet ef database update

```


4. **Iniciar o Servidor API:** Com o banco de dados configurado, dê o comando para rodar o servidor C#:
```bash
dotnet run

```


5. **Acessar a Interface:** Mantendo o terminal do C# aberto e rodando, basta abrir o arquivo `index.html` diretamente em qualquer navegador de sua preferência para interagir com o sistema.

---

## 4. Endpoints da API

Abaixo estão os principais endpoints disponíveis no sistema para a comunicação com o frontend:

### Autenticação e Usuários (`/Api/Auth`)

| Método | Endpoint | Descrição |
| --- | --- | --- |
| `POST` | `/Api/Auth/register` | Cria uma nova conta bancária (Suporta termos da LGPD). |
| `POST` | `/Api/Auth/login` | Realiza a autenticação e retorna o Token JWT de acesso. |

### Detalhes da Conta (`/Api/Contas`)

| Método | Endpoint | Descrição |
| --- | --- | --- |
| `GET` | `/Api/Contas/detalhes` | Retorna o saldo, limite, cofrinho e transações do usuário logado. *(Requer Token JWT)* |

### Operações Bancárias (`/Api/Transacoes` e `/Api/Cofrinho`)

| Método | Endpoint | Descrição |
| --- | --- | --- |
| `POST` | `/Api/Transacoes/depositar` | Adiciona fundos ao saldo disponível da conta. *(Requer Token JWT)* |
| `POST` | `/Api/Transacoes/sacar` | Retira fundos do saldo disponível da conta. *(Requer Token JWT)* |
| `POST` | `/Api/Cofrinho/guardar` | Move dinheiro do saldo disponível para o cofrinho. *(Requer Token JWT)* |
| `POST` | `/Api/Cofrinho/resgatar` | Retorna o dinheiro guardado no cofrinho para o saldo disponível. *(Requer Token JWT)* |
