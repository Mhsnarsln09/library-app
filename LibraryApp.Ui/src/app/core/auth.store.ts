import { Injectable, computed, signal } from '@angular/core';
import { AuthResponse, UserRole } from '../models/api.models';

interface SessionState {
  token: string | null;
  email: string | null;
  fullName: string | null;
  role: UserRole | null;
  memberId: number | null;
}

const STORAGE_KEY = 'libraryapp.ui.session';

@Injectable({ providedIn: 'root' })
export class AuthStore {
  private readonly state = signal<SessionState>(this.loadInitialState());

  readonly token = computed(() => this.state().token);
  readonly role = computed(() => this.state().role);
  readonly fullName = computed(() => this.state().fullName);
  readonly email = computed(() => this.state().email);
  readonly memberId = computed(() => this.state().memberId);
  readonly isAuthenticated = computed(() => !!this.state().token);
  readonly isAdmin = computed(() => this.state().role === 'Admin');

  save(auth: AuthResponse): void {
    const next: SessionState = {
      token: auth.token,
      email: auth.email,
      fullName: auth.fullName,
      role: auth.role,
      memberId: this.extractMemberId(auth.token)
    };

    this.state.set(next);
    localStorage.setItem(STORAGE_KEY, JSON.stringify(next));
  }

  logout(): void {
    this.state.set({ token: null, email: null, fullName: null, role: null, memberId: null });
    localStorage.removeItem(STORAGE_KEY);
  }

  private loadInitialState(): SessionState {
    try {
      const raw = localStorage.getItem(STORAGE_KEY);
      if (!raw) {
        return { token: null, email: null, fullName: null, role: null, memberId: null };
      }
      const parsed = JSON.parse(raw) as SessionState;
      return {
        token: parsed.token ?? null,
        email: parsed.email ?? null,
        fullName: parsed.fullName ?? null,
        role: parsed.role ?? null,
        memberId: parsed.memberId ?? this.extractMemberId(parsed.token)
      };
    } catch {
      return { token: null, email: null, fullName: null, role: null, memberId: null };
    }
  }

  private extractMemberId(token: string | null): number | null {
    if (!token) return null;
    const payload = token.split('.')[1];
    if (!payload) return null;

    try {
      const normalized = payload.replace(/-/g, '+').replace(/_/g, '/');
      const decoded = JSON.parse(atob(normalized)) as Record<string, unknown>;
      const raw = decoded['nameid'] ?? decoded['sub'] ?? decoded['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'];
      const asNumber = Number(raw);
      return Number.isFinite(asNumber) ? asNumber : null;
    } catch {
      return null;
    }
  }
}
