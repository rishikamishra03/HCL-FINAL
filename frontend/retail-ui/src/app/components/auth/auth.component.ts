import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-auth',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './auth.component.html',
  styleUrls: ['./auth.component.css']
})
export class AuthComponent {
  isLoginMode = true;
  errorMessage = '';
  formData = {
    fullName: '',
    email: '',
    password: ''
  };

  constructor(private authService: AuthService) {}

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
