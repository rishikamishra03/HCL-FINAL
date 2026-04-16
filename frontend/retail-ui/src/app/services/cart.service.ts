import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Cart } from '../models/retail.model';

@Injectable({
  providedIn: 'root'
})
export class CartService {
  private baseUrl = '/api/cart';

  constructor(private http: HttpClient) { }

  getCart(): Observable<Cart> {
    return this.http.get<Cart>(this.baseUrl);
  }

  addToCart(productId: number, quantity: number): Observable<any> {
    return this.http.post(`${this.baseUrl}/items`, { productId, quantity });
  }

  checkout(deliveryAddress: string, paymentMethod: string): Observable<any> {
    return this.http.post('/api/orders/checkout', { deliveryAddress, paymentMethod });
  }
}
