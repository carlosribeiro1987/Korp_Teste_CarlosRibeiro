import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { finalize } from 'rxjs';
import { getErrorMessage } from '../../core/error-message';
import { NotaFiscal } from '../../core/models';
import { NotasFiscaisService } from '../../core/notas-fiscais.service';

@Component({
  selector: 'app-notas-page',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './notas-page.component.html'
})
export class NotasPageComponent implements OnInit {
  notas: NotaFiscal[] = [];
  carregando = false;
  criando = false;
  erro = '';

  constructor(
    private readonly notasService: NotasFiscaisService,
    private readonly router: Router
  ) {}

  ngOnInit(): void {
    this.carregar();
  }

  carregar(): void {
    this.carregando = true;
    this.erro = '';

    this.notasService.listar()
      .pipe(finalize(() => this.carregando = false))
      .subscribe({
        next: notas => this.notas = notas,
        error: error => this.erro = getErrorMessage(error)
      });
  }

  criar(): void {
    this.criando = true;
    this.erro = '';

    this.notasService.criar()
      .pipe(finalize(() => this.criando = false))
      .subscribe({
        next: nota => void this.router.navigate(['/notas', nota.id]),
        error: error => this.erro = getErrorMessage(error)
      });
  }
}
