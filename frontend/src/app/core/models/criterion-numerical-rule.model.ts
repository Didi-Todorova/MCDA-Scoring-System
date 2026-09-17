export enum NumericType {
  Scope = 0,
  Interval = 1,
  TargetValue = 2
}

export enum Direction {
  Minimize = 0,
  Maximize = 1
}

export interface CriterionNumericalRule {
  id: number;
  criterionId: number;
  numericType: NumericType;
  minValue: number;
  maxValue: number;
  targetValue: number | null;
  direction: Direction | null;
}

export interface CreateCriterionNumericalRuleRequest {
  criterionId: number;
  numericType: NumericType;
  minValue: number;
  maxValue: number;
  targetValue: number | null;
  direction: Direction | null;
}

export interface UpdateCriterionNumericalRuleRequest {
  criterionId: number;
  numericType: NumericType;
  minValue: number;
  maxValue: number;
  targetValue: number | null;
  direction: Direction | null;
}