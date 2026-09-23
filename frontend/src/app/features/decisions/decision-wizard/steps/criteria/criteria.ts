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
  Router
} from '@angular/router';

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
    ReactiveFormsModule
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
    private readonly router = inject(Router);

  readonly CriterionType = CriterionType;
  readonly NumericType = NumericType;
  readonly Direction = Direction;

  criteria: Criterion[] = [];
  numericalRules: CriterionNumericalRule[] = [];
  categoricalOptions: CriterionOption[] = [];
  intervalRanges: IntervalRange[] = [];

  isLoading = true;
  isSaving = false;

  errorMessage = '';

  showForm = false;
  configuringCriterionId: number | null = null;
  draggedIntervalIndex: number | null = null;
  draggedCategoricalOptionIndex: number | null = null;

  criterionToDelete: Criterion | null = null;

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

  editingCriterion: Criterion | null = null;

  newIntervalRanges: IntervalRange[] = [];
  newCategoricalOptions: CriterionOption[] = [];

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
        this.loadIntervalRanges();
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

  private loadIntervalRanges(): void {
    this.intervalRangeService.getRanges().subscribe({
      next: (ranges) => {
        this.intervalRanges = ranges.filter(range =>
          this.numericalRules.some(
            rule =>
              rule.id === range.criterionNumericalRuleId
          )
        );

        this.isLoading = false;
        this.changeDetector.markForCheck();
      },

      error: (error) => {
        console.error(
          'Failed to load interval ranges',
          error
        );

        this.errorMessage =
          'Unable to load interval ranges. Please try again.';

        this.isLoading = false;
        this.changeDetector.markForCheck();
      }
    });
  }

  openForm(): void {
    this.editingCriterion = null;
    this.configuringCriterionId = null;

    this.resetForm();

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
    this.editingCriterion = null;
    this.errorMessage = '';
  }

  goToBasic(): void {
    this.router.navigate(
      ['../basic'],
      { relativeTo: this.route }
    );
  }

  configureCriterion(
    criterion: Criterion
  ): void {
    this.showForm = false;
    this.errorMessage = '';

    this.editingCriterion = criterion;
    this.configuringCriterionId = criterion.id;

    const rule = this.getNumericalRule(
      criterion.id
    );

    this.criterionForm.reset({
      name: criterion.name,
      criterionType: criterion.criterionType,
      unit: criterion.unit ?? '',
      numericType:
        rule?.numericType ??
        NumericType.Scope,
      minValue:
        rule?.minValue ??
        0,
      maxValue:
        rule?.maxValue ??
        100,
      targetValue:
        rule?.targetValue ??
        null,
      direction:
        rule?.direction ??
        Direction.Minimize
    });

    this.newIntervalRanges = rule
      ? this.getIntervalRanges(rule.id)
          .map(range => ({ ...range }))
      : [];

    this.newCategoricalOptions =
      this.getCategoricalOptions(
        criterion.id
      ).map(option => ({ ...option }));

    this.changeDetector.markForCheck();
  }

  closeConfiguration(): void {
    if (this.isSaving) {
      return;
    }

    this.configuringCriterionId = null;
    this.editingCriterion = null;
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
        criterionNumericalRuleId:
          this.getCurrentRuleId(),
        minValue,
        maxValue: minValue + 10,
        rank:
          this.newIntervalRanges.length + 1
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
  onIntervalDragStart(index: number): void {
  this.draggedIntervalIndex = index;
}

onIntervalDragOver(event: DragEvent): void {
  event.preventDefault();
}

onIntervalDrop(targetIndex: number): void {
  if (
    this.draggedIntervalIndex === null ||
    this.draggedIntervalIndex === targetIndex
  ) {
    return;
  }

  const ranges = [...this.newIntervalRanges];

  const [movedRange] = ranges.splice(
    this.draggedIntervalIndex,
    1
  );

  ranges.splice(targetIndex, 0, movedRange);

  this.newIntervalRanges = ranges;

  this.draggedIntervalIndex = null;
}

onIntervalDragEnd(): void {
  this.draggedIntervalIndex = null;
}

moveIntervalUp(index: number): void {
  if (index <= 0) {
    return;
  }

  const ranges = [...this.newIntervalRanges];

  [ranges[index - 1], ranges[index]] = [
    ranges[index],
    ranges[index - 1]
  ];

  this.newIntervalRanges = ranges;
}

moveIntervalDown(index: number): void {
  if (index >= this.newIntervalRanges.length - 1) {
    return;
  }

  const ranges = [...this.newIntervalRanges];

  [ranges[index], ranges[index + 1]] = [
    ranges[index + 1],
    ranges[index]
  ];

  this.newIntervalRanges = ranges;
}

  addCategoricalOption(): void {
    this.newCategoricalOptions = [
      ...this.newCategoricalOptions,
      {
        id: 0,
        criterionId: this.editingCriterion?.id ?? 0,
        value: '',
        rank: this.newCategoricalOptions.length + 1
      }
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
          i === index
            ? { ...option, value }
            : option
      );
  }

  removeCategoricalOption(index: number): void {
    this.newCategoricalOptions =
      this.newCategoricalOptions
        .filter((_, i) => i !== index)
        .map((option, i) => ({
          ...option,
          rank: i + 1
        }));
  }

  onCategoricalDragStart(index: number): void {
    this.draggedCategoricalOptionIndex = index;
  }

  onCategoricalDragOver(event: DragEvent): void {
    event.preventDefault();
  }

  onCategoricalDrop(targetIndex: number): void {
    if (
      this.draggedCategoricalOptionIndex === null ||
      this.draggedCategoricalOptionIndex === targetIndex
    ) {
      return;
    }

    const options = [
      ...this.newCategoricalOptions
    ];

    const [movedOption] = options.splice(
      this.draggedCategoricalOptionIndex,
      1
    );

    options.splice(
      targetIndex,
      0,
      movedOption
    );

    this.newCategoricalOptions = options.map(
      (option, index) => ({
        ...option,
        rank: index + 1
      })
    );

    this.draggedCategoricalOptionIndex = null;
  }

  onCategoricalDragEnd(): void {
    this.draggedCategoricalOptionIndex = null;
  }

  moveCategoricalOptionUp(index: number): void {
    if (index <= 0) {
      return;
    }

    const options = [
      ...this.newCategoricalOptions
    ];

    [options[index - 1], options[index]] = [
      options[index],
      options[index - 1]
    ];

    this.newCategoricalOptions = options.map(
      (option, i) => ({
        ...option,
        rank: i + 1
      })
    );
  }

  moveCategoricalOptionDown(index: number): void {
    if (
      index >=
      this.newCategoricalOptions.length - 1
    ) {
      return;
    }

    const options = [
      ...this.newCategoricalOptions
    ];

    [options[index], options[index + 1]] = [
      options[index + 1],
      options[index]
    ];

    this.newCategoricalOptions = options.map(
      (option, i) => ({
        ...option,
        rank: i + 1
      })
    );
  }

  addCriterion(): void {
    if (!this.validateForm()) {
      return;
    }

    const decisionId = this.getDecisionId();

    if (!decisionId) {
      this.errorMessage = 'Invalid decision ID.';
      return;
    }

    const formValue =
      this.criterionForm.getRawValue();

    this.isSaving = true;
    this.errorMessage = '';

    this.criterionService.createCriterion({
      decisionId,
      name: formValue.name.trim(),
      criterionType: formValue.criterionType,
      unit:
        formValue.criterionType ===
        CriterionType.Numerical
          ? formValue.unit.trim() || null
          : null,
      weight: 0
    }).subscribe({
      next: (criterion) => {
        this.createConfigurationForNewCriterion(
          criterion
        );
      },

      error: (error) => {
        console.error(
          'Failed to create criterion',
          error
        );

        this.errorMessage =
          this.getApiError(
            error,
            'Unable to create the criterion. Please try again.'
          );

        this.isSaving = false;
        this.changeDetector.markForCheck();
      }
    });
  }

  saveConfiguration(): void {
    if (!this.editingCriterion) {
      return;
    }

    if (!this.validateForm()) {
      return;
    }

    const criterion =
      this.editingCriterion;

    const formValue =
      this.criterionForm.getRawValue();

    this.isSaving = true;
    this.errorMessage = '';

    this.criterionService.updateCriterion(
      criterion.id,
      {
        decisionId: criterion.decisionId,
        name: formValue.name.trim(),
        criterionType: formValue.criterionType,
        unit:
          formValue.criterionType ===
          CriterionType.Numerical
            ? formValue.unit.trim() || null
            : null,
        weight: criterion.weight
      }
    ).subscribe({
      next: () => {
        const updatedCriterion: Criterion = {
          ...criterion,
          name: formValue.name.trim(),
          criterionType: formValue.criterionType,
          unit:
            formValue.criterionType ===
            CriterionType.Numerical
              ? formValue.unit.trim() || null
              : null
        };

        this.updateCriterionConfiguration(
          updatedCriterion
        );
      },

      error: (error) => {
        console.error(
          'Failed to update criterion',
          error
        );

        this.errorMessage =
          this.getApiError(
            error,
            'Unable to update the criterion. Please try again.'
          );

        this.isSaving = false;
        this.changeDetector.markForCheck();
      }
    });
  }

  private createConfigurationForNewCriterion(
    criterion: Criterion
  ): void {
    if (
      criterion.criterionType ===
      CriterionType.Numerical
    ) {
      this.createNumericalConfiguration(
        criterion
      );
      return;
    }

    this.createCategoricalConfiguration(
      criterion
    );
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
          this.intervalRangeService.createRanges({
            criterionNumericalRuleId: rule.id,
            ranges:
              this.newIntervalRanges.map(
                range => ({
                  minValue: range.minValue,
                  maxValue: range.maxValue
                })
              )
          }).subscribe({
            next: (ranges) => {
              this.numericalRules = [
                ...this.numericalRules,
                rule
              ];

              this.intervalRanges = [
                ...this.intervalRanges,
                ...ranges
              ];

              this.finishNewCriterion(
                criterion
              );
            },

            error: (error) => {
              console.error(
                'Failed to create interval ranges',
                error
              );

              this.errorMessage =
                'The criterion was created, but its interval ranges could not be saved. Please configure it again.';

              this.isSaving = false;
              this.changeDetector.markForCheck();
            }
          });

          return;
        }

        this.numericalRules = [
          ...this.numericalRules,
          rule
        ];

        this.finishNewCriterion(
          criterion
        );
      },

      error: (error) => {
        console.error(
          'Failed to create numerical configuration',
          error
        );

        this.errorMessage =
          'The criterion was created, but its numerical configuration could not be saved. Please configure it again.';

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
          option => option.value.trim()
        )
    }).subscribe({
      next: (options) => {
        this.categoricalOptions = [
          ...this.categoricalOptions,
          ...options
        ];

        this.finishNewCriterion(
          criterion
        );
      },

      error: (error) => {
        console.error(
          'Failed to create categorical options',
          error
        );

        this.errorMessage =
          'The criterion was created, but its options could not be saved. Please configure it again.';

        this.isSaving = false;
        this.changeDetector.markForCheck();
      }
    });
  }

  private updateCriterionConfiguration(
    criterion: Criterion
  ): void {
    const formValue =
      this.criterionForm.getRawValue();

    if (
      criterion.criterionType ===
      CriterionType.Numerical
    ) {
      this.updateNumericalConfiguration(
        criterion,
        formValue
      );

      return;
    }

    this.updateCategoricalConfiguration(
      criterion
    );
  }

  private updateNumericalConfiguration(
    criterion: Criterion,
    formValue: ReturnType<
      typeof this.criterionForm.getRawValue
    >
  ): void {
    const existingRule =
      this.getNumericalRule(
        criterion.id
      );

    const request = {
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
    };

    if (existingRule) {
      this.numericalRuleService.updateRule(
        existingRule.id,
        request
      ).subscribe({
        next: () => {
          const updatedRule: CriterionNumericalRule = {
            id: existingRule.id,
            ...request
          };

          this.numericalRules =
            this.numericalRules.map(
              rule =>
                rule.id === existingRule.id
                  ? updatedRule
                  : rule
            );

          this.saveIntervalConfiguration(
            updatedRule,
            criterion
          );
        },

        error: (error) => {
          console.error(
            'Failed to update numerical rule',
            error
          );

          this.errorMessage =
            this.getApiError(
              error,
              'Unable to update the numerical configuration. Please try again.'
            );

          this.isSaving = false;
          this.changeDetector.markForCheck();
        }
      });

      return;
    }

    this.numericalRuleService.createRule(
      request
    ).subscribe({
      next: (rule) => {
        this.numericalRules = [
          ...this.numericalRules,
          rule
        ];

        this.saveIntervalConfiguration(
          rule,
          criterion
        );
      },

      error: (error) => {
        console.error(
          'Failed to create numerical rule',
          error
        );

        this.errorMessage =
          this.getApiError(
            error,
            'Unable to save the numerical configuration. Please try again.'
          );

        this.isSaving = false;
        this.changeDetector.markForCheck();
      }
    });
  }

  private saveIntervalConfiguration(
    rule: CriterionNumericalRule,
    criterion: Criterion
  ): void {
    if (rule.numericType !== NumericType.Interval) {
      this.intervalRanges =
        this.intervalRanges.filter(
          range =>
            range.criterionNumericalRuleId !==
            rule.id
        );

      this.finishCriterionConfiguration(
        criterion
      );

      return;
    }

    this.intervalRangeService
      .getRangesByRule(rule.id)
      .subscribe({
        next: (storedRanges) => {
          const currentRanges =
            this.newIntervalRanges;

          const currentIds = new Set(
            currentRanges
              .filter(range => range.id > 0)
              .map(range => range.id)
          );

          const deletedRanges =
            storedRanges.filter(
              range => !currentIds.has(range.id)
            );

          this.deleteIntervalRanges(
            deletedRanges,
            () =>
              this.saveCurrentIntervalRanges(
                rule,
                criterion
              )
          );
        },

        error: (error) => {
          console.error(
            'Failed to load interval ranges',
            error
          );

          this.errorMessage =
            'Unable to update the interval ranges. Please try again.';

          this.isSaving = false;
          this.changeDetector.markForCheck();
        }
      });
  }

  private deleteIntervalRanges(
    ranges: IntervalRange[],
    onComplete: () => void
  ): void {
    if (ranges.length === 0) {
      onComplete();
      return;
    }

    let remaining = ranges.length;
    let failed = false;

    for (const range of ranges) {
      this.intervalRangeService
        .deleteRange(range.id)
        .subscribe({
          next: () => {
            remaining--;

            if (
              remaining === 0 &&
              !failed
            ) {
              onComplete();
            }
          },

          error: (error) => {
            console.error(
              'Failed to delete interval range',
              error
            );

            if (!failed) {
              failed = true;

              this.errorMessage =
                'Unable to update the interval ranges. Please try again.';

              this.isSaving = false;
              this.changeDetector.markForCheck();
            }
          }
        });
    }
  }

  private saveCurrentIntervalRanges(
    rule: CriterionNumericalRule,
    criterion: Criterion
  ): void {
    const allRanges = [...this.newIntervalRanges];

    const existingRanges =
      allRanges.filter(
        range => range.id > 0
      );

    const newRanges =
      allRanges.filter(
        range => range.id === 0
      );

    if (newRanges.length > 0) {
      this.intervalRangeService
        .createRanges({
          criterionNumericalRuleId: rule.id,
          ranges: newRanges.map(
            (range, index) => ({
              minValue: range.minValue,
              maxValue: range.maxValue,
              rank: index + 1
            })
          )
        })
        .subscribe({
          next: (createdRanges) => {
            this.intervalRanges =
              this.intervalRanges.filter(
                range =>
                  range.criterionNumericalRuleId !==
                  rule.id
              );

            this.intervalRanges = [
              ...this.intervalRanges,
              ...existingRanges,
              ...createdRanges
            ];

            this.finishCriterionConfiguration(
              criterion
            );
          },

          error: (error) => {
            console.error(
              'Failed to create interval ranges',
              error
            );

            this.errorMessage =
              'Unable to save the interval ranges. Please try again.';

            this.isSaving = false;
            this.changeDetector.markForCheck();
          }
        });

      return;
    }

    this.intervalRangeService.updateRanges(
      rule.id,
      {
         ranges: existingRanges.map(
          (range, index) => ({
            id: range.id,
            minValue: range.minValue,
            maxValue: range.maxValue,
            rank: index + 1
          })
        )
      }
    ).subscribe({
      next: () => {
        this.intervalRanges =
          this.intervalRanges.filter(
            range =>
              range.criterionNumericalRuleId !==
              rule.id
          );

        this.intervalRanges = [
          ...this.intervalRanges,
          ...existingRanges
        ];

        this.finishCriterionConfiguration(
          criterion
        );
      },

      error: (error) => {
        console.error(
          'Failed to update interval ranges',
          error
        );

        this.errorMessage =
          'Unable to update the interval ranges. Please try again.';

        this.isSaving = false;
        this.changeDetector.markForCheck();
      }
    });
  }

  private updateCategoricalConfiguration(
    criterion: Criterion
  ): void {
    const existingOptions =
      this.getCategoricalOptions(
        criterion.id
      );

    if (existingOptions.length === 0) {
      this.errorMessage =
        'This criterion has no categorical options configured.';
      this.isSaving = false;
      this.changeDetector.markForCheck();
      return;
    }

    const requestOptions =
      this.newCategoricalOptions
        .filter(option => option.id > 0)
        .map(option => ({
          id: option.id,
          value: option.value.trim()
        }));

    if (
      requestOptions.length !==
      existingOptions.length
    ) {
      this.errorMessage =
        'Please keep all existing categorical options when configuring this criterion.';
      this.isSaving = false;
      this.changeDetector.markForCheck();
      return;
    }

    this.optionService.updateOptions(
      criterion.id,
      {
        options: requestOptions
      }
    ).subscribe({
      next: () => {
        const updatedOptions =
          this.newCategoricalOptions.map(
            (option, index) => ({
              ...option,
              value: option.value.trim(),
              rank: index + 1
            })
          );

        this.categoricalOptions =
          this.categoricalOptions
            .filter(
              option =>
                option.criterionId !== criterion.id
            )
            .concat(updatedOptions);

        this.finishCriterionConfiguration(
          criterion
        );
      },

      error: (error) => {
        console.error(
          'Failed to update categorical options',
          error
        );

        this.errorMessage =
          this.getApiError(
            error,
            'Unable to update categorical options. Please try again.'
          );

        this.isSaving = false;
        this.changeDetector.markForCheck();
      }
    });
  }

  private finishNewCriterion(
    criterion: Criterion
  ): void {
    this.criteria = [
      ...this.criteria,
      criterion
    ];

    this.isSaving = false;
    this.showForm = false;

    this.resetForm();
    this.newIntervalRanges = [];
    this.newCategoricalOptions = [];

    this.errorMessage = '';

    this.changeDetector.markForCheck();
  }

  private finishCriterionConfiguration(
    criterion: Criterion
  ): void {
    this.criteria = this.criteria.map(
      item =>
        item.id === criterion.id
          ? criterion
          : item
    );

    this.isSaving = false;
    this.configuringCriterionId = null;
    this.editingCriterion = null;
    this.errorMessage = '';

    this.changeDetector.markForCheck();
  }

  private validateForm(): boolean {
    if (this.criterionForm.invalid) {
      this.criterionForm.markAllAsTouched();

      this.errorMessage =
        'Please complete the required fields.';

      return false;
    }

    const formValue =
      this.criterionForm.getRawValue();

    const name = formValue.name.trim();

    if (!name) {
      this.errorMessage =
        'Please enter a criterion name.';
      return false;
    }

    if (
      formValue.criterionType ===
      CriterionType.Numerical
    ) {
      return this.validateNumericalConfiguration();
    }

    return this.validateCategoricalConfiguration();
  }

  private validateNumericalConfiguration(): boolean {
    const formValue =
      this.criterionForm.getRawValue();

    const minValue = Number(formValue.minValue);
    const maxValue = Number(formValue.maxValue);

    if (
      !Number.isFinite(minValue) ||
      !Number.isFinite(maxValue)
    ) {
      this.errorMessage =
        'Minimum and maximum values must be valid numbers.';
      return false;
    }

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
        !Number.isFinite(targetValue) ||
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
        if (
          range.minValue >= range.maxValue
        ) {
          this.errorMessage =
            'Each interval must have a minimum value smaller than its maximum value.';
          return false;
        }

        if (
          range.minValue < minValue ||
          range.maxValue > maxValue
        ) {
          this.errorMessage =
            'Interval ranges must be inside the minimum and maximum values.';
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
        option => option.value.trim()
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

  private resetForm(): void {
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
  }

  private getCurrentRuleId(): number {
    if (!this.editingCriterion) {
      return 0;
    }

    return this.getNumericalRule(
      this.editingCriterion.id
    )?.id ?? 0;
  }

  getNumericalRule(
    criterionId: number
  ): CriterionNumericalRule | undefined {
    return this.numericalRules.find(
      rule =>
        rule.criterionId === criterionId
    );
  }

  getIntervalRanges(
    ruleId: number
  ): IntervalRange[] {
    return this.intervalRanges
      .filter(
        range =>
          range.criterionNumericalRuleId ===
          ruleId
      )
      .sort(
        (a, b) => a.rank - b.rank
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

        const ruleIds =
          this.numericalRules
            .filter(
              rule =>
                rule.criterionId ===
                criterion.id
            )
            .map(rule => rule.id);

        this.numericalRules =
          this.numericalRules.filter(
            rule =>
              rule.criterionId !==
              criterion.id
          );

        this.intervalRanges =
          this.intervalRanges.filter(
            range =>
              !ruleIds.includes(
                range.criterionNumericalRuleId
              )
          );

        this.categoricalOptions =
          this.categoricalOptions.filter(
            option =>
              option.criterionId !==
              criterion.id
          );

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
          this.getApiError(
            error,
            'Unable to delete the criterion. Please try again.'
          );

        this.isSaving = false;
        this.changeDetector.markForCheck();
      }
    });
  }

  private getApiError(
    error: any,
    fallback: string
  ): string {
    return (
      error?.error?.Error ??
      error?.error?.error ??
      fallback
    );
  }

  saveAndContinue(): void {
  if (this.isSaving) {
    return;
  }

  if (this.criteria.length === 0) {
    this.errorMessage =
      'Please add at least one criterion before continuing.';

    this.changeDetector.markForCheck();
    return;
  }

  const unconfiguredCriterion = this.criteria.find(criterion => {
    if (criterion.criterionType === CriterionType.Numerical) {
      return !this.getNumericalRule(criterion.id);
    }

    return this.getCategoricalOptionCount(criterion.id) < 2;
  });

  if (unconfiguredCriterion) {
    this.errorMessage =
      `Please configure the criterion "${unconfiguredCriterion.name}" before continuing.`;

    this.changeDetector.markForCheck();
    return;
  }

  this.errorMessage = '';

  this.router.navigate(
    ['../weighting'],
    { relativeTo: this.route }
  );
}
}