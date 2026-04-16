import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { CartService } from '../../services/cart.service';
import { Cart } from '../../models/retail.model';

@Component({
  selector: 'app-cart',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './cart.component.html',
  styleUrls: ['./cart.component.css']
})
export class CartComponent implements OnInit {
  cart: Cart | null = null;

  constructor(private cartService: CartService) {}

  ngOnInit() {
    this.cartService.getCart().subscribe({
      next: (res) => this.cart = res,
      error: (err) => console.log('User might not be logged in', err)
    });
  }

  getTotalAmount(): number {
    if (!this.cart || !this.cart.cartItems) return 0;
    return this.cart.cartItems.reduce((acc, item) => acc + item.totalPrice, 0);
  }
}
