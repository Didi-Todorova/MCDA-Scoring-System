export enum CriterionType {
  Numerical = 0,
  Categorical = 1
}

export interface Criterion {
  id: number;
  decisionId: number;
  name: string;
  criterionType: CriterionType;
  weight: number | null;
}

export interface CreateCriterionRequest {
  decisionId: number;
  name: string;
  criterionType: CriterionType;
  weight: number | null;
}