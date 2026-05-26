const API_URL = "http://localhost:5250/Api";
let meuGrafico = null; // Controle global do gráfico

// FUNÇÃO PARA NAVEGAR ENTRE AS TELAS (Com travas de segurança)
function mudarTela(telaDestino) {
  const login = document.getElementById("loginScreen");
  const cadastro = document.getElementById("cadastroScreen");
  const dash = document.getElementById("dashboard");

  if (login) login.style.display = "none";
  if (cadastro) cadastro.style.display = "none";
  if (dash) dash.style.display = "none";

  const destino = document.getElementById(telaDestino);
  if (destino) destino.style.display = "block";
}

// FUNÇÃO DE CADASTRO
async function fazerCadastro() {
  const msgCad = document.getElementById("cadastroMensagem");
  const btnCad = document.querySelector("#cadastroScreen button");

  const usuario = document.getElementById("cadUsuario")?.value || "";
  const email = document.getElementById("cadEmail")?.value || "";
  const cpf = document.getElementById("cadCpf")?.value || "";
  const senha = document.getElementById("cadSenha")?.value || "";
  const tipoConta =
    document.getElementById("cadTipoConta")?.value || "Corrente";
  const aceitouLgpd = document.getElementById("cadLgpd")?.checked || false;

  if (!aceitouLgpd) {
    if (msgCad) {
      msgCad.className = "erro";
      msgCad.innerText =
        "Você precisa aceitar os termos da LGPD para continuar.";
    }
    return;
  }

  if (btnCad) {
    btnCad.disabled = true;
    btnCad.innerText = "Processando registro...";
  }
  if (msgCad) msgCad.innerText = "";

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
        AceitouLgpd: aceitouLgpd,
      }),
    });

    const data = await response.json();

    if (response.ok) {
      if (msgCad) {
        msgCad.className = "sucesso";
        msgCad.innerText =
          data.mensagem || "Conta criada com sucesso! Redirecionando...";
      }
      setTimeout(() => {
        mudarTela("loginScreen");
        if (msgCad) msgCad.innerText = "";

        // Limpa campos com segurança
        if (document.getElementById("cadUsuario"))
          document.getElementById("cadUsuario").value = "";
        if (document.getElementById("cadEmail"))
          document.getElementById("cadEmail").value = "";
        if (document.getElementById("cadCpf"))
          document.getElementById("cadCpf").value = "";
        if (document.getElementById("cadSenha"))
          document.getElementById("cadSenha").value = "";
        if (document.getElementById("cadLgpd"))
          document.getElementById("cadLgpd").checked = false;
      }, 2500);
    } else {
      if (msgCad) {
        msgCad.className = "erro";
        msgCad.innerText = data.erro || "Erro ao criar conta.";
      }
    }
  } catch (error) {
    if (msgCad) {
      msgCad.className = "erro";
      msgCad.innerText = "Erro ao conectar com o servidor.";
    }
  } finally {
    if (btnCad) {
      btnCad.disabled = false;
      btnCad.innerText = "Cadastrar";
    }
  }
}

// FUNÇÃO DE LOGIN
async function fazerLogin() {
  const usuario = document.getElementById("loginUsuario")?.value || "";
  const senha = document.getElementById("loginSenha")?.value || "";
  const msgLogin = document.getElementById("loginMensagem");
  const btnLogin = document.querySelector("#loginScreen button");

  if (btnLogin) {
    btnLogin.disabled = true;
    btnLogin.innerText = "Autenticando...";
  }

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

      const txtUsuario = document.getElementById("nomeUsuario");
      if (txtUsuario) txtUsuario.innerText = usuario;

      mudarTela("dashboard");
      buscarDadosDashboard();
    } else {
      if (msgLogin)
        msgLogin.innerText = data.erro || "Usuário ou senha incorretos";
    }
  } catch (error) {
    if (msgLogin) msgLogin.innerText = "Erro ao conectar com o servidor.";
  } finally {
    if (btnLogin) {
      btnLogin.disabled = false;
      btnLogin.innerText = "Entrar";
    }
  }
}

