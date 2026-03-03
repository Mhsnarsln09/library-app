import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { AuthorListItem, BookListItem, CategoryListItem, LoanListItem, MemberListItem } from '../models/api.models';

@Component({
  selector: 'app-dashboard-overview',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './dashboard-overview.component.html',
  styleUrl: './dashboard-overview.component.css'
})
export class DashboardOverviewComponent {
  @Input() authors: AuthorListItem[] = [];
  @Input() categories: CategoryListItem[] = [];
  @Input() books: BookListItem[] = [];
  @Input() loans: LoanListItem[] = [];
  @Input() members: MemberListItem[] = [];
  @Input() isAdmin = false;
  @Input() memberId: number | null = null;

  @Output() refresh = new EventEmitter<void>();
  @Output() health = new EventEmitter<void>();
  @Output() openTab = new EventEmitter<'authors' | 'categories' | 'books' | 'loans' | 'members' | 'penalties'>();

  get counts(): { returned: number; overdue: number; active: number } {
    let returned = 0;
    let overdue = 0;
    let active = 0;
    for (const loan of this.loans) {
      if (loan.isReturned) returned += 1;
      else if (loan.isOverdue) overdue += 1;
      else active += 1;
    }
    return { returned, overdue, active };
  }

  get statusPct(): { returned: number; overdue: number; active: number } {
    const total = this.loans.length || 1;
    const c = this.counts;
    return {
      returned: (c.returned / total) * 100,
      overdue: (c.overdue / total) * 100,
      active: (c.active / total) * 100
    };
  }

  get topCategories(): Array<{ label: string; value: number; percent: number }> {
    const byCategory = new Map<string, number>();
    for (const book of this.books) {
      const key = book.category?.name || 'Unknown';
      byCategory.set(key, (byCategory.get(key) || 0) + 1);
    }
    const rows = [...byCategory.entries()]
      .map(([label, value]) => ({ label, value }))
      .sort((a, b) => b.value - a.value)
      .slice(0, 5);

    const max = rows[0]?.value || 1;
    return rows.map((r) => ({ ...r, percent: (r.value / max) * 100 }));
  }
}
