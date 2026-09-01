import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CatalogoService } from '../../../core/services/catalogo.service';
import { Produto } from '../../../core/models/catalogo.models';
import { AuthService } from '../../../core/services/auth.service';
import { Router } from '@angular/router';
import { API_ROOT_URL } from '../../../core/config/api.config';
import { DatepickerDirective } from '../../../shared/directives/datepicker.directive';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, FormsModule, DatepickerDirective],
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

  // Upload de imagem por produto
  uploadEmAndamento = signal<string | null>(null);

  // Formulário de lote (estoque)
  produtoComFormLoteAberto = signal<string | null>(null);
  quantidadeLote: number | null = null;
  dataValidadeLote = '';
  adicionandoLote = signal(false);
  erroLote = signal<string | null>(null);

  // Edição de produto
  produtoEmEdicao = signal<string | null>(null);
  nomeEdicao = '';
  categoriaEdicao = '';
  unidadeMedidaEdicao = '';
  precoUnitarioEdicao: number | null = null;
  salvandoEdicao = signal(false);
  erroEdicao = signal<string | null>(null);

  // Exclusão de produto
  produtoParaExcluir = signal<string | null>(null);
  excluindo = signal(false);

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

    input.value = '';
  }

  abrirFormLote(produtoId: string): void {
    this.produtoComFormLoteAberto.set(produtoId);
    this.quantidadeLote = null;
    this.dataValidadeLote = '';
    this.erroLote.set(null);
  }

  fecharFormLote(): void {
    this.produtoComFormLoteAberto.set(null);
  }

  adicionarLote(produtoId: string): void {
    if (!this.quantidadeLote || !this.dataValidadeLote) {
      return;
    }

    this.erroLote.set(null);
    this.adicionandoLote.set(true);

    this.catalogoService
      .adicionarLote(produtoId, {
        quantidade: this.quantidadeLote,
        dataValidade: new Date(this.dataValidadeLote).toISOString(),
      })
      .subscribe({
        next: () => {
          this.adicionandoLote.set(false);
          this.fecharFormLote();
          this.carregarProdutos();
        },
        error: (err) => {
          this.adicionandoLote.set(false);
          this.erroLote.set(err?.error?.erro ?? 'Não foi possível adicionar o estoque.');
        },
      });
  }

  abrirEdicao(produto: Produto): void {
    this.produtoEmEdicao.set(produto.id);
    this.nomeEdicao = produto.nome;
    this.categoriaEdicao = produto.categoria;
    this.unidadeMedidaEdicao = produto.unidadeMedida;
    this.precoUnitarioEdicao = produto.precoUnitario;
    this.erroEdicao.set(null);
  }

  fecharEdicao(): void {
    this.produtoEmEdicao.set(null);
  }

  salvarEdicao(produtoId: string): void {
    if (!this.nomeEdicao || !this.categoriaEdicao || !this.unidadeMedidaEdicao || !this.precoUnitarioEdicao) {
      return;
    }

    this.erroEdicao.set(null);
    this.salvandoEdicao.set(true);

    this.catalogoService
      .atualizarProduto(produtoId, {
        nome: this.nomeEdicao,
        categoria: this.categoriaEdicao,
        unidadeMedida: this.unidadeMedidaEdicao,
        precoUnitario: this.precoUnitarioEdicao,
      })
      .subscribe({
        next: () => {
          this.salvandoEdicao.set(false);
          this.fecharEdicao();
          this.carregarProdutos();
        },
        error: (err) => {
          this.salvandoEdicao.set(false);
          this.erroEdicao.set(err?.error?.erro ?? 'Não foi possível salvar as alterações.');
        },
      });
  }

  pedirConfirmacaoExclusao(produtoId: string): void {
    this.produtoParaExcluir.set(produtoId);
  }

  cancelarExclusao(): void {
    this.produtoParaExcluir.set(null);
  }

  confirmarExclusao(): void {
    const produtoId = this.produtoParaExcluir();
    if (!produtoId) return;

    this.excluindo.set(true);

    this.catalogoService.excluirProduto(produtoId).subscribe({
      next: () => {
        this.excluindo.set(false);
        this.produtoParaExcluir.set(null);
        this.carregarProdutos();
      },
      error: () => {
        this.excluindo.set(false);
        this.produtoParaExcluir.set(null);
      },
    });
  }

  sair(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}