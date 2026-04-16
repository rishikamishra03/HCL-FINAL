import { Routes } from '@angular/router';
import { HomeComponent } from './components/home/home.component';
import { AuthComponent } from './components/auth/auth.component';
import { CartComponent } from './components/cart/cart.component';
import { CheckoutComponent } from './components/checkout/checkout.component';
import { OrdersComponent } from './components/orders/orders.component';

export const routes: Routes = [
    { path: '', component: HomeComponent },
    { path: 'auth', component: AuthComponent },
    { path: 'cart', component: CartComponent },
    { path: 'checkout', component: CheckoutComponent },
    { path: 'orders', component: OrdersComponent },
    { path: '**', redirectTo: '' }
];
