import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { API_CONFIG } from './api.config';
import { AdicionarItemRequest, NotaFiscal } from './models';

@Injectable({ providedIn: 'root' })
export class NotasFiscaisService {
  private readonly baseUrl = `${API_CONFIG.faturamentoUrl}/api/notas-fiscais`;

  constructor(private readonly http: HttpClient) {}

  listar() {
    return this.http.get<NotaFiscal[]>(this.baseUrl);
  }

  obter(id: string) {
    return this.http.get<NotaFiscal>(`${this.baseUrl}/${id}`);
  }

  criar() {
    return this.http.post<NotaFiscal>(this.baseUrl, null);
  }

  adicionarItem(notaId: string, request: AdicionarItemRequest) {
    return this.http.post<NotaFiscal>(`${this.baseUrl}/${notaId}/itens`, request);
  }

  removerItem(notaId: string, itemId: string) {
    return this.http.delete<NotaFiscal>(`${this.baseUrl}/${notaId}/itens/${itemId}`);
  }

  imprimir(notaId: string) {
    return this.http.post<NotaFiscal>(`${this.baseUrl}/${notaId}/imprimir`, null);
  }
}
