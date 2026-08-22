namespace Estoque.API.Models {
    public sealed class Produto {
        public Guid Id { get; private set; }
        public string Codigo { get; private set; } = string.Empty;
        public string Descricao { get; private set; } = string.Empty;
        public int Saldo { get; private set; } 
        public DateTime CriadoEm { get; private set; }
        public DateTime ModificadoEm { get; private set; }

        private Produto() { }

        public Produto(string codigo, string descricao, int saldo) {
            if(string.IsNullOrWhiteSpace(codigo)) {
                throw new ArgumentException("Codigo e obrigatorio.", nameof(codigo));
            }

            if(string.IsNullOrWhiteSpace(descricao)) {
                throw new ArgumentException("Descricao e obrigatoria.", nameof(descricao));
            }

            if(saldo < 0) {
                throw new ArgumentException("Saldo nao pode ser negativo.", nameof(saldo));
            }

            Id = Guid.NewGuid();
            Codigo = codigo.Trim();
            Descricao = descricao.Trim();
            Saldo = saldo;
            CriadoEm = DateTime.UtcNow;
            ModificadoEm = CriadoEm;
        }

        public void ReduzirSaldo(int quantidade) {
            if(quantidade <= 0) {
                throw new InvalidOperationException("Quantidade deve ser maior que zero.");
            }
            if(quantidade > Saldo) {
                throw new InvalidOperationException("Saldo insuficiente.");
            }

            Saldo -= quantidade;
            ModificadoEm = DateTime.UtcNow;
        }

        public void AumentarSaldo(int quantidade) {
            if(quantidade <= 0) {
                throw new InvalidOperationException("Quantidade deve ser maior que zero.");
            }
            
            Saldo += quantidade;
            ModificadoEm = DateTime.UtcNow;
        }
    }
}
