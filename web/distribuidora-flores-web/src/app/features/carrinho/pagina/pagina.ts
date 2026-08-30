import { Component, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { CarrinhoService } from '../../../core/services/carrinho.service';
import { PedidoService } from '../../../core/services/pedido.service';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-pagina',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './pagina.html',
  styleUrl: './pagina.css',
})
export class Pagina {
  finalizando = signal(false);
  erro = signal<string | null>(null);
  sucesso = signal(false);

  constructor(
    public carrinhoService: CarrinhoService,
    private pedidoService: PedidoService,
    private authService: AuthService,
    private router: Router
  ) {}

  aumentar(produtoId: string, quantidadeAtual: number): void {
    this.carrinhoService.atualizarQuantidade(produtoId, quantidadeAtual + 1);
  }

  diminuir(produtoId: string, quantidadeAtual: number): void {
    this.carrinhoService.atualizarQuantidade(produtoId, quantidadeAtual - 1);
  }

  remover(produtoId: string): void {
    this.carrinhoService.removerItem(produtoId);
  }

  finalizarPedido(): void {
    const usuario = this.authService.currentUser();
    const clienteId = usuario?.clienteId;

    if (!clienteId) {
      this.erro.set('Não foi possível identificar seu cadastro. Faça login novamente.');
      return;
    }

    this.erro.set(null);
    this.finalizando.set(true);

    const request = {
      clienteId,
      itens: this.carrinhoService.itens().map((item) => ({
        produtoId: item.produtoId,
        quantidade: item.quantidade,
      })),
    };

    this.pedidoService.criarPedido(request).subscribe({
      next: () => {
        this.finalizando.set(false);
        this.sucesso.set(true);
        this.carrinhoService.limpar();

        setTimeout(() => this.router.navigate(['/catalogo']), 2000);
      },
      error: (err) => {
        this.finalizando.set(false);
        this.erro.set(err?.error?.erro ?? 'Não foi possível finalizar o pedido.');
      },
    });
  }
}