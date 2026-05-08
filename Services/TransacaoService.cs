using Api.DTOs;
using Api.Interfaces;
using Api.Models;
using Api.Repositories;

namespace Api.Services
{
    public class TransacaoService : ITransacaoService
    {
        private readonly ITransacaoRepository _transacaoRepository;
        private readonly IContaRepository _contaRepository;

        public TransacaoService(ITransacaoRepository transacaoRepository, IContaRepository contaRepository)
        {
            _transacaoRepository = transacaoRepository;
            _contaRepository = contaRepository;
        }

        public async Task<decimal> ObterSaldoAsync(int contaId)
        {
            var conta = await _contaRepository.GetByIdAsync(contaId);
            return conta?.Saldo ?? 0;
        }

        public async Task RealizarDeposito(TransacaoDTO dto)
        {
            var conta = await _contaRepository.GetByIdAsync(dto.ContaId);
            if (conta == null) throw new Exception("Conta não encontrada.");

            // Atualiza saldo
            conta.Saldo += dto.Valor;
            await _contaRepository.UpdateAsync(conta.Id, conta);

            // Registra a transação
            var transacao = new Transacao
            {
                ContaId = dto.ContaId,
                Valor = dto.Valor,
                DataHora = DateTime.Now,
                Tipo = TipoTransacao.Deposito
            };
            await _transacaoRepository.CreateAsync(transacao);
        }

        public async Task RealizarSaque(TransacaoDTO dto)
        {
            var conta = await _contaRepository.GetByIdAsync(dto.ContaId);
            if (conta == null) throw new Exception("Conta não encontrada.");

            // REGRA DE NEGÓCIO: Taxas baseadas no tipo
            decimal taxa = 0;
            if (conta.Tipo == "Corrente") taxa = 5.00m;
            else if (conta.Tipo == "Empresarial") taxa = 10.00m;
            // Poupança = 0.00m

            decimal valorTotalSaque = dto.Valor + taxa;

            // REGRA DE NEGÓCIO: Verificar saldo
            if (conta.Saldo < valorTotalSaque)
                throw new Exception($"Saldo insuficiente. Lembre-se da taxa de R$ {taxa} para saques.");

            // Desconta e salva
            conta.Saldo -= valorTotalSaque;
            await _contaRepository.UpdateAsync(conta.Id, conta);

            // Registra a transação
            var transacao = new Transacao
            {
                ContaId = dto.ContaId,
                Valor = dto.Valor,
                DataHora = DateTime.Now,
                Tipo = TipoTransacao.Saque
            };
            await _transacaoRepository.CreateAsync(transacao);
        }

    }
}
