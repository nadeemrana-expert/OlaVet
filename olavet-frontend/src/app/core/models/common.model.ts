// =============================================
// Shared / Common models
// =============================================

/** Generic paged result — maps to PagedResult<T> */
export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
  hasPrevious: boolean;
  hasNext: boolean;
}

/** Standard API error shape */
export interface ApiError {
  error: string;
  errors?: string[];
  message?: string;
}

/** Pagination query params */
export interface PaginationParams {
  page: number;
  pageSize: number;
}
