import {
  ChangeDetectorRef,
  Component,
  inject,
  OnInit
} from '@angular/core';

import {
  FormBuilder,
  ReactiveFormsModule,
  Validators
} from '@angular/forms';

import {
  ActivatedRoute,
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
  CriterionNumericalRule,
  NumericType,
  Direction
} from '../../../../../core/models/criterion-numerical-rule.model';

import {
  CriterionNumericalRuleService
} from '../../../../../core/services/criterion-numerical-rule.service';

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
  selector: 'app-alternatives',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './alternatives.html',
  styleUrl: './alternatives.css'
})
export class AlternativesComponent implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly alternativeService = inject(AlternativeService);
  private readonly formBuilder = inject(FormBuilder);
  private readonly changeDetector = inject(ChangeDetectorRef);
  private readonly criterionService = inject(CriterionService);
  private readonly numericalRuleService = inject(CriterionNumericalRuleService);
  private readonly optionService = inject(CriterionOptionService);
  private readonly alternativeValueService = inject(AlternativeValueService);

  alternatives: Alternative[] = [];
  criteria: Criterion[] = [];
  numericalRules: CriterionNumericalRule[] = [];
  categoricalOptions: CriterionOption[] = [];
  alternativeValues: AlternativeValue[] = [];

  isLoading = true;
  isSaving = false;
  errorMessage = '';
  showForm = false;

  decisionId: number | null = null;
  savedValueKey: string | null = null;

  readonly CriterionType = CriterionType;
  readonly NumericType = NumericType;
  readonly Direction = Direction;
  readonly alternativeForm =
    this.formBuilder.nonNullable.group({
      name: [
        '',
        [
          Validators.required,
          Validators.maxLength(200)
        ]
      ]
    });

  ngOnInit(): void {
    this.decisionId = this.getDecisionId();

    if (!this.decisionId) {
      this.errorMessage = 'Invalid decision ID.';
      this.isLoading = false;
      this.changeDetector.markForCheck();
      return;
    }

    this.loadAlternatives(this.decisionId);
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

    private loadAlternatives(decisionId: number): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.alternativeService.getAlternatives().subscribe({
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
    this.criterionService.getCriteria().subscribe({
      next: (criteria) => {
        this.criteria = criteria
          .filter(
            criterion =>
              criterion.decisionId === decisionId
          )
          .sort((a, b) => a.id - b.id);

        this.loadNumericalRules();
        this.loadCategoricalOptions();
        this.loadAlternativeValues();
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

    private loadNumericalRules(): void {
    this.numericalRuleService.getRules().subscribe({
      next: (rules) => {
        this.numericalRules = rules.filter(rule =>
          this.criteria.some(
            criterion =>
              criterion.id === rule.criterionId
          )
        );

        this.changeDetector.markForCheck();
      },

      error: (error) => {
        console.error(
          'Failed to load numerical rules',
          error
        );

        this.errorMessage =
          'Unable to load numerical configuration. Please try again.';

        this.isLoading = false;
        this.changeDetector.markForCheck();
      }
    });
  }

  private loadCategoricalOptions(): void {
  this.optionService.getOptions().subscribe({
    next: (options) => {
      this.categoricalOptions = options.filter(option =>
        this.criteria.some(
          criterion =>
            criterion.id === option.criterionId
        )
      );

      this.changeDetector.markForCheck();
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
        this.alternativeValues = values.filter(value =>
          this.alternatives.some(
            alternative =>
              alternative.id === value.alternativeId
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

  openForm(): void {
    this.alternativeForm.reset({
      name: ''
    });

    this.errorMessage = '';
    this.showForm = true;
  }

  cancelForm(): void {
    this.showForm = false;
    this.errorMessage = '';
  }

  addAlternative(): void {
    if (this.alternativeForm.invalid) {
      this.alternativeForm.markAllAsTouched();
      return;
    }

    if (!this.decisionId) {
      this.errorMessage = 'Invalid decision ID.';
      return;
    }

    const name =
      this.alternativeForm.controls.name.value.trim();

    if (!name) {
      this.alternativeForm.controls.name.setErrors({
        required: true
      });
      return;
    }

    this.isSaving = true;
    this.errorMessage = '';

    this.alternativeService
      .createAlternative({
        decisionId: this.decisionId,
        name
      })
      .subscribe({
        next: (alternative) => {
          this.alternatives = [
            ...this.alternatives,
            alternative
          ];

          this.alternativeForm.reset({
            name: ''
          });

          this.showForm = false;
          this.isSaving = false;

          this.changeDetector.markForCheck();
        },

        error: (error) => {
          console.error(
            'Failed to create alternative',
            error
          );

          this.errorMessage =
            error?.error?.Error ??
            'Unable to create the alternative. Please try again.';

          this.isSaving = false;
          this.changeDetector.markForCheck();
        }
      });
  }

  alternativeToDelete: Alternative | null = null;

    confirmDeleteAlternative(
    alternative: Alternative
    ): void {
    this.alternativeToDelete = alternative;
    this.errorMessage = '';
    this.changeDetector.markForCheck();
    }

    cancelDelete(): void {
    this.alternativeToDelete = null;
    this.changeDetector.markForCheck();
    }

    deleteAlternative(): void {
    if (!this.alternativeToDelete) {
        return;
    }

    const alternative = this.alternativeToDelete;

    this.isSaving = true;
    this.errorMessage = '';

    this.alternativeService
        .deleteAlternative(alternative.id)
        .subscribe({
        next: () => {
            this.alternatives =
            this.alternatives.filter(
                item => item.id !== alternative.id
            );

            this.alternativeToDelete = null;
            this.isSaving = false;

            this.changeDetector.markForCheck();
        },

        error: (error) => {
            console.error(
            'Failed to delete alternative',
            error
            );

            this.errorMessage =
            error?.error?.Error ??
            'Unable to delete the alternative. Please try again.';

            this.isSaving = false;
            this.changeDetector.markForCheck();
        }
        });
    }

    saveNumericValue(
      alternativeId: number,
      criterionId: number,
      event: Event
    ): void {
      const input = event.target as HTMLInputElement;

      const rawValue = input.value.trim();

      if (!rawValue) {
        return;
      }

      const numericValue = Number(rawValue);

      if (Number.isNaN(numericValue)) {
        return;
      }

      const existingValue = this.getAlternativeValue(
        alternativeId,
        criterionId
      );

      const request = {
        alternativeId,
        criterionId,
        numericValue,
        criterionOptionId: null
      };

      if (existingValue) {

        this.alternativeValueService
          .updateAlternativeValue(
            existingValue.id,
            request
          )
          .subscribe({
            next: () => {
              this.alternativeValues =
                this.alternativeValues.map(value =>
                  value.id === existingValue.id
                    ? {
                        ...value,
                        numericValue,
                        criterionOptionId: null
                      }
                    : value
                );

                this.showSavedValue(
                  alternativeId,
                  criterionId
                );

              this.changeDetector.markForCheck();
            },

            error: (error) => {
              console.error(
                'Failed to update alternative value',
                error
              );

              this.errorMessage =
                error?.error?.Error ??
                'Unable to save the value. Please try again.';

              this.changeDetector.markForCheck();
            }
          });

        return;
      }

      this.alternativeValueService
        .createAlternativeValue(request)
        .subscribe({
          next: (value) => {
            this.alternativeValues = [
              ...this.alternativeValues,
              value
            ];

            this.showSavedValue(
              alternativeId,
              criterionId
            );

            this.changeDetector.markForCheck();
          },

          error: (error) => {
            console.error(
              'Failed to create alternative value',
              error
            );

            this.errorMessage =
              error?.error?.Error ??
              'Unable to save the value. Please try again.';

            this.changeDetector.markForCheck();
          }
        });
    }

    saveCategoricalValue(
        alternativeId: number,
        criterionId: number,
        event: Event
      ): void {
        const select = event.target as HTMLSelectElement;

        const rawValue = select.value;

        if (!rawValue) {
          return;
        }

        const criterionOptionId = Number(rawValue);

        if (Number.isNaN(criterionOptionId)) {
          return;
        }

        const existingValue = this.getAlternativeValue(
          alternativeId,
          criterionId
        );

        const request = {
          alternativeId,
          criterionId,
          numericValue: null,
          criterionOptionId
        };

        if (existingValue) {

          this.alternativeValueService
            .updateAlternativeValue(
              existingValue.id,
              request
            )
            .subscribe({
              next: () => {
                this.alternativeValues =
                  this.alternativeValues.map(value =>
                    value.id === existingValue.id
                      ? {
                          ...value,
                          numericValue: null,
                          criterionOptionId
                        }
                      : value
                  );

                this.showSavedValue(
                  alternativeId,
                  criterionId
                );

                this.changeDetector.markForCheck();
},

              error: (error) => {
                console.error(
                  'Failed to update categorical value',
                  error
                );

                this.errorMessage =
                  error?.error?.Error ??
                  'Unable to save the value. Please try again.';

                this.changeDetector.markForCheck();
              }
            });

          return;
        }

        this.alternativeValueService
          .createAlternativeValue(request)
          .subscribe({
            next: (value) => {
              this.alternativeValues = [
                ...this.alternativeValues,
                value
              ];

              this.showSavedValue(
                alternativeId,
                criterionId
              );

              this.changeDetector.markForCheck();
            },

            error: (error) => {
              console.error(
                'Failed to create categorical value',
                error
              );

              this.errorMessage =
                error?.error?.Error ??
                'Unable to save the value. Please try again.';

              this.changeDetector.markForCheck();
            }
          });
      }

      showSavedValue(
      alternativeId: number,
      criterionId: number
    ): void {
      const key = this.getValueKey(
        alternativeId,
        criterionId
      );

      this.savedValueKey = key;

      setTimeout(() => {
        if (this.savedValueKey === key) {
          this.savedValueKey = null;
          this.changeDetector.markForCheck();
        }
      }, 1500);
    }

    getNumericalRule(
  criterionId: number
): CriterionNumericalRule | undefined {
  return this.numericalRules.find(
    rule => rule.criterionId === criterionId
  );
    }

    getCategoricalOptions(
      criterionId: number
    ): CriterionOption[] {
      return this.categoricalOptions
        .filter(
          option => option.criterionId === criterionId
        )
        .sort((a, b) => a.rank - b.rank);
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

    getNumericValue(
  alternativeId: number,
  criterionId: number
): number | null {
  const value = this.getAlternativeValue(
    alternativeId,
    criterionId
  );

  return value?.numericValue ?? null;
    }

    getCategoricalOptionId(
  alternativeId: number,
  criterionId: number
): number | null {
  const value = this.getAlternativeValue(
    alternativeId,
    criterionId
  );

  return value?.criterionOptionId ?? null;
    }

    getCriterionUnit(
      criterion: Criterion
    ): string {
      return criterion.unit ?? '';
    }

    getDirectionText(
      direction: Direction | null
    ): string {
      if (direction === Direction.Minimize) {
        return 'Lower values are preferred.';
      }

      if (direction === Direction.Maximize) {
        return 'Higher values are preferred.';
      }

      return '';
    }

    getValueKey(
      alternativeId: number,
      criterionId: number
    ): string {
      return `${alternativeId}-${criterionId}`;
    }

    
}