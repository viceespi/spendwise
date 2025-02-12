import { Component, input, output } from '@angular/core';

@Component({
  selector: 'app-button-component',
  imports: [],
  templateUrl: './button.component.html',
  styleUrl: './button.component.css'
})
export class ButtonComponent {
  name = input.required<string>();
  clicked = output();
  size = input.required<'small' | 'medium' | 'big'>();
}
