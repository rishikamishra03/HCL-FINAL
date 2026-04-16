import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { CartService } from '../../services/cart.service';

@Component({
  selector: 'app-checkout',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './checkout.component.html',
  styleUrls: ['./checkout.component.css']
})
export class CheckoutComponent {
  deliveryAddress = '';
  paymentMethod = 'COD';
  successMessage = '';
  errorMessage = '';
  orderId = 0;

  constructor(private cartService: CartService) {}

  onCheckout() {
    this.cartService.checkout(this.deliveryAddress, this.paymentMethod).subscribe({
      next: (res) => {
        this.successMessage = res.Message || 'Order placed successfully!';
        this.orderId = res.OrderId;
      },
      error: (err) => {
        this.errorMessage = err.error?.message || 'Failed to place order. Cart might be empty.';
      }
    });
  }
}
