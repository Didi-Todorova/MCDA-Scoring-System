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
  Direction
} from '../../../../../core/models/criterion-numerical-rule.model';

import {
  CriterionOption
} from '../../../../../core/models/criterion-option.model';

import {
  CriterionOptionService
} from '../../../../../core/services/criterion-option.service';

import {
  NumericalConfigurationComponent
} from './numerical-configuration/numerical-configuration';

import {
  CategoricalConfigurationComponent
} from './categorical-configuration/categorical-configuration';

import {
  CriterionNumericalRuleService
} from '../../../../../core/services/criterion-numerical-rule.service';

import {
  IntervalRange
} from '../../../../../core/models/interval-range.model';

import {
  IntervalRangeService
} from '../../../../../core/services/interval-range.service';

@Component({
  selector: 'app-criteria',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    CategoricalConfigurationComponent,
    NumericalConfigurationComponent
  ],
  templateUrl: './criteria.html',
  styleUrl: './criteria.css'
})
export class CriteriaComponent implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly criterionService = inject(CriterionService);
  private readonly numericalRuleService =
    inject(CriterionNumericalRuleService);
  private readonly optionService =
    inject(CriterionOptionService);
  private readonly intervalRangeService =
    inject(IntervalRangeService);
  private readonly formBuilder = inject(FormBuilder);
  private readonly changeDetector =
    inject(ChangeDetectorRef);

  readonly CriterionType = CriterionType;
  readonly NumericType = NumericType;
  readonly Direction = Direction;

  criteria: Criterion[] = [];
  numericalRules: CriterionNumericalRule[] = [];
  categoricalOptions: CriterionOption[] = [];

  isLoading = true;
  isSaving = false;

  errorMessage = '';

  showForm = false;
  configuringCriterionId: number | null = null;

  criterionToDelete: Criterion | null = null;
  criterionToEdit: Criterion | null = null;

  readonly criterionForm =
    this.formBuilder.nonNullable.group({
      name: [
        '',
        [
          Validators.required,
          Validators.maxLength(200)
        ]
      ],

      criterionType: [
        CriterionType.Numerical,
        Validators.required
      ],

      unit: [
        '',
        Validators.maxLength(50)
      ],

      numericType: [
        NumericType.Scope,
        Validators.required
      ],

      minValue: [
        0,
        Validators.required
      ],

      maxValue: [
        100,
        Validators.required
      ],

      targetValue: [
        null as number | null
      ],

      direction: [
        Direction.Minimize as Direction | null
      ]
    });

  newIntervalRanges: IntervalRange[] = [];
  newCategoricalOptions: string[] = [];

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
          criterion =>
            criterion.decisionId === decisionId
        );

        this.loadNumericalRules();
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

  private loadNumericalRules(): void {
    this.numericalRuleService.getRules().subscribe({
      next: (rules) => {
        this.numericalRules = rules.filter(rule =>
          this.criteria.some(
            criterion =>
              criterion.id === rule.criterionId
          )
        );

        this.isLoading = false;
        this.changeDetector.markForCheck();
      },

      error: (error) => {
        console.error(
          'Failed to load numerical rules',
          error
        );

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
    this.criterionToEdit = null;

    this.criterionForm.reset({
      name: '',
      criterionType: CriterionType.Numerical,
      unit: '',
      numericType: NumericType.Scope,
      minValue: 0,
      maxValue: 100,
      targetValue: null,
      direction: Direction.Minimize
    });

    this.newIntervalRanges = [];
    this.newCategoricalOptions = [];

    this.errorMessage = '';
    this.showForm = true;

    this.changeDetector.markForCheck();
  }

  cancelForm(): void {
    if (this.isSaving) {
      return;
    }

    this.showForm = false;
    this.criterionToEdit = null;
    this.errorMessage = '';
  }

  onCriterionTypeChanged(): void {
    const type =
      this.criterionForm.controls.criterionType.value;

    if (type === CriterionType.Numerical) {
      this.newCategoricalOptions = [];
    } else {
      this.newIntervalRanges = [];
    }

    this.errorMessage = '';
    this.changeDetector.markForCheck();
  }

  onNumericTypeChanged(): void {
    this.errorMessage = '';

    if (
      this.criterionForm.controls.numericType.value !==
      NumericType.Interval
    ) {
      this.newIntervalRanges = [];
    }

    this.changeDetector.markForCheck();
  }

  addIntervalRange(): void {
    const last =
      this.newIntervalRanges[
        this.newIntervalRanges.length - 1
      ];

    const minValue = last
      ? last.maxValue + 1
      : Number(
          this.criterionForm.controls.minValue.value
        );

    this.newIntervalRanges = [
      ...this.newIntervalRanges,
      {
        id: 0,
        criterionNumericalRuleId: 0,
        minValue,
        maxValue: minValue + 10,
        rank: this.newIntervalRanges.length + 1
      }
    ];
  }

  updateNewIntervalMin(
    index: number,
    event: Event
  ): void {
    const value = Number(
      (event.target as HTMLInputElement).value
    );

    this.newIntervalRanges =
      this.newIntervalRanges.map(
        (range, i) =>
          i === index
            ? { ...range, minValue: value }
            : range
      );
  }

  updateNewIntervalMax(
    index: number,
    event: Event
  ): void {
    const value = Number(
      (event.target as HTMLInputElement).value
    );

    this.newIntervalRanges =
      this.newIntervalRanges.map(
        (range, i) =>
          i === index
            ? { ...range, maxValue: value }
            : range
      );
  }

  removeNewIntervalRange(index: number): void {
    this.newIntervalRanges =
      this.newIntervalRanges
        .filter((_, i) => i !== index)
        .map((range, i) => ({
          ...range,
          rank: i + 1
        }));
  }

  addCategoricalOption(): void {
    this.newCategoricalOptions = [
      ...this.newCategoricalOptions,
      ''
    ];
  }

  updateCategoricalOption(
    index: number,
    event: Event
  ): void {
    const value =
      (event.target as HTMLInputElement).value;

    this.newCategoricalOptions =
      this.newCategoricalOptions.map(
        (option, i) =>
          i === index ? value : option
      );
  }

  removeCategoricalOption(index: number): void {
    this.newCategoricalOptions =
      this.newCategoricalOptions.filter(
        (_, i) => i !== index
      );
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

    const formValue =
      this.criterionForm.getRawValue();

    const name = formValue.name.trim();

    if (!name) {
      this.errorMessage =
        'Please enter a criterion name.';
      return;
    }

    if (
      formValue.criterionType ===
      CriterionType.Numerical
    ) {
      if (!this.validateNumericalConfiguration()) {
        return;
      }
    } else {
      if (!this.validateCategoricalConfiguration()) {
        return;
      }
    }

    const unit =
      formValue.criterionType ===
      CriterionType.Numerical
        ? formValue.unit.trim() || null
        : null;

    this.isSaving = true;
    this.errorMessage = '';

    this.criterionService.createCriterion({
      decisionId,
      name,
      criterionType: formValue.criterionType,
      unit,
      weight: 0
    }).subscribe({
      next: (criterion) => {
        if (
          criterion.criterionType ===
          CriterionType.Numerical
        ) {
          this.createNumericalConfiguration(
            criterion
          );
        } else {
          this.createCategoricalConfiguration(
            criterion
          );
        }
      },

      error: (error) => {
        console.error(
          'Failed to create criterion',
          error
        );

        this.errorMessage =
          'Unable to create the criterion. Please try again.';

        this.isSaving = false;
        this.changeDetector.markForCheck();
      }
    });
  }

  private validateNumericalConfiguration(): boolean {
    const formValue =
      this.criterionForm.getRawValue();

    const minValue = Number(formValue.minValue);
    const maxValue = Number(formValue.maxValue);

    if (minValue >= maxValue) {
      this.errorMessage =
        'Minimum value must be smaller than maximum value.';
      return false;
    }

    if (
      formValue.numericType ===
      NumericType.TargetValue
    ) {
      if (formValue.targetValue === null) {
        this.errorMessage =
          'Please enter a target value.';
        return false;
      }

      const targetValue =
        Number(formValue.targetValue);

      if (
        targetValue < minValue ||
        targetValue > maxValue
      ) {
        this.errorMessage =
          'Target value must be between the minimum and maximum values.';
        return false;
      }
    }

    if (
      formValue.numericType ===
      NumericType.Scope
    ) {
      if (formValue.direction === null) {
        this.errorMessage =
          'Please select a direction.';
        return false;
      }
    }

    if (
      formValue.numericType ===
      NumericType.Interval
    ) {
      if (this.newIntervalRanges.length < 2) {
        this.errorMessage =
          'Please configure at least two interval ranges.';
        return false;
      }

      for (const range of this.newIntervalRanges) {
        if (range.minValue >= range.maxValue) {
          this.errorMessage =
            'Each interval must have a minimum value smaller than its maximum value.';
          return false;
        }
      }

      const sortedRanges = [
        ...this.newIntervalRanges
      ].sort(
        (a, b) => a.minValue - b.minValue
      );

      for (
        let i = 1;
        i < sortedRanges.length;
        i++
      ) {
        if (
          sortedRanges[i].minValue <=
          sortedRanges[i - 1].maxValue
        ) {
          this.errorMessage =
            'Interval ranges must not overlap.';
          return false;
        }
      }
    }

    return true;
  }

  private validateCategoricalConfiguration(): boolean {
    const options =
      this.newCategoricalOptions.map(
        option => option.trim()
      );

    if (options.length < 2) {
      this.errorMessage =
        'Please add at least two categorical options.';
      return false;
    }

    if (options.some(option => !option)) {
      this.errorMessage =
        'All categorical options must have a value.';
      return false;
    }

    const normalized =
      options.map(option =>
        option.toLowerCase()
      );

    if (
      new Set(normalized).size !==
      normalized.length
    ) {
      this.errorMessage =
        'Categorical options must be unique.';
      return false;
    }

    return true;
  }

  private createNumericalConfiguration(
    criterion: Criterion
  ): void {
    const formValue =
      this.criterionForm.getRawValue();

    this.numericalRuleService.createRule({
      criterionId: criterion.id,
      numericType: formValue.numericType,
      minValue: Number(formValue.minValue),
      maxValue: Number(formValue.maxValue),
      targetValue:
        formValue.numericType ===
        NumericType.TargetValue
          ? Number(formValue.targetValue)
          : null,
      direction:
        formValue.numericType ===
        NumericType.Scope
          ? formValue.direction
          : null
    }).subscribe({
      next: (rule) => {
        if (
          rule.numericType ===
          NumericType.Interval
        ) {
          this.createNewIntervalRanges(
            criterion,
            rule.id
          );
        } else {
          this.finishNewCriterion(
            criterion,
            rule
          );
        }
      },

      error: (error) => {
        console.error(
          'Failed to create numerical configuration',
          error
        );

        this.errorMessage =
          'The criterion was created, but its numerical configuration could not be saved. Please configure it using Configure.';

        this.criteria = [
          ...this.criteria,
          criterion
        ];

        this.isSaving = false;
        this.changeDetector.markForCheck();
      }
    });
  }

  private createNewIntervalRanges(
    criterion: Criterion,
    ruleId: number
  ): void {
    this.intervalRangeService.createRanges({
      criterionNumericalRuleId: ruleId,
      ranges: this.newIntervalRanges.map(
        range => ({
          minValue: range.minValue,
          maxValue: range.maxValue
        })
      )
    }).subscribe({
      next: (ranges) => {
        const rule =
          this.numericalRules.find(
            item => item.id === ruleId
          );

        if (rule) {
          this.numericalRules = [
            ...this.numericalRules,
            rule
          ];
        }

        this.finishNewCriterion(
          criterion,
          undefined,
          ranges
        );
      },

      error: (error) => {
        console.error(
          'Failed to create interval ranges',
          error
        );

        this.errorMessage =
          'The criterion was created, but its interval ranges could not be saved. Please configure it using Configure.';

        this.criteria = [
          ...this.criteria,
          criterion
        ];

        this.isSaving = false;
        this.changeDetector.markForCheck();
      }
    });
  }

  private createCategoricalConfiguration(
    criterion: Criterion
  ): void {
    this.optionService.createOptions({
      criterionId: criterion.id,
      options:
        this.newCategoricalOptions.map(
          option => option.trim()
        )
    }).subscribe({
      next: (options) => {
        this.categoricalOptions = [
          ...this.categoricalOptions,
          ...options
        ];

        this.finishNewCriterion(
          criterion,
          undefined,
          undefined,
          options
        );
      },

      error: (error) => {
        console.error(
          'Failed to create categorical options',
          error
        );

        this.errorMessage =
          'The criterion was created, but its options could not be saved. Please configure it using Configure.';

        this.criteria = [
          ...this.criteria,
          criterion
        ];

        this.isSaving = false;
        this.changeDetector.markForCheck();
      }
    });
  }

  private finishNewCriterion(
    criterion: Criterion,
    rule?: CriterionNumericalRule,
    ranges?: IntervalRange[],
    options?: CriterionOption[]
  ): void {
    this.criteria = [
      ...this.criteria,
      criterion
    ];

    if (rule) {
      this.numericalRules = [
        ...this.numericalRules,
        rule
      ];
    }

    if (ranges) {
      // Ranges are loaded again when Configure is opened.
    }

    if (options) {
      this.categoricalOptions = [
        ...this.categoricalOptions,
        ...options
      ];
    }

    this.isSaving = false;
    this.showForm = false;
    this.errorMessage = '';

    this.criterionForm.reset({
      name: '',
      criterionType: CriterionType.Numerical,
      unit: '',
      numericType: NumericType.Scope,
      minValue: 0,
      maxValue: 100,
      targetValue: null,
      direction: Direction.Minimize
    });

    this.newIntervalRanges = [];
    this.newCategoricalOptions = [];

    this.changeDetector.markForCheck();
  }

  startEditCriterion(
    criterion: Criterion
  ): void {
    this.criterionToEdit = criterion;

    this.criterionForm.reset({
      name: criterion.name,
      criterionType: criterion.criterionType,
      unit: criterion.unit ?? '',
      numericType:
        this.getNumericalRule(
          criterion.id
        )?.numericType ??
        NumericType.Scope,
      minValue:
        this.getNumericalRule(
          criterion.id
        )?.minValue ??
        0,
      maxValue:
        this.getNumericalRule(
          criterion.id
        )?.maxValue ??
        100,
      targetValue:
        this.getNumericalRule(
          criterion.id
        )?.targetValue ??
        null,
      direction:
        this.getNumericalRule(
          criterion.id
        )?.direction ??
        Direction.Minimize
    });

    this.errorMessage = '';
    this.showForm = true;
    this.changeDetector.markForCheck();
  }

  saveCriterionEdit(): void {
    if (!this.criterionToEdit) {
      return;
    }

    if (this.criterionForm.invalid) {
      this.criterionForm.markAllAsTouched();
      return;
    }

    const criterion =
      this.criterionToEdit;

    const formValue =
      this.criterionForm.getRawValue();

    this.isSaving = true;
    this.errorMessage = '';

    this.criterionService.updateCriterion(
      criterion.id,
      {
        decisionId: criterion.decisionId,
        name: formValue.name.trim(),
        criterionType:
          criterion.criterionType,
        unit:
          criterion.criterionType ===
          CriterionType.Numerical
            ? formValue.unit.trim() || null
            : null,
        weight: criterion.weight
      }
    ).subscribe({
      next: (updatedCriterion) => {
        this.criteria = this.criteria.map(
          item =>
            item.id === updatedCriterion.id
              ? updatedCriterion
              : item
        );

        this.isSaving = false;
        this.showForm = false;
        this.criterionToEdit = null;

        this.changeDetector.markForCheck();
      },

      error: (error) => {
        console.error(
          'Failed to update criterion',
          error
        );

        this.errorMessage =
          'Unable to update the criterion. Please try again.';

        this.isSaving = false;
        this.changeDetector.markForCheck();
      }
    });
  }

  confirmDeleteCriterion(
    criterion: Criterion
  ): void {
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

    const criterion =
      this.criterionToDelete;

    this.isSaving = true;
    this.errorMessage = '';

    this.criterionService.deleteCriterion(
      criterion.id
    ).subscribe({
      next: () => {
        this.criteria =
          this.criteria.filter(
            item => item.id !== criterion.id
          );

        this.numericalRules =
          this.numericalRules.filter(
            rule =>
              rule.criterionId !== criterion.id
          );

        this.categoricalOptions =
          this.categoricalOptions.filter(
            option =>
              option.criterionId !== criterion.id
          );

        if (
          this.configuringCriterionId ===
          criterion.id
        ) {
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

  configureCriterion(
    criterion: Criterion
  ): void {
    this.errorMessage = '';
    this.configuringCriterionId =
      criterion.id;

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
    const existingIndex =
      this.numericalRules.findIndex(
        item => item.id === rule.id
      );

    if (existingIndex >= 0) {
      this.numericalRules =
        this.numericalRules.map(
          item =>
            item.id === rule.id
              ? rule
              : item
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
      rule =>
        rule.criterionId === criterionId
    );
  }

  getCriterionTypeName(
    type: CriterionType
  ): string {
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
      .filter(
        option =>
          option.criterionId === criterionId
      )
      .sort(
        (a, b) => a.rank - b.rank
      );
  }

  getCategoricalOptionCount(
    criterionId: number
  ): number {
    return this.getCategoricalOptions(
      criterionId
    ).length;
  }
}