import { Directive, ElementRef, OnDestroy, AfterViewInit, Output, EventEmitter } from '@angular/core';
import flatpickr from 'flatpickr';
import { Portuguese } from 'flatpickr/dist/l10n/pt.js';

@Directive({
  selector: '[appDatepicker]',
  standalone: true,
})
export class DatepickerDirective implements AfterViewInit, OnDestroy {
  @Output() dataSelecionada = new EventEmitter<string>();

  private instancia: flatpickr.Instance | null = null;

  constructor(private el: ElementRef<HTMLInputElement>) {}

  ngAfterViewInit(): void {
    this.instancia = flatpickr(this.el.nativeElement, {
      locale: Portuguese,
      dateFormat: 'Y-m-d',
      altInput: true,
      altFormat: 'd/m/Y',
      minDate: 'today',
      onChange: (_dates, dataStr) => {
        this.dataSelecionada.emit(dataStr);
      },
    });
  }

  ngOnDestroy(): void {
    this.instancia?.destroy();
  }
}