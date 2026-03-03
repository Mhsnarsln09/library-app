import { CommonModule } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { ApiClientService } from '../../core/api-client.service';
import { AuthStore } from '../../core/auth.store';

@Component({
    selector: 'app-auth-page',
    standalone: true,
    imports: [CommonModule, FormsModule],
    templateUrl: './auth-page.component.html',
    styleUrl: './auth-page.component.css'
})
export class AuthPageComponent {
    private readonly api = inject(ApiClientService);
    private readonly auth = inject(AuthStore);
    private readonly router = inject(Router);

    readonly mode = signal<'login' | 'register'>('login');
    readonly loading = signal(false);
    readonly message = signal('');
    readonly messageType = signal<'success' | 'error'>('success');

    apiBaseUrl = this.api.baseUrl;
    email = 'admin@library.com';
    password = 'admin123';
    registerForm = { fullName: '' };

    constructor() {
        if (this.auth.isAuthenticated()) {
            void this.router.navigateByUrl('/');
        }
    }

    submit(): void {
        this.message.set('');
        this.api.setBaseUrl(this.apiBaseUrl.trim() || 'http://localhost:5098');
        this.loading.set(true);

        if (this.mode() === 'login') {
            this.api.login({ email: this.email, password: this.password }).subscribe({
                next: (res) => {
                    this.loading.set(false);
                    if (!res.isSuccess) {
                        this.fail(res.message || 'Login failed');
                        return;
                    }
                    this.auth.save(res.data);
                    this.success('Login successful');
                    void this.router.navigateByUrl('/');
                },
                error: (err) => {
                    this.loading.set(false);
                    this.fail(err?.error?.detail || err?.error?.message || 'API connection failed');
                }
            });
            return;
        }

        this.api.register({ fullName: this.registerForm.fullName, email: this.email, password: this.password }).subscribe({
            next: (res) => {
                this.loading.set(false);
                if (!res.isSuccess) {
                    this.fail(res.message || 'Register failed');
                    return;
                }
                this.auth.save(res.data);
                this.success('Account created');
                void this.router.navigateByUrl('/');
            },
            error: (err) => {
                this.loading.set(false);
                this.fail(err?.error?.detail || err?.error?.message || 'API connection failed');
            }
        });
    }

    private success(msg: string): void {
        this.messageType.set('success');
        this.message.set(msg);
    }

    private fail(msg: string): void {
        this.messageType.set('error');
        this.message.set(msg);
    }
}
