import { Routes } from '@angular/router';
import { AppCatalog } from './pages/app-catalog/app-catalog';
import { ProductPage } from './pages/product-page/product-page';

export const routes: Routes = [
  {
    path: '',
    title: 'Catalog',
    component: AppCatalog,
    data: { breadCrumb: 'Home' }
  },
  {
    path: 'product:id',
    component: ProductPage,
    data: { breadCrumb: 'Home' }
  }
];