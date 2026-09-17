import {
  ChangeDetectorRef,
  Component,
  EventEmitter,
  Input,
  Output,
  inject
} from '@angular/core';

import {
  FormBuilder,
  ReactiveFormsModule,
  Validators
} from '@angular/forms';

import {
  Criterion
} from '../../../../../../core/models/criterion.model';

import {
  CriterionNumericalRule,
  NumericType,
  Direction
} from '../../../../../../core/models/criterion-numerical-rule.model';

import {
  IntervalRange
} from '../../../../../../core/models/interval-range.model';

import {
  CriterionNumericalRuleService
} from '../../../../../../core/services/criterion-numerical-rule.service';

import {
  IntervalRangeService
} from '../../../../../../core/services/interval-range.service';

@Component({
  selector: 'app-numerical-configuration',
  standalone: true,
  imports: [ReactiveFormsModule],
  templateUrl: './numerical-configuration.html',
  styleUrl: './numerical-configuration.css'
})
export class NumericalConfigurationComponent {
  private readonly numericalRuleService =
    inject(CriterionNumericalRuleService);

  private readonly intervalRangeService =
    inject(IntervalRangeService);

  private readonly formBuilder =
    inject(FormBuilder);

  private readonly changeDetector =
    inject(ChangeDetectorRef);

  @Input({ required: true })
  criterion!: Criterion;

  @Input()
  existingRule: CriterionNumericalRule | undefined;

  @Output()
  close = new EventEmitter<void>();

  @Output()
  ruleSaved = new EventEmitter<CriterionNumericalRule>();

  readonly NumericType = NumericType;
  readonly Direction = Direction;

  intervalRanges: IntervalRange[] = [];

  isSaving = false;
  errorMessage = '';

