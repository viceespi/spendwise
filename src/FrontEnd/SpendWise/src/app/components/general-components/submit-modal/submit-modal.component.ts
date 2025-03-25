import { Component, input, output } from '@angular/core';
import { ButtonComponent } from '../button/button.component';

@Component({
  selector: 'app-submit-modal',
  imports: [ButtonComponent],
  templateUrl: './submit-modal.component.html',
  styleUrl: './submit-modal.component.css',
})
export class SubmitModalComponent {
  size = input.required<'small' | 'medium'>();
  submitClicked = output<void>();
  closeClicked = output<void>();
}
