import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-list-toolbar',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './list-toolbar.component.html',
  styleUrl: './list-toolbar.component.css'
})
export class ListToolbarComponent {
  @Input() search = '';
  @Input() searchPlaceholder = 'Search...';
  @Output() searchChange = new EventEmitter<string>();
}
