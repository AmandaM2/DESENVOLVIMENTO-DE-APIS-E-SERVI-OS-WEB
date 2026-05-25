const API_URL = "http://localhost:5250/Api";

// FUNÇÃO PARA NAVEGAR ENTRE AS TELAS DO CARD
function mudarTela(telaDestino) {
  document.getElementById("loginScreen").style.display = "none";
  document.getElementById("cadastroScreen").style.display = "none";
  document.getElementById("dashboard").style.display = "none";

  document.getElementById(telaDestino).style.display = "block";
}

// FUNÇÃO DE CADASTRO (Com validações de campos e feedback visual)
async function fazerCadastro() {
  const usuario = document.getElementById("cadUsuario").value;
  const email = document.getElementById("cadEmail").value;
  const cpf = document.getElementById("cadCpf").value;
  const senha = document.getElementById("cadSenha").value;
  const tipoConta = document.getElementById("cadTipoConta").value;
  const msgCad = document.getElementById("cadastroMensagem");
  const btnCad = document.querySelector("#cadastroScreen button"); // Pega o botão de cadastro

  // Melhoria de UX: Estado de carregamento
  btnCad.disabled = true;
  btnCad.innerText = "Processando registro...";
  msgCad.innerText = "";

  try {
    const response = await fetch(`${API_URL}/Auth/register`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({
        Usuario: usuario,
        Email: email,
        Cpf: cpf,
        Senha: senha,
        TipoConta: tipoConta,
      }),
    });

    const data = await response.json();

    if (response.ok) {
      msgCad.className = "sucesso";
      msgCad.innerText =
        data.mensagem || "Conta criada com sucesso! Redirecionando...";
      setTimeout(() => {
        mudarTela("loginScreen");
        msgCad.innerText = "";
        // Limpa o formulário
        document.getElementById("cadUsuario").value = "";
        document.getElementById("cadEmail").value = "";
        document.getElementById("cadCpf").value = "";
        document.getElementById("cadSenha").value = "";
      }, 2500);
    } else {
      msgCad.className = "erro";
      msgCad.innerText = data.erro || "Erro ao criar conta.";
    }
  } catch (error) {
    msgCad.className = "erro";
    msgCad.innerText = "Erro ao conectar com o servidor.";
  } finally {
    // Reativa o botão após o término da requisição
    btnCad.disabled = false;
    btnCad.innerText = "Cadastrar";
  }
}

// FUNÇÃO DE LOGIN (Com feedback visual de carregamento)
async function fazerLogin() {
  const usuario = document.getElementById("loginUsuario").value;
  const senha = document.getElementById("loginSenha").value;
  const msgLogin = document.getElementById("loginMensagem");
  const btnLogin = document.querySelector("#loginScreen button");

  btnLogin.disabled = true;
  btnLogin.innerText = "Autenticando...";

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

      buscarDadosDashboard(); // Carrega todas as informações financeiras de uma vez
    } else {
      msgLogin.innerText = data.erro || "Usuário ou senha incorretos";
    }
  } catch (error) {
    msgLogin.innerText = "Erro ao conectar com o servidor.";
  } finally {
    btnLogin.disabled = false;
    btnLogin.innerText = "Entrar";
  }
}

// FUNÇÃO PARA ATUALIZAR O DASHBOARD INTEGRAL (Saldo, Limite e Cofrinho)
async function buscarDadosDashboard() {
  const token = localStorage.getItem("token_banco");

  try {
    // Faz a chamada para buscar os detalhes completos da conta logada
    const response = await fetch(`${API_URL}/Contas/detalhes`, {
      method: "GET",
      headers: { Authorization: `Bearer ${token}` },
    });

    if (response.ok) {
      const data = await response.json();

      // Atualiza todos os elementos visuais adicionados na interface do Dashboard
      document.getElementById("saldoAtual").innerText = data.saldo.toFixed(2);
      document.getElementById("limiteCartao").innerText =
        data.limiteCartao.toFixed(2);
      document.getElementById("saldoCofrinho").innerText =
        data.cofrinho.toFixed(2);

      // Se tiver uma tag para o tipo da conta, atualiza ela também
      const badge = document.getElementById("tipoContaBadge");
      if (badge) badge.innerText = `Conta ${data.tipo}`;
    }
  } catch (e) {
    console.error(
      "Não foi possível carregar os dados atualizados do dashboard.",
    );
  }
}

// FUNÇÃO DE TRANSAÇÃO PADRÃO (SAQUE/DEPÓSITO)
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

      buscarDadosDashboard(); // Mantém todo o dashboard sincronizado
    } else {
      msgBox.className = "erro";
      msgBox.innerText = data.erro || "Erro na transação.";
    }
  } catch (error) {
    msgBox.className = "erro";
    msgBox.innerText = "Erro de conexão com a API.";
  }
}

// NOVO: FUNÇÃO PARA CONTROLAR O COFRINHO (Guardar / Resgatar)
async function operarCofrinho(operacao) {
  const valor = parseFloat(document.getElementById("valorCofrinho").value);
  const token = localStorage.getItem("token_banco");
  const msgBox = document.getElementById("transacaoMensagem");

  if (!valor || valor <= 0) {
    msgBox.className = "erro";
    msgBox.innerText = "Insira um valor válido para o cofrinho.";
    return;
  }

  try {
    const response = await fetch(`${API_URL}/Cofrinho/${operacao}`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${token}`,
      },
      body: JSON.stringify({ Valor: valor }),
    });

    const data = await response.json();

    if (response.ok) {
      msgBox.className = "sucesso";
      msgBox.innerText = data.mensagem || "Cofrinho atualizado!";
      document.getElementById("valorCofrinho").value = "";

      buscarDadosDashboard();
    } else {
      msgBox.className = "erro";
      msgBox.innerText = data.erro || "Erro ao operar cofrinho.";
    }
  } catch (error) {
    msgBox.className = "erro";
    msgBox.innerText = "Erro de conexão ao acessar o cofrinho.";
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
    buscarDadosDashboard();
  } else {
    // SE NÃO HOUVER TOKEN, FORÇA EXIBIR APENAS A TELA DE LOGIN
    mudarTela("loginScreen");
  }
};
