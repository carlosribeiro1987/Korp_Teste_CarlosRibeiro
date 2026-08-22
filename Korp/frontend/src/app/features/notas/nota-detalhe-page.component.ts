import { CommonModule } from '@angular/common';
import { Component, inject, OnInit } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { finalize } from 'rxjs';
import { getErrorMessage } from '../../core/error-message';
import { ItemNotaFiscal, NotaFiscal, Produto } from '../../core/models';
import { NotasFiscaisService } from '../../core/notas-fiscais.service';
import { ProdutosService } from '../../core/produtos.service';

@Component({
  selector: 'app-nota-detalhe-page',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './nota-detalhe-page.component.html'
})
export class NotaDetalhePageComponent implements OnInit {
  private readonly fb = inject(FormBuilder);

  nota: NotaFiscal | null = null;
  produtos: Produto[] = [];
  carregando = false;
  salvandoItem = false;
  removendoItemId = '';
  imprimindo = false;
  erro = '';
  sucesso = '';

  form = this.fb.nonNullable.group({
    produtoId: ['', Validators.required],
    quantidade: [1, [Validators.required, Validators.min(1)]]
  });

  constructor(
    private readonly route: ActivatedRoute,
    private readonly notasService: NotasFiscaisService,
    private readonly produtosService: ProdutosService
  ) {}

  ngOnInit(): void {
    this.carregar();
  }

  get notaId(): string {
    return this.route.snapshot.paramMap.get('id') ?? '';
  }

  get notaFechada(): boolean {
    return this.nota?.status === 'Fechada';
  }

  carregar(): void {
    this.carregando = true;
    this.erro = '';

    this.notasService.obter(this.notaId)
      .pipe(finalize(() => this.carregando = false))
      .subscribe({
        next: nota => this.nota = nota,
        error: error => this.erro = getErrorMessage(error)
      });

    this.produtosService.listar().subscribe({
      next: produtos => this.produtos = produtos,
      error: error => this.erro = getErrorMessage(error)
    });
  }

  adicionarItem(): void {
    if (!this.nota || this.form.invalid || this.notaFechada) {
      this.form.markAllAsTouched();
      return;
    }

    const value = this.form.getRawValue();
    const produto = this.produtos.find(p => p.id === value.produtoId);
    if (!produto) {
      this.erro = 'Selecione um produto.';
      return;
    }

    this.salvandoItem = true;
    this.erro = '';
    this.sucesso = '';

    this.notasService.adicionarItem(this.nota.id, {
      produtoId: produto.id,
      codigoProduto: produto.codigo,
      descricaoProduto: produto.descricao,
      quantidade: value.quantidade
    })
      .pipe(finalize(() => this.salvandoItem = false))
      .subscribe({
        next: nota => {
          this.nota = nota;
          this.form.reset({ produtoId: '', quantidade: 1 });
        },
        error: error => this.erro = getErrorMessage(error)
      });
  }

  removerItem(item: ItemNotaFiscal): void {
    const itemId = item.itemId ?? item.id;
    if (!this.nota || !itemId || this.notaFechada) {
      return;
    }

    this.removendoItemId = itemId;
    this.erro = '';
    this.sucesso = '';

    this.notasService.removerItem(this.nota.id, itemId)
      .pipe(finalize(() => this.removendoItemId = ''))
      .subscribe({
        next: nota => this.nota = nota,
        error: error => this.erro = getErrorMessage(error)
      });
  }

  imprimir(): void {
    if (!this.nota || this.notaFechada) {
      return;
    }

    this.imprimindo = true;
    this.erro = '';
    this.sucesso = '';

    this.notasService.imprimir(this.nota.id)
      .pipe(finalize(() => this.imprimindo = false))
      .subscribe({
        next: nota => {
          this.nota = nota;
          this.sucesso = 'Nota impressa e fechada.';
        },
        error: error => this.erro = getErrorMessage(error)
      });
  }
}
