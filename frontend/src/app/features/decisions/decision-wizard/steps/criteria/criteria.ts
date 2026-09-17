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

import { ActivatedRoute } from '@angular/router';

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
} from '../../../../../core/models/criterion-numerical-rule.model';

import {
  CriterionOption
} from '../../../../../core/models/criterion-option.model';

import {
  CriterionOptionService
} from '../../../../../core/services/criterion-option.service';

import {
  CategoricalConfigurationComponent
} from './categorical-configuration/categorical-configuration';

import {
  NumericalConfigurationComponent
} from './numerical-configuration/numerical-configuration';

import {
  CriterionNumericalRuleService
} from '../../../../../core/services/criterion-numerical-rule.service';

@Component({
  selector: 'app-criteria',
  standalone: true,
  imports: [ReactiveFormsModule,
    CategoricalConfigurationComponent,
    NumericalConfigurationComponent
  ],
  templateUrl: './criteria.html',
  styleUrl: './criteria.css'
})
export class CriteriaComponent implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly criterionService = inject(CriterionService);
  private readonly numericalRuleService = inject(CriterionNumericalRuleService);
  private readonly optionService = inject(CriterionOptionService);
  private readonly formBuilder = inject(FormBuilder);
  private readonly changeDetector = inject(ChangeDetectorRef);

  readonly CriterionType = CriterionType;
  readonly NumericType = NumericType;

  criteria: Criterion[] = [];
  numericalRules: CriterionNumericalRule[] = [];
  categoricalOptions: CriterionOption[] = [];

  isLoading = true;
  isSaving = false;

  errorMessage = '';

  showForm = false;
  configuringCriterionId: number | null = null;

  readonly criterionForm = this.formBuilder.nonNullable.group({
    name: ['', [Validators.required, Validators.maxLength(200)]],
    criterionType: [CriterionType.Numerical, Validators.required]
  });



  ngOnInit(): void {
    const decisionId = this.getDecisionId();

    if (!decisionId) {
      this.errorMessage = 'Invalid decision ID.';
      this.isLoading = false;
      this.changeDetector.markForCheck();
      return;
    }

    this.loadCriteria(decisionId);
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

  private loadCriteria(decisionId: number): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.criterionService.getCriteria().subscribe({
      next: (criteria) => {
        this.criteria = criteria.filter(
          criterion => criterion.decisionId === decisionId
        );

        this.loadNumericalRules();
        this.loadCategoricalOptions();
      },
      error: (error) => {
        console.error('Failed to load criteria', error);

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
            criterion => criterion.id === rule.criterionId
          )
        );

        this.isLoading = false;
        this.changeDetector.markForCheck();
      },
      error: (error) => {
        console.error('Failed to load numerical rules', error);

        this.errorMessage =
          'Unable to load criterion configuration. Please try again.';

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

      this.changeDetector.markForCheck();
    }
  });
}

  openForm(): void {
    this.criterionForm.reset({
      name: '',
      criterionType: CriterionType.Numerical
    });

    this.errorMessage = '';
    this.showForm = true;
  }

  cancelForm(): void {
    this.showForm = false;
  }

  addCriterion(): void {
    if (this.criterionForm.invalid) {
      this.criterionForm.markAllAsTouched();
      return;
    }

    const decisionId = this.getDecisionId();

    if (!decisionId) {
      this.errorMessage = 'Invalid decision ID.';
      return;
    }

    const formValue = this.criterionForm.getRawValue();

    this.isSaving = true;
    this.errorMessage = '';

    this.criterionService.createCriterion({
      decisionId,
      name: formValue.name.trim(),
      criterionType: formValue.criterionType,
      weight: 0
    }).subscribe({
      next: (criterion) => {
        this.criteria = [...this.criteria, criterion];

        this.isSaving = false;
        this.showForm = false;

        this.changeDetector.markForCheck();
      },
      error: (error) => {
        console.error('Failed to create criterion', error);

        this.errorMessage =
          'Unable to create the criterion. Please try again.';

        this.isSaving = false;
        this.changeDetector.markForCheck();
      }
    });
  }

  criterionToDelete: Criterion | null = null;

confirmDeleteCriterion(criterion: Criterion): void {
  this.criterionToDelete = criterion;
  this.errorMessage = '';
  this.changeDetector.markForCheck();
}

cancelDeleteCriterion(): void {
  this.criterionToDelete = null;
  this.changeDetector.markForCheck();
}

deleteCriterion(): void {
  if (!this.criterionToDelete) {
    return;
  }

  const criterion = this.criterionToDelete;

  this.isSaving = true;
  this.errorMessage = '';

  this.criterionService.deleteCriterion(criterion.id).subscribe({
    next: () => {
      this.criteria = this.criteria.filter(
        item => item.id !== criterion.id
      );

      this.numericalRules =
        this.numericalRules.filter(
          rule => rule.criterionId !== criterion.id
        );

      this.categoricalOptions =
        this.categoricalOptions.filter(
          option => option.criterionId !== criterion.id
        );

      if (this.configuringCriterionId === criterion.id) {
        this.configuringCriterionId = null;
      }

      this.criterionToDelete = null;
      this.isSaving = false;

      this.changeDetector.markForCheck();
    },

    error: (error) => {
      console.error(
        'Failed to delete criterion',
        error
      );

      this.errorMessage =
        error?.error?.Error ??
        'Unable to delete the criterion. Please try again.';

      this.isSaving = false;
      this.changeDetector.markForCheck();
    }
  });
}

    configureCriterion(criterion: Criterion): void {
    this.errorMessage = '';
    this.configuringCriterionId = criterion.id;
    this.changeDetector.markForCheck();
    }

    onCategoricalOptionsChanged(
  options: CriterionOption[]
): void {
  this.categoricalOptions = options;
  this.changeDetector.markForCheck();
}

onNumericalRuleSaved(
  rule: CriterionNumericalRule
): void {
  const existingIndex = this.numericalRules.findIndex(
    item => item.id === rule.id
  );

  if (existingIndex >= 0) {
    this.numericalRules = this.numericalRules.map(item =>
      item.id === rule.id ? rule : item
    );
  } else {
    this.numericalRules = [
      ...this.numericalRules,
      rule
    ];
  }

  this.changeDetector.markForCheck();
}

  closeConfiguration(): void {
    this.configuringCriterionId = null;
    this.errorMessage = '';
  }

  getNumericalRule(
    criterionId: number
  ): CriterionNumericalRule | undefined {
    return this.numericalRules.find(
      rule => rule.criterionId === criterionId
    );
  }

    getCriterionTypeName(type: CriterionType): string {
    return type === CriterionType.Numerical
      ? 'Numerical'
      : 'Categorical';
  }

  getNumericTypeName(
    numericType: NumericType
  ): string {
    switch (numericType) {
      case NumericType.Scope:
        return 'Scope';

      case NumericType.Interval:
        return 'Interval';

      case NumericType.TargetValue:
        return 'Target Value';

      default:
        return '';
    }
  }

  getCategoricalOptions(
  criterionId: number
): CriterionOption[] {
  return this.categoricalOptions
    .filter(option => option.criterionId === criterionId)
    .sort((a, b) => a.rank - b.rank);
}

getCategoricalOptionCount(
  criterionId: number
): number {
  return this.getCategoricalOptions(criterionId).length;
}

}