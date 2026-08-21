namespace Faturamento.API.Exceptions {
    public sealed class EstoqueIndisponivelException : Exception {
        public EstoqueIndisponivelException(string message) 
            : base(message) { 
        }

        public EstoqueIndisponivelException(string message, Exception innerException) 
            : base(message, innerException) {
        }
    }
}
