export interface Produto {
  id: string;
  codigo: string;
  descricao: string;
  saldo: number;
  criadoEm: string;
  atualizadoEm: string;
}

export interface CriarProdutoRequest {
  codigo: string;
  descricao: string;
  saldo: number;
}

export interface NotaFiscal {
  id: string;
  numero: number;
  status: 'Aberta' | 'Fechada';
  criadaEm: string;
  fechadaEm: string | null;
  itens: ItemNotaFiscal[];
}

export interface ItemNotaFiscal {
  id?: string;
  itemId?: string;
  produtoId: string;
  codigoProduto: string;
  descricaoProduto: string;
  quantidade: number;
}

export interface AdicionarItemRequest {
  produtoId: string;
  codigoProduto: string;
  descricaoProduto: string;
  quantidade: number;
}
