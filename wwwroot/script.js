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
    const tipoConta = document.getElementById("cadTipoConta")?.value || "Corrente";
    const aceitouLgpd = document.getElementById("cadLgpd")?.checked || false;

    if (!aceitouLgpd) {
        if (msgCad) {
            msgCad.className = "erro";
            msgCad.innerText = "Você precisa aceitar os termos da LGPD para continuar.";
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
                msgCad.innerText = data.mensagem || "Conta criada com sucesso! Redirecionando...";
            }
            setTimeout(() => {
                mudarTela("loginScreen");
                if (msgCad) msgCad.innerText = "";

                // Limpa campos com segurança
                if (document.getElementById("cadUsuario")) document.getElementById("cadUsuario").value = "";
                if (document.getElementById("cadEmail")) document.getElementById("cadEmail").value = "";
                if (document.getElementById("cadCpf")) document.getElementById("cadCpf").value = "";
                if (document.getElementById("cadSenha")) document.getElementById("cadSenha").value = "";
                if (document.getElementById("cadLgpd")) document.getElementById("cadLgpd").checked = false;
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
            if (msgLogin) msgLogin.innerText = data.erro || "Usuário ou senha incorretos";
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

            // Tratamento anti-erro para aceitar maiúsculas e minúsculas vindas do C#
            const saldo = data.saldo !== undefined ? data.saldo : (data.Saldo || 0);
            const limite = data.limiteCartao !== undefined ? data.limiteCartao : (data.LimiteCartao || 0);
            const cofrinho = data.cofrinho !== undefined ? data.cofrinho : (data.Cofrinho || 0);
            const tipo = data.tipo !== undefined ? data.tipo : (data.Tipo || "Corrente");
            const listaTransacoes = data.transacoes || data.Transacoes || [];

            // Atualiza elementos se eles existirem na tela
            const elSaldo = document.getElementById("saldoAtual");
            const elLimite = document.getElementById("limiteCartao");
            const elCofrinho = document.getElementById("saldoCofrinho");
            const elBadge = document.getElementById("tipoContaBadge");

            if (elSaldo) elSaldo.innerText = Number(saldo).toFixed(2);
            if (elLimite) elLimite.innerText = Number(limite).toFixed(2);
            if (elCofrinho) elCofrinho.innerText = Number(cofrinho).toFixed(2);
            if (elBadge) elBadge.innerText = `Conta ${tipo}`;

            // Atualiza o Histórico
            atualizarTabelaHistorico(listaTransacoes);
        }
    } catch (e) {
        console.error("Erro ao carregar dados do dashboard.", e);
    }
}

// FUNÇÃO DO HISTÓRICO
// FUNÇÃO DO HISTÓRICO (Versão Corrigida Anti-Enum C#)
function atualizarTabelaHistorico(transacoes) {
    const cuerpoTabela = document.getElementById("corpoHistorico");
    if (!cuerpoTabela) return;

    cuerpoTabela.innerHTML = "";

    if (!transacoes || transacoes.length === 0) {
        cuerpoTabela.innerHTML = `<tr><td colspan="3" style="text-align:center;">Nenhuma transação realizada.</td></tr>`;
        return;
    }

    transacoes
        .slice(-5)
        .reverse()
        .forEach((t) => {
            const dataBr = new Date(t.dataHora || t.data || t.DataHora || t.Data).toLocaleDateString("pt-BR");

            // 1. Captura o tipo e força virar Texto (String) para evitar o erro toLowerCase()
            const tipoBruto = t.tipoOperacao !== undefined ? t.tipoOperacao : (t.tipo || t.TipoOperacao || t.Tipo || "");
            const tipoStr = String(tipoBruto).toLowerCase();

            // 2. Define a cor: se for o texto "sacar" ou o número "1" (Enum comum para saque)
            const classeEstilo = (tipoStr === "sacar" || tipoStr === "1") ? "texto-erro" : "texto-sucesso";

            // 3. Traduz o número do Enum para um texto amigável na tabela
            let tipoExibicao = tipoStr.toUpperCase();
            if (tipoStr === "1" || tipoStr === "sacar") tipoExibicao = "SACAR";
            if (tipoStr === "0" || tipoStr === "depositar") tipoExibicao = "DEPOSITAR";

            const valorTransacao = t.valor !== undefined ? t.valor : (t.Valor || 0);

            cuerpoTabela.innerHTML += `
      <tr>
        <td>${dataBr}</td>
        <td class="${classeEstilo}">${tipoExibicao}</td>
        <td>R$ ${Number(valorTransacao).toFixed(2)}</td>
      </tr>
    `;
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
            body: JSON.stringify({ valor: valor }), // Usando 'valor' minúsculo para evitar Erro 400
        });

        const data = await response.json();

        if (response.ok) {
            if (msgBox) {
                msgBox.className = "sucesso";
                msgBox.innerText = data.mensagem || "Cofrinho atualizado!";
            }
            if (valorInput) valorInput.value = "";
            await buscarDadosDashboard();
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

    if (document.getElementById("loginUsuario")) document.getElementById("loginUsuario").value = "";
    if (document.getElementById("loginSenha")) document.getElementById("loginSenha").value = "";
    if (document.getElementById("transacaoMensagem")) document.getElementById("transacaoMensagem").innerText = "";

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