// FUNÇÃO PARA ATUALIZAR O DASHBOARD
async function buscarDadosDashboard() {
  const token = localStorage.getItem("token_banco");
  if (!token) return;

  try {
    const response = await fetch(`${API_URL}/Contas/detalhes`, {
      method: "GET",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${token}`,
      },
    });

    if (response.ok) {
      const data = await response.json();
      console.log(data);

      // Atualiza elementos se eles existirem na tela
      const elSaldo = document.getElementById("saldoAtual");
      const elLimite = document.getElementById("limiteCartao");
      const elCofrinho = document.getElementById("saldoCofrinho");
      const elBadge = document.getElementById("tipoContaBadge");

      if (elSaldo) elSaldo.innerText = data.saldo.toFixed(2);
      if (elLimite) elLimite.innerText = data.limiteCartao.toFixed(2);
      if (elCofrinho) elCofrinho.innerText = data.cofrinho.toFixed(2);
      if (elBadge) elBadge.innerText = `Conta ${data.tipo}`;

      if (data.transacoes) {
        atualizarTabelaHistorico(data.transacoes);
        gerarGraficoFinancas(data.transacoes);
      }
    }
  } catch (e) {
    console.error("Erro ao carregar dados do dashboard.");
  }
}

// FUNÇÃO DO HISTÓRICO
function atualizarTabelaHistorico(transacoes) {
  const corpoTabela = document.getElementById("corpoHistorico");
  if (!corpoTabela) return;

  corpoTabela.innerHTML = "";

  if (!transacoes || transacoes.length === 0) {
    corpoTabela.innerHTML = `<tr><td colspan="3" style="text-align:center;">Nenhuma transação realizada.</td></tr>`;
    return;
  }

  transacoes
    .slice(-5)
    .reverse()
    .forEach((t) => {
      const dataBr = new Date(t.dataHora || t.data).toLocaleDateString("pt-BR");
      const tipo = t.tipoOperacao || t.tipo || "";
      const classeEstilo =
        tipo.toLowerCase() === "sacar" ? "texto-erro" : "texto-sucesso";

      corpoTabela.innerHTML += `
      <tr>
        <td>${dataBr}</td>
        <td class="${classeEstilo}">${tipo.toUpperCase()}</td>
        <td>R$ ${t.valor.toFixed(2)}</td>
      </tr>
    `;
    });
}

// FUNÇÃO DO GRÁFICO
function gerarGraficoFinancas(transacoes) {
  const canvas = document.getElementById("graficoFinancas");
  if (!canvas || typeof Chart === "undefined") return; // Segurança caso Chart.js não tenha carregado

  const ctx = canvas.getContext("2d");
  const ultimasSeis = transacoes.slice(-6);
  const labelsDatas = ultimasSeis.map((t) =>
    new Date(t.dataHora || t.data).toLocaleDateString("pt-BR"),
  );
  const valoresEvolucao = ultimasSeis.map((t) => t.valor);

  if (meuGrafico) {
    meuGrafico.destroy();
  }

  meuGrafico = new Chart(ctx, {
    type: "line",
    data: {
      labels: labelsDatas.length ? labelsDatas : ["Sem dados"],
      datasets: [
        {
          label: "Volume de Movimentações (R$)",
          data: valoresEvolucao.length ? valoresEvolucao : [0],
          borderColor: "#4CAF50",
          backgroundColor: "rgba(76, 175, 80, 0.1)",
          borderWidth: 2.5,
          tension: 0.3,
        },
      ],
    },
    options: {
      responsive: true,
      maintainAspectRatio: false,
      scales: { y: { beginAtZero: true } },
    },
  });
}

// FUNÇÃO DE MOVIMENTAÇÃO (SAQUE/DEPÓSITO)
async function realizarTransacao(tipo) {
  const valorInput = document.getElementById("valorTransacao");
  const valor = parseFloat(valorInput?.value || "0");
  const token = localStorage.getItem("token_banco");
  const msgBox = document.getElementById("transacaoMensagem");

  if (!valor || valor <= 0) {
    if (msgBox) {
      msgBox.className = "erro";
      msgBox.innerText = "Insira um valor válido.";
    }
    return;
  }

  try {
    const response = await fetch(`${API_URL}/Transacoes/${tipo}`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${token}`,
      },
      body: JSON.stringify({ Valor: valor, TipoOperacao: tipo }),
    });

    const data = await response.json();
    if (response.ok) {
      if (msgBox) {
        msgBox.className = "sucesso";
        msgBox.innerText = data.mensagem || "Operação realizada com sucesso.";
      }
      if (valorInput) valorInput.value = "";
      await buscarDadosDashboard();
      console.log("foi");
    } else {
      if (msgBox) {
        msgBox.className = "erro";
        msgBox.innerText = data.erro || "Erro na transação.";
      }
    }
  } catch (error) {
    if (msgBox) {
      msgBox.className = "erro";
      msgBox.innerText = "Erro de conexão com a API.";
    }
  }
}

// FUNÇÃO DO COFRINHO
async function operarCofrinho(operacao) {
  const valorInput = document.getElementById("valorCofrinho");
  const valor = parseFloat(valorInput?.value || "0");
  const token = localStorage.getItem("token_banco");
  const msgBox = document.getElementById("transacaoMensagem");

  if (!valor || valor <= 0) {
    if (msgBox) {
      msgBox.className = "erro";
      msgBox.innerText = "Insira um valor válido para o cofrinho.";
    }
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
      if (msgBox) {
        msgBox.className = "sucesso";
        msgBox.innerText = data.mensagem || "Cofrinho atualizado!";
      }
      if (valorInput) valorInput.value = "";
      buscarDadosDashboard();
    } else {
      if (msgBox) {
        msgBox.className = "erro";
        msgBox.innerText = data.erro || "Erro ao operar cofrinho.";
      }
    }
  } catch (error) {
    if (msgBox) {
      msgBox.className = "erro";
      msgBox.innerText = "Erro de conexão ao acessar o cofrinho.";
    }
  }
}

// FUNÇÃO DE LOGOUT
function logout() {
  localStorage.removeItem("token_banco");
  localStorage.removeItem("usuario_banco");

  if (document.getElementById("loginUsuario"))
    document.getElementById("loginUsuario").value = "";
  if (document.getElementById("loginSenha"))
    document.getElementById("loginSenha").value = "";
  if (document.getElementById("transacaoMensagem"))
    document.getElementById("transacaoMensagem").innerText = "";

  if (meuGrafico) {
    meuGrafico.destroy();
    meuGrafico = null;
  }

  mudarTela("loginScreen");
}

// VERIFICAÇÃO DE SESSÃO ATIVA AO RECARREGAR
window.onload = function () {
  const tokenSalvo = localStorage.getItem("token_banco");
  const usuarioSalvo = localStorage.getItem("usuario_banco");

  if (tokenSalvo && usuarioSalvo) {
    const txtUsuario = document.getElementById("nomeUsuario");
    if (txtUsuario) txtUsuario.innerText = usuarioSalvo;
    mudarTela("dashboard");
    buscarDadosDashboard();
  } else {
    mudarTela("loginScreen");
  }
};
