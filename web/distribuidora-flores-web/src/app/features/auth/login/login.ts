import { Component, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './login.html',
  styleUrl: './login.css',
})
export class Login {
  email = '';
  senha = '';
  carregando = signal(false);
  erro = signal<string | null>(null);

  constructor(private authService: AuthService, private router: Router) {}

  onSubmit(): void {
    this.erro.set(null);
    this.carregando.set(true);

    this.authService.login({ email: this.email, senha: this.senha }).subscribe({
      next: () => {
        const usuario = this.authService.currentUser();
        this.carregando.set(false);

        if (usuario?.role === 'Admin') {
          this.router.navigate(['/admin']);
        } else {
          this.router.navigate(['/catalogo']);
        }
      },
      error: () => {
        this.carregando.set(false);
        this.erro.set('Email ou senha inválidos.');
      },
    });
  }
}