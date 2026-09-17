import {
  ChangeDetectorRef,
  Component,
  EventEmitter,
  Input,
  Output,
  inject
} from '@angular/core';

import {
  CriterionOption
} from '../../../../../../core/models/criterion-option.model';

import {
  CriterionOptionService
} from '../../../../../../core/services/criterion-option.service';

@Component({
  selector: 'app-categorical-configuration',
  standalone: true,
  templateUrl: './categorical-configuration.html',
  styleUrl: './categorical-configuration.css'
})
export class CategoricalConfigurationComponent {
  private readonly optionService = inject(CriterionOptionService);
  private readonly changeDetector = inject(ChangeDetectorRef);

  @Input({ required: true })
  criterionId!: number;

  @Input({ required: true })
  criterionName!: string;

  @Input()
  options: CriterionOption[] = [];

  @Output()
  close = new EventEmitter<void>();

  @Output()
  optionsChanged = new EventEmitter<CriterionOption[]>();

  isSaving = false;
  errorMessage = '';

  get sortedOptions(): CriterionOption[] {
    return [...this.options]
      .filter(option => option.criterionId === this.criterionId)
      .sort((a, b) => a.rank - b.rank);
  }

  addOption(): void {
    const nextRank = this.sortedOptions.length + 1;

    const newOption: CriterionOption = {
      id: 0,
      criterionId: this.criterionId,
      value: '',
      rank: nextRank
    };

    this.options = [
      ...this.options,
      newOption
    ];

    this.optionsChanged.emit(this.options);
    this.changeDetector.markForCheck();
  }

  updateOption(
    index: number,
    event: Event
  ): void {
    const value = (
      event.target as HTMLInputElement
    ).value;

    const option = this.sortedOptions[index];

    if (!option) {
      return;
    }

    this.options = this.options.map(item =>
      item === option
        ? {
            ...item,
            value
          }
        : item
    );

    this.optionsChanged.emit(this.options);
  }

  removeOption(index: number): void {
    const option = this.sortedOptions[index];

    if (!option) {
      return;
    }

    this.options = this.options
      .filter(item => item !== option)
      .map((item, i) => ({
        ...item,
        rank: item.criterionId === this.criterionId
          ? i + 1
          : item.rank
      }));

    this.optionsChanged.emit(this.options);
    this.changeDetector.markForCheck();
  }

  save(): void {
  const options = this.sortedOptions;

  if (options.length < 2) {
    this.errorMessage =
      'Please configure at least two options.';
    return;
  }

  const cleanedOptions = options.map(option => ({
    ...option,
    value: option.value.trim()
  }));

  if (cleanedOptions.some(option => !option.value)) {
    this.errorMessage =
      'All categorical options must have a value.';
    return;
  }

  const values = cleanedOptions.map(option =>
    option.value.toLowerCase()
  );

  if (new Set(values).size !== values.length) {
    this.errorMessage =
      'Categorical options must be unique.';
    return;
  }

  this.isSaving = true;
  this.errorMessage = '';

  const existingOptions =
    cleanedOptions.filter(option => option.id > 0);

  const newOptions =
    cleanedOptions.filter(option => option.id === 0);

  if (existingOptions.length === 0) {
    this.createOptions(cleanedOptions);
    return;
  }

  if (newOptions.length > 0) {
    this.errorMessage =
      'Please save the existing options before adding new ones.';
    this.isSaving = false;
    return;
  }

  this.optionService.updateOptions(
    this.criterionId,
    {
      options: cleanedOptions.map(option => ({
        id: option.id,
        value: option.value
      }))
    }
  ).subscribe({
    next: () => {
      this.options = cleanedOptions;

      this.optionsChanged.emit(this.options);

      this.isSaving = false;
      this.changeDetector.markForCheck();
    },

    error: (error) => {
      console.error(
        'Failed to update categorical options',
        error
      );

      this.errorMessage =
        'Unable to update categorical options. Please try again.';

      this.isSaving = false;
      this.changeDetector.markForCheck();
    }
  });
}

  private createOptions(
    options: CriterionOption[]
  ): void {
    this.optionService.createOptions({
      criterionId: this.criterionId,
      options: options.map(option => option.value)
    }).subscribe({
      next: (createdOptions) => {
        this.options = [
          ...this.options.filter(
            option =>
              option.criterionId !== this.criterionId
          ),
          ...createdOptions
        ];

        this.optionsChanged.emit(this.options);

        this.isSaving = false;
        this.changeDetector.markForCheck();
      },

      error: (error) => {
        console.error(
          'Failed to create categorical options',
          error
        );

        this.errorMessage =
          'Unable to save categorical options. Please try again.';

        this.isSaving = false;
        this.changeDetector.markForCheck();
      }
    });
  }

  closeConfiguration(): void {
    this.close.emit();
  }
}