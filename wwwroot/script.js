const API_URL =
  "https://desenvolvimento-de-apis-e-servi-os-web.onrender.com/Api";

// FUNÇÃO PARA NAVEGAR ENTRE AS TELAS DO CARD
function mudarTela(telaDestino) {
  document.getElementById("loginScreen").style.display = "none";
  document.getElementById("cadastroScreen").style.display = "none";
  document.getElementById("dashboard").style.display = "none";

  document.getElementById(telaDestino).style.display = "block";
}

// FUNÇÃO DE CADASTRO
async function fazerCadastro() {
  const usuario = document.getElementById("cadUsuario").value;
  const senha = document.getElementById("cadSenha").value;
  const msgCad = document.getElementById("cadastroMensagem");

  try {
    const response = await fetch(`${API_URL}/Auth/register`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ Usuario: usuario, Senha: senha }),
    });

    if (response.ok) {
      msgCad.className = "sucesso";
      msgCad.innerText = "Conta criada com sucesso! Redirecionando...";
      setTimeout(() => {
        mudarTela("loginScreen");
        msgCad.innerText = "";
      }, 2000);
    } else {
      const data = await response.json();
      msgCad.className = "erro";
      msgCad.innerText = data.erro || "Erro ao criar conta.";
    }
  } catch (error) {
    msgCad.className = "erro";
    msgCad.innerText = "Erro ao conectar com o servidor.";
  }
}

// FUNÇÃO DE LOGIN
async function fazerLogin() {
  const usuario = document.getElementById("loginUsuario").value;
  const senha = document.getElementById("loginSenha").value;
  const msgLogin = document.getElementById("loginMensagem");

  try {
    const response = await fetch(`${API_URL}/Auth/login`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ Usuario: usuario, Senha: senha }),
    });

    const data = await response.json();

    if (response.ok) {
      localStorage.setItem("token_banco", data.token);
      localStorage.setItem("usuario_banco", usuario);

      document.getElementById("nomeUsuario").innerText = usuario;
      mudarTela("dashboard");

      buscarSaldoAtual();
    } else {
      msgLogin.innerText = data.erro || "Usuário ou senha incorretos";
    }
  } catch (error) {
    msgLogin.innerText = "Erro ao conectar com o servidor.";
  }
}

// FUNÇÃO PARA BUSCAR O SALDO DO BANCO DE DADOS
async function buscarSaldoAtual() {
  const token = localStorage.getItem("token_banco");
  try {
    const response = await fetch(`${API_URL}/Contas/saldo`, {
      method: "GET",
      headers: { Authorization: `Bearer ${token}` },
    });
    if (response.ok) {
      const data = await response.json();
      document.getElementById("saldoAtual").innerText = data.saldo.toFixed(2);
    }
  } catch (e) {
    console.log("Não foi possível carregar o saldo inicial.");
  }
}

// FUNÇÃO DE TRANSAÇÃO (SAQUE/DEPÓSITO)
async function realizarTransacao(tipo) {
  const valor = parseFloat(document.getElementById("valorTransacao").value);
  const token = localStorage.getItem("token_banco");
  const msgBox = document.getElementById("transacaoMensagem");

  if (!valor || valor <= 0) {
    msgBox.className = "erro";
    msgBox.innerText = "Insira um valor válido.";
    return;
  }

  try {
    const response = await fetch(`${API_URL}/Transacoes/${tipo}`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${token}`,
      },
      body: JSON.stringify({
        Valor: valor,
        TipoOperacao: tipo,
      }),
    });

    const data = await response.json();

    if (response.ok) {
      msgBox.className = "sucesso";
      msgBox.innerText = data.mensagem || "Operação realizada com sucesso.";
      document.getElementById("valorTransacao").value = "";

      if (data.novoSaldo !== undefined) {
        document.getElementById("saldoAtual").innerText =
          data.novoSaldo.toFixed(2);
      } else {
        buscarSaldoAtual();
      }
    } else {
      msgBox.className = "erro";
      msgBox.innerText = data.erro || data.Erro || "Erro na transação.";
    }
  } catch (error) {
    msgBox.className = "erro";
    msgBox.innerText = "Erro de conexão com a API.";
  }
}

// FUNÇÃO DE LOGOUT
function logout() {
  localStorage.removeItem("token_banco");
  localStorage.removeItem("usuario_banco");
  document.getElementById("loginUsuario").value = "";
  document.getElementById("loginSenha").value = "";
  document.getElementById("transacaoMensagem").innerText = "";
  mudarTela("loginScreen");
}

// VERIFICAÇÃO DE SESSÃO ATIVA AO RECARREGAR A PÁGINA
window.onload = function () {
  const tokenSalvo = localStorage.getItem("token_banco");
  const usuarioSalvo = localStorage.getItem("usuario_banco");
  if (tokenSalvo && usuarioSalvo) {
    document.getElementById("nomeUsuario").innerText = usuarioSalvo;
    mudarTela("dashboard");
    buscarSaldoAtual();
  }
};
