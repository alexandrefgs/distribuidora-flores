import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CatalogoService } from '../../../core/services/catalogo.service';
import { Produto } from '../../../core/models/catalogo.models';
import { AuthService } from '../../../core/services/auth.service';
import { Router } from '@angular/router';
import { API_ROOT_URL } from '../../../core/config/api.config';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.css',
})
export class Dashboard implements OnInit {
  produtos = signal<Produto[]>([]);
  carregando = signal(true);
  apiRootUrl = API_ROOT_URL;

  // Formulário de novo produto
  nome = '';
  categoria = '';
  unidadeMedida = '';
  precoUnitario: number | null = null;
  criandoProduto = signal(false);
  erroFormulario = signal<string | null>(null);

  // Upload de imagem por produto (guarda qual produto está com upload em andamento)
  uploadEmAndamento = signal<string | null>(null);

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
    this.catalogoService.listarProdutos().subscribe({
      next: (produtos) => {
        this.produtos.set(produtos);
        this.carregando.set(false);
      },
      error: () => this.carregando.set(false),
    });
  }

  criarProduto(): void {
    if (!this.nome || !this.categoria || !this.unidadeMedida || !this.precoUnitario) {
      return;
    }

    this.erroFormulario.set(null);
    this.criandoProduto.set(true);

    this.catalogoService
      .criarProduto({
        nome: this.nome,
        categoria: this.categoria,
        unidadeMedida: this.unidadeMedida,
        precoUnitario: this.precoUnitario,
      })
      .subscribe({
        next: () => {
          this.criandoProduto.set(false);
          this.limparFormulario();
          this.carregarProdutos();
        },
        error: (err) => {
          this.criandoProduto.set(false);
          this.erroFormulario.set(err?.error?.erro ?? 'Não foi possível criar o produto.');
        },
      });
  }

  limparFormulario(): void {
    this.nome = '';
    this.categoria = '';
    this.unidadeMedida = '';
    this.precoUnitario = null;
  }

  aoSelecionarImagem(event: Event, produtoId: string): void {
    const input = event.target as HTMLInputElement;
    const arquivo = input.files?.[0];

    if (!arquivo) return;

    this.uploadEmAndamento.set(produtoId);

    this.catalogoService.enviarImagem(produtoId, arquivo).subscribe({
      next: () => {
        this.uploadEmAndamento.set(null);
        this.carregarProdutos();
      },
      error: () => {
        this.uploadEmAndamento.set(null);
      },
    });

    input.value = ''; // permite selecionar o mesmo arquivo de novo depois, se precisar
  }

  sair(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}