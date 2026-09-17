import {
  ChangeDetectorRef,
  Component,
  inject,
  OnInit
} from '@angular/core';

import {
  ActivatedRoute,
  Router,
  RouterLink
} from '@angular/router';

import {
  Criterion,
  CriterionType
} from '../../../../../core/models/criterion.model';

import {
  CriterionService
} from '../../../../../core/services/criterion.service';

import {
  Decision,
  WeightingMethod
} from '../../../../../core/models/decision.model';

import {
  DecisionService
} from '../../../../../core/services/decision.service';

import {
  WeightingService
} from '../../../../../core/services/weighting.service';

@Component({
  selector: 'app-weighting',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './weighting.html',
  styleUrl: './weighting.css'
})
export class WeightingComponent implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly decisionService = inject(DecisionService);
  private readonly criterionService = inject(CriterionService);
  private readonly weightingService = inject(WeightingService);
  private readonly changeDetector = inject(ChangeDetectorRef);

  readonly WeightingMethod = WeightingMethod;
  readonly CriterionType = CriterionType;

  decision: Decision | null = null;
  criteria: Criterion[] = [];

  isLoading = true;
  isSaving = false;
  errorMessage = '';

  percentageWeights: {
    criterionId: number;
    weight: number;
  }[] = [];

  rankingValues: {
    criterionId: number;
    rank: number;
  }[] = [];

  ngOnInit(): void {
    const decisionId = this.getDecisionId();

    if (!decisionId) {
      this.errorMessage = 'Invalid decision ID.';
      this.isLoading = false;
      this.changeDetector.markForCheck();
      return;
    }

    this.loadDecision(decisionId);
  }

  private getDecisionId(): number | null {
    let route: ActivatedRoute | null = this.route;

    while (route) {
      const id = route.snapshot.paramMap.get('id');

      if (id) {
        const decisionId = Number(id);

        if (decisionId) {
          return decisionId;
        }
      }

      route = route.parent;
    }

    return null;
  }

  private loadDecision(id: number): void {
    this.decisionService.getDecision(id).subscribe({
      next: (decision) => {
        this.decision = decision;
        this.loadCriteria(id);
      },

      error: (error) => {
        console.error(
          'Failed to load decision',
          error
        );

        this.errorMessage =
          'Unable to load the decision. Please try again.';

        this.isLoading = false;
        this.changeDetector.markForCheck();
      }
    });
  }

  private loadCriteria(decisionId: number): void {
    this.criterionService.getCriteria().subscribe({
      next: (criteria) => {
        this.criteria = criteria
          .filter(
            criterion =>
              criterion.decisionId === decisionId
          )
          .sort((a, b) => a.id - b.id);

        this.initializeValues();

        this.isLoading = false;
        this.changeDetector.markForCheck();
      },

      error: (error) => {
        console.error(
          'Failed to load criteria',
          error
        );

        this.errorMessage =
          'Unable to load criteria. Please try again.';

        this.isLoading = false;
        this.changeDetector.markForCheck();
      }
    });
  }

  private initializeValues(): void {
    this.percentageWeights =
      this.criteria.map(criterion => ({
        criterionId: criterion.id,
        weight: Math.round(
          (criterion.weight ?? 0) * 100
        )
      }));

    this.rankingValues =
      this.criteria.map((criterion, index) => ({
        criterionId: criterion.id,
        rank: index + 1
      }));
  }

  get currentWeightingMethod(): WeightingMethod | null {
    return this.decision?.weightingMethod ?? null;
  }

  get totalPercentage(): number {
    return this.percentageWeights.reduce(
      (total, item) => total + Number(item.weight || 0),
      0
    );
  }

  updatePercentageWeight(
    criterionId: number,
    event: Event
  ): void {
    const value = Number(
      (event.target as HTMLInputElement).value
    );

    this.percentageWeights =
      this.percentageWeights.map(item =>
        item.criterionId === criterionId
          ? {
              ...item,
              weight: Number.isFinite(value)
                ? value
                : 0
            }
          : item
      );
  }

  updateRank(
    criterionId: number,
    event: Event
  ): void {
    const value = Number(
      (event.target as HTMLInputElement).value
    );

    this.rankingValues =
      this.rankingValues.map(item =>
        item.criterionId === criterionId
          ? {
              ...item,
              rank: Number.isFinite(value)
                ? value
                : 0
            }
          : item
      );
  }

  saveAndContinue(): void {
    if (!this.decision) {
      return;
    }

    if (
      this.decision.weightingMethod ===
      WeightingMethod.PercentageAllocation
    ) {
      this.savePercentageAllocation();
      return;
    }

    if (
      this.decision.weightingMethod ===
      WeightingMethod.DirectRanking
    ) {
      this.saveDirectRanking();
      return;
    }

    this.errorMessage =
      'This weighting method is not supported yet.';
  }

  private savePercentageAllocation(): void {
    if (this.totalPercentage !== 100) {
      this.errorMessage =
        `Weights must total 100%. Current total: ${this.totalPercentage}%.`;
      return;
    }

    if (
      this.percentageWeights.some(
        item =>
          item.weight < 0 ||
          item.weight > 100
      )
    ) {
      this.errorMessage =
        'Each weight must be between 0% and 100%.';
      return;
    }

    this.isSaving = true;
    this.errorMessage = '';

    this.weightingService
      .setPercentageAllocation(
        this.decision!.id,
        this.percentageWeights
      )
      .subscribe({
        next: () => {
          this.isSaving = false;
          this.goToNextStep();
        },

        error: (error) => {
          console.error(
            'Failed to save percentage allocation',
            error
          );

          this.errorMessage =
            error?.error?.Error ??
            'Unable to save the criterion weights. Please try again.';

          this.isSaving = false;
          this.changeDetector.markForCheck();
        }
      });
  }

  private saveDirectRanking(): void {
    const ranks =
      this.rankingValues.map(item => item.rank);

    const numberOfCriteria =
      this.criteria.length;

    if (
      this.rankingValues.some(
        item =>
          item.rank < 1 ||
          item.rank > numberOfCriteria
      )
    ) {
      this.errorMessage =
        `Ranks must be between 1 and ${numberOfCriteria}.`;
      return;
    }

    if (
      new Set(ranks).size !== ranks.length
    ) {
      this.errorMessage =
        'Each criterion must have a unique rank.';
      return;
    }

    this.isSaving = true;
    this.errorMessage = '';

    this.weightingService
      .setDirectRanking(
        this.decision!.id,
        this.rankingValues
      )
      .subscribe({
        next: () => {
          this.isSaving = false;
          this.goToNextStep();
        },

        error: (error) => {
          console.error(
            'Failed to save direct ranking',
            error
          );

          this.errorMessage =
            error?.error?.Error ??
            'Unable to save the criterion ranking. Please try again.';

          this.isSaving = false;
          this.changeDetector.markForCheck();
        }
      });
  }

  private goToNextStep(): void {
    this.router.navigate(
      ['../alternatives'],
      { relativeTo: this.route }
    );
  }
}