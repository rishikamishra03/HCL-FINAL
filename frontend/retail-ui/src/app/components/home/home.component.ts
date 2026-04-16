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
  styleUrl: './home.component.css'
})
export class HomeComponent implements OnInit {
  products: Product[] = [];
  categories: Category[] = [];
  activeCategoryId: number | null = null;
  loading: boolean = false;

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
    this.loading = true;
    this.apiService.getProducts(categoryId).subscribe({
      next: res => {
        this.products = res;
        this.loading = false;
      },
      error: err => {
        console.warn('Products not loaded', err);
        this.loading = false;
      }
    });
  }

  onCategorySelect(catId: number | null): void {
    this.activeCategoryId = catId;
    this.loadProducts(catId !== null ? catId : undefined);
  }

  addToCart(product: Product): void {
    this.cartService.addToCart(product.productId, 1).subscribe({
      next: () => alert(`${product.productName} added to cart!`),
      error: (err) => alert('Please login to add to cart.')
    });
  }
}
