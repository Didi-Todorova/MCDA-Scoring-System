export interface AlternativeValue {
  id: number;
  alternativeId: number;
  criterionId: number;
  numericValue: number | null;
  criterionOptionId: number | null;
}

export interface CreateAlternativeValueRequest {
  alternativeId: number;
  criterionId: number;
  numericValue: number | null;
  criterionOptionId: number | null;
}

export interface UpdateAlternativeValueRequest {
  alternativeId: number;
  criterionId: number;
  numericValue: number | null;
  criterionOptionId: number | null;
}

export interface PatchAlternativeValueRequest {
  alternativeId?: number;
  criterionId?: number;
  numericValue?: number | null;
  criterionOptionId?: number | null;
}