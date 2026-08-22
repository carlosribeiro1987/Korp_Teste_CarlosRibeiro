import { bootstrapApplication } from '@angular/platform-browser';
import { provideHttpClient } from '@angular/common/http';
import { provideRouter, Routes } from '@angular/router';
import { AppComponent } from './app/app.component';
import { ProdutosPageComponent } from './app/features/produtos/produtos-page.component';
import { NotasPageComponent } from './app/features/notas/notas-page.component';
import { NotaDetalhePageComponent } from './app/features/notas/nota-detalhe-page.component';

const routes: Routes = [
  { path: '', redirectTo: 'produtos', pathMatch: 'full' },
  { path: 'produtos', component: ProdutosPageComponent },
  { path: 'notas', component: NotasPageComponent },
  { path: 'notas/:id', component: NotaDetalhePageComponent },
  { path: '**', redirectTo: 'produtos' }
];

bootstrapApplication(AppComponent, {
  providers: [
    provideHttpClient(),
    provideRouter(routes)
  ]
}).catch((err) => console.error(err));
