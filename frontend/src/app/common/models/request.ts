export type CommonPaginationPayload = {
  pageSize?: number
  pageNumber?: number
  showAll?: boolean
  searchString?: string
}

export type CommonPaginatedResponse<T> = { total: number, items: T[] }