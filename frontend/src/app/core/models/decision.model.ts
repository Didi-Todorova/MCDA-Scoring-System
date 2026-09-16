export enum WeightingMethod {
  PercentageAllocation = 0,
  DirectRanking = 1,
  SwingWeighting = 2
}

export interface Decision {
  id: number;
  name: string;
  weightingMethod: WeightingMethod;
}