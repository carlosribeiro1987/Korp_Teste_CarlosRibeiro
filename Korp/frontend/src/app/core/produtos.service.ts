import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { API_CONFIG } from './api.config';
import { CriarProdutoRequest, Produto } from './models';

@Injectable({ providedIn: 'root' })
export class ProdutosService {
  private readonly baseUrl = `${API_CONFIG.estoqueUrl}/api/produtos`;

  constructor(private readonly http: HttpClient) {}

  listar() {
    return this.http.get<Produto[]>(this.baseUrl);
  }

  criar(request: CriarProdutoRequest) {
    return this.http.post<Produto>(this.baseUrl, request);
  }
}
