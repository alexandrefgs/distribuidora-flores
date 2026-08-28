import { Component, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-registro',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './registro.html',
  styleUrl: './registro.css',
})
export class Registro {
  nomeFantasia = '';
  documento = '';
  telefone = '';
  endereco = '';
  email = '';
  senha = '';

  carregando = signal(false);
  erro = signal<string | null>(null);
  sucesso = signal(false);

  constructor(private authService: AuthService, private router: Router) {}

  onSubmit(): void {
    this.erro.set(null);
    this.carregando.set(true);

    this.authService
      .registrarComerciante({
        nomeFantasia: this.nomeFantasia,
        documento: this.documento,
        telefone: this.telefone,
        endereco: this.endereco,
        email: this.email,
        senha: this.senha,
      })
      .subscribe({
        next: () => {
          this.carregando.set(false);
          this.sucesso.set(true);

          // Pequena pausa pra o usuário ver a mensagem de sucesso antes de ir pro login
          setTimeout(() => this.router.navigate(['/login']), 1500);
        },
        error: (err) => {
          this.carregando.set(false);
          this.erro.set(this.extrairMensagemErro(err));
        },
      });
  }

  private extrairMensagemErro(err: any): string {
    if (err?.error?.erro) {
      return err.error.erro;
    }
    return 'Não foi possível concluir o registro. Verifique os dados e tente novamente.';
  }
}