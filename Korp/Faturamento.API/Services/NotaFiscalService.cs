using Faturamento.API.Clients.Interfaces;
using Faturamento.API.Data;
using Faturamento.API.DTOs;
using Faturamento.API.Enums;
using Faturamento.API.Models;
using Faturamento.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Faturamento.API.Services {
    public sealed class NotaFiscalService : INotaFiscalService {
        private readonly FaturamentoDbContext _dbContext;
        private readonly IEstoqueClient _estoqueClient;

        public NotaFiscalService(FaturamentoDbContext dbContext, IEstoqueClient estoqueClient) {
            _dbContext = dbContext;
            _estoqueClient = estoqueClient;
        }


        public async Task<IReadOnlyList<NotaFiscalResponseDto>> ListarAsync(CancellationToken cancellationToken) {
            var result = await _dbContext.NotasFiscais
                                         .AsNoTracking()
                                         .Include(nota => nota.Itens)
                                         .OrderByDescending(nota => nota.Numero)
                                         .Select(nota => NotaFiscalResponseDto.FromModel(nota))
                                         .ToListAsync(cancellationToken);
            return result;
        }

        public async Task<NotaFiscalResponseDto?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken) {
            var nota = await _dbContext.NotasFiscais
                                       .AsNoTracking()
                                       .Include(nf => nf.Itens)
                                       .FirstOrDefaultAsync(nf => nf.NotaFiscalId == id, cancellationToken);
            return nota is null ? null : NotaFiscalResponseDto.FromModel(nota);
        }

        public async Task<NotaFiscalResponseDto> CriarAsync(CancellationToken cancellationToken) {
            var ultimoNumero = await _dbContext.NotasFiscais.MaxAsync(nota => (int?)nota.Numero, cancellationToken) ?? 0;
            var nota = new NotaFiscal(ultimoNumero + 1);

            _dbContext.NotasFiscais.Add(nota);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return NotaFiscalResponseDto.FromModel(nota);
        }

        public async Task<NotaFiscalResponseDto> AdicionarItemAsync(Guid notaId, AdicionarItemRequestDto request, 
                                                                    CancellationToken cancellationToken) {
            var nota = await _dbContext.NotasFiscais
                                       .Include(n => n.Itens)
                                       .FirstOrDefaultAsync(n => n.NotaFiscalId == notaId, cancellationToken);
            if(nota is null) {
                throw new KeyNotFoundException("Nota fiscal nao encontrada.");
            }
            nota.AdicionarItem(request.ProdutoId, request.CodigoProduto, request.DescricaoProduto, request.Quantidade);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return NotaFiscalResponseDto.FromModel(nota);
        }

        public async Task<NotaFiscalResponseDto> RemoverItemAsync(Guid notaId, Guid itemId, CancellationToken cancellationToken) {
            var nota = await _dbContext.NotasFiscais
                                       .Include(n => n.Itens)
                                       .FirstOrDefaultAsync(n => n.NotaFiscalId == notaId, cancellationToken);
            if(nota is null) {
                throw new KeyNotFoundException("Nota fiscal nao encontrada.");
            }
            nota.RemoverItem(itemId);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return NotaFiscalResponseDto.FromModel(nota);
        }

        public async Task<NotaFiscalResponseDto> ImprimirAsync(Guid notaId, CancellationToken cancellationToken) {
            var nota = await _dbContext.NotasFiscais
                                       .Include(n => n.Itens)
                                       .FirstOrDefaultAsync(n => n.NotaFiscalId == notaId, cancellationToken);
            if(nota is null) {
                throw new KeyNotFoundException("Nota fiscal nao encontrada.");
            }
            if(nota.Status != StatusNotaFiscal.Aberta) {
                throw new InvalidOperationException("Apenas notas abertas podem ser impressas.");
            }
            foreach(var item in nota.Itens) {
                await _estoqueClient.ReduzirSaldoAsync(item.ProdutoId, item.Quantidade, cancellationToken);
            }

            nota.Fechar();
            await _dbContext.SaveChangesAsync(cancellationToken);
            return NotaFiscalResponseDto.FromModel(nota);
        }
                
    }
}
