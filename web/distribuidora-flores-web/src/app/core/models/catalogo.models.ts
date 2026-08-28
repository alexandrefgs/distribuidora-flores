export interface Produto {
  id: string;
  nome: string;
  categoria: string;
  unidadeMedida: string;
  precoUnitario: number;
  quantidadeDisponivel: number;
  imagemUrl: string | null;
}