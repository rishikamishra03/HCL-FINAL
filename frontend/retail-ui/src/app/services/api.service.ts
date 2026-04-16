import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Product, Category, Brand } from '../models/retail.model';

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  private baseUrl = '/api/products';

  constructor(private http: HttpClient) { }

  getProducts(categoryId?: number, brandId?: number): Observable<Product[]> {
    let params = new HttpParams();
    if (categoryId) params = params.set('categoryId', categoryId.toString());
    if (brandId) params = params.set('brandId', brandId.toString());
    
    return this.http.get<Product[]>(this.baseUrl, { params });
  }

  getProductById(id: number): Observable<Product> {
    return this.http.get<Product>(`${this.baseUrl}/${id}`);
  }

  getCategories(): Observable<Category[]> {
    return this.http.get<Category[]>(`${this.baseUrl}/categories`);
  }

  getBrands(): Observable<Brand[]> {
    return this.http.get<Brand[]>(`${this.baseUrl}/brands`);
  }
}
