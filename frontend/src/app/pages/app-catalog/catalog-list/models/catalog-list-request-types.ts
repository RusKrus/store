import { CategoryLabel } from "../../../../common/constants/product-categories"
import { CommonPaginatedResponse } from "../../../../common/models/request"

export type GetCategoriesRequestParams = {
  showAll?: boolean
}

export type Product = {
  id: number
  name: string
  description: string
  price: number
  quantity: number
  category: CategoryLabel
}

export type ProductsResponse = CommonPaginatedResponse<Product>