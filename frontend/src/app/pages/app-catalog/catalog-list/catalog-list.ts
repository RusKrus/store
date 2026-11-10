import { Component, inject } from "@angular/core";
import { CatalogListApi } from "./catalog-list-api";
import { AsyncPipe } from "@angular/common";
import { MatButtonModule } from "@angular/material/button";
import { CategoryLabel, PRODUCT_CATEGORIES } from "../../../common/constants/product-categories";
import { ActivatedRoute, Router } from "@angular/router";
import { toSignal } from "@angular/core/rxjs-interop";
import { map } from "rxjs";
import { MatIcon } from "@angular/material/icon";
import { MatChip } from "@angular/material/chips";

@Component({
  selector: 'catalog-list',
  imports: [AsyncPipe, MatButtonModule, MatIcon, MatChip],
  templateUrl: './catalog-list.html',
  styleUrl: './catalog-list.scss'
})
export class CatalogList {
  catalogApi = inject(CatalogListApi)
  route = inject(ActivatedRoute)
  router = inject(Router)

  categories$ = this.catalogApi.getProductCategories()
  products = toSignal(this.catalogApi.getProducts())

  productCategories = PRODUCT_CATEGORIES

  activeCategory = toSignal(this.route.queryParamMap.pipe(map(params => params.get('category'))), { initialValue: 'all' })

  setCategory(category: CategoryLabel) {
    this.router.navigate([], { 
      relativeTo: this.route, 
      queryParams: { category }, 
      replaceUrl: true, 
      queryParamsHandling: 'merge',
    })
  }
}