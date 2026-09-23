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
  Alternative
} from '../../../../../core/models/alternative.model';

import {
  AlternativeService
} from '../../../../../core/services/alternative.service';

import {
  Criterion,
  CriterionType
} from '../../../../../core/models/criterion.model';

import {
  CriterionService
} from '../../../../../core/services/criterion.service';

import {
  CriterionOption
} from '../../../../../core/models/criterion-option.model';

import {
  CriterionOptionService
} from '../../../../../core/services/criterion-option.service';

import {
  AlternativeValue
} from '../../../../../core/models/alternative-value.model';

import {
  AlternativeValueService
} from '../../../../../core/services/alternative-value.service';

@Component({
  selector: 'app-preview',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './preview.html',
  styleUrl: './preview.css'
})
export class PreviewComponent implements OnInit {
  private readonly route = inject(ActivatedRoute);

private readonly router = inject(Router);

  private readonly changeDetector =
    inject(ChangeDetectorRef);

  private readonly alternativeService =
    inject(AlternativeService);

  private readonly criterionService =
    inject(CriterionService);

  private readonly optionService =
    inject(CriterionOptionService);

  private readonly alternativeValueService =
    inject(AlternativeValueService);

    readonly openedFromDecisionList =
  this.route.snapshot.queryParamMap.get('fromDecisionList') === 'true';

  alternatives: Alternative[] = [];
  criteria: Criterion[] = [];
  categoricalOptions: CriterionOption[] = [];
  alternativeValues: AlternativeValue[] = [];

  isLoading = true;
  errorMessage = '';

  decisionId: number | null = null;

  readonly CriterionType = CriterionType;

  ngOnInit(): void {
    this.decisionId = this.getDecisionId();

    if (!this.decisionId) {
      this.errorMessage = 'Invalid decision ID.';
      this.isLoading = false;
      this.changeDetector.markForCheck();
      return;
    }

    this.loadData(this.decisionId);
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

  private loadData(decisionId: number): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.alternativeService
      .getAlternatives()
      .subscribe({
        next: (alternatives) => {
          this.alternatives = alternatives
            .filter(
              alternative =>
                alternative.decisionId === decisionId
            )
            .sort((a, b) => a.id - b.id);

          this.loadCriteria(decisionId);
        },

        error: (error) => {
          console.error(
            'Failed to load alternatives',
            error
          );

          this.errorMessage =
            'Unable to load alternatives. Please try again.';

          this.isLoading = false;
          this.changeDetector.markForCheck();
        }
      });
  }

  private loadCriteria(decisionId: number): void {
    this.criterionService
      .getCriteria()
      .subscribe({
        next: (criteria) => {
          this.criteria = criteria
            .filter(
              criterion =>
                criterion.decisionId === decisionId
            )
            .sort((a, b) => a.id - b.id);

          this.loadCategoricalOptions();
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

  private loadCategoricalOptions(): void {
    this.optionService
      .getOptions()
      .subscribe({
        next: (options) => {
          this.categoricalOptions =
            options.filter(option =>
              this.criteria.some(
                criterion =>
                  criterion.id === option.criterionId
              )
            );

          this.loadAlternativeValues();
        },

        error: (error) => {
          console.error(
            'Failed to load categorical options',
            error
          );

          this.errorMessage =
            'Unable to load categorical options. Please try again.';

          this.isLoading = false;
          this.changeDetector.markForCheck();
        }
      });
  }

  private loadAlternativeValues(): void {
    this.alternativeValueService
      .getAlternativeValues()
      .subscribe({
        next: (values) => {
          this.alternativeValues =
            values.filter(value =>
              this.alternatives.some(
                alternative =>
                  alternative.id ===
                  value.alternativeId
              )
            );

          this.isLoading = false;
          this.changeDetector.markForCheck();
        },

        error: (error) => {
          console.error(
            'Failed to load alternative values',
            error
          );

          this.errorMessage =
            'Unable to load alternative values. Please try again.';

          this.isLoading = false;
          this.changeDetector.markForCheck();
        }
      });
  }

  getAlternativeValue(
    alternativeId: number,
    criterionId: number
  ): AlternativeValue | undefined {
    return this.alternativeValues.find(
      value =>
        value.alternativeId === alternativeId &&
        value.criterionId === criterionId
    );
  }

  getCategoricalOptionValue(
    optionId: number | null
  ): string {
    if (optionId === null) {
      return '';
    }

    return (
      this.categoricalOptions.find(
        option => option.id === optionId
      )?.value ?? ''
    );
  }

  formatCriterionValue(
    alternative: Alternative,
    criterion: Criterion
  ): string {
    const value = this.getAlternativeValue(
      alternative.id,
      criterion.id
    );

    if (!value) {
      return 'Not entered';
    }

    if (
      criterion.criterionType ===
      CriterionType.Numerical
    ) {
      if (value.numericValue === null) {
        return 'Not entered';
      }

      const unit = criterion.unit
        ? ` ${criterion.unit}`
        : '';

      return `${value.numericValue}${unit}`;
    }

    const optionValue =
      this.getCategoricalOptionValue(
        value.criterionOptionId
      );

    return optionValue || 'Not entered';
  }

  calculateResult(): void {
    if (!this.decisionId) {
      this.errorMessage = 'Invalid decision ID.';
      return;
    }

    this.router.navigate(
      ['/decisions', this.decisionId, 'results']
    );
  }

  backToDecisions(): void {
  this.router.navigate(['/decisions']);
}
}