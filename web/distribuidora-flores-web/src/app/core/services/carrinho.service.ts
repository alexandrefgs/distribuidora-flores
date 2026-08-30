import { Injectable, signal, computed } from '@angular/core';
import { ItemCarrinho } from '../models/carrinho.models';
import { Produto } from '../models/catalogo.models';

@Injectable({ providedIn: 'root' })
export class CarrinhoService {
  private itensSignal = signal<ItemCarrinho[]>([]);

  itens = this.itensSignal.asReadonly();

  totalItens = computed(() =>
    this.itensSignal().reduce((soma, item) => soma + item.quantidade, 0)
  );

  totalValor = computed(() =>
    this.itensSignal().reduce((soma, item) => soma + item.quantidade * item.precoUnitario, 0)
  );

  adicionar(produto: Produto, quantidade: number): void {
    const itensAtuais = this.itensSignal();
    const existente = itensAtuais.find((i) => i.produtoId === produto.id);

    if (existente) {
      this.itensSignal.set(
        itensAtuais.map((i) =>
          i.produtoId === produto.id ? { ...i, quantidade: i.quantidade + quantidade } : i
        )
      );
    } else {
      this.itensSignal.set([
        ...itensAtuais,
        {
          produtoId: produto.id,
          nome: produto.nome,
          precoUnitario: produto.precoUnitario,
          unidadeMedida: produto.unidadeMedida,
          quantidade,
        },
      ]);
    }
  }

  removerItem(produtoId: string): void {
    this.itensSignal.set(this.itensSignal().filter((i) => i.produtoId !== produtoId));
  }

  atualizarQuantidade(produtoId: string, quantidade: number): void {
    if (quantidade <= 0) {
      this.removerItem(produtoId);
      return;
    }

    this.itensSignal.set(
      this.itensSignal().map((i) => (i.produtoId === produtoId ? { ...i, quantidade } : i))
    );
  }

  limpar(): void {
    this.itensSignal.set([]);
  }
}