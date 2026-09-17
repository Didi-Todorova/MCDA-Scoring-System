export interface CriterionOption {
  id: number;
  criterionId: number;
  value: string;
  rank: number;
}

export interface CreateCriterionOptionsRequest {
  criterionId: number;
  options: string[];
}

export interface CriterionOptionOrder {
  id: number;
  value: string;
}

export interface UpdateCriterionOptionsRequest {
  options: CriterionOptionOrder[];
}