  readonly numericalRuleForm =
    this.formBuilder.group({
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

  ngOnInit(): void {
    this.loadConfiguration();
  }

  private loadConfiguration(): void {
    if (!this.existingRule) {
      this.resetForm();
      return;
    }

    this.numericalRuleForm.patchValue({
      numericType: this.existingRule.numericType,
      minValue: this.existingRule.minValue,
      maxValue: this.existingRule.maxValue,
      targetValue: this.existingRule.targetValue,
      direction: this.existingRule.direction
    });

    if (
      this.existingRule.numericType ===
      NumericType.Interval
    ) {
      this.loadIntervalRanges(
        this.existingRule.id
      );
    }
  }

  private resetForm(): void {
    this.numericalRuleForm.reset({
      numericType: NumericType.Scope,
      minValue: 0,
      maxValue: 100,
      targetValue: null,
      direction: Direction.Minimize
    });

    this.intervalRanges = [];
  }

  private loadIntervalRanges(
    ruleId: number
  ): void {
    this.intervalRangeService
      .getRangesByRule(ruleId)
      .subscribe({
        next: (ranges) => {
          this.intervalRanges = [...ranges].sort(
            (a, b) => a.rank - b.rank
          );

          this.changeDetector.markForCheck();
        },

        error: (error) => {
          console.error(
            'Failed to load interval ranges',
            error
          );

          this.errorMessage =
            'Unable to load interval ranges. Please try again.';

          this.changeDetector.markForCheck();
        }
      });
  }

  addIntervalRange(): void {
    const nextMin =
      this.intervalRanges.length > 0
        ? this.intervalRanges[
            this.intervalRanges.length - 1
          ].maxValue + 1
        : 0;

    this.intervalRanges = [
      ...this.intervalRanges,
      {
        id: 0,
        criterionNumericalRuleId:
          this.existingRule?.id ?? 0,
        minValue: nextMin,
        maxValue: nextMin + 10,
        rank: this.intervalRanges.length + 1
      }
    ];

    this.changeDetector.markForCheck();
  }

  updateIntervalMin(
    index: number,
    event: Event
  ): void {
    const value = Number(
      (event.target as HTMLInputElement).value
    );

    this.intervalRanges =
      this.intervalRanges.map(
        (range, i) =>
          i === index
            ? { ...range, minValue: value }
            : range
      );
  }

  updateIntervalMax(
    index: number,
    event: Event
  ): void {
    const value = Number(
      (event.target as HTMLInputElement).value
    );

    this.intervalRanges =
      this.intervalRanges.map(
        (range, i) =>
          i === index
            ? { ...range, maxValue: value }
            : range
      );
  }

  removeIntervalRange(
    index: number
  ): void {
    this.intervalRanges =
      this.intervalRanges
        .filter((_, i) => i !== index)
        .map((range, i) => ({
          ...range,
          rank: i + 1
        }));

    this.changeDetector.markForCheck();
  }

  save(): void {
    if (this.numericalRuleForm.invalid) {
      this.numericalRuleForm.markAllAsTouched();
      return;
    }

    const formValue =
      this.numericalRuleForm.getRawValue();

    const minValue = Number(formValue.minValue);
    const maxValue = Number(formValue.maxValue);

    if (minValue >= maxValue) {
      this.errorMessage =
        'Minimum value must be smaller than maximum value.';
      return;
    }

    const numericType = formValue.numericType;

    if (numericType === null) {
      this.errorMessage =
        'Please select a numerical evaluation type.';
      return;
    }

    let targetValue: number | null = null;
    let direction: Direction | null = null;

    if (numericType === NumericType.TargetValue) {
      targetValue =
        formValue.targetValue === null
          ? null
          : Number(formValue.targetValue);

      if (targetValue === null) {
        this.errorMessage =
          'Please enter a target value.';
        return;
      }

      if (
        targetValue < minValue ||
        targetValue > maxValue
      ) {
        this.errorMessage =
          'Target value must be between the minimum and maximum values.';
        return;
      }
    }

    if (numericType === NumericType.Scope) {
      direction = formValue.direction;

      if (direction === null) {
        this.errorMessage =
          'Please select a direction.';
        return;
      }
    }

    if (numericType === NumericType.Interval) {
      if (this.intervalRanges.length < 2) {
        this.errorMessage =
          'Please configure at least two interval ranges.';
        return;
      }

      for (const range of this.intervalRanges) {
        if (range.minValue >= range.maxValue) {
          this.errorMessage =
            'Each interval must have a minimum value smaller than its maximum value.';
          return;
        }
      }

      const sortedRanges = [
        ...this.intervalRanges
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
          return;
        }
      }
    }

    const request = {
      criterionId: this.criterion.id,
      numericType,
      minValue,
      maxValue,
      targetValue,
      direction
    };

    this.isSaving = true;
    this.errorMessage = '';

    if (this.existingRule) {
      this.updateRule(
        this.existingRule.id,
        request
      );
    } else {
      this.createRule(request);
    }
  }

  private createRule(
    request: {
      criterionId: number;
      numericType: NumericType;
      minValue: number;
      maxValue: number;
      targetValue: number | null;
      direction: Direction | null;
    }
  ): void {
    this.numericalRuleService
      .createRule(request)
      .subscribe({
        next: (rule) => {
          this.saveIntervalsIfNeeded(rule);
        },

        error: (error) => {
          console.error(
            'Failed to create numerical rule',
            error
          );

          this.errorMessage =
            'Unable to save the numerical configuration. Please try again.';

          this.isSaving = false;
          this.changeDetector.markForCheck();
        }
      });
  }

  private updateRule(
    ruleId: number,
    request: {
      criterionId: number;
      numericType: NumericType;
      minValue: number;
      maxValue: number;
      targetValue: number | null;
      direction: Direction | null;
    }
  ): void {
    this.numericalRuleService
      .updateRule(ruleId, request)
      .subscribe({
        next: () => {
          const updatedRule: CriterionNumericalRule = {
            id: ruleId,
            ...request
          };

          this.saveIntervalsIfNeeded(
            updatedRule
          );
        },

        error: (error) => {
          console.error(
            'Failed to update numerical rule',
            error
          );

          this.errorMessage =
            'Unable to save the numerical configuration. Please try again.';

          this.isSaving = false;
          this.changeDetector.markForCheck();
        }
      });
  }

  private saveIntervalsIfNeeded(
    rule: CriterionNumericalRule
  ): void {
    if (rule.numericType !== NumericType.Interval) {
      this.isSaving = false;
      this.ruleSaved.emit(rule);
      this.close.emit();
      this.changeDetector.markForCheck();
      return;
    }

    this.saveIntervalRanges(rule.id, rule);
  }

  private saveIntervalRanges(
    ruleId: number,
    rule: CriterionNumericalRule
  ): void {
    const currentRanges =
      this.intervalRanges;

    this.intervalRangeService
      .getRangesByRule(ruleId)
      .subscribe({
        next: (storedRanges) => {

          const currentIds = new Set(
            currentRanges
              .filter(range => range.id > 0)
              .map(range => range.id)
          );

          const rangesToDelete =
            storedRanges.filter(
              range => !currentIds.has(range.id)
            );

          this.deleteRemovedRanges(
            rangesToDelete,
            () =>
              this.createOrUpdateIntervalRanges(
                ruleId,
                rule
              )
          );
        },

        error: (error) => {
          console.error(
            'Failed to load existing interval ranges',
            error
          );

          this.errorMessage =
            'Unable to save the interval ranges. Please try again.';

          this.isSaving = false;
          this.changeDetector.markForCheck();
        }
      });
  }

  private deleteRemovedRanges(
    ranges: IntervalRange[],
    onComplete: () => void
  ): void {
    if (ranges.length === 0) {
      onComplete();
      return;
    }

    let remaining = ranges.length;
    let hasError = false;

    for (const range of ranges) {
      this.intervalRangeService
        .deleteRange(range.id)
        .subscribe({
          next: () => {
            remaining--;

            if (
              remaining === 0 &&
              !hasError
            ) {
              onComplete();
            }
          },

          error: (error) => {
            console.error(
              'Failed to delete interval range',
              error
            );

            if (!hasError) {
              hasError = true;

              this.errorMessage =
                'Unable to update the interval ranges. Please try again.';

              this.isSaving = false;
              this.changeDetector.markForCheck();
            }
          }
        });
    }
  }

  private createOrUpdateIntervalRanges(
    ruleId: number,
    rule: CriterionNumericalRule
  ): void {
    const existingRanges =
      this.intervalRanges.filter(
        range => range.id > 0
      );

    const newRanges =
      this.intervalRanges.filter(
        range => range.id === 0
      );

    if (newRanges.length > 0) {

      this.intervalRangeService
        .createRanges({
          criterionNumericalRuleId: ruleId,
          ranges: newRanges.map(range => ({
            minValue: range.minValue,
            maxValue: range.maxValue
          }))
        })
        .subscribe({
          next: (createdRanges) => {

            this.intervalRanges = [
              ...existingRanges,
              ...createdRanges
            ];

            this.updateExistingIntervalRanges(
              ruleId,
              rule
            );
          },

          error: (error) => {
            console.error(
              'Failed to create interval ranges',
              error
            );

            this.errorMessage =
              'Unable to save the new interval ranges. Please try again.';

            this.isSaving = false;
            this.changeDetector.markForCheck();
          }
        });

      return;
    }

    this.updateExistingIntervalRanges(
      ruleId,
      rule
    );
  }

  private updateExistingIntervalRanges(
    ruleId: number,
    rule: CriterionNumericalRule
  ): void {
    const ranges =
      this.intervalRanges.map(range => ({
        id: range.id,
        minValue: range.minValue,
        maxValue: range.maxValue
      }));

    this.intervalRangeService
      .updateRanges(ruleId, { ranges })
      .subscribe({
        next: () => {

          this.intervalRanges =
            this.intervalRanges.map(
              (range, index) => ({
                ...range,
                rank: index + 1
              })
            );

          this.isSaving = false;
          this.ruleSaved.emit(rule);
          this.close.emit();

          this.changeDetector.markForCheck();
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

  closeConfiguration(): void {
    this.close.emit();
  }
}