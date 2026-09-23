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
  Router,
  RouterLink
} from '@angular/router';

import { forkJoin, Observable, of } from 'rxjs';

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
  private readonly router = inject(Router);
  private readonly alternativeService = inject(AlternativeService);
  private readonly formBuilder = inject(FormBuilder);
  private readonly changeDetector = inject(ChangeDetectorRef);
  private readonly criterionService = inject(CriterionService);
  private readonly numericalRuleService =
    inject(CriterionNumericalRuleService);
  private readonly optionService =
    inject(CriterionOptionService);
  private readonly alternativeValueService =
    inject(AlternativeValueService);

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

  /**
   * null = creating a new alternative
   * number = editing an existing alternative
   */
  editingAlternativeId: number | null = null;

  /**
   * Local values being edited inside the form.
   * Nothing here is persisted until Save Alternative is clicked.
   */
  draftValues: Record<
    number,
    {
      numericValue: number | null;
      criterionOptionId: number | null;
    }
  > = {};

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


  // ---------------------------------------------------------
  // FORM
  // ---------------------------------------------------------

  openForm(): void {

    this.editingAlternativeId = null;

    this.alternativeForm.reset({
      name: ''
    });

    this.draftValues = {};
    this.errorMessage = '';
    this.showForm = true;

    this.changeDetector.markForCheck();
  }


  openEditForm(alternative: Alternative): void {

    this.editingAlternativeId = alternative.id;

    this.alternativeForm.reset({
      name: alternative.name
    });

    this.draftValues = {};

    for (const criterion of this.criteria) {

      const existingValue =
        this.getAlternativeValue(
          alternative.id,
          criterion.id
        );

      this.draftValues[criterion.id] = {
        numericValue:
          existingValue?.numericValue ?? null,

        criterionOptionId:
          existingValue?.criterionOptionId ?? null
      };
    }

    this.errorMessage = '';
    this.showForm = true;

    this.changeDetector.markForCheck();
  }


  cancelForm(): void {

    this.showForm = false;
    this.editingAlternativeId = null;
    this.draftValues = {};
    this.errorMessage = '';

    this.changeDetector.markForCheck();
  }


  // ---------------------------------------------------------
  // LOCAL FORM VALUE CHANGES
  // ---------------------------------------------------------

  updateNumericValue(
    criterionId: number,
    event: Event
  ): void {

    const input =
      event.target as HTMLInputElement;

    const rawValue = input.value.trim();

    this.draftValues[criterionId] = {
      numericValue:
        rawValue === ''
          ? null
          : Number(rawValue),

      criterionOptionId: null
    };

    this.changeDetector.markForCheck();
  }


  updateCategoricalValue(
    criterionId: number,
    event: Event
  ): void {

    const select =
      event.target as HTMLSelectElement;

    const rawValue = select.value;

    this.draftValues[criterionId] = {
      numericValue: null,

      criterionOptionId:
        rawValue === ''
          ? null
          : Number(rawValue)
    };

    this.changeDetector.markForCheck();
  }


  getDraftNumericValue(
    criterionId: number
  ): number | null {

    return this.draftValues[criterionId]?.numericValue ?? null;
  }


  getDraftCategoricalOptionId(
    criterionId: number
  ): number | null {

    return this.draftValues[criterionId]?.criterionOptionId ?? null;
  }


  // ---------------------------------------------------------
  // SAVE ALTERNATIVE
  // ---------------------------------------------------------

  saveAlternative(): void {

    if (this.alternativeForm.invalid) {

      this.alternativeForm.markAllAsTouched();
      this.errorMessage =
        'Please enter an alternative name.';

      return;
    }

    if (!this.decisionId) {

      this.errorMessage =
        'Invalid decision ID.';

      return;
    }

    const name =
      this.alternativeForm.controls.name.value.trim();

    if (!name) {

      this.alternativeForm.controls.name.setErrors({
        required: true
      });

      this.errorMessage =
        'Please enter an alternative name.';

      return;
    }

    const validationError =
      this.validateAlternativeValues();

    if (validationError) {

      this.errorMessage = validationError;
      return;
    }

    this.isSaving = true;
    this.errorMessage = '';

    if (this.editingAlternativeId === null) {

      this.createAlternativeWithValues(name);

    } else {

      this.updateAlternativeWithValues(
        this.editingAlternativeId,
        name
      );
    }
  }


  private createAlternativeWithValues(
    name: string
  ): void {

    this.alternativeService
      .createAlternative({
        decisionId: this.decisionId!,
        name
      })
      .subscribe({

        next: (alternative) => {

          this.alternatives = [
            ...this.alternatives,
            alternative
          ].sort((a, b) => a.id - b.id);

          this.saveValuesForAlternative(
            alternative
          );
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


  private updateAlternativeWithValues(
    alternativeId: number,
    name: string
  ): void {

    const alternative =
      this.alternatives.find(
        item => item.id === alternativeId
      );

    if (!alternative) {

      this.errorMessage =
        'Unable to find the alternative. Please try again.';

      this.isSaving = false;
      return;
    }

    const nameChanged =
      alternative.name !== name;

    const updateRequest = {
      decisionId: this.decisionId!,
      name,
    };

    const saveValues =
      () => this.saveValuesForAlternative(
        {
          ...alternative,
          name
        }
      );

    if (!nameChanged) {

      saveValues();
      return;
    }

    this.alternativeService
      .updateAlternative(
        alternativeId,
        updateRequest
      )
      .subscribe({

        next: () => {

          this.alternatives =
            this.alternatives.map(item =>
              item.id === alternativeId
                ? {
                    ...item,
                    name
                  }
                : item
            );

          saveValues();
        },

        error: (error) => {

          console.error(
            'Failed to update alternative',
            error
          );

          this.errorMessage =
            error?.error?.Error ??
            'Unable to update the alternative. Please try again.';

          this.isSaving = false;
          this.changeDetector.markForCheck();
        }
      });
  }


  private saveValuesForAlternative(
    alternative: Alternative
  ): void {

    const requests: Observable<
      AlternativeValue | void
    >[] = [];

    for (const criterion of this.criteria) {

      const draft =
        this.draftValues[criterion.id];

      if (!draft) {
        continue;
      }

      const existingValue =
        this.getAlternativeValue(
          alternative.id,
          criterion.id
        );

      const request = {
        alternativeId: alternative.id,
        criterionId: criterion.id,
        numericValue:
          criterion.criterionType === CriterionType.Numerical
            ? draft.numericValue
            : null,
        criterionOptionId:
          criterion.criterionType === CriterionType.Categorical
            ? draft.criterionOptionId
            : null
      };

      if (existingValue) {

        requests.push(
          this.alternativeValueService
            .updateAlternativeValue(
              existingValue.id,
              request
            )
        );

      } else {

        requests.push(
          this.alternativeValueService
            .createAlternativeValue(request)
        );
      }
    }

    if (requests.length === 0) {

      this.finishSavingAlternative();
      return;
    }

    forkJoin(requests).subscribe({

      next: (results) => {

        results.forEach((result, index) => {

          const criterion =
            this.criteria[index];

          const draft =
            this.draftValues[criterion.id];

          const existingValue =
            this.getAlternativeValue(
              alternative.id,
              criterion.id
            );

          if (result && typeof result === 'object') {

            this.alternativeValues = [
              ...this.alternativeValues.filter(
                value =>
                  value.id !== result.id
              ),
              result
            ];

            return;
          }

          if (existingValue && draft) {

            this.alternativeValues =
              this.alternativeValues.map(value =>
                value.id === existingValue.id
                  ? {
                      ...value,
                      numericValue:
                        draft.numericValue,
                      criterionOptionId:
                        draft.criterionOptionId
                    }
                  : value
              );
          }
        });

        this.finishSavingAlternative();
      },

      error: (error) => {

        console.error(
          'Failed to save alternative values',
          error
        );

        this.errorMessage =
          error?.error?.Error ??
          'Unable to save the alternative values. Please try again.';

        this.isSaving = false;
        this.changeDetector.markForCheck();
      }
    });
  }


  private finishSavingAlternative(): void {

    this.showForm = false;
    this.editingAlternativeId = null;
    this.draftValues = {};
    this.isSaving = false;
    this.errorMessage = '';

    this.changeDetector.markForCheck();
  }


  // ---------------------------------------------------------
  // VALIDATION
  // ---------------------------------------------------------

  private validateAlternativeValues(): string | null {

    if (this.criteria.length === 0) {

      return 'No criteria are configured for this decision.';
    }

    for (const criterion of this.criteria) {

      if (
        criterion.criterionType ===
        CriterionType.Numerical
      ) {

        const rule =
          this.getNumericalRule(criterion.id);

        if (!rule) {

          return `"${criterion.name}" is not configured correctly.`;
        }

        const value =
          this.draftValues[criterion.id]
            ?.numericValue;

        if (
          value === null ||
          value === undefined ||
          Number.isNaN(value)
        ) {

          return `Please enter a value for "${criterion.name}".`;
        }

        if (
          value < rule.minValue ||
          value > rule.maxValue
        ) {

          return `"${criterion.name}" must be between ${rule.minValue} and ${rule.maxValue}.`;
        }

      } else {

        const options =
          this.getCategoricalOptions(
            criterion.id
          );

        if (options.length === 0) {

          return `"${criterion.name}" has no configured options.`;
        }

        const optionId =
          this.draftValues[criterion.id]
            ?.criterionOptionId;

        if (
          optionId === null ||
          optionId === undefined
        ) {

          return `Please select an option for "${criterion.name}".`;
        }

        const validOption =
          options.some(
            option => option.id === optionId
          );

        if (!validOption) {

          return `The selected option for "${criterion.name}" is invalid.`;
        }
      }
    }

    return null;
  }


  // ---------------------------------------------------------
  // DELETE
  // ---------------------------------------------------------

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

    const alternative =
      this.alternativeToDelete;

    this.isSaving = true;
    this.errorMessage = '';

    this.alternativeService
      .deleteAlternative(alternative.id)
      .subscribe({

        next: () => {

          this.alternatives =
            this.alternatives.filter(
              item =>
                item.id !== alternative.id
            );

          this.alternativeValues =
            this.alternativeValues.filter(
              value =>
                value.alternativeId !== alternative.id
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


  // ---------------------------------------------------------
  // SAVE & CONTINUE
  // ---------------------------------------------------------

  saveAndContinue(): void {

    this.errorMessage = '';

    if (this.showForm) {

      this.errorMessage =
        'Please save or cancel the alternative you are currently editing before continuing.';

      return;
    }

    if (this.alternatives.length === 0) {

      this.errorMessage =
        'Please add at least one alternative before continuing.';

      return;
    }

    if (this.criteria.length === 0) {

      this.errorMessage =
        'No criteria are configured for this decision.';

      return;
    }

    const validationError =
      this.validateSavedAlternatives();

    if (validationError) {

      this.errorMessage = validationError;

      this.changeDetector.markForCheck();

      return;
    }

    this.router.navigate([
      '../preview'
    ], {
      relativeTo: this.route
    });
  }


  private validateSavedAlternatives(): string | null {

    for (const alternative of this.alternatives) {

      for (const criterion of this.criteria) {

        const value =
          this.getAlternativeValue(
            alternative.id,
            criterion.id
          );

        if (!value) {

          return `Please enter a value for "${criterion.name}" in "${alternative.name}" and save the alternative.`;
        }

        if (
          criterion.criterionType ===
          CriterionType.Numerical
        ) {

          const rule =
            this.getNumericalRule(
              criterion.id
            );

          if (!rule) {

            return `"${criterion.name}" is not configured correctly.`;
          }

          if (
            value.numericValue === null ||
            value.numericValue === undefined
          ) {

            return `Please enter a numerical value for "${criterion.name}" in "${alternative.name}".`;
          }

          if (
            value.numericValue < rule.minValue ||
            value.numericValue > rule.maxValue
          ) {

            return `"${criterion.name}" in "${alternative.name}" must be between ${rule.minValue} and ${rule.maxValue}.`;
          }

        } else {

          const options =
            this.getCategoricalOptions(
              criterion.id
            );

          if (options.length === 0) {

            return `"${criterion.name}" has no configured options.`;
          }

          if (
            value.criterionOptionId === null ||
            value.criterionOptionId === undefined
          ) {

            return `Please select an option for "${criterion.name}" in "${alternative.name}".`;
          }

          const validOption =
            options.some(
              option =>
                option.id ===
                value.criterionOptionId
            );

          if (!validOption) {

            return `The selected option for "${criterion.name}" in "${alternative.name}" is invalid.`;
          }
        }
      }
    }

    return null;
  }


  // ---------------------------------------------------------
  // HELPERS
  // ---------------------------------------------------------

  getNumericalRule(
    criterionId: number
  ): CriterionNumericalRule | undefined {

    return this.numericalRules.find(
      rule =>
        rule.criterionId === criterionId
    );
  }


  getCategoricalOptions(
    criterionId: number
  ): CriterionOption[] {

    return this.categoricalOptions
      .filter(
        option =>
          option.criterionId === criterionId
      )
      .sort(
        (a, b) =>
          a.rank - b.rank
      );
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

    const value =
      this.getAlternativeValue(
        alternativeId,
        criterionId
      );

    return value?.numericValue ?? null;
  }


  getCategoricalOptionId(
    alternativeId: number,
    criterionId: number
  ): number | null {

    const value =
      this.getAlternativeValue(
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
}