import { Component, ElementRef, input, output, ViewChild } from '@angular/core';
import { ButtonComponent } from '../button/button.component';

@Component({
  selector: 'app-info-modal',
  imports: [ButtonComponent],
  templateUrl: './info-modal.component.html',
  styleUrl: './info-modal.component.css',
})
export class InfoModalComponent {
  size = input.required<'small' | 'medium'>();
  closeClicked = output<void>();
}
