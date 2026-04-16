import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CartService } from '../../services/cart.service';
import { AuthService } from '../../services/auth.service';
import { Cart } from '../../models/retail.model';

@Component({
  selector: 'app-cart',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './cart.component.html',
  styleUrl: './cart.component.css'
})
export class CartComponent implements OnInit {
  cart: Cart | null = null;
  couponCode: string = '';
  discount: number = 0;
  loyaltyPoints: number = 0;

  constructor(private cartService: CartService, private authService: AuthService) {}

  ngOnInit() {
    this.cartService.getCart().subscribe({
      next: (res) => this.cart = res,
      error: (err) => console.log('User might not be logged in', err)
    });

    this.authService.getProfile().subscribe({
      next: (res) => this.loyaltyPoints = res.loyaltyPoints,
      error: (err) => console.log('Error fetching loyalty points', err)
    });
  }

  applyCoupon(): void {
    if (this.loyaltyPoints < 500) {
      alert(`You need at least 500 loyalty points to use coupons. Your current balance: ${this.loyaltyPoints} pts`);
      return;
    }

    const code = this.couponCode.toUpperCase().trim();
    if (code === 'WELCOME10') {
      this.discount = this.getSubtotal() * 0.1;
      alert('Coupon WELCOME10 applied! 10% discount subtracted.');
    } else if (code === 'FREEDRINK') {
      this.discount = 2.00;
      alert('Coupon FREEDRINK applied! $2.00 discount subtracted.');
    } else {
      this.discount = 0;
      alert('Invalid coupon code.');
    }
  }

  getSubtotal(): number {
    if (!this.cart || !this.cart.cartItems) return 0;
    return this.cart.cartItems.reduce((acc, item) => acc + item.totalPrice, 0);
  }

  getTotalAmount(): number {
    return Math.max(0, this.getSubtotal() - this.discount);
  }
}
