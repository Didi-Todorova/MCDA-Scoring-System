export interface IntervalRange {
  id: number;
  criterionNumericalRuleId: number;
  minValue: number;
  maxValue: number;
  rank: number;
}

export interface CreateIntervalRangesRequest {
  criterionNumericalRuleId: number;
  ranges: IntervalRangeInput[];
}

export interface IntervalRangeInput {
  minValue: number;
  maxValue: number;
}

export interface IntervalRangeUpdate {
  id: number;
  minValue: number;
  maxValue: number;
}

export interface UpdateIntervalRangesRequest {
  ranges: IntervalRangeUpdate[];
}