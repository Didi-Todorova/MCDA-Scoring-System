export interface Alternative {
  id: number;
  name: string;
  decisionId: number;
}

export interface CreateAlternativeRequest {
  decisionId: number;
  name: string;
}

export interface UpdateAlternativeRequest {
  name: string;
  decisionId: number;
}

export interface PatchAlternativeRequest {
  name?: string;
  decisionId?: number;
}