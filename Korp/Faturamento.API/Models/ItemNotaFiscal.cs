namespace Faturamento.API.Models {
    public sealed class ItemNotaFiscal {
        public Guid ItemId { get; private set; }
        public Guid NotaFiscalId { get; private set; }
        public Guid ProdutoId { get; private set; }
        public string CodigoProduto { get; private set; }
        public string DescricaoProduto { get; private set; }
        public int Quantidade { get; private set; }

        private ItemNotaFiscal() {
            CodigoProduto = string.Empty;
            DescricaoProduto = string.Empty;
        }

        public ItemNotaFiscal(Guid notaFiscalId, Guid produtoId, string codigoProduto, string descricaoProduto, int quantidade) {
            ItemId = Guid.NewGuid();
            NotaFiscalId = notaFiscalId;
            ProdutoId = produtoId;
            CodigoProduto = codigoProduto;
            DescricaoProduto = descricaoProduto;
            Quantidade = quantidade;
        }
    }
}
