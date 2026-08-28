import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_BASE_URL } from '../config/api.config';
import { Produto } from '../models/catalogo.models';

export interface CriarProdutoRequest {
  nome: string;
  categoria: string;
  unidadeMedida: string;
  precoUnitario: number;
}

@Injectable({ providedIn: 'root' })
export class CatalogoService {
  constructor(private http: HttpClient) {}

  listarProdutos(): Observable<Produto[]> {
    return this.http.get<Produto[]>(`${API_BASE_URL}/produtos`);
  }

  criarProduto(request: CriarProdutoRequest): Observable<{ id: string }> {
    return this.http.post<{ id: string }>(`${API_BASE_URL}/produtos`, request);
  }

  enviarImagem(produtoId: string, arquivo: File): Observable<{ imagemUrl: string }> {
    const formData = new FormData();
    formData.append('arquivo', arquivo);

    return this.http.post<{ imagemUrl: string }>(
      `${API_BASE_URL}/produtos/${produtoId}/imagem`,
      formData
    );
  }
}