import { HttpErrorResponse } from '@angular/common/http';

export function getErrorMessage(error: unknown): string {
  if (error instanceof HttpErrorResponse) {
    const body = error.error as { mensagem?: string } | null;
    return body?.mensagem ?? 'Nao foi possivel concluir a operacao.';
  }

  return 'Nao foi possivel concluir a operacao.';
}
