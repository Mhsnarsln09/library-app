import { CommonModule, DatePipe, DecimalPipe } from '@angular/common';
import { Component, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { DashboardOverviewComponent } from '../../components/dashboard-overview.component';
import { ListToolbarComponent } from '../../components/list-toolbar.component';
import { PaginationControlsComponent } from '../../components/pagination-controls.component';
import { ApiClientService } from '../../core/api-client.service';
import { AuthStore } from '../../core/auth.store';
import {
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
    MemberPenalty
} from '../../models/api.models';

type PortalTab = 'overview' | 'authors' | 'categories' | 'books' | 'loans' | 'members' | 'penalties';
type ListTab = 'authors' | 'categories' | 'books' | 'loans' | 'members' | 'penaltyRows';
type PageState = { page: number; pageSize: number };

@Component({
    selector: 'app-portal-page',
    standalone: true,
    imports: [
        CommonModule,
        FormsModule,
        DatePipe,
        DecimalPipe,
        DashboardOverviewComponent,
        ListToolbarComponent,
        PaginationControlsComponent
    ],
    templateUrl: './portal-page.component.html',
    styleUrl: './portal-page.component.css'
})
export class PortalPageComponent {
    readonly auth = inject(AuthStore);
    private readonly api = inject(ApiClientService);
    private readonly router = inject(Router);

    readonly activeTab = signal<PortalTab>('overview');
    readonly toast = signal('');
    readonly toastType = signal<'success' | 'error'>('success');
    readonly loading = signal(false);

    readonly tabs = [
        { key: 'overview', label: 'Overview' },
        { key: 'authors', label: 'Authors' },
        { key: 'categories', label: 'Categories' },
        { key: 'books', label: 'Books' },
        { key: 'loans', label: 'Loans' },
        { key: 'members', label: 'Members' },
        { key: 'penalties', label: 'Penalties' }
    ] as const;

    apiBaseUrlInput = this.api.baseUrl;

    authors: AuthorListItem[] = [];
    categories: CategoryListItem[] = [];
    books: BookListItem[] = [];
    loans: LoanListItem[] = [];
    members: MemberListItem[] = [];

    authorDetail: AuthorDetail | null = null;
    categoryDetail: CategoryDetail | null = null;
    bookDetail: BookDetail | null = null;
    loanDetail: LoanDetail | null = null;
    memberDetail: MemberDetail | null = null;
    penaltyDetail: MemberPenalty | null = null;

    authorDetailId: number | null = null;
    categoryDetailId: number | null = null;
    bookDetailId: number | null = null;
    loanDetailId: number | null = null;
    memberDetailId: number | null = null;
    penaltyMemberId: number | null = null;

    authorForm = { fullName: '' };
    authorEdit = { id: null as number | null, fullName: '' };

    categoryForm = { name: '' };
    categoryEdit = { id: null as number | null, name: '' };

    bookForm = { title: '', isbn: '', totalCopies: 1, authorId: 1, categoryId: 1 };
    bookEdit = { id: null as number | null, title: '', isbn: '', totalCopies: 1, authorId: 1, categoryId: 1 };

    loanForm = {
        bookId: 1,
        memberId: this.auth.memberId() ?? 1
    };

    memberEdit = { id: null as number | null, fullName: '', email: '', role: 'Member' };

    authorSearch = '';
    categorySearch = '';
    bookSearch = '';
    loanSearch = '';
    memberSearch = '';
    penaltySearch = '';

    bookCategoryFilter = 'all';
    loanStatusFilter: 'all' | 'active' | 'returned' | 'overdue' = 'all';
    memberRoleFilter: 'all' | 'Admin' | 'Member' = 'all';

    pageState: Record<ListTab, PageState> = {
        authors: { page: 1, pageSize: 10 },
        categories: { page: 1, pageSize: 10 },
        books: { page: 1, pageSize: 10 },
        loans: { page: 1, pageSize: 10 },
        members: { page: 1, pageSize: 10 },
        penaltyRows: { page: 1, pageSize: 10 }
    };

    readonly canUseMyId = computed(() => this.auth.memberId() != null);

    constructor() {
        if (!this.auth.isAuthenticated()) {
            void this.router.navigateByUrl('/auth');
            return;
        }

        this.penaltyMemberId = this.auth.memberId();
        this.refreshAll();
    }

    get filteredAuthors(): AuthorListItem[] {
        return this.filterByQuery(this.authors, this.authorSearch, (item) => `${item.id} ${item.fullName}`);
    }

    get pagedAuthors(): AuthorListItem[] {
        return this.paginate('authors', this.filteredAuthors);
    }

    get filteredCategories(): CategoryListItem[] {
        return this.filterByQuery(this.categories, this.categorySearch, (item) => `${item.id} ${item.name}`);
    }

    get pagedCategories(): CategoryListItem[] {
        return this.paginate('categories', this.filteredCategories);
    }

    get filteredBooks(): BookListItem[] {
        let rows = this.filterByQuery(
            this.books,
            this.bookSearch,
            (item) => `${item.id} ${item.title} ${item.author?.fullName ?? ''} ${item.category?.name ?? ''}`
        );
        if (this.bookCategoryFilter !== 'all') {
            rows = rows.filter((b) => b.category?.name === this.bookCategoryFilter);
        }
        return rows;
    }

    get pagedBooks(): BookListItem[] {
        return this.paginate('books', this.filteredBooks);
    }

    get filteredLoans(): LoanListItem[] {
        let rows = this.filterByQuery(
            this.loans,
            this.loanSearch,
            (item) => `${item.id} ${item.bookTitle} ${item.memberFullName}`
        );

        if (this.loanStatusFilter === 'returned') rows = rows.filter((x) => x.isReturned);
        if (this.loanStatusFilter === 'active') rows = rows.filter((x) => !x.isReturned && !x.isOverdue);
        if (this.loanStatusFilter === 'overdue') rows = rows.filter((x) => !x.isReturned && x.isOverdue);

        return rows;
    }

    get pagedLoans(): LoanListItem[] {
        return this.paginate('loans', this.filteredLoans);
    }

    get filteredMembers(): MemberListItem[] {
        let rows = this.filterByQuery(
            this.members,
            this.memberSearch,
            (item) => `${item.id} ${item.fullName} ${item.email} ${item.role}`
        );

        if (this.memberRoleFilter !== 'all') {
            rows = rows.filter((m) => String(m.role) === this.memberRoleFilter);
        }
        return rows;
    }

    get pagedMembers(): MemberListItem[] {
        return this.paginate('members', this.filteredMembers);
    }

    get filteredPenaltyRows(): MemberPenalty['overdueLoans'] {
        const rows = this.penaltyDetail?.overdueLoans ?? [];
        return this.filterByQuery(rows, this.penaltySearch, (item) => `${item.loanId} ${item.bookTitle}`);
    }

    get pagedPenaltyRows(): MemberPenalty['overdueLoans'] {
        return this.paginate('penaltyRows', this.filteredPenaltyRows);
    }

    goToTab(tab: Exclude<PortalTab, 'overview'>): void {
        this.activeTab.set(tab);
    }

    setSearch(tab: ListTab, value: string): void {
        const next = value ?? '';
        if (tab === 'authors') this.authorSearch = next;
        if (tab === 'categories') this.categorySearch = next;
        if (tab === 'books') this.bookSearch = next;
        if (tab === 'loans') this.loanSearch = next;
        if (tab === 'members') this.memberSearch = next;
        if (tab === 'penaltyRows') this.penaltySearch = next;
        this.pageState[tab].page = 1;
    }

    setPage(tab: ListTab, page: number): void {
        this.pageState[tab].page = Math.max(1, page);
    }

    setPageSize(tab: ListTab, pageSize: number): void {
        this.pageState[tab].pageSize = Math.max(1, pageSize);
        this.pageState[tab].page = 1;
    }

    setBookCategoryFilter(value: string): void {
        this.bookCategoryFilter = value || 'all';
        this.pageState.books.page = 1;
    }

    setLoanStatusFilter(value: string): void {
        if (value === 'active' || value === 'returned' || value === 'overdue') {
            this.loanStatusFilter = value;
        } else {
            this.loanStatusFilter = 'all';
        }
        this.pageState.loans.page = 1;
    }

    setMemberRoleFilter(value: string): void {
        if (value === 'Admin' || value === 'Member') {
            this.memberRoleFilter = value;
        } else {
            this.memberRoleFilter = 'all';
        }
        this.pageState.members.page = 1;
    }

    saveApiBaseUrl(): void {
        this.api.setBaseUrl(this.apiBaseUrlInput);
        this.notify('API base URL updated');
    }

    logout(): void {
        this.auth.logout();
        void this.router.navigateByUrl('/auth');
    }

    checkHealth(): void {
        this.api.getHealth().subscribe({
            next: (res) => this.notify(`Health: ${res.status}`),
            error: (err) => this.fail(err?.error?.detail || 'Health check failed')
        });
    }

    refreshAll(): void {
        this.loadAuthors();
        this.loadCategories();
        this.loadBooks();
        this.loadLoans();
        if (this.auth.isAdmin()) {
            this.loadMembers();
        }
    }

    loadAuthors(): void {
        this.api.getAuthors({ pageNumber: 1, pageSize: 50 }).subscribe({
            next: (res) => {
                this.authors = res.data.items;
                this.ensurePageInRange('authors', this.filteredAuthors.length);
            },
            error: (err) => this.failFromError(err)
        });
    }

    fetchAuthorDetail(): void {
        if (!this.authorDetailId) return;
        this.api.getAuthor(this.authorDetailId).subscribe({
            next: (res) => (this.authorDetail = res.data),
            error: (err) => this.failFromError(err)
        });
    }

    createAuthor(): void {
        if (!this.authorForm.fullName.trim()) return;
        this.api.createAuthor({ fullName: this.authorForm.fullName.trim() }).subscribe({
            next: (res) => this.afterMutation(res.isSuccess, res.message, () => {
                this.authorForm.fullName = '';
                this.loadAuthors();
            }),
            error: (err) => this.failFromError(err)
        });
    }

    updateAuthor(): void {
        if (!this.authorEdit.id || !this.authorEdit.fullName.trim()) return;
        this.api.updateAuthor(this.authorEdit.id, { fullName: this.authorEdit.fullName.trim() }).subscribe({
            next: (res) => this.afterMutation(res.isSuccess, res.message, () => this.loadAuthors()),
            error: (err) => this.failFromError(err)
        });
    }

    deleteAuthor(): void {
        if (!this.authorEdit.id) return;
        this.api.deleteAuthor(this.authorEdit.id).subscribe({
            next: (res) => this.afterMutation(res.isSuccess, res.message, () => this.loadAuthors()),
            error: (err) => this.failFromError(err)
        });
    }

    loadCategories(): void {
        this.api.getCategories({ pageNumber: 1, pageSize: 50 }).subscribe({
            next: (res) => {
                this.categories = res.data.items;
                this.ensurePageInRange('categories', this.filteredCategories.length);
            },
            error: (err) => this.failFromError(err)
        });
    }

    fetchCategoryDetail(): void {
        if (!this.categoryDetailId) return;
        this.api.getCategory(this.categoryDetailId).subscribe({
            next: (res) => (this.categoryDetail = res.data),
            error: (err) => this.failFromError(err)
        });
    }

    createCategory(): void {
        if (!this.categoryForm.name.trim()) return;
        this.api.createCategory({ name: this.categoryForm.name.trim() }).subscribe({
            next: (res) => this.afterMutation(res.isSuccess, res.message, () => {
                this.categoryForm.name = '';
                this.loadCategories();
            }),
            error: (err) => this.failFromError(err)
        });
    }

    updateCategory(): void {
        if (!this.categoryEdit.id || !this.categoryEdit.name.trim()) return;
        this.api.updateCategory(this.categoryEdit.id, { name: this.categoryEdit.name.trim() }).subscribe({
            next: (res) => this.afterMutation(res.isSuccess, res.message, () => this.loadCategories()),
            error: (err) => this.failFromError(err)
        });
    }

    deleteCategory(): void {
        if (!this.categoryEdit.id) return;
        this.api.deleteCategory(this.categoryEdit.id).subscribe({
            next: (res) => this.afterMutation(res.isSuccess, res.message, () => this.loadCategories()),
            error: (err) => this.failFromError(err)
        });
    }

    loadBooks(): void {
        this.api.getBooks({ pageNumber: 1, pageSize: 50 }).subscribe({
            next: (res) => {
                this.books = res.data.items;
                this.ensurePageInRange('books', this.filteredBooks.length);
            },
            error: (err) => this.failFromError(err)
        });
    }

    fetchBookDetail(): void {
        if (!this.bookDetailId) return;
        this.api.getBook(this.bookDetailId).subscribe({
            next: (res) => (this.bookDetail = res.data),
            error: (err) => this.failFromError(err)
        });
    }

    createBook(): void {
        if (!this.bookForm.title.trim()) return;
        this.api.createBook({
            title: this.bookForm.title.trim(),
            isbn: this.bookForm.isbn.trim() || null,
            totalCopies: Number(this.bookForm.totalCopies),
            authorId: Number(this.bookForm.authorId),
            categoryId: Number(this.bookForm.categoryId)
        }).subscribe({
            next: (res) => this.afterMutation(res.isSuccess, res.message, () => this.loadBooks()),
            error: (err) => this.failFromError(err)
        });
    }

    updateBook(): void {
        if (!this.bookEdit.id || !this.bookEdit.title.trim()) return;
        this.api.updateBook(this.bookEdit.id, {
            title: this.bookEdit.title.trim(),
            isbn: this.bookEdit.isbn.trim() || null,
            totalCopies: Number(this.bookEdit.totalCopies),
            authorId: Number(this.bookEdit.authorId),
            categoryId: Number(this.bookEdit.categoryId)
        }).subscribe({
            next: (res) => this.afterMutation(res.isSuccess, res.message, () => this.loadBooks()),
            error: (err) => this.failFromError(err)
        });
    }

    deleteBook(): void {
        if (!this.bookEdit.id) return;
        this.api.deleteBook(this.bookEdit.id).subscribe({
            next: (res) => this.afterMutation(res.isSuccess, res.message, () => this.loadBooks()),
            error: (err) => this.failFromError(err)
        });
    }

    loadLoans(): void {
        this.api.getLoans({ pageNumber: 1, pageSize: 50 }).subscribe({
            next: (res) => {
                this.loans = res.data.items;
                this.ensurePageInRange('loans', this.filteredLoans.length);
            },
            error: (err) => this.failFromError(err)
        });
    }

    fetchLoanDetail(): void {
        if (!this.loanDetailId) return;
        this.api.getLoan(this.loanDetailId).subscribe({
            next: (res) => (this.loanDetail = res.data),
            error: (err) => this.failFromError(err)
        });
    }

    createLoan(): void {
        this.api.createLoan({ bookId: Number(this.loanForm.bookId), memberId: Number(this.loanForm.memberId) }).subscribe({
            next: (res) => this.afterMutation(res.isSuccess, res.message, () => this.loadLoans()),
            error: (err) => this.failFromError(err)
        });
    }

    returnLoan(): void {
        if (!this.loanDetailId) return;
        this.api.returnLoan(this.loanDetailId).subscribe({
            next: (res) => this.afterMutation(res.isSuccess, res.message, () => {
                this.loadLoans();
                this.fetchLoanDetail();
            }),
            error: (err) => this.failFromError(err)
        });
    }

    deleteLoan(): void {
        if (!this.loanDetailId) return;
        this.api.deleteLoan(this.loanDetailId).subscribe({
            next: (res) => this.afterMutation(res.isSuccess, res.message, () => {
                this.loanDetail = null;
                this.loadLoans();
            }),
            error: (err) => this.failFromError(err)
        });
    }

    loadMembers(): void {
        if (!this.auth.isAdmin()) return;
        this.api.getMembers({ pageNumber: 1, pageSize: 50 }).subscribe({
            next: (res) => {
                this.members = res.data.items;
                this.ensurePageInRange('members', this.filteredMembers.length);
            },
            error: (err) => this.failFromError(err)
        });
    }

    fetchMemberDetail(): void {
        if (!this.memberDetailId || !this.auth.isAdmin()) return;
        this.api.getMember(this.memberDetailId).subscribe({
            next: (res) => {
                this.memberDetail = res.data;
                this.memberEdit = {
                    id: res.data.id,
                    fullName: res.data.fullName,
                    email: res.data.email,
                    role: String(res.data.role)
                };
            },
            error: (err) => this.failFromError(err)
        });
    }

    updateMember(): void {
        if (!this.memberEdit.id || !this.auth.isAdmin()) return;
        this.api.updateMember(this.memberEdit.id, {
            fullName: this.memberEdit.fullName,
            email: this.memberEdit.email,
            role: this.memberEdit.role
        }).subscribe({
            next: (res) => this.afterMutation(res.isSuccess, res.message, () => this.loadMembers()),
            error: (err) => this.failFromError(err)
        });
    }

    deleteMember(): void {
        if (!this.memberEdit.id || !this.auth.isAdmin()) return;
        this.api.deleteMember(this.memberEdit.id).subscribe({
            next: (res) => this.afterMutation(res.isSuccess, res.message, () => this.loadMembers()),
            error: (err) => this.failFromError(err)
        });
    }

    useMyMemberId(): void {
        this.penaltyMemberId = this.auth.memberId();
    }

    loadPenalties(): void {
        if (!this.penaltyMemberId) {
            this.fail('Member id gerekli');
            return;
        }
        this.api.getMemberPenalties(this.penaltyMemberId).subscribe({
            next: (res) => {
                this.penaltyDetail = res.data;
                this.ensurePageInRange('penaltyRows', this.filteredPenaltyRows.length);
                this.notify(res.message || 'Penalty data loaded');
            },
            error: (err) => this.failFromError(err)
        });
    }

    private filterByQuery<T>(items: T[], query: string, project: (item: T) => string): T[] {
        const q = query.trim().toLocaleLowerCase();
        if (!q) return items;
        return items.filter((item) => project(item).toLocaleLowerCase().includes(q));
    }

    private paginate<T>(tab: ListTab, items: T[]): T[] {
        const state = this.pageState[tab];
        const totalPages = Math.max(1, Math.ceil(items.length / state.pageSize));
        if (state.page > totalPages) {
            state.page = totalPages;
        }
        const start = (state.page - 1) * state.pageSize;
        return items.slice(start, start + state.pageSize);
    }

    private ensurePageInRange(tab: ListTab, totalItems: number): void {
        const state = this.pageState[tab];
        const totalPages = Math.max(1, Math.ceil(totalItems / state.pageSize));
        if (state.page > totalPages) state.page = totalPages;
    }

    private afterMutation(isSuccess: boolean, message: string, onSuccess: () => void): void {
        if (!isSuccess) {
            this.fail(message || 'Operation failed');
            return;
        }
        onSuccess();
        this.notify(message || 'Operation successful');
    }

    private notify(message: string): void {
        this.toastType.set('success');
        this.toast.set(message);
    }

    private fail(message: string): void {
        this.toastType.set('error');
        this.toast.set(message);
    }

    private failFromError(err: unknown): void {
        const anyErr = err as { error?: { detail?: string; message?: string } };
        this.fail(anyErr?.error?.detail || anyErr?.error?.message || 'Request failed');
    }
}
