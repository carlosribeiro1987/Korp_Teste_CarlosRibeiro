namespace Estoque.API.Exceptions {
    public sealed class EstoqueIndisponivelException : Exception {
        public EstoqueIndisponivelException()
            : base("Servico de Estoque indisponivel.") {
        }
    }
}
