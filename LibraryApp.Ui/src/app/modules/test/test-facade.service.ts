import { Injectable, inject } from '@angular/core';
import { forkJoin, map, Observable } from 'rxjs';
import { ApiClientService } from '../../core/api-client.service';
import {
  AuthorListItem,
  BookListItem,
  CategoryListItem,
  LoanListItem,
  MemberListItem
} from '../../models/api.models';

export interface TestFeatureSnapshot {
  authors: AuthorListItem[];
  categories: CategoryListItem[];
  books: BookListItem[];
  loans: LoanListItem[];
  members: MemberListItem[];
}

@Injectable()
export class TestFacadeService {
  private readonly api = inject(ApiClientService);

  loadSnapshot(): Observable<TestFeatureSnapshot> {
    return forkJoin({
      authors: this.api.getAuthors({ pageNumber: 1, pageSize: 8 }),
      categories: this.api.getCategories({ pageNumber: 1, pageSize: 8 }),
      books: this.api.getBooks({ pageNumber: 1, pageSize: 12 }),
      loans: this.api.getLoans({ pageNumber: 1, pageSize: 12 }),
      members: this.api.getMembers({ pageNumber: 1, pageSize: 8 })
    }).pipe(
      map((result) => ({
        authors: result.authors.data.items,
        categories: result.categories.data.items,
        books: result.books.data.items,
        loans: result.loans.data.items,
        members: result.members.data.items
      }))
    );
  }
}
