namespace Faturamento.API.Exceptions {
    public sealed class EstoqueApiException  : Exception {
        public EstoqueApiException(string message)
        : base(message) {
        }
    }
}
