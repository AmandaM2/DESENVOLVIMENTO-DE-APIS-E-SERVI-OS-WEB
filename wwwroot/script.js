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

// Função para alternar visualização da senha (ícone de olho)
function toggleSenha(idInput, icone) {
    const input = document.getElementById(idInput);
    if (!input) return;
    if (input.type === "password") {
        input.type = "text";
        icone.classList.add("ativo");
    } else {
        input.type = "password";
        icone.classList.remove("ativo");
    }
}

// FUNÇÃO DE CADASTRO
async function fazerCadastro() {
    const msgCad = document.getElementById("cadastroMensagem");
    const btnCad = document.querySelector("#cadastroScreen button");


    const usuario = document.getElementById("cadUsuario")?.value || "";
    const email = document.getElementById("cadEmail")?.value || "";
    const cpf = document.getElementById("cadCpf")?.value || "";
    const senha = document.getElementById("cadSenha")?.value || "";
    const confirmaSenha = document.getElementById("cadConfirmaSenha")?.value || "";
    const tipoConta = document.getElementById("cadTipoConta")?.value || "Corrente";
    const aceitouLgpd = document.getElementById("cadLgpd")?.checked || false;
    const erroSenha = document.getElementById("erroConfirmaSenha");
    if (erroSenha) erroSenha.innerText = "";
    if (!senha || !confirmaSenha) {
        if (erroSenha) {
            erroSenha.innerText = "Preencha e confirme a senha.";
        }
        return;
    }
    if (senha !== confirmaSenha) {
        if (erroSenha) {
            erroSenha.innerText = "As senhas não coincidem.";
        }
        return;
    }

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
    const corpoTabela = document.getElementById("corpoHistorico");

    if (!corpoTabela) return;

    corpoTabela.innerHTML = "";

    if (!transacoes || transacoes.length === 0) {
        corpoTabela.innerHTML = `
            <tr>
                <td colspan="3" style="text-align:center;">
                    Nenhuma transação realizada.
                </td>
            </tr>
        `;
        return;
    }

    transacoes
        .sort((a, b) =>
            new Date(b.dataHora || b.DataHora) -
            new Date(a.dataHora || a.DataHora)
        )
        .forEach((t) => {

            const dataOriginal = t.dataHora || t.DataHora;

            const dataBr = new Date(dataOriginal).toLocaleString("pt-BR", {
                day: "2-digit",
                month: "2-digit",
                year: "numeric",
                hour: "2-digit",
                minute: "2-digit"
            });

            const tipo =
                String(
                    t.tipoOperacao ||
                    t.TipoOperacao ||
                    t.tipo ||
                    ""
                ).toLowerCase();

            const valor = Number(
                t.valor ||
                t.Valor ||
                0
            );

            const ehSaque =
                tipo.includes("sacar") ||
                tipo.includes("resgatar");

            const classe = ehSaque
                ? "texto-erro"
                : "texto-sucesso";

            const simbolo = ehSaque ? "-" : "+";

            corpoTabela.innerHTML += `
                <tr>
                    <td>${dataBr}</td>

                    <td class="${classe}">
                        ${tipo.toUpperCase()}
                    </td>

                    <td class="${classe}">
                        ${simbolo} R$ ${valor.toFixed(2)}
                    </td>
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