import { HttpClient, HttpParams } from "@angular/common/http";
import { inject, Service } from "@angular/core";
import { GetCategoriesRequestParams, ProductsResponse } from "./models/catalog-list-request-types";
import { CategoryLabel } from "../../../common/constants/product-categories";
import { CommonPaginationPayload } from "../../../common/models/request";

const CatalogListPaths = {
  GetProductCategoriesPath: () => '/Product/categories',
  GetProductsPath: () => '/Product'
}

@Service()
export class CatalogListApi {
  httpClient = inject(HttpClient)

  getProductCategories(payload: GetCategoriesRequestParams = {}) {
    const params = new HttpParams({ fromObject: payload })
    return this.httpClient.get<CategoryLabel[]>(CatalogListPaths.GetProductCategoriesPath(), { params }) 
  }

  getProducts(payload: CommonPaginationPayload = {}) {
    const params = new HttpParams({ fromObject: payload })
    return this.httpClient.get<ProductsResponse>(CatalogListPaths.GetProductsPath(), { params })
  }
    
}