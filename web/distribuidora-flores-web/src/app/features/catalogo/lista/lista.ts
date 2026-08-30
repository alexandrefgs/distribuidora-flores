import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { CatalogoService } from '../../../core/services/catalogo.service';
import { Produto } from '../../../core/models/catalogo.models';
import { AuthService } from '../../../core/services/auth.service';
import { CarrinhoService } from '../../../core/services/carrinho.service';
import { API_ROOT_URL } from '../../../core/config/api.config';

@Component({
  selector: 'app-lista',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './lista.html',
  styleUrl: './lista.css',
})
export class Lista implements OnInit {
  produtos = signal<Produto[]>([]);
  carregando = signal(true);
  erro = signal<string | null>(null);
  apiRootUrl = API_ROOT_URL;

  // Quantidade selecionada por produto (antes de adicionar ao carrinho)
  quantidades = signal<Record<string, number>>({});

  constructor(
    private catalogoService: CatalogoService,
    private authService: AuthService,
    private router: Router,
    public carrinhoService: CarrinhoService
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

        // Inicializa a quantidade de cada produto como 1
        const inicial: Record<string, number> = {};
        produtos.forEach((p) => (inicial[p.id] = 1));
        this.quantidades.set(inicial);

        this.carregando.set(false);
      },
      error: () => {
        this.erro.set('Não foi possível carregar o catálogo.');
        this.carregando.set(false);
      },
    });
  }

  obterQuantidade(produtoId: string): number {
    return this.quantidades()[produtoId] ?? 1;
  }

  definirQuantidade(produtoId: string, valor: number): void {
    this.quantidades.set({ ...this.quantidades(), [produtoId]: Math.max(1, valor) });
  }

  adicionarAoCarrinho(produto: Produto): void {
    const quantidade = this.obterQuantidade(produto.id);
    this.carrinhoService.adicionar(produto, quantidade);
    this.definirQuantidade(produto.id, 1);
  }

  irParaCarrinho(): void {
    this.router.navigate(['/carrinho']);
  }

  sair(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}