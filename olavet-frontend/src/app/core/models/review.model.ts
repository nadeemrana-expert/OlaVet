// =============================================
// Review models — maps to OlaVet.Application.DTOs.Review
// =============================================

export interface Review {
  reviewId: number;
  rating: number;
  comments?: string;       // backend sends 'comments'
  reviewDateTime: string;  // backend sends 'reviewDateTime'
  ownerName: string;       // backend sends 'ownerName'
  // Dashboard recent-reviews also sends these:
  reviewType?: string;     // 'Vet' | 'Lab' | 'Store'
  entityId?: number;
  entityName?: string;
}

export interface VetReview extends Review {
  vetReviewId: number;
  vetId: number;
  vetName: string;
}

export interface LabReview extends Review {
  labReviewId: number;
  labId: number;
  labName: string;
}

export interface StoreReview extends Review {
  storeReviewId: number;
  storeId: number;
  storeName: string;
}

export interface CreateVetReviewRequest {
  vetId: number;
  petOwnerId: number;
  vetAppointmentId: number;
  rating: number;
  comment?: string;
}

export interface CreateLabReviewRequest {
  labId: number;
  petOwnerId: number;
  labAppointmentId: number;
  rating: number;
  comment?: string;
}

export interface CreateStoreReviewRequest {
  storeId: number;
  petOwnerId: number;
  medicineOrderId: number;
  rating: number;
  comment?: string;
}

export interface RatingDistribution {
  fiveStars: number;
  fourStars: number;
  threeStars: number;
  twoStars: number;
  oneStar: number;
  totalReviews: number;
  averageRating: number;
}
