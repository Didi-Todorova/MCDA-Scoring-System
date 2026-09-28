import {
  ChangeDetectorRef,
  Component,
  inject,
  OnDestroy,
  OnInit
} from '@angular/core';

import { CommonModule } from '@angular/common';

import {
  ActivatedRoute,
  Router,
  RouterLink
} from '@angular/router';

import {
  DecisionService
} from '../../../../../core/services/decision.service';

import {
  DecisionScore
} from '../../../../../core/models/decision-score.model';

@Component({
  selector: 'app-results',
  standalone: true,
  imports: [
    CommonModule,
    RouterLink
  ],
  templateUrl: './results.html',
  styleUrl: './results.css'
})
export class ResultsComponent implements OnInit, OnDestroy {
  private readonly route =
    inject(ActivatedRoute);

    private readonly router = inject(Router);

  private readonly decisionService =
    inject(DecisionService);

  private readonly changeDetector =
    inject(ChangeDetectorRef);

    readonly openedFromDecisionList =
  this.route.snapshot.queryParamMap.get('fromDecisionList') === 'true';

  private errorTimeout: ReturnType<typeof setTimeout> | null = null;

  results: DecisionScore[] = [];

  isLoading = true;
  errorMessage = '';

  decisionId: number | null = null;

  ngOnInit(): void {
    this.decisionId = this.getDecisionId();

    if (!this.decisionId) {
      this.showError('Invalid decision ID.');

      this.isLoading = false;

      this.changeDetector.markForCheck();

      return;
    }

    this.loadResults(this.decisionId);
  }

  ngOnDestroy(): void {
    if (this.errorTimeout) {
      clearTimeout(this.errorTimeout);
      this.errorTimeout = null;
    }
  }

  private getDecisionId(): number | null {
    let route: ActivatedRoute | null =
      this.route;

    while (route) {
      const id =
        route.snapshot.paramMap.get('id');

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

  private loadResults(
    decisionId: number
  ): void {
    this.isLoading = true;
    this.showError('');

    this.decisionService
      .getDecisionScore(decisionId)
      .subscribe({
        next: (results) => {
          this.results = results;

          this.isLoading = false;

          this.changeDetector.markForCheck();
        },

        error: (error) => {
          console.error(
            'Failed to load decision results',
            error
          );

          this.showError(error?.error?.Error ??
            'Unable to calculate the results. Please try again.');

          this.isLoading = false;

          this.changeDetector.markForCheck();
        }
      });
  }

  private showError(message: string): void {
    if (this.errorTimeout) {
      clearTimeout(this.errorTimeout);
      this.errorTimeout = null;
    }

    this.errorMessage = message;

    if (!message) {
      this.changeDetector.markForCheck();
      return;
    }

    this.errorTimeout = setTimeout(() => {
      this.errorMessage = '';
      this.errorTimeout = null;
      this.changeDetector.detectChanges();
    }, 4000);
  }

  getScorePercentage(
    score: number
  ): number {
    if (this.results.length === 0) {
      return 0;
    }

    const maxScore =
      this.results[0].score;

    if (maxScore <= 0) {
      return 0;
    }

    return Math.min(
      100,
      Math.max(
        0,
        (score / maxScore) * 100
      )
    );
  }

  back(): void {
    if (!this.decisionId) {
      return;
    }

    if (this.openedFromDecisionList) {
      this.router.navigate(
        [
          '/decisions',
          this.decisionId,
          'wizard',
          'preview'
        ],
        {
          queryParams: {
            fromDecisionList: 'true'
          }
        }
      );

      return;
    }

    this.router.navigate([
      '/decisions',
      this.decisionId,
      'wizard',
      'preview'
    ]);
  }
}