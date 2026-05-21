using Api.DTOs;
using Api.Interfaces;
using Api.Models;
using System;
using System.Threading.Tasks;

namespace Api.Services
{
    public class ContaService : IContaService
    {
        private readonly IContaRepository _contaRepository;
        private readonly ITransacaoRepository _transacaoRepository;

        // Injetando os repositórios necessários para manipular Contas e Transações
        public ContaService(IContaRepository contaRepository, ITransacaoRepository transacaoRepository)
        {
            _contaRepository = contaRepository;
            _transacaoRepository = transacaoRepository;
        }

        public async Task<Conta?> BuscarPorId(int contaId)
        {
            return await _contaRepository.GetByIdAsync(contaId);
        }

        public async Task<Conta> CriarConta(ContaCreateDTO dto)
        {
            // 1. Instancia o novo Usuário com os dados vindos do DTO
            var novoUsuario = new Usuario
            {
                Nome = dto.Titular,
                Senha = dto.Senha,
                Email = "" // Iniciado vazio por padrão
            };

            // 2. Instancia a Conta vinculando o objeto do Usuário nela
            var novaConta = new Conta
            {
                Tipo = dto.Tipo,
                Saldo = dto.Saldo,
                Usuario = novoUsuario
            };

            // 3. Salva a estrutura no banco através do repositório
            return await _contaRepository.CreateAsync(novaConta);
        }

        public async Task<decimal> ObterSaldoAsync(int contaId)
        {
            var conta = await _contaRepository.GetByIdAsync(contaId);
            if (conta == null) throw new Exception("Conta não encontrada.");

            return conta.Saldo;
        }

        public async Task<bool> DepositarAsync(TransacaoDTO transacaoDto)
        {
            var conta = await _contaRepository.GetByIdAsync(transacaoDto.ContaId);
            if (conta == null) throw new Exception("Conta não encontrada para depósito.");

            if (transacaoDto.Valor <= 0) throw new Exception("O valor do depósito deve ser maior que zero.");

            // 1. Atualiza o saldo da conta
            conta.Saldo += transacaoDto.Valor;
            await _contaRepository.UpdateAsync(conta.Id, conta);

            // 2. Registra o histórico da operação
            var transacao = new Transacao
            {
                ContaId = conta.Id,
                Valor = transacaoDto.Valor,
                DataHora = DateTime.Now,
                Tipo = TipoTransacao.Deposito
            };
            await _transacaoRepository.CreateAsync(transacao);

            return true;
        }

        public async Task<bool> SacarAsync(TransacaoDTO transacaoDto)
        {
            var conta = await _contaRepository.GetByIdAsync(transacaoDto.ContaId);
            if (conta == null) throw new Exception("Conta não encontrada para saque.");

            if (transacaoDto.Valor <= 0) throw new Exception("O valor do saque deve ser maior que zero.");

            // Regra de Negócio: Definição de taxas por tipo de conta
            decimal taxa = 0;
            if (conta.Tipo == "Corrente") taxa = 5.00m;
            else if (conta.Tipo == "Empresarial") taxa = 10.00m;

            decimal valorTotalSaque = transacaoDto.Valor + taxa;

            // Regra de Negócio: Validação de fundos antes de efetuar o saque
            if (conta.Saldo < valorTotalSaque)
            {
                throw new Exception($"Saldo insuficiente. O saque de R$ {transacaoDto.Valor} exige saldo de R$ {valorTotalSaque} devido à taxa de R$ {taxa} para contas {conta.Tipo}.");
            }

            // 1. Deduz o valor total do saldo da conta
            conta.Saldo -= valorTotalSaque;
            await _contaRepository.UpdateAsync(conta.Id, conta);

            // 2. Registra o histórico do saque
            var transacao = new Transacao
            {
                ContaId = conta.Id,
                Valor = transacaoDto.Valor,
                DataHora = DateTime.Now,
                Tipo = TipoTransacao.Saque
            };
            await _transacaoRepository.CreateAsync(transacao);

            return true;
        }
    }
}
