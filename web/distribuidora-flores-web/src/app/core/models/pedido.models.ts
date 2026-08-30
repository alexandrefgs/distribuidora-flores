export interface ItemPedidoRequest {
  produtoId: string;
  quantidade: number;
}

export interface CriarPedidoRequest {
  clienteId: string;
  itens: ItemPedidoRequest[];
}