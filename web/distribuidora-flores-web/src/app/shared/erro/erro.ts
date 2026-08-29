import { Component, computed, input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';

interface ConfigErro {
  emoji: string;
  titulo: string;
  mensagem: string;
}

const CONFIGS: Record<string, ConfigErro> = {
  '401': {
    emoji: '🔒',
    titulo: 'Opa, quem é você?',
    mensagem: 'Essa área é só pra quem tem login. Volte e se identifique, por favor.',
  },
  '403': {
    emoji: '🌵',
    titulo: 'Terreno proibido!',
    mensagem: 'Você até tem login, mas não tem permissão pra pisar nesse canteiro.',
  },
  '404': {
    emoji: '🥀',
    titulo: 'Essa flor murchou...',
    mensagem: 'A página que você procura não existe (ou nunca floresceu por aqui).',
  },
  '500': {
    emoji: '🐛',
    titulo: 'Achamos uma praga no sistema!',
    mensagem: 'Algo quebrou do nosso lado. Já chamamos o jardineiro pra consertar.',
  },
};

@Component({
  selector: 'app-erro',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './erro.html',
  styleUrl: './erro.css',
})
export class Erro {
  codigo = input<string>('404');

  config = computed<ConfigErro>(() => CONFIGS[this.codigo()] ?? CONFIGS['404']);
}