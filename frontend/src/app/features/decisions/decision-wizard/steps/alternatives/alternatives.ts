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

@Component({
  selector: 'app-alternatives',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './alternatives.html',
  styleUrl: './alternatives.css'
})
export class AlternativesComponent implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly alternativeService =
    inject(AlternativeService);
  private readonly formBuilder =
    inject(FormBuilder);
  private readonly changeDetector =
    inject(ChangeDetectorRef);

  alternatives: Alternative[] = [];

  isLoading = true;
  isSaving = false;
  errorMessage = '';
  showForm = false;

  decisionId: number | null = null;

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

        this.isLoading = false;
        this.changeDetector.markForCheck();
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
}