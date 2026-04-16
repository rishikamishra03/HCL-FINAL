import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { ApiService } from '../../services/api.service';
import { CartService } from '../../services/cart.service';
import { Product, Category } from '../../models/retail.model';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
  products: Product[] = [];
  categories: Category[] = [];

  constructor(private apiService: ApiService, private cartService: CartService) {}

  ngOnInit(): void {
    this.apiService.getCategories().subscribe({
      next: res => this.categories = res,
      error: err => console.warn('Categories not loaded', err)
    });
    this.loadProducts();
  }

  scrollToMenu(): void {
    window.scrollTo({ top: 500, behavior: 'smooth' });
  }

  loadProducts(categoryId?: number): void {
    this.apiService.getProducts(categoryId).subscribe({
      next: res => this.products = res,
      error: err => console.warn('Products not loaded', err)
    });
  }

  onCategoryChange(event: any): void {
    const catId = event.target.value;
    this.loadProducts(catId ? Number(catId) : undefined);
  }

  addToCart(product: Product): void {
    this.cartService.addToCart(product.productId, 1).subscribe({
      next: () => alert(`${product.productName} added to cart!`),
      error: (err) => alert('Please login to add to cart.')
    });
  }
}
