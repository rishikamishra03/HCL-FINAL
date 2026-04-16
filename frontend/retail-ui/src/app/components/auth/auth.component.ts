import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-auth',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './auth.component.html',
  styleUrl: './auth.component.css'
})
export class AuthComponent implements OnInit {
  isLoginMode = true;
  errorMessage = '';
  formData = {
    fullName: '',
    email: '',
    password: ''
  };

  constructor(private authService: AuthService, private router: Router) {}

  ngOnInit() {
    if (this.authService.isLoggedIn()) {
      this.router.navigate(['/dashboard']);
    }
  }

  toggleMode() {
    this.isLoginMode = !this.isLoginMode;
  }

  onSubmit() {
    if (this.isLoginMode) {
      this.authService.login({ email: this.formData.email, password: this.formData.password })
        .subscribe({
          error: (err) => this.errorMessage = 'Login failed. Invalid credentials.'
        });
    } else {
      this.authService.register(this.formData)
        .subscribe({
          next: () => {
            this.isLoginMode = true;
            this.errorMessage = 'Registration successful! Please login.';
          },
          error: (err) => this.errorMessage = 'Registration failed. Email might exist.'
        });
    }
  }
}
