import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { CartService } from '../../services/cart.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-orders',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './orders.component.html',
  styleUrls: ['./orders.component.css']
})
export class OrdersComponent implements OnInit {
  orders: any[] = [];

  constructor(private http: HttpClient, private cartService: CartService, private router: Router) {}

  ngOnInit(): void {
    this.http.get<any[]>('/api/orders').subscribe({
      next: (res) => this.orders = res,
      error: (err) => console.log('Could not fetch orders', err)
    });
  }

  quickReorder(order: any): void {
    if (!order.orderItems) return;
    
    // Add first item of the past order to cart as an example Quick Reorder
    const firstItem = order.orderItems[0];
    if (firstItem) {
       this.cartService.addToCart(firstItem.productId, firstItem.quantity).subscribe({
         next: () => {
           alert('Items added to cart!');
           this.router.navigate(['/cart']);
         }
       });
    }
  }
}
