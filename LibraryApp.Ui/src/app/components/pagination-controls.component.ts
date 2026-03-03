import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
  selector: 'app-pagination-controls',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './pagination-controls.component.html',
  styleUrl: './pagination-controls.component.css'
})
export class PaginationControlsComponent {
  @Input({ required: true }) totalItems = 0;
  @Input({ required: true }) page = 1;
  @Input({ required: true }) pageSize = 10;
  @Input() pageSizeOptions: number[] = [5, 10, 20, 50];

  @Output() pageChange = new EventEmitter<number>();
  @Output() pageSizeChange = new EventEmitter<number>();

  get totalPages(): number {
    return Math.max(1, Math.ceil(this.totalItems / this.pageSize));
  }

  get startItem(): number {
    if (this.totalItems === 0) return 0;
    return (this.page - 1) * this.pageSize + 1;
  }

  get endItem(): number {
    return Math.min(this.totalItems, this.page * this.pageSize);
  }

  prev(): void {
    if (this.page > 1) this.pageChange.emit(this.page - 1);
  }

  next(): void {
    if (this.page < this.totalPages) this.pageChange.emit(this.page + 1);
  }

  onPageSizeChange(value: string): void {
    const parsed = Number(value);
    if (!Number.isFinite(parsed) || parsed < 1) return;
    this.pageSizeChange.emit(parsed);
  }
}
