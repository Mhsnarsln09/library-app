import { CommonModule } from '@angular/common';
import { Component, DestroyRef, OnInit, computed, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { RouterLink } from '@angular/router';
import { DashboardOverviewComponent } from '../../components/dashboard-overview.component';
import { ListToolbarComponent } from '../../components/list-toolbar.component';
import { PaginationControlsComponent } from '../../components/pagination-controls.component';
import { AuthorListItem, BookListItem, CategoryListItem, LoanListItem, MemberListItem } from '../../models/api.models';
import { TEST_FEATURE_CONFIG, TestFeatureConfig } from './test-feature.config';
import { TestFacadeService } from './test-facade.service';

@Component({
  selector: 'app-test-page',
  standalone: true,
  imports: [
    CommonModule,
    RouterLink,
    DashboardOverviewComponent,
    ListToolbarComponent,
    PaginationControlsComponent
  ],
  templateUrl: './test-page.component.html',
  styleUrl: './test-page.component.css'
})
export class TestPageComponent implements OnInit {
  private readonly destroyRef = inject(DestroyRef);
  private readonly facade = inject(TestFacadeService);

  readonly config: TestFeatureConfig = inject(TEST_FEATURE_CONFIG);
  readonly loading = signal(false);
  readonly error = signal('');
  readonly authorSearch = signal('');
  readonly page = signal(1);

  readonly authors = signal<AuthorListItem[]>([]);
  readonly categories = signal<CategoryListItem[]>([]);
  readonly books = signal<BookListItem[]>([]);
  readonly loans = signal<LoanListItem[]>([]);
  readonly members = signal<MemberListItem[]>([]);

  readonly filteredAuthors = computed(() => {
    const query = this.authorSearch().trim().toLowerCase();
    if (!query) return this.authors();
    return this.authors().filter((author) => author.fullName.toLowerCase().includes(query));
  });

  readonly pageSize = this.config.pageSizeOptions[0] ?? 5;

  readonly pagedAuthors = computed(() => {
    const start = (this.page() - 1) * this.pageSize;
    return this.filteredAuthors().slice(start, start + this.pageSize);
  });

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading.set(true);
    this.error.set('');

    this.facade
      .loadSnapshot()
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (snapshot) => {
          this.authors.set(snapshot.authors);
          this.categories.set(snapshot.categories);
          this.books.set(snapshot.books);
          this.loans.set(snapshot.loans);
          this.members.set(snapshot.members);
          this.page.set(1);
          this.loading.set(false);
        },
        error: (error: unknown) => {
          const message = error instanceof Error ? error.message : 'Data load failed.';
          this.error.set(message);
          this.loading.set(false);
        }
      });
  }

  setSearch(value: string): void {
    this.authorSearch.set(value);
    this.page.set(1);
  }

  setPage(page: number): void {
    this.page.set(Math.max(1, page));
  }
}
