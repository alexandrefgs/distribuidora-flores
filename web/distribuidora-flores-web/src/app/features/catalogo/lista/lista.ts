import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CatalogoService } from '../../../core/services/catalogo.service';
import { Produto } from '../../../core/models/catalogo.models';
import { AuthService } from '../../../core/services/auth.service';
import { Router } from '@angular/router';
import { API_ROOT_URL } from '../../../core/config/api.config';

@Component({
  selector: 'app-lista',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './lista.html',
  styleUrl: './lista.css',
})
export class Lista implements OnInit {
  produtos = signal<Produto[]>([]);
  carregando = signal(true);
  erro = signal<string | null>(null);
  apiRootUrl = API_ROOT_URL;

  constructor(
    private catalogoService: CatalogoService,
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.carregarProdutos();
  }

  carregarProdutos(): void {
    this.carregando.set(true);
    this.erro.set(null);

    this.catalogoService.listarProdutos().subscribe({
      next: (produtos) => {
        this.produtos.set(produtos);
        this.carregando.set(false);
      },
      error: () => {
        this.erro.set('Não foi possível carregar o catálogo.');
        this.carregando.set(false);
      },
    });
  }

  sair(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}