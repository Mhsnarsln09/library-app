import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import {
  ApiResponse,
  ApiResponseOf,
  AuthResponse,
  AuthorDetail,
  AuthorListItem,
  BookDetail,
  BookListItem,
  CategoryDetail,
  CategoryListItem,
  LoanDetail,
  LoanListItem,
  MemberDetail,
  MemberListItem,
  MemberPenalty,
  PagedResult,
  PaginationQuery
} from '../models/api.models';

@Injectable({ providedIn: 'root' })
export class ApiClientService {
  private readonly defaultBaseUrl = 'http://localhost:5098';

  constructor(private readonly http: HttpClient) {}

  get baseUrl(): string {
    return localStorage.getItem('libraryapp.ui.apiBaseUrl') || this.defaultBaseUrl;
  }

  setBaseUrl(url: string): void {
    localStorage.setItem('libraryapp.ui.apiBaseUrl', url.replace(/\/$/, ''));
  }

  login(payload: { email: string; password: string }): Observable<ApiResponseOf<AuthResponse>> {
    return this.http.post<ApiResponseOf<AuthResponse>>(this.url('/api/auth/login'), payload);
  }

  register(payload: { fullName: string; email: string; password: string }): Observable<ApiResponseOf<AuthResponse>> {
    return this.http.post<ApiResponseOf<AuthResponse>>(this.url('/api/auth/register'), payload);
  }

  getHealth(): Observable<{ status: string }> {
    return this.http.get<{ status: string }>(this.url('/health'));
  }

  getAuthors(query: PaginationQuery): Observable<ApiResponseOf<PagedResult<AuthorListItem>>> {
    return this.http.get<ApiResponseOf<PagedResult<AuthorListItem>>>(this.url('/api/authors'), { params: this.pageParams(query) });
  }

  getAuthor(id: number): Observable<ApiResponseOf<AuthorDetail>> {
    return this.http.get<ApiResponseOf<AuthorDetail>>(this.url(`/api/authors/${id}`));
  }

  createAuthor(payload: { fullName: string }): Observable<ApiResponse> {
    return this.http.post<ApiResponse>(this.url('/api/authors'), payload);
  }

  updateAuthor(id: number, payload: { fullName: string }): Observable<ApiResponse> {
    return this.http.put<ApiResponse>(this.url(`/api/authors/${id}`), payload);
  }

  deleteAuthor(id: number): Observable<ApiResponse> {
    return this.http.delete<ApiResponse>(this.url(`/api/authors/${id}`));
  }

  getCategories(query: PaginationQuery): Observable<ApiResponseOf<PagedResult<CategoryListItem>>> {
    return this.http.get<ApiResponseOf<PagedResult<CategoryListItem>>>(this.url('/api/categories'), { params: this.pageParams(query) });
  }

  getCategory(id: number): Observable<ApiResponseOf<CategoryDetail>> {
    return this.http.get<ApiResponseOf<CategoryDetail>>(this.url(`/api/categories/${id}`));
  }

  createCategory(payload: { name: string }): Observable<ApiResponse> {
    return this.http.post<ApiResponse>(this.url('/api/categories'), payload);
  }

  updateCategory(id: number, payload: { name: string }): Observable<ApiResponse> {
    return this.http.put<ApiResponse>(this.url(`/api/categories/${id}`), payload);
  }

  deleteCategory(id: number): Observable<ApiResponse> {
    return this.http.delete<ApiResponse>(this.url(`/api/categories/${id}`));
  }

  getBooks(query: PaginationQuery): Observable<ApiResponseOf<PagedResult<BookListItem>>> {
    return this.http.get<ApiResponseOf<PagedResult<BookListItem>>>(this.url('/api/books'), { params: this.pageParams(query) });
  }

  getBook(id: number): Observable<ApiResponseOf<BookDetail>> {
    return this.http.get<ApiResponseOf<BookDetail>>(this.url(`/api/books/${id}`));
  }

  createBook(payload: { title: string; isbn?: string | null; totalCopies: number; authorId: number; categoryId: number }): Observable<ApiResponse> {
    return this.http.post<ApiResponse>(this.url('/api/books'), payload);
  }

  updateBook(id: number, payload: { title: string; isbn?: string | null; totalCopies: number; authorId: number; categoryId: number }): Observable<ApiResponse> {
    return this.http.put<ApiResponse>(this.url(`/api/books/${id}`), payload);
  }

  deleteBook(id: number): Observable<ApiResponse> {
    return this.http.delete<ApiResponse>(this.url(`/api/books/${id}`));
  }

  getLoans(query: PaginationQuery): Observable<ApiResponseOf<PagedResult<LoanListItem>>> {
    return this.http.get<ApiResponseOf<PagedResult<LoanListItem>>>(this.url('/api/loans'), { params: this.pageParams(query) });
  }

  getLoan(id: number): Observable<ApiResponseOf<LoanDetail>> {
    return this.http.get<ApiResponseOf<LoanDetail>>(this.url(`/api/loans/${id}`));
  }

  createLoan(payload: { bookId: number; memberId: number }): Observable<ApiResponse> {
    return this.http.post<ApiResponse>(this.url('/api/loans'), payload);
  }

  returnLoan(id: number): Observable<ApiResponse> {
    return this.http.patch<ApiResponse>(this.url(`/api/loans/${id}/return`), {});
  }

  deleteLoan(id: number): Observable<ApiResponse> {
    return this.http.delete<ApiResponse>(this.url(`/api/loans/${id}`));
  }

  getMembers(query: PaginationQuery): Observable<ApiResponseOf<PagedResult<MemberListItem>>> {
    return this.http.get<ApiResponseOf<PagedResult<MemberListItem>>>(this.url('/api/members'), { params: this.pageParams(query) });
  }

  getMember(id: number): Observable<ApiResponseOf<MemberDetail>> {
    return this.http.get<ApiResponseOf<MemberDetail>>(this.url(`/api/members/${id}`));
  }

  updateMember(id: number, payload: { fullName: string; email: string; role: string }): Observable<ApiResponse> {
    return this.http.put<ApiResponse>(this.url(`/api/members/${id}`), payload);
  }

  deleteMember(id: number): Observable<ApiResponse> {
    return this.http.delete<ApiResponse>(this.url(`/api/members/${id}`));
  }

  getMemberPenalties(id: number): Observable<ApiResponseOf<MemberPenalty>> {
    return this.http.get<ApiResponseOf<MemberPenalty>>(this.url(`/api/members/${id}/penalties`));
  }

  private url(path: string): string {
    return `${this.baseUrl}${path}`;
  }

  private pageParams(query: PaginationQuery): HttpParams {
    let params = new HttpParams();
    if (query.pageNumber) params = params.set('pageNumber', query.pageNumber);
    if (query.pageSize) params = params.set('pageSize', query.pageSize);
    return params;
  }
}
