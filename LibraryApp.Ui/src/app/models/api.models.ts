export type UserRole = 'Admin' | 'Member' | string;

export interface ApiResponse {
  isSuccess: boolean;
  message: string;
}

export interface ApiResponseOf<T> extends ApiResponse {
  data: T;
}

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
}

export interface AuthResponse {
  token: string;
  email: string;
  fullName: string;
  role: UserRole;
}

export interface AuthorListItem { id: number; fullName: string; }
export interface AuthorDetail { id: number; fullName: string; }
export interface CategoryListItem { id: number; name: string; }
export interface CategoryDetail { id: number; name: string; }

export interface BookListItem {
  id: number;
  title: string;
  totalCopies: number;
  author: AuthorDetail;
  category: CategoryListItem;
}

export interface BookDetail {
  id: number;
  title: string;
  isbn?: string | null;
  totalCopies: number;
  author: AuthorDetail;
  category: CategoryListItem;
}

export interface MemberListItem {
  id: number;
  fullName: string;
  email: string;
  role: UserRole;
}

export interface MemberDetail {
  id: number;
  fullName: string;
  email: string;
  role: UserRole;
}

export interface LoanListItem {
  id: number;
  bookTitle: string;
  memberFullName: string;
  loanedAtUtc: string;
  dueAtUtc: string;
  isReturned: boolean;
  isOverdue: boolean;
}

export interface LoanDetail {
  id: number;
  book: BookListItem;
  member: MemberListItem;
  loanedAtUtc: string;
  dueAtUtc: string;
  returnedAtUtc?: string | null;
  isReturned: boolean;
  isOverdue: boolean;
}

export interface PenaltyDetail {
  loanId: number;
  bookTitle: string;
  dueAtUtc: string;
  returnedAtUtc?: string | null;
  penaltyAmount: number;
  overdueDays: number;
}

export interface MemberPenalty {
  memberId: number;
  fullName: string;
  totalPenalty: number;
  overdueLoans: PenaltyDetail[];
}

export interface PaginationQuery {
  pageNumber?: number;
  pageSize?: number;
}
