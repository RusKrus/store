import { Routes } from '@angular/router';
import { AppCatalog } from './pages/app-catalog/app-catalog';
import { ProductPage } from './pages/product-page/product-page';
import { LoginPage } from './pages/login-page/login-page';

export const routes: Routes = [
  {
    path: '',
    title: 'Catalog',
    component: AppCatalog,
  },
  {
    path: 'product/:id',
    component: ProductPage,
  },
  {
    path: 'login',
    title: 'Welcome back!',
    component: LoginPage
  }
];
