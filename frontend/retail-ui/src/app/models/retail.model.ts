export interface User {
  userId: number;
  fullName: string;
  email: string;
  role: string;
}

export interface Category {
  categoryId: number;
  categoryName: string;
  description: string;
}

export interface Brand {
  brandId: number;
  brandName: string;
  description: string;
}

export interface Packaging {
  packagingId: number;
  packagingType: string;
  size: string;
  extraPrice: number;
}

export interface Product {
  productId: number;
  productName: string;
  description: string;
  basePrice: number;
  categoryId: number;
  brandId: number;
  packagingId: number;
  stockQuantity: number;
  isAvailable: boolean;
  category?: Category;
  brand?: Brand;
  packaging?: Packaging;
}

export interface CartItem {
  cartItemId?: number;
  productId: number;
  quantity: number;
  unitPrice: number;
  totalPrice: number;
  product?: Product;
}

export interface Cart {
  cartId: number;
  userId: number;
  cartItems: CartItem[];
}

export interface Order {
  orderId: number;
  userId: number;
  orderDate: Date;
  totalAmount: number;
  orderStatus: string;
  paymentStatus: string;
  deliveryAddress: string;
  orderItems: any[];
}
