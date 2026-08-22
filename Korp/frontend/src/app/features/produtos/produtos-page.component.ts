import { CommonModule } from '@angular/common';
import { Component, inject, OnInit } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { finalize } from 'rxjs';
import { getErrorMessage } from '../../core/error-message';
import { Produto } from '../../core/models';
import { ProdutosService } from '../../core/produtos.service';

@Component({
  selector: 'app-produtos-page',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './produtos-page.component.html'
})
export class ProdutosPageComponent implements OnInit {
  private readonly fb = inject(FormBuilder);

  produtos: Produto[] = [];
  carregando = false;
  salvando = false;
  erro = '';
  sucesso = '';

  form = this.fb.nonNullable.group({
    codigo: ['', [Validators.required, Validators.maxLength(30)]],
    descricao: ['', [Validators.required, Validators.maxLength(120)]],
    saldo: [0, [Validators.required, Validators.min(0)]]
  });

  constructor(
    private readonly produtosService: ProdutosService
  ) {}

  ngOnInit(): void {
    this.carregar();
  }

  carregar(): void {
    this.carregando = true;
    this.erro = '';

    this.produtosService.listar()
      .pipe(finalize(() => this.carregando = false))
      .subscribe({
        next: produtos => this.produtos = produtos,
        error: error => this.erro = getErrorMessage(error)
      });
  }

  salvar(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.salvando = true;
    this.erro = '';
    this.sucesso = '';

    this.produtosService.criar(this.form.getRawValue())
      .pipe(finalize(() => this.salvando = false))
      .subscribe({
        next: produto => {
          this.produtos = [...this.produtos, produto].sort((a, b) => a.codigo.localeCompare(b.codigo));
          this.form.reset({ codigo: '', descricao: '', saldo: 0 });
          this.sucesso = 'Produto cadastrado.';
        },
        error: error => this.erro = getErrorMessage(error)
      });
  }
}
