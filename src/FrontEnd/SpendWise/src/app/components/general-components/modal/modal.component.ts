import { Component, ElementRef, input, output, ViewChild } from '@angular/core';
import { ButtonComponent } from '../button/button.component';

@Component({
  selector: 'app-modal',
  imports: [ButtonComponent],
  templateUrl: './modal.component.html',
  styleUrl: './modal.component.css',
})
export class ModalComponent {
  size = input.required<'small' | 'medium'>();
  submitClicked = output<void>();
  closeClicked = output<void>();
